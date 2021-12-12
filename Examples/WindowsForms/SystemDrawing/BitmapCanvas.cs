//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Drawing;
using System.Drawing.Imaging;

namespace Codecrete.SwissQRBill.SystemDrawing
{
    /// <summary>
    /// Canvas for generating a GDI+ bitmap.
    /// </summary>
    public class BitmapCanvas : SystemDrawingCanvas
    {
        private Bitmap _bitmap;

        /// <summary>
        /// Creates a new canvas.
        /// </summary>
        /// <param name="width">Width of resulting bitmap, in pixels</param>
        /// <param name="height">Height of resulting bitmap, in pixels</param>
        /// <param name="dpi">Resolution of bitmap, in dpi (pixels per inch)</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public BitmapCanvas(int width, int height, float dpi, string fontFamilyList)
            : base(fontFamilyList)
        {
            _bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            _bitmap.SetResolution(dpi, dpi);
            Graphics graphics = Graphics.FromImage(_bitmap);
            graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
            float scale = dpi / 25.4f;
            SetOffset(0, height);
            InitGraphics(graphics, true, scale);
        }

        /// <summary>
        /// Returns the result as a GDI+ bitmap.
        /// <para>
        /// The caller must take ownership of the bitmap and dispose it.
        /// </para>
        /// <para>
        /// This method can only be called once. Thereafter, it is no longer
        /// possible to draw to this canvas or to call this method a second time.
        /// </para>
        /// </summary>
        /// <returns>The bitmap.</returns>
        public Bitmap ToBitmap()
        {
            Bitmap bitmap = _bitmap;
            _bitmap = null;
            Close();

            return bitmap;
        }

        protected override void Dispose(bool disposing)
        {
            base.Close();

            if (_bitmap != null)
            {
                _bitmap.Dispose();
            }
        }
    }
}
