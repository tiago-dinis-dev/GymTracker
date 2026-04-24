CI/CD for GymTracker (dev/prod)

Overview
- Frontend: Vercel (preferred) – auto-deploys on GitHub push or via Vercel token
- Backend: Render (or container registry + host) – deploy from Git or trigger via API
- Database: Supabase (managed Postgres)

Environments
- dev branch -> Dev environment (Vercel preview, Render dev service, Supabase dev DB)
- main branch -> Prod environment (Vercel production, Render prod service, Supabase prod DB)

Secrets (set in GitHub repo Settings -> Secrets)
- VERCEL_TOKEN, VERCEL_PROJECT_ID, VERCEL_ORG_ID
- RENDER_API_KEY, RENDER_SERVICE_ID
- DEFAULT_CONNECTION (connection string for EF migrations)
- JWT__KEY (production JWT signing key)
- GITHUB_TOKEN (provided automatically; use for GHCR pushes)

Workflow summary
- Frontend workflow (./github/workflows/frontend.yml): build on PRs/pushes; deploy to Vercel for dev/main
- Backend workflow (./github/workflows/backend.yml): build/test, publish container to GHCR, trigger Render deploy; run EF migrations post-deploy

Promoting from dev to prod
1. Merge your dev branch into main (via PR after tests pass)
2. GitHub Actions will run prod workflows
3. Post-deploy: monitor logs on Render and run smoke tests

Notes
- Do NOT store real secrets in .env files committed to the repo. Use platform secret stores.
- For zero-downtime DB migrations, prefer backward-compatible migrations and rolling deploys.
- If using Render Git deploy, Render will build from your repo; otherwise use the Docker image build+push approach above.

Need help setting up the provider dashboards (Vercel/Render/Supabase) and adding the required secrets? See deployment/secrets-setup.md for exact gh CLI commands and recommended secret names.