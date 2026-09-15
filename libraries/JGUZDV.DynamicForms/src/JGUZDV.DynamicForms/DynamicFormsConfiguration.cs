
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
            _registeredFieldTypes = new FieldType[] {
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

        private static readonly Dictionary<FieldTypeId, FieldType> _registeredFieldTypes;
        private static readonly Dictionary<ConstraintId, Type> _registeredConstraintTypes;
        

        /// <summary>
        /// Gets the list of registered FieldTypes.
        /// </summary>
        public static IReadOnlyDictionary<FieldTypeId, FieldType> RegisteredFieldTypes => _registeredFieldTypes;

        /// <summary>
        /// Gets the list of registered ConstraintTypes.
        /// </summary>
        public static IReadOnlyDictionary<ConstraintId, Type> RegisteredConstraintTypes => _registeredConstraintTypes;


        /// <summary>
        /// Adds a new FieldType to the registered field types and sets the allowed constraints for it.
        /// </summary>
        /// <param name="type">The FieldType to add.</param>
        public static void AddFieldType(FieldType type)
        {
            foreach(var constraintId in type.AllowedConstraints)
            {
                if (!_registeredConstraintTypes.ContainsKey(constraintId))
                {
                    throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
                }
            }

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
        public static void SetConstraintTypes(FieldType fieldType, List<ConstraintId> constraintTypes)
        {
            foreach (var constraintId in constraintTypes)
            {
                if (!_registeredConstraintTypes.ContainsKey(constraintId))
                {
                    throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
                }
            }

            fieldType.AllowedConstraints.UnionWith(constraintTypes);
        }


        /// <summary>
        /// Gets the localized name of a given constraint type.
        /// </summary>
        public static L10nString GetConstraintName(ConstraintId constraintId)
        {
            return _registeredConstraintTypes.TryGetValue(constraintId, out var constraintType)
                ? (L10nString)constraintType.GetProperty("DisplayName")!.GetValue(null)!
                : throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
        }

        /// <summary>
        /// Adds a constraint to a given FieldDefinition.
        /// </summary>
        public static void AddConstraintToFieldDefinition(FieldDefinition fieldDefinition, ConstraintId constraintId)
        {
            if (!_registeredConstraintTypes.TryGetValue(constraintId, out Type? value))
            {
                throw new InvalidOperationException($"Constraint type {constraintId} is not registered.");
            }

            var constraint = (IConstraint)Activator.CreateInstance(value)!;
            fieldDefinition.Constraints.Add(constraint);
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
