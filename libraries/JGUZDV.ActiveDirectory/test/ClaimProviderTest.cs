using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

using JGUZDV.ActiveDirectory.Claims;
using JGUZDV.ActiveDirectory.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JGUZDV.ActiveDirectory.Tests;

[SupportedOSPlatform("windows")]
public class ClaimProviderTest
{
    private readonly IServiceProvider _serviceProvider;

    public ClaimProviderTest()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddPropertyReader(o =>
        {
            o.PropertyInfos.Add("test-string", new("test-string", typeof(string)));
            o.PropertyInfos.Add("test-dn", new("test-dn", typeof(string)));
            o.PropertyInfos.Add("test-int", new("test-int", typeof(int)));
            o.PropertyInfos.Add("test-byte", new("test-byte", typeof(byte[])));
            o.PropertyInfos.Add("test-guid", new("test-guid", typeof(byte[])));
            o.PropertyInfos.Add("test-sid", new("test-sid", typeof(byte[])));
        });
        services.AddClaimProvider(o =>
        {
            o.ClaimSources.Add(new("test-string", "test-string", null, Casing.Unchanged));
            o.ClaimSources.Add(new("test-string-upper", "test-string", null, Casing.Upper));
            o.ClaimSources.Add(new("test-string-lower", "test-string", null, Casing.Lower));

            o.ClaimSources.Add(new("test-dn", "test-dn", null, Casing.Unchanged));
            o.ClaimSources.Add(new("test-cn", "test-dn", OutputFormats.ADStrings.CN, Casing.Unchanged));
            o.ClaimSources.Add(new("test-cn-lower", "test-dn", OutputFormats.ADStrings.CN, Casing.Lower));

            o.ClaimSources.Add(new("test-int", "test-int", null, Casing.Unchanged));
            o.ClaimSources.Add(new("test-byte", "test-byte", OutputFormats.ByteArrays.Base64, Casing.Unchanged));
            o.ClaimSources.Add(new("test-guid", "test-guid", OutputFormats.ByteArrays.Guid, Casing.Unchanged));
            o.ClaimSources.Add(new("test-sid", "test-sid", OutputFormats.ByteArrays.SDDL, Casing.Unchanged));
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    private DirectoryEntry CreateEntryWithProperties()
    {
        var directoryEntry = new DirectoryEntry();
        directoryEntry.Properties["test-string"].Value = "Test";
        directoryEntry.Properties["test-dn"].Value = "CN=Test,OU=Users,DC=example,DC=com";
        directoryEntry.Properties["test-int"].Value = 42;
        directoryEntry.Properties["test-byte"].Value = new byte[] { 0x01, 0x02, 0x03 };
        directoryEntry.Properties["test-guid"].Value = new Guid("00000000-0000-0000-0000-000000000042").ToByteArray();

        var sid = new SecurityIdentifier("S-1-5-21-3623811015-3361044348-30300820-1013");
        var sidBytes = new byte[sid.BinaryLength];
        sid.GetBinaryForm(sidBytes, 0);
        directoryEntry.Properties["test-sid"].Value = sidBytes;

        return directoryEntry;
    }

    [PlatformFact(nameof(OSPlatform.Windows))]
    public void ClaimProvider_Provides_String()
    {
        var sut = _serviceProvider.GetRequiredService<IClaimProvider>();

        var directoryEntry = CreateEntryWithProperties();
        var claims = sut.GetClaims(directoryEntry, ["test-string", "test-string-upper", "test-string-lower", "test-dn", "test-cn", "test-cn-lower", "test-int", "test-byte", "test-guid", "test-sid"]);

        Assert.Contains((Type: "test-string", Value: "Test"), claims);
        Assert.Contains((Type: "test-string-upper", Value: "TEST"), claims);
        Assert.Contains((Type: "test-string-lower", Value: "test"), claims);
        Assert.Contains((Type: "test-dn", Value: "CN=Test,OU=Users,DC=example,DC=com"), claims);
        Assert.Contains((Type: "test-cn", Value: "Test"), claims);
        Assert.Contains((Type: "test-cn-lower", Value: "test"), claims);
        Assert.Contains((Type: "test-int", Value: "42"), claims);
        Assert.Contains((Type: "test-byte", Value: "AQID"), claims);
        Assert.Contains((Type: "test-guid", Value: "00000000-0000-0000-0000-000000000042"), claims);
        Assert.Contains((Type: "test-sid", Value: "S-1-5-21-3623811015-3361044348-30300820-1013"), claims);

        //Assert.Contains(claims, x => x.Type == "test-string" && x.Value == "Test");
        //Assert.Contains(claims, x => x.Type == "test-string-upper" && x.Value == "TEST");
        //Assert.Contains(claims, x => x.Type == "test-string-lower" && x.Value == "test");
        //Assert.Contains(claims, x => x.Type == "test-dn" && x.Value == "CN=Test,OU=Users,DC=example,DC=com");
        //Assert.Contains(claims, x => x.Type == "test-cn" && x.Value == "Test");
        //Assert.Contains(claims, x => x.Type == "test-cn-lower" && x.Value == "test");
        //Assert.Contains(claims, x => x.Type == "test-int" && x.Value == "42");
        //Assert.Contains(claims, x => x.Type == "test-byte" && x.Value == "AQID");
        //Assert.Contains(claims, x => x.Type == "test-guid" && x.Value == "00000000-0000-0000-0000-000000000042");
        //Assert.Contains(claims, x => x.Type == "test-sid" && x.Value == "S-1-5-21-3623811015-3361044348-30300820-1013");
    }
}
