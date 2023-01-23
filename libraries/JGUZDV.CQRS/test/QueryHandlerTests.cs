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

            var result = await sut.ExecuteAsync(query, null, default);

            Assert.True(result.HasResult);

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

            var result = await sut.ExecuteAsync(query, null, default);

            Assert.False(result.HasResult);
            Assert.IsType<UnauthorizedResult<TestResult>>(result);
        }

        [Fact]
        public async Task Returns_Unauthorized_If_NotResultAuthorized()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, true, false, true, new());

            var result = await sut.ExecuteAsync(query, null, default);

            Assert.False(result.HasResult);
            Assert.IsType<UnauthorizedResult<TestResult>>(result);
        }


        [Fact]
        public async Task Returns_ValidationError_If_NotValid()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, false, true, true, new());

            var result = await sut.ExecuteAsync(query, null, default);

            Assert.False(result.HasResult);
            Assert.IsType<ValidationErrorResult<TestResult>>(result);
        }

        [Fact]
        public async Task Returns_GenericError_On_Error()
        {
            var sut = new TestQueryHandler();
            var query = new TestQuery(true, true, true, false, new());

            var result = await sut.ExecuteAsync(query, null, default);

            Assert.False(result.HasResult);
            Assert.IsType<GenericErrorResult<TestResult>>(result);
        }
    }
}
