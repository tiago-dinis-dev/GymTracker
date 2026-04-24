Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot "ensure-jwt-key.ps1")

# Determine repository root (two levels up from deployment\scripts)
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..") | Select-Object -ExpandProperty Path

# Load environment variables from deployment/.env (if present)
$envFile = Join-Path $PSScriptRoot "..\.env"
if (Test-Path $envFile) {
    Get-Content $envFile | ForEach-Object {
        $line = $_.Trim()
        if ($line -and -not $line.StartsWith('#') -and $line -match '^[A-Za-z0-9_]+=') {
            $parts = $line -split '=',2
            $name = $parts[0].Trim()
            $value = $parts[1].Trim()
            try {
                Set-Item -Path "Env:$name" -Value $value -ErrorAction Stop
            } catch {
                Write-Verbose ("Failed to set env {0}: {1}" -f $name, $_)
            }
        }
    }
}

# Ensure local data directory exists
$dataDir = Join-Path $repoRoot 'data'
if (-not (Test-Path $dataDir)) {
    New-Item -ItemType Directory -Path $dataDir | Out-Null
}

# Override DB paths for local development (host filesystem)
$env:DEFAULT_CONNECTION = "Data Source=$($dataDir)\gymtracker.db"
$env:DEFAULT_CONNECTION_AI = "Data Source=$($dataDir)\gymtracker_ai.db"

# Ensure frontend dev proxy targets the local backend
$env:VITE_API_PROXY_TARGET = "http://localhost:5008"

# Backend URL used for local dev
$backendUrl = "http://localhost:5008"

# Use explicit PowerShell 7 executable
$pwshCmd = 'C:\Program Files\PowerShell\7\pwsh.exe'
if (-not (Test-Path $pwshCmd)) { throw "pwsh not found at $pwshCmd. Please install PowerShell 7 or update this script with the correct path." }

Write-Host "Starting backend (dotnet watch) in new pwsh window..."
$backendCmd = "cd `"$repoRoot\backend\Api`"; dotnet watch run --no-restore --urls `"$backendUrl`""
Start-Process -FilePath $pwshCmd -ArgumentList "-NoExit","-Command",$backendCmd

Write-Host "Starting frontend (npm dev) in new pwsh window..."
$frontendPath = Join-Path $repoRoot 'frontend'
if (-not (Test-Path (Join-Path $frontendPath 'node_modules'))) {
    $installCmd = "cd `"$frontendPath`"; npm install; npm run dev"
    Start-Process -FilePath $pwshCmd -ArgumentList "-NoExit","-Command",$installCmd
} else {
    $frontendCmd = "cd `"$frontendPath`"; npm run dev"
    Start-Process -FilePath $pwshCmd -ArgumentList "-NoExit","-Command",$frontendCmd
}

Write-Host "Dev services started. Backend: $backendUrl, Frontend: http://localhost:5173"
