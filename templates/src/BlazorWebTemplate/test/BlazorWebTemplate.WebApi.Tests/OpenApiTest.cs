namespace BlazorWebTemplate.WebApi.Tests;

public class OpenApiTest : IClassFixture<DefaultWebApplicationFactory>
{
    private readonly DefaultWebApplicationFactory _factory;

    public OpenApiTest(DefaultWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OpenApi_Document_Is_Reachable()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("openapi/v1.json");

        Assert.True(response.IsSuccessStatusCode, "OpenAPI document is not reachable.");
    }
}
