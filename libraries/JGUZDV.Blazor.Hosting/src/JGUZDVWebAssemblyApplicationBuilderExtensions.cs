﻿using JGUZDV.AspNetCore.Components.Localization;
using JGUZDV.Blazor.Hosting.FeatureManagement;
using JGUZDV.Blazor.Hosting.Localization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace JGUZDV.Blazor.Hosting;

/// <summary>
/// Extension methods for the JGUZDVWebAssemblyApplicationBuilder.
/// </summary>
public static class JGUZDVWebAssemblyApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the required services for authorization.
    /// AuthenticationState will be handled via AuthenticationStateDeserialization.
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder AddAuthoriztion(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddAuthorizationCore();
        appBuilder.Services.AddCascadingAuthenticationState();
        appBuilder.Services.AddAuthenticationStateDeserialization();

        return appBuilder;
    }


    /// <summary>
    /// Adds the required services for localization.
    /// The current language and allowed languages will be handled via LocalizationStateDeserialization.
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder AddLocalization(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddLocalization();
        appBuilder.Services.AddScoped<ILanguageService, PersistentStateLanguageService>();

        return appBuilder;
    }

    /// <summary>
    /// Adds the required services for feature management.
    /// The feature states will be read from the the PersistentComponentState.
    /// </summary>
    public static JGUZDVWebAssemblyApplicationBuilder AddPersistedFeatureManagement(this JGUZDVWebAssemblyApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddFeatureManagement();
        appBuilder.Services.AddScoped<IFeatureDefinitionProvider, PersistentStateFeatureDefinitionProvider>();
        return appBuilder;
    }
}
