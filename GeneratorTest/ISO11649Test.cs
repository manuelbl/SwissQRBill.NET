//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class ISO11649Test
    {
        [Fact]
        public void Valid()
        {
            Assert.True(Payments.IsValidISO11649Reference("RF49N73GBST73AKL38ZX"));
        }

        [Fact]
        void ValidWithSpaces()
        {
            Assert.True(Payments.IsValidISO11649Reference("RF08 B370 0321"));
        }

        [Fact]
        void ValidWithLowercaseLetters()
        {
            Assert.True(Payments.IsValidISO11649Reference("RF 44 alll ower case"));
        }

        [Fact]
        void ValidWithTrailingAndLeadingSpaces()
        {
            Assert.True(Payments.IsValidISO11649Reference(" RF19N8BG33KQ9HSS7BG "));
        }

        [Fact]
        void ValidWithLowercase()
        {
            Assert.True(Payments.IsValidISO11649Reference("RF66qs9H7NJ4fvs99SPO"));
        }

        [Fact]
        void ValidShort()
        {
            Assert.True(Payments.IsValidISO11649Reference("RF040"));
        }

        [Fact]
        void TooShort()
        {
            Assert.False(Payments.IsValidISO11649Reference("RF04"));
        }

        [Fact]
        void TooShortWithSpaces()
        {
            Assert.False(Payments.IsValidISO11649Reference("RF 04"));
        }

        [Fact]
        void TooLong()
        {
            Assert.False(Payments.IsValidISO11649Reference("RF04GHJ74CV9B4DFH99RXPLMMQ43JKL0"));
        }

        [Fact]
        void InvalidChars()
        {
            Assert.False(Payments.IsValidISO11649Reference("RF20.0000.3"));
        }

        [Fact]
        void InvalidCharCodeOnPos1()
        {
            Assert.False(Payments.IsValidISO11649Reference("DK5750510001322617"));
        }

        [Fact]
        void InvalidCharCodeOnPos2()
        {
            Assert.False(Payments.IsValidISO11649Reference(" RO49AAAA1B31007593840000"));
        }

        [Fact]
        void InvalidCheckDigitOnPos3()
        {
            Assert.False(Payments.IsValidISO11649Reference(" RFA8FN3DD938494"));
        }

        [Fact]
        void InvalidCheckDigitOnPos4()
        {
            Assert.False(Payments.IsValidISO11649Reference("RF0CNHF"));
        }

        [Fact]
        void InvalidChecksum()
        {
            Assert.False(Payments.IsValidIBAN("RF43029348BDEF3823"));
        }

        [Fact]
        void InvalidChecksum00()
        {
            Assert.False(Payments.IsValidIBAN("RF0072"));
        }

        [Fact]
        void InvalidChecksum01()
        {
            Assert.False(Payments.IsValidIBAN("RF0154"));
        }

        [Fact]
        void InvalidChecksum99()
        {
            Assert.False(Payments.IsValidIBAN("RF991X"));
        }

        [Fact]
        void FormatShort()
        {
            Assert.Equal("RF15 093", Payments.FormatIBAN("RF15093"));
        }

        [Fact]
        void FormatLong()
        {
            Assert.Equal("RF41 BD93 DJ3Q GGD9 JI22 D", Payments.FormatIBAN("RF41BD93DJ3QGGD9JI22D"));
        }

        [Fact]
        void CreateCreditorReference()
        {
            Assert.Equal("RF91B334BOPQE39D902DC", Payments.CreateISO11649Reference("B334BOPQE39D902DC"));
        }

        [Fact]
        void CreateCreditorReferenceWithLeadingZero()
        {
            Assert.Equal("RF097", Payments.CreateISO11649Reference("7"));
        }

        [Fact]
        void TooShortException()
        {
            Assert.Throws<ArgumentException>(() => Payments.CreateISO11649Reference(""));
        }

        [Fact]
        void InvalidCharacterException()
        {
            Assert.Throws<ArgumentException>(() => Payments.CreateISO11649Reference("ABC-DEF"));
        }
    }

}
