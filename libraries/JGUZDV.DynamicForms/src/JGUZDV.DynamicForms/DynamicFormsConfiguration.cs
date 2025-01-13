using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms
{
    //the types and relationships (FieldTypes, Constraints) should not be dynamic but extensible by consumer, hence static class
    public static class DynamicFormsConfiguration
    {
        private static HashSet<FieldType> _knownFieldTypes = new()
            {
                new DateOnlyFieldType(),
                new IntFieldType(),
                new StringFieldType(),
            };

        public static void AddFieldType(FieldType type, List<Type> allowedConstrains)
        {
            SetConstraintTypes(type, allowedConstrains);
            _knownFieldTypes.Add(type);
        }

        public static List<FieldType> KnownFieldTypes => _knownFieldTypes.ToList();

        private static Dictionary<FieldType, List<Type>> _constraintTypes = new(){
            { new StringFieldType(), [typeof(RegexConstraint), typeof(StringLengthConstraint), typeof(RangeConstraint)] },
            { new DateOnlyFieldType(), [typeof(RangeConstraint)] },
            { new IntFieldType(), [typeof(RangeConstraint)] }
            };

        public static void SetConstraintTypes(FieldType fieldType, List<Type> constraintTypes)
        {
            if(constraintTypes.Any(constraintTypes => !typeof(Constraint).IsAssignableFrom(constraintTypes)))
            {
                throw new InvalidOperationException("All constraint types must be of type Constraint");
            }

            _constraintTypes[fieldType] = constraintTypes;
        }

        public static List<Type> GetConstraintTypes(FieldDefinition fieldDefinition)
        {
            if (string.IsNullOrWhiteSpace(fieldDefinition.InputDefinition.InputType))
            {
                return [];
            }

            var result = _constraintTypes[FieldType.FromJson(fieldDefinition.InputDefinition.InputType)].ToList();

            if (fieldDefinition.IsList)
            {
                result.Add(typeof(SizeConstraint));
            }

            return result;
        }
    }
}
