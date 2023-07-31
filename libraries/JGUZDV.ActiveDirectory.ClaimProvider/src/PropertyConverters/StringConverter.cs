namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public class StringConverter : IPropertyConverter
{
    public virtual string ConverterName => nameof(StringConverter);


    public virtual IEnumerable<string> ConvertProperty(IEnumerable<object> values)
    {
        return values.OfType<string>();
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
