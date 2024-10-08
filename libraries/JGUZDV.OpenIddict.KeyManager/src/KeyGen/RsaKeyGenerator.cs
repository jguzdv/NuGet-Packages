﻿using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.IdentityModel.Tokens;

namespace JGUZDV.OpenIddict.KeyManager.KeyGen
{
    internal class RsaKeyGenerator : IKeyGenerator
    {
        public AsymmetricSecurityKey CreateKey(KeyUsage keyUsage, DateTimeOffset notBefore, DateTimeOffset notAfter)
        {
            using var rsa = System.Security.Cryptography.RSA.Create(2048);
            var rsaParameters = rsa.ExportParameters(includePrivateParameters: true);
            return new RsaSecurityKey(rsaParameters);
        }
    }
}
