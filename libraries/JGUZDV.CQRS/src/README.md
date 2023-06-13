# JGUZDV.CQRS

This implements a simple CQRS style command and query structure.
Combine it with [JGUZDV.CQRS.AspNetCore](https://www.nuget.org/packages/JGUZDV.CQRS.AspNetCore) for simple usage in AspNetCore scenarios.

The package contains classes describing a `HandlerResult`, a basic `QueryHandler` and a `CommandHandler` as well as interfaces
for queries, commands, and the aforementioned handlers.

Both handlers will run a 'default pipeline' that allows you to provide initialization code (commands only), command or query object normalization, 
authorization, validation, execution and result authorization (queries only).  
You can return results, that will easily translate to http results from the execution method of both handlers and you may throw `CommandExceptions` in command handlers
to stop the handler running during any point in the pipeline. It's written async only and will check your `CancellationTokens` between all steps.

Command handlers will faciliate context objects to better handle nullability, that can be used, if initialization is necessary.
Authorization of commands will return `false`, if you do not override them, whilst authorization of queries will return `true`.
Validation will in both cases return `valid` (e.g: an empty list).

**QuerySample**
```csharp
using JGUZDV.CQRS;

namespace MyNamespace;

public class WeatherQuery : IQuery<MyResult> {
    public string Location { get; set; }
}

public class WeatherQueryHandler : QueryHandler<WeatherQuery, MyResult> {
    private IWeatherApi _weatherApi;
    private ILogger<WeatherQueryHandler> _logger;

    // You need to override the logger
    public override ILogger Logger => _logger;
    
    public WeatherQueryHandler(IWeatherApi weatherApi, ILogger<WeatherQueryHandler> logger)
    {
        _weatherApi = weatherApi;
        _logger = logger;
    }

    protected override NormalizeQuery(WeatherQuery query, ClaimsPrincipal? principal)
    {
        query.Location = query.Location.Trim();
        return query;
    }

    protected override Task<bool> AuthorizeExecuteAsync(WeatherQuery query, ClaimsPrincipal? principal, CancellationToken ct) 
    {
        // If you choose to not override this, it will automatically return `true`
        return Task.FromResult(principal != null && principal.Name == "Johannes");
    }

    protected override Task<List<ValidationResult>> ValidateAsync(WeatherQuery query, ClaimsPrincipal? principal, CancellationToken ct)
    {
        // If you choose to not override this, it will automatically be valid.
        var result = new List<ValidationResult>();
        if(query.Location == string.Empty) {
            result.Add(new ("Location may not be empty"));
        }

        return result;
    }

    // You need to override at least this method
    protected override async Task<QueryResult<TValue>> ExecuteInternalAsync(WeatherQuery query, ClaimsPrincipal? principal, CancellationToken ct) {
        var result = await _weatherApi.GetWeatherOfLocation(query.Location, ct);

        return new QueryResult(result);
    }

    protected override Task<bool> AuthorizeValueAsync(WeatherQuery query, MyResult value, ClaimsPrincipal? principal, CancellationToken ct) 
    {
        // If you choose to not override this, it will automatically return `true`
        return Task.FromResult(principal != null && value.DoesNotContainSecrets);
    }
}
```

**CommandSample**
```csharp
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using Microsoft.Extensions.Logging;

namespace JGUZDV.CQRS.Commands;

// Sample class - DBSet<Counter> Counters
public class Counter
{
    public int Id { get; set; }

    public int Value { get; set; }
    public string Reasons { get; set; }
}


// Command object
public class IncrementCounter : ICommand
{
    public IncrementCounter(int counterId, int incValue, string reason)
    {
        CounterId = counterId;
        IncValue = incValue;
        Reason = reason;
    }

    public int CounterId { get; }
    public int IncValue { get; }
    public string Reason { get; }
}


internal class IncrementCounterHandler : CommandHandler<IncrementCounter, IncrementCounterHandler.CounterContext>
{
    private readonly CounterDb _dbContext;
    private readonly ILogger<IncrementCounterHandler> _logger;

    public override ILogger Logger => _logger;

    public IncrementCounterHandler(CounterDb dbContext, ILogger<IncrementCounterHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    protected override async Task<CounterContext> InitializeAsync(IncrementCounter command, ClaimsPrincipal? principal, CancellationToken ct)
    {
        var counter = await _dbContext.Counters.FirstOrDefaultAsync(command.CounterId, ct);
        if (counter == null)
            throw new CommandException(HandlerResult.NotFound());

        return new CounterContext(counter);
    }

    protected override IncrementCounter NormalizeCommand(IncrementCounter command, CounterContext context, ClaimsPrincipal? principal)
        => new IncrementCounter(
            command.CounterId,
            command.IncValue,
            command.Reason.Trim()
        );

    protected override Task<bool> AuthorizeAsync(IncrementCounter command, CounterContext context, ClaimsPrincipal? principal, CancellationToken ct)
        // If you do not override authorize async, authorization of the command will fail. You can skip authZ by setting `SkipAuthorization` to `true`
        => Task.FromResult(principal?.HasClaim(c => c.Type == "CounterAccess" && c.Value == $"{command.CounterId}") ?? false);


    protected override Task<List<ValidationResult>> ValidateAsync(IncrementCounter command, CounterContext context, ClaimsPrincipal? principal, CancellationToken ct)
    {
        // If you do not override validation, the command will be considered valid.
        var result = new List<ValidationResult>();

        
        if (command.IncValue <= 0)
            result.Add(new("Value may not be negative"));

        if(command.IncValue > 10)
            result.Add(new("Value needs to be <= 10"));

        
        return Task.FromResult(result);
    }


    protected override async Task<HandlerResult> ExecuteInternalAsync(IncrementCounter command, CounterContext context, ClaimsPrincipal? principal, CancellationToken ct)
    {
        if (context.Counter.Value >= 100)
            return HandlerResult.Conflict("Counter already at maximum");

        context.Counter.Value += command.IncValue;
        if (!string.IsNullOrEmpty(command.Reason))
            context.Counter.Reasons += $"\r\n{command.Reason}";

        await _dbContext.SaveChangesAsync(ct);

        return HandlerResult.Success();

        //If you would want to communication successfull creation, there is:
        return HandlerResult.Created<int>(context.Counter.Id);
    }


    // Command Context object (not neccessarily nested)
    public class CounterContext
    {
        public Counter Counter { get; set; }

        public CounterContext(Counter c)
        {
            Counter = c;
        }
    }
}
```

