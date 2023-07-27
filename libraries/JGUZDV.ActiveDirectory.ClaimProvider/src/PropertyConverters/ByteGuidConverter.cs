using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader
{
    internal class ByteGuidConverter : IPropertyConverter
    {
        public IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues)
        {
            if (propertyValues.Value is not object[] values)
            {
                if(propertyValues.Value is null)
                    return Array.Empty<string>();

                values = new[] { propertyValues.Value };
            }

            return values.OfType<byte[]>()
                .Select(x => new Guid(x).ToString())
                .ToList();
        }
    }
}
