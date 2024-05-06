using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.Claims
{
    /// <summary>
    /// Provides properties as simple claim representation.
    /// </summary>
    public interface IClaimProvider
    {
        /// <summary>
        /// Gets the claims for the given directory entry.
        /// </summary>
        IEnumerable<(string Type, string Value)> GetClaims(DirectoryEntry directoryEntry, params string[] claimTypes);

        /// <summary>
        /// Filters the requested claim types to those available from this provider
        /// </summary>
        IEnumerable<string> GetProvidedClaimTypes(params string[] claimTypes);
    }
}