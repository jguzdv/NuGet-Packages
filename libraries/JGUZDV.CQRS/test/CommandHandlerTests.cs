using JGUZDV.CQRS.Commands;
using Xunit;

namespace JGUZDV.CQRS.Tests
{
    public class CommandHandlerTests
    {
        [Fact]
        public async Task Execution_Order_Is_Correct()
        {
            var sut = new TestCommandHandler(false);
            var command = new TestCommand(true, true, true, HandlerResult.Success());

            var result = await sut.ExecuteAsync(command, null, default);

            Assert.True(result.IsSuccess);

            Assert.Equal(5, command.Methods.Count);
            Assert.Equal(TestCommandHandler.InitializeAsyncMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.NormalizeCommandMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.AuthorizeAsyncMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.ValidateAsyncMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.ExecuteInternalAsyncMethod, command.Methods.Dequeue());
        }

        [Fact]
        public async Task Returns_Unauthorized_If_NotAuthorized()
        {
            var sut = new TestCommandHandler(false);
            var command = new TestCommand(false, true, true, HandlerResult.Success());

            var result = await sut.ExecuteAsync(command, null, default);

            Assert.False(result.IsSuccess);
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Authorize_Will_Be_Skipped()
        {
            var sut = new TestCommandHandler(true);
            var command = new TestCommand(false, true, true, HandlerResult.Success());

            var result = await sut.ExecuteAsync(command, null, default);

            Assert.True(result.IsSuccess);

            Assert.Equal(4, command.Methods.Count);
            Assert.Equal(TestCommandHandler.InitializeAsyncMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.NormalizeCommandMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.ValidateAsyncMethod, command.Methods.Dequeue());
            Assert.Equal(TestCommandHandler.ExecuteInternalAsyncMethod, command.Methods.Dequeue());
        }

        [Fact]
        public async Task Returns_ValidationError_If_NotValid()
        {
            var sut = new TestCommandHandler(false);
            var command = new TestCommand(true, false, true, HandlerResult.Success());

            var result = await sut.ExecuteAsync(command, null, default);

            Assert.False(result.IsSuccess);
            Assert.IsType<ValidationErrorResult>(result);
        }

        [Fact]
        public async Task Returns_GenericError_On_Error()
        {
            var sut = new TestCommandHandler(false);
            var command = new TestCommand(true, true, false, HandlerResult.Success());

            var result = await sut.ExecuteAsync(command, null, default);

            Assert.False(result.IsSuccess);
            Assert.IsType<GenericErrorResult>(result);
        }
    }
}
