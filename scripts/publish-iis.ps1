param(
    [string]$ProjectPath = "backend\src\Services\CleanArchWeb.Api\CleanArchWeb.Api.csproj",
    [string]$ProfileName = "IIS-WebDeploy",
    [string]$Configuration = "Release",
    [string]$UserName,
    [string]$Password,
    [switch]$AllowUntrustedCertificate
)

Write-Host "==> Project: $ProjectPath" -ForegroundColor Cyan
Write-Host "==> Profile: $ProfileName" -ForegroundColor Cyan
Write-Host "==> Configuration: $Configuration" -ForegroundColor Cyan

if (!(Test-Path $ProjectPath)) {
    throw "Project not found: $ProjectPath"
}

$projectDir = Split-Path -Parent $ProjectPath
$profilePath = Join-Path $projectDir "Properties/PublishProfiles/$ProfileName.pubxml"
if (!(Test-Path $profilePath)) {
    throw "Publish profile not found: $profilePath"
}

# Build MSBuild arguments
$props = @(
    "/t:Build",
    "/p:Configuration=$Configuration",
    "/p:DeployOnBuild=true",
    "/p:PublishProfile=$ProfileName"
)

if ($UserName) { $props += "/p:UserName=$UserName" }
if ($Password) { $props += "/p:Password=$Password" }
if ($AllowUntrustedCertificate) { $props += "/p:AllowUntrustedCertificate=True" }

Write-Host "==> Running: dotnet msbuild $ProjectPath $($props -join ' ')" -ForegroundColor Yellow

# Use dotnet msbuild to avoid requiring full VS msbuild locally
$argsList = @("msbuild", $ProjectPath) + $props
$process = Start-Process -FilePath "dotnet" -ArgumentList $argsList -NoNewWindow -PassThru -Wait

if ($process.ExitCode -ne 0) {
    throw "Publish failed with exit code $($process.ExitCode)"
}

Write-Host "==> Publish completed successfully." -ForegroundColor Green
