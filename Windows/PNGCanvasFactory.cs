//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;

namespace Codecrete.SwissQRBill.Windows
{
    /// <summary>
    /// Factory for creating <c>PngCanvas</c> instances
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Rename would break API backward compatibility")]
    public class PNGCanvasFactory : ICanvasFactory
    {
        /// <inheritdoc />
        bool ICanvasFactory.CanCreate(BillFormat format)
        {
            return format.GraphicsFormat == GraphicsFormat.PNG;
        }

        /// <inheritdoc />
        ICanvas ICanvasFactory.Create(BillFormat format, double width, double height)
        {
            return new PNGCanvas(width, height, format.Resolution, format.FontFamily);
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
            CanvasCreator.Register(new PNGCanvasFactory());
        }
    }
}