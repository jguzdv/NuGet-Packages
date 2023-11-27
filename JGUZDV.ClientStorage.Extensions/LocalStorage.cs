using Microsoft.JSInterop;

namespace JGUZDV.ClientStorage.Extensions
{
    /// Simple localStorage implementation
    public class LocalStorage : JGUZDV.ClientStorage.IKeyValueStorage
    {
        private readonly IJSRuntime _JSRuntime;

        /// <inheritdoc/>
        public LocalStorage(IJSRuntime jSRuntime)
        {
            _JSRuntime = jSRuntime;
        }


        /// <inheritdoc/>
        public Task SetItem<T>(string key, T value, DateTimeOffset? expiresAt = null)
        {

            var entry = new CacheEntry<T>
            {
                ExpiresAt = expiresAt,
                Value = value
            };

            var json = System.Text.Json.JsonSerializer.Serialize(entry);

            return _JSRuntime.InvokeVoidAsync("localStorage.setItem", key, json).AsTask();
        }

        /// <inheritdoc/>
        public async Task<T?> GetItem<T>(string key) where T : class
        {
            var value = await _JSRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (value == null)
                return null;

            try
            {
                var entry = System.Text.Json.JsonSerializer.Deserialize<CacheEntry<T>>(value);
                if (entry == null)
                    return null;

                if (entry.ExpiresAt.HasValue && DateTimeOffset.Now > entry.ExpiresAt)
                {
                    _ = RemoveItem(key);
                    return null;
                }

                return entry.Value;
            }
            catch
            {
                _ = RemoveItem(key);
                return null;
            }

        }

        /// <inheritdoc/>
        public async Task RemoveItem(string key)
        {
            await _JSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        /// <inheritdoc/>
        public async Task Clear()
        {
            await _JSRuntime.InvokeVoidAsync("localStorage.clear");
        }

        private class CacheEntry<T>
        {
            public required T Value { get; set; }
            public DateTimeOffset? ExpiresAt { get; set; }
        }

    }
}
