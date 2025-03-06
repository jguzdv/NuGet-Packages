using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Samples.Client.Model;


/// <summary>
/// 
/// </summary>
public class Document
{
    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<Field> Fields { get; set; } = new();
}

/// <summary>
/// 
/// </summary>
public class DocumentDefinition
{
    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<FieldDefinition> FieldDefinitions { get; set; } = new();
}
