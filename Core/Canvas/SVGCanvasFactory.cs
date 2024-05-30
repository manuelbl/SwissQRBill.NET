//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Factory for creating <c>SVGCanvas</c> instances
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Rename would break API backward compatibility")]
    public class SVGCanvasFactory : ICanvasFactory
    {
        bool ICanvasFactory.CanCreate(BillFormat format)
        {
            return format.GraphicsFormat == GraphicsFormat.SVG;
        }

        ICanvas ICanvasFactory.Create(BillFormat format, double width, double height)
        {
            return new SVGCanvas(width, height, format.FontFamily);
        }
    }
}
