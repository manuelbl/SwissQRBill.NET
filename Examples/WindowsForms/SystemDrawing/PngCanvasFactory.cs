//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;

namespace Codecrete.SwissQRBill.SystemDrawing
{
    /// <summary>
    /// Factory for creating <c>PngCanvas</c> instances
    /// </summary>
    public class PngCanvasFactory : ICanvasFactory
    {
        /// <inheritdoc />
        bool ICanvasFactory.CanCreate(BillFormat format)
        {
            return format.GraphicsFormat == GraphicsFormat.PNG;
        }

        /// <inheritdoc />
        ICanvas ICanvasFactory.Create(BillFormat format, double width, double height)
        {
            var iWidth = (int)(width / 25.4 * format.Resolution);
            var iHeight = (int)(height / 25.4 * format.Resolution);
            return new PngCanvas(iWidth, iHeight, format.Resolution, format.FontFamily);
        }

        /// <summary>
        /// Register a canvas factory for PNG files for use with <see cref="QRBill"/>.
        /// <para>
        /// Call this method once at program start to enable the generation of PNG
        /// files using the System.Drawing / GDI+ classes on Windows.
        /// </para>
        /// </summary>
        public void Register()
        {
            CanvasCreator.Register(new PngCanvasFactory());
        }
    }
}