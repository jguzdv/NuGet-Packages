namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration
{
    public static class Defaults
    {
        public static IReadOnlyList<ClaimSource> KnownClaimSources = new List<ClaimSource>()
        {
            new ("sub", "objectGuid", "Guid"),
            new ("upn", "userPrincipalName"),
            new ("security_identifier", "objectSid", "SDDL"),
            new ("role", "msds-tokenGroupNamesGlobalAndUniversal"),
            new ("name", "displayName"),
            new ("email", "mail", "lower"),
            new ("family_name", "sn"),
            new ("given_name", "givenName"),
            new ("home_directory", "homeDirectory"),
            new ("scoped_affiliation", "eduPersonScopedAffiliation"),
            new ("affiliation", "eduPersonAffiliation"),
            new ("uid", "sAMAccountName")
        };
    }
}
