# 🤖 ClawBot Model Deployer

Automatische deployment van AI modellen voor ClawBot via Azure AI Foundry met SSH configuratie.

**Laatst bijgewerkt:** 27 januari 2026

## 🎯 Wat doet dit?

Deze scripts deployen automatisch een actueel AI model naar Azure AI Foundry en configureren ClawBot via SSH:

1. ✅ Deploy een model naar Azure AI Foundry
2. ✅ Haal deployment informatie op
3. ✅ Configureer ClawBot via SSH (optioneel)
4. ✅ Genereer startinstructies

## 📋 Vereisten

- Azure CLI geïnstalleerd en ingelogd (`az login`)
- Azure MCP Server geïnstalleerd (`azmcp`)
- SSH toegang tot ClawBot server (voor automatische configuratie)
- Azure AI Services resource aangemaakt

## 🚀 Quick Start

### Windows (PowerShell)

```powershell
# Basis deployment (zonder SSH)
.\deploy-clawdbot-model.ps1 `
    -ResourceGroup "mijn-resource-group" `
    -AzureAiServicesName "mijn-ai-service"

# Met SSH configuratie
.\deploy-clawdbot-model.ps1 `
    -ResourceGroup "mijn-resource-group" `
    -AzureAiServicesName "mijn-ai-service" `
    -SSHHost "clawbot.example.com" `
    -SSHUser "admin" `
    -SSHKeyPath "~/.ssh/id_rsa"

# Custom model
.\deploy-clawdbot-model.ps1 `
    -ResourceGroup "mijn-resource-group" `
    -AzureAiServicesName "mijn-ai-service" `
    -ModelName "Meta-Llama-3.1-8B-Instruct" `
    -DeploymentName "clawbot-llama"
```

### Linux/macOS/SSH (Bash)

```bash
# Maak executable
chmod +x deploy-clawdbot-model.sh

# Basis deployment
./deploy-clawdbot-model.sh \
    --resource-group "mijn-resource-group" \
    --ai-service "mijn-ai-service"

# Met SSH configuratie
./deploy-clawdbot-model.sh \
    --resource-group "mijn-resource-group" \
    --ai-service "mijn-ai-service" \
    --ssh-host "clawbot.example.com" \
    --ssh-user "admin" \
    --ssh-key "~/.ssh/id_rsa"

# Custom model
./deploy-clawdbot-model.sh \
    --resource-group "mijn-resource-group" \
    --ai-service "mijn-ai-service" \
    --model "Mistral-7B-Instruct-v0.3" \
    --deployment "clawbot-mistral"
```

## 🤖 Premium Modellen (27 januari 2026)

**Nieuwste flagship modellen** voor productie ClawBot deployments:

### 🔥 Top Tier (Januari 2026)

| Model | Provider | Sterke Punten | Gebruik |
|-------|----------|---------------|---------|
| **gpt-5.2-turbo** 🚀 | OpenAI | Nieuwste GPT, ultra-fast, uitstekende instructievolging | **Default - Beste all-round** |
| gpt-5.2 💎 | OpenAI | Maximum capabilities, langste context | Ultra-premium taken |
| minimax-01 ⚡ | MiniMax | Multimodal powerhouse, Chinese AI excellence | Multimodal + competitieve pricing |
| claude-3.5-sonnet 🧠 | Anthropic | Beste reasoning en code generatie | Complexe logic & coding |
| gemini-2.0-ultra 🔮 | Google | Multimodal flagship, video/image understanding | Vision & multimodal taken |
| o1-pro 🎓 | OpenAI | Dedicated reasoning model, PhD-level | Research & complexe problemen |

### 🦙 Open Source Premium

| Model | Parameters | Sterke Punten | Gebruik |
|-------|-----------|---------------|---------|
| Meta-Llama-3.1-70B-Instruct | 70B | Open-source premium, excellent coding | Data privacy vereist |
| Meta-Llama-3.1-405B-Instruct | 405B | Grootste open-source model | Maximum open-source capability |
| Mixtral-8x22B-Instruct | 176B (8x22B) | Mixture of Experts, cost-efficient | High throughput |
| Mistral-Large-2407 | 123B | European flagship | EU compliance |

🚀 = **2026 Default - GPT-5.2 Turbo**
💎 = **Ultra-premium opties**
⚡ = **Multimodal specialist**

### Model Selectie Gids 2026

**Kies gpt-5.2-turbo als:** 🚀 **[RECOMMENDED DEFAULT]**
- Je de nieuwste OpenAI technologie wilt (januari 2026)
- Snelheid én kwaliteit belangrijk zijn
- Je een productie-ready chatbot wilt
- Uitstekende instructievolging nodig is
- **Beste all-round keuze voor ClawBot**

**Kies gpt-5.2 als:** 💎
- Je de absolute beste GPT capabilities wilt
- Langste context window nodig is
- Budget geen beperking is
- Maximum OpenAI performance vereist is

**Kies minimax-01 als:** ⚡
- Je multimodal capabilities nodig hebt
- Competitieve pricing belangrijk is
- Je nieuwste Chinese AI wilt proberen
- Image/video understanding vereist is

**Kies claude-3.5-sonnet als:** 🧠
- Code generatie de hoofdfocus is
- Beste reasoning capabilities nodig zijn
- Je lange, coherente outputs wilt
- Anthropic's safety focus belangrijk is

**Kies gemini-2.0-ultra als:** 🔮
- Vision/video understanding critical is
- Je Google's multimodal flagship wilt
- Integrated search capabilities handig zijn
- Gemini ecosystem voordelig is

**Kies o1-pro als:** 🎓
- Complexe reasoning problems centraal staan
- PhD-level analyse nodig is
- Research taken uitgevoerd moeten worden
- "Thinking" transparency belangrijk is

**Kies Meta-Llama-3.1-70B als:** 🦙
- Data privacy/on-premise deployment vereist is
- Open-source model gewenst is
- Je self-hosting wilt
- EU compliance belangrijk is

## 📖 Parameters

### Verplicht

| Parameter | Type | Beschrijving |
|-----------|------|--------------|
| `ResourceGroup` / `--resource-group` | String | Azure Resource Group naam |
| `AzureAiServicesName` / `--ai-service` | String | Azure AI Services instance naam |

### Optioneel - Model

| Parameter | Type | Default | Beschrijving |
|-----------|------|---------|--------------|
| `ModelName` / `--model` | String | `gpt-5.2-turbo` | Te deployen model (2026 default) |
| `DeploymentName` / `--deployment` | String | `clawbot-deployment` | Deployment naam in Azure |
| `ModelFormat` / `--model-format` | String | `OpenAI` | Model format type |
| `SkuName` / `--sku-name` | String | `Standard` | SKU naam voor deployment |
| `SkuCapacity` / `--sku-capacity` | Int | `2` | SKU capacity (premium default) |
| `Subscription` / `--subscription` | String | - | Azure Subscription ID |

### Optioneel - SSH

| Parameter | Type | Beschrijving |
|-----------|------|--------------|
| `SSHHost` / `--ssh-host` | String | SSH hostname of IP adres |
| `SSHUser` / `--ssh-user` | String | SSH gebruikersnaam |
| `SSHKeyPath` / `--ssh-key` | String | Pad naar SSH private key |

## 🔐 SSH Configuratie

Als je SSH parameters meegeeft, doet het script automatisch:

1. ✅ Maakt `~/clawbot/config/` directory aan op de SSH server
2. ✅ Genereert een `azure-ai.env` configuratie bestand
3. ✅ Upload het naar `~/clawbot/config/azure-ai.env`
4. ✅ Geeft instructies om ClawBot te starten

### ClawBot starten na deployment

```bash
# SSH naar je server
ssh admin@clawbot.example.com

# Laad de configuratie
source ~/clawbot/config/azure-ai.env

# Verifieer de configuratie
echo $AZURE_AI_ENDPOINT
echo $AZURE_AI_DEPLOYMENT

# Start ClawBot met Azure AI
./clawbot --azure-ai
```

## 🛠️ Handmatige ClawBot Configuratie

Als je SSH niet gebruikt, configureer ClawBot handmatig:

```bash
export AZURE_AI_ENDPOINT="https://jouw-ai-service.cognitiveservices.azure.com/"
export AZURE_AI_DEPLOYMENT="clawbot-deployment"
export AZURE_AI_MODEL="Phi-3.5-mini-instruct"
export AZURE_RESOURCE_GROUP="jouw-resource-group"
export AZURE_AI_SERVICES_NAME="jouw-ai-service"

# Haal API key op (eenmalig)
export AZURE_API_KEY=$(az cognitiveservices account keys list \
    --name jouw-ai-service \
    --resource-group jouw-resource-group \
    --query key1 -o tsv)

# Start ClawBot
./clawbot --azure-ai
```

## 📊 Deployment Verificatie

Na deployment kun je verifiëren met:

```bash
# Lijst alle deployments
azmcp foundry models deployments list \
    --azure-ai-services-name "jouw-ai-service" \
    --resource-group "jouw-resource-group"

# Test de endpoint (met curl)
curl -X POST "https://jouw-ai-service.cognitiveservices.azure.com/openai/deployments/clawbot-deployment/chat/completions?api-version=2024-02-01" \
    -H "Content-Type: application/json" \
    -H "api-key: $AZURE_API_KEY" \
    -d '{
        "messages": [{"role": "user", "content": "Hallo ClawBot!"}],
        "max_tokens": 100
    }'
```

## 🐛 Troubleshooting

### Model niet gevonden

**Probleem:** `Model 'X' not found in Azure AI Foundry catalog`

**Oplossing:**
```bash
# Lijst beschikbare modellen
azmcp foundry models list \
    --azure-ai-services-name "jouw-ai-service" \
    --resource-group "jouw-resource-group"

# Filter op naam
azmcp foundry models list \
    --azure-ai-services-name "jouw-ai-service" \
    --resource-group "jouw-resource-group" \
    --model-name "Phi"
```

### SSH Connection weigerd

**Probleem:** `Permission denied (publickey)`

**Oplossing:**
1. Verifieer SSH key permissions: `chmod 600 ~/.ssh/id_rsa`
2. Test SSH connectie: `ssh -i ~/.ssh/id_rsa admin@host`
3. Controleer SSH key op server: `cat ~/.ssh/authorized_keys`

### Deployment faalt

**Probleem:** `Deployment failed: Quota exceeded`

**Oplossing:**
1. Check quota: `az cognitiveservices account list-usage`
2. Verhoog quota in Azure Portal
3. Gebruik lagere SKU capacity: `--sku-capacity 1`

### Azure authenticatie fout

**Probleem:** `Authentication failed`

**Oplossing:**
```bash
# Login opnieuw
az login

# Selecteer juiste subscription
az account set --subscription "jouw-subscription-id"

# Verifieer toegang
az account show
```

## 📚 Azure AI Services Setup

Als je nog geen Azure AI Services hebt:

```bash
# Maak resource group
az group create \
    --name "clawbot-rg" \
    --location "westeurope"

# Maak AI Services account
az cognitiveservices account create \
    --name "clawbot-ai" \
    --resource-group "clawbot-rg" \
    --kind "AIServices" \
    --sku "S0" \
    --location "westeurope" \
    --yes

# Deploy model met script
./deploy-clawdbot-model.sh \
    --resource-group "clawbot-rg" \
    --ai-service "clawbot-ai"
```

## 🔄 Model Updates

Om een nieuw model te deployen:

```bash
# Deploy nieuw model met andere deployment naam
./deploy-clawdbot-model.sh \
    --resource-group "clawbot-rg" \
    --ai-service "clawbot-ai" \
    --model "Meta-Llama-3.1-8B-Instruct" \
    --deployment "clawbot-llama-v2"

# Update ClawBot config
export AZURE_AI_DEPLOYMENT="clawbot-llama-v2"

# Herstart ClawBot
./clawbot --azure-ai
```

## 💡 Tips & Best Practices 2026

1. **2026 Default:** GPT-5.2 Turbo is nu default - nieuwste OpenAI tech
2. **Multimodal?** Kies minimax-01 of gemini-2.0-ultra voor vision/video
3. **Coding focus?** Claude-3.5-sonnet is koning voor code generatie
4. **Reasoning?** o1-pro voor complexe logic en research
5. **Privacy?** Open-source Llama 3.1 voor data soevereiniteit
6. **Test eerst:** Deploy naar test resource group voor cost management
7. **Monitor kosten:** Nieuwe modellen zijn krachtiger maar ook duurder
8. **Right-size:** Start met gpt-5.2-turbo, upgrade naar gpt-5.2 als nodig
9. **Backup config:** Sla azure-ai.env op in version control (zonder keys!)
10. **Logs monitoren:** Check Azure Monitor voor deployment metrics en latency
11. **Roteer keys:** Regenereer API keys regelmatig voor security
12. **Compare eerst:** Test meerdere modellen voor je use case

## 🆘 Support

- Azure MCP Issues: https://github.com/Azure/azure-mcp/issues
- Azure AI Foundry Docs: https://learn.microsoft.com/azure/ai-studio/
- Model Catalog: https://ai.azure.com/explore/models

## 📄 Licentie

Deze scripts zijn onderdeel van het Azure MCP project (MIT License).

---

**Gemaakt:** 27 januari 2026
**Versie:** 1.0
**Compatibility:** Azure MCP Server 0.4.0+
