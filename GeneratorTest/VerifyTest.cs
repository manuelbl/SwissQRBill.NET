using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ImageMagick;
using VerifyTests;
using VerifyXunit;

namespace Codecrete.SwissQRBill.GeneratorTest
{
    public class VerifyTest
    {
        static VerifyTest()
        {
            VerifyImageMagick.RegisterPdfToPngConverter();
        }

        protected readonly VerifySettings SvgSettings;
        protected readonly VerifySettings PngSettings;
        protected readonly VerifySettings PdfSettings;

        public VerifyTest()
        {
            SvgSettings = new VerifySettings();
            SvgSettings.UseExtension("svg");

            PngSettings = new VerifySettings();
            PngSettings.UseExtension("png");
            PngSettings.UseStreamComparer(ComparePng);

            PdfSettings = new VerifySettings();
            PdfSettings.UseExtension("pdf");
            PdfSettings.UseStreamComparer(ComparePng);
        }

        private static Task<CompareResult> ComparePng(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
        {
            using var imgReceived = new MagickImage(received, MagickFormat.Png);
            using var imgVerified = new MagickImage(verified, MagickFormat.Png);
            var diff = imgReceived.Compare(imgVerified, ErrorMetric.PerceptualHash);
            const double threshold = 0.02;
            var compare = diff < threshold;
            if (compare)
            {
                return Task.FromResult(CompareResult.Equal);
            }

            return Task.FromResult(CompareResult.NotEqual($"diff < threshold. threshold: {threshold}, diff: {diff}"));
        }

        protected Task VerifySvg(byte[] svg, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(svg, SvgSettings, sourceFile);
        }

        protected Task VerifyPng(byte[] png, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(png, PngSettings, sourceFile);
        }

        protected Task VerifyPdf(byte[] pdf, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(pdf, PdfSettings, sourceFile);
        }
    }
}