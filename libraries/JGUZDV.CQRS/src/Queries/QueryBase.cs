namespace JGUZDV.CQRS.Queries;

/// <summary>
/// Base query implementation that already contains a result of T
/// </summary>
public abstract record QueryBase<T> : IQuery<T>
{
    /// <summary>
    /// The result of the query.
    /// </summary>
    public QueryResult<T> Result { get; set; } = HandlerResult.Indeterminate();
}
