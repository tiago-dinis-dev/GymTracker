# Provision GitHub secrets for GymTracker using the GitHub CLI (gh)
# Run this from the repo root: deployment\provision-secrets.ps1
# Requires: gh CLI authenticated (run `gh auth login`) and you must have repo admin rights.

function Read-Secret($name, $default = '') {
    $val = Read-Host -AsSecureString "Enter value for $name (leave blank to keep default)"
    if ([Runtime.InteropServices.Marshal]::PtrToStringUni([Runtime.InteropServices.Marshal]::SecureStringToBSTR($val)) -eq '') {
        return $default
    }
    return [Runtime.InteropServices.Marshal]::PtrToStringUni([Runtime.InteropServices.Marshal]::SecureStringToBSTR($val))
}

Write-Host "Provisioning GitHub secrets for repository: $env:GITHUB_REPOSITORY" -ForegroundColor Cyan

# Optionally read from .env.prod
$envFile = Join-Path $PSScriptRoot ".env.prod"
$envValues = @{}
if (Test-Path $envFile) {
    Write-Host "Found .env.prod, loading values as defaults (will not overwrite secrets unless provided)" -ForegroundColor Green
    Get-Content $envFile | ForEach-Object {
        if ($_ -match '^([^#=]+)=(.*)$') { $envValues[$matches[1].Trim()] = $matches[2].Trim() }
    }
}

$secrets = @(
    'VERCEL_TOKEN', 'VERCEL_ORG_ID', 'VERCEL_PROJECT_ID',
    'RENDER_API_KEY', 'RENDER_SERVICE_ID',
    'DEFAULT_CONNECTION', 'JWT__KEY', 'FITNESS_AGENT__MODELID', 'FITNESS_AGENT__ENDPOINT',
    'SMOKE_BACKEND_URL', 'SMOKE_FRONTEND_URL'
)

foreach ($s in $secrets) {
    $default = $envValues[$s]
    if ($default -eq $null) { $default = '' }
    Write-Host "\nSecret: $s" -ForegroundColor Yellow
    $val = Read-Secret $s $default
    if ($val -ne '') {
        Write-Host "Setting secret $s..." -ForegroundColor Gray
        gh secret set $s --body "$val"
        if ($?) { Write-Host "Set $s" -ForegroundColor Green } else { Write-Host "Failed to set $s" -ForegroundColor Red }
    } else {
        Write-Host "Skipped $s (empty)" -ForegroundColor DarkYellow
    }
}

Write-Host "All done. Verify secrets at https://github.com/${env:GITHUB_REPOSITORY}/settings/secrets/actions" -ForegroundColor Cyan
