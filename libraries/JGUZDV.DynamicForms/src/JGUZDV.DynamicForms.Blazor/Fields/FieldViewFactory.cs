using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Blazor.Fields;

//TODO: Check if Interface + DI has benefit for the factories
/// <summary>
/// Provides a factory for mapping fields to their corresponding view types.
/// </summary>
public static class FieldViewFactory
{
    private static readonly Dictionary<Type, Type> _viewTypes = new()
        {
            { typeof(FileFieldType), typeof(FileFieldInput) },
        };

    /// <summary>
    /// Gets the view type associated with the specified field.
    /// </summary>
    /// <param name="field">The field to get the view type for.</param>
    /// <returns>The view type associated with the specified field.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the field type is unknown.</exception>
    public static Type GetViewType(Field field)
    {
        return _viewTypes.GetValueOrDefault(field.FieldDefinition.Type?.GetType())
            ?? typeof(DefaultFieldInput);
    }

    /// <summary>
    /// Sets the view type for the specified field.
    /// </summary>
    /// <param name="field">The field definition to set the view type for.</param>
    /// <param name="viewType">The view type to associate with the field definition.</param>
    public static void SetViewType(Field field, Type viewType)
    {
        if (field.FieldDefinition.Type == null)
        {
            throw new InvalidOperationException("FieldDefinition Type cannot be null");
        }

        _viewTypes[field.FieldDefinition.Type.GetType()] = viewType;
    }
}
