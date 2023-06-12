# JGUZDV.CQRS.AspNetCore

This provides extensions to easily call and return (https://www.nuget.org/packages/JGUZDV.CQRS.AspNetCore)[JGUZDV.CQRS] handler results via AspNetCore MVC or minimal API.

**MVC**
```csharp
using JGUZDV.CQRS.AspNetCore.Mvc;

public class MyController : MVC.Controller 
{
    public MyController(IStringLocalizer<MyResource> loc) {
        _loc = loc;
    }

    public async Task<IActionResult> MyAction(IQueryHandler<MyQuery> myHandler) {
        var query = new MyQuery();
        var result = await myHandler.ExecuteAsync<MyQuery, MyResult>(query, this, _loc); // This will automatically provide the User and CancellationToken from the HttpContext of the controller.

        return result.ToActionResult(_loc); //This will translate the result into an action result.
    }

    public async Task<IActionResult> MyAction(ICommandHandler<MyCommand> myHandler) {
        var command = new MyCommand();
        var result = await myHandler.ExecuteAsync(command, this, _loc); // This will automatically provide the User and CancellationToken from the HttpContext of the controller.

        return result.ToActionResult(_loc); //This will translate the result into an action result.
    }
}
```

**Minimal API**
```csharp
using JGUZDV.CQRS.AspNetCore.Http;

public async Task<IResult> MyAction(IQueryHandler<MyQuery> myHandler, HttpContext context) {
        var query = new MyQuery();
        var result = await myHandler.ExecuteAsync<MyQuery, MyResult>(query, this, _loc); // This will automatically provide the User and CancellationToken from the HttpContext of the controller.

        return result.ToActionResult(_loc); //This will translate the result into an action result.
    }

    public async Task<IResult> MyAction(ICommandHandler<MyCommand> myHandler) {
        var command = new MyCommand();
        var result = await myHandler.ExecuteAsync(command, this, _loc); // This will automatically provide the User and CancellationToken from the HttpContext of the controller.

        return result.ToActionResult(_loc); //This will translate the result into an action result.
    }
```
