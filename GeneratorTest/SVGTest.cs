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
    public class SVGTest
    {
        [Fact]
        void SvgWithChallengingCharacters()
        {
            Bill bill = SampleData.CreateExample1();
            bill.UnstructuredMessage = "<h1>&&\"ff\"'t'";
            bill.Format.OutputSize = OutputSize.QRBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.SVG;
            byte[] svg = QRBill.Generate(bill);
            FileComparison.AssertFileContentsEqual(svg, "qrbill_sc1.svg");
        }
    }
}
