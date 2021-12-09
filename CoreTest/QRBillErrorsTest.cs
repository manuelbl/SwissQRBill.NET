//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System;
using Xunit;

namespace Codecrete.SwissQRBill.CoreTest
{
    public class QRBillErrorsTest
    {
        [Fact]
        public void ThrowsRuntimeException()
        {
            Assert.Throws<QRBillGenerationException>(
                () => GenerateWithFailingCanvas()
            );
        }

        private void GenerateWithFailingCanvas()
        {
            Bill bill = SampleData.CreateExample1();
            FailingCanvas canvas = new FailingCanvas();
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            QRBill.Draw(bill, canvas);
        }

        [Fact]
        public void ThrowsValidationError1()
        {
            var exception = Assert.Throws<QRBillValidationException>(
                () => GenerateWithInvalidData1()
            );
            Assert.Equal("QR bill data is invalid: field \"creditor.name\" may not be empty (field_value_missing)", exception.Message);
        }

        private static void GenerateWithInvalidData1()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Creditor.Name = " ";
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.PDF;
            QRBill.Generate(bill);
        }

        [Fact]
        public void ThrowsValidationError2()
        {
            var exception = Assert.Throws<QRBillValidationException>(
                () => GenerateWithInvalidData2()
            );
            Assert.Equal("QR bill data is invalid: the value for field \"billInformation\" should not exceed a length of 140 characters (field_value_too_long)", exception.Message);
        }

        private static void GenerateWithInvalidData2()
        {
            Bill bill = SampleData.CreateExample1();
            bill.UnstructuredMessage = null;
            bill.BillInformation = "//" + new String('X', 150);
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.PDF;
            QRBill.Generate(bill);
        }

        [Fact]
        public void ThrowsValidationError3()
        {
            var exception = Assert.Throws<QRBillValidationException>(
                () => GenerateWithInvalidData3()
            );
            Assert.Equal("QR bill data is invalid: currency should be \"CHF\" or \"EUR\" (currency_not_chf_or_eur); reference is invalid; it is neither a valid QR reference nor a valid ISO 11649 reference (ref_invalid)", exception.Message);
        }

        private static void GenerateWithInvalidData3()
        {
            Bill bill = SampleData.CreateExample1();
            bill.Reference = "RF1234";
            bill.Currency = "XXX";
            bill.Format.OutputSize = OutputSize.QrBillOnly;
            bill.Format.GraphicsFormat = GraphicsFormat.PDF;
            QRBill.Generate(bill);
        }

        class FailingCanvas : AbstractCanvas
        {
            public override void AddRectangle(double x, double y, double width, double height)
            {
                throw new NotImplementedException();
            }

            public override void CloseSubpath()
            {
                throw new NotImplementedException();
            }

            public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
            {
                throw new NotImplementedException();
            }

            protected override void Dispose(bool disposing)
            {
                // be nice
            }

            public override void FillPath(int color, bool smoothing)
            {
                throw new NotImplementedException();
            }

            public override void LineTo(double x, double y)
            {
                throw new NotImplementedException();
            }

            public override void MoveTo(double x, double y)
            {
                throw new NotImplementedException();
            }

            public override void PutText(string text, double x, double y, int fontSize, bool isBold)
            {
                throw new NotImplementedException();
            }

            public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
            {
                throw new NotImplementedException();
            }

            public override void StartPath()
            {
                throw new NotImplementedException();
            }

            public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle, bool smoothing)
            {
                throw new NotImplementedException();
            }

            public override byte[] ToByteArray()
            {
                throw new NotImplementedException();
            }
        }
    }
}
