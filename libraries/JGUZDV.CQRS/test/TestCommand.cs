﻿using JGUZDV.CQRS.Commands;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace JGUZDV.CQRS.Tests
{
    internal class TestCommand : ICommand
    {
        public TestCommand(bool isAuthorized, bool isValid, bool canBeExecuted, HandlerResult result)
        {
            Methods = new();

            IsAuthorized = isAuthorized;
            IsValid = isValid;
            CanBeExecuted = canBeExecuted;
            Result = result;
        }

        public bool IsAuthorized { get; }
        public bool IsValid { get; }
        public bool CanBeExecuted { get; }
        public HandlerResult Result { get; }

        public Queue<string> Methods { get; }
        public void TraceMethod([CallerMemberName] string method = null!)
        {
            Methods.Enqueue(method);
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

        protected override Task<HandlerResult> ExecuteInternalAsync(TestCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        {
            command.TraceMethod();

            if (command.CanBeExecuted)
                return Task.FromResult(command.Result);

            throw new InvalidOperationException("Can't execute");
        }
    }
}
