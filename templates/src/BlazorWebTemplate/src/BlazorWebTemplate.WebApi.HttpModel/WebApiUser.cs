namespace ZDV.BlazorWebTemplate.WebApi.HttpModel;

public record WebApiUser(IEnumerable<WebApiClaim> Claims);

public record WebApiClaim(string Type, string Value);
