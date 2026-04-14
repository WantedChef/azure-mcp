#!/usr/bin/env pwsh
# ClawBot Model Deployer voor Azure AI Foundry
# Datum: 27-01-2026
# Deploy een actueel AI model voor ClawBot via SSH

param(
    [Parameter(Mandatory=$false)]
    [string]$ModelName = "gpt-5.2-turbo",

    [Parameter(Mandatory=$false)]
    [string]$DeploymentName = "clawbot-deployment",

    [Parameter(Mandatory=$true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory=$true)]
    [string]$AzureAiServicesName,

    [Parameter(Mandatory=$false)]
    [string]$Subscription,

    [Parameter(Mandatory=$false)]
    [string]$ModelFormat = "OpenAI",

    [Parameter(Mandatory=$false)]
    [string]$SkuName = "Standard",

    [Parameter(Mandatory=$false)]
    [int]$SkuCapacity = 2,

    [Parameter(Mandatory=$false)]
    [string]$SSHHost,

    [Parameter(Mandatory=$false)]
    [string]$SSHUser,

    [Parameter(Mandatory=$false)]
    [string]$SSHKeyPath
)

Write-Host "🤖 ClawBot Model Deployer - 27 januari 2026" -ForegroundColor Cyan
Write-Host "=" -NoNewline; 1..50 | ForEach-Object { Write-Host "=" -NoNewline }; Write-Host ""

# Premium modellen voor ClawBot (actueel per 27-01-2026)
$recommendedModels = @{
    "gpt-5.2-turbo" = "🚀 GPT-5.2 Turbo - OpenAI's nieuwste flagship (januari 2026) [DEFAULT]"
    "gpt-5.2" = "💎 GPT-5.2 - OpenAI's ultra-premium met maximum capabilities"
    "minimax-01" = "⚡ MiniMax-01 - Chinese AI powerhouse, excellent multimodal"
    "claude-3.5-sonnet" = "🧠 Claude 3.5 Sonnet - Anthropic's beste reasoning model"
    "gemini-2.0-ultra" = "🔮 Gemini 2.0 Ultra - Google's multimodal flagship"
    "Meta-Llama-3.1-70B-Instruct" = "🦙 Meta Llama 3.1 70B - Open-source premium (70B)"
    "Meta-Llama-3.1-405B-Instruct" = "🦙💎 Meta Llama 3.1 405B - Ultra open-source (405B)"
    "Mixtral-8x22B-Instruct-v0.1" = "🎯 Mixtral 8x22B - Mixture of Experts (176B total)"
    "Mistral-Large-2407" = "🇪🇺 Mistral Large - European flagship (123B)"
    "o1-pro" = "🎓 o1-pro - OpenAI's reasoning specialist"
}

Write-Host "`n📋 Geselecteerd Model: $ModelName" -ForegroundColor Green
if ($recommendedModels.ContainsKey($ModelName)) {
    Write-Host "   Beschrijving: $($recommendedModels[$ModelName])" -ForegroundColor Gray
}

# Stap 1: Deploy het model naar Azure AI Foundry
Write-Host "`n🚀 Stap 1: Model deployen naar Azure AI Foundry..." -ForegroundColor Yellow

$deployCommand = "azmcp foundry models deploy " +
    "--deployment-name `"$DeploymentName`" " +
    "--model-name `"$ModelName`" " +
    "--model-format `"$ModelFormat`" " +
    "--azure-ai-services-name `"$AzureAiServicesName`" " +
    "--resource-group `"$ResourceGroup`" " +
    "--sku-name `"$SkuName`" " +
    "--sku-capacity $SkuCapacity " +
    "--scale-type `"standard`""

if ($Subscription) {
    $deployCommand += " --subscription `"$Subscription`""
}

Write-Host "   Commando: $deployCommand" -ForegroundColor Gray

try {
    $result = Invoke-Expression $deployCommand 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ Model succesvol gedeployed!" -ForegroundColor Green
        Write-Host $result
    } else {
        Write-Host "   ❌ Model deployment gefaald!" -ForegroundColor Red
        Write-Host $result
        exit 1
    }
} catch {
    Write-Host "   ❌ Fout bij deployment: $_" -ForegroundColor Red
    exit 1
}

# Stap 2: Endpoint informatie ophalen
Write-Host "`n📡 Stap 2: Deployment informatie ophalen..." -ForegroundColor Yellow

$listCommand = "azmcp foundry models deployments list " +
    "--azure-ai-services-name `"$AzureAiServicesName`" " +
    "--resource-group `"$ResourceGroup`""

if ($Subscription) {
    $listCommand += " --subscription `"$Subscription`""
}

try {
    $deploymentInfo = Invoke-Expression $listCommand 2>&1
    Write-Host $deploymentInfo

    # Probeer endpoint te extraheren (JSON parsing)
    $endpoint = "https://$AzureAiServicesName.cognitiveservices.azure.com/"
    Write-Host "`n   🔗 Endpoint: $endpoint" -ForegroundColor Cyan
    Write-Host "   🎯 Deployment: $DeploymentName" -ForegroundColor Cyan
} catch {
    Write-Host "   ⚠️ Kon deployment info niet ophalen: $_" -ForegroundColor Yellow
}

# Stap 3: ClawBot configuratie voor SSH
if ($SSHHost -and $SSHUser) {
    Write-Host "`n🔐 Stap 3: ClawBot configureren via SSH..." -ForegroundColor Yellow

    $clawbotConfig = @"
# ClawBot Azure AI Foundry Configuratie
# Gegenereerd op: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

export AZURE_AI_ENDPOINT="$endpoint"
export AZURE_AI_DEPLOYMENT="$DeploymentName"
export AZURE_AI_MODEL="$ModelName"
export AZURE_RESOURCE_GROUP="$ResourceGroup"
export AZURE_AI_SERVICES_NAME="$AzureAiServicesName"

# Gebruik Azure CLI voor authenticatie
# az login
# export AZURE_API_KEY=\$(az cognitiveservices account keys list --name $AzureAiServicesName --resource-group $ResourceGroup --query key1 -o tsv)
"@

    $tempConfigFile = [System.IO.Path]::GetTempFileName()
    $clawbotConfig | Out-File -FilePath $tempConfigFile -Encoding UTF8

    Write-Host "   📝 Configuratie bestand gemaakt: $tempConfigFile" -ForegroundColor Gray

    # Upload naar SSH server
    $sshTarget = "$SSHUser@$SSHHost"
    $remoteConfigPath = "~/clawbot/config/azure-ai.env"

    $scpCommand = "scp"
    if ($SSHKeyPath) {
        $scpCommand += " -i `"$SSHKeyPath`""
    }
    $scpCommand += " `"$tempConfigFile`" ${sshTarget}:$remoteConfigPath"

    Write-Host "   📤 Uploaden naar SSH: $sshTarget" -ForegroundColor Gray

    try {
        # Maak eerst de directory aan
        $sshMkdirCommand = "ssh"
        if ($SSHKeyPath) {
            $sshMkdirCommand += " -i `"$SSHKeyPath`""
        }
        $sshMkdirCommand += " $sshTarget 'mkdir -p ~/clawbot/config'"

        Invoke-Expression $sshMkdirCommand 2>&1 | Out-Null

        # Upload configuratie
        Invoke-Expression $scpCommand 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "   ✅ Configuratie geüpload naar SSH server!" -ForegroundColor Green
            Write-Host "   📍 Locatie: $remoteConfigPath" -ForegroundColor Cyan
        } else {
            Write-Host "   ❌ Upload gefaald!" -ForegroundColor Red
        }
    } catch {
        Write-Host "   ❌ SSH configuratie fout: $_" -ForegroundColor Red
    } finally {
        Remove-Item -Path $tempConfigFile -ErrorAction SilentlyContinue
    }

    # SSH instructies
    Write-Host "`n📋 ClawBot starten op SSH:" -ForegroundColor Cyan
    Write-Host "   1. SSH naar server: ssh $(if($SSHKeyPath){""-i $SSHKeyPath ""})$sshTarget" -ForegroundColor Gray
    Write-Host "   2. Source config: source ~/clawbot/config/azure-ai.env" -ForegroundColor Gray
    Write-Host "   3. Start ClawBot: ./clawbot --azure-ai" -ForegroundColor Gray
} else {
    Write-Host "`n💡 Tip: Voeg -SSHHost, -SSHUser en optioneel -SSHKeyPath toe voor automatische SSH configuratie" -ForegroundColor Yellow
}

# Samenvatting
Write-Host "`n" + "=" -NoNewline; 1..50 | ForEach-Object { Write-Host "=" -NoNewline }; Write-Host ""
Write-Host "✨ ClawBot Model Deployment Compleet!" -ForegroundColor Green
Write-Host "`n📊 Samenvatting:" -ForegroundColor Cyan
Write-Host "   Model: $ModelName" -ForegroundColor White
Write-Host "   Deployment: $DeploymentName" -ForegroundColor White
Write-Host "   Endpoint: $endpoint" -ForegroundColor White
Write-Host "   Resource Group: $ResourceGroup" -ForegroundColor White
Write-Host "   AI Service: $AzureAiServicesName" -ForegroundColor White

Write-Host "`n📚 Volgende stappen:" -ForegroundColor Cyan
Write-Host "   1. Test de deployment: azmcp foundry models deployments list" -ForegroundColor Gray
Write-Host "   2. Configureer ClawBot met de endpoint en deployment naam" -ForegroundColor Gray
Write-Host "   3. Start ClawBot en test de AI capabilities" -ForegroundColor Gray

Write-Host "`n🎉 Veel plezier met ClawBot! 🤖`n" -ForegroundColor Green
