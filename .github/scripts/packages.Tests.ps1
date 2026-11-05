$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$script:helper = Join-Path $PSScriptRoot 'packages.ps1'
$script:commands = [System.Collections.Generic.List[object]]::new()
$script:failCommand = ''

function Assert {
    param([bool] $Condition, [string] $Message)
    if (!$Condition) { throw $Message }
}

function Plan {
    param([string] $Package = 'all', [string[]] $ChangedFiles)
    $arguments = @{ Mode = 'Plan'; Package = $Package }
    if ($PSBoundParameters.ContainsKey('ChangedFiles')) { $arguments.ChangedFiles = $ChangedFiles }
    (& $script:helper @arguments | ConvertFrom-Json).include
}

function Record-Command {
    param([string] $Name, [object[]] $Arguments)
    $commands.Add([pscustomobject]@{ Name = $Name; Arguments = $Arguments; Directory = $PWD.Path })
    $global:LASTEXITCODE = if ($failCommand -eq $Name) { 1 } else { 0 }
}

function dotnet { Record-Command dotnet $args }
function npm { Record-Command npm $args }
function msbuild { Record-Command msbuild $args }

function Build {
    param([string] $Package)
    $script:commands.Clear()
    & $script:helper -Mode Build -Package $Package
}

function Assert-Throws {
    param([scriptblock] $Action, [string] $Pattern)
    $caught = $null
    try { & $Action } catch { $caught = $_ }
    Assert ($null -ne $caught -and $caught.Exception.Message -like $Pattern) "Expected error matching '$Pattern'."
}

$previousOutput = $env:GITHUB_OUTPUT
$previousExitCode = if (Test-Path variable:LASTEXITCODE) { $LASTEXITCODE } else { 0 }
$output = [IO.Path]::GetTempFileName()
try {
    $env:GITHUB_OUTPUT = $output
    $all = @(Plan)
    Assert ($all.Count -ge 34) 'Existing package groups must all be discovered.'
    Assert (@($all.name | Sort-Object -Unique).Count -eq $all.Count) 'Matrix names must be unique.'
    Assert (@($all | Where-Object runner -eq 'windows-latest').Count -eq 2) 'Both COM builds must use Windows.'
    Assert (@($all | Where-Object name -eq 'JGUZDV.JsonMerge').Count -eq 1) 'Tool package is missing.'
    Assert (@($all | Where-Object name -eq 'JGUZDV.Templates').Count -eq 1) 'Template package is missing.'
    Assert (@(Plan -ChangedFiles @('README.md')).Count -eq 0) 'Root documentation should not build packages.'
    Assert (@(Plan -ChangedFiles @()).Count -eq 0) 'Empty changes should produce an empty matrix.'
    Assert ((Get-Content $output -Raw).Contains('has-packages=false')) 'No-op selection output must be false.'
    foreach ($shared in @('global.json', '.github/workflows/packages.yml', '.github/scripts/packages.ps1', 'libraries/Directory.Build.props')) {
        Assert (@(Plan -ChangedFiles @($shared)).Count -eq $all.Count) "Shared change $shared should build all groups."
    }
    $jobHost = @(Plan -ChangedFiles @('libraries\JGUZDV.JobHost\Directory.Build.props'))
    Assert ($jobHost.Count -eq 1 -and $jobHost[0].name -eq 'JGUZDV.JobHost') 'JobHost should stay a single group.'
    $consumers = @(Plan -ChangedFiles @('libraries/JGUZDV.Extensions.Json/src/SomeFile.cs'))
    foreach ($name in @('JGUZDV.Extensions.Json', 'JGUZDV.Blazor.Components', 'JGUZDV.DynamicForms', 'JGUZDV.JobHost')) {
        Assert ($name -in $consumers.name) "Transitive consumer $name is missing."
    }
    $com = @(Plan -ChangedFiles @('libraries/JGUZDV.ActiveDirectory.COM/src/Com.cs'))
    Assert ($com.Count -eq 2) 'COM changes should also rebuild ActiveDirectory.'
    $templates = @(Plan -ChangedFiles @('templates/src/BlazorWebTemplate/build/build_BlazorWebTemplate.Web.yml'))
    Assert ($templates.Count -eq 1 -and $templates[0].name -eq 'JGUZDV.Templates') 'Template contents should trigger packing.'
    $alias = @(Plan -Package JGUZDV.L10n.EntityFrameworkCore)
    Assert ($alias.Count -eq 1 -and $alias[0].name -eq 'JGUZDV.L10n.EntityFramework') 'NuGet ID alias selection failed.'
    Assert-Throws { Plan -Package 'not-a-package' } "*Unknown package/group*"
    Assert-Throws { & $script:helper -Mode Plan -Base HEAD } '*Both Base and Head*'

    Build JGUZDV.JobHost
    Assert (@($script:commands | Where-Object { $_.Arguments[0] -eq 'pack' }).Count -eq 6) 'JobHost must pack six projects.'
    Assert (@($script:commands | Where-Object { $_.Arguments[0] -eq 'test' }).Count -eq 5) 'JobHost must retain its test exclusion.'
    foreach ($test in ($script:commands | Where-Object { $_.Arguments[0] -eq 'test' })) {
        Assert ($test.Arguments[1] -eq '--project' -and '--ignore-exit-code' -in $test.Arguments) 'Library test runner arguments are incorrect.'
    }
    Build JGUZDV.JobHost.Shared
    Assert (@($script:commands | Where-Object { $_.Arguments[0] -eq 'pack' }).Count -eq 1) 'Single package selection must not pack the whole group.'
    Build JGUZDV.DynamicForms
    Assert (@($script:commands | Where-Object { $_.Arguments[0] -eq 'pack' }).Count -eq 3) 'DynamicForms must pack three projects.'
    Assert (@($script:commands | Where-Object { $_.Arguments[0] -eq 'test' }).Count -eq 1) 'DynamicForms must retain its test exclusions.'
    Assert (@($script:commands | Where-Object { $_.Arguments[0] -eq 'build' }).Count -eq 6) 'Excluded tests must still compile.'
    Build JGUZDV.Blazor.Components.Map
    Assert ($script:commands[0].Name -eq 'npm' -and $script:commands[0].Arguments[0] -eq 'ci') 'npm restore must precede .NET builds.'
    Assert ($script:commands[1].Name -eq 'npm' -and $script:commands[1].Arguments[1] -eq 'build:Release') 'npm release assets must build.'
    Assert ($script:commands[0].Directory.EndsWith('src')) 'npm must run in the source directory.'
    Build JGUZDV.ActiveDirectory
    Assert ($script:commands.Count -eq 1 -and $script:commands[0].Name -eq 'msbuild') 'COM builds must use full MSBuild, without tests.'
    Assert ($script:commands[0].Arguments[1] -eq '/t:Restore,Build,Pack') 'COM build targets are incorrect.'
    Assert (@($script:commands[0].Arguments | Where-Object { $_ -like '/p:PackageOutputPath=*publish*' }).Count -eq 1) 'COM output must use the publish directory.'
    Build JGUZDV.Templates
    Assert ($script:commands.Count -eq 2 -and $script:commands[1].Arguments[0] -eq 'pack') 'Templates should restore and pack without compiling.'
    Build JGUZDV.JsonMerge
    $toolTest = @($script:commands | Where-Object { $_.Arguments[0] -eq 'test' })
    Assert ($toolTest.Count -eq 1 -and '--ignore-exit-code' -notin $toolTest[0].Arguments) 'Tool tests must not ignore empty-test errors.'
    $script:failCommand = 'dotnet'
    Assert-Throws { Build JGUZDV.Linq } '*dotnet failed with exit code 1*'
    Assert ($script:commands.Count -eq 1) 'Build must stop at the first failed command.'

    Write-Host 'Package workflow tests passed.'
} finally {
    $env:GITHUB_OUTPUT = $previousOutput
    $global:LASTEXITCODE = $previousExitCode
    Remove-Item -LiteralPath $output
}
