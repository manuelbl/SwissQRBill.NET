//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2022 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Windows;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Codecrete.SwissQRBill.WindowsTest
{
    [UsesVerify]
    public class EmfCanvasTest
    {
        public EmfCanvasTest()
        {
            SetProcessDPIAware();
        }

        [Fact]
        public Task EmfBillQrBill_ComparePng()
        {
            Bill bill = SampleData.CreateExample5();
            bill.Format.OutputSize = OutputSize.QrBillExtraSpace;

            byte[] emf;
            using (MetafileCanvas canvas = new MetafileCanvas(QRBill.QrBillWithHoriLineWidth, QRBill.QrBillWithHoriLineHeight, "\"Liberation Sans\",Arial, Helvetica"))
            {
                QRBill.Draw(bill, canvas);
                emf = canvas.ToByteArray();
            }

            return VerifyImages.VerifyEmf(emf);
        }

        [Fact]
        public Task EmfBillA4_ComparePng()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;

            byte[] png;
            using (MetafileCanvas canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Arial,\"Liberation Sans\",Helvetica"))
            {
                QRBill.Draw(bill, canvas);
                png = canvas.ToByteArray();
            }

            return VerifyImages.VerifyEmf(png);
        }

        [Fact]
        public void EmfToStream_RunsSuccessfully()
        {
            Bill bill = SampleData.CreateExample5();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            using MetafileCanvas canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, \"Liberation Sans\"");
            QRBill.Draw(bill, canvas);
            MemoryStream ms = new MemoryStream();
            canvas.ToStream().CopyTo(ms);
        }

        [Fact]
        public void EmfToByteArray_CorrectFrame()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            using MetafileCanvas canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, \"Liberation Sans\"");
            QRBill.Draw(bill, canvas);

            EmfMetaInfo metaInfo = new EmfMetaInfo(canvas.ToByteArray());
            Assert.Equal(257, metaInfo.NumRecords);

            var scale = metaInfo.Dpi / 25.4f;
            // Returns the frame in pixels
            var frame = metaInfo.GetFrame();
            Assert.Equal(0, frame.Left);
            Assert.Equal(0, frame.Top);
            int expectedWidth = (int)(QRBill.A4PortraitWidth * scale);
            Assert.InRange(frame.Right, expectedWidth - 2, expectedWidth + 2);
            int expectedHeight = (int)(QRBill.A4PortraitHeight * scale);
            Assert.InRange(frame.Bottom, expectedHeight - 2, expectedHeight + 2);
        }

        [Fact]
        public void EmfToMetafile_CorrectSize()
        {
            Bill bill = SampleData.CreateExample6();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            using MetafileCanvas canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, \"Liberation Sans\"");
            QRBill.Draw(bill, canvas);
            using Metafile metafile = canvas.ToMetafile();
            var graphicsUnit = GraphicsUnit.Millimeter;
            // GetBounds() returns the metafile frame in pixels
            var bounds = metafile.GetBounds(ref graphicsUnit);
            Assert.Equal(GraphicsUnit.Pixel, graphicsUnit);

            // Since we've just created the metafile, pixel units will use the current dpi
            float dpi = GetScreenDpi();
            float expectedWidth = (float)QRBill.A4PortraitWidth / 25.4f * dpi;
            Assert.InRange(bounds.Width, expectedWidth - 2, expectedWidth + 2);
            float expectedHeight = (float)QRBill.A4PortraitHeight / 25.4f * dpi;
            Assert.InRange(bounds.Height, expectedHeight - 2, expectedHeight + 2);
        }

        private static float GetScreenDpi()
        {
            using Graphics offScreenGraphics = Graphics.FromHwndInternal(IntPtr.Zero);
            return offScreenGraphics.DpiX;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
