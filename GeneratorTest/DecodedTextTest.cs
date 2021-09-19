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
    public class DecodedTextTest
    {
        [Fact]
        public void DecodeText1()
        {
            Bill bill = SampleData.CreateExample1();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeText2()
        {
            Bill bill = SampleData.CreateExample2();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeText3()
        {
            Bill bill = SampleData.CreateExample3();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeText4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format = new BillFormat(); // set default values
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(QRBill.EncodeQrCodeText(bill));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeTextB1A()
        {
            Bill bill = SampleQRCodeText.CreateBillData1();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText1(false));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeTextB1B()
        {
            Bill bill = SampleQRCodeText.CreateBillData1();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText1(true));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeTextB2()
        {
            Bill bill = SampleQRCodeText.CreateBillData2();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText2(false));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeTextB3()
        {
            Bill bill = SampleQRCodeText.CreateBillData3();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText3(false));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeTextB4()
        {
            Bill bill = SampleQRCodeText.CreateBillData4();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText4(false));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeTextB5()
        {
            Bill bill = SampleQRCodeText.CreateBillData5();
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText5(false));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeInvalidRefType()
        {
            Bill bill = SampleData.CreateExample3();
            string qrText = QRBill.EncodeQrCodeText(bill);
            qrText = qrText.Replace("SCOR", "XXXX");
            Bill bill2 = QRBill.DecodeQrCodeText(qrText);
            Assert.Equal("XXXX", bill2.ReferenceType);
        }

        [Fact]
        public void DecodeInvalidFormat1()
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText("garbage"));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyInvalidDataStructure, ValidationConstants.FieldQrType);
        }

        [Fact]
        public void DecodeInvalidFormat2A()
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                    () => QRBill.DecodeQrCodeText("SPC\r\n0100\r\n\r\n\r\n"));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyInvalidDataStructure, ValidationConstants.FieldQrType);
        }

        [Fact]
        public void DecodeInvalidFormat2B()
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText(
                       "SPC1\r\n0200\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyInvalidDataStructure, ValidationConstants.FieldQrType);
        }

        [Fact]
        public void DecodeInvalidFormat3()
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText(
                       "SPC1\r\n0200\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyInvalidDataStructure, ValidationConstants.FieldQrType);
        }

        [Theory]
        [InlineData("SPC\r\n0101\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("SPC\r\n020\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("SPC\r\n020f\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        public void DecodeInvalidVersion(string qrCodeText)
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText(qrCodeText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyUnsupportedVersion, ValidationConstants.FieldVersion);
        }

        [Fact]
        public void DecodeIgnoreMinorVersion()
        {
            Bill bill = SampleQRCodeText.CreateBillData1();
            TestHelper.NormalizeSourceBill(bill);
            string qrCodeText = SampleQRCodeText.CreateQrCodeText1(false);
            qrCodeText = qrCodeText.Replace("\n0200\n", "\n0201\n");
            Bill bill2 = QRBill.DecodeQrCodeText(qrCodeText);
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Fact]
        public void DecodeInvalidCodingType()
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText(
                       "SPC\r\n0200\r\n0\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n"));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyUnsupportedCodingType, ValidationConstants.FieldCodingType);
        }

        [Fact]
        public void DecodeInvalidNumber()
        {
            string invalidText = SampleQRCodeText.CreateQrCodeText1(false).Replace("3949.75", "1239d49.75");
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(invalidText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyInvalidNumber, ValidationConstants.FieldAmount);
        }

        [Fact]
        public void DecodeMissingEpd()
        {
            string invalidText = SampleQRCodeText.CreateQrCodeText1(false).Replace("EPD", "E_P");
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(invalidText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyInvalidDataStructure, ValidationConstants.FieldTrailer);
        }
    }
}
