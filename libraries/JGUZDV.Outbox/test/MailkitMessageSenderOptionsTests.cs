using System.Text;

using JGUZDV.Outbox.Email.Mailkit;

using MailKit.Security;

using Microsoft.Extensions.Configuration;

namespace JGUZDV.Outbox.Tests;

public sealed class MailkitMessageSenderOptionsTests
{
    [Fact]
    public void Configuration_BindsInlineJsonToOptions()
    {
        const string json = """
            {
              "Mailkit": {
              "FromAddress" : "\"Outbox Sender\"<outbox@example.com>",
              "SmtpHost": "smtp.example.com",
              "SmtpPort": 465,
              "SmtpUsername": "smtp-user",
              "SmtpPassword": "test-password",
              "SocketOptions": "SslOnConnect",
              "RetryDelay": 250,
              "MaxSendAttempts": 5
              }
            }
            """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        using var configuration = new ConfigurationManager();
        configuration.AddJsonStream(stream);

        var options = configuration.GetSection("Mailkit").Get<MailkitMessageSenderOptions>();


        Assert.NotNull(options);
        Assert.Empty(options.Validate(new(options)));
        Assert.NotNull(options.FromAddress);
        Assert.Equal("smtp.example.com", options.SmtpHost);
        Assert.Equal(465, options.SmtpPort);
        Assert.Equal("smtp-user", options.SmtpUsername);
        Assert.Equal("test-password", options.SmtpPassword);
        Assert.Equal(SecureSocketOptions.SslOnConnect, options.SocketOptions);
        Assert.Equal(250, options.RetryDelay);
        Assert.Equal(5, options.MaxSendAttempts);
    }
}
