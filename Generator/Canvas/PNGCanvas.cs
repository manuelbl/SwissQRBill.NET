//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Canvas for generating PNG files.
    /// </summary>
    /// <remarks>
    /// PNGs are not an optimal file format for QR bills. Vector formats such a SVG
    /// or PDF are of better quality and use far less processing power to generate
    /// </remarks>
    public class PNGCanvas : AbstractCanvas
    {
        private readonly int _resolution;
        private readonly float _coordinateScale;
        private Bitmap _bitmap;
        private Graphics _graphics;
        private List<PointF> _pathPoints;
        private List<byte> _pathTypes;
        private FontFamily _fontFamily;

        /// <summary>
        /// Initializes a new instance using the specified resolution.
        /// <para>
        /// It is recommended to use at least 144 dpi for a readable result.
        /// </para>
        /// </summary>
        /// <param name="resolution">The resolutionn of the image to generate (in pixels per inch).</param>
        public PNGCanvas(int resolution)
        {
            _resolution = resolution;
            _coordinateScale = (float)(resolution / 25.4);
        }

        public override void SetupPage(double width, double height, string fontFamily)
        {
            // setup font metrics
            SetupFontMetrics(fontFamily);
            _fontFamily = new FontFamily(FontMetrics.FirstFontFamily);

            // create image
            int w = (int)(width * _coordinateScale + 0.5);
            int h = (int)(height * _coordinateScale + 0.5);
            _bitmap = new Bitmap(w, h);
            _bitmap.SetResolution(_resolution, _resolution);

            // create graphics context
            _graphics = Graphics.FromImage(_bitmap);

            // clear background
            _graphics.FillRectangle(Brushes.White, 0, 0, w, h);

            // enable high quality output
            _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
            _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // initialize transformation
            SetTransformation(0, 0, 0, 1, 1);
        }

        public override byte[] GetResult()
        {
            _graphics.Dispose();
            _graphics = null;

            using (MemoryStream stream = new MemoryStream())
            {
                _bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        protected void Close()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            if (_fontFamily != null)
            {
                _fontFamily.Dispose();
                _fontFamily = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            // Our coordinate system extends from the bottom upwards. .NET's system
            // extends from the top downwards. So Y coordinates need to be treated specially.
            translateX *= _coordinateScale;
            translateY *= _coordinateScale;

            Matrix matrix = new Matrix();
            matrix.Translate((float)translateX, _bitmap.Height - (float)translateY);
            if (rotate != 0)
            {
                matrix.Rotate((float)(-rotate / Math.PI * 180));
            }

            if (scaleX != 1 || scaleY != 1)
            {
                matrix.Scale((float)scaleX, (float)scaleY);
            }

            _graphics.Transform = matrix;
        }

        public override void StartPath()
        {
            _pathPoints = new List<PointF>();
            _pathTypes = new List<byte>();
        }

        public override void CloseSubpath()
        {
            int lastIndex = _pathTypes.Count - 1;
            byte pathType = _pathTypes[lastIndex];
            pathType |= (byte)PathPointType.CloseSubpath;
            _pathTypes[lastIndex] = pathType;
        }

        public override void MoveTo(double x, double y)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _pathPoints.Add(new PointF((float)x, (float)y));
            _pathTypes.Add((byte)PathPointType.Start);
        }

        public override void LineTo(double x, double y)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _pathPoints.Add(new PointF((float)x, (float)y));
            _pathTypes.Add((byte)PathPointType.Line);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;
            width *= _coordinateScale;
            height *= -_coordinateScale;

            _pathPoints.Add(new PointF((float)x, (float)y));
            _pathTypes.Add((byte)PathPointType.Start);
            _pathPoints.Add(new PointF((float)(x + width), (float)y));
            _pathTypes.Add((byte)PathPointType.Line);
            _pathPoints.Add(new PointF((float)(x + width), (float)(y + height)));
            _pathTypes.Add((byte)PathPointType.Line);
            _pathPoints.Add(new PointF((float)x, (float)(y + height)));
            _pathTypes.Add((byte)PathPointType.Line);
            _pathPoints.Add(new PointF((float)x, (float)y));
            _pathTypes.Add((byte)PathPointType.Line | (byte)PathPointType.CloseSubpath);
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            x1 *= _coordinateScale;
            y1 *= -_coordinateScale;
            x2 *= _coordinateScale;
            y2 *= -_coordinateScale;
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _pathPoints.Add(new PointF((float)x1, (float)y1));
            _pathTypes.Add((byte)PathPointType.Bezier);
            _pathPoints.Add(new PointF((float)x2, (float)y2));
            _pathTypes.Add((byte)PathPointType.Bezier);
            _pathPoints.Add(new PointF((float)x, (float)y));
            _pathTypes.Add((byte)PathPointType.Bezier);
        }

        public override void FillPath(int color)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color - 16777216)))
            using (GraphicsPath path = new GraphicsPath(_pathPoints.ToArray(), _pathTypes.ToArray(), FillMode.Winding))
            {
                _graphics.FillPath(brush, path);
            }
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            using (Pen pen = new Pen(Color.FromArgb(color - 16777216), (float)strokeWidth))
            using (GraphicsPath path = new GraphicsPath(_pathPoints.ToArray(), _pathTypes.ToArray()))
            {
                _graphics.DrawPath(pen, path);
            }
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            FontStyle style = isBold ? FontStyle.Bold : FontStyle.Regular;
            using (Font font = new Font(_fontFamily, fontSize, style, GraphicsUnit.Point))
            {
                float ascent = _fontFamily.GetCellAscent(style) / 2048.0f * fontSize / 72 * _resolution;
                x *= _coordinateScale;
                y *= -_coordinateScale;
                y -= ascent;

                _graphics.DrawString(text, font, Brushes.Black, (float)x, (float)y, StringFormat.GenericTypographic);
            }
        }
    }
}
