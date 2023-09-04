using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

using OpenIddict.Server;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerConfiguration : IConfigureOptions<OpenIddictServerOptions>
{
    private readonly X509KeyStore _keyStore;
    private readonly ILogger<KeyManagerConfiguration> _logger;

    public KeyManagerConfiguration(
        X509KeyStore keyStore,
        ILogger<KeyManagerConfiguration> logger)
    {
        _keyStore = keyStore;
        _logger = logger;
    }

    public void Configure(OpenIddictServerOptions options)
    {
        foreach(var signingKey in _keyStore.GetCachedKeys(KeyUsage.Signature))
            AddSignatureKey(signingKey);

        foreach(var encryptionKey in _keyStore.GetCachedKeys(KeyUsage.Encryption))
            AddEncryptionKey(encryptionKey);


        void AddEncryptionKey(SecurityKey key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // If the encryption key is an asymmetric security key, ensure it has a private key.
            if (key is AsymmetricSecurityKey asymmetricSecurityKey &&
                asymmetricSecurityKey.PrivateKeyStatus is PrivateKeyStatus.DoesNotExist)
            {
                throw new InvalidOperationException("Private key is missing");
            }

            if (key.IsSupportedAlgorithm(SecurityAlgorithms.Aes256KW))
            {
                if (key.KeySize != 256)
                {
                    throw new InvalidOperationException("Key size may only be 256");
                }

                options.EncryptionCredentials.Add(new EncryptingCredentials(key,
                    SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512));
            }

            if (key.IsSupportedAlgorithm(SecurityAlgorithms.RsaOAEP))
            {
                options.EncryptionCredentials.Add(new EncryptingCredentials(key,
                    SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512));
            }

            throw new InvalidOperationException("Neither RsaOAEP nor Aes256KW ar supported by the encryption key");
        }

        void AddSignatureKey(SecurityKey key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // If the signing key is an asymmetric security key, ensure it has a private key.
            if (key is AsymmetricSecurityKey asymmetricSecurityKey &&
                asymmetricSecurityKey.PrivateKeyStatus is PrivateKeyStatus.DoesNotExist)
            {
                throw new InvalidOperationException("Private key is missing");
            }

            if (key.IsSupportedAlgorithm(SecurityAlgorithms.RsaSha256))
            {
                options.SigningCredentials.Add(new SigningCredentials(key, SecurityAlgorithms.RsaSha256));
            }

            if (key.IsSupportedAlgorithm(SecurityAlgorithms.HmacSha256))
            {
                options.SigningCredentials.Add(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            }


            throw new InvalidOperationException("Neither RsaSha256 nor HmacSha256 ar supported by the encryption key");
        }
    }
}

public class KeyConfigurationProvider : IConfigurationProvider
{
    public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public void Load()
    {
        throw new NotImplementedException();
    }

    public void Set(string key, string value)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string key, out string value)
    {
        throw new NotImplementedException();
    }
}
