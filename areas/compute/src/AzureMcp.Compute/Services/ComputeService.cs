// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Resources;
using AzureMcp.Compute.Models;
using AzureMcp.Core.Options;
using AzureMcp.Core.Services.Azure;
using AzureMcp.Core.Services.Azure.Subscription;
using AzureMcp.Core.Services.Azure.Tenant;
using AzureMcp.Core.Services.Caching;

namespace AzureMcp.Compute.Services;

public sealed class ComputeService(
    ISubscriptionService subscriptionService,
    ITenantService tenantService,
    ICacheService cacheService) : BaseAzureService(tenantService), IComputeService
{
    private readonly ISubscriptionService _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

    private const string CacheGroup = "compute";
    private const string VmsCacheKey = "vms";
    private static readonly TimeSpan s_cacheDuration = TimeSpan.FromMinutes(15);

    public async Task<List<VirtualMachine>> ListVirtualMachines(
        string subscription,
        string? resourceGroup = null,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription);

        var cacheKey = string.IsNullOrEmpty(tenant)
            ? $"{VmsCacheKey}_{subscription}_{resourceGroup ?? "all"}"
            : $"{VmsCacheKey}_{subscription}_{resourceGroup ?? "all"}_{tenant}";

        var cachedVms = await _cacheService.GetAsync<List<VirtualMachine>>(CacheGroup, cacheKey, s_cacheDuration);
        if (cachedVms != null)
        {
            return cachedVms;
        }

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);
        var vms = new List<VirtualMachine>();

        try
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                await foreach (var vm in subscriptionResource.GetVirtualMachinesAsync())
                {
                    if (vm?.Data != null)
                    {
                        vms.Add(await ConvertToVmModelAsync(vm));
                    }
                }
            }
            else
            {
                var rgResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
                if (rgResource?.Value != null)
                {
                    await foreach (var vm in rgResource.Value.GetVirtualMachinesAsync())
                    {
                        if (vm?.Data != null)
                        {
                            vms.Add(await ConvertToVmModelAsync(vm));
                        }
                    }
                }
            }

            await _cacheService.SetAsync(CacheGroup, cacheKey, vms, s_cacheDuration);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving Virtual Machines: {ex.Message}", ex);
        }

        return vms;
    }

    public async Task<VirtualMachine?> GetVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var cacheKey = string.IsNullOrEmpty(tenant)
            ? $"vm_{subscription}_{resourceGroup}_{vmName}"
            : $"vm_{subscription}_{resourceGroup}_{vmName}_{tenant}";

        var cachedVm = await _cacheService.GetAsync<VirtualMachine>(CacheGroup, cacheKey, s_cacheDuration);
        if (cachedVm != null)
        {
            return cachedVm;
        }

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                return null;
            }

            var vmResource = await resourceGroupResource.Value.GetVirtualMachineAsync(vmName);
            if (vmResource?.Value?.Data == null)
            {
                return null;
            }

            var vm = await ConvertToVmModelAsync(vmResource.Value);
            await _cacheService.SetAsync(CacheGroup, cacheKey, vm, s_cacheDuration);

            return vm;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<VirtualMachine?> CreateVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string location,
        string size,
        string image,
        string adminUsername,
        string? sshPublicKeyPath = null,
        bool? generateSshKeys = null,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup, location, size, image, adminUsername);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                throw new Exception($"Resource group '{resourceGroup}' not found.");
            }

            // Parse image URN (Publisher:Offer:Sku:Version)
            var imageParts = image.Split(':');
            if (imageParts.Length != 4)
            {
                throw new Exception("Invalid image format. Expected: Publisher:Offer:Sku:Version");
            }

            // Create VM parameters
            var vmData = new VirtualMachineData
            {
                Location = location,
                AdminUsername = adminUsername,
                NetworkProfile = new VirtualMachineNetworkProfile
                {
                    NetworkInterfaces =
                    [
                        new VirtualMachineNetworkInterfaceReference
                        {
                            Primary = true
                        }
                    ]
                },
                OsProfile = new VirtualMachineOsProfile
                {
                    ComputerName = vmName,
                    AdminUsername = adminUsername,
                    LinuxConfiguration = new VirtualMachineLinuxConfiguration
                    {
                        DisablePasswordAuthentication = true,
                        Ssh = new VirtualMachineSshConfiguration
                        {
                            PublicKeys =
                            [
                                new VirtualMachineSshPublicKeyConfiguration
                                {
                                    Path = $"/home/{adminUsername}/.ssh/authorized_keys",
                                    KeyData = await ReadSshPublicKeyAsync(sshPublicKeyPath, generateSshKeys ?? false)
                                }
                            ]
                        }
                    }
                },
                StorageProfile = new VirtualMachineStorageProfile
                {
                    ImageReference = new ImageReference
                    {
                        Publisher = imageParts[0],
                        Offer = imageParts[1],
                        Sku = imageParts[2],
                        Version = imageParts[3]
                    },
                    OsDisk = new VirtualMachineOSDisk
                    {
                        Name = $"{vmName}_osdisk",
                        Caching = Azure.ResourceManager.Compute.Models.CachingTypes.ReadWrite,
                        CreateOption = DiskCreateOption.FromImage,
                        ManagedDisk = new VirtualMachineManagedDisk
                        {
                            StorageAccountType = StorageAccountTypes.StandardLrs
                        }
                    }
                },
                HardwareProfile = new VirtualMachineHardwareProfile
                {
                    VmSize = size
                }
            };

            // Note: This is a simplified implementation
            // In production, you'd need to create:
            // - Virtual Network
            // - Subnet
            // - Public IP (optional)
            // - Network Interface
            // - Network Security Group
            // Then associate them with the VM

            throw new NotImplementedException("VM creation requires network infrastructure setup. This will be implemented in a follow-up.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> StartVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                return false;
            }

            var vmResource = await resourceGroupResource.Value.GetVirtualMachineAsync(vmName);
            if (vmResource?.Value == null)
            {
                return false;
            }

            await vmResource.Value.StartAsync(WaitUntil.Completed);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error starting Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> StopVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                return false;
            }

            var vmResource = await resourceGroupResource.Value.GetVirtualMachineAsync(vmName);
            if (vmResource?.Value == null)
            {
                return false;
            }

            // Stop (but not deallocate - keeps the VM provisioned)
            await vmResource.Value.PowerOffAsync(WaitUntil.Completed);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error stopping Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DeallocateVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                return false;
            }

            var vmResource = await resourceGroupResource.Value.GetVirtualMachineAsync(vmName);
            if (vmResource?.Value == null)
            {
                return false;
            }

            // Deallocate (stops and releases resources)
            await vmResource.Value.DeallocateAsync(WaitUntil.Completed);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deallocating Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> RestartVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                return false;
            }

            var vmResource = await resourceGroupResource.Value.GetVirtualMachineAsync(vmName);
            if (vmResource?.Value == null)
            {
                return false;
            }

            await vmResource.Value.RestartAsync(WaitUntil.Completed);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error restarting Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenant, retryPolicy);

        try
        {
            var resourceGroupResource = await subscriptionResource.GetResourceGroupAsync(resourceGroup);
            if (resourceGroupResource?.Value == null)
            {
                return false;
            }

            var vmResource = await resourceGroupResource.Value.GetVirtualMachineAsync(vmName);
            if (vmResource?.Value == null)
            {
                return false;
            }

            await vmResource.Value.DeleteAsync(WaitUntil.Completed);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting Virtual Machine '{vmName}': {ex.Message}", ex);
        }
    }

    public async Task<VmSshInfo?> GetVirtualMachineSshInfo(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, vmName, resourceGroup);

        var vm = await GetVirtualMachine(subscription, vmName, resourceGroup, tenant, retryPolicy);
        if (vm == null)
        {
            return null;
        }

        return new VmSshInfo
        {
            Name = vm.Name,
            Host = !string.IsNullOrEmpty(vm.PublicIpAddress) ? vm.PublicIpAddress : vm.PrivateIpAddress,
            Port = 22,
            Username = vm.AdminUsername,
            SshConnectionString = $"ssh {vm.AdminUsername}@{vm.PublicIpAddress ?? vm.PrivateIpAddress}",
            ResourceGroupName = vm.ResourceGroupName,
            SubscriptionId = vm.SubscriptionId
        };
    }

    private static async Task<VirtualMachine> ConvertToVmModelAsync(VirtualMachineResource vmResource)
    {
        var data = vmResource.Data;

        // Get power state
        var powerState = "Unknown";
        try
        {
            var instanceView = await vmResource.GetInstanceViewAsync();
            powerState = instanceView.Value.Data.Statuses?.FirstOrDefault(s => s.Code?.StartsWith("PowerState/") == true)?.DisplayStatus ?? "Unknown";
        }
        catch { }

        // Get network information
        var networkInterfaces = new List<VmNetworkInfo>();
        if (data.NetworkProfile?.NetworkInterfaces != null)
        {
            foreach (var nicRef in data.NetworkProfile.NetworkInterfaces)
            {
                if (nicRef.Id != null)
                {
                    networkInterfaces.Add(new VmNetworkInfo
                    {
                        Id = nicRef.Id,
                        Name = nicRef.Id.Split('/').Last()
                    });
                }
            }
        }

        // Get image info
        VmImageInfo? imageInfo = null;
        if (data.StorageProfile?.ImageReference != null)
        {
            var imgRef = data.StorageProfile.ImageReference;
            imageInfo = new VmImageInfo
            {
                Publisher = imgRef.Publisher,
                Offer = imgRef.Offer,
                Sku = imgRef.Sku,
                Version = imgRef.Version,
                Urn = $"{imgRef.Publisher}:{imgRef.Offer}:{imgRef.Sku}:{imgRef.Version}"
            };
        }

        return new VirtualMachine
        {
            Name = data.Name,
            SubscriptionId = vmResource.Id.SubscriptionId,
            ResourceGroupName = vmResource.Id.ResourceGroupName,
            Location = data.Location.ToString(),
            Size = data.HardwareProfile?.VmSize.ToString(),
            OsType = data.OSProfile?.WindowsConfiguration != null ? "Windows" : "Linux",
            AdminUsername = data.OSProfile?.AdminUsername,
            ProvisioningState = data.ProvisioningState?.ToString(),
            PowerState = powerState,
            Image = imageInfo,
            NetworkInterfaces = networkInterfaces,
            Tags = data.Tags?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            Id = vmResource.Id.ToString(),
            Type = vmResource.Data.Type.ToString()
        };
    }

    private static async Task<string> ReadSshPublicKeyAsync(string? sshPublicKeyPath, bool generateSshKeys)
    {
        if (!string.IsNullOrEmpty(sshPublicKeyPath) && File.Exists(sshPublicKeyPath))
        {
            return await File.ReadAllTextAsync(sshPublicKeyPath);
        }

        if (generateSshKeys)
        {
            // Generate SSH key pair
            var keyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_rsa");
            if (!File.Exists($"{keyPath}.pub"))
            {
                // In production, you'd use a proper SSH key generation library
                throw new NotImplementedException("SSH key generation requires additional implementation.");
            }

            return await File.ReadAllTextAsync($"{keyPath}.pub");
        }

        throw new FileNotFoundException("SSH public key not found. Provide --ssh-key-path or use --generate-ssh-keys.");
    }
}
