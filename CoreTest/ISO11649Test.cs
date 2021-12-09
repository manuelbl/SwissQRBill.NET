//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class ISO11649Test
    {
        [Fact]
        public void Valid()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF49N73GBST73AKL38ZX"));
        }

        [Fact]
        public void ValidWithSpaces()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF08 B370 0321"));
        }

        [Fact]
        public void ValidWithLowercaseLetters()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF 44 all lower case"));
        }

        [Fact]
        public void ValidWithTrailingAndLeadingSpaces()
        {
            Assert.True(Payments.IsValidIso11649Reference(" RF19N8BG33KQ9HSS7BG "));
        }

        [Fact]
        public void ValidWithLowercase()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF66qs9H7NJ4fvs99SPO"));
        }

        [Fact]
        public void ValidShort()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF040"));
        }

        [Fact]
        public void TooShort()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF04"));
        }

        [Fact]
        public void TooShortWithSpaces()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF 04"));
        }

        [Fact]
        public void TooLong()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF04GHJ74CV9B4DFH99RXPLMMQ43JKL0"));
        }

        [Fact]
        public void InvalidChars()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF20.0000.3"));
        }

        [Fact]
        public void InvalidCharCodeOnPos1()
        {
            Assert.False(Payments.IsValidIso11649Reference("DK5750510001322617"));
        }

        [Fact]
        public void InvalidCharCodeOnPos2()
        {
            Assert.False(Payments.IsValidIso11649Reference(" RO49AAAA1B31007593840000"));
        }

        [Fact]
        public void InvalidCheckDigitOnPos3()
        {
            Assert.False(Payments.IsValidIso11649Reference(" RFA8FN3DD938494"));
        }

        [Fact]
        public void InvalidCheckDigitOnPos4()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF0CNHF"));
        }

        [Fact]
        public void InvalidChecksum()
        {
            Assert.False(Payments.IsValidIban("RF43029348BDEF3823"));
        }

        [Fact]
        public void InvalidChecksum00()
        {
            Assert.False(Payments.IsValidIban("RF0072"));
        }

        [Fact]
        public void InvalidChecksum01()
        {
            Assert.False(Payments.IsValidIban("RF0154"));
        }

        [Fact]
        public void InvalidChecksum99()
        {
            Assert.False(Payments.IsValidIban("RF991X"));
        }

        [Fact]
        public void FormatShort()
        {
            Assert.Equal("RF15 093", Payments.FormatIban("RF15093"));
        }

        [Fact]
        public void FormatLong()
        {
            Assert.Equal("RF41 BD93 DJ3Q GGD9 JI22 D", Payments.FormatIban("RF41BD93DJ3QGGD9JI22D"));
        }

        [Fact]
        public void CreateCreditorReference()
        {
            Assert.Equal("RF91B334BOPQE39D902DC", Payments.CreateIso11649Reference("B334BOPQE39D902DC"));
        }

        [Fact]
        public void CreateCreditorReferenceWithLeadingZero()
        {
            Assert.Equal("RF097", Payments.CreateIso11649Reference("7"));
        }

        [Fact]
        public void TooShortException()
        {
            Assert.Throws<ArgumentException>(() => Payments.CreateIso11649Reference(""));
        }

        [Fact]
        public void InvalidCharacterException()
        {
            Assert.Throws<ArgumentException>(() => Payments.CreateIso11649Reference("ABC-DEF"));
        }
    }

}
