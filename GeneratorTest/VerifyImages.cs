//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Codecrete.SwissQRBill.Generator;
using VerifyTests;
using VerifyXunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class VerifyImages
    {
        static VerifyImages()
        {
            VerifyImageMagick.RegisterComparers();

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

        protected static readonly VerifySettings SvgSettings;
        protected static readonly VerifySettings PngSettings;
        protected static readonly VerifySettings PdfSettings;

        public static Task Verify(byte[] imageData, GraphicsFormat format, [CallerFilePath] string sourceFile = "")
        {
            VerifySettings settings;
            if (format == GraphicsFormat.SVG)
            {
                settings = SvgSettings;
            }
            else if (format == GraphicsFormat.PNG)
            {
                settings = PngSettings;
            }
            else
            {
                settings = PdfSettings;
            }

            return Verifier.Verify(imageData, settings, sourceFile);
        }

        public static Task VerifySvg(byte[] svg, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(svg, SvgSettings, sourceFile);
        }

        public static Task VerifyPng(byte[] png, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(png, PngSettings, sourceFile);
        }

        public static Task VerifyPdf(byte[] pdf, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(pdf, PdfSettings, sourceFile);
        }
    }
}