//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Codecrete.SwissQRBill.PixelCanvasTest
{
    public class VerifyImages
    {
        protected VerifyImages() { }

        static VerifyImages()
        {
            VerifyImageMagick.RegisterComparers(threshold: 0.35, ImageMagick.ErrorMetric.PerceptualHash);

            Settings.UseDirectory("ReferenceFiles");
        }

        protected static readonly VerifySettings Settings = new();

        public static SettingsTask VerifyPng(byte[] png, [CallerFilePath] string sourceFile = "")
        {
            return Verifier.Verify(png, settings: Settings, extension: "png", sourceFile: sourceFile);
        }
    }
}