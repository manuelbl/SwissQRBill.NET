//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
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

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void DecodeText1NewLine(Bill.QrDataSeparator separator, string newLine, bool extraNewLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData1(separator);
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText1(newLine) + (extraNewLine ? newLine : ""));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void DecodeText2NewLine(Bill.QrDataSeparator separator, string newLine, bool extraNewLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData2(separator);
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText2(newLine) + (extraNewLine ? newLine : ""));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void DecodeText3NewLine(Bill.QrDataSeparator separator, string newLine, bool extraNewLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData3(separator);
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText3(newLine) + (extraNewLine ? newLine : ""));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void DecodeText4NewLine(Bill.QrDataSeparator separator, string newLine, bool extraNewLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData4(separator);
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText4(newLine) + (extraNewLine ? newLine : ""));
            TestHelper.NormalizeDecodedBill(bill2);
            Assert.Equal(bill, bill2);
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void DecodeText5NewLine(Bill.QrDataSeparator separator, string newLine, bool extraNewLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData5(separator);
            TestHelper.NormalizeSourceBill(bill);
            Bill bill2 = QRBill.DecodeQrCodeText(SampleQRCodeText.CreateQrCodeText5(newLine) + (extraNewLine ? newLine : ""));
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

        [Theory]
        [InlineData("garbage")]
        [InlineData("SPC\r\n0100\r\n\r\n\r\n")]
        [InlineData("SPC1\r\n0200\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("SPC1\r\n0200\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        public void InvalidText_KeyDataStructureInvalidError(string qrText)
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText(qrText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyDataStructureInvalid, ValidationConstants.FieldQrType);
        }

        [Theory]
        [InlineData("SPC\r\n0101\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("SPC\r\n020\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        [InlineData("SPC\r\n020f\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n")]
        public void DecodeInvalidVersion(string qrCodeText)
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(() => QRBill.DecodeQrCodeText(qrCodeText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyVersionUnsupported, ValidationConstants.FieldVersion);
        }

        [Fact]
        public void DecodeIgnoreMinorVersion()
        {
            Bill bill = SampleQRCodeText.CreateBillData1(Bill.QrDataSeparator.Lf);
            TestHelper.NormalizeSourceBill(bill);
            string qrCodeText = SampleQRCodeText.CreateQrCodeText1();
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
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyCodingTypeUnsupported, ValidationConstants.FieldCodingType);
        }

        [Fact]
        public void DecodeInvalidNumber()
        {
            string invalidText = SampleQRCodeText.CreateQrCodeText1().Replace("3949.75", "1239d49.75");
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(invalidText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyNumberInvalid, ValidationConstants.FieldAmount);
        }

        [Fact]
        public void DecodeMissingEpd()
        {
            string invalidText = SampleQRCodeText.CreateQrCodeText1().Replace("EPD", "E_P");
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                        () => QRBill.DecodeQrCodeText(invalidText));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyDataStructureInvalid, ValidationConstants.FieldTrailer);
        }
        
         [Fact]
        public void DecodeMethodShouldReturnValidBillWhenQrCodeTextIsValid()
        {
            var bill = QRBill.DecodeQrCodeText(SampleData.CreateQrCode1());
            Assert.NotNull(bill);
            Assert.Equal(Bill.QrBillStandardVersion.V2_0, bill.Version);
            Assert.Equal(2500.00m, bill.Amount);
            Assert.Equal("CHF", bill.Currency);
            Assert.Equal("CH1234567890123456789", bill.Account);
            Assert.Equal("Steuerverwaltung der Stadt Bern", bill.Creditor.Name);
            Assert.Equal("Bundesgasse", bill.Creditor.Street);
            Assert.Equal("33", bill.Creditor.HouseNo);
            Assert.Equal("3011", bill.Creditor.PostalCode);
            Assert.Equal("Bern", bill.Creditor.Town);
            Assert.Equal("CH", bill.Creditor.CountryCode);
            Assert.Equal("Martina Muster", bill.Debtor.Name);
            Assert.Equal("Bubenbergplatz", bill.Debtor.Street);
            Assert.Equal("1", bill.Debtor.HouseNo);
            Assert.Equal("3011", bill.Debtor.PostalCode);
            Assert.Equal("Bern", bill.Debtor.Town);
            Assert.Equal("CH", bill.Debtor.CountryCode);
            Assert.Equal("QRR", bill.ReferenceType);
            Assert.Equal("123456789012345678901234567", bill.Reference);
            Assert.Equal("1. Steuerrate 2020", bill.UnstructuredMessage);
            Assert.Equal("//S1/11/200627/30/115140892/31/200627/32/7.7/40/0:30", bill.BillInformation);
        }

        [Fact]
        public void DecodeMethodShouldHandleInvalidAmountIfParameterIsSupplied()
        {
            var bill = QRBill.DecodeQrCodeText(SampleData.CreateInvalidQrCode1(), true);
            Assert.NotNull(bill);
            Assert.Equal(Bill.QrBillStandardVersion.V2_0, bill.Version);
            Assert.Null(bill.Amount);
            Assert.Equal("CHF", bill.Currency);
            Assert.Equal("CH8430000001800003797", bill.Account);
            Assert.Equal("AXA Versicherungen AG", bill.Creditor.Name);
            Assert.Equal("General-Guisan-Str.", bill.Creditor.Street);
            Assert.Equal("40", bill.Creditor.HouseNo);
            Assert.Equal("8401", bill.Creditor.PostalCode);
            Assert.Equal("Winterthur", bill.Creditor.Town);
            Assert.Equal("CH", bill.Creditor.CountryCode);
            Assert.Equal("Testfirma AG", bill.Debtor.Name);
            Assert.Equal("Hauptstrasse", bill.Debtor.Street);
            Assert.Equal("61", bill.Debtor.HouseNo);
            Assert.Equal("6210", bill.Debtor.PostalCode);
            Assert.Equal("Sursee", bill.Debtor.Town);
            Assert.Equal("CH", bill.Debtor.CountryCode);
            Assert.Equal("QRR", bill.ReferenceType);
            Assert.Equal("000000001000285497220812814", bill.Reference);
            Assert.Equal("", bill.UnstructuredMessage);
            Assert.Equal("", bill.BillInformation);
        }

        [Fact]
        public void DecodeMethodShouldFailIfAmountIsInvalidValue()
        {
            QRBillValidationException err = Assert.Throws<QRBillValidationException>(
                () => QRBill.DecodeQrCodeText(SampleData.CreateInvalidQrCode1()));
            TestHelper.AssertSingleError(err.Result, ValidationConstants.KeyNumberInvalid, ValidationConstants.FieldAmount);
        }

        private class NewLineTheoryData : TheoryData<Bill.QrDataSeparator, string, bool>
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "false positive")]
            public NewLineTheoryData()
            {
                Add(Bill.QrDataSeparator.Lf, "\n", false);
                Add(Bill.QrDataSeparator.Lf, "\n", true);
                Add(Bill.QrDataSeparator.CrLf, "\r\n", false);
                Add(Bill.QrDataSeparator.CrLf, "\r\n", true);
                Add(Bill.QrDataSeparator.Lf, "\r", false);
                Add(Bill.QrDataSeparator.Lf, "\r", true);
            }
        }
    }
}
