using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader
{
    public class StringConverter : IPropertyConverter
    {
        public virtual IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues)
        {
            if (propertyValues.Value is not object[] values)
            {
                if (propertyValues.Value is null)
                    return Array.Empty<string>();

                values = new[] { propertyValues.Value };
            }

            return values.OfType<string>().ToArray();
        }
    }

    public class LowerStringConverter : StringConverter, IPropertyConverter
    {
        public override IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues)
        {
            return base.ConvertProperty(propertyValues)
                .Select(x => x.ToLowerInvariant())
                .ToArray();
        }
    }

    public class UpperStringConverter : StringConverter, IPropertyConverter
    {
        public override IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues)
        {
            return base.ConvertProperty(propertyValues)
                .Select(x => x.ToUpperInvariant())
                .ToArray();
        }
    }

    //internal class SIDToNameConverter : IPropertyReader
    //{
    //    public string[] ConvertProperty(PropertyValueCollection propertyValues)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
    //internal class DNToNameConverter : IPropertyReader
    //{
    //    public string[] ConvertProperty(PropertyValueCollection propertyValues)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
