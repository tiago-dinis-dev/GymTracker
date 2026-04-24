Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot "ensure-jwt-key.ps1")

$envFile = Join-Path $PSScriptRoot ".env"
docker compose -f (Join-Path $PSScriptRoot "docker-compose.yml") --env-file $envFile up --build
