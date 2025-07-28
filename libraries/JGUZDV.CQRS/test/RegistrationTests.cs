using System.Security.Claims;

using JGUZDV.CQRS.Commands;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.CQRS.Tests;

internal class InternalCommand : ICommand { }
internal class InternalCommandHandler : ICommandHandler<InternalCommand>
{
    public Task<HandlerResult> ExecuteAsync(InternalCommand command, ClaimsPrincipal? principal, CancellationToken ct) => throw new NotImplementedException();
}

public class PublicCommand : ICommand { }
public class PublicCommandHandler : ICommandHandler<PublicCommand>
{
    public Task<HandlerResult> ExecuteAsync(PublicCommand command, ClaimsPrincipal? principal, CancellationToken ct) => throw new NotImplementedException();
}

public class RegistrationTests
{
    [Fact]
    public void CommandHandlersGetRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCQRSHandlers(GetType());

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<ICommandHandler<InternalCommand>>());
        Assert.NotNull(provider.GetService<ICommandHandler<PublicCommand>>());
    }
}