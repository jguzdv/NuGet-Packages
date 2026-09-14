
using System.Text.Json;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms
{
    // The types and relationships (FieldTypes, Constraints) should not be dynamic but extensible by consumer, hence static class
    // TODO: should this logic be placed in Constraint and FieldType base classes?
    /// <summary>
    /// Provides configuration for the dynamic forms library.
    /// </summary>
    public static class DynamicFormsConfiguration
    {
        static DynamicFormsConfiguration()
        {
            _registeredFieldTypes = new BaseFieldType[] {
                BoolFieldType.Instance,
                DateOnlyFieldType.Instance,
                FileFieldType.Instance,
                FloatFieldType.Instance,
                IntFieldType.Instance,
                StringFieldType.Instance,
                TimeOnlyFieldType.Instance
            }.ToDictionary(x => x.TypeId);

            _registeredConstraintTypes = new Dictionary<ConstraintId, Type>
            {
                { RegexConstraint.ConstraintId, typeof(RegexConstraint) },
                { StringLengthConstraint.ConstraintId, typeof(StringLengthConstraint) },
                { RangeConstraint.ConstraintId, typeof(RangeConstraint) },
                { SizeConstraint.ConstraintId, typeof(SizeConstraint) },
                { FileSizeConstraint.ConstraintId, typeof(FileSizeConstraint) }
            };
        }

        private static readonly Dictionary<FieldTypeId, BaseFieldType> _registeredFieldTypes;
        private static readonly Dictionary<ConstraintId, Type> _registeredConstraintTypes;
        

        /// <summary>
        /// Gets the list of registered FieldTypes.
        /// </summary>
        public static IReadOnlyDictionary<FieldTypeId, BaseFieldType> RegisteredFieldTypes => _registeredFieldTypes;

        /// <summary>
        /// Gets the list of registered ConstraintTypes.
        /// </summary>
        public static IReadOnlyDictionary<ConstraintId, Type> RegisteredConstraintTypes => _registeredConstraintTypes;


        /// <summary>
        /// Adds a new FieldType to the registered field types and sets the allowed constraints for it.
        /// </summary>
        /// <param name="type">The FieldType to add.</param>
        public static void AddFieldType(BaseFieldType type)
        {
            _registeredFieldTypes.Add(type.TypeId, type);
        }

        /// <summary>
        /// Removes a FieldType from the registered field types.
        /// </summary>
        public static void RemoveFieldType(FieldTypeId fieldTypeId)
        {
            _registeredFieldTypes.Remove(fieldTypeId);
        }


        /// <summary>
        /// Adds a new ConstraintType to the registered constraint types.
        /// </summary>
        public static void AddConstraintType<TConstraint>()
            where TConstraint : IConstraint
        {
            _registeredConstraintTypes.Add(TConstraint.ConstraintId, typeof(TConstraint));
        }

        /// <summary>
        /// Removes a ConstraintType from the registered constraint types.
        /// </summary>
        public static void RemoveConstraintType<TConstraint>()
            where TConstraint : IConstraint
        {
            _registeredConstraintTypes.Remove(TConstraint.ConstraintId);
        }


        /// <summary>
        /// Sets the allowed constraint types for a given FieldType.
        /// </summary>
        /// <param name="fieldType">The FieldType to set constraints for.</param>
        /// <param name="constraintTypes">The list of allowed constraint types.</param>
        /// <exception cref="InvalidOperationException">Thrown if any of the constraint types are not of type Constraint.</exception>
        public static void SetConstraintTypes(BaseFieldType fieldType, List<ConstraintId> constraintTypes)
        {
            fieldType.AllowedConstraints.UnionWith(constraintTypes);
        }

        /// <summary>
        /// Gets the list of allowed constraint types for a given FieldDefinition.
        /// </summary>
        /// <param name="fieldDefinition">The FieldDefinition to get constraints for.</param>
        /// <returns>The list of allowed constraint types.</returns>
        public static List<Type> GetConstraintTypes(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition.Type == null)
            {
                return new List<Type>();
            }

            var result = _fieldConstraints[fieldDefinition.Type.TypeId].ToList();

            if (fieldDefinition.IsList)
            {
                result.Add(typeof(SizeConstraint));
            }

            return result;
        }


        /// <summary>
        /// Gets the localized name of a given constraint type.
        /// </summary>
        /// <param name="constraintType">The constraint type to get the name for.</param>
        /// <returns>The localized name of the constraint type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the type is not of type Constraint.</exception>
        public static L10nString GetConstraintName(Type constraintType)
        {
            return typeof(IConstraint).IsAssignableFrom(constraintType)
                ? (L10nString)constraintType.GetProperty("DisplayName")!.GetValue(null)!
                : throw new InvalidOperationException("Type must be of type IConstraint");
        }

        /// <summary>
        /// Creates an instance of a constraint by its type name and associates it with a given FieldType.
        /// </summary>
        /// <param name="typeName">The name of the constraint type to create.</param>
        /// <returns>The created constraint instance.</returns>
        public static IConstraint Create(string typeName)
        {
            var constraintType = _allConstraints
                .First(x => x.Name == typeName);

            // TODO: check fieldType is allowed for constraint

            var constraint = (IConstraint)Activator.CreateInstance(constraintType)!;
            return constraint;
        }

        /// <summary>
        /// Gets the JSON serializer options for the dynamic forms library.
        /// </summary>
        public static JsonSerializerOptions JsonSerializerOptions { get; } = new();

        /// <summary>
        /// Gets or sets the prefix for form field names.
        /// </summary>
        public static string FormFieldPrefix { get; set; } = "form_field_";
    }
}
