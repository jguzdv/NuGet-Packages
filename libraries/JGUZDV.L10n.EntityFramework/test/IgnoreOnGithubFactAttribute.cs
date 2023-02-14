namespace JGUZDV.L10n.EntityFramework.Tests;

public sealed class IgnoreOnGithubFactAttribute : FactAttribute
{
    public IgnoreOnGithubFactAttribute()
    {
        if (IsGitHubAction())
        {
            Skip = "Ignore the test when run in Github agent.";
        }
    }

    private static bool IsGitHubAction()
        => Environment.GetEnvironmentVariable("GITHUB_ACTION") != null;
}