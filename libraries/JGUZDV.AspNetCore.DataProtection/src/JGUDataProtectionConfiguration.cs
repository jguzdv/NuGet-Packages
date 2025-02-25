using System.ComponentModel.DataAnnotations;

namespace JGUZDV.AspNetCore.DataProtection;

internal class JGUDataProtectionConfiguration : IValidatableObject
{
    public string? ApplicationDiscriminator { get; set; }
    
    public bool DisableAutomaticKeyGeneration { get; set; }

    public bool UsePersistence { get; set; } = true;
    public Persistence? Persistence { get; set; }

    public bool UseProtection { get; set; } = true;
    public Protection? Protection { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext? validationContext)
    {
        if (string.IsNullOrWhiteSpace(ApplicationDiscriminator))
            yield return new ValidationResult("ApplicationName must be set", new[] { nameof(ApplicationDiscriminator) });

        if (UsePersistence && Persistence == null)
            yield return new ValidationResult("Persistence must be set", new[] { nameof(UsePersistence), nameof(Persistence) });

        if (UsePersistence && Persistence?.UseFileSystem == true)
        {
            if (string.IsNullOrWhiteSpace(Persistence?.FileSystem?.Path))
                yield return new ValidationResult("Persistence:FileSystem:Path must be set", new[] { nameof(Persistence.UseFileSystem), nameof(Persistence.FileSystem) });
        }

        if (UseProtection && Protection == null)
            yield return new ValidationResult("Protection must be set", new[] { nameof(UseProtection), nameof(Protection) });

        if (UseProtection && Protection?.UseCertificate == true)
        {
            if (Protection.Certificate == null)
                yield return new ValidationResult("Protection:Certificate must be set", new[] { nameof(Protection.Certificate) });
            else if (string.IsNullOrWhiteSpace(Protection.Certificate.Thumbprint) && string.IsNullOrWhiteSpace(Protection.Certificate.FileName))
                yield return new ValidationResult("Protection:Certificate:Filename or Protection:Certificate:Thumbprint must be set", new[] { nameof(Protection.Certificate.FileName), nameof(Protection.Certificate.Thumbprint) });
        }
    }
}

internal class Persistence
{
    public bool UseFileSystem { get; set; }
    public FileSystemPersistence? FileSystem { get; set; }
}


internal class FileSystemPersistence
{
    public string? Path { get; set; }

    public string? IsolatedPathDiscriminator { get; set; }
}



internal class Protection
{
    public bool UseCertificate { get; set; }
    public Certificate? Certificate { get; set; }

    public bool UseDpapi { get; set; }
    public bool UseDpapiNG { get; set; }
}

internal class Certificate
{
    public string? Thumbprint { get; set; }
    public string? FileName { get; set; }
    public string? Password { get; set; }
}