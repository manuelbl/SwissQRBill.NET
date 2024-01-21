//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class MarginTest
    {

        [Fact]
        public Task CreateA4SvgBill1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Format.MarginLeft = 8.0;
            bill.Format.MarginRight = 8.0;
            return GenerateAndCompareBill(bill);
        }

        [Fact]
        public Task CreateA4SvgBill2()
        {
            Bill bill = SampleData.CreateExample2();
            bill.Format.MarginLeft = 12.0;
            bill.Format.MarginRight = 12.0;
            return GenerateAndCompareBill(bill);
        }

        [Fact]
        public Task CreateA4SvgBill6()
        {
            Bill bill = SampleData.CreateExample6();
            bill.Format.MarginLeft = 10.0;
            bill.Format.MarginRight = 9.0;
            return GenerateAndCompareBill(bill);
        }

        private static Task GenerateAndCompareBill(Bill bill)
        {
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] imageData = QRBill.Generate(bill);
            return VerifyImages.VerifySvg(imageData);
        }
    }
}
