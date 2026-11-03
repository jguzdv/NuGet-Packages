[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateSet('Plan', 'Build')]
    [string] $Mode,
    [string] $Package = 'all',
    [string] $Base,
    [string] $Head,
    [string[]] $ChangedFiles
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Invoke-Checked {
    param([string] $Command, [string[]] $Arguments)
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$Command failed with exit code $LASTEXITCODE."
    }
}

function Get-Group {
    param([string] $Path)
    if ($Path -match '^(libraries|tools)/([^/]+)/') { return "$($Matches[1])/$($Matches[2])" }
    if ($Path.StartsWith('templates/')) { return 'templates' }
    throw "Project outside package directories: $Path"
}

function Get-ProjectInputs {
    param([string] $Project, [System.Collections.Generic.HashSet[string]] $Visited)
    if (!$Visited.Add($Project)) { return }
    $Project
    [xml] $xml = Get-Content -LiteralPath $Project -Raw
    foreach ($reference in $xml.SelectNodes('/Project/ItemGroup/ProjectReference')) {
        $path = $reference.GetAttribute('Include').Replace('\', '/')
        $absolute = [IO.Path]::GetFullPath((Join-Path (Split-Path $Project) $path))
        $relative = [IO.Path]::GetRelativePath($PWD.Path, $absolute).Replace('\', '/')
        if (!(Test-Path -LiteralPath $relative)) { throw "Missing project reference: $relative" }
        Get-ProjectInputs $relative $Visited
    }
}

Push-Location (Join-Path $PSScriptRoot '..\..')
try {
    $tracked = @(Invoke-Checked git @('ls-files', '--', '*.csproj'))
    $packages = @(
        foreach ($project in $tracked) {
            if ($project -notmatch '^(libraries|tools)/[^/]+/src/' -and $project -ne 'templates/JGUZDV.Templates.csproj') { continue }
            [xml] $xml = Get-Content -LiteralPath $project -Raw
            if (@($xml.SelectNodes('/Project/PropertyGroup/IsPackable') | Where-Object { $_.InnerText -eq 'true' }).Count -eq 0) { continue }
            $name = [IO.Path]::GetFileNameWithoutExtension($project)
            $id = $xml.SelectSingleNode('/Project/PropertyGroup/PackageId')
            if ($null -eq $id -or [string]::IsNullOrWhiteSpace($id.InnerText)) { throw "$name must declare a PackageId." }
            $group = Get-Group $project
            $tests = @($tracked | Where-Object {
                $_.StartsWith("$group/") -and [IO.Path]::GetFileName($_) -eq "$name.Tests.csproj"
            })
            $useMSBuild = $group -in @('libraries/JGUZDV.ActiveDirectory', 'libraries/JGUZDV.ActiveDirectory.COM')
            $isTemplate = $group -eq 'templates'
            if (!$useMSBuild -and !$isTemplate -and $tests.Count -ne 1) {
                throw "Expected one test project for $name; found $($tests.Count)."
            }
            $inputs = @(Get-ProjectInputs $project ([System.Collections.Generic.HashSet[string]]::new()))
            if (!$useMSBuild -and !$isTemplate) {
                $inputs += @(Get-ProjectInputs $tests[0] ([System.Collections.Generic.HashSet[string]]::new()))
            }
            [pscustomobject]@{
                Name = $name
                PackageId = $id.InnerText
                Group = $group
                GroupName = if ($isTemplate) { $name } else { Split-Path $group -Leaf }
                Project = $project
                Test = if (!$useMSBuild -and !$isTemplate) { $tests[0] } else { $null }
                RunTests = $name -notin @('JGUZDV.JobHost', 'JGUZDV.DynamicForms.Extensions', 'JGUZDV.DynamicForms.Blazor')
                UseMSBuild = $useMSBuild
                IsTemplate = $isTemplate
                Inputs = $inputs
                Watch = @($inputs | ForEach-Object { Get-Group $_ } | Sort-Object -Unique)
            }
        }
    )
    if ($packages.Count -eq 0) { throw 'No packable source projects found.' }
    if (@($packages | Group-Object Name | Where-Object Count -gt 1).Count -gt 0) { throw 'Duplicate package project names.' }

    $isChangeSelection = $PSBoundParameters.ContainsKey('ChangedFiles') -or $Base -or $Head
    if ($isChangeSelection) {
        if ($Mode -ne 'Plan') { throw 'Changed-file selection is only supported in Plan mode.' }
        if (!$PSBoundParameters.ContainsKey('ChangedFiles')) {
            if (!$Base -or !$Head) { throw 'Both Base and Head are required.' }
            $ChangedFiles = @(Invoke-Checked git @('diff', '--name-only', '--no-renames', "$Base...$Head", '--'))
        }
        $changed = @($ChangedFiles | ForEach-Object { $_.Replace('\', '/') })
        $buildAll = @($changed | Where-Object {
            $_ -match '^\.github/' -or $_ -in @('global.json', '.editorconfig', 'NuGet-Packages.slnx', 'libraries/Directory.Build.props')
        }).Count -gt 0
        $selected = @($packages | Where-Object {
            $candidate = $_
            $buildAll -or @($changed | Where-Object {
                $file = $_
                @($candidate.Watch | Where-Object { $file.StartsWith("$_/") }).Count -gt 0
            }).Count -gt 0
        })
    } elseif ($Package -eq 'all') {
        $selected = $packages
    } else {
        $selected = @($packages | Where-Object GroupName -eq $Package)
        if ($selected.Count -eq 0) {
            $selected = @($packages | Where-Object { $_.Name -eq $Package -or $_.PackageId -eq $Package })
        }
        if ($selected.Count -eq 0) {
            throw "Unknown package/group '$Package'. Available: $(($packages.GroupName | Sort-Object -Unique) -join ', ')"
        }
    }

    if ($Mode -eq 'Plan') {
        $matrix = @(
            foreach ($group in ($selected | Group-Object Group | Sort-Object Name)) {
                $selection = if ($isChangeSelection -or $Package -eq 'all') { $group.Group[0].GroupName } else { $Package }
                [pscustomobject]@{
                    name = $group.Group[0].GroupName
                    selection = $selection
                    runner = if ($group.Group[0].UseMSBuild) { 'windows-latest' } else { 'ubuntu-latest' }
                }
            }
        )
        $json = ConvertTo-Json -InputObject @{ include = $matrix } -Depth 5 -Compress
        if ($env:GITHUB_OUTPUT) {
            Add-Content -LiteralPath $env:GITHUB_OUTPUT -Value "matrix=$json"
            Add-Content -LiteralPath $env:GITHUB_OUTPUT -Value "has-packages=$($matrix.Count -gt 0)".ToLowerInvariant()
        }
        $json
        return
    }

    # Referenced projects build locally; they do not need to be published first.
    $npmProjects = @($selected.Inputs | Sort-Object -Unique | Where-Object {
        Test-Path -LiteralPath (Join-Path (Split-Path $_) 'package.json')
    })
    foreach ($project in $npmProjects) {
        Push-Location (Split-Path $project)
        try {
            Invoke-Checked npm @('ci')
            Invoke-Checked npm @('run', 'build:Release')
        } finally {
            Pop-Location
        }
    }
    foreach ($item in ($selected | Sort-Object Name)) {
        $output = Join-Path $PWD.Path "publish\$($item.Name)"
        if ($item.UseMSBuild) {
            Invoke-Checked msbuild @($item.Project, '/t:Restore,Build,Pack', '/p:Configuration=Release', "/p:PackageOutputPath=$output")
            continue
        }
        Invoke-Checked dotnet @('restore', $item.Project)
        if (!$item.IsTemplate) {
            Invoke-Checked dotnet @('build', $item.Project, '--no-restore', '-c', 'Release')
            Invoke-Checked dotnet @('restore', $item.Test)
            Invoke-Checked dotnet @('build', $item.Test, '--no-restore', '-c', 'Release')
            if ($item.RunTests) {
                $arguments = @('test', '--project', $item.Test, '--no-build', '-c', 'Release')
                if ($item.Group.StartsWith('libraries/')) { $arguments += @('--ignore-exit-code', '8') }
                Invoke-Checked dotnet $arguments
            } else {
                Write-Host "Test execution disabled for $($item.Name) (existing workflow exclusion)."
            }
        }
        Invoke-Checked dotnet @('pack', $item.Project, '--no-build', '-c', 'Release', '--output', $output)
    }
} finally {
    Pop-Location
}
