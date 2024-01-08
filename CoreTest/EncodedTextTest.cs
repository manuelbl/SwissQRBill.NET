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
    public class EncodedTextTest
    {
        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateText1(Bill.QrDataSeparator separator, string newLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData1(separator);
            Assert.Equal(SampleQRCodeText.CreateQrCodeText1(newLine), QRBill.EncodeQrCodeText(bill));
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateText2(Bill.QrDataSeparator separator, string newLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData2(separator);
            Assert.Equal(SampleQRCodeText.CreateQrCodeText2(newLine), QRBill.EncodeQrCodeText(bill));
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateText3(Bill.QrDataSeparator separator, string newLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData3(separator);
            Assert.Equal(SampleQRCodeText.CreateQrCodeText3(newLine), QRBill.EncodeQrCodeText(bill));
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateText4(Bill.QrDataSeparator separator, string newLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData4(separator);
            Assert.Equal(SampleQRCodeText.CreateQrCodeText4(newLine), QRBill.EncodeQrCodeText(bill));
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateText5(Bill.QrDataSeparator separator, string newLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData5(separator);
            Assert.Equal(SampleQRCodeText.CreateQrCodeText5(newLine), QRBill.EncodeQrCodeText(bill));
        }

        [Fact]
        public void CreateTextError1()
        {
            Assert.Throws<QRBillValidationException>(() =>
            {
                Bill bill = SampleData.CreateExample4();
                bill.Amount = -0.01m;
                QRBill.EncodeQrCodeText(bill);
            });
        }

        [Theory]
        [ClassData(typeof(NewLineTheoryData))]
        public void CreateTextEmptyReference(Bill.QrDataSeparator separator, string newLine)
        {
            Bill bill = SampleQRCodeText.CreateBillData3(separator);
            ValidationResult result = QRBill.Validate(bill);
            Assert.False(result.HasErrors);
            bill = result.CleanedBill;
            bill.Reference = "";
            Assert.Equal(SampleQRCodeText.CreateQrCodeText3(newLine), QRBill.EncodeQrCodeText(bill));
        }

        private class NewLineTheoryData : TheoryData<Bill.QrDataSeparator, string>
        {
            public NewLineTheoryData()
            {
                Add(Bill.QrDataSeparator.Lf, "\n");
                Add(Bill.QrDataSeparator.CrLf, "\r\n");
            }
        }

    }
}
