using JGUZDV.CQRS.Queries;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace JGUZDV.CQRS.Tests
{
    internal class TestQuery : IQuery<TestResult>
    {
        public TestQuery(bool isQueryAuthorized, bool isValid, bool isResultAuthorized, bool canExecute, TestResult resultToBeSet)
        {
            Methods = new();

            IsQueryAuthorized = isQueryAuthorized;
            IsValid = isValid;
            IsResultAuthorized = isResultAuthorized;
            CanExecute = canExecute;
            ResultToBeSet = resultToBeSet;
        }

        public bool IsQueryAuthorized { get; }
        public bool IsValid { get; }
        public bool IsResultAuthorized { get; }
        public bool CanExecute { get; }
        public TestResult ResultToBeSet { get; }
        public QueryResult<TestResult> Result { get; set; }

        public Queue<string> Methods { get; }
        public void TraceMethod([CallerMemberName] string method = null!)
        {
            Methods.Enqueue(method);
        }
    }

    internal class TestResult
    {
    }

    internal class TestQueryHandler : QueryHandler<TestQuery, TestResult>
    {
        public const string NormalizeQueryMethod = nameof(NormalizeQuery);
        public const string AuthorizeQueryAsyncMethod = nameof(AuthorizeExecuteAsync);
        public const string ValidateAsyncMethod = nameof(ValidateAsync);
        public const string ExecuteInternalAsyncMethod = nameof(ExecuteInternalAsync);
        public const string AuthorizeResultAsyncMethod = nameof(AuthorizeValueAsync);

        public override ILogger Logger { get; } = NullLogger<TestCommandHandler>.Instance;

        protected override TestQuery NormalizeQuery(TestQuery query, ClaimsPrincipal? principal)
        {
            query.TraceMethod();
            return query;
        }

        protected override Task<bool> AuthorizeExecuteAsync(TestQuery query, ClaimsPrincipal? principal, CancellationToken ct)
        {
            query.TraceMethod();
            return Task.FromResult(query.IsQueryAuthorized);
        }

        protected override Task<List<ValidationResult>> ValidateAsync(TestQuery query, ClaimsPrincipal? principal, CancellationToken ct)
        {
            query.TraceMethod();
            return Task.FromResult(query.IsValid
                ? new List<ValidationResult>()
                : new List<ValidationResult> { new ValidationResult("Not valid") }
            );
        }

        protected override Task<bool> AuthorizeValueAsync(TestQuery query, TestResult result, ClaimsPrincipal? principal, CancellationToken ct)
        {
            query.TraceMethod();
            return Task.FromResult(query.IsResultAuthorized);
        }

        protected override Task<QueryResult<TestResult>> ExecuteInternalAsync(TestQuery query, ClaimsPrincipal? principal, CancellationToken ct)
        {
            query.TraceMethod();

            if (query.CanExecute)
            {
                return Task.FromResult((QueryResult<TestResult>)query.ResultToBeSet);
            }

            throw new InvalidOperationException("Cannot execute.");
        }
    }
}
