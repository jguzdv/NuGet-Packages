# NuGet packages

This monorepo uses one GitHub Actions workflow, [packages.yml](.github/workflows/packages.yml),
for libraries, tools and templates. Packable source projects are discovered from tracked
project files with explicit `IsPackable=true` and `PackageId` properties; adding a
library following the existing project layout no longer requires adding a workflow.

## Building

- Pull requests to `main` build changed package groups and their transitive
  project-reference consumers. Shared build configuration and workflow changes build all groups.
  Documentation outside package directories does not trigger package builds.
- Under **Actions > NuGet packages > Run workflow**, select a branch and enter
  a project/package name, a group such as `JGUZDV.JobHost` or `JGUZDV.DynamicForms`,
  or `all`. Leave **Publish to NuGet.org** unchecked for a build-only run.
  A group name takes precedence over a package name: `JGUZDV.JobHost` builds all six packages.
  NuGet IDs that differ from project names are also accepted.
- Each group produces a uniquely named `NuGetPackage-*` artifact, retained for seven days.
  Project references build from the checkout, without depending on newly published packages.
- ActiveDirectory and ActiveDirectory.COM use Windows MSBuild for COM interop.
  Blazor.Components.Map runs `npm ci` and `npm run build:Release`.
  Existing test-execution exclusions for JobHost and DynamicForms.Extensions/Blazor remain;
  their test projects still compile. ActiveDirectory and templates remain pack-only.
  The workflow uses .NET 10 and the Microsoft Testing Platform runner from `global.json`.

The same build helper can be used locally (MSBuild must be on PATH for COM projects):

```powershell
.\.github\scripts\packages.ps1 -Mode Plan -Package all
.\.github\scripts\packages.ps1 -Mode Build -Package JGUZDV.JobHost
.\.github\scripts\packages.Tests.ps1
```

## Trusted publishing setup

Publishing requires a manual run on `main` in `jguzdv/NuGet-Packages`. All selected
builds and tests must succeed before the single `publish` job can start.

1. Create a GitHub Actions environment named **nuget**. Restrict deployment branches
   to **main** and configure required reviewers to protect publishing.
2. Set the environment secret **NUGET_USER** to the publishing account's
   nuget.org **profile name**, not its email address.
3. On nuget.org, create a trusted publishing policy with:
   - Repository owner: **jguzdv**
   - Repository: **NuGet-Packages**
   - Workflow file: **packages.yml** (filename only)
   - Environment: **nuget**
   - Package scopes covering the packages to publish (for example `JGUZDV.*`).
     The policy owner must have publishing rights for those packages.
4. Run **NuGet packages** on **main**, select a package/group or `all`, and check
   **Publish to NuGet.org**. Approve the environment deployment when prompted.
5. After a successful trusted-publishing run, remove/revoke the old long-lived
   `NUGET_API_KEY`; the workflow no longer reads it.

Only the publish job has `id-token: write`. It downloads artifacts from the current run
and uses `NuGet/login@v1` immediately before pushing to exchange GitHub OIDC for a
short-lived API key. One workflow identity can cover all permitted packages; no
per-package workflows or long-lived publishing keys are needed.

Publishing is serialized, never cancels an in-flight publish, and skips versions
already present on NuGet.org so `all` can publish a mix of changed and unchanged versions.
Other push errors fail the job. Publishing multiple packages is not atomic: if a push
fails, earlier packages may already be published; rerun after resolving the error.
Increment versions in the project files or group build properties before releasing changes.

See [NuGet trusted publishing documentation](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing).