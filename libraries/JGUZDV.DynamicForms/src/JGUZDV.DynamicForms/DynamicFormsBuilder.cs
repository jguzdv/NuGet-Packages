using JGUZDV.DynamicForms.Model;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JGUZDV.DynamicForms;

public class DynamicFormsBuilder
{
    private readonly IServiceCollection _services;

    public DynamicFormsBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public DynamicFormsBuilder AddFieldType(FieldType type, List<Type> allowedConstraints)
    {
        DynamicFormsConfiguration.AddFieldType(type, allowedConstraints);
        return this;
    }

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

    public DynamicFormsBuilder RemoveFieldType<TFieldType>()
        where TFieldType : FieldType
    {
        DynamicFormsConfiguration.RemoveFieldType<TFieldType>();
        return this;
    }

    /// <summary>
    /// Sets the allowed constraint types for a given FieldType. If a new constraint type is added you must also set the name of the constraint via <see cref="SetConstraintName(Type, L10nString)"/>.
    /// </summary>
    public DynamicFormsBuilder SetConstraintTypes(FieldType fieldType, List<Type> constraintType)
    {
        DynamicFormsConfiguration.SetConstraintTypes(fieldType, constraintType);
        return this;
    }

    /// <summary>
    /// Sets the name of a constraint type.
    /// </summary>
    public DynamicFormsBuilder SetConstraintName(Type constraintType, L10nString name)
    {
        DynamicFormsConfiguration.SetConstraintName(constraintType, name);
        return this;
    }
}
