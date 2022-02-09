//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace libQrCodeGenerator.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Factory for creating <c>PDFCanvas</c> instances
    /// </summary>
    public class PDFCanvasFactory : ICanvasFactory
    {
        bool ICanvasFactory.CanCreate(BillFormat format)
        {
            return format.GraphicsFormat == GraphicsFormat.PDF;
        }

        ICanvas ICanvasFactory.Create(BillFormat format, double width, double height)
        {
            return new PDFCanvas(width, height);
        }
    }
}
