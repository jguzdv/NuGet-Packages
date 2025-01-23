using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using JGUZDV.DynamicForms.Model;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Serialization
{
    public class DefaultResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var typeInfo = base.GetTypeInfo(type, options);

            if (typeInfo.Type == typeof(FieldType))
                typeInfo.PolymorphismOptions = BuildFieldTypeJsonPolymorphismOptions();

            if (typeInfo.Type == typeof(Constraint))
                typeInfo.PolymorphismOptions = BuildConstraintJsonPolymorphismOptions();

            if (typeInfo.Type.IsAssignableTo(typeof(FieldType)))
            {
                typeInfo.Properties.Remove(typeInfo.Properties.FirstOrDefault(x => x.Name == "ClrType")!);
            }

            return typeInfo;
        }

        private static JsonPolymorphismOptions BuildFieldTypeJsonPolymorphismOptions()
        {
            var options = new JsonPolymorphismOptions
            {
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            foreach (var fieldType in DynamicFormsConfiguration.KnownFieldTypes)
            {
                options.DerivedTypes.Add(new JsonDerivedType(fieldType.GetType(), fieldType.GetType().Name));
            }

            return options;
        }

        private static JsonPolymorphismOptions BuildConstraintJsonPolymorphismOptions()
        {
            var options = new JsonPolymorphismOptions
            {
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            foreach (var constraintType in DynamicFormsConfiguration.GetConstraintTypes())
            {
                options.DerivedTypes.Add(new JsonDerivedType(constraintType, constraintType.Name));
            }

            return options;
        }
    }
}
