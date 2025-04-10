using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JGUZDV.Extensions.Authorization.Tests
{
    public class OpenApiDocumentTest : IDisposable
    {
        private readonly IHost _openApiTestHost;

        public OpenApiDocumentTest()
        {
            _openApiTestHost = GetFakeExternalBindingServer();
        }

        internal IHost GetFakeExternalBindingServer()
        {
            var builder = WebApplication.CreateSlimBuilder();

            builder.WebHost.UseTestServer();
            builder.Services.AddRouting();
            builder.Services.AddOpenApi();

            var app = builder.Build();



            app.UseRouting();
            app.MapOpenApi();

            app.MapPost("/claim-requirement", (HttpContext ctx, Request request) =>
            {
                return TypedResults.Ok(new Response { Requirement = request.ClaimRequirement});
            });

            app.Start();
            return app;
        }

        public class Request
        {
            public required ClaimRequirement ClaimRequirement { get; set; }
        }

        public class Response
        {
            public required ClaimRequirement Requirement { get; set; }
        }


        [Fact]
        public async Task Open_API_Documents_Can_Be_Created()
        {
            var httpClient = _openApiTestHost.GetTestClient();
            var result = await httpClient.GetStringAsync("/openapi/v1.json");

            Assert.NotNull(result);
        }



        public void Dispose()
        {
            _openApiTestHost.Dispose();
        }
    }
}
