// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;

namespace AzureMcp.Compute.Options;

public static class ComputeOptionDefinitions
{
    public const string VirtualMachineName = "vm";
    public const string ResourceGroupName = "resource-group";

    public static readonly Option<string> VirtualMachine = new(
        $"--{VirtualMachineName}",
        "Virtual Machine name."
    )
    {
        IsRequired = true
    };

    public static readonly Option<string> ResourceGroup = new(
        $"--{ResourceGroupName}",
        "Resource group name."
    )
    {
        IsRequired = true
    };

    public static readonly Option<string> Location = new Option<string>(
        $"--location",
        "Azure region for the Virtual Machine (e.g., westeurope, eastus)."
    );

    public static readonly Option<string> Size = new Option<string>(
        $"--size",
        "Virtual Machine size (SKU). Example: Standard_D4s_v5, Standard_D2s_v3."
    );

    public static readonly Option<string> Image = new Option<string>(
        $"--image",
        "OS Image to use. Format: Publisher:Offer:Sku:Version. Example: Canonical:UbuntuServer:22.04-LTS:latest."
    );

    public static readonly Option<string> AdminUsername = new Option<string>(
        $"--admin-username",
        "Admin username for the Virtual Machine."
    );

    public static readonly Option<string> SshPublicKeyPath = new Option<string>(
        $"--ssh-key-path",
        "Path to SSH public key file for authentication."
    );

    public static readonly Option<bool> GenerateSshKeys = new Option<bool>(
        $"--generate-ssh-keys",
        "Generate SSH public and private key files if missing."
    );
}
