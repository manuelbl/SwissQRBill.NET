//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Docnet.Core;
using Docnet.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class VerifyImages
    {
        static VerifyImages()
        {
            VerifierSettings.RegisterFileConverter("pdf", ConvertPdfToPng);
            VerifyImageMagick.RegisterComparers(threshold: 0.35, ImageMagick.ErrorMetric.PerceptualHash);

            SvgSettings = new VerifySettings();
            SvgSettings.UseExtension("svg");
            SvgSettings.UseDirectory("ReferenceFiles");

            PngSettings = new VerifySettings();
            PngSettings.UseExtension("png");
            PngSettings.UseDirectory("ReferenceFiles");

            PdfSettings = new VerifySettings();
            PdfSettings.UseExtension("pdf");
            PdfSettings.UseDirectory("ReferenceFiles");
        }

        private static ConversionResult ConvertPdfToPng(Stream stream, IReadOnlyDictionary<string, object> context)
        {
            var pngStreams = new List<Stream>();

            using var docReader = DocLib.Instance.GetDocReader(stream.ToArray(), new PageDimensions(scalingFactor: 2));
            for (var pageIndex = 0; pageIndex < docReader.GetPageCount(); pageIndex++)
            {
                using var pageReader = docReader.GetPageReader(pageIndex);
                var width = pageReader.GetPageWidth();
                var height = pageReader.GetPageHeight();
                var pixelData = pageReader.GetImage();
                var image = Image.LoadPixelData<Bgra32>(pixelData, width, height);
                var pngStream = new MemoryStream();
                image.SaveAsPng(pngStream);
                pngStreams.Add(pngStream);
            }

            return new ConversionResult(null, pngStreams.Select(e => new Target("png", e)));
        }

        protected static readonly VerifySettings SvgSettings;
        protected static readonly VerifySettings PngSettings;
        protected static readonly VerifySettings PdfSettings;

        public static SettingsTask Verify(byte[] imageData, GraphicsFormat format, [CallerFilePath] string sourceFile = "")
        {
            VerifySettings settings = format switch
            {
                GraphicsFormat.SVG => SvgSettings,
                GraphicsFormat.PNG => PngSettings,
                _ => PdfSettings
            };

            return Verifier.Verify(imageData, settings, sourceFile);
        }

        public static SettingsTask VerifySvg(byte[] svg, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(svg, SvgSettings, sourceFile);
        }

        public static SettingsTask VerifyPng(byte[] png, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(png, PngSettings, sourceFile);
        }

        public static SettingsTask VerifyPdf(byte[] pdf, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(pdf, PdfSettings, sourceFile);
        }
    }
}