using JGUZDV.DynamicForms.Model;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms
{
    //the types and relationships (FieldTypes, Constraints) should not be dynamic but extensible by consumer, hence static class
    //TODO: should this logic be placed in Constaint and FieldType base classes?
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

        private static Dictionary<FieldType, List<Type>> _fieldConstraints = new(){
            { new StringFieldType(), [typeof(RegexConstraint), typeof(StringLengthConstraint), typeof(RangeConstraint)] },
            { new DateOnlyFieldType(), [typeof(RangeConstraint)] },
            { new IntFieldType(), [typeof(RangeConstraint)] }
            };

        public static void SetConstraintTypes(FieldType fieldType, List<Type> constraintTypes)
        {
            if (constraintTypes.Any(constraintTypes => !typeof(Constraint).IsAssignableFrom(constraintTypes)))
            {
                throw new InvalidOperationException("All constraint types must be of type Constraint");
            }

            _fieldConstraints[fieldType] = constraintTypes;
        }
        
        public static List<Type> GetConstraintTypes(FieldDefinition fieldDefinition)
        {
            if (string.IsNullOrWhiteSpace(fieldDefinition.InputDefinition.Type))
            {
                return [];
            }

            var result = _fieldConstraints[FieldType.FromJson(fieldDefinition.InputDefinition.Type)].ToList();

            if (fieldDefinition.IsList)
            {
                result.Add(typeof(SizeConstraint));
            }

            return result;
        }

        public static List<Type> GetConstraintTypes()
        {
            var types = _fieldConstraints.Values.SelectMany(x => x).Distinct().ToList();
            return types;
        }

        private static Dictionary<Type, L10nString> _constraintNames = new()
        {
            { typeof(RegexConstraint), new L10nString()
                {
                    ["de"] = "Regex",
                    ["en"] = "Regex"
                }
            },
            { typeof(StringLengthConstraint), new L10nString()
                {
                    ["de"] = "Textlänge",
                    ["en"] = "Text Length"
                }
            },
            { typeof(RangeConstraint), new L10nString()
                {
                    ["de"] = "Intervall",
                    ["en"] = "Range"
                }
            },
            { typeof(SizeConstraint), new L10nString()
                {
                    ["de"] = "Listenlänge",
                    ["en"] = "List Length"
                }
            }
        };

        public static L10nString GetConstraintName(Type constraintType)
        {
            if (!typeof(Constraint).IsAssignableFrom(constraintType))
            {
                throw new InvalidOperationException("Type must be of type Constraint");
            }

            return _constraintNames[constraintType];
        }

        public static void SetConstraintName(Type constraintType, L10nString name)
        {
            if (!typeof(Constraint).IsAssignableFrom(constraintType))
            {
                throw new InvalidOperationException("Type must be of type Constraint");
            }

            _constraintNames[constraintType] = name;
        }

        //TODO: should we use a ConstraintInfo class with one instance per constraint type
        //      that carries the needed meta information so we dont need to handle the raw Type
        public static Constraint Create(string typeName, FieldType fieldType)
        {
            var constraintType = _fieldConstraints.Values
                .SelectMany(x => x)
                .Distinct()
                .First(x => x.Name == typeName);

            //TODO: check fieldType is allowed for constraint

            var constraint = (Constraint)Activator.CreateInstance(constraintType)!;
            constraint.FieldType = fieldType;
            return constraint;
        }
    }
}
