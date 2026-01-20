// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AzureMcp.Compute.Models;

public class VirtualMachine
{
    /// <summary> Name of the Virtual Machine resource. </summary>
    public string? Name { get; set; }

    /// <summary> ID of the Azure subscription containing the Virtual Machine resource. </summary>
    public string? SubscriptionId { get; set; }

    /// <summary> Name of the resource group containing the Virtual Machine resource. </summary>
    public string? ResourceGroupName { get; set; }

    /// <summary> Azure geo-location where the Virtual Machine resource lives. </summary>
    public string? Location { get; set; }

    /// <summary> Virtual Machine size (SKU). </summary>
    public string? Size { get; set; }

    /// <summary> OS type (Windows/Linux). </summary>
    public string? OsType { get; set; }

    /// <summary> Admin username for the Virtual Machine. </summary>
    public string? AdminUsername { get; set; }

    /// <summary> Provisioning state of the Virtual Machine. </summary>
    public string? ProvisioningState { get; set; }

    /// <summary> Power state of the Virtual Machine. </summary>
    public string? PowerState { get; set; }

    /// <summary> Public IP address associated with the Virtual Machine. </summary>
    public string? PublicIpAddress { get; set; }

    /// <summary> Private IP address associated with the Virtual Machine. </summary>
    public string? PrivateIpAddress { get; set; }

    /// <summary> OS image information. </summary>
    public VmImageInfo? Image { get; set; }

    /// <summary> Network interface information. </summary>
    public List<VmNetworkInfo>? NetworkInterfaces { get; set; }

    /// <summary> Resource tags associated with the Virtual Machine. </summary>
    public IDictionary<string, string>? Tags { get; set; }

    /// <summary> ID of the Virtual Machine resource. </summary>
    public string? Id { get; set; }

    /// <summary> Resource type. </summary>
    public string? Type { get; set; }
}

public class VmImageInfo
{
    /// <summary> Image publisher. </summary>
    public string? Publisher { get; set; }

    /// <summary> Image offer. </summary>
    public string? Offer { get; set; }

    /// <summary> Image SKU. </summary>
    public string? Sku { get; set; }

    /// <summary> Image version. </summary>
    public string? Version { get; set; }

    /// <summary> Exact image URN. </summary>
    public string? Urn { get; set; }
}

public class VmNetworkInfo
{
    /// <summary> Network interface ID. </summary>
    public string? Id { get; set; }

    /// <summary> Network interface name. </summary>
    public string? Name { get; set; }

    /// <summary> Private IP address. </summary>
    public string? PrivateIpAddress { get; set; }

    /// <summary> Private IP allocation method. </summary>
    public string? PrivateIpAllocationMethod { get; set; }

    /// <summary> Subnet name. </summary>
    public string? Subnet { get; set; }
}

public class VmSshInfo
{
    /// <summary> Virtual Machine name. </summary>
    public string? Name { get; set; }

    /// <summary> Public IP address or FQDN for SSH connection. </summary>
    public string? Host { get; set; }

    /// <summary> SSH port (typically 22). </summary>
    public int? Port { get; set; }

    /// <summary> Admin username for SSH login. </summary>
    public string? Username { get; set; }

    /// <summary> Complete SSH connection string. </summary>
    public string? SshConnectionString { get; set; }

    /// <summary> Resource group name. </summary>
    public string? ResourceGroupName { get; set; }

    /// <summary> Subscription ID. </summary>
    public string? SubscriptionId { get; set; }
}
