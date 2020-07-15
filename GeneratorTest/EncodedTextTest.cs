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
    public class EncodedTextTest
    {
        [Fact]
        public void CreateText1()
        {
            Bill bill = SampleQRCodeText.CreateBillData1();
            Assert.Equal(SampleQRCodeText.CreateQrCodeText1(false), QRBill.EncodeQrCodeText(bill));
        }

        [Fact]
        public void CreateText2()
        {
            Bill bill = SampleQRCodeText.CreateBillData2();
            Assert.Equal(SampleQRCodeText.CreateQrCodeText2(false), QRBill.EncodeQrCodeText(bill));
        }

        [Fact]
        public void CreateText3()
        {
            Bill bill = SampleQRCodeText.CreateBillData3();
            Assert.Equal(SampleQRCodeText.CreateQrCodeText3(false), QRBill.EncodeQrCodeText(bill));
        }

        [Fact]
        public void CreateText4()
        {
            Bill bill = SampleQRCodeText.CreateBillData4();
            Assert.Equal(SampleQRCodeText.CreateQrCodeText4(false), QRBill.EncodeQrCodeText(bill));
        }

        [Fact]
        public void CreateText5()
        {
            Bill bill = SampleQRCodeText.CreateBillData5();
            Assert.Equal(SampleQRCodeText.CreateQrCodeText5(false), QRBill.EncodeQrCodeText(bill));
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

        [Fact]
        public void CreateTextEmptyReference()
        {
            Bill bill = SampleQRCodeText.CreateBillData3();
            ValidationResult result = QRBill.Validate(bill);
            Assert.False(result.HasErrors);
            bill = result.CleanedBill;
            bill.Reference = "";
            Assert.Equal(SampleQRCodeText.CreateQrCodeText3(false), QRBill.EncodeQrCodeText(bill));
        }
    }
}
