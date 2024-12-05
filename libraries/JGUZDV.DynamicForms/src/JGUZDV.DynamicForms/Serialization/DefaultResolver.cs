using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Serialization
{
    public class DefaultResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var typeInfo = base.GetTypeInfo(type, options);

            if (typeInfo.Type == typeof(FieldType))
                typeInfo.PolymorphismOptions = BuildJsonPolymorphismOptions();

            if (typeInfo.Type.IsAssignableTo(typeof(FieldType)))
            {
                typeInfo.Properties.Remove(typeInfo.Properties.FirstOrDefault(x => x.Name == "ClrType")!);
            }

            return typeInfo;
        }

        private static JsonPolymorphismOptions BuildJsonPolymorphismOptions()
        {
            var options = new JsonPolymorphismOptions
            {
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            foreach (var fieldType in FieldType.KnownFieldTypes)
            {
                options.DerivedTypes.Add(new JsonDerivedType(fieldType.GetType(), fieldType.GetType().Name));
            }

            return options;
        }
    }
}
