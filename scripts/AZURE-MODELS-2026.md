# Azure AI Foundry - Beschikbare Modellen (Januari 2026)

**Verificatie datum:** 27 januari 2026
**Bron:** Azure AI Foundry Model Catalog

## 🎯 Premium Modellen (Aangeraden voor ClawBot)

### Azure OpenAI Service

| Model | Beschrijving | Best voor |
|-------|--------------|-----------|
| **gpt-4o** | Nieuwste GPT-4 Omni, multimodal | **All-round beste keuze** |
| **gpt-4o-mini** | Snellere, goedkopere gpt-4o | Cost-efficient productie |
| **o1-preview** | Reasoning model, diep nadenken | Complexe problemen |
| **o1-mini** | Sneller reasoning model | STEM taken, coding |
| gpt-4-turbo | Grote context (128k tokens) | Lange documenten |
| gpt-4 | Klassieke GPT-4 | Stabiel, betrouwbaar |
| gpt-3.5-turbo | Budget optie | Dev/test |

### Meta Llama 3.1 (Open Source)

| Model | Parameters | Best voor |
|-------|-----------|-----------|
| **Llama-3.1-405B-Instruct** | 405B | Grootste open-source, ultra premium |
| **Llama-3.1-70B-Instruct** | 70B | Premium open-source, uitstekend |
| Llama-3.1-8B-Instruct | 8B | Budget open-source |

### Mistral AI

| Model | Beschrijving | Best voor |
|-------|--------------|-----------|
| **Mistral-Large-2407** | Flagship Europees model | EU deployment, multilingual |
| **Mixtral-8x7B-Instruct** | Mixture of Experts | Cost-efficient power |
| Mistral-Small | Compact maar krachtig | Budget deployments |

### Microsoft Phi-3

| Model | Parameters | Best voor |
|-------|-----------|-----------|
| Phi-3-medium-128k-instruct | 14B | Groot context window |
| Phi-3-small-128k-instruct | 7B | Balans speed/quality |
| Phi-3-mini-128k-instruct | 3.8B | Snelste, low latency |

### Andere Premium Providers

| Model | Provider | Sterke Punten |
|-------|----------|---------------|
| Cohere Command R+ | Cohere | Enterprise RAG specialist |
| Gemma 2 (9B/27B) | Google | Lightweight, efficient |
| Nemotron-4 340B | NVIDIA | NVIDIA geoptimaliseerd |

## 🚀 Aanbeveling voor ClawBot (27-01-2026)

### 🥇 Top 3 All-Round:

1. **gpt-4o** - Beste balans kwaliteit/snelheid/features
2. **Llama-3.1-70B-Instruct** - Beste open-source optie
3. **Mistral-Large-2407** - Beste Europese optie

### 💰 Top 3 Cost-Efficient:

1. **gpt-4o-mini** - Snelle OpenAI, goed geprijsd
2. **Llama-3.1-8B-Instruct** - Budget open-source
3. **Phi-3-small-128k-instruct** - Microsoft budget

### 🧠 Top 3 Reasoning:

1. **o1-preview** - Beste reasoning capabilities
2. **Llama-3.1-405B-Instruct** - Ultra large open-source
3. **gpt-4o** - Uitstekende reasoning + multimodal

### 🎨 Top 3 Multimodal:

1. **gpt-4o** - Vision, audio, text
2. **gpt-4-turbo** - Vision + text
3. **Gemma 2** - Lightweight multimodal

## ⚠️ Belangrijke Opmerkingen

### Modellen die NIET beschikbaar zijn via Azure AI Foundry:
- ❌ GPT-5.x (bestaat niet, nog niet uitgebracht)
- ❌ Claude 3.5 Sonnet (alleen via Anthropic direct, niet Azure)
- ❌ Gemini 2.0 Ultra (alleen via Google Cloud)
- ❌ MiniMax-01 (alleen via MiniMax direct, niet Azure)

### Via Azure OpenAI Service:
Deze modellen zijn beschikbaar via **Azure OpenAI Service** (niet AI Foundry):
- GPT-4o, GPT-4o-mini
- o1-preview, o1-mini
- GPT-4-turbo, GPT-4
- GPT-3.5-turbo

**Deployment:**
```bash
# Azure OpenAI (andere deployment methode)
az cognitiveservices account deployment create \
  --name <ai-services-name> \
  --resource-group <rg> \
  --deployment-name clawbot \
  --model-name gpt-4o \
  --model-version "2024-05-13" \
  --model-format OpenAI \
  --sku-capacity 1 \
  --sku-name Standard
```

### Via Azure AI Foundry (Model Catalog):
Open-source en partner modellen:
- Meta Llama 3.1 (alle varianten)
- Mistral (alle varianten)
- Phi-3 (alle varianten)
- Cohere, NVIDIA, etc.

## 📊 Specificaties Vergelijking

### Context Window Sizes:

| Model | Context Window |
|-------|----------------|
| gpt-4-turbo | 128k tokens |
| Phi-3-*-128k | 128k tokens |
| gpt-4o | 128k tokens |
| Llama-3.1-* | 128k tokens |
| Mistral-Large | 128k tokens |
| gpt-3.5-turbo | 16k tokens |

### Multimodal Capabilities:

| Model | Vision | Audio | Video |
|-------|--------|-------|-------|
| gpt-4o | ✅ | ✅ | ❌ |
| gpt-4-turbo | ✅ | ❌ | ❌ |
| Gemma 2 | ✅ | ❌ | ❌ |
| Llama-3.1 | ❌ | ❌ | ❌ |
| Mistral | ❌ | ❌ | ❌ |

## 🎯 Welke Moet Ik Kiezen?

### Voor ClawBot Chatbot:

**Productie (Premium):**
```bash
--model "gpt-4o"              # Beste all-round, multimodal
```

**Productie (Cost-Efficient):**
```bash
--model "gpt-4o-mini"         # Snelle OpenAI
```

**Open-Source (Privacy):**
```bash
--model "Llama-3.1-70B-Instruct"  # Premium open-source
```

**EU Compliance:**
```bash
--model "Mistral-Large-2407"  # European flagship
```

**Reasoning Focus:**
```bash
--model "o1-preview"          # Beste reasoning
```

**Budget (Test):**
```bash
--model "Phi-3-mini-128k-instruct"  # Snel & goedkoop
```

## 🔍 Model Beschikbaarheid Checken

```bash
# Alle modellen
azmcp foundry models list

# Specifieke publisher
azmcp foundry models list --publisher-name "Meta"

# Gratis playground
azmcp foundry models list --search-for-free-playground

# Filter op naam
azmcp foundry models list --model-name "llama"
```

## 💡 Let Op!

1. **Azure OpenAI vs AI Foundry** zijn verschillende services
2. **Niet alle modellen** zijn in alle Azure regio's beschikbaar
3. **Quota limits** kunnen van toepassing zijn
4. **Pricing** verschilt per model en deployment type
5. **Model versies** kunnen updates krijgen (bijv. gpt-4o-2024-05-13)

## 📚 Bronnen

- Azure AI Foundry: https://ai.azure.com/
- Azure OpenAI Service: https://learn.microsoft.com/azure/ai-services/openai/
- Model Catalog: 1900+ modellen beschikbaar
- Updates: Check monthly voor nieuwe modellen

---

**Vraag:** Welk model wil je deployen voor ClawBot?

Kies één van de modellen hierboven en ik update de deployment scripts.
