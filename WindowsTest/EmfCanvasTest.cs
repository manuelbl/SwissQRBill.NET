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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Codecrete.SwissQRBill.WindowsTest
{
    public partial class EmfCanvasTest
    {
        public EmfCanvasTest()
        {
            SetProcessDPIAware();
        }

        [WindowsFact]
        public Task QrBillExtraSpace_ComparePng()
        {
            Bill bill = SampleData.CreateExample5();
            bill.Format.OutputSize = OutputSize.QrBillExtraSpace;

            byte[] emf;
            using (var canvas = new MetafileCanvas(QRBill.QrBillWithHoriLineWidth, QRBill.QrBillWithHoriLineHeight, "\"Liberation Sans\",Arial, Helvetica"))
            {
                QRBill.Draw(bill, canvas);
                emf = canvas.ToByteArray();
            }

            return VerifyImages.VerifyEmf(emf);
        }

        [WindowsFact]
        public Task QrBillA4_ComparePng()
        {
            Bill bill = SampleData.CreateExample3();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;

            byte[] png;
            using (var canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Arial,\"Liberation Sans\",Helvetica"))
            {
                QRBill.Draw(bill, canvas);
                png = canvas.ToByteArray();
            }

            return VerifyImages.VerifyEmf(png);
        }

        [WindowsFact]
        public void ToStream_RunsSuccessfully()
        {
            Bill bill = SampleData.CreateExample5();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            using (var canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, \"Liberation Sans\""))
            {
                QRBill.Draw(bill, canvas);
                var ms = new MemoryStream();
                canvas.WriteTo(ms);
            }

            Assert.True(true);
        }

        [WindowsFact]
        public void ToByteArray_CorrectFrame()
        {
            Bill bill = SampleData.CreateExample4();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            EmfMetaInfo metaInfo;
            using (var canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, \"Liberation Sans\""))
            {
                QRBill.Draw(bill, canvas);

                metaInfo = new EmfMetaInfo(canvas.ToByteArray());
            }

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

        [WindowsFact]
        public void ToMetafile_CorrectFrame()
        {
            Bill bill = SampleData.CreateExample6();
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
            GraphicsUnit graphicsUnit;
            RectangleF bounds;
            using (var canvas = new MetafileCanvas(QRBill.A4PortraitWidth, QRBill.A4PortraitHeight, "Helvetica, Arial, \"Liberation Sans\""))
            {
                QRBill.Draw(bill, canvas);
                using (Metafile metafile = canvas.ToMetafile())
                {
                    graphicsUnit = GraphicsUnit.Millimeter;
                    // GetBounds() returns the metafile frame in pixels
                    bounds = metafile.GetBounds(ref graphicsUnit);
                }
            }

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
            using (var offScreenGraphics = Graphics.FromHwndInternal(IntPtr.Zero))
            {
                return offScreenGraphics.DpiX;
            }
        }

        [DllImport("User32.dll")]
        static extern bool SetProcessDPIAware();
    }
}
