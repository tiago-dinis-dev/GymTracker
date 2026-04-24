# GymTracker deployment

This folder keeps the Docker deployment files for the app.

## Production-style setup

Use this when you want image-based containers closer to production.

```powershell
cd deployment
.\start-prod.ps1
```

## Dev setup

Use this when you want hot reload during development.

```powershell
cd deployment
.\start-dev.ps1
```

To rotate the JWT signing key:

```powershell
.\update-jwt-key.ps1
```

If you want to create the key only when missing, use:

```powershell
.\ensure-jwt-key.ps1
```

## How it works

1. The frontend calls `/api/...`.
2. Nginx or Vite forwards that to the backend container.
3. The backend runs the API and agent.
4. The backend talks to Ollama (dev/local) or OpenClaw (prod) for model responses.

## Services

| Service | Purpose | Port |
|---|---|---|
| frontend | React app served by Nginx / Vite dev server | 8080 / 5173 |
| backend | ASP.NET Core API + agent | 5008 |
| ollama (dev) | Local model runtime (dev) | 11434 |
| openclaw (prod) | Local model runtime (prod) | 18789 |

## Notes

- SQLite files are stored in a Docker volume so data survives restarts.
- The backend uses environment variables for JWT, database paths, and agent settings.
- The dev compose file mounts your source code so backend and frontend reload as you edit.
