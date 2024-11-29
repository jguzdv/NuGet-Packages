using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.Blazor.Components.Localization;

public class LanguageAwareMessageHandler : DelegatingHandler
{
    private readonly LanguageService _languageService;

    private string? _language;

    public LanguageAwareMessageHandler(LanguageService languageService)
    {
        _languageService = languageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_language == null)
        {
            _language = await _languageService.GetCurrentLanguage();
        }

        request.Headers.AcceptLanguage.Clear();
        request.Headers.AcceptLanguage.Add(new(_language));

        return await base.SendAsync(request, cancellationToken);
    }
}

public static class LanguageAwareMessageHandlerExtensions
{
    public static IHttpClientBuilder AddMessageHandlerWithLanguageHeader(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<LanguageAwareMessageHandler>();
}