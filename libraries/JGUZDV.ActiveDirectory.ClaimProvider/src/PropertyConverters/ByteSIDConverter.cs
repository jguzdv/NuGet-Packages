using System.DirectoryServices;
using System.Security.Principal;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader
{
    internal class ByteSIDConverter : IPropertyConverter
    {
        public virtual IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues)
        {
            if (propertyValues.Value is not object[] values)
            {
                if (propertyValues.Value is null)
                    return Array.Empty<string>();

                values = new[] { propertyValues.Value };
            }

            return values.OfType<byte[]>()
                .Select(x => new SecurityIdentifier(x, 0).ToString())
                .ToList();
        }
    }
}
