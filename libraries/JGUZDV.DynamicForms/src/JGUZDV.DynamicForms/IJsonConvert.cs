namespace JGUZDV.DynamicForms;

public interface IJsonConvert<T> where T : class
{
    string ToJson();

    static abstract T FromJson(string json);
}
