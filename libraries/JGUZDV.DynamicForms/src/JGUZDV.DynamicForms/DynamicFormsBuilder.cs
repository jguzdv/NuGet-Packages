using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.DynamicForms;

/// <summary>
/// Builder for configuring dynamic forms in the application.
/// </summary>
public class DynamicFormsBuilder
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public DynamicFormsBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds a new FieldType to the dynamic forms configuration.
    /// </summary>
    public DynamicFormsBuilder AddFieldType(FieldType type)
    {
        DynamicFormsConfiguration.AddFieldType(type);
        return this;
    }

    /// <summary>
    /// Adds metadata for the specified FieldType using the specified metadata provider.
    /// </summary>
    public DynamicFormsBuilder AddMetadata<TFieldType, TMetadataProvider>()
        where TFieldType : FieldType
        where TMetadataProvider : class, IFieldTypeMetadataProvider
    {
        _services.TryAddScoped<TMetadataProvider>();
        _services.TryAddKeyedScoped<IFieldTypeMetadataProvider>(typeof(TFieldType).Name, (sp, key) =>
        {
            return sp.GetRequiredService<TMetadataProvider>();
        });

        return this;
    }

    /// <summary>
    /// Adds a value provider for the specified FieldType. The value provider will be used to provide the allowed/selectable values for the FieldType.
    /// </summary>
    /// <typeparam name="TFieldType"></typeparam>
    /// <typeparam name="TMetadataProvider"></typeparam>
    /// <returns></returns>
    public DynamicFormsBuilder AddValueProvider<TFieldType, TMetadataProvider>()
       where TFieldType : FieldType
       where TMetadataProvider : class, IFieldTypeValueProvider
    {
        _services.TryAddScoped<TMetadataProvider>();
        _services.TryAddKeyedScoped<IFieldTypeValueProvider>(typeof(TFieldType).Name, (sp, key) =>
        {
            return sp.GetRequiredService<TMetadataProvider>();
        });

        return this;
    }

    /// <summary>
    /// Removes a FieldType from the dynamic forms configuration.
    /// </summary>
    public DynamicFormsBuilder RemoveFieldType(FieldTypeId fieldTypeId)
    {
        DynamicFormsConfiguration.RemoveFieldType(fieldTypeId);
        return this;
    }

    /// <summary>
    /// Registers a new constraint type in the dynamic forms configuration. This allows the constraint to be used in field definitions.
    /// </summary>
    public DynamicFormsBuilder RegisterConstraintType<TConstraint>()
        where TConstraint : class, IConstraint
    {
        DynamicFormsConfiguration.AddConstraintType<TConstraint>();
        return this;
    }
}
