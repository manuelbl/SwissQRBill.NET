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
            Assert.True(Payments.IsValidIso11649Reference("RF49N73GBST73AKL38ZX"));
        }

        [Fact]
        private void ValidWithSpaces()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF08 B370 0321"));
        }

        [Fact]
        private void ValidWithLowercaseLetters()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF 44 alll ower case"));
        }

        [Fact]
        private void ValidWithTrailingAndLeadingSpaces()
        {
            Assert.True(Payments.IsValidIso11649Reference(" RF19N8BG33KQ9HSS7BG "));
        }

        [Fact]
        private void ValidWithLowercase()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF66qs9H7NJ4fvs99SPO"));
        }

        [Fact]
        private void ValidShort()
        {
            Assert.True(Payments.IsValidIso11649Reference("RF040"));
        }

        [Fact]
        private void TooShort()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF04"));
        }

        [Fact]
        private void TooShortWithSpaces()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF 04"));
        }

        [Fact]
        private void TooLong()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF04GHJ74CV9B4DFH99RXPLMMQ43JKL0"));
        }

        [Fact]
        private void InvalidChars()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF20.0000.3"));
        }

        [Fact]
        private void InvalidCharCodeOnPos1()
        {
            Assert.False(Payments.IsValidIso11649Reference("DK5750510001322617"));
        }

        [Fact]
        private void InvalidCharCodeOnPos2()
        {
            Assert.False(Payments.IsValidIso11649Reference(" RO49AAAA1B31007593840000"));
        }

        [Fact]
        private void InvalidCheckDigitOnPos3()
        {
            Assert.False(Payments.IsValidIso11649Reference(" RFA8FN3DD938494"));
        }

        [Fact]
        private void InvalidCheckDigitOnPos4()
        {
            Assert.False(Payments.IsValidIso11649Reference("RF0CNHF"));
        }

        [Fact]
        private void InvalidChecksum()
        {
            Assert.False(Payments.IsValidIban("RF43029348BDEF3823"));
        }

        [Fact]
        private void InvalidChecksum00()
        {
            Assert.False(Payments.IsValidIban("RF0072"));
        }

        [Fact]
        private void InvalidChecksum01()
        {
            Assert.False(Payments.IsValidIban("RF0154"));
        }

        [Fact]
        private void InvalidChecksum99()
        {
            Assert.False(Payments.IsValidIban("RF991X"));
        }

        [Fact]
        private void FormatShort()
        {
            Assert.Equal("RF15 093", Payments.FormatIban("RF15093"));
        }

        [Fact]
        private void FormatLong()
        {
            Assert.Equal("RF41 BD93 DJ3Q GGD9 JI22 D", Payments.FormatIban("RF41BD93DJ3QGGD9JI22D"));
        }

        [Fact]
        private void CreateCreditorReference()
        {
            Assert.Equal("RF91B334BOPQE39D902DC", Payments.CreateIso11649Reference("B334BOPQE39D902DC"));
        }

        [Fact]
        private void CreateCreditorReferenceWithLeadingZero()
        {
            Assert.Equal("RF097", Payments.CreateIso11649Reference("7"));
        }

        [Fact]
        private void TooShortException()
        {
            Assert.Throws<ArgumentException>(() => Payments.CreateIso11649Reference(""));
        }

        [Fact]
        private void InvalidCharacterException()
        {
            Assert.Throws<ArgumentException>(() => Payments.CreateIso11649Reference("ABC-DEF"));
        }
    }

}
