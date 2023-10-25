using JGUZDV.CQRS.Queries;
using Xunit;

namespace JGUZDV.CQRS.Tests
{
    public class QueryHandlerTests
    {
        [Fact]
        public async Task Execution_Order_Is_Correct()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, true, true, true, new());

            await sut.ExecuteAsync(query, null, default);

            Assert.True(query.Result.HasValue);

            Assert.Equal(5, query.Methods.Count);
            Assert.Equal(TestQueryHandler.NormalizeQueryMethod, query.Methods.Dequeue());
            Assert.Equal(TestQueryHandler.AuthorizeQueryAsyncMethod, query.Methods.Dequeue());
            Assert.Equal(TestQueryHandler.ValidateAsyncMethod, query.Methods.Dequeue());
            Assert.Equal(TestQueryHandler.ExecuteInternalAsyncMethod, query.Methods.Dequeue());
            Assert.Equal(TestQueryHandler.AuthorizeResultAsyncMethod, query.Methods.Dequeue());
        }

        [Fact]
        public async Task Returns_Unauthorized_If_NotQueryAuthorized()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(false, true, true, true, new());

            await sut.ExecuteAsync(query, null, default);

            Assert.NotNull(query.Result);
            Assert.IsType<UnauthorizedResult>(query.Result?.HandlerResult);
        }

        [Fact]
        public async Task Returns_Unauthorized_If_NotResultAuthorized()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, true, false, true, new());

            await sut.ExecuteAsync(query, null, default);

            Assert.NotNull(query.Result);
            Assert.IsType<UnauthorizedResult>(query.Result?.HandlerResult);
        }


        [Fact]
        public async Task Returns_ValidationError_If_NotValid()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, false, true, true, new());

            await sut.ExecuteAsync(query, null, default);

            Assert.NotNull(query.Result);
            Assert.IsType<ValidationErrorResult>(query.Result?.HandlerResult);
        }

        [Fact]
        public async Task Returns_GenericError_On_Error()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, true, true, false, new());

            await sut.ExecuteAsync(query, null, default);

            Assert.NotNull(query.Result);
            Assert.IsType<GenericErrorResult>(query.Result?.HandlerResult);
        }

        [Fact]
        public async Task Unwrapping_Return_Works()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, true, true, true, new());

            var queryResult = await sut.ExecuteQuery(query, null, default);
            Assert.NotNull(queryResult?.HandlerResult);
            Assert.True(queryResult?.HasValue);
        }
    }
}
