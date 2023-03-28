using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.Authorization.Tests
{
    public class OptionsUsageTests
    {
        private const string _appsettings = """
            {
                "ClaimRequirementCollection": {
                    "MatchType": "MatchAny",
                    "Requirements": [
                        {
                            "ClaimType": "c1",
                            "ClaimValue": "v1"
                        },
                        {
                            "ClaimType": "c2",
                            "ClaimValue": "v2"
                        }
                    ]
                },

                "ClaimValueRequirement": {
                    "ClaimType": "c1",
                    "ClaimValue": "v1"
                }
            }
        """;

        public class OptionsDTO
        {
            public ClaimRequirementOptions ClaimRequirementCollection { get; set; }
            public ClaimRequirementOptions ClaimValueRequirement { get; set; }
        }

        [Fact]
        public void ClaimRequirements_can_be_read_from_config()
        {
            var appSettings = new MemoryStream();
            appSettings.Write(Encoding.UTF8.GetBytes(_appsettings));
            appSettings.Position = 0;

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonStream(appSettings);

            var config = configBuilder.Build();
            var options = new OptionsDTO();
            config.Bind(options);
            
            Assert.NotNull(options.ClaimRequirementCollection.ClaimRequirement); 
            Assert.NotNull(options.ClaimValueRequirement.ClaimRequirement);
        }
    }
}
