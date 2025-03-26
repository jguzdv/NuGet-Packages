using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;

using Microsoft.Extensions.Logging;

namespace JGUZDV.CQRS.Queries
{
    /// <summary>
    /// Basis-QueryHandler.
    /// Authorisiert den Query (falls notwendig)
    /// Führt den Query aus
    /// Authorisiert das Query-Ergebnis (falls notwendig)
    /// Gibt Query-Ergebnis zurück.
    /// </summary>
    /// <typeparam name="TQuery">Query-Beschreibungstyp</typeparam>
    /// <typeparam name="TValue">Rückgabetyp</typeparam>
    public abstract partial class QueryHandler<TQuery, TValue> : IQueryHandler<TQuery>
        where TQuery : IQuery<TValue>
    {
        public abstract ILogger Logger { get; }

        protected virtual TQuery NormalizeQuery(TQuery query, ClaimsPrincipal? principal)
            => query;

        protected virtual Task<bool> AuthorizeExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct)
            => Task.FromResult(true);
        protected virtual Task<bool> AuthorizeValueAsync(TQuery query, TValue value, ClaimsPrincipal? principal, CancellationToken ct)
            => Task.FromResult(true);

        protected virtual Task<List<ValidationResult>> ValidateAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct)
            => Task.FromResult(new List<ValidationResult>());


        protected abstract Task<QueryResult<TValue>> ExecuteInternalAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct);


        public async Task ExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct)
        {
            try
            {
                if (ct.IsCancellationRequested)
                {
                    query.Result = HandlerResult.Canceled(ct);
                    Log.Cancelled(Logger);
                    return;
                }

                query = NormalizeQuery(query, principal);

                
                var isAuthorized = await AuthorizeExecuteAsync(query, principal, ct);
                Log.QueryExecutionAuthorizationResult(Logger, isAuthorized);

                if (!isAuthorized)
                {
                    query.Result = HandlerResult.NotAllowed();
                    return;
                }

                if (ct.IsCancellationRequested) {
                    Log.StepCancelled(Logger, nameof(AuthorizeExecuteAsync));
                    query.Result = HandlerResult.Canceled(ct);
                    return;
                }

                var validationResult = await ValidateAsync(query, principal, ct);
                Log.ValidationResult(Logger, !validationResult.Any());

                if (validationResult.Any())
                {
                    if (Logger.IsEnabled(LogLevel.Debug))
                    {
                        foreach (var v in validationResult)
                            Log.ValidationResultDetail(Logger, string.Join(", ", v.MemberNames), v.ErrorMessage ?? "n/a");
                    }

                    query.Result = HandlerResult.NotValid(validationResult);
                    return;

                }

                if (ct.IsCancellationRequested)
                {
                    Log.StepCancelled(Logger, nameof(ValidateAsync));
                    query.Result = HandlerResult.Canceled(ct);
                    return;
                }

                var executionResult = await ExecuteInternalAsync(query, principal, ct);
                Debug.Assert(executionResult != null);

                if (executionResult.HasValue)
                {
                    var isResultAuthorized = await AuthorizeValueAsync(query, executionResult.Value, principal, ct);
                    Log.QueryValueAuthorizationResult(Logger, isResultAuthorized);
                    
                    if (!isResultAuthorized)
                    {
                        query.Result = HandlerResult.NotAllowed();
                        return;
                    }
                }

                query.Result = executionResult;
                return;
            }
            catch (TaskCanceledException tcex)
            {
                query.Result = HandlerResult.Canceled(tcex.CancellationToken);
                Log.Cancelled(Logger);
                return;
            }
            catch (Exception ex)
            {
                Log.ExecutionError(Logger, ex);
                query.Result = HandlerResult.Fail("GenericError");
                return;
            }
        }

        protected static partial class Log
        {
            [LoggerMessage(1, LogLevel.Debug, "Query has been cancelled after {step}.")]
            internal static partial void StepCancelled(ILogger logger, string step);

            [LoggerMessage(2, LogLevel.Debug, "Query has been cancelled.")]
            internal static partial void Cancelled(ILogger logger);


            [LoggerMessage(3, LogLevel.Information, "Query execution authorization result was: {authorized}", EventName = "QueryAuthorization")]
            internal static partial void QueryExecutionAuthorizationResult(ILogger logger, bool authorized);

            [LoggerMessage(4, LogLevel.Information, "Query value authorization result was: {authorized}", EventName = "QueryAuthorization")]
            internal static partial void QueryValueAuthorizationResult(ILogger logger, bool authorized);


            [LoggerMessage(5, LogLevel.Information, "Query validation result was: {valid}", EventName = "QueryValidation")]
            internal static partial void ValidationResult(ILogger logger, bool valid);

            [LoggerMessage(6, LogLevel.Debug, "Query validation result for {memberNames}: {message}", EventName = "QueryValidation", SkipEnabledCheck = true)]
            internal static partial void ValidationResultDetail(ILogger logger, string memberNames, string message);


            [LoggerMessage(7, LogLevel.Error, "Query execution threw an exception.")]
            internal static partial void ExecutionError(ILogger logger, Exception ex);
        }
    }
}
