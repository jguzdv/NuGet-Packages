using System.DirectoryServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace JGUZDV.ActiveDirectory.ClaimProvider
{
    [SupportedOSPlatform("Windows")]
    internal partial class ADHelper
    {
        [GeneratedRegex("^S(-\\d+)+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, 200, "de-DE")]
        private static partial Regex SidRegex();

        [GeneratedRegex("^[0-9a-f]{8}(?:-[0-9a-f]{4}){3}-[0-9a-f]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, 200, "de-DE")]
        private static partial Regex GuidRegex();


        private static string CreateLdapPath(string? ldapServer, string? pathOrBind)
        {
            var hasServer = !string.IsNullOrWhiteSpace(ldapServer);

            if(!hasServer && string.IsNullOrWhiteSpace(pathOrBind))
                throw new ArgumentException("Both parameters (ldapServer, pathOrBind) are null or empty");

            if(!hasServer)
                return $"LDAP://{pathOrBind}";

            return $"LDAP://{ldapServer}/{pathOrBind}";
        }

        public static (bool isBindable, string? BindPath) IsBindableIdentity(string adIdentity)
        {
            if (SidRegex().IsMatch(adIdentity))
                return (true, $"<SID={adIdentity}>");
            else if (GuidRegex().IsMatch(adIdentity))
                return (true, $"<GUID={adIdentity}>");
            else
                return (false, null);
        }

        public static DirectoryEntry BindDirectoryEntry(string? ldapServer, string bindPath, IEnumerable<string> propertiesToLoad)
        {
            var directoryEntry = new DirectoryEntry(CreateLdapPath(ldapServer, bindPath));
            directoryEntry.RefreshCache(propertiesToLoad.ToArray());

            return directoryEntry;
        }

        public static DirectoryEntry? FindUserDirectoryEntry(string? ldapServer, string? ldapBasePath, string ldapFilter, IEnumerable<string> propertiesToLoad)
        {
            using var searcher = new DirectorySearcher(
                new DirectoryEntry(CreateLdapPath(ldapServer, ldapBasePath)),
                $"(&{ldapFilter}(objectCategory=user))", new[] { "objectGuid" });

            var searchResult = searcher.FindOne();
            if (searchResult == null)
                return null;

            var byteGuid = searchResult.Properties["objectGuid"].OfType<byte[]>().First();
            
            var directoryEntry = new DirectoryEntry(CreateLdapPath(ldapServer, $"<GUID={new Guid(byteGuid)}>"));
            directoryEntry.RefreshCache(propertiesToLoad.ToArray());

            return directoryEntry;
        }
    }
}
