﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using libQrCodeGenerator.SwissQRBill.Generator;
using libQrCodeGenerator.SwissQRBill.Generator.Canvas;

namespace libQrCodeGenerator.SwissQRBill.PixelCanvas
{
    /// <summary>
    /// Factory for creating <c>SVGCanvas</c> instances
    /// </summary>
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
