//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2024 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Text;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class StringsTest
    {
        [Fact]
        public void NoSpaces_NoChange()
        {
            Assert.Equal("abc", "abc".SpacesCleaned());
        }

        [Fact]
        public void LeadingSpaces_Removed()
        {
            Assert.Equal("abc", "  abc".SpacesCleaned());
        }

        [Fact]
        public void TrailingSpaces_Removed()
        {
            Assert.Equal("abc", "abc  ".SpacesCleaned());
        }

        [Fact]
        public void SingleSpaces_NoChange()
        {
            Assert.Equal("a b c", "a b c".SpacesCleaned());
        }

        [Fact]
        public void MultipleSpaces_Merged()
        {
            Assert.Equal("a b c", "a  b    c".SpacesCleaned());
        }


        [Fact]
        public void ManySpaces_Cleaned()
        {
            Assert.Equal("a b c", "  a b   c ".SpacesCleaned());
        }

        [Theory]
        [InlineData(0x00DC, "Ü")]
        [InlineData(0x0409, "Љ")]
        [InlineData(0x1F609, "😉")]
        [InlineData(0x1F680, "🚀")]
        public void AppendCodePoint_ExpectedResult(int codePoint, string expectedResult)
        {
            var sb = new StringBuilder();
            sb.Append("xx");
            StringExtensions.AppendCodePoint(sb, codePoint);
            sb.Append("yy");
            Assert.Equal($"xx{expectedResult}yy", sb.ToString());
        }
    }
}
