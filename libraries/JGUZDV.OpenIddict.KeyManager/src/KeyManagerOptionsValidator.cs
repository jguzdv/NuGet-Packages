using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager;

public class KeyManagerOptionsValidator : IValidateOptions<KeyManagerOptions>
{
    public ValidateOptionsResult Validate(string name, KeyManagerOptions options)
    {
        if (string.IsNullOrEmpty(options.KeyStorePath))
            return ValidateOptionsResult.Fail($"{nameof(options.KeyStorePath)} was empty");

        return ValidateOptionsResult.Success;
    }
}
