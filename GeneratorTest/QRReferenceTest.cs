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
    public class QRReferenceTest
    {
        [Fact]
        void ValidQRReference()
        {
            Assert.True(Payments.IsValidQRReference("210000000003139471430009017"));
        }

        [Fact]
        void InvalidLengthQRReference()
        {
            Assert.False(Payments.IsValidQRReference("2100000003139471430009017"));
        }

        [Fact]
        void ValidQRReferenceWithSpaces()
        {
            Assert.True(Payments.IsValidQRReference("21 00000 00003 13947 14300 09017"));
        }

        [Fact]
        void InvalidQRReferenceWithLetters()
        {
            Assert.False(Payments.IsValidQRReference("210000S00003139471430009017"));
        }

        [Fact]
        void IinvalidQRReferenceWithSpecialChar()
        {
            Assert.False(Payments.IsValidQRReference("210000000%03139471430009017"));
        }

        [Fact]
        void InvalidCheckDigitQRReference()
        {
            Assert.False(Payments.IsValidQRReference("210000000003139471430009016"));
        }

        [Fact]
        void FormatQRReference()
        {
            Assert.Equal("12 34560 00000 00129 11462 90514",
                    Payments.FormatQRReferenceNumber("123456000000001291146290514"));
        }
    }
}
