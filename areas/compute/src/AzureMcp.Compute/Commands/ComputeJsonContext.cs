// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using AzureMcp.Compute.Commands.VirtualMachine;

namespace AzureMcp.Compute.Commands;

[JsonSerializable(typeof(VirtualMachineListCommand.VirtualMachineListCommandResult), TypeInfoPropertyName = "VirtualMachineListCommandResult")]
[JsonSerializable(typeof(VirtualMachineShowCommand.VirtualMachineShowCommandResult), TypeInfoPropertyName = "VirtualMachineShowCommandResult")]
[JsonSerializable(typeof(VirtualMachineStartCommand.VirtualMachineStartCommandResult), TypeInfoPropertyName = "VirtualMachineStartCommandResult")]
[JsonSerializable(typeof(VirtualMachineStopCommand.VirtualMachineStopCommandResult), TypeInfoPropertyName = "VirtualMachineStopCommandResult")]
[JsonSerializable(typeof(VirtualMachineDeallocateCommand.VirtualMachineDeallocateCommandResult), TypeInfoPropertyName = "VirtualMachineDeallocateCommandResult")]
[JsonSerializable(typeof(VirtualMachineRestartCommand.VirtualMachineRestartCommandResult), TypeInfoPropertyName = "VirtualMachineRestartCommandResult")]
[JsonSerializable(typeof(VirtualMachineDeleteCommand.VirtualMachineDeleteCommandResult), TypeInfoPropertyName = "VirtualMachineDeleteCommandResult")]
[JsonSerializable(typeof(VirtualMachineSshInfoCommand.VirtualMachineSshInfoCommandResult), TypeInfoPropertyName = "VirtualMachineSshInfoCommandResult")]
[JsonSerializable(typeof(Models.VirtualMachine))]
[JsonSerializable(typeof(Models.VmImageInfo))]
[JsonSerializable(typeof(Models.VmNetworkInfo))]
[JsonSerializable(typeof(Models.VmSshInfo))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ComputeJsonContext : JsonSerializerContext;
