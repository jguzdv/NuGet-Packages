using System.DirectoryServices;

using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader
{
    internal class ByteSIDToCommonNameConverter : ByteSIDConverter, IPropertyConverter
    {
        private readonly LowerStringConverter _lowerStringConverter;
        private readonly IOptions<ActiveDirectoryOptions> _options;

        public ByteSIDAsCommonNameReader(LowerStringConverter lowerStringConverter,
            IOptions<ActiveDirectoryOptions> options)
        {
            _lowerStringConverter = lowerStringConverter;
            _options = options;
        }

        public override IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues)
        {
            var result = new List<string>();

            foreach (var property in base.ConvertProperty(propertyValues))
            {
                var directoryEntry = ADHelper.GetDirectoryEntry(_options.Value.Connection.Server, property, new[] { "cn" });
                result.AddRange(_lowerStringConverter.ConvertProperty(directoryEntry.Properties["cn"]));
            }

            return result.ToArray();
        }
    }
    {
    }
}
