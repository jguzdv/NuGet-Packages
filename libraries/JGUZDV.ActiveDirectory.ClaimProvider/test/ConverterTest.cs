using JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

using Xunit;

namespace JGUZDV.ActiveDirectory.ClaimProvider.Tests
{
    public class ConverterTests
    {
        [Fact]
        public void ByteGuidConverter_Converts_Byte_To_Guid()
        {
            var guid = Guid.NewGuid();
            var sut = new ByteGuidConverter();

            var values = new object[] { guid.ToByteArray() };
            var result = sut.ConvertProperty(values);

            Assert.NotNull(result);
            Assert.True(result.Count() == 1);
            Assert.Equal(guid.ToString(), result.First());
        }
    }
}
