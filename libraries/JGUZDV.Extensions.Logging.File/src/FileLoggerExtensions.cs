using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

using JGUZDV.Extensions.Logging.File;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// Provides extension methods for the <see cref="ILoggingBuilder"/> and <see cref="ILoggerProviderConfiguration{FileLoggerProvider}"/> classes.
/// </summary>
[UnsupportedOSPlatform("browser")]
public static partial class FileLoggerExtensions
{
    internal const string RequiresDynamicCodeMessage = "Binding TOptions to configuration values may require generating dynamic code at runtime.";
    internal const string TrimmingRequiresUnreferencedCodeMessage = "TOptions's dependent types may have their members trimmed. Ensure all required members are preserved.";

    /// <summary>
    /// Adds a file logger named 'File' to the factory.
    /// If you call this yourself, also call AddFileFormatter to add a formatter.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<FileLoggerOptions>, FileLoggerConfigureOptions>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<FileLoggerOptions>, LoggerProviderOptionsChangeTokenSource<FileLoggerOptions, FileLoggerProvider>>());

        return builder;
    }

    /// <summary>
    /// Adds a file logger named 'File' to the factory.
    /// If you call this yourself, also call AddFileFormatter to add a formatter.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="FileLogger"/>.</param>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        builder.AddFile();
        builder.Services.Configure(configure);

        return builder;
    }


    /// <summary>
    /// Add the default file log and formatter with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddPlainTextFile(this ILoggingBuilder builder) 
        => AddPlainTextFile(builder, _ => { }, _ => { });

    /// <summary>
    /// Add and configure plain text file logger with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureLogger">A delegate to configure the <see cref="FileLogger"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddPlainTextFile(
        this ILoggingBuilder builder,
        Action<FileLoggerOptions> configureLogger)
            => AddPlainTextFile(builder, configureLogger, _ => { });


    /// <summary>
    /// Add and configure plain text file logger with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureFormatter">A delegate to configure the <see cref="PlainTextFileFormatter"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddPlainTextFile(
        this ILoggingBuilder builder,
        Action<PlainTextFileFormatterOptions> configureFormatter)
            => AddPlainTextFile(builder, _ => { }, configureFormatter);


    /// <summary>
    /// Add the default file log and formatter with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureLogger">A delegate to configure the <see cref="FileLogger"/> options for the built-in default log formatter.</param>
    /// <param name="configureFormatter">A delegate to configure the <see cref="PlainTextFileFormatter"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddPlainTextFile(
        this ILoggingBuilder builder, 
        Action<FileLoggerOptions> configureLogger, 
        Action<PlainTextFileFormatterOptions> configureFormatter)
    {
        builder.Services.TryAddSingleton<TimeProvider>(sp => TimeProvider.System);
        builder.AddFile(configureLogger);
        builder.AddFileFormatter<PlainTextFileFormatter, PlainTextFileFormatterOptions>(configureFormatter);
        builder.Services.Configure<FileLoggerOptions>(opt => opt.FileExtension ??= ".log");

        return builder;
    }




    /// <summary>
    /// Add the json file log and formatter with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddJsonFile(this ILoggingBuilder builder)
        => AddJsonFile(builder, _ => { }, _ => { });

    /// <summary>
    /// Add and configure json file logger with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureLogger">A delegate to configure the <see cref="FileLogger"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddJsonFile(
        this ILoggingBuilder builder,
        Action<FileLoggerOptions> configureLogger)
            => AddJsonFile(builder, configureLogger, _ => { });


    /// <summary>
    /// Add and configure json file logger with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureFormatter">A delegate to configure the <see cref="JsonFileFormatter"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddJsonFile(
        this ILoggingBuilder builder,
        Action<JsonFileFormatterOptions> configureFormatter)
            => AddJsonFile(builder, _ => { }, configureFormatter);


    /// <summary>
    /// Add the json file log and formatter with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureLogger">A delegate to configure the <see cref="FileLogger"/> options for the built-in default log formatter.</param>
    /// <param name="configureFormatter">A delegate to configure the <see cref="JsonFileFormatter"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddJsonFile(
        this ILoggingBuilder builder,
        Action<FileLoggerOptions> configureLogger,
        Action<JsonFileFormatterOptions> configureFormatter)
    {
        builder.Services.TryAddSingleton<TimeProvider>(sp => TimeProvider.System);
        builder.AddFile(configureLogger);
        builder.AddFileFormatter<JsonFileFormatter, JsonFileFormatterOptions>(configureFormatter);
        builder.Services.Configure<FileLoggerOptions>(opt => opt.FileExtension ??= ".log.json");

        return builder;
    }



    /// <summary>
    /// Adds a custom file logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(TrimmingRequiresUnreferencedCodeMessage)]
    public static ILoggingBuilder AddFileFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(this ILoggingBuilder builder)
        where TOptions : FileFormatterOptions
        where TFormatter : FileFormatter
    {
        return AddFileFormatter<TFormatter, TOptions, FileLoggerFormatterConfigureOptions<TFormatter, TOptions>>(builder);
    }

    /// <summary>
    /// Adds a custom file logger formatter 'TFormatter' to be configured with options 'TOptions'.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(TrimmingRequiresUnreferencedCodeMessage)]
    public static ILoggingBuilder AddFileFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(this ILoggingBuilder builder, Action<TOptions> configure)
        where TOptions : FileFormatterOptions
        where TFormatter : FileFormatter
    {
        ArgumentNullException.ThrowIfNull(configure);

        builder.AddFileFormatter<TFormatter, TOptions>();
        builder.Services.Configure(configure);
        return builder;
    }

    private static ILoggingBuilder AddFileFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConfigureOptions>(this ILoggingBuilder builder)
        where TOptions : FileFormatterOptions
        where TFormatter : FileFormatter
        where TConfigureOptions : class, IConfigureOptions<TOptions>
    {
        builder.AddConfiguration();

        builder.Services.Add(ServiceDescriptor.Singleton<FileFormatter, TFormatter>());
        builder.Services.Add(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, TConfigureOptions>());
        builder.Services.Add(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, FileLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());

        return builder;
    }

    internal static IConfiguration GetFormatterOptionsSection(this ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
    {
        return providerConfiguration.Configuration.GetSection("FormatterOptions");
    }
}

[UnsupportedOSPlatform("browser")]
internal sealed class FileLoggerFormatterConfigureOptions<TFormatter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions> : ConfigureFromConfigurationOptions<TOptions>
    where TOptions : FileFormatterOptions
    where TFormatter : FileFormatter
{
    [RequiresDynamicCode(FileLoggerExtensions.RequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(FileLoggerExtensions.TrimmingRequiresUnreferencedCodeMessage)]
    public FileLoggerFormatterConfigureOptions(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration) :
        base(providerConfiguration.GetFormatterOptionsSection())
    {
    }
}

[UnsupportedOSPlatform("browser")]
internal sealed class FileLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions> : ConfigurationChangeTokenSource<TOptions>
    where TOptions : FileFormatterOptions
    where TFormatter : FileFormatter
{
    public FileLoggerFormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
        : base(providerConfiguration.GetFormatterOptionsSection())
    {
    }
}
