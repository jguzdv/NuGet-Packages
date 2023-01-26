using System.Globalization;

namespace ZDV.L10n.Tests
{
    public class L10nStringTests
    {
        [Fact]
        public void Returns_requested_language()
        {
            var sut = new L10nString();
            sut["De"] = "Deutscher Text";
            sut["En"] = "English text";

            Assert.NotNull(sut["De"]);
            Assert.NotNull(sut["eN"]);

            var t = sut.GetBestAvailableMatch("EN");
            Assert.Equal("English text", t);
        }

        [Fact]
        public void Returns_less_specific_language()
        {
            var sut = new L10nString();
            sut["de-de"] = "Deutscher Text";
            sut["en"] = "English text";

            var t = sut.GetBestAvailableMatch("en-Us");
            Assert.Equal("English text", t);
        }

        [Fact]
        public void Returns_default_language()
        {
            var sut = new L10nString();
            sut["De"] = "Deutscher Text";

            var t = sut.GetBestAvailableMatch("en-Us");
            Assert.Equal("Deutscher Text", t);
        }

        [Fact]
        public void ToString_Uses_CurrentUICulture()
        {
            var sut = new L10nString();
            sut["de"] = "Deutscher Text";
            sut["en"] = "English text";
            sut["fr"] = "Le mots francais";

            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var t = sut.ToString();
            Assert.Equal("Le mots francais", t);
        }
    }
}
