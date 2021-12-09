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
    public class BillFormatTest
    {
        [Fact]
        public void DefaultValueTest()
        {
            BillFormat format = new BillFormat();
            Assert.Equal(Language.EN, format.Language);
            Assert.Equal(GraphicsFormat.SVG, format.GraphicsFormat);
            Assert.Equal(OutputSize.QrBillOnly, format.OutputSize);
            Assert.Equal("Helvetica,Arial,\"Liberation Sans\"", format.FontFamily);
            Assert.Equal(SeparatorType.DashedLineWithScissors, format.SeparatorType);
            Assert.Equal(144, format.Resolution);
            Assert.Equal(5.0, format.MarginLeft);
            Assert.Equal(5.0, format.MarginRight);
        }

        [Fact]
        public void HashCodeTest()
        {
            BillFormat format1 = new BillFormat();
            BillFormat format2 = new BillFormat();
            Assert.Equal(format1.GetHashCode(), format2.GetHashCode());
        }

        [Fact]
        public void TestEqualsTrivial()
        {
            BillFormat format = new BillFormat();
            Assert.Equal(format, format);
            BillFormat nullFormat = null;
            Assert.NotEqual(format, nullFormat);
            Assert.NotEqual((object)"xxx", format);
        }

        [Fact]
        public void TestEquals()
        {
            BillFormat format1 = new BillFormat();
            BillFormat format2 = new BillFormat();
            Assert.Equal(format1, format2);
            Assert.Equal(format1, format2);

            format2.OutputSize = OutputSize.A4PortraitSheet;
            Assert.NotEqual(format1, format2);
        }
    }
}
