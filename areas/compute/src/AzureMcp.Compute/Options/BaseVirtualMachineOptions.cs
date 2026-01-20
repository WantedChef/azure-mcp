// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AzureMcp.Compute.Options;

public class BaseVirtualMachineOptions : BaseComputeOptions
{
    public string? VirtualMachine { get; set; }
    public string? ResourceGroup { get; set; }
}
