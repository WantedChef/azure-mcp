# Claude Sonnet 4.5 Deployment voor ClawBot

Simpel, werkend deployment script voor Claude Sonnet 4.5 via Azure AI Foundry.

## Vereisten

```bash
# 1. Azure CLI
az --version

# 2. Inloggen
az login

# 3. AI Studio Workspace (maak aan via portal of CLI)
az ml workspace create -n my-workspace -g my-rg -l westeurope
```

## Gebruik

```bash
./deploy-claude-sonnet.sh <resource-group> <workspace-name> [deployment-name]
```

### Voorbeeld:

```bash
./deploy-claude-sonnet.sh my-rg my-workspace clawbot
```

## Wat doet het script?

1. ✅ Checkt Azure CLI
2. ✅ Installeert/update ML extension
3. ✅ Verifieert login
4. ✅ Maakt endpoint config
5. ✅ Deployt Claude Sonnet 4.5
6. ✅ Haalt credentials op
7. ✅ Genereert ClawBot config

## Output

Script genereert `/tmp/clawbot-claude.env`:

```bash
export CLAUDE_ENDPOINT="https://..."
export CLAUDE_API_KEY="..."
export CLAUDE_MODEL="claude-sonnet-4-5-20250929"
export CLAUDE_DEPLOYMENT="clawbot"
```

## Gebruik met ClawBot

```bash
# Load config
source /tmp/clawbot-claude.env

# Test
curl -X POST "$CLAUDE_ENDPOINT/v1/messages" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $CLAUDE_API_KEY" \
  -d '{
    "model": "claude-sonnet-4-5-20250929",
    "messages": [{"role": "user", "content": "Hello!"}],
    "max_tokens": 100
  }'
```

## Troubleshooting

**Workspace bestaat niet:**
```bash
az ml workspace create -n my-workspace -g my-rg -l westeurope
```

**ML extension fout:**
```bash
az extension remove -n ml
az extension add -n ml
```

**Model ID onjuist:**
Check in AI Studio: https://ai.azure.com → Model Catalog → Claude Sonnet 4.5

**Deployment verwijderen:**
```bash
az ml serverless-endpoint delete -n clawbot -g my-rg -w my-workspace
```
