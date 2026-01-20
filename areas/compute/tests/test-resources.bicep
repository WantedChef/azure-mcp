targetScope = 'resourceGroup'

@minLength(3)
@maxLength(24)
@description('The base resource name.')
param baseName string = resourceGroup().name

@description('The location of the resource. By default, this is the same as the resource group.')
param location string = resourceGroup().location

@description('The tenant ID to which the application and resources belong.')
param tenantId string = '72f988bf-86f1-41af-91ab-2d7cd011db47'

@description('The client OID to grant access to test resources.')
param testApplicationOid string

// Create a Virtual Network for VM testing
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2024-01-01' = {
  name: '${baseName}-vnet'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '10.0.1.0/24'
          delegations: []
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
    ]
    enableDdosProtection: false
    enableVmProtection: false
  }
}

// Create a Network Security Group for VM testing
resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2024-01-01' = {
  name: '${baseName}-nsg'
  location: location
  properties: {
    securityRules: [
      {
        name: 'SSH'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '22'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1000
          direction: 'Inbound'
        }
      }
      {
        name: 'HTTP'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '80'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1001
          direction: 'Inbound'
        }
      }
      {
        name: 'HTTPS'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 1002
          direction: 'Inbound'
        }
      }
    ]
  }
}

// Create a Public IP address for VM testing (Dynamic, Basic SKU for cost savings)
resource publicIpAddress 'Microsoft.Network/publicIPAddresses@2024-01-01' = {
  name: '${baseName}-pip'
  location: location
  sku: {
    name: 'Basic'
    tier: 'Regional'
  }
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    publicIPAddressVersion: 'IPv4'
  }
}

// NOTE: We don't create an actual VM because:
// 1. VMs are expensive (even smallest VM costs ~$10-15/month)
// 2. Tests should focus on the API structure and template validation
// 3. Actual VM creation can be tested in integration tests with proper teardown
//
// The network infrastructure above is sufficient to validate:
// - VNet and Subnet creation
// - Network Security Group configuration
// - Public IP allocation
// - Template syntax and deployment
//
// If you need to test actual VM operations, you can:
// 1. Create a VM in the post-deployment script manually
// 2. Use a separate ARM template specifically for VM tests
// 3. Use Azure CLI/PowerShell in tests to create/deallocate VMs

// Virtual Machine Contributor role - for managing VMs
resource vmContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  // This is the Virtual Machine Contributor role.
  // Lets you manage virtual machines, but not access to them, and not the virtual network or storage account they're connected to.
  // See https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#virtual-machine-contributor
  name: '9980e02c-c2be-4d73-94e8-173b1dc7cf3c'
}

// Network Contributor role - for managing network resources
resource networkContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  // This is the Network Contributor role.
  // Lets you manage networks, but not access to them.
  // See https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#network-contributor
  name: '4d97b98b-1d4f-4787-a291-c67834d212e7'
}

// Assign the test application as VM Contributor on the resource group
resource appVmContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(vmContributorRoleDefinition.id, testApplicationOid, resourceGroup().id)
  scope: resourceGroup()
  properties: {
    principalId: testApplicationOid
    roleDefinitionId: vmContributorRoleDefinition.id
    description: 'VM Contributor for testApplicationOid'
  }
}

// Assign the test application as Network Contributor on the resource group
resource appNetworkContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(networkContributorRoleDefinition.id, testApplicationOid, resourceGroup().id)
  scope: resourceGroup()
  properties: {
    principalId: testApplicationOid
    roleDefinitionId: networkContributorRoleDefinition.id
    description: 'Network Contributor for testApplicationOid'
  }
}

// Output resource names and IDs for test consumption
output virtualNetworkName string = virtualNetwork.name
output virtualNetworkId string = virtualNetwork.id
output subnetName string = 'default'
output subnetId string = '${virtualNetwork.id}/subnets/default'
output networkSecurityGroupName string = networkSecurityGroup.name
output networkSecurityGroupId string = networkSecurityGroup.id
output publicIpAddressName string = publicIpAddress.name
output publicIpAddressId string = publicIpAddress.id
output resourceGroupName string = resourceGroup().name
output location string = location
