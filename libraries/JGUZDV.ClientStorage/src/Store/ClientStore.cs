using JGUZDV.ClientStorage.Entries;

using Microsoft.Extensions.Caching.Memory;

namespace JGUZDV.ClientStorage.Store;

/// <summary>
/// Client storage to handle in memory cashing and local storage (i. e. 'SQLite' or 'localStorage' etc.)
/// </summary>
public class ClientStore : IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly IKeyValueStorage _storage;
    private readonly ILifeCycleEvents _lifeCycleEvents;

    private readonly Dictionary<string, CancellationTokenSource> _cancellationTokenSources = new();
    private readonly Dictionary<string, (TimeSpan ExiresIn, Func<CancellationToken, Task<object>> LoadFunc, bool UseBackgroundRefresh, Func<(object Value, DateTimeOffset CreatedAt, DateTimeOffset ExpiresIn), IStoreEntry> CreateEntry)> _jobInformation = new();
    private readonly Dictionary<string, Task> _loadingTasks = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cache">The in memory cache to be used</param>
    /// <param name="storage">The persistent storage</param>
    /// <param name="lifeCycleEvents">Events used to stop and resume background refreshs</param>
    public ClientStore(IMemoryCache cache, IKeyValueStorage storage, ILifeCycleEvents lifeCycleEvents)
    {
        _cache = cache;
        _storage = storage;
        _lifeCycleEvents = lifeCycleEvents;

        _lifeCycleEvents.Resumed += OnResumed;
        _lifeCycleEvents.Stopped += OnStopped;
    }

    /// <summary>
    /// Is triggered every time a value (entry) is loaded
    /// </summary>
    public event Action<LoadingContext>? StoreEntryLoaded;

    /// <summary>
    /// Is triggered every time a key is registered
    /// </summary>
    public event Action<StoreChangedEvent>? KeyRegistered;

    /// <summary>
    /// Is triggered every time a key is unregistered
    /// </summary>
    public event Action<StoreChangedEvent>? KeyUnregistered;

    /// <summary>
    /// Is triggered every time a value (entry) changes
    /// </summary>
    public event Action<StoreChangedEvent>? ValueChanged;

    private async Task InitKey<T>(string key)
    {
        var item = await _storage.GetItem<StoreEntry<T>>(key);

        if (item != null && item.ExpiresAt > DateTimeOffset.Now)
        {
            item = new StoreEntry<T>
            {
                CreatedAt = DateTimeOffset.Now,
                ExpiresAt = item.ExpiresAt,
                Value = item.Value
            };
            _cache.Set(key, item, item.ExpiresAt - DateTimeOffset.Now);
        }
        else
        {
            await RefreshEntry(key);
        }
    }

    /// <summary>
    /// Registers a new key using <see cref="StoreOptions{T}"/> to configure loading and caching
    /// </summary>
    /// <typeparam name="T">type of the value</typeparam>
    /// <param name="key">the key</param>
    /// <param name="storeOptions">options to configure loading and caching</param>
    public void Register<T>(string key, StoreOptions<T> storeOptions) where T : class
    {
        UnregisterKey(key);

        _jobInformation[key] = (storeOptions.ValueExpiry,
            async (ct) => await storeOptions.LoadFunc(ct),
            storeOptions.UsesBackgroundRefresh,
            ((object value, DateTimeOffset createdAt, DateTimeOffset expiredAt) input) => new StoreEntry<T>()
            {
                CreatedAt = input.createdAt,
                ExpiresAt = input.expiredAt,
                Value = (T)input.value
            });
        _ = InitKey<T>(key);

        if (storeOptions.UsesBackgroundRefresh)
            StartBackgroundRefresh(key, storeOptions.ValueExpiry);
        KeyRegistered?.Invoke(new StoreChangedEvent { Key = key });
    }

    private void UnregisterKey(string key)
    {
        _jobInformation.Remove(key);
        _cancellationTokenSources.GetValueOrDefault(key)?.Cancel();
        _cancellationTokenSources.Remove(key);
    }

    /// <summary>
    /// Unregisters a key
    /// </summary>
    /// <param name="key">the key to unregister</param>
    public void Unregister(string key)
    {
        KeyUnregistered?.Invoke(new StoreChangedEvent { Key = key });
        UnregisterKey(key);
    }

    /// <summary>
    /// Refreshes all values - potentially expensive operation - consider force loading a subset of entries
    /// </summary>
    /// <returns></returns>
    public async Task RefreshCache()
    {
        var tasks = new List<Task>();
        foreach (var item in _jobInformation)
        {
            tasks.Add(RefreshEntry(item.Key));
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// refreshs the entry for the given key
    /// </summary>
    /// <param name="key">the key</param>
    /// <returns></returns>
    public async Task RefreshEntry(string key)
    {
        if (_loadingTasks.ContainsKey(key))
        {
            await _loadingTasks[key];
            return;
        }

        var (expiresIn, loadFunc, _, _) = _jobInformation[key];

        async Task Refresh(string key, TimeSpan expiresIn, Func<CancellationToken, Task<object>> loadFunc)
        {
            var value = await loadFunc(CancellationToken.None);
            var item = _jobInformation[key].CreateEntry((value, DateTimeOffset.Now, DateTimeOffset.Now.AddTicks(expiresIn.Ticks)));

            _cache.Set(key, item, expiresIn);
            _ = _storage.SetItem(key, item);
            ValueChanged?.Invoke(new StoreChangedEvent { Key = key });
        }

        var task = Refresh(key, expiresIn, loadFunc);
        _loadingTasks[key] = task;
        await task;
        _loadingTasks.Remove(key);
    }

    /// <summary>
    /// Get the entry for a given key
    /// </summary>
    /// <param name="key">the key</param>
    /// <param name="forceLoad">enforce using the load function</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<StoreEntryResult<T>> GetEntry<T>(string key, bool forceLoad = false)
    {
        if (!_jobInformation.ContainsKey(key))
        {
            throw new InvalidOperationException($"storage does not contains key: {key}");
        }

        var item = _cache.Get<StoreEntry<T>>(key);

        if (!forceLoad && item != null)
        {
            var context = new LoadingContext { Key = key, LoadedFromCache = true };
            StoreEntryLoaded?.Invoke(context);
            return new() { Context = context, Entry = item };
        }

        try
        {
            //try refreshing cache
            await RefreshEntry(key);
            item = _cache.Get<StoreEntry<T>>(key);
            if (item is null)
                throw new InvalidOperationException("tried to access cache after failed refresh");

            var context = new LoadingContext { Key = key, LoadedHot = true };
            StoreEntryLoaded?.Invoke(context);
            return new() { Context = context, Entry = item };
        }
        catch (Exception e)
        {
            item = _cache.Get<StoreEntry<T>>(key);
            if (forceLoad && item != null)
            {
                var ctx = new LoadingContext { Key = key, LoadedFromCache = true, Exception = e };
                StoreEntryLoaded?.Invoke(ctx);
                return new() { Context = ctx, Entry = item };
            }

            //load persisted data
            item = (await _storage.GetItem<StoreEntry<T>>(key)) ?? throw new InvalidOperationException("Could not fall back to persisted storage");

            var context = new LoadingContext { Key = key, LoadedFromStorage = true, Exception = e };
            StoreEntryLoaded?.Invoke(context);
            return new() { Context = context, Entry = item };
        }
    }

    /// <summary>
    /// Get the entry if key is already registered. Otherwise registers the key - using <see cref="StoreOptions{T}"/> to configure loading and caching - and then gets the entry.
    /// </summary>
    /// <typeparam name="T">type of the value</typeparam>
    /// <param name="key">the key</param>
    /// <param name="storeOptions">options to configure loading and caching</param>
    /// <returns>the store entry result <see cref="StoreEntryResult{T}"/></returns>
    public Task<StoreEntryResult<T>> GetOrLoadEntry<T>(string key, StoreOptions<T> storeOptions) where T : class
    {
        if (!_jobInformation.ContainsKey(key))
            Register(key, storeOptions);

        return GetEntry<T>(key);
    }

    /// <summary>
    /// Get the value if key is already registered. Otherwise registers the key - using <see cref="StoreOptions{T}"/> to configure loading and caching - and then gets the value.
    /// </summary>
    /// <typeparam name="T">type of the value</typeparam>
    /// <param name="key">the key</param>
    /// <param name="storeOptions">options to configure loading and caching</param>
    /// <returns>the value</returns>
    public async Task<T> GetOrLoad<T>(string key, StoreOptions<T> storeOptions) where T : class
    {
        var entry = await GetOrLoadEntry<T>(key, storeOptions);
        return (T)entry.Entry.Value;
    }

    /// <summary>
    /// Get the value
    /// </summary>
    /// <typeparam name="T">type of the value</typeparam>
    /// <param name="key">the key</param>
    /// <param name="forceLoad">enforce using the load function</param>
    /// <returns>the value</returns>
    public async Task<T> Get<T>(string key, bool forceLoad = false) where T : class
    {
        var entry = await GetEntry<T>(key, forceLoad);
        return (T)entry.Entry.Value;
    }

    private void HandleBackgroundRefresh(string key, TimeSpan startIn, CancellationToken ct)
    {
        var jobInfo = _jobInformation.GetValueOrDefault(key);

        if (ct.IsCancellationRequested == true)
            return;

        var loadFunc = jobInfo.LoadFunc;
        var expiresIn = jobInfo.ExiresIn;

        _ = Task.Delay((int)(startIn.TotalMilliseconds * 0.95), ct)
            .ContinueWith(_ => loadFunc(ct).ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully || ct.IsCancellationRequested)
                    return;

                var entry = jobInfo.CreateEntry((t.Result, DateTimeOffset.Now, DateTimeOffset.Now.AddTicks(expiresIn.Ticks)));

                _cache.Set(key, entry, expiresIn);
                _ = _storage.SetItem(key, entry);
                ValueChanged?.Invoke(new StoreChangedEvent { Key = key });

                HandleBackgroundRefresh(key, expiresIn, ct);
            }), TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    private void StartBackgroundRefresh(string key, TimeSpan startIn)
    {
        var cts = new CancellationTokenSource();
        _cancellationTokenSources[key] = cts;

        HandleBackgroundRefresh(key, startIn, cts.Token);
    }

    private void OnStopped(object? sender, EventArgs args)
    {
        foreach (var source in _cancellationTokenSources)
        {
            source.Value.Cancel();
        }
    }

    private void OnResumed(object? sender, EventArgs args)
    {
        foreach (var source in _cancellationTokenSources)
        {
            source.Value.Cancel();
        }

        foreach (var job in _jobInformation)
        {
            if (!job.Value.UseBackgroundRefresh)
                continue;

            var entry = _cache.Get<IStoreEntry>(job.Key);

            if (entry == null || DateTime.Now > entry.ExpiresAt) // if expired
            {
                StartBackgroundRefresh(job.Key, TimeSpan.FromSeconds(0));
                continue;
            }

            var expiresIn = entry.ExpiresAt - DateTimeOffset.Now;
            StartBackgroundRefresh(job.Key, expiresIn);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        _lifeCycleEvents.Resumed -= OnResumed;
        _lifeCycleEvents.Stopped -= OnStopped;

        foreach (var source in _cancellationTokenSources)
        {
            source.Value.Cancel();
        }
    }
}
