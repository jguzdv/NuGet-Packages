using JGUZDV.ClientStorage.Store;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace JGUZDV.ClientStorage.Tests
{
    public class Tests
    {
        private class KeyValueStorage : IKeyValueStorage
        {
            private readonly Dictionary<string, object?> _dict = new();

            public Task Clear()
            {
                _dict.Clear();
                return Task.CompletedTask;
            }

            public Task<T?> GetItem<T>(string key) where T : class
            {
                return Task.FromResult(_dict.GetValueOrDefault(key) as T);
            }

            public Task RemoveItem(string key)
            {
                _dict.Remove(key);
                return Task.CompletedTask;
            }

            public Task SetItem<T>(string key, T value, DateTimeOffset? expiresAt = null)
            {
                _dict[key] = value;
                return Task.CompletedTask;
            }
        }

        private class LifeCycleEvents : ILifeCycleEvents
        {
            public event EventHandler? Stopped;
            public event EventHandler? Resumed;

            public void TriggerStopped(object s, EventArgs a) => Stopped?.Invoke(s, a);
            public void TriggerResumed(object s, EventArgs a) => Resumed?.Invoke(s, a);
        }

        [Fact]
        public async Task TestLoad()
        {
            IOptions<MemoryCacheOptions> cacheOptions = Options.Create(new MemoryCacheOptions());
            IMemoryCache cache = new MemoryCache(cacheOptions);
            var storage = new KeyValueStorage();
            var events = new LifeCycleEvents();
            var store = new ClientStore(cache, storage, events);

            store.Register("testKey", new StoreOptions<string>
            {
                LoadFunc = (ct) => { return Task.FromResult("testValue"); },
                UsesBackgroundRefresh = false,
                ValueExpiry = TimeSpan.FromMilliseconds(1)
            });

            var value = await store.Get<string>("testKey");
            Assert.Equal("testValue", value);


            await Task.Delay(1);
            var entry = await store.GetEntry<string>("testKey");

            Assert.True(entry.Context.LoadedHot);
        }

        [Fact]
        public async Task TestFallback()
        {
            IOptions<MemoryCacheOptions> cacheOptions = Options.Create(new MemoryCacheOptions());
            IMemoryCache cache = new MemoryCache(cacheOptions);
            var storage = new KeyValueStorage();
            var events = new LifeCycleEvents();
            var store = new ClientStore(cache, storage, events);

            var isOnline = true;
            store.Register("testKey", new StoreOptions<string>
            {
                LoadFunc = (ct) =>
                {
                    if (isOnline)
                        return Task.FromResult("testValue");
                    throw new InvalidOperationException();
                },
                UsesBackgroundRefresh = false,
                ValueExpiry = TimeSpan.FromMilliseconds(1)
            });

            await Task.Delay(1);
            isOnline = false;
            var entry = await store.GetEntry<string>("testKey");

            Assert.False(entry.Context.LoadedFromCache);
            Assert.True(entry.Context.LoadedFromStorage);
        }


        [Fact]
        public async Task TestStopResume()
        {
            IOptions<MemoryCacheOptions> cacheOptions = Options.Create(new MemoryCacheOptions());
            IMemoryCache cache = new MemoryCache(cacheOptions);
            var storage = new KeyValueStorage();
            var events = new LifeCycleEvents();
            var store = new ClientStore(cache, storage, events);

            var count = 0;
            store.Register("testKey", new StoreOptions<string>
            {
                LoadFunc = (ct) => { count++; return Task.FromResult("testValue"); },
                UsesBackgroundRefresh = true,
                ValueExpiry = TimeSpan.FromMilliseconds(5)
            });

            Assert.Equal(1, count);
            events.TriggerStopped(this, new());

            await Task.Delay(25);
            Assert.Equal(1, count);

            events.TriggerResumed(this, new());
            await store.Get<string>("testKey");
            Assert.Equal(2, count);
        }
    }
}