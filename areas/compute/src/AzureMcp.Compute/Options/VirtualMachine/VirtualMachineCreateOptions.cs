// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AzureMcp.Compute.Options.VirtualMachine;

public class VirtualMachineCreateOptions : BaseComputeOptions
{
    public string? VirtualMachine { get; set; }
    public string? ResourceGroup { get; set; }
    public string? Location { get; set; }
    public string? Size { get; set; }
    public string? Image { get; set; }
    public string? AdminUsername { get; set; }
    public string? SshPublicKeyPath { get; set; }
    public bool? GenerateSshKeys { get; set; }
}
