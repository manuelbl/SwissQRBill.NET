//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;

namespace Codecrete.SwissQRBill.PixelCanvas
{
    /// <summary>
    /// Factory for creating <c>SVGCanvas</c> instances
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Rename would break API backward compatibility")]
    public class PNGCanvasFactory : ICanvasFactory
    {
        bool ICanvasFactory.CanCreate(BillFormat format)
        {
            return format.GraphicsFormat == GraphicsFormat.PNG;
        }

        ICanvas ICanvasFactory.Create(BillFormat format, double width, double height)
        {
            return new PNGCanvas(width, height, format.Resolution, format.FontFamily);
        }
    }
}
