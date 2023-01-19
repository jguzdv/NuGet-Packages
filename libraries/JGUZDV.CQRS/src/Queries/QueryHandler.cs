using JGUZDV.CQRS.Queries.Results;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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
    public abstract partial class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public abstract ILogger Logger { get; }

        protected virtual TQuery NormalizeQuery(TQuery query, ClaimsPrincipal? principal)
            => query;

        protected virtual Task<bool> AuthorizeQueryAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct)
            => Task.FromResult(true);
        protected virtual Task<bool> AuthorizeResultAsync(TQuery query, TResult result, ClaimsPrincipal? principal, CancellationToken ct)
            => Task.FromResult(true);

        protected virtual Task<List<ValidationResult>> ValidateAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct)
            => Task.FromResult(new List<ValidationResult>());


        protected abstract Task<QueryResult<TResult>> ExecuteInternalAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct);


        public async Task<QueryResult<TResult>> ExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct)
        {
            try
            {
                if (ct.IsCancellationRequested)
                    // TODO: Log Cancellation?
                    return QueryResult<TResult>.Canceled(ct);

                query = NormalizeQuery(query, principal);

                
                var isAuthorized = await AuthorizeQueryAsync(query, principal, ct);
                if (!isAuthorized)
                    // TODO: Log Authorization Result as Information
                    return QueryResult<TResult>.NotAllowed();

                if (ct.IsCancellationRequested)
                    // TODO: Log Cancellation?
                    return QueryResult<TResult>.Canceled(ct);

                var validationResult = await ValidateAsync(query, principal, ct);
                if (validationResult.Any())
                    // TODO: Log Valiation Result as Debug
                    return QueryResult<TResult>.NotValid(validationResult);

                if (ct.IsCancellationRequested)
                    // TODO: Log Cancellation?
                    return QueryResult<TResult>.Canceled(ct);

                var result = await ExecuteInternalAsync(query, principal, ct);

                var isResultAuthorized = await AuthorizeResultAsync(query, result, principal, ct);
                if(!isResultAuthorized)
                    // TODO: Log Authorization Result as Information
                    return QueryResult<TResult>.NotAllowed();

                return result;
            }
            catch (TaskCanceledException)
            {
                // TODO: Log Exception?
                return QueryResult<TResult>.Canceled();
            }
            catch (Exception ex)
            {
                // TODO: Log Exception
                return QueryResult<TResult>.Fail("GenericError");
            }
        }

        protected static partial class Log
        {
            [LoggerMessage(1, LogLevel.Debug, "Query {param1} has not been valid.")]
            internal static partial void Invalid(ILogger logger, object param1);
        }
    }
}
