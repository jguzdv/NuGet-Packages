namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerOptions
{
    public string KeyStorePath { get; set; }
    public bool DisableKeyGeneration { get; set; }

    public TimeSpan KeyReloadInterval { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan MaxKeyAge { get; set; } = TimeSpan.FromDays(60);
}
