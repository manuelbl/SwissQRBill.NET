//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Codecrete.SwissQRBill.SystemDrawing
{
    /// <summary>
    /// Canvas for generating Windows Metafiles (EMF).
    /// </summary>
    public class MetafileCanvas : SystemDrawingCanvas
    {
        private MemoryStream _stream;
        private Metafile _metafile;

        /// <summary>
        /// Creates a new canvas.
        /// </summary>
        /// <param name="width">Width of resulting bitmap, in mm</param>
        /// <param name="height">Height of resulting bitmap, in mm</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public MetafileCanvas(double width, double height, string fontFamilyList)
            : base(fontFamilyList)
        {
            _stream = new MemoryStream();
            using Graphics offScreenGraphics = Graphics.FromHwndInternal(IntPtr.Zero);

            // The applied dpi seems to depend on the scaling of the screen.
            // So it needs to be taken into consideration.
            float scale = offScreenGraphics.DpiX / 25.4f;
            _metafile = new Metafile(
                _stream,
                offScreenGraphics.GetHdc(),
                new RectangleF(0, 0, (float)width * scale, (float)height * scale),
                MetafileFrameUnit.Pixel,
                EmfType.EmfPlusDual
            );
            offScreenGraphics.ReleaseHdc();

            var graphics = Graphics.FromImage(_metafile);
            SetOffset(0, (float)height * scale);
            InitGraphics(graphics, true, scale);
        }

        public override byte[] ToByteArray()
        {
            Close();
            _metafile.Dispose();
            _metafile = null;
            return _stream.ToArray();
        }

        public Stream ToStream()
        {
            Close();
            _metafile.Dispose();
            _metafile = null;
            Stream stream = _stream;
            _stream.Dispose();
            _stream = null;
            return stream;
        }

        /// <summary>
        /// Returns the result as a metafile instance.
        /// <para>
        /// The caller must take ownership of the bitmap and dispose it.
        /// </para>
        /// <para>
        /// This method can only be called once. Thereafter, it is no longer
        /// possible to draw to this canvas or to call this method a second time.
        /// </para>
        /// </summary>
        /// <returns>The metafile.</returns>
        public Metafile ToMetafile()
        {
            Metafile metafile = _metafile;
            _metafile = null;
            Close();

            return metafile;
        }

        protected override void Dispose(bool disposing)
        {
            base.Close();

            if (_metafile != null)
            {
                _metafile.Dispose();
            }

            if (_stream != null)
            {
                _stream.Dispose();
            }
        }
    }
}
