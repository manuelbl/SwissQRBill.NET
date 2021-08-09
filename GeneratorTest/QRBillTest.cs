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


namespace Codecrete.SwissQRBill.GeneratorTest
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
        public Task CreateQrBill5a()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.QrBillExtraSpace;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(svg);
        }

        [Fact]
        public Task CreateQrBill5b()
        {
            Bill bill = SampleData.CreateExample3();
#pragma warning disable CS0618 // Type or member is obsolete
            bill.Format.OutputSize = OutputSize.QrBillWithHorizontalLine;
#pragma warning restore CS0618 // Type or member is obsolete
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
        public void GetLibraryVersion()
        {
            string version = QRBill.LibraryVersion;
            Assert.Matches(@"^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$", version);
        }
    }
}
