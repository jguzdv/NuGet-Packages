using JGUZDV.AspNetCore.Hosting;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Controllers;

var builder = JGUZDVHostApplicationBuilder.CreateWebApi(args);

var app = builder.BuildAndConfigure();

await app.RunAsync();

internal partial class Program { }
