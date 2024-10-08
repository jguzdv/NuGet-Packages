﻿using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using OpenIddict.Server;

namespace JGUZDV.OpenIddict.KeyManager;

public class OpenIdDictServerConfiguration : IConfigureOptions<OpenIddictServerOptions>
{
    private readonly TimeProvider _timeProvider;
    private readonly KeyContainer _keyContainer;
    private readonly ILogger<OpenIdDictServerConfiguration> _logger;

    public OpenIdDictServerConfiguration(
        TimeProvider timeProvider,
        KeyContainer keyContainer,
        ILogger<OpenIdDictServerConfiguration> logger)
    {
        _timeProvider = timeProvider;
        _keyContainer = keyContainer;
        _logger = logger;
    }

    public void Configure(OpenIddictServerOptions options)
    {
        var utcNow = _timeProvider.GetUtcNow();

        foreach(var signingKey in _keyContainer.SignatureKeys.Where(x => x.NotBefore <= utcNow && x.NotAfter >= utcNow))
            AddSignatureKey(signingKey.SecurityKey);
        

        foreach(var encryptionKey in _keyContainer.EncryptionKeys.Where(x => x.NotBefore <= utcNow && x.NotAfter >= utcNow))
            AddEncryptionKey(encryptionKey.SecurityKey);


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
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.RsaOAEP))
            {
                options.EncryptionCredentials.Add(new EncryptingCredentials(key,
                    SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512));
            }
            else
            {
                throw new InvalidOperationException("Neither RsaOAEP nor Aes256KW ar supported by the encryption key");
            }
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
            else if (key.IsSupportedAlgorithm(SecurityAlgorithms.HmacSha256))
            {
                options.SigningCredentials.Add(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            }
            else
            {
                throw new InvalidOperationException("Neither RsaSha256 nor HmacSha256 ar supported by the encryption key");
            }
        }
    }
}
