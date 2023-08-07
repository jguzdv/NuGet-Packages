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

        public static DirectoryEntry GetDirectoryEntry(string ldapServer, string adIdentity, IEnumerable<string> propertiesToLoad)
        {
            string? ldapPath;
            if (SidRegex().IsMatch(adIdentity))
                ldapPath = $"<SID={adIdentity}>";
            else if (GuidRegex().IsMatch(adIdentity))
                ldapPath = $"<GUID={adIdentity}>";
            else
                throw new InvalidOperationException($"Could not determine type of identifier '{adIdentity}'");

            var directoryEntry = new DirectoryEntry($"LDAP://{ldapServer}/{ldapPath}");
            directoryEntry.RefreshCache(propertiesToLoad.ToArray());

            return directoryEntry;
        }

        public static DirectoryEntry? FindUserDirectoryEntry(string ldapServer, string? ldapBasePath, string ldapFilter, IEnumerable<string> propertiesToLoad)
        {
            using var searcher = new DirectorySearcher(
                new DirectoryEntry($"LDAP://{ldapServer}{ldapBasePath}"),
                $"(&{ldapFilter}(objectCategory=user))", new[] { "objectGuid" });

            var searchResult = searcher.FindOne();
            if (searchResult == null)
                return null;

            var byteGuid = searchResult.Properties["objectGuid"].OfType<byte[]>().First();
            var ldapBindPath = 

            searchResult.Properties["objectGuid"];
            var directoryEntry = new DirectoryEntry($"LDAP://{ldapServer}<GUID={new Guid(byteGuid)}>");
            directoryEntry.RefreshCache(propertiesToLoad.ToArray());

            return directoryEntry;
        }
    }
}
