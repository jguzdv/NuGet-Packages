using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.CQRS.Queries
{
    public class QueryResult<T>
    {
        public QueryResult(T value)
        {
            if (value == null)
                throw new ArgumentNullException("Value should never be null. Use a more significant QueryResultObject");

            Value = value;
            HandlerResult = HandlerResult.Success();
        }

        public QueryResult(HandlerResult result)
        {
            ArgumentNullException.ThrowIfNull(result, "Result cannot be null");
            if (result is SuccessResult)
                throw new ArgumentException("A query result cannot be success only, since it's meant to return data.");

            HandlerResult = result;
        }


        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue => Value != null;
        public T? Value { get; }

        public HandlerResult HandlerResult { get; }


        /// <summary>
        /// Use this to indicate the query completed successfully.
        /// </summary>
        public static QueryResult<T> Success(T result)
            => new QueryResult<T>(result);



        public static implicit operator QueryResult<T>(T value) => new(value);

        public static implicit operator QueryResult<T>(HandlerResult result) => new(result);

        public static implicit operator T?(QueryResult<T> queryResult) => queryResult.Value;
    }
}
