using System.Text.Json;

using JGUZDV.DynamicForms.Model;
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
        private static HashSet<FieldType> _knownFieldTypes = new()
                {
                    new DateOnlyFieldType(),
                    new IntFieldType(),
                    new StringFieldType(),
                    new FileFieldType()
                };

        /// <summary>
        /// Adds a new FieldType to the known field types and sets the allowed constraints for it.
        /// </summary>
        /// <param name="type">The FieldType to add.</param>
        /// <param name="allowedConstraints">The list of allowed constraints for the FieldType.</param>
        public static void AddFieldType(FieldType type, List<Type> allowedConstraints)
        {
            SetConstraintTypes(type, allowedConstraints);
            _knownFieldTypes.Add(type);
        }

        /// <summary>
        /// Gets the list of known FieldTypes.
        /// </summary>
        public static List<FieldType> KnownFieldTypes => _knownFieldTypes.ToList();

        private static Dictionary<FieldType, List<Type>> _fieldConstraints = new()
                {
                    { new StringFieldType(), [ typeof(RegexConstraint), typeof(StringLengthConstraint), typeof(RangeConstraint) ] },
                    { new DateOnlyFieldType(), [ typeof(RangeConstraint) ] },
                    { new IntFieldType(), [typeof(RangeConstraint)] },
                    { new FileFieldType(), [typeof(FileSizeConstraint)] } // Added FileSizeConstraint
                };

        /// <summary>
        /// Sets the allowed constraint types for a given FieldType.
        /// </summary>
        /// <param name="fieldType">The FieldType to set constraints for.</param>
        /// <param name="constraintTypes">The list of allowed constraint types.</param>
        /// <exception cref="InvalidOperationException">Thrown if any of the constraint types are not of type Constraint.</exception>
        public static void SetConstraintTypes(FieldType fieldType, List<Type> constraintTypes)
        {
            if (constraintTypes.Any(constraintType => !typeof(Constraint).IsAssignableFrom(constraintType)))
            {
                throw new InvalidOperationException("All constraint types must be of type Constraint");
            }

            _fieldConstraints[fieldType] = constraintTypes;
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

            var result = _fieldConstraints[fieldDefinition.Type].ToList();

            if (fieldDefinition.IsList)
            {
                result.Add(typeof(SizeConstraint));
            }

            return result;
        }

        /// <summary>
        /// Gets the list of all distinct constraint types.
        /// </summary>
        /// <returns>The list of all distinct constraint types.</returns>
        public static List<Type> GetConstraintTypes()
        {
            var types = _fieldConstraints.Values.SelectMany(x => x).Distinct().ToList();
            return types;
        }

        private static Dictionary<Type, L10nString> _constraintNames = new()
                {
                    { typeof(RegexConstraint), new L10nString { ["de"] = "Regex", ["en"] = "Regex" } },
                    { typeof(StringLengthConstraint), new L10nString { ["de"] = "Textlänge", ["en"] = "Text Length" } },
                    { typeof(RangeConstraint), new L10nString { ["de"] = "Intervall", ["en"] = "Range" } },
                    { typeof(SizeConstraint), new L10nString { ["de"] = "Listenlänge", ["en"] = "List Length" } },
                    { typeof(FileSizeConstraint), new L10nString { ["de"] = "Dateigröße", ["en"] = "File Size" } } // Added FileSizeConstraint
                };

        /// <summary>
        /// Gets the localized name of a given constraint type.
        /// </summary>
        /// <param name="constraintType">The constraint type to get the name for.</param>
        /// <returns>The localized name of the constraint type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the type is not of type Constraint.</exception>
        public static L10nString GetConstraintName(Type constraintType)
        {
            if (!typeof(Constraint).IsAssignableFrom(constraintType))
            {
                throw new InvalidOperationException("Type must be of type Constraint");
            }

            return _constraintNames[constraintType];
        }

        /// <summary>
        /// Sets the localized name for a given constraint type.
        /// </summary>
        /// <param name="constraintType">The constraint type to set the name for.</param>
        /// <param name="name">The localized name to set.</param>
        /// <exception cref="InvalidOperationException">Thrown if the type is not of type Constraint.</exception>
        public static void SetConstraintName(Type constraintType, L10nString name)
        {
            if (!typeof(Constraint).IsAssignableFrom(constraintType))
            {
                throw new InvalidOperationException("Type must be of type Constraint");
            }

            _constraintNames[constraintType] = name;
        }

        /// <summary>
        /// Creates an instance of a constraint by its type name and associates it with a given FieldType.
        /// </summary>
        /// <param name="typeName">The name of the constraint type to create.</param>
        /// <param name="fieldType">The FieldType to associate with the constraint.</param>
        /// <returns>The created constraint instance.</returns>
        public static Constraint Create(string typeName, FieldType fieldType)
        {
            var constraintType = _fieldConstraints.Values
                .SelectMany(x => x)
                .Distinct()
                .First(x => x.Name == typeName);

            // TODO: check fieldType is allowed for constraint

            var constraint = (Constraint)Activator.CreateInstance(constraintType)!;
            constraint.FieldType = fieldType;
            return constraint;
        }

        public static JsonSerializerOptions JsonSerializerOptions { get; } = new();

        public static string FormFieldPrefix { get; set; } = "form_field_";
    }
}
