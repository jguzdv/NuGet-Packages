using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace JGUZDV.AspNetCore.DataProtection;

public static class DataProtectionBuilderExtensions
{

    public static IDataProtectionBuilder UseDataProtectionConfig(this IDataProtectionBuilder builder, IConfigurationSection configuration, IWebHostEnvironment? environment)
    {
        var config = new Configuration();
        configuration.Bind(config);

        config.ApplicationName ??= environment?.ApplicationName;
        
        var validation = config.Validate(null);
        if (validation.Any())
            throw new ValidationException(validation.First(), null, null);

        if (config.SetApplicationName)
            builder.SetApplicationName(config.ApplicationName!);

        if(config.DisableAutomaticKeyGeneration)
            builder.DisableAutomaticKeyGeneration();

        if (!config.UsePersistence)
            return builder;

        PersistKeys(builder, config);

        if (config.UseProtection)
            ProtectKeys(builder, config);

        return builder;
    }

    private static void PersistKeys(IDataProtectionBuilder builder, Configuration config)
    {
        var directoryInfo = config.Persistence!.FileSystem!.UseIsolatedPath
            ? new DirectoryInfo(Path.Combine(config.Persistence.FileSystem.Path!, config.ApplicationName!))
            : new DirectoryInfo(config.Persistence.FileSystem.Path!);

        builder.PersistKeysToFileSystem(directoryInfo);
    }

    private static bool ProtectKeys(IDataProtectionBuilder builder, Configuration config)
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
