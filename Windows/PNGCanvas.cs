//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Drawing.Imaging;
using System.IO;

namespace Codecrete.SwissQRBill.Windows
{
    /// <summary>
    /// Canvas for generating a PNG file.
    /// <para>
    /// The resulting PNG file can be retrieved using <see cref="ToByteArray()"/>.</para>
    /// </summary>
    public class PNGCanvas : BitmapCanvas
    {
        /// <summary>
        /// Creates a new canvas.
        /// </summary>
        /// <param name="width">Width of resulting PNG file, in mm</param>
        /// <param name="height">Height of resulting PNG file, in mm</param>
        /// <param name="dpi">Resolution of PNG file, in dpi (pixels per inch)</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public PNGCanvas(double width, double height, float dpi, string fontFamilyList)
            : base((int)(width / 25.4 * dpi), (int)(height / 25.4 * dpi), dpi, fontFamilyList)
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
            using (var bitmap = ToBitmap())
            {
                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Writes the resulting graphics as a PNG image to the specified stream.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            using (var bitmap = ToBitmap())
            {
                bitmap.Save(stream, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Writes the resulting graphics as a PNG image to the specified file path.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="path">The path (file name) to write to.</param>
        public void SaveAs(string path)
        {
            using (var bitmap = ToBitmap())
            using (var stream = File.OpenWrite(path))
            {
                bitmap.Save(stream, ImageFormat.Png);
            }
        }
    }
}
