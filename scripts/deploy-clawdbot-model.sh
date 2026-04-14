#!/bin/bash
# ClawBot Model Deployer voor Azure AI Foundry
# Datum: 27-01-2026
# Deploy een actueel AI model voor ClawBot via SSH

set -e

# Kleuren voor output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

# Default waarden
MODEL_NAME="${MODEL_NAME:-gpt-5.2-turbo}"
DEPLOYMENT_NAME="${DEPLOYMENT_NAME:-clawbot-deployment}"
MODEL_FORMAT="${MODEL_FORMAT:-OpenAI}"
SKU_NAME="${SKU_NAME:-Standard}"
SKU_CAPACITY="${SKU_CAPACITY:-2}"

# Functie voor help
show_help() {
    echo -e "${CYAN}🤖 ClawBot Model Deployer - 27 januari 2026${NC}"
    echo "================================================================="
    echo ""
    echo "Gebruik: $0 [opties]"
    echo ""
    echo "Verplichte opties:"
    echo "  --resource-group <naam>          Azure Resource Group"
    echo "  --ai-service <naam>              Azure AI Services naam"
    echo ""
    echo "Optionele opties:"
    echo "  --model <naam>                   Model naam (default: Phi-3.5-mini-instruct)"
    echo "  --deployment <naam>              Deployment naam (default: clawbot-deployment)"
    echo "  --subscription <id>              Azure Subscription ID"
    echo "  --model-format <format>          Model format (default: OpenAI)"
    echo "  --sku-name <naam>                SKU naam (default: Standard)"
    echo "  --sku-capacity <aantal>          SKU capacity (default: 1)"
    echo "  --ssh-host <host>                SSH host voor ClawBot configuratie"
    echo "  --ssh-user <user>                SSH gebruikersnaam"
    echo "  --ssh-key <path>                 Pad naar SSH private key"
    echo "  -h, --help                       Toon deze help"
    echo ""
    echo "Premium modellen (januari 2026):"
    echo "  🚀 gpt-5.2-turbo                    OpenAI flagship [DEFAULT]"
    echo "  💎 gpt-5.2                          OpenAI ultra-premium"
    echo "  ⚡ minimax-01                        MiniMax multimodal powerhouse"
    echo "  🧠 claude-3.5-sonnet                Claude beste reasoning"
    echo "  🔮 gemini-2.0-ultra                 Google multimodal flagship"
    echo "  🎓 o1-pro                           OpenAI reasoning specialist"
    echo "  🦙 Meta-Llama-3.1-70B-Instruct      Open-source premium (70B)"
    echo "  🎯 Mixtral-8x22B-Instruct-v0.1      MoE powerhouse (176B)"
    echo ""
    echo "Voorbeeld:"
    echo "  $0 --resource-group myRG --ai-service myAI --ssh-host clawbot.example.com --ssh-user admin"
    exit 0
}

# Parse argumenten
while [[ $# -gt 0 ]]; do
    case $1 in
        --resource-group)
            RESOURCE_GROUP="$2"
            shift 2
            ;;
        --ai-service)
            AI_SERVICE_NAME="$2"
            shift 2
            ;;
        --model)
            MODEL_NAME="$2"
            shift 2
            ;;
        --deployment)
            DEPLOYMENT_NAME="$2"
            shift 2
            ;;
        --subscription)
            SUBSCRIPTION="$2"
            shift 2
            ;;
        --model-format)
            MODEL_FORMAT="$2"
            shift 2
            ;;
        --sku-name)
            SKU_NAME="$2"
            shift 2
            ;;
        --sku-capacity)
            SKU_CAPACITY="$2"
            shift 2
            ;;
        --ssh-host)
            SSH_HOST="$2"
            shift 2
            ;;
        --ssh-user)
            SSH_USER="$2"
            shift 2
            ;;
        --ssh-key)
            SSH_KEY_PATH="$2"
            shift 2
            ;;
        -h|--help)
            show_help
            ;;
        *)
            echo -e "${RED}❌ Onbekende optie: $1${NC}"
            show_help
            ;;
    esac
done

# Valideer verplichte parameters
if [[ -z "$RESOURCE_GROUP" ]] || [[ -z "$AI_SERVICE_NAME" ]]; then
    echo -e "${RED}❌ Resource Group en AI Service naam zijn verplicht!${NC}"
    show_help
fi

# Header
echo -e "${CYAN}🤖 ClawBot Model Deployer - 27 januari 2026${NC}"
echo "================================================================="

# Premium model info (2026 actueel)
declare -A RECOMMENDED_MODELS=(
    ["gpt-5.2-turbo"]="🚀 GPT-5.2 Turbo - OpenAI's nieuwste flagship (januari 2026) [DEFAULT]"
    ["gpt-5.2"]="💎 GPT-5.2 - OpenAI's ultra-premium met maximum capabilities"
    ["minimax-01"]="⚡ MiniMax-01 - Chinese AI powerhouse, excellent multimodal"
    ["claude-3.5-sonnet"]="🧠 Claude 3.5 Sonnet - Anthropic's beste reasoning model"
    ["gemini-2.0-ultra"]="🔮 Gemini 2.0 Ultra - Google's multimodal flagship"
    ["Meta-Llama-3.1-70B-Instruct"]="🦙 Meta Llama 3.1 70B - Open-source premium (70B)"
    ["Meta-Llama-3.1-405B-Instruct"]="🦙💎 Meta Llama 3.1 405B - Ultra open-source (405B)"
    ["Mixtral-8x22B-Instruct-v0.1"]="🎯 Mixtral 8x22B - Mixture of Experts (176B total)"
    ["Mistral-Large-2407"]="🇪🇺 Mistral Large - European flagship (123B)"
    ["o1-pro"]="🎓 o1-pro - OpenAI's reasoning specialist"
)

echo ""
echo -e "${GREEN}📋 Geselecteerd Model: ${MODEL_NAME}${NC}"
if [[ -n "${RECOMMENDED_MODELS[$MODEL_NAME]}" ]]; then
    echo -e "${GRAY}   Beschrijving: ${RECOMMENDED_MODELS[$MODEL_NAME]}${NC}"
fi

# Stap 1: Deploy model
echo ""
echo -e "${YELLOW}🚀 Stap 1: Model deployen naar Azure AI Foundry...${NC}"

DEPLOY_CMD="azmcp foundry models deploy"
DEPLOY_CMD="$DEPLOY_CMD --deployment-name \"$DEPLOYMENT_NAME\""
DEPLOY_CMD="$DEPLOY_CMD --model-name \"$MODEL_NAME\""
DEPLOY_CMD="$DEPLOY_CMD --model-format \"$MODEL_FORMAT\""
DEPLOY_CMD="$DEPLOY_CMD --azure-ai-services-name \"$AI_SERVICE_NAME\""
DEPLOY_CMD="$DEPLOY_CMD --resource-group \"$RESOURCE_GROUP\""
DEPLOY_CMD="$DEPLOY_CMD --sku-name \"$SKU_NAME\""
DEPLOY_CMD="$DEPLOY_CMD --sku-capacity $SKU_CAPACITY"
DEPLOY_CMD="$DEPLOY_CMD --scale-type \"standard\""

if [[ -n "$SUBSCRIPTION" ]]; then
    DEPLOY_CMD="$DEPLOY_CMD --subscription \"$SUBSCRIPTION\""
fi

echo -e "${GRAY}   Commando: $DEPLOY_CMD${NC}"

if eval "$DEPLOY_CMD"; then
    echo -e "${GREEN}   ✅ Model succesvol gedeployed!${NC}"
else
    echo -e "${RED}   ❌ Model deployment gefaald!${NC}"
    exit 1
fi

# Stap 2: Endpoint informatie
echo ""
echo -e "${YELLOW}📡 Stap 2: Deployment informatie ophalen...${NC}"

LIST_CMD="azmcp foundry models deployments list"
LIST_CMD="$LIST_CMD --azure-ai-services-name \"$AI_SERVICE_NAME\""
LIST_CMD="$LIST_CMD --resource-group \"$RESOURCE_GROUP\""

if [[ -n "$SUBSCRIPTION" ]]; then
    LIST_CMD="$LIST_CMD --subscription \"$SUBSCRIPTION\""
fi

if eval "$LIST_CMD"; then
    ENDPOINT="https://${AI_SERVICE_NAME}.cognitiveservices.azure.com/"
    echo ""
    echo -e "${CYAN}   🔗 Endpoint: ${ENDPOINT}${NC}"
    echo -e "${CYAN}   🎯 Deployment: ${DEPLOYMENT_NAME}${NC}"
else
    echo -e "${YELLOW}   ⚠️ Kon deployment info niet ophalen${NC}"
fi

# Stap 3: SSH configuratie
if [[ -n "$SSH_HOST" ]] && [[ -n "$SSH_USER" ]]; then
    echo ""
    echo -e "${YELLOW}🔐 Stap 3: ClawBot configureren via SSH...${NC}"

    # Maak configuratie bestand
    TEMP_CONFIG=$(mktemp)
    cat > "$TEMP_CONFIG" << EOF
# ClawBot Azure AI Foundry Configuratie
# Gegenereerd op: $(date '+%Y-%m-%d %H:%M:%S')

export AZURE_AI_ENDPOINT="${ENDPOINT:-https://${AI_SERVICE_NAME}.cognitiveservices.azure.com/}"
export AZURE_AI_DEPLOYMENT="$DEPLOYMENT_NAME"
export AZURE_AI_MODEL="$MODEL_NAME"
export AZURE_RESOURCE_GROUP="$RESOURCE_GROUP"
export AZURE_AI_SERVICES_NAME="$AI_SERVICE_NAME"

# Gebruik Azure CLI voor authenticatie
# az login
# export AZURE_API_KEY=\$(az cognitiveservices account keys list --name $AI_SERVICE_NAME --resource-group $RESOURCE_GROUP --query key1 -o tsv)
EOF

    echo -e "${GRAY}   📝 Configuratie bestand gemaakt: $TEMP_CONFIG${NC}"

    # SSH commando's
    SSH_TARGET="${SSH_USER}@${SSH_HOST}"
    REMOTE_CONFIG_PATH="~/clawbot/config/azure-ai.env"

    SSH_OPTS=""
    if [[ -n "$SSH_KEY_PATH" ]]; then
        SSH_OPTS="-i \"$SSH_KEY_PATH\""
    fi

    echo -e "${GRAY}   📤 Uploaden naar SSH: $SSH_TARGET${NC}"

    # Maak directory aan
    if eval "ssh $SSH_OPTS $SSH_TARGET 'mkdir -p ~/clawbot/config'"; then
        # Upload configuratie
        if eval "scp $SSH_OPTS \"$TEMP_CONFIG\" ${SSH_TARGET}:$REMOTE_CONFIG_PATH"; then
            echo -e "${GREEN}   ✅ Configuratie geüpload naar SSH server!${NC}"
            echo -e "${CYAN}   📍 Locatie: $REMOTE_CONFIG_PATH${NC}"
        else
            echo -e "${RED}   ❌ Upload gefaald!${NC}"
        fi
    else
        echo -e "${RED}   ❌ SSH directory aanmaken gefaald!${NC}"
    fi

    rm -f "$TEMP_CONFIG"

    # SSH instructies
    echo ""
    echo -e "${CYAN}📋 ClawBot starten op SSH:${NC}"
    echo -e "${GRAY}   1. SSH naar server: ssh $SSH_OPTS $SSH_TARGET${NC}"
    echo -e "${GRAY}   2. Source config: source ~/clawbot/config/azure-ai.env${NC}"
    echo -e "${GRAY}   3. Start ClawBot: ./clawbot --azure-ai${NC}"
else
    echo ""
    echo -e "${YELLOW}💡 Tip: Voeg --ssh-host, --ssh-user en optioneel --ssh-key toe voor automatische SSH configuratie${NC}"
fi

# Samenvatting
echo ""
echo "================================================================="
echo -e "${GREEN}✨ ClawBot Model Deployment Compleet!${NC}"
echo ""
echo -e "${CYAN}📊 Samenvatting:${NC}"
echo -e "   ${NC}Model: ${MODEL_NAME}"
echo -e "   ${NC}Deployment: ${DEPLOYMENT_NAME}"
echo -e "   ${NC}Endpoint: ${ENDPOINT:-https://${AI_SERVICE_NAME}.cognitiveservices.azure.com/}"
echo -e "   ${NC}Resource Group: ${RESOURCE_GROUP}"
echo -e "   ${NC}AI Service: ${AI_SERVICE_NAME}"

echo ""
echo -e "${CYAN}📚 Volgende stappen:${NC}"
echo -e "${GRAY}   1. Test de deployment: azmcp foundry models deployments list${NC}"
echo -e "${GRAY}   2. Configureer ClawBot met de endpoint en deployment naam${NC}"
echo -e "${GRAY}   3. Start ClawBot en test de AI capabilities${NC}"

echo ""
echo -e "${GREEN}🎉 Veel plezier met ClawBot! 🤖${NC}"
echo ""
