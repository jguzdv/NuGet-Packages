using JGUZDV.Outbox.Email;

namespace JGUZDV.Outbox.Tests;

public sealed class OutboxMessageTests
{
    private static readonly DateTimeOffset DueDate =
        new(2026, 9, 24, 12, 30, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_SetsRequiredPropertiesAndDefaultsId()
    {
        var message = new OutboxMessage(DueDate, "UserRegistered", """{"userId":42}""");

        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(DueDate, message.DueDate);
        Assert.Equal("UserRegistered", message.MessageType);
        Assert.Equal("""{"userId":42}""", message.MessageData);
        Assert.Equal(default, message.RecordDate);
        Assert.Null(message.ProcessedDate);
        Assert.Null(message.HasBeenProcessed);
        Assert.Null(message.Tags);
        Assert.Null(message.MessageState);
        Assert.Null(message.Failures);
        Assert.Equal(0, message.FailCount);
    }

    [Fact]
    public void Create_SerializesMessageData()
    {
        var data = new UserRegisteredMessage(42, "user@example.com");

        var message = OutboxMessage.Create(DueDate, "UserRegistered", data);

        Assert.Equal(DueDate, message.DueDate);
        Assert.Equal("UserRegistered", message.MessageType);
        Assert.NotEmpty(message.MessageData);
        Assert.True(message.TryGetJsonMessageData<UserRegisteredMessage>(out var deserialized));
        Assert.NotNull(deserialized);
        Assert.Equal(data.UserId, deserialized.UserId);
        Assert.Equal(data.Email, deserialized.Email);
    }

    [Fact]
    public void TryGetJsonMessageData_ReturnsFalseForInvalidJson()
    {
        var message = new OutboxMessage(DueDate, "Invalid", "{not valid json");

        var result = message.TryGetJsonMessageData<UserRegisteredMessage>(out var data);

        Assert.False(result);
        Assert.Null(data);
    }

    [Fact]
    public void WithTags_String_SetsTagsAndReturnsSameMessage()
    {
        var message = new OutboxMessage(DueDate, "Test", "{}");

        var result = message.WithTags("user:42");

        Assert.Same(message, result);
        Assert.Equal("user:42", message.Tags);
    }

    [Fact]
    public void WithTags_Object_SerializesTagsAndCanReadThemBack()
    {
        var message = new OutboxMessage(DueDate, "Test", "{}");
        var tags = new MessageTags("user:42", "important");

        var result = message.WithTags(tags);

        Assert.Same(message, result);
        Assert.True(message.TryGetJsonTags<MessageTags>(out var deserialized));
        Assert.NotNull(deserialized);
        Assert.Equal(tags.User, deserialized.User);
        Assert.Equal(tags.Category, deserialized.Category);
    }

    [Fact]
    public void TryGetJsonTags_ReturnsFalseWhenTagsAreMissing()
    {
        var message = new OutboxMessage(DueDate, "Test", "{}");

        var result = message.TryGetJsonTags<MessageTags>(out var tags);

        Assert.False(result);
        Assert.Null(tags);
    }

    [Fact]
    public void CreatePlainTextEmail_CreatesEmailMessage()
    {
        var message = OutboxMessage.CreatePlainTextEmail(
            DueDate,
            "Welcome",
            "Welcome to the service.",
            ["to@example.com"],
            ["cc@example.com"],
            ["bcc@example.com"]);

        Assert.Equal(Constants.MessageType, message.MessageType);
        Assert.True(message.TryGetEmailMessageData(out var email));
        Assert.NotNull(email);
        Assert.Equal("Welcome", email.Subject);
        Assert.Equal("Welcome to the service.", email.Body);
        Assert.Equal("text/plain", email.BodyContentType);
        Assert.Equal(["to@example.com"], email.EmailRecipients.To);
        Assert.Equal(["cc@example.com"], email.EmailRecipients.Cc);
        Assert.Equal(["bcc@example.com"], email.EmailRecipients.Bcc);
    }

    [Fact]
    public void CreateHtmlEmail_SetsHtmlContentType()
    {
        var message = OutboxMessage.CreateHtmlEmail(
            DueDate,
            "Welcome",
            "<h1>Welcome</h1>",
            ["to@example.com"]);

        Assert.True(message.TryGetEmailMessageData(out var email));
        Assert.NotNull(email);
        Assert.Equal("<h1>Welcome</h1>", email.Body);
        Assert.Equal("text/html", email.BodyContentType);
        Assert.Null(email.EmailRecipients.Cc);
        Assert.Null(email.EmailRecipients.Bcc);
    }

    [Fact]
    public void TryGetEmailMessageData_ThrowsForNonEmailMessage()
    {
        var message = new OutboxMessage(DueDate, "UserRegistered", "{}");

        Assert.Throws<InvalidOperationException>(
            () => message.TryGetEmailMessageData(out _));
    }

    [Fact]
    public async Task EmailMessageDataFactory_CreatesEmailData()
    {
        var message = OutboxMessage.CreatePlainTextEmail(
            DueDate,
            "Subject",
            "Body",
            ["to@example.com"]);

        var factory = new EmailMessageDataFactory();

        Assert.True(await factory.CanCreateMessageAsync(message, CancellationToken.None));

        var email = await factory.CreateMessageAsync(message, CancellationToken.None);

        Assert.Equal("Subject", email.Subject);
        Assert.Equal("Body", email.Body);
        Assert.Equal("text/plain", email.BodyContentType);
    }

    [Fact]
    public async Task EmailMessageDataFactory_RejectsNonEmailMessages()
    {
        var message = new OutboxMessage(DueDate, "Other", "{}");
        var factory = new EmailMessageDataFactory();

        Assert.False(await factory.CanCreateMessageAsync(message, CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => factory.CreateMessageAsync(message, CancellationToken.None));
    }

    private sealed record UserRegisteredMessage(int UserId, string Email);

    private sealed record MessageTags(string User, string Category);
}