//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class SuperfluousLineFeedTest
    {
        [Fact]
        private void WithoutAlternativeSchemes()
        {
            Bill bill = SampleData.CreateExample2();
            DecodedTextTest.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill) + "\r\n");
            DecodedTextTest.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        private void WithAlternativeSchemes()
        {
            Bill bill = SampleData.CreateExample1();
            DecodedTextTest.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill) + "\r\n");
            DecodedTextTest.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        private void InvalidLineFeed()
        {
            Bill bill = SampleData.CreateExample1();
            DecodedTextTest.NormalizeSourceBill(bill);
            string qrText = QRBill.EncodeQrCodeText(bill) + "\r\n ";
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(qrText));
            DecodedTextTest.AssertSingleError(err.Result, ValidationConstants.KeyValidDataStructure, ValidationConstants.FieldQrType);
        }
    }
}
