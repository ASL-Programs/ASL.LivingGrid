param(
    [string]$Configuration = "Release"
)

# Determine the directory of this script so the script can be executed
# from any location. `$PSScriptRoot` points to the folder containing the
# current PowerShell script.
$scriptDir = $PSScriptRoot
$solutionPath = Join-Path (Join-Path $scriptDir "..") "ASL.LivingGrid.sln"

Write-Host "Restoring packages..."
dotnet restore $solutionPath

Write-Host "Building solution..."
dotnet build $solutionPath -c $Configuration

Write-Host "Running tests..."
dotnet test $solutionPath -c $Configuration --no-build
