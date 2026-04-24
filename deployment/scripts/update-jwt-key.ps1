$envFile = Join-Path $PSScriptRoot ".env"
$exampleFile = Join-Path $PSScriptRoot ".env.example"

if (-not (Test-Path $envFile)) {
    Copy-Item $exampleFile $envFile
}

$secret = [Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
$content = Get-Content $envFile -Raw

if ($content -match '(?m)^JWT__KEY=.*$') {
    $content = $content -replace '(?m)^JWT__KEY=.*$', "JWT__KEY=$secret"
} else {
    $content = $content.TrimEnd() + "`r`nJWT__KEY=$secret`r`n"
}

Set-Content -Path $envFile -Value $content
Write-Host "Updated JWT__KEY in $envFile"
