param(
    [Parameter(Mandatory=$false)]
    [string]$ProjectRoot = "."
)

$ErrorActionPreference = "Stop"
$packageRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceRoot = Join-Path $packageRoot "files"
$projectRootResolved = (Resolve-Path $ProjectRoot).Path
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupRoot = Join-Path $projectRootResolved ".hotfix-backups/entity-dto-$stamp"

New-Item -ItemType Directory -Force -Path $backupRoot | Out-Null

Get-ChildItem $sourceRoot -File -Recurse | ForEach-Object {
    $relative = $_.FullName.Substring($sourceRoot.Length).TrimStart('\','/')
    $target = Join-Path $projectRootResolved $relative
    $backup = Join-Path $backupRoot $relative

    if (Test-Path $target) {
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $backup) | Out-Null
        Copy-Item $target $backup -Force
    }

    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
    Copy-Item $_.FullName $target -Force
}

Set-Content -Path (Join-Path $projectRootResolved ".hotfix-backups/entity-dto-latest.txt") -Value $backupRoot
Write-Host "Entity DTO refactor uygulandı." -ForegroundColor Green
Write-Host "Yedek: $backupRoot"
