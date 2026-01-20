// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Compute.Models;
using AzureMcp.Core.Options;

namespace AzureMcp.Compute.Services;

public interface IComputeService
{
    Task<List<VirtualMachine>> ListVirtualMachines(
        string subscription,
        string? resourceGroup = null,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<VirtualMachine?> GetVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<VirtualMachine?> CreateVirtualMachine(
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
        RetryPolicyOptions? retryPolicy = null);

    Task<bool> StartVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<bool> StopVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<bool> DeallocateVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<bool> RestartVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<bool> DeleteVirtualMachine(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<VmSshInfo?> GetVirtualMachineSshInfo(
        string subscription,
        string vmName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);
}
