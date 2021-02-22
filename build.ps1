#! pwsh

param (
    [SwitchParameter]$BuildNuGet
)

$Projects = @(
    'Exiled.Bootstrap',
    'Exiled.Loader',
    'Exiled.API',
    'Exiled.Permissions',
    'Exiled.Events',
    'Exiled.Updater',
    'Exiled.CreditTags',
    'Exiled.Example'
)

# Restore projects
Execute 'dotnet restore'
# Build projects
Execute 'dotnet build' '-c release'
# Build a NuGet package if needed
if ($BuildNuGet) {
    $Version = GetSolutionVersion
    $Year = [System.DateTime]::Now.ToString('yyyy')

    Write-Host "Generating NuGet package for version $Version"

    nuget pack Exiled/Exiled.nuspec -Version $Version -Properties Year=$Year
}

function Execute {
    param (
        [string]$Cmd,
        [string[]]$CmdArgs
    )

    foreach ($Project in $Projects) {
        Invoke-Command $Cmd -ArgumentList $Project $CmdArgs
        CheckLastOperationStatus
    }
}

function CheckLastOperationStatus {
    if ($? -eq $false) {
        Exit 1
    }
}

function GetSolutionVersion {
    [XML]$PropsFile = Get-Content Exiled.props
    $Version = $PropsFile.Project.PropertyGroup[2].Version
    $Version = $Version.'#text'.Trim()
    return $Version
}
