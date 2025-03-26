namespace JGUZDV.CQRS.Queries;

/// <summary>
/// Defines a query that can be executed.
/// </summary>
public interface IQuery { }

/// <summary>
/// Defines a query that can be executed and has a result.
/// </summary>
/// <typeparam name="T">The result type of the query.</typeparam>
public interface IQuery<T> : IQuery {
    /// <summary>
    /// The result of the query.
    /// </summary>
    QueryResult<T> Result { get; set; }
}
