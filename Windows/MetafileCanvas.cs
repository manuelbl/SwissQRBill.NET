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

namespace Codecrete.SwissQRBill.Windows
{
    /// <summary>
    /// Canvas for generating Windows Metafiles (EMF).
    /// <para>
    /// For this class to generate correct EMF files, the application must be configured to be
    /// *dpiAware*, either by adding an application manifest and uncommenting the relevant section
    /// or by calling <c>SetProcessDPIAware()</c> at application start.
    /// </para>
    /// <code>
    /// [System.Runtime.InteropServices.DllImport("user32.dll")]
    /// private static extern bool SetProcessDPIAware();
    /// </code>
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
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first installed font family will be used.</param>
        public MetafileCanvas(double width, double height, string fontFamilyList)
            : base(fontFamilyList)
        {
            _stream = new MemoryStream();
            using (var offScreenGraphics = Graphics.FromHwndInternal(IntPtr.Zero))
            {
                var scale = offScreenGraphics.DpiX / 25.4f;
                _metafile = new Metafile(
                    _stream,
                    offScreenGraphics.GetHdc(),
                    new RectangleF(0, 0, (float)width * scale, (float)height * scale),
                    MetafileFrameUnit.Pixel,
                    EmfType.EmfPlusDual
                );
                offScreenGraphics.ReleaseHdc();

                var graphics = Graphics.FromImage(_metafile);
                graphics.PageUnit = GraphicsUnit.Pixel;
                SetOffset(0, (float)height * scale);
                InitGraphics(graphics, true, scale);
            }
        }

        /// <inheritdoc />
        public override byte[] ToByteArray()
        {
            Close();
            _metafile.Dispose();
            _metafile = null;
            return _stream.ToArray();
        }

        /// <summary>
        /// Returns the result as a metafile instance.
        /// <para>
        /// The caller must take ownership of the metafile and dispose it.
        /// </para>
        /// <para>
        /// This method can only be called once. Thereafter, it is no longer
        /// possible to draw to this canvas or to call this method a second time.
        /// </para>
        /// </summary>
        /// <returns>The metafile.</returns>
        public Metafile ToMetafile()
        {
            var metafile = _metafile;
            _metafile = null;
            Close();

            return metafile;
        }

        /// <summary>
        /// Writes the enhanced metafile (EMF) to the specified stream.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            Close();

            _metafile.Dispose();
            _metafile = null;
            _stream.CopyTo(stream);
            _stream.Dispose();
            _stream = null;
        }

        /// <summary>
        /// Writes the enhanced metafile (EMF) to the specified file path.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="path">The path (file name) to write to.</param>
        public void SaveAs(string path)
        {
            Close();

            _metafile.Dispose();
            _metafile = null;

            using (var stream = File.OpenWrite(path))
            {
                _stream.CopyTo(stream);
            }
            _stream.Dispose();
            _stream = null;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _metafile?.Dispose();

            _stream?.Dispose();

            Close();
        }
    }
}
