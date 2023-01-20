using JGUZDV.CQRS.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Xunit;

namespace JGUZDV.CQRS.Tests
{
    public class CommandHandlerTests
    {
        [Fact]
        public async Task Execution_Order_Is_Correct()
        {
            var sut = new TestCommandHandler(false);
            var command = new TestCommand(true, true, true, CommandResult.Success());

            var result = await sut.ExecuteAsync(command, null, default);

            Assert.True(result.IsSuccess);

            Assert.Equal(5, command.Methods.Count);
            Assert.Equal(TestCommandHandler.InitializeAsyncMethod, command.Methods[0]);
            Assert.Equal(TestCommandHandler.NormalizeCommandMethod, command.Methods[1]);
            Assert.Equal(TestCommandHandler.AuthorizeAsyncMethod, command.Methods[2]);
            Assert.Equal(TestCommandHandler.ValidateAsyncMethod, command.Methods[3]);
            Assert.Equal(TestCommandHandler.ExecuteInternalAsyncMethod, command.Methods[4]);
        }
    }

    internal class TestCommand : ICommand
    {
        private readonly List<string> _methods;

        public TestCommand(bool isAuthorized, bool isValid, bool canBeExecuted, CommandResult result)
        {
            _methods = new();

            IsAuthorized = isAuthorized;
            IsValid = isValid;
            CanBeExecuted = canBeExecuted;
            Result = result;
        }

        public bool IsAuthorized { get; }
        public bool IsValid { get; }
        public bool CanBeExecuted { get; }
        public CommandResult Result { get; }

        public IReadOnlyList<string> Methods => _methods;
        public void TraceMethod([CallerMemberName] string method = null!)
        {
            _methods.Add(method);
        }
    }

    internal class TestCommandHandler : CommandHandler<TestCommand, object>
    {
        public const string InitializeAsyncMethod = nameof(InitializeAsync);
        public const string NormalizeCommandMethod = nameof(NormalizeCommand);
        public const string AuthorizeAsyncMethod = nameof(AuthorizeAsync);
        public const string ValidateAsyncMethod = nameof(ValidateAsync);
        public const string ExecuteInternalAsyncMethod = nameof(ExecuteInternalAsync);


        public override ILogger Logger { get; } = NullLogger<TestCommandHandler>.Instance;

        public TestCommandHandler(bool skipAuthorization)
        {
            SkipAuthorization = skipAuthorization;
        }

        protected override Task<object> InitializeAsync(TestCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        {
            command.TraceMethod();
            return Task.FromResult(new object());
        }

        protected override TestCommand NormalizeCommand(TestCommand command, object context, ClaimsPrincipal? principal)
        {
            command.TraceMethod();
            return command;
        }

        protected override Task<bool> AuthorizeAsync(TestCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        {
            command.TraceMethod();

            return Task.FromResult(command.IsAuthorized);
        }

        protected override Task<List<ValidationResult>> ValidateAsync(TestCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        {
            command.TraceMethod();

            return Task.FromResult(command.IsValid
                ? new List<ValidationResult>()
                : new List<ValidationResult>() { new ValidationResult("invalid") });
        }

        protected override Task<CommandResult> ExecuteInternalAsync(TestCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        {
            command.TraceMethod();

            if(command.CanBeExecuted)
                return Task.FromResult(command.Result);

            throw new InvalidOperationException("Can't execute");
        }
    }
}
