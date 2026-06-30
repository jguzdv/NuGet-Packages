using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.AspNetCore.TestTools.FakeAuth;

/// <summary>
/// Provides extension methods for adding fake authentication claims to HTTP request messages and reading them from HTTP headers.
/// </summary>
public static class FakeAuthClaimsExtension
{
    /// <summary>
    /// Adds fake authentication claims to the HTTP client's default request headers by encoding them into a default header.
    /// </summary>
    public static void AddFakeAuthClaims(this HttpClient client, params FakeClaim[]? fakeAuthClaims)
        => client.AddFakeAuthClaims(FakeAuthDefaults.ClaimsHeaderName, fakeAuthClaims);

    /// <summary>
    /// Adds fake authentication claims to the HTTP client's default request headers by encoding them into a specified header.
    /// </summary>
    public static void AddFakeAuthClaims(this HttpClient client, string headerName, params FakeClaim[]? fakeAuthClaims)
    {
        if (fakeAuthClaims == null)
            return;

        client.DefaultRequestHeaders.AddFakeAuthClaims(headerName, fakeAuthClaims);
    }

    /// <summary>
    /// Adds fake authentication claims to the request message by encoding them into a default header.
    /// </summary>
    public static HttpRequestMessage AddFakeAuthClaims(
        this HttpRequestMessage message,
        params FakeClaim[]? fakeAuthClaims)
        => message.AddFakeAuthClaims(FakeAuthDefaults.ClaimsHeaderName, fakeAuthClaims);

    /// <summary>
    /// Adds fake authentication claims to the request message by encoding them into a specified header. 
    /// Each claim is represented as a string in the format "Type::Value". 
    /// This allows for simulating authenticated requests in testing scenarios without requiring actual authentication mechanisms.
    /// </summary>
    public static HttpRequestMessage AddFakeAuthClaims(
        this HttpRequestMessage message,
        string headerName,
        params FakeClaim[]? fakeAuthClaims)
    {
        if (fakeAuthClaims == null)
            return message;

        message.Headers.AddFakeAuthClaims(headerName, fakeAuthClaims);

        return message;
    }

    /// <summary>
    /// Adds fake authentication claims to the HTTP request headers by encoding them into a default header.
    /// </summary>
    public static void AddFakeAuthClaims(this HttpRequestHeaders headers, params FakeClaim[]? fakeAuthClaims)
        => AddFakeAuthClaims(headers, FakeAuthDefaults.ClaimsHeaderName, fakeAuthClaims);

    /// <summary>
    /// Adds fake authentication claims to the HTTP request headers by encoding them into a specified header.
    /// </summary>
    public static void AddFakeAuthClaims(this HttpRequestHeaders headers, string headerName, params FakeClaim[]? fakeAuthClaims)
    {
        if (fakeAuthClaims == null)
            return;

        var claimValues = fakeAuthClaims
            .Select(x => $"{x.Type}::{x.Value}");

        headers.MergeHeaders(headerName, claimValues);
    }


    private static void MergeHeaders(this HttpRequestHeaders headers, string headerName, IEnumerable<string> values)
    {
        if (headers.TryGetValues(headerName, out var currentValues))
        {
            headers.Remove(headerName);
        }

        currentValues ??= Enumerable.Empty<string>();
        headers.Add(headerName, values.Union(currentValues));
    }

    internal static IEnumerable<FakeClaim> ReadClaimsHeader(this IHeaderDictionary headers)
    {
        return !headers.TryGetValue(FakeAuthDefaults.ClaimsHeaderName, out var claimValues)
            ? []
            : claimValues
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.TrimSplit())
                .Where(x => x.Length == 2)
                .Select(x => new FakeClaim(x.First(), x.Last()))
                .ToList();
    }

    private static string[] TrimSplit(this string s) =>
        s.Split("::", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
}
