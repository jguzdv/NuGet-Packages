using System.ComponentModel.DataAnnotations;

namespace JGUZDV.CQRS.Queries.Results
{
    public class QueryResult<T> 
    {
        protected QueryResult() { }

        public QueryResult(T result) 
        {
            if (result == null) 
                throw new ArgumentNullException("Result should never be null. Use a more significant QueryResultObject");
            
            _result = result;
        }


        private T? _result;
        public T Result
        {
            get
            {
                if (_result == null)
                    throw new InvalidOperationException("Cannot access result, when it has not been set.");

                return _result;
            }
        }

        public bool HasResult => _result != null;

        public static implicit operator T(QueryResult<T> queryResult)
        {
            return queryResult.Result;
        }


        /// <summary>
        /// Use this to indicate the query completed successfully.
        /// </summary>
        public static QueryResult<T> Success(T result)
            => new QueryResult<T>(result);


        /// <summary>
        /// Use this to indicate the query failed generally (though an unhandled exception)
        /// </summary>
        public static GenericErrorResult<T> Fail(string reason)
            => new GenericErrorResult<T>(reason);

        /// <summary>
        /// Use this to indicate the query failed generally (though an unhandled exception)
        /// </summary>
        public static GenericErrorResult<T> Error(string reason)
            => new GenericErrorResult<T>(reason);

        /// <summary>
        /// Use this to indicate the query was not vaild (invalid data passed [HTTP 400])
        /// </summary>
        public static ValidationErrorResult<T> NotValid(IEnumerable<ValidationResult> validationErrors) =>
            new ValidationErrorResult<T>(validationErrors);

        /// <summary>
        /// Use this to indicate the query was not vaild (invalid data passed [HTTP 400])
        /// </summary>
        public static ValidationErrorResult<T> NotValid(params string[] reasons) =>
            new ValidationErrorResult<T>(reasons.Select(x => new ValidationResult(x)).ToArray());

        /// <summary>
        /// Use this to indicate the query was not allowed (user may not do this [HTTP 403])
        /// </summary>
        public static UnauthorizedResult<T> NotAllowed()
            => new UnauthorizedResult<T>();

        /// <summary>
        /// Use this to indicate the query could not find something (wrong ids and similar [HTTP 404])
        /// </summary>
        public static NotFoundResult<T> NotFound()
            => new NotFoundResult<T>();


        public static CanceledResult<T> Canceled(CancellationToken ct = default)
            => new CanceledResult<T>(ct);
    }
}
