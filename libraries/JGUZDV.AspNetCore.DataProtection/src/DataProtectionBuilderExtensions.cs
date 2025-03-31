using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace JGUZDV.AspNetCore.DataProtection;

/// <summary>
/// Extension methods for configuring data protection.
/// </summary>
public static class DataProtectionBuilderExtensions
{
    /// <summary>
    /// Configures the data protection system with the specified configuration.
    /// </summary>
    public static IDataProtectionBuilder UseDataProtectionConfig(this IDataProtectionBuilder builder, IConfigurationSection configuration, IHostEnvironment environment)
    {
        var config = new JGUDataProtectionConfiguration();
        configuration.Bind(config);

        var validation = config.Validate(null);
        if (validation.Any())
            throw new ValidationException(validation.First(), null, null);

        builder.SetApplicationName(config.ApplicationDiscriminator ?? environment.ApplicationName);

        if(config.DisableAutomaticKeyGeneration)
            builder.DisableAutomaticKeyGeneration();

        PersistKeys(builder, config, environment);

        if (config.UseProtection)
            ProtectKeys(builder, config);

        return builder;
    }

    private static void PersistKeys(IDataProtectionBuilder builder, JGUDataProtectionConfiguration config, IHostEnvironment environment)
    {
        var fsPersistence = config.Persistence!.FileSystem!;
        var pathDiscriminator = fsPersistence.IsolatedPathDiscriminator ?? environment.ApplicationName;

        var directoryInfo = new DirectoryInfo(Path.Combine(fsPersistence.Path!, pathDiscriminator));

        builder.PersistKeysToFileSystem(directoryInfo);
    }

    private static bool ProtectKeys(IDataProtectionBuilder builder, JGUDataProtectionConfiguration config)
    {
        if (config.Protection!.UseCertificate)
        {

            if (!string.IsNullOrWhiteSpace(config.Protection.Certificate!.Thumbprint))
            {
                builder.ProtectKeysWithCertificate(config.Protection.Certificate.Thumbprint);
                return true;
            }

            var certFileName = config.Protection.Certificate.FileName;
            if (!string.IsNullOrWhiteSpace(certFileName))
            {
                var password = config.Protection.Certificate.Password;
                // accept KeyStoreFlags?
                var cert = new X509Certificate2(certFileName, password);
                builder.ProtectKeysWithCertificate(cert);
                return true;
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (config.Protection.UseDpapi)
            {
                // accept protectToLocalMachine?
                builder.ProtectKeysWithDpapi();
                return true;
            }

            if (config.Protection.UseDpapiNG)
            {
                // accept protectionDescriptorRule + DPapiNGProtectionDescriptorFlags?
                builder.ProtectKeysWithDpapiNG();
                return true;
            }
        }

        return false;
    }
}
