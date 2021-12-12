//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Codecrete.SwissQRBill.SystemDrawing
{
    /// <summary>
    /// Canvas for generating a PNG file.
    /// <para>
    /// The resulting PNG file can be retrieved using <see cref="ToByteArray()"/>.</para>
    /// </summary>
    public class PngCanvas : BitmapCanvas
    {
        /// <summary>
        /// Creates a new canvas.
        /// </summary>
        /// <param name="width">Width of resulting PNG file, in pixels</param>
        /// <param name="height">Height of resulting PNG file, in pixels</param>
        /// <param name="dpi">Resolution of PNG file, in dpi (pixels per inch)</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public PngCanvas(int width, int height, float dpi, string fontFamilyList)
            : base(width, height, dpi, fontFamilyList)
        {
        }

        /// <summary>
        /// Returns the result as a GDI+ bitmap.
        /// <para>
        /// The caller must take ownership of the bitmap and dispose it.
        /// </para>
        /// <para>
        /// This method can only be called once. Thereafter, it is no longer
        /// possible to call any drawing methods or to call it a second time.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public override byte[] ToByteArray()
        {
            Bitmap bitmap = ToBitmap();
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            bitmap.Dispose();
            return stream.ToArray();
        }
    }
}
