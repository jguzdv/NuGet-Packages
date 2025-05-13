namespace JGUZDV.ActiveDirectory.Configuration;

/// <summary>
/// Defaults for the library.
/// </summary>
public static class Defaults
{
    /// <summary>
    /// A list of known properties in Active Directory.
    /// </summary>
    public static IReadOnlyList<ADPropertyInfo> KnownProperties = new List<ADPropertyInfo>()
    {
        new("objectGuid", typeof(byte[])),
        new("objectSid", typeof(byte[])),
        new("thumbnailPhoto", typeof(byte[])),

        new("whenChanged", typeof(DateTime)),
        new("whenCreated", typeof(DateTime)),

        new("accountExpires", typeof(long)),
        new("badPasswordTime", typeof(long)),
        new("lastLogoff", typeof(long)),
        new("lastLogon", typeof(long)),
        new("lastLogonTimestamp", typeof(long)),
        new("lockoutTime", typeof(long)),
        new("pwdLastSet", typeof(long)),
        new("badPwdCount", typeof(int)),
        new("countryCode", typeof(int)),
        new("logonCount", typeof(int)),
        new("primaryGroupID", typeof(int)),
        new("sAMAccountType", typeof(int)),
        new("uidNumber", typeof(int)),
        new("userAccountControl", typeof(int)),

        new("cn", typeof(string)),
        new("co", typeof(string)),
        new("company", typeof(string)),
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
        new("mail", typeof(string)),
        new("mailNickname", typeof(string)),
        new("managedObjects", typeof(string)),
        new("memberOf", typeof(string)),
        new("name", typeof(string)),
        new("objectCategory", typeof(string)),
        new("objectClass", typeof(string)),
        new("postalCode", typeof(string)),
        new("protocolSettings", typeof(string)),
        new("proxyAddresses", typeof(string)),
        new("sAMAccountName", typeof(string)),
        new("serialNumber", typeof(string)),
        new("sn", typeof(string)),
        new("streetAddress", typeof(string)),
        new("telephoneNumber", typeof(string)),
        new("uid", typeof(string)),
        new("userPrincipalName", typeof(string)),

        new("tokenGroupsGlobalAndUniversal", typeof(byte[])),
        new("msds-tokenGroupNamesGlobalAndUniversal", typeof(string))
    };

    /// <summary>
    /// A list of known claim sources in Active Directory.
    /// </summary>
    public static IReadOnlyList<ClaimSource> KnownClaimSources = new List<ClaimSource>()
        {
            new ("affiliation", "eduPersonAffiliation"),
            new ("department","department"),
            new ("home_directory", "homeDirectory"),
            new ("email", "mail", Casing: Casing.Lower),
            new ("family_name", "sn"),
            new ("given_name", "givenName"),
            new ("name", "displayName"),
            new ("security_identifier", "objectSid", OutputFormats.ByteArrays.SDDL),
            new ("scoped_affiliation", "eduPersonScopedAffiliation"),
            new ("uid", "sAMAccountName")
            new ("upn", "userPrincipalName"),
        };
}
