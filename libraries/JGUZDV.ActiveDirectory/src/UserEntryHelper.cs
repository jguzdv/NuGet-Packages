using System.DirectoryServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace JGUZDV.ActiveDirectory;

/// <summary>
/// Provides helper methods for retrieving DirectoryEntry objects.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class UserEntryHelper
{
    [GeneratedRegex("^S(-\\d+)+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, 200, "de-DE")]
    private static partial Regex SidRegex();

    [GeneratedRegex("^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, 200, "de-DE")]
    private static partial Regex GuidRegex();


    private static string CreateLdapPath(string? ldapServer, string? pathOrBind)
    {
        var hasServer = !string.IsNullOrWhiteSpace(ldapServer);

        if (!hasServer && string.IsNullOrWhiteSpace(pathOrBind))
            throw new ArgumentException("Both parameters (ldapServer, pathOrBind) are null or empty");

        if (!hasServer)
            return $"LDAP://{pathOrBind}";

        return $"LDAP://{ldapServer}/{pathOrBind}";
    }

    /// <summary>
    /// Checks if the given adIdentity is direct bindable and returns the bind path if it is.
    /// </summary>
    public static (bool isBindable, string? BindPath) IsBindableIdentity(string adIdentity)
    {
        if (SidRegex().IsMatch(adIdentity))
        {
            return (true, $"<SID={adIdentity}>");
        }
        else if (GuidRegex().IsMatch(adIdentity))
        {
            return (true, $"<GUID={adIdentity}>");
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>
    /// Given a sid or guid, returns a DirectoryEntry that is direct bound to the object.
    /// </summary>
    public static DirectoryEntry BindDirectoryEntry(string? ldapServer, string bindPath, IEnumerable<string> propertiesToLoad)
    {
        var directoryEntry = new DirectoryEntry(CreateLdapPath(ldapServer, bindPath));
        directoryEntry.RefreshCache(propertiesToLoad.ToArray());

        return directoryEntry;
    }

    /// <summary>
    /// Direct binds the DirectoryEntry from the objectGuid.
    /// </summary>
    public static DirectoryEntry BindDirectoryEntry(string? ldapServer, Guid objectGuid, IEnumerable<string> propertiesToLoad)
    {
        return BindDirectoryEntry(ldapServer, $"<GUID={objectGuid}>", propertiesToLoad);
    }

    /// <summary>
    /// Direct binds the DirectoryEntry from the objectSid
    /// </summary>
    public static DirectoryEntry BindDirectoryEntry(string? ldapServer, SecurityIdentifier objectSid, IEnumerable<string> propertiesToLoad)
    {
        return BindDirectoryEntry(ldapServer, $"<SID={objectSid}>", propertiesToLoad);
    }

    /// <summary>
    /// Searches the user on ldapServer in ldapBasePath with ldapFilter given and then returns an DirectoryEntry using the guid to direct bind the entry.
    /// </summary>
    public static DirectoryEntry? FindUserDirectoryEntry(string? ldapServer, string? ldapBasePath, string ldapFilter, IEnumerable<string> propertiesToLoad)
    {
        using var searcher = new DirectorySearcher(
            new DirectoryEntry(CreateLdapPath(ldapServer, ldapBasePath)),
            $"(&{ldapFilter}(objectCategory=user))", ["objectGuid"]);

        var searchResult = searcher.FindOne();
        if (searchResult == null)
            return null;

        var byteGuid = searchResult.Properties["objectGuid"].OfType<byte[]>().First();

        var directoryEntry = new DirectoryEntry(CreateLdapPath(ldapServer, $"<GUID={new Guid(byteGuid)}>"));
        directoryEntry.RefreshCache(propertiesToLoad.ToArray());

        return directoryEntry;
    }
}
