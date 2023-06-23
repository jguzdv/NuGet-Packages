namespace JGUZDV.ClientStorage.Store;

/// <summary>
/// Additional information how the entry was retrieved
/// </summary>
public class LoadingContext
{
    public bool LoadedFromStorage { get; set; }
    public bool LoadedFromCache { get; set; }
    public bool LoadedHot { get; set; }
    public required string Key { get; set; }
}
