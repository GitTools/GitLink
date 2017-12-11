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

    # Restore VS solution dependencies
    msbuild.exe /t:restore "$PSScriptRoot\src\GitLink.sln"

    Write-Host "Successfully restored all dependencies" -ForegroundColor Yellow
}
catch {
    Write-Error "Aborting script due to error"
    exit $lastexitcode
}
finally {
    Pop-Location
}
