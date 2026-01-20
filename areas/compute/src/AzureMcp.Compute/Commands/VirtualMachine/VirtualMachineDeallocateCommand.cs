// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Compute.Options.VirtualMachine;
using AzureMcp.Compute.Services;
using AzureMcp.Core.Commands;
using AzureMcp.Core.Services.Telemetry;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Compute.Commands.VirtualMachine;

public sealed class VirtualMachineDeallocateCommand(ILogger<VirtualMachineDeallocateCommand> logger)
    : BaseVirtualMachineCommand<VirtualMachineDeallocateOptions>
{
    private const string CommandTitle = "Deallocate Virtual Machine";
    private readonly ILogger<VirtualMachineDeallocateCommand> _logger = logger;

    private readonly Option<string> _resourceGroupOption = ComputeOptionDefinitions.ResourceGroup;
    private readonly Option<string> _vmNameOption = ComputeOptionDefinitions.VirtualMachine;

    public override string Name => "deallocate";

    public override string Description =>
        """
        Deallocate (stop and release resources) an Azure Virtual Machine.
        This stops the VM and releases compute resources, stopping billing for the VM.
        The VM storage and configuration are preserved. Use start to recreate the VM.
        Returns the operation status and any error messages if the deallocate operation fails.
        """;

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = false };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_resourceGroupOption);
        command.AddOption(_vmNameOption);
    }

    protected override VirtualMachineDeallocateOptions BindOptions(ParseResult parseResult)
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
            var success = await computeService.DeallocateVirtualMachine(
                options.Subscription!,
                options.VirtualMachine!,
                options.ResourceGroup!,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = success ?
                ResponseResult.Create(
                    new VirtualMachineDeallocateCommandResult("Virtual Machine deallocated successfully"),
                    ComputeJsonContext.Default.VirtualMachineDeallocateCommandResult) :
                null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error deallocating Virtual Machine. VM: {VmName}, ResourceGroup: {ResourceGroup}, Options: {@Options}",
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
            $"Authorization failed deallocating the Virtual Machine. Details: {reqEx.Message}",
        Azure.RequestFailedException reqEx => reqEx.Message,
        _ => base.GetErrorMessage(ex)
    };

    protected override int GetStatusCode(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx => reqEx.Status,
        _ => base.GetStatusCode(ex)
    };

    internal record VirtualMachineDeallocateCommandResult(string Message);
}
