// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Compute.Commands.VirtualMachine;
using AzureMcp.Compute.Services;
using AzureMcp.Core.Areas;
using AzureMcp.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Compute;

public class ComputeSetup : IAreaSetup
{
    public string Name => "compute";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IComputeService, ComputeService>();
    }

    public void RegisterCommands(CommandGroup rootGroup, ILoggerFactory loggerFactory)
    {
        // Create Compute command group
        var compute = new CommandGroup(Name,
            """
            Compute operations - Commands for managing Azure Compute resources including Virtual Machines.
            Use this tool when you need to list, create, start, stop, or manage Azure Virtual Machines in your
            subscription. This tool supports operations for VM lifecycle management including provisioning, power
            management (start/stop/deallocate), and deletion. It also provides SSH connection information for
            Linux VMs to facilitate remote access. This tool requires appropriate Azure RBAC permissions and will
            only access compute resources accessible to the authenticated user.
            """);
        rootGroup.AddSubGroup(compute);

        // Create VirtualMachine subgroup
        var vm = new CommandGroup("vm",
            """
            Virtual Machine operations - Commands for managing Azure Virtual Machines (VMs) including listing VMs,
            retrieving VM details, managing VM power state (start/stop/restart/deallocate), and deleting VMs.
            Also provides SSH connection information for Linux VMs.
            """);
        compute.AddSubGroup(vm);

        // Register VM commands
        vm.AddCommand("list", new VirtualMachineListCommand(loggerFactory.CreateLogger<VirtualMachineListCommand>()));
        vm.AddCommand("show", new VirtualMachineShowCommand(loggerFactory.CreateLogger<VirtualMachineShowCommand>()));
        vm.AddCommand("start", new VirtualMachineStartCommand(loggerFactory.CreateLogger<VirtualMachineStartCommand>()));
        vm.AddCommand("stop", new VirtualMachineStopCommand(loggerFactory.CreateLogger<VirtualMachineStopCommand>()));
        vm.AddCommand("deallocate", new VirtualMachineDeallocateCommand(loggerFactory.CreateLogger<VirtualMachineDeallocateCommand>()));
        vm.AddCommand("restart", new VirtualMachineRestartCommand(loggerFactory.CreateLogger<VirtualMachineRestartCommand>()));
        vm.AddCommand("delete", new VirtualMachineDeleteCommand(loggerFactory.CreateLogger<VirtualMachineDeleteCommand>()));
        vm.AddCommand("ssh-info", new VirtualMachineSshInfoCommand(loggerFactory.CreateLogger<VirtualMachineSshInfoCommand>()));

        // Note: VM create command requires additional implementation for network infrastructure
        // It will be added in a future update
    }
}
