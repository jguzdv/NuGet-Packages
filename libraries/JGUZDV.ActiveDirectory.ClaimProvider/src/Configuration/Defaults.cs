using JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration
{
    public static class Defaults
    {
        public static Dictionary<string, string> KnownConverters = new()
        {
            { "objectGuid", nameof(ByteGuidConverter) },
            { "objectSid", nameof(ByteSIDConverter) },
            { "serialNumber", nameof(StringConverter) },
            
            { "userPrincipalName", nameof(StringConverter) },
            { "sAMAccountName", nameof(StringConverter) },

            { "tokenGroupNamesGlobalAndUniversal", nameof(StringConverter) },
            
            { "mail", nameof(LowerStringConverter) },
            { "homeDirectory", nameof(StringConverter) },

            { "sn", nameof(StringConverter) },
            { "givenName", nameof(StringConverter) },
            { "displayName", nameof(StringConverter) },
            
            { "eduPersonScopedAffiliation", nameof(StringConverter) },
            { "zdvStudentID", nameof(StringConverter) }
        };
    }
}
