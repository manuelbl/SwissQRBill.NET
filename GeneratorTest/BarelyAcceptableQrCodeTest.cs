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
    public class BarelyAcceptableQrCodeTest
    {
        [Fact]
        private void WithoutAlternativeSchemes()
        {
            Bill bill = SampleData.CreateExample2();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill) + "\n");
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        private void WithAlternativeSchemes()
        {
            Bill bill = SampleData.CreateExample1();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill) + "\n");
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        private void InvalidLineFeed()
        {
            Bill bill = SampleData.CreateExample1();
            TestHelper.NormalizeSourceBill(bill);
            string qrText = QRBill.EncodeQrCodeText(bill) + "\n ";
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(qrText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyValidDataStructure, ValidationConstants.FieldQrType);
        }

        [Fact]
        private void TooManyLines()
        {
            Bill bill = SampleData.CreateExample1();
            TestHelper.NormalizeSourceBill(bill);
            string qrText = QRBill.EncodeQrCodeText(bill) + "\n\n";
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(qrText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyValidDataStructure, ValidationConstants.FieldQrType);
        }

        [Fact]
        private void NoNLAfterEPD()
        {
            Bill bill = SampleData.CreateExample2();
            TestHelper.NormalizeSourceBill(bill);
            string qrText = QRBill.EncodeQrCodeText(bill);
            qrText = qrText.Substring(0, qrText.Length - 1);
            Assert.EndsWith("EPD", qrText);

            Bill bill2 = QRBill.DecodeQrCodeText(qrText);
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        private void TooFewLines()
        {
            Bill bill = SampleData.CreateExample2();
            bill.UnstructuredMessage = null;
            TestHelper.NormalizeSourceBill(bill);
            string qrText = QRBill.EncodeQrCodeText(bill);
            qrText = qrText.Substring(0, qrText.Length - 5);
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(qrText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyValidDataStructure, ValidationConstants.FieldQrType);
        }
    }
}
