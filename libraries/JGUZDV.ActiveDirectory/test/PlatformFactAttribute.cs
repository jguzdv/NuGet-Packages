using System.Runtime.InteropServices;

namespace JGUZDV.ActiveDirectory.Tests;

/// <summary>
/// Attribute to specify that a test should only be run on certain platforms.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PlatformFactAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformFactAttribute"/> class.
    /// </summary>
    /// <param name="platform">The platforms on which the test should be run.</param>
    public PlatformFactAttribute(params string[] platform)
    {
        if (!platform.Any(p => RuntimeInformation.IsOSPlatform(OSPlatform.Create(p))))
        {
            Skip = "Test is not supported on this platform";
        }
    }
}
