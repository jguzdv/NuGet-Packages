using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager
{
    public interface IKeyStore
    {
        Task<List<KeyInfo>> LoadKeysAsync(CancellationToken ct = default);
        Task SaveKeyAsync(KeyInfo keyInfo, CancellationToken ct);
        Task PurgeExpiredKeys(DateTimeOffset refDate, CancellationToken ct);
    }
}