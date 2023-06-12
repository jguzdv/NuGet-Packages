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
    /// <typeparam name="TResult">Rückgabetyp</typeparam>
    /// <exception cref="UnauthorizedQueryException">Wird geworfen wenn ein Authorisierungsschritt fehlschlägt.</exception>
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
                    // TODO: Log Cancellation?
                    return;
                }

                query = NormalizeQuery(query, principal);

                
                var isAuthorized = await AuthorizeExecuteAsync(query, principal, ct);
                if (!isAuthorized)
                {
                    // TODO: Log Authorization Result as Information
                    query.Result = HandlerResult.NotAllowed();
                    return;
                }

                if (ct.IsCancellationRequested) {
                    // TODO: Log Cancellation?
                    query.Result = HandlerResult.Canceled(ct);
                    return;
                }

                var validationResult = await ValidateAsync(query, principal, ct);
                if (validationResult.Any())
                {
                    // TODO: Log Valiation Result as Debug
                    query.Result = HandlerResult.NotValid(validationResult);
                    return;

                }

                if (ct.IsCancellationRequested)
                {
                    // TODO: Log Cancellation?
                    query.Result = HandlerResult.Canceled(ct);
                    return;
                }

                var executionResult = await ExecuteInternalAsync(query, principal, ct);
                Debug.Assert(executionResult != null);

                if (executionResult.HasValue)
                {
                    var isResultAuthorized = await AuthorizeValueAsync(query, executionResult.Value, principal, ct);
                    if (!isResultAuthorized)
                    {
                        // TODO: Log Authorization Result as Information
                        query.Result = HandlerResult.NotAllowed();
                        return;
                    }
                }

                query.Result = executionResult;
                return;
            }
            catch (TaskCanceledException)
            {
                // TODO: Log Exception?
                query.Result = HandlerResult.Canceled();
                return;
            }
            catch (Exception ex)
            {
                // TODO: Log Exception
                query.Result = HandlerResult.Fail("GenericError");
                return;
            }
        }

        protected static partial class Log
        {
            [LoggerMessage(1, LogLevel.Debug, "Query {param1} has not been valid.")]
            internal static partial void Invalid(ILogger logger, object param1);
        }
    }
}
