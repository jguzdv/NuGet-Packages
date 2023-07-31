using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration
{
    public static class Defaults
    {
        public static IReadOnlyDictionary<string, string> KnownConverters = new Dictionary<string, string>()
        {
            { "objectGuid", nameof(ByteGuidConverter) },
            { "objectSid", nameof(ByteSIDConverter) },
            { "serialNumber", nameof(StringConverter) },

            { "userPrincipalName", nameof(StringConverter) },
            { "sAMAccountName", nameof(StringConverter) },

            { "msds-tokenGroupNamesGlobalAndUniversal", nameof(StringConverter) },

            { "mail", nameof(LowerStringConverter) },
            { "homeDirectory", nameof(StringConverter) },

            { "sn", nameof(StringConverter) },
            { "givenName", nameof(StringConverter) },
            { "displayName", nameof(StringConverter) },

            { "eduPersonScopedAffiliation", nameof(StringConverter) }
        };

        public static IReadOnlyList<ClaimSource> KnownClaimSources = new List<ClaimSource>()
        {
            new ("sub", "objectGuid"),
            new ("upn", "userPrincipalName"),
            new ("uuid", "serialNumber"),
            new ("security_identifier", "objectSid"),
            new ("role", "tokenGroupNamesGlobalAndUniversal"),
            new ("name", "displayName"),
            new ("email", "mail"),
            new ("family_name", "sn"),
            new ("given_name", "givenName"),
            new ("home_directory", "homeDirectory"),
            new ("scoped_affiliation", "eduPersonScopedAffiliation"),
            new ("uid", "sAMAccountName")
        };
    }
}
