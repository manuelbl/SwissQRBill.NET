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
    public class QRReferenceTest
    {
        [Fact]
        public void ValidQrReference()
        {
            Assert.True(Payments.IsValidQrReference("210000000003139471430009017"));
        }

        [Fact]
        public void InvalidLengthQrReference()
        {
            Assert.False(Payments.IsValidQrReference("2100000003139471430009017"));
        }

        [Fact]
        public void ValidQrReferenceWithSpaces()
        {
            Assert.True(Payments.IsValidQrReference("21 00000 00003 13947 14300 09017"));
        }

        [Fact]
        public void InvalidQrReferenceWithLetters()
        {
            Assert.False(Payments.IsValidQrReference("210000S00003139471430009017"));
        }

        [Fact]
        public void InvalidQrReferenceWithSpecialChar()
        {
            Assert.False(Payments.IsValidQrReference("210000000%03139471430009017"));
        }

        [Fact]
        public void InvalidCheckDigitQrReference()
        {
            Assert.False(Payments.IsValidQrReference("210000000003139471430009016"));
        }

        [Fact]
        public void FormatQrReference()
        {
            Assert.Equal("12 34560 00000 00129 11462 90514",
                    Payments.FormatQrReferenceNumber("123456000000001291146290514"));
        }

        [Fact]
        public void CreateQRReference()
        {
            Assert.Equal(
                    "000000000000000000001234565",
                    Payments.CreateQRReference("123456"));
        }

        [Fact]
        public void CreateQRReferenceWithWhitespace()
        {
            Assert.Equal(
                    "000000000000000000001234565",
                    Payments.CreateQRReference("  12 3456 "));
        }

        [Fact]
        public void RawReferenceWithInvalidCharacters()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                () => Payments.CreateQRReference("1134a56")
            );
            Assert.Equal("Invalid character in reference (digits allowed only)", ex.Message);
        }

        [Fact]
        public void RawReferenceTooLong()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                    () => Payments.CreateQRReference("123456789012345678901234567")
                );
            Assert.Equal("Reference number is too long", ex.Message);
        }
    }
}
