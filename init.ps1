<#
.SYNOPSIS
    Restores NuGet packages.
#>
Param(
)

Push-Location $PSScriptRoot
try {
    $HeaderColor = 'Green'
    $toolsPath = "$PSScriptRoot\tools"
    $nugetVerbosity = 'quiet'
    if ($Verbose) { $nugetVerbosity = 'normal' }

    # First restore NuProj packages since the solution restore depends on NuProj evaluation succeeding.
    gci "$PSScriptRoot\src\project.json" -rec |? { $_.FullName -imatch 'nuget' } |% {
        & "$toolsPath\Restore-NuGetPackages.ps1" -Path $_ -Verbosity $nugetVerbosity
    }

    # Restore VS solution dependencies
    gci "$PSScriptRoot\src" -rec |? { $_.FullName.EndsWith('.sln') } |% {
        & "$toolsPath\Restore-NuGetPackages.ps1" -Path $_.FullName -Verbosity $nugetVerbosity
    }

    Write-Host "Successfully restored all dependencies" -ForegroundColor Yellow
}
catch {
    Write-Error "Aborting script due to error"
    exit $lastexitcode
}
finally {
    Pop-Location
}
