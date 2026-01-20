// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Compute.Options.VirtualMachine;
using AzureMcp.Compute.Services;
using AzureMcp.Core.Commands;
using AzureMcp.Core.Services.Telemetry;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Compute.Commands.VirtualMachine;

public sealed class VirtualMachineShowCommand(ILogger<VirtualMachineShowCommand> logger)
    : BaseVirtualMachineCommand<VirtualMachineShowOptions>
{
    private const string CommandTitle = "Show Virtual Machine Details";
    private readonly ILogger<VirtualMachineShowCommand> _logger = logger;

    private readonly Option<string> _resourceGroupOption = ComputeOptionDefinitions.ResourceGroup;
    private readonly Option<string> _vmNameOption = ComputeOptionDefinitions.VirtualMachine;

    public override string Name => "show";

    public override string Description =>
        """
        Get detailed information about a specific Azure Virtual Machine.
        Returns complete VM configuration including size, OS, network settings, disks, and status.
        """;

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = true };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_resourceGroupOption);
        command.AddOption(_vmNameOption);
    }

    protected override VirtualMachineShowOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.ResourceGroup = parseResult.GetValueForOption(_resourceGroupOption);
        options.VirtualMachine = parseResult.GetValueForOption(_vmNameOption);
        return options;
    }

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        var options = BindOptions(parseResult);

        try
        {
            if (!Validate(parseResult.CommandResult, context.Response).IsValid)
            {
                return context.Response;
            }

            context.Activity?.WithSubscriptionTag(options);

            var computeService = context.GetService<IComputeService>();
            var vm = await computeService.GetVirtualMachine(
                options.Subscription!,
                options.VirtualMachine!,
                options.ResourceGroup!,
                options.Tenant,
                options.RetryPolicy);

            if (vm == null)
            {
                context.Response.Status = 404;
                context.Response.Message = $"Virtual Machine '{options.VirtualMachine}' not found in resource group '{options.ResourceGroup}'.";
                return context.Response;
            }

            context.Response.Results = ResponseResult.Create(
                new VirtualMachineShowCommandResult(vm),
                ComputeJsonContext.Default.VirtualMachineShowCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error getting Virtual Machine. VM: {VmName}, ResourceGroup: {ResourceGroup}, Options: {@Options}",
                options.VirtualMachine, options.ResourceGroup, options);
            HandleException(context, ex);
        }

        return context.Response;
    }

    protected override string GetErrorMessage(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx when reqEx.Status == 404 =>
            "Virtual Machine not found. Verify the VM name and resource group.",
        Azure.RequestFailedException reqEx when reqEx.Status == 403 =>
            $"Authorization failed accessing the Virtual Machine. Details: {reqEx.Message}",
        Azure.RequestFailedException reqEx => reqEx.Message,
        _ => base.GetErrorMessage(ex)
    };

    protected override int GetStatusCode(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx => reqEx.Status,
        _ => base.GetStatusCode(ex)
    };

    internal record VirtualMachineShowCommandResult(Models.VirtualMachine VirtualMachine);
}
