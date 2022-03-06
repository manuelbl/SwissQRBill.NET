//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Codecrete.SwissQRBill.WindowsTest
{
    public class VerifyImages
    {
        static VerifyImages()
        {
            VerifierSettings.RegisterFileConverter("emf", Convert);
            VerifyImageMagick.RegisterComparers(threshold: 0.35, ImageMagick.ErrorMetric.PerceptualHash);

            PngSettings = new VerifySettings();
            PngSettings.UseExtension("png");
            PngSettings.UseDirectory("ReferenceFiles");

            EmfSettings = new VerifySettings();
            EmfSettings.UseExtension("emf");
            EmfSettings.UseDirectory("ReferenceFiles");
        }

        protected static readonly VerifySettings PngSettings;
        protected static readonly VerifySettings EmfSettings;

        public static SettingsTask VerifyPng(byte[] png, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(png, PngSettings, sourceFile);
        }

        public static SettingsTask VerifyEmf(byte[] emf, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(emf, EmfSettings, sourceFile);
        }

        private static ConversionResult Convert(Stream stream, IReadOnlyDictionary<string, object> context)
        {
            // copy metafile to buffer
            using MemoryStream buffer = new MemoryStream();
            stream.CopyTo(buffer);

            // retrieve DPI from metafile
            EmfMetaInfo metaInfo = new EmfMetaInfo(buffer.ToArray());
            int sourceDpi = metaInfo.Dpi;

            // read metafile
            buffer.Position = 0;
            using Metafile metafile = (Metafile)Image.FromStream(buffer);

            // compute bitmap size (300 dpi, independent of source DPI)
            var pageUnit = GraphicsUnit.Pixel;
            var metafileBounds = metafile.GetBounds(ref pageUnit);
            float scale = 300f / sourceDpi;
            var bitmapRect = new Rectangle(0, 0, (int)Math.Round(metafileBounds.Width * scale), (int)Math.Round(metafileBounds.Height * scale));

            // create bitmap and fill with white background
            using Bitmap bitmap = new Bitmap(bitmapRect.Width, bitmapRect.Height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(300, 300);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, bitmapRect);

            // draw metafile to bitmap
            graphics.DrawImage(metafile, bitmapRect, metafileBounds, pageUnit);

            // save bitmap as PNG
            MemoryStream result = new MemoryStream();
            bitmap.Save(result, ImageFormat.Png);

            // return PNG
            return new ConversionResult(null, "png", result);
        }
    }
}