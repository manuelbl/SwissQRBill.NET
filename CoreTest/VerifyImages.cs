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

namespace Codecrete.SwissQRBill.CoreTest
{
    public class VerifyImages
    {
        protected static readonly VerifySettings Settings = new();

        static VerifyImages()
        {
            VerifierSettings.RegisterFileConverter("pdf", ConvertPdfToPng);
            VerifyImageMagick.RegisterComparers(threshold: 0.1, ImageMagick.ErrorMetric.PerceptualHash);

            Settings.UseDirectory("ReferenceFiles");
        }

        protected VerifyImages() { }

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

        public static SettingsTask Verify(byte[] imageData, GraphicsFormat format, [CallerFilePath] string sourceFile = "")
        {
            var extension = format.ToString().ToLowerInvariant();
            return Verifier.Verify(imageData, settings: Settings, extension: extension, sourceFile: sourceFile);
        }

        public static SettingsTask VerifySvg(byte[] svg, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(svg, settings: Settings, extension: "svg", sourceFile: sourceFile);
        }

        public static SettingsTask VerifyPng(byte[] png, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(png, settings: Settings, extension: "png", sourceFile: sourceFile);
        }

        public static SettingsTask VerifyPdf(byte[] pdf, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(pdf, settings: Settings, extension: "pdf", sourceFile: sourceFile);
        }
    }
}