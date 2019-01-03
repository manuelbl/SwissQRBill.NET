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
        private readonly int resolution;
        private readonly float coordinateScale;
        private Bitmap bitmap;
        private Graphics graphics;
        private List<PointF> pathPoints;
        private List<byte> pathTypes;
        private FontFamily fontFamily;

        /// <summary>
        /// Initializes a new instance using the specified resolution.
        /// </summary>
        /// <remarks>
        /// It is recommended to use at least 144 dpi for a readable result.
        /// </remarks>
        /// <param name="resolution">resolution (in pixels per inch)</param>
        public PNGCanvas(int resolution)
        {
            this.resolution = resolution;
            coordinateScale = (float)(resolution / 25.4);
        }

        public override void SetupPage(double width, double height, string fontFamilyList)
        {
            // setup font metrics
            SetupFontMetrics(fontFamilyList);
            fontFamily = new FontFamily(fontMetrics.FirstFontFamily);

            // create image
            int w = (int)(width * coordinateScale + 0.5);
            int h = (int)(height * coordinateScale + 0.5);
            bitmap = new Bitmap(w, h);
            bitmap.SetResolution(resolution, resolution);

            // create graphics context
            graphics = Graphics.FromImage(bitmap);

            // clear background
            graphics.FillRectangle(Brushes.White, 0, 0, w, h);

            // enable high quality output
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // initialize transformation
            SetTransformation(0, 0, 0, 1, 1);
        }

        public override byte[] GetResult()
        {
            graphics.Dispose();
            graphics = null;

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        protected void Close()
        {
            if (graphics != null)
            {
                graphics.Dispose();
                graphics = null;
            }
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            if (fontFamily != null)
            {
                fontFamily.Dispose();
                fontFamily = null;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            // Our coordinate system extends from the bottom upwards. .NET's system
            // extends from the top downwards. So Y coordinates need to be treated specially.
            translateX *= coordinateScale;
            translateY *= coordinateScale;

            Matrix matrix = new Matrix();
            matrix.Translate((float)translateX, bitmap.Height - (float)translateY);
            if (rotate != 0)
            {
                matrix.Rotate((float)(-rotate / Math.PI * 180));
            }

            if (scaleX != 1 || scaleY != 1)
            {
                matrix.Scale((float)scaleX, (float)scaleY);
            }

            graphics.Transform = matrix;
        }

        public override void StartPath()
        {
            pathPoints = new List<PointF>();
            pathTypes = new List<byte>();
        }

        public override void CloseSubpath()
        {
            int lastIndex = pathTypes.Count - 1;
            byte pathType = pathTypes[lastIndex];
            pathType |= (byte)PathPointType.CloseSubpath;
            pathTypes[lastIndex] = pathType;
        }

        public override void MoveTo(double x, double y)
        {
            x *= coordinateScale;
            y *= -coordinateScale;

            pathPoints.Add(new PointF((float)x, (float)y));
            pathTypes.Add((byte)PathPointType.Start);
        }

        public override void LineTo(double x, double y)
        {
            x *= coordinateScale;
            y *= -coordinateScale;

            pathPoints.Add(new PointF((float)x, (float)y));
            pathTypes.Add((byte)PathPointType.Line);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= coordinateScale;
            y *= -coordinateScale;
            width *= coordinateScale;
            height *= -coordinateScale;

            pathPoints.Add(new PointF((float)x, (float)y));
            pathTypes.Add((byte)PathPointType.Start);
            pathPoints.Add(new PointF((float)(x + width), (float)y));
            pathTypes.Add((byte)PathPointType.Line);
            pathPoints.Add(new PointF((float)(x + width), (float)(y + height)));
            pathTypes.Add((byte)PathPointType.Line);
            pathPoints.Add(new PointF((float)x, (float)(y + height)));
            pathTypes.Add((byte)PathPointType.Line);
            pathPoints.Add(new PointF((float)x, (float)y));
            pathTypes.Add((byte)(PathPointType.Line | PathPointType.CloseSubpath));
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            x1 *= coordinateScale;
            y1 *= -coordinateScale;
            x2 *= coordinateScale;
            y2 *= -coordinateScale;
            x *= coordinateScale;
            y *= -coordinateScale;

            pathPoints.Add(new PointF((float)x1, (float)y1));
            pathTypes.Add((byte)PathPointType.Bezier);
            pathPoints.Add(new PointF((float)x2, (float)y2));
            pathTypes.Add((byte)PathPointType.Bezier);
            pathPoints.Add(new PointF((float)x, (float)y));
            pathTypes.Add((byte)PathPointType.Bezier);
        }

        public override void FillPath(int color)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color - 16777216)))
            using (GraphicsPath path = new GraphicsPath(pathPoints.ToArray(), pathTypes.ToArray(), FillMode.Winding))
            {
                graphics.FillPath(brush, path);
            }
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            using (Pen pen = new Pen(Color.FromArgb(color - 16777216), (float)strokeWidth))
            using (GraphicsPath path = new GraphicsPath(pathPoints.ToArray(), pathTypes.ToArray()))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            FontStyle style = isBold ? FontStyle.Bold : FontStyle.Regular;
            using (Font font = new Font(fontFamily, fontSize, style, GraphicsUnit.Point))
            {
                float ascent = fontFamily.GetCellAscent(style) / 2048.0f * fontSize / 72 * resolution;
                x *= coordinateScale;
                y *= -coordinateScale;
                y -= ascent;

                graphics.DrawString(text, font, Brushes.Black, (float)x, (float)y, StringFormat.GenericTypographic);
            }
        }
    }
}
