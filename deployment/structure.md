Proposed deployment/ organization

Goal: split files by theme so it's easy to find Docker, CI, scripts, docs.

Suggested layout:

deployment/
  docker/                # production Dockerfiles and compose
    backend.Dockerfile
    frontend.Dockerfile
    nginx.conf
    docker-compose.yml
  docker-dev/            # dev Dockerfiles and compose
    backend.dev.Dockerfile
    frontend.dev.Dockerfile
    docker-compose.dev.yml
  scripts/               # local helper scripts
    start-dev.ps1
    start-prod.ps1
    ensure-jwt-key.ps1
    update-jwt-key.ps1
    provision-secrets.ps1
  ci/                    # CI helper artifacts & docs
    CI-CD.md
    secrets-setup.md
  docs/                  # deployment docs and README
    README.md
    .env.example
    .env.prod

Manual reorganization steps (run from repo root):

1. Create directories:
   mkdir deployment\docker
   mkdir deployment\docker-dev
   mkdir deployment\scripts
   mkdir deployment\ci
   mkdir deployment\docs

2. Move files (example):
   git mv deployment\backend.Dockerfile deployment\docker\backend.Dockerfile
   git mv deployment\frontend.Dockerfile deployment\docker\frontend.Dockerfile
   git mv deployment\nginx.conf deployment\docker\nginx.conf
   git mv deployment\docker-compose.yml deployment\docker\docker-compose.yml

   git mv deployment\backend.dev.Dockerfile deployment\docker-dev\backend.dev.Dockerfile
   git mv deployment\frontend.dev.Dockerfile deployment\docker-dev\frontend.dev.Dockerfile
   git mv deployment\docker-compose.dev.yml deployment\docker-dev\docker-compose.dev.yml

   git mv deployment\start-dev.ps1 deployment\scripts\start-dev.ps1
   git mv deployment\start-prod.ps1 deployment\scripts\start-prod.ps1
   git mv deployment\ensure-jwt-key.ps1 deployment\scripts\ensure-jwt-key.ps1
   git mv deployment\update-jwt-key.ps1 deployment\scripts\update-jwt-key.ps1
   git mv deployment\provision-secrets.ps1 deployment\scripts\provision-secrets.ps1

   git mv deployment\CI-CD.md deployment\ci\CI-CD.md
   git mv deployment\secrets-setup.md deployment\ci\secrets-setup.md

   git mv deployment\README.md deployment\docs\README.md
   git mv deployment\.env.example deployment\docs\.env.example
   git mv deployment\.env.prod deployment\docs\.env.prod

3. Commit changes:
   git add -A
   git commit -m "Reorganize deployment folder into docker, docker-dev, scripts, ci, docs"

If you want, I can generate a PowerShell script to perform these git mv steps for you. Run it after reviewing the file list to ensure no conflicts.
