Setting GitHub secrets and provider steps

Recommended secrets to set in GitHub (Repository Settings -> Secrets -> Actions):

- VERCEL_TOKEN
- VERCEL_ORG_ID
- VERCEL_PROJECT_ID
- RENDER_API_KEY
- RENDER_SERVICE_ID
- DEFAULT_CONNECTION  (connection string for EF migrations)
- JWT__KEY (production JWT signing key)
- FITNESS_AGENT__APIKEY
- FITNESS_AGENT__MODELID
- FITNESS_AGENT__ENDPOINT
- SMOKE_BACKEND_URL
- SMOKE_FRONTEND_URL

Use the GitHub CLI to set secrets locally:

# login first
gh auth login

# set a secret interactively
gh secret set JWT__KEY --body "$(pwsh -Command '[Convert]::ToBase64String((1..32|%{Get-Random -Max 256}))')"

# or set from a file
gh secret set DEFAULT_CONNECTION --body "Server=...;Database=...;User Id=...;Password=..."

Notes about permissions:
- The Promote workflow uses the Render API key to trigger deploys. Keep that key secret.
- The Promote workflow triggers smoke tests via the GitHub repository_dispatch API using the GITHUB_TOKEN.

Provider quick links:
- Vercel dashboard: https://vercel.com
- Render dashboard: https://dashboard.render.com
- Supabase dashboard: https://app.supabase.com

If you want, I can generate the exact gh/az/aws CLI commands for your provider and create a secrets provisioning script.