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
    }
}
