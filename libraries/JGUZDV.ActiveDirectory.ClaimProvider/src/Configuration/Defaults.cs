using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration
{
    public static class Defaults
    {
        public static IReadOnlyList<ADPropertyInfo> KnownProperties = new List<ADPropertyInfo>()
        {
            new("objectGuid", typeof(byte), "Base64"),
            new("objectSid", typeof(byte), "SID"),

            new("msds-tokenGroupNamesGlobalAndUniversal", typeof(string)),

            new("accountExpires", typeof(string)),
            new("badPasswordTime", typeof(string)),
            new("lastLogoff", typeof(string)),
            new("lastLogon", typeof(string)),
            new("lastLogonTimestamp", typeof(string)),
            new("lockoutTime", typeof(string)),
            new("pwdLastSet", typeof(string)),
            new("whenChanged", typeof(string)),
            new("whenCreated", typeof(string)),

            new("badPwdCount", typeof(int)),
            new("logonCount", typeof(int)),

            new("cn", typeof(string)),
            new("co", typeof(string)),
            new("company", typeof(string)),
            new("countryCode", typeof(string)),
            new("department", typeof(string)),
            new("displayName", typeof(string)),
            new("distinguishedName", typeof(string)),
            new("eduPersonAffiliation", typeof(string)),
            new("eduPersonEntitlement", typeof(string)),
            new("eduPersonPrincipalName", typeof(string)),
            new("eduPersonScopedAffiliation", typeof(string)),
            new("eduPersonTargetedID", typeof(string)),
            new("employeeType", typeof(string)),
            new("givenName", typeof(string)),
            new("homeDirectory", typeof(string)),
            new("homeDrive", typeof(string)),
            new("l", typeof(string)),
            new("mail", typeof(string), "lower"),
            new("mailNickname", typeof(string)),
            new("managedObjects", typeof(string)),
            new("memberOf", typeof(string)),
            new("name", typeof(string)),
            new("objectCategory", typeof(string)),
            new("objectClass", typeof(string)),
            new("postalCode", typeof(string)),
            new("primaryGroupID", typeof(string)),
            new("protocolSettings", typeof(string)),
            new("proxyAddresses", typeof(string)),
            new("sAMAccountName", typeof(string)),
            new("sAMAccountType", typeof(string)),
            new("serialNumber", typeof(string)),
            new("sn", typeof(string)),
            new("streetAddress", typeof(string)),
            new("telephoneNumber", typeof(string)),
            new("thumbnailPhoto", typeof(string)),
            new("uid", typeof(string)),
            new("uidNumber", typeof(string)),
            new("userAccountControl", typeof(string)),
            new("userPrincipalName", typeof(string))
        };

        public static IReadOnlyList<ClaimSource> KnownClaimSources = new List<ClaimSource>()
        {
            new ("sub", "objectGuid"),
            new ("upn", "userPrincipalName"),
            new ("security_identifier", "objectSid"),
            new ("role", "msds-tokenGroupNamesGlobalAndUniversal"),
            new ("name", "displayName"),
            new ("email", "mail"),
            new ("family_name", "sn"),
            new ("given_name", "givenName"),
            new ("home_directory", "homeDirectory"),
            new ("scoped_affiliation", "eduPersonScopedAffiliation"),
            new ("affiliation", "eduPersonAffiliation"),
            new ("uid", "sAMAccountName")
        };
    }
}
