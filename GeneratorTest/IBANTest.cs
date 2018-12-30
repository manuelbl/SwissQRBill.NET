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
        void Valid()
        {
            Assert.True(Payments.IsValidIBAN("FR7630066100410001057380116"));
        }

        [Fact]
        void ValidWithSpaces()
        {
            Assert.True(Payments.IsValidIBAN("FR76 3006 6100 4100 0105 7380 116"));
        }

        [Fact]
        void ValidWithTrailingAndLeadingSpaces()
        {
            Assert.True(Payments.IsValidIBAN(" DE12500105170648489890 "));
        }

        [Fact]
        void ValidWithLowercase()
        {
            Assert.True(Payments.IsValidIBAN("MT98mmeb44093000000009027293051"));
        }

        [Fact]
        void TooShort()
        {
            Assert.False(Payments.IsValidIBAN("CH04"));
        }

        [Fact]
        void TooShortWithSpaces()
        {
            Assert.False(Payments.IsValidIBAN("CH 04"));
        }

        [Fact]
        void InvalidChars()
        {
            Assert.False(Payments.IsValidIBAN("SE64-1200-0000-0121-7014-5230"));
        }

        [Fact]
        void InvalidCountryCodeOnPos1()
        {
            Assert.False(Payments.IsValidIBAN("0K9311110000001057361004"));
        }

        [Fact]
        void InvalidCountryCodeOnPos2()
        {
            Assert.False(Payments.IsValidIBAN("S 056031001001300933"));
        }

        [Fact]
        void InvalidCheckDigitOnPos3()
        {
            Assert.False(Payments.IsValidIBAN(" GBF2ESSE40486562136016"));
        }

        [Fact]
        void InvalidCheckDigitOnPos4()
        {
            Assert.False(Payments.IsValidIBAN("FR7A30066100410001057380116"));
        }

        [Fact]
        void InvalidChecksum()
        {
            Assert.False(Payments.IsValidIBAN("DK5650510001322617"));
        }

        [Fact]
        void FormatIBAN1()
        {
            Assert.Equal("BE68 8440 1037 0034", Payments.FormatIBAN("BE68844010370034"));
        }

        [Fact]
        void FormatIBAN2()
        {
            Assert.Equal("IT68 D030 0203 2800 0040 0162 854", Payments.FormatIBAN("IT68D0300203280000400162854"));
        }
    }
}
