using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager
{
    public interface IKeyStore
    {
        /// <summary>
        /// Loads all keys of the given key usage.
        /// </summary>
        Task<List<KeyInfo>> LoadKeysAsync(KeyUsage keyUsage, CancellationToken ct);

        /// <summary>
        /// Persists the given key to storage.
        /// </summary>
        Task SaveKeyAsync(KeyInfo keyInfo, CancellationToken ct);

        /// <summary>
        /// Purges all expired keys of the given key usage, that are expired before the given reference date.
        /// </summary>
        Task PurgeExpiredKeys(KeyUsage keyUsage, DateTimeOffset refDate, CancellationToken ct);
    }
}