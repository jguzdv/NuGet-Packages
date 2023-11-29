using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerOptions
{
    [NotNull]
    public string? KeyStorePath { get; set; }
    public bool DisableKeyGeneration { get; set; }

    public TimeSpan KeyReloadInterval { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan MaxKeyAge { get; set; } = TimeSpan.FromDays(60);
    public double ThresholdFactor { get; set; } = 0.3;

    public TimeSpan KeyRetention { get; set; } = TimeSpan.FromDays(30);
}
