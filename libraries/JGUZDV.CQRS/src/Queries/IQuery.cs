namespace JGUZDV.CQRS.Queries;

public interface IQuery { }
public interface IQuery<T> : IQuery {
    QueryResult<T> Result { get; set; }
}
