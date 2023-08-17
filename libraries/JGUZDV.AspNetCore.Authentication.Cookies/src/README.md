# JGUZDV.AspNetCore.Authentication.Cookies

This package provides a SessionStore (aka TicketStore) based on IDistributedCache and can be used to move
authentication cookie contents from the cookie to a distributed cache.

```csharp
services.AddAuthentication()
    .AddCookies()
    .AddCookieDistributedTicketStore();
```
