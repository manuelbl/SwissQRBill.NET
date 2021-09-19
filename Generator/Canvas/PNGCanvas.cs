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
using System.Text.RegularExpressions;

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
        private readonly float _fontScale;
        private Bitmap _bitmap;
        private Graphics _graphics;
        private List<PointF> _pathPoints;
        private List<byte> _pathTypes;
        private FontFamily _fontFamily;

        /// <summary>
        /// Initializes a new instance of a PNG canvas with the given size, resolution and font family.
        /// <para>
        /// The QR bill will be drawn in the bottom left corner of the image.
        /// </para>
        /// <para>
        /// It is recommended to use at least 144 dpi for a readable result.
        /// </para>
        /// </summary>
        /// <param name="width">The image width, in mm.</param>
        /// <param name="height">The image height, in mm.</param>
        /// <param name="resolution">The resolution of the image to generate (in pixels per inch).</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public PNGCanvas(double width, double height, int resolution, string fontFamilyList)
        {
            // setup font metrics
            SetupFontMetrics(FindInstalledFontFamily(fontFamilyList));
            _fontFamily = new FontFamily(FontMetrics.FirstFontFamily);

            // create image
            _resolution = resolution;
            _coordinateScale = (float)(resolution / 25.4);
            _fontScale = (float)(resolution / 72.0);
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
            Matrix matrix = new Matrix();
            matrix.Translate(0, _bitmap.Height);
            _graphics.Transform = matrix;
        }

        /// <summary>
        /// Finds the first font family from the specified list that is installed and not replaced with an alternative font.
        /// </summary>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        /// <returns>font family name (if font is installed), or unchanged font family list (if none of the fonts is found)</returns>
        private static string FindInstalledFontFamily(string fontFamilyList)
        {
            foreach (string family in SplitCommaSeparated(fontFamilyList))
            {
                string trimmedFamily = family.Trim();
                try
                {
                    using (FontFamily fontFamily = new FontFamily(trimmedFamily))
                    {
                        return trimmedFamily;
                    }
                }
                catch (ArgumentException)
                {
                    // font was not found; try next one
                }
            }

            return fontFamilyList;
        }

        private static readonly Regex quotedSplitter = new Regex("(?:^|,)(\"[^\"]*\"|[^,]*)", RegexOptions.Compiled);

        /// <summary>
        /// Splits the comma separated list into its components.
        /// <para>
        /// A component may use double quotes (similar to CSV formats).
        /// </para>
        /// </summary>
        /// <param name="input">comma separated list</param>
        /// <returns>list of components</returns>
        private static IEnumerable<string> SplitCommaSeparated(string input)
        {
            foreach (Match match in quotedSplitter.Matches(input))
            {
                string component = match.Groups[1].Value;
                if (component[0] == '"' && component[component.Length - 1] == '"')
                {
                    component = component.Substring(1, component.Length - 2);
                }
                yield return component;
            }
        }

        /// <summary>
        /// Gets the resulting graphics encoded as a PNG image in a byte array.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <returns>The byte array containing the PNG image</returns>
        public override byte[] ToByteArray()
        {
            _graphics.Dispose();
            _graphics = null;

            MemoryStream stream = new MemoryStream();
            _bitmap.Save(stream, ImageFormat.Png);
            Close();
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the resulting graphics as a PNG image to the specified stream.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            _graphics.Dispose();
            _graphics = null;
            _bitmap.Save(stream, ImageFormat.Png);
            Close();
        }

        /// <summary>
        /// Writes the resulting graphics as a PNG image to the specified file path.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="path">The path (file name) to write to.</param>
        public void SaveAs(string path)
        {
            _graphics.Dispose();
            _graphics = null;
            _bitmap.Save(path, ImageFormat.Png);
            Close();
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

        public override void FillPath(int color, bool smoothing)
        {
            // turn off antialiasing if smoothing is not desired
            if (!smoothing)
            {
                _graphics.SmoothingMode = SmoothingMode.None;
                _graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            }

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(color - 16777216)))
            using (GraphicsPath path = new GraphicsPath(_pathPoints.ToArray(), _pathTypes.ToArray(), FillMode.Winding))
            {
                _graphics.FillPath(brush, path);
            }

            // turn antialiasing on
            if (!smoothing)
            {
                _graphics.SmoothingMode = SmoothingMode.HighQuality;
                _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            }
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle, bool smoothing)
        {
            // turn off antialiasing if smoothing is not desired
            if (!smoothing && lineStyle != LineStyle.Dotted)
            {
                _graphics.SmoothingMode = SmoothingMode.None;
                _graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            }

            float width = (float)strokeWidth * _fontScale;

            using (Pen pen = new Pen(Color.FromArgb(color - 16777216), width))
            {
                switch (lineStyle)
                {
                    case LineStyle.Dashed:
                        pen.DashPattern = new float[] { 4, 4 };
                        break;
                    case LineStyle.Dotted:
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        pen.DashCap = DashCap.Round;
                        pen.DashPattern = new float[] { 0.01f, 2 };
                        break;
                    default:
                        break;
                }

                using (GraphicsPath path = new GraphicsPath(_pathPoints.ToArray(), _pathTypes.ToArray()))
                {
                    _graphics.DrawPath(pen, path);
                }
            }

            // turn antialiasing on
            if (!smoothing && lineStyle != LineStyle.Dotted)
            {
                _graphics.SmoothingMode = SmoothingMode.HighQuality;
                _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            }
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            FontStyle style = isBold ? FontStyle.Bold : FontStyle.Regular;
            using (Font font = new Font(_fontFamily, fontSize * _fontScale, style, GraphicsUnit.Pixel))
            {
                float ascent = _fontFamily.GetCellAscent(style) / 2048.0f * fontSize * _fontScale;
                x *= _coordinateScale;
                y *= -_coordinateScale;
                y -= ascent;

                _graphics.DrawString(text, font, Brushes.Black, (float)x, (float)y, StringFormat.GenericTypographic);
            }
        }
    }
}
