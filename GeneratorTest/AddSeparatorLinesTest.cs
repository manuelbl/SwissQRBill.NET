﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    [VerifyXunit.UsesVerify]
    public class AddSeparatorLinesTest : VerifyTest
    {
        [Fact]
        public Task AddBothSeparators()
        {
            var bill = SampleData.CreateExample1();
            bill.Format.SeparatorType = SeparatorType.None;
            using (PDFCanvas canvas =
                new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);

                QRBill.DrawSeparators(SeparatorType.DottedLineWithScissors, true, canvas);

                MemoryStream ms = new MemoryStream();
                canvas.WriteTo(ms);

                return VerifyPdf(ms.ToArray());
            }
        }

        [Fact]
        public Task AddVerticalSeparator()
        {
            var bill = SampleData.CreateExample1();
            bill.Format.SeparatorType = SeparatorType.None;
            using (PDFCanvas canvas =
                new PDFCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight))
            {
                QRBill.Draw(bill, canvas);

                QRBill.DrawSeparators(SeparatorType.DashedLineWithScissors, false, canvas);

                MemoryStream ms = new MemoryStream();
                canvas.WriteTo(ms);

                return VerifyPdf(ms.ToArray());
            }
        }
    }
}
