using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.CQRS.Queries;

public interface IQuery { }
public interface IQuery<T> : IQuery {
    [DisallowNull]
    QueryResult<T>? Result { get; set; }
}
