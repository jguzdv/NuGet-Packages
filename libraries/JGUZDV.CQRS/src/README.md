# JGUZDV.CQRS

This implements a simple CQRS style command and query structure.
Combine it with (https://www.nuget.org/packages/JGUZDV.CQRS.AspNetCore)[JGUZDV.CQRS.AspNetCore] for simple usage in AspNetCore scenarios.

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
        return Task.FromResult(principal != null && principal.Name == "Johannes");
    }

    protected override Task<List<ValidationResult>> ValidateAsync(WeatherQuery query, ClaimsPrincipal? principal, CancellationToken ct)
    {
        var result = new List<ValidationResult>();
        if(query.Location == string.Empty) {
            result.Add(new ("Location may not be empty"));
        }

        return result;
    }

    protected override async Task<QueryResult<TValue>> ExecuteInternalAsync(WeatherQuery query, ClaimsPrincipal? principal, CancellationToken ct) {
        var result = await _weatherApi.GetWeatherOfLocation(query.Location, ct);

        return new QueryResult(result);
    }

    protected override Task<bool> AuthorizeValueAsync(WeatherQuery query, MyResult value, ClaimsPrincipal? principal, CancellationToken ct) 
    {
        return Task.FromResult(principal != null && value.DoesNotContainSecrets);
    }
}
```

**CommandSample**
```csharp
```

