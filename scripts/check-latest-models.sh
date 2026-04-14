#!/bin/bash
# Check actuele beschikbare modellen in Azure AI Foundry
# Datum: 27-01-2026

set -e

# Kleuren
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
GRAY='\033[0;90m'
NC='\033[0m'

echo -e "${CYAN}🔍 Azure AI Foundry - Actuele Modellen Checker${NC}"
echo "================================================================"
echo ""

echo -e "${YELLOW}📋 Ophalen van ALLE beschikbare modellen...${NC}"
echo ""

# Check azmcp
if ! command -v azmcp &> /dev/null; then
    echo -e "${RED}❌ azmcp niet gevonden!${NC}"
    echo -e "${GRAY}   Installeer eerst Azure MCP Server${NC}"
    exit 1
fi

# Alle modellen ophalen
echo -e "${CYAN}🌐 Alle modellen ophalen...${NC}"
if azmcp foundry models list &> /tmp/azure-models.json; then
    echo -e "${GREEN}✅ Modellen opgehaald!${NC}"
    echo ""
else
    echo -e "${RED}❌ Fout bij ophalen van modellen${NC}"
    echo ""
    echo -e "${YELLOW}💡 Zorg ervoor dat:${NC}"
    echo -e "${GRAY}   1. Je bent ingelogd in Azure (az login)${NC}"
    echo -e "${GRAY}   2. Je toegang hebt tot Azure AI Foundry${NC}"
    exit 1
fi

# Top providers
echo -e "${MAGENTA}🏢 Top nieuwste modellen per provider:${NC}"
echo ""

declare -A providers=(
    ["OpenAI"]="gpt"
    ["Anthropic"]="claude"
    ["Google"]="gemini"
    ["Meta"]="llama"
    ["Mistral"]="mistral"
    ["Microsoft"]="phi"
    ["Cohere"]="command"
)

for provider in "${!providers[@]}"; do
    model_filter="${providers[$provider]}"
    echo -e "  ${CYAN}📦 $provider:${NC}"

    if azmcp foundry models list --publisher-name "$provider" --model-name "$model_filter" &> /dev/null; then
        echo -e "     ${GREEN}✅ Modellen gevonden${NC}"
    else
        echo -e "     ${YELLOW}⚠️ Geen modellen of geen toegang${NC}"
    fi
done

echo ""
echo "================================================================"
echo ""

# Nieuwste modellen zoeken
echo -e "${YELLOW}🔥 Zoeken naar NIEUWSTE modellen (2025/2026)...${NC}"
echo ""

searches=(
    "GPT-5:gpt-5"
    "GPT-4.5:gpt-4.5"
    "GPT-4o:gpt-4o"
    "Claude 3.5/4:claude"
    "Gemini 2.0:gemini-2"
    "Llama 3.2/4:llama"
    "Mistral Large 2:mistral-large"
    "Phi-4:phi-4"
)

for search in "${searches[@]}"; do
    IFS=':' read -r name filter <<< "$search"
    echo -ne "  ${CYAN}🔎 $name...${NC}"

    if azmcp foundry models list --model-name "$filter" 2>&1 | grep -q "models"; then
        echo -e " ${GREEN}✅ GEVONDEN!${NC}"
    else
        echo -e " ${RED}❌ Niet beschikbaar${NC}"
    fi
done

echo ""
echo "================================================================"
echo ""

echo -e "${CYAN}💡 Voor complete lijst met details:${NC}"
echo -e "${GRAY}   azmcp foundry models list${NC}"
echo ""
echo -e "${CYAN}💡 Filter op publisher:${NC}"
echo -e "${GRAY}   azmcp foundry models list --publisher-name \"OpenAI\"${NC}"
echo ""
echo -e "${CYAN}💡 Filter op model naam:${NC}"
echo -e "${GRAY}   azmcp foundry models list --model-name \"gpt\"${NC}"
echo ""

echo -e "${GREEN}🎯 Welk model wil je gebruiken voor ClawBot?${NC}"
echo ""
