//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class IBANTest
    {
        [Fact]
        public void Valid()
        {
            Assert.True(Payments.IsValidIban("FR7630066100410001057380116"));
        }

        [Fact]
        public void ValidWithSpaces()
        {
            Assert.True(Payments.IsValidIban("FR76 3006 6100 4100 0105 7380 116"));
        }

        [Fact]
        public void ValidWithTrailingAndLeadingSpaces()
        {
            Assert.True(Payments.IsValidIban(" DE12500105170648489890 "));
        }

        [Fact]
        public void ValidWithLowercase()
        {
            Assert.True(Payments.IsValidIban("MT98mmeb44093000000009027293051"));
        }

        [Fact]
        public void TooShort()
        {
            Assert.False(Payments.IsValidIban("CH04"));
        }

        [Fact]
        public void TooShortWithSpaces()
        {
            Assert.False(Payments.IsValidIban("CH 04"));
        }

        [Fact]
        public void InvalidChars()
        {
            Assert.False(Payments.IsValidIban("SE64-1200-0000-0121-7014-5230"));
        }

        [Fact]
        public void InvalidCountryCodeOnPos1()
        {
            Assert.False(Payments.IsValidIban("0K9311110000001057361004"));
        }

        [Fact]
        public void InvalidCountryCodeOnPos2()
        {
            Assert.False(Payments.IsValidIban("S 056031001001300933"));
        }

        [Fact]
        public void InvalidCheckDigitOnPos3()
        {
            Assert.False(Payments.IsValidIban(" GBF2ESSE40486562136016"));
        }

        [Fact]
        public void InvalidCheckDigitOnPos4()
        {
            Assert.False(Payments.IsValidIban("FR7A30066100410001057380116"));
        }

        [Fact]
        public void InvalidChecksum()
        {
            Assert.False(Payments.IsValidIban("DK5650510001322617"));
        }

        [Fact]
        public void FormatIban1()
        {
            Assert.Equal("BE68 8440 1037 0034", Payments.FormatIban("BE68844010370034"));
        }

        [Fact]
        public void FormatIban2()
        {
            Assert.Equal("IT68 D030 0203 2800 0040 0162 854", Payments.FormatIban("IT68D0300203280000400162854"));
        }
    }
}
