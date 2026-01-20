// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Compute.Options.VirtualMachine;
using AzureMcp.Compute.Services;
using AzureMcp.Core.Commands;
using AzureMcp.Core.Services.Telemetry;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Compute.Commands.VirtualMachine;

public sealed class VirtualMachineListCommand(ILogger<VirtualMachineListCommand> logger)
    : BaseComputeCommand<VirtualMachineListOptions>
{
    private const string CommandTitle = "List Virtual Machines";
    private readonly ILogger<VirtualMachineListCommand> _logger = logger;

    private readonly Option<string> _resourceGroupOption = ComputeOptionDefinitions.ResourceGroup;

    public override string Name => "list";

    public override string Description =>
        """
        List all Azure Virtual Machines in a subscription.
        Returns detailed VM information including size, OS, power state, network configuration, and resource group.
        Optionally filter by resource group.
        """;

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = true };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_resourceGroupOption);
    }

    protected override VirtualMachineListOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.ResourceGroup = parseResult.GetValueForOption(_resourceGroupOption);
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
            var vms = await computeService.ListVirtualMachines(
                options.Subscription!,
                options.ResourceGroup,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = vms?.Count > 0 ?
                ResponseResult.Create(
                    new VirtualMachineListCommandResult(vms),
                    ComputeJsonContext.Default.VirtualMachineListCommandResult) :
                null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error listing Virtual Machines. Subscription: {Subscription}, ResourceGroup: {ResourceGroup}, Options: {@Options}",
                options.Subscription, options.ResourceGroup, options);
            HandleException(context, ex);
        }

        return context.Response;
    }

    protected override string GetErrorMessage(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx when reqEx.Status == 404 =>
            "Subscription not found. Verify the subscription ID and you have access.",
        Azure.RequestFailedException reqEx when reqEx.Status == 403 =>
            $"Authorization failed accessing Virtual Machines. Details: {reqEx.Message}",
        Azure.RequestFailedException reqEx => reqEx.Message,
        _ => base.GetErrorMessage(ex)
    };

    protected override int GetStatusCode(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx => reqEx.Status,
        _ => base.GetStatusCode(ex)
    };

    internal record VirtualMachineListCommandResult(List<Models.VirtualMachine> VirtualMachines);
}
