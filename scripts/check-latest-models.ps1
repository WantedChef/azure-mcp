#!/usr/bin/env pwsh
# Check actuele beschikbare modellen in Azure AI Foundry
# Datum: 27-01-2026

Write-Host "🔍 Azure AI Foundry - Actuele Modellen Checker" -ForegroundColor Cyan
Write-Host "=" -NoNewline; 1..60 | ForEach-Object { Write-Host "=" -NoNewline }; Write-Host ""
Write-Host ""

Write-Host "📋 Ophalen van ALLE beschikbare modellen..." -ForegroundColor Yellow
Write-Host ""

# Check of azmcp beschikbaar is
if (-not (Get-Command "azmcp" -ErrorAction SilentlyContinue)) {
    Write-Host "❌ azmcp niet gevonden!" -ForegroundColor Red
    Write-Host "   Installeer eerst Azure MCP Server" -ForegroundColor Gray
    exit 1
}

# Functie om modellen te filteren op jaar/datum
function Get-LatestModels {
    param($allModels)

    # Filter op recente modellen (2025/2026 in naam, of nieuwste versies)
    $latestKeywords = @(
        "2026", "2025",
        "gpt-5", "gpt-4.5", "gpt-4o",
        "claude-3.5", "claude-4",
        "gemini-2", "gemini-ultra",
        "llama-3.2", "llama-3.1", "llama-4",
        "mistral-large-2", "mixtral-8x22b",
        "phi-4", "phi-3.5"
    )

    return $allModels | Where-Object {
        $modelName = $_.Name.ToLower()
        $latestKeywords | ForEach-Object {
            if ($modelName -like "*$($_.ToLower())*") { return $true }
        }
    }
}

# Haal alle modellen op
Write-Host "🌐 Alle modellen ophalen..." -ForegroundColor Cyan
try {
    $allModelsJson = azmcp foundry models list 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Fout bij ophalen van modellen" -ForegroundColor Red
        Write-Host $allModelsJson
        Write-Host ""
        Write-Host "💡 Zorg ervoor dat:" -ForegroundColor Yellow
        Write-Host "   1. Je bent ingelogd in Azure (az login)" -ForegroundColor Gray
        Write-Host "   2. Je toegang hebt tot Azure AI Foundry" -ForegroundColor Gray
        exit 1
    }

    Write-Host "✅ Modellen opgehaald!" -ForegroundColor Green
    Write-Host ""

} catch {
    Write-Host "❌ Fout: $_" -ForegroundColor Red
    exit 1
}

# Top providers
Write-Host "🏢 Top nieuwste modellen per provider:" -ForegroundColor Magenta
Write-Host ""

$providers = @{
    "OpenAI" = "gpt"
    "Anthropic" = "claude"
    "Google" = "gemini"
    "Meta" = "llama"
    "Mistral" = "mistral"
    "Microsoft" = "phi"
    "Cohere" = "command"
}

foreach ($provider in $providers.GetEnumerator()) {
    Write-Host "  📦 $($provider.Key):" -ForegroundColor Cyan

    $cmd = "azmcp foundry models list --publisher-name `"$($provider.Key)`" --model-name `"$($provider.Value)`""

    try {
        $result = Invoke-Expression $cmd 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "     ✅ Modellen gevonden" -ForegroundColor Green
        } else {
            Write-Host "     ⚠️ Geen modellen of geen toegang" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "     ❌ Fout bij ophalen" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=" -NoNewline; 1..60 | ForEach-Object { Write-Host "=" -NoNewline }; Write-Host ""
Write-Host ""

# Nieuwste modellen zoeken
Write-Host "🔥 Zoeken naar NIEUWSTE modellen (2025/2026)..." -ForegroundColor Yellow
Write-Host ""

$searches = @(
    @{Name="GPT-5"; Filter="gpt-5"},
    @{Name="GPT-4.5"; Filter="gpt-4.5"},
    @{Name="GPT-4o"; Filter="gpt-4o"},
    @{Name="Claude 3.5/4"; Filter="claude"},
    @{Name="Gemini 2.0"; Filter="gemini-2"},
    @{Name="Llama 3.2/4"; Filter="llama"},
    @{Name="Mistral Large 2"; Filter="mistral-large"},
    @{Name="Phi-4"; Filter="phi-4"}
)

foreach ($search in $searches) {
    Write-Host "  🔎 $($search.Name)..." -NoNewline

    $cmd = "azmcp foundry models list --model-name `"$($search.Filter)`""
    $result = Invoke-Expression $cmd 2>&1

    if ($LASTEXITCODE -eq 0 -and $result -notlike "*no models found*") {
        Write-Host " ✅ GEVONDEN!" -ForegroundColor Green
    } else {
        Write-Host " ❌ Niet beschikbaar" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=" -NoNewline; 1..60 | ForEach-Object { Write-Host "=" -NoNewline }; Write-Host ""
Write-Host ""

Write-Host "💡 Voor complete lijst met details:" -ForegroundColor Cyan
Write-Host "   azmcp foundry models list" -ForegroundColor Gray
Write-Host ""
Write-Host "💡 Filter op publisher:" -ForegroundColor Cyan
Write-Host "   azmcp foundry models list --publisher-name `"OpenAI`"" -ForegroundColor Gray
Write-Host ""
Write-Host "💡 Filter op model naam:" -ForegroundColor Cyan
Write-Host "   azmcp foundry models list --model-name `"gpt`"" -ForegroundColor Gray
Write-Host ""

Write-Host "🎯 Welk model wil je gebruiken voor ClawBot?" -ForegroundColor Green
Write-Host ""
