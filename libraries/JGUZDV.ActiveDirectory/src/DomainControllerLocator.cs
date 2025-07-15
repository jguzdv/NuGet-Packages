using System.DirectoryServices.ActiveDirectory;
using System.Runtime.Versioning;

namespace JGUZDV.ActiveDirectory;

/// <summary>
/// 
/// </summary>
[SupportedOSPlatform("windows")]
public static class DomainControllerLocator
{
    /// <summary>
    /// Returns a <see cref="DirectoryContext"/> for the current or requested domain.
    /// </summary>
    public static DirectoryContext GetDomainContext(string? domainName = null)
        => domainName is not null
            ? new DirectoryContext(DirectoryContextType.Domain, domainName)
            : new DirectoryContext(DirectoryContextType.Domain);

    /// <summary>
    /// Returns a collection of <see cref="DomainController"/> objects for the current or requested domain.
    /// </summary>
    public static DomainControllerCollection GetDomainControllers(string? domainName = null, string? siteName = null)
    {
        var directoryContext = GetDomainContext(domainName);
        return GetSiteDomainControllers(directoryContext, siteName);
    }

    /// <summary>
    /// Returns all <see cref="DomainController"/> objects for the current or requested site.
    /// </summary>
    public static DomainControllerCollection GetSiteDomainControllers(DirectoryContext context, string? siteName = null)
        => siteName is not null
            ? DomainController.FindAll(context, siteName)
            : DomainController.FindAll(context);

    /// <summary>
    /// Provides a random (or arbitrary but stable) <see cref="DomainController"/> from the current or requested domain.
    /// The Fallback will be the PDC Emulator if requested, otherwise it will be another Domain Controller from the same domain.
    /// Fallback will be null if the result is the same as the 'primary' result.
    /// </summary>
    public static (DomainController Result, DomainController? Fallback) GetDomainControllerAndFallback(
        string? domainName = null,
        string? siteName = null,
        bool pickRandom = true,
        bool fallbackToPDCEmulator = false)
    {
        var domainContext = GetDomainContext(domainName);
        var result = PickDomainController(domainContext, siteName, pickRandom);

        DomainController? fallback = fallbackToPDCEmulator 
            ? Domain.GetDomain(domainContext).PdcRoleOwner 
            : PickDomainController(domainContext, siteName, false);

        // Discard the fallback if it is the same as the result.
        if (fallback.Name.Equals(result.Name, StringComparison.OrdinalIgnoreCase))
        {
            fallback.Dispose();
            fallback = null;
        }

        return (result, fallback);
    }

    private static DomainController PickDomainController(DirectoryContext domainContext, string? siteName, bool pickRandom)
    {
        var locatorOptions = pickRandom ? LocatorOptions.ForceRediscovery : 0;
        return siteName is not null
            ? DomainController.FindOne(domainContext, siteName, locatorOptions)
            : DomainController.FindOne(domainContext, locatorOptions);
    }

    /// <summary>
    /// Returns the LDAP base path for the given <see cref="DomainController"/>,
    /// e.g. "ldap://dc1.example.com:636"
    /// </summary>
    public static string LdapBasePath(this DomainController domainController, int port = 636)
        => $"ldap://{domainController.Name}:{port:0}";
}
