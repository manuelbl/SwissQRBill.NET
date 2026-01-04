//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Factory for creating <c>PDFCanvas</c> instances
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Rename would break API backward compatibility")]
    public class PDFCanvasFactory : ICanvasFactory2
    {
        bool ICanvasFactory.CanCreate(BillFormat format)
        {
            return format.GraphicsFormat == GraphicsFormat.PDF;
        }

        ICanvas ICanvasFactory.Create(BillFormat format, double width, double height)
        {
            return new PDFCanvas(width, height);
        }

        ICanvas ICanvasFactory2.Create(BillFormat format, SpsCharacterSet characterSet, double width, double height)
        {
            if (characterSet == SpsCharacterSet.FullUnicode)
            {
                throw new NotSupportedException("The built-in PDF generator does not support the full Unicode character range.");
            }

            var fontSettings = characterSet == SpsCharacterSet.Latin1Subset ? PDFFontSettings.StandardHelvetica()
                : PDFFontSettings.EmbeddedLiberationSans();

            return new PDFCanvas(width, height, fontSettings);
        }
    }
}
