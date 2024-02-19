[CmdLetBinding]
function Add-JGUZDVLibrary {
    param(
        [Parameter(Mandatory = $true, ValueFromPipeline=$true)]
        [string] $LibraryName,

        [Parameter()]
        [string] $Template = 'classlib'
    )

    process {
        Push-Location $PSScriptRoot

        Invoke-Expression "dotnet new $Template -n $LibraryName -o ./libraries/$LibraryName/src"
        Invoke-Expression "dotnet new xunit -n $LibraryName.Tests -o ./libraries/$LibraryName/test"

        Invoke-Expression "dotnet sln . add ./libraries/$LibraryName/src"
        Invoke-Expression "dotnet sln . add ./libraries/$LibraryName/test"

        Pop-Location
    }
}