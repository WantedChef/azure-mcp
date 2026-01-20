param(
    [string] $TenantId,
    [string] $TestApplicationId,
    [string] $ResourceGroupName,
    [string] $BaseName,
    [hashtable] $DeploymentOutputs
)

$ErrorActionPreference = "Stop"

. "$PSScriptRoot/../../../eng/common/scripts/common.ps1"
. "$PSScriptRoot/../../../eng/scripts/helpers/TestResourcesHelpers.ps1"

$testSettings = New-TestSettings @PSBoundParameters -OutputPath $PSScriptRoot

# NOTE: This is a baseline post-deployment script for Compute testing.
#
# Unlike other areas (Storage, KeyVault, etc.) that create actual resources in
# the Bicep template, Compute focuses on network infrastructure to avoid
# expensive VM deployment costs.
#
# If you need to test actual VM operations, you can:
# 1. Create a VM here using Azure PowerShell (with proper cleanup logic)
# 2. Use a separate test template specifically for VM tests
# 3. Create VMs in individual tests with automatic deallocation/deletion
#
# Example VM creation (commented out - requires careful cost management):
#
# Write-Host "Compute test-resources-post.ps1: VM creation is disabled to avoid costs" -ForegroundColor Yellow
# Write-Host "To enable VM testing, uncomment the code below and ensure proper cleanup" -ForegroundColor Yellow
#
# $vmName = "${BaseName}-testvm"
# $location = $DeploymentOutputs['location']
# $vnetName = $DeploymentOutputs['virtualNetworkName']
# $subnetName = $DeploymentOutputs['subnetName']
# $nsgName = $DeploymentOutputs['networkSecurityGroupName']
# $pipName = $DeploymentOutputs['publicIpAddressName']
#
# Write-Host "Would create VM: $vmName in location: $location" -ForegroundColor Cyan
# Write-Host "Network: VNet=$vnetName, Subnet=$subnetName, NSG=$nsgName, PIP=$pipName" -ForegroundColor Cyan
#
# # Actual VM creation would look like this:
# # $cred = Get-Credential
# # New-AzVM -ResourceGroupName $ResourceGroupName -Name $vmName -Location $location `
# #     -VirtualNetworkName $vnetName -SubnetName $subnetName `
# #     -SecurityGroupName $nsgName -PublicIpAddressName $pipName `
# #     -Credential $cred -OpenPorts 22,80,443

Write-Host "Compute test infrastructure deployed successfully" -ForegroundColor Green
Write-Host "Network resources available for VM testing:" -ForegroundColor Cyan
Write-Host "  - Virtual Network: $($DeploymentOutputs['virtualNetworkName'])" -ForegroundColor Cyan
Write-Host "  - Subnet: $($DeploymentOutputs['subnetName'])" -ForegroundColor Cyan
Write-Host "  - Network Security Group: $($DeploymentOutputs['networkSecurityGroupName'])" -ForegroundColor Cyan
Write-Host "  - Public IP: $($DeploymentOutputs['publicIpAddressName'])" -ForegroundColor Cyan
