namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public class LowerStringConverter : StringConverter, IPropertyConverter
{
    public override IEnumerable<string> ConvertProperty(IEnumerable<object> values)
    {
        return base.ConvertProperty(values)
            .Select(x => x.ToLowerInvariant());
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
