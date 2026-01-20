// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Compute.Options.VirtualMachine;
using AzureMcp.Compute.Services;
using AzureMcp.Core.Commands;
using AzureMcp.Core.Services.Telemetry;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Compute.Commands.VirtualMachine;

public sealed class VirtualMachineCreateCommand(ILogger<VirtualMachineCreateCommand> logger)
    : BaseComputeCommand<VirtualMachineCreateOptions>
{
    private const string CommandTitle = "Create Virtual Machine";
    private readonly ILogger<VirtualMachineCreateCommand> _logger = logger;

    private readonly Option<string> _vmNameOption = ComputeOptionDefinitions.VirtualMachine;
    private readonly Option<string> _resourceGroupOption = ComputeOptionDefinitions.ResourceGroup;
    private readonly Option<string> _locationOption = ComputeOptionDefinitions.Location;
    private readonly Option<string> _sizeOption = ComputeOptionDefinitions.Size;
    private readonly Option<string> _imageOption = ComputeOptionDefinitions.Image;
    private readonly Option<string> _adminUsernameOption = ComputeOptionDefinitions.AdminUsername;
    private readonly Option<string> _sshKeyPathOption = ComputeOptionDefinitions.SshPublicKeyPath;
    private readonly Option<bool> _generateSshKeysOption = ComputeOptionDefinitions.GenerateSshKeys;

    public override string Name => "create";

    public override string Description =>
        """
        Create a new Azure Virtual Machine.
        Creates a new Linux VM with the specified configuration.
        Note: This command requires existing network infrastructure (VNet, Subnet, Public IP, NIC) to be available.
        For automated network setup, consider using Azure Portal or ARM templates instead.
        """;

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new() { Destructive = false, ReadOnly = false };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_vmNameOption);
        command.AddOption(_resourceGroupOption);
        command.AddOption(_locationOption);
        command.AddOption(_sizeOption);
        command.AddOption(_imageOption);
        command.AddOption(_adminUsernameOption);
        command.AddOption(_sshKeyPathOption);
        command.AddOption(_generateSshKeysOption);
    }

    protected override VirtualMachineCreateOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.VirtualMachine = parseResult.GetValueForOption(_vmNameOption);
        options.ResourceGroup = parseResult.GetValueForOption(_resourceGroupOption);
        options.Location = parseResult.GetValueForOption(_locationOption);
        options.Size = parseResult.GetValueForOption(_sizeOption);
        options.Image = parseResult.GetValueForOption(_imageOption);
        options.AdminUsername = parseResult.GetValueForOption(_adminUsernameOption);
        options.SshPublicKeyPath = parseResult.GetValueForOption(_sshKeyPathOption);
        options.GenerateSshKeys = parseResult.GetValueForOption(_generateSshKeysOption);
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

            try
            {
                var vm = await computeService.CreateVirtualMachine(
                    options.Subscription!,
                    options.VirtualMachine!,
                    options.ResourceGroup!,
                    options.Location!,
                    options.Size!,
                    options.Image!,
                    options.AdminUsername!,
                    options.SshPublicKeyPath,
                    options.GenerateSshKeys,
                    options.Tenant,
                    options.RetryPolicy);

                context.Response.Results = vm != null ?
                    ResponseResult.Create(
                        new VirtualMachineCreateCommandResult(vm, "Virtual Machine created successfully"),
                        ComputeJsonContext.Default.VirtualMachine) :
                    null;
            }
            catch (NotImplementedException)
            {
                context.Response.Status = 501;
                context.Response.Message =
                    """
                    VM creation requires network infrastructure setup (VNet, Subnet, NIC, Public IP).
                    This is not yet implemented in the CLI command.

                    Recommended alternatives:
                    1. Use Azure Portal: https://portal.azure.com
                    2. Use Azure CLI: az vm create --resource-group <rg> --name <vm> --image <image> --admin-username <user>
                    3. Use ARM templates or Bicep for reproducible deployments

                    The VM commands are ready for: list, show, start, stop, deallocate, restart, delete, ssh-info
                    """;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating Virtual Machine. VM: {VmName}, ResourceGroup: {ResourceGroup}, Options: {@Options}",
                options.VirtualMachine, options.ResourceGroup, options);
            HandleException(context, ex);
        }

        return context.Response;
    }

    protected override string GetErrorMessage(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx when reqEx.Status == 404 =>
            "Resource group or network resource not found. Verify all resources exist.",
        Azure.RequestFailedException reqEx when reqEx.Status == 403 =>
            $"Authorization failed creating the Virtual Machine. Details: {reqEx.Message}",
        Azure.RequestFailedException reqEx => reqEx.Message,
        _ => base.GetErrorMessage(ex)
    };

    protected override int GetStatusCode(Exception ex) => ex switch
    {
        Azure.RequestFailedException reqEx => reqEx.Status,
        _ => base.GetStatusCode(ex)
    };
}
