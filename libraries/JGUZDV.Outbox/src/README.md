# JGUZDV.Outbox

This library provides an implementation of the Outbox pattern for .NET applications. The Outbox pattern is a design pattern that helps ensure reliable message delivery in distributed systems by storing messages before they are sent.

## How to use

The library provides two main components: the recorder and the sender.  
The recorder is responsible for storing messages, while the sender is responsible for sending the messages to their intended recipients.

## Setting up with EFCore and Mailkit (both included)

If you want to use the included EntityFrameworkCore based storage, you need to add it to your DbContext.  
You can do this by calling the `UseOutbox` extension method on your `ModelBuilder`:

```csharp
public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseOutbox();
    }
}
```

### Message Recorder

To use the recorder, register it in your DI container:
```csharp
services.AddOutboxRecorder()
    .UseDbContextRecorder<MyDbContext>();
```

And use it in your code like this:
```csharp
internal class SomeRecordingClass(
        IOutboxMessageRecorder outboxMessageRecorder,
    )
{
    private readonly IOutboxMessageRecorder _outboxMessageRecorder = outboxMessageRecorder;

    protected override async Task ExecuteInternalAsync(CancellationToken ct)
    {
        await _outboxMessageRecorder.EnqueueMessageAsync(
            OutboxMessage.CreatePlainTextEmail(
                DateTimeOffset.UtcNow,
                "MailSubject",
                "MailBody",
                ["some@example.com"]
            )
            .WithTags("SomeTag")
            , ct);
    }
}
```

### Message Sender

To register the sender use the following code:
```csharp
services.AddOutboxSender()
    .UseDbContextMessageStorage<MyDbContext>()
    .AddDefaultEmailSender(opt => { /*configure mail settings here*/ });
```

Then inject the worker into your code and call the `ExecuteAsync` method to send the messages (the sample `IJob` uses Quartz.NET):
```csharp
internal class MailJob : IJob
{
    private readonly OutboxWorker _worker;

    public MailJob(OutboxWorker worker)
    {
        _worker = worker;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _worker.ExecuteAsync(context.CancellationToken);
    }
}
```