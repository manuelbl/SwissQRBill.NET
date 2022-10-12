//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;


namespace Codecrete.SwissQRBill.CoreTest
{
    [UsesVerify]
    public class QRBillTest
    {
        [Fact]
        public Task CreateQrBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBill3()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBill4()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBill5()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillExtraSpace;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBillFrench()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            bill.Format.Language = Language.FR;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task QrBillSentFromLI_CorrectAddress()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            bill.Format.LocalCountryCode = "LI";
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public void GetLibraryVersion()
        {
            string version = QRBill.LibraryVersion;
            Assert.Matches(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$", version);
        }
    }
}
