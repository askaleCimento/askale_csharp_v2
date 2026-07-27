param([string]$ProjectRoot = ".")
$modelsPath = Join-Path $ProjectRoot "AskalePortal.Data/Models"
$controllersPath = Join-Path $ProjectRoot "AskalePortal/Controllers"
$entityNames = Get-ChildItem $modelsPath -Filter *.cs -Recurse | ForEach-Object { $_.BaseName }
$violations = @()
Get-ChildItem $controllersPath -Filter *.cs -Recurse | ForEach-Object {
    $file = $_
    foreach ($entity in $entityNames) {
        $hits = Select-String -Path $file.FullName -Pattern "\[From(Form|Body)\].*\b$entity\b|ActionResult<[^>]*\b$entity\b" -AllMatches
        foreach ($hit in $hits) { $violations += "$($file.FullName):$($hit.LineNumber): $($hit.Line.Trim())" }
    }
}
if ($violations.Count -gt 0) {
    $violations | Sort-Object -Unique
    throw "API contract still contains entity types."
}
Write-Host "OK: Controller request/response signatures do not expose entity types." -ForegroundColor Green
