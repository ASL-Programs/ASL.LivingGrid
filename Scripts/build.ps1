param(
    [string]$Configuration = "Release"
)

Write-Host "Restoring packages..."
dotnet restore ..\ASL.LivingGrid.sln

Write-Host "Building solution..."
dotnet build ..\ASL.LivingGrid.sln -c $Configuration

Write-Host "Running tests..."
dotnet test ..\ASL.LivingGrid.sln -c $Configuration --no-build
