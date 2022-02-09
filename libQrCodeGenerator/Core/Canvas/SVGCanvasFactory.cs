//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace libQrCodeGenerator.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Factory for creating <c>SVGCanvas</c> instances
    /// </summary>
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
