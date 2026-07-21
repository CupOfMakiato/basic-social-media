# zip file
$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$artifacts = Join-Path $root "artifacts"
$publishDir = Join-Path $artifacts "lambda-publish"
$zipPath = Join-Path $artifacts "BasicSocialMedia.API.zip"
$project = Join-Path $root "BasicSocialMedia.API\BasicSocialMedia.API.csproj"

New-Item -ItemType Directory -Force -Path $artifacts | Out-Null
Remove-Item -Recurse -Force -LiteralPath $publishDir -ErrorAction SilentlyContinue
Remove-Item -Force -LiteralPath $zipPath -ErrorAction SilentlyContinue

dotnet publish $project -c Release -o $publishDir --no-restore
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipPath -Force

Write-Host "Created $zipPath"
