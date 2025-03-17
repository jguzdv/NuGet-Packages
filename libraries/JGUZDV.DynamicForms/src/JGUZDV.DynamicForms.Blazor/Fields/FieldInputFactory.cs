using JGUZDV.DynamicForms.Model;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.DynamicForms.Blazor.Fields;

//TODO: Check if Interface + DI has benefit for the factories
/// <summary>
/// Provides a factory for mapping fields to their corresponding view types.
/// </summary>
public static class FieldInputFactory
{
    private static readonly Dictionary<Type, Type> _viewTypes = new()
        {
            { typeof(FileFieldType), typeof(FileFieldInput) },
        };

    /// <summary>
    /// Gets the view type associated with the specified field.
    /// </summary>
    /// <param name="type">The field type to get the component type for.</param>
    /// <returns>The view type associated with the specified field.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the field type is unknown.</exception>
    public static Type GetViewType(FieldType type)
    {
        return _viewTypes.GetValueOrDefault(type.GetType())
            ?? typeof(DefaultFieldInput);
    }

    /// <summary>
    /// Sets the component type for the specified field type.
    /// </summary>
    public static void SetViewType<TFieldType, TComponentType>()
        where TFieldType : FieldType
        where TComponentType : ComponentBase
    {
        _viewTypes[typeof(TFieldType)] = typeof(TComponentType);
    }
}
