//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Codecrete.SwissQRBill.Examples.Wpf
{
    /// <summary>
    /// Canvas for draing to a System.Drawing graphics surface.
    /// </summary>
    public class SystemDrawingCanvas : AbstractCanvas
    {
        private readonly float _xOffset;
        private readonly float _yOffset;
        private readonly float _coordinateScale;
        private readonly float _fontScale;
        private readonly Graphics _graphics;
        private readonly FontFamily _fontFamily;
        private List<PointF> _pathPoints;
        private List<byte> _pathTypes;

        /// <summary>
        /// Creates a new canvas instance.
        /// <para>
        /// The offset is specified in the drawing surface coordinate system. Positive y coordinates point downwards.
        /// </para>
        /// <para>
        /// The drawing surface is not owned or disposed.
        /// </para>
        /// </summary>
        /// <param name="graphics">GDI+ drawing surface for rendering the QR bill.</param>
        /// <param name="xOffset">The x-offset to the bottom left corner of the drawing area, in the drawing surface coordinate system.</param>
        /// <param name="yOffset">The y-offset to the bottom left corner of the drawing area, in the drawing surface coordinate system.</param>
        /// <param name="scale">The conversion factor from mm to the drawing surface coordinate system.</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public SystemDrawingCanvas(Graphics graphics, float xOffset, float yOffset, float scale, string fontFamilyList)
        {
            // setup font metrics
            SetupFontMetrics(fontFamilyList);
            _fontFamily = new FontFamily(FontMetrics.FirstFontFamily);

            _xOffset = xOffset;
            _yOffset = yOffset;
            _coordinateScale = scale;
            _fontScale = (float)(scale * 25.4 / 72.0);
            _graphics = graphics;

            // enable high quality output
            _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
            _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // initialize transformation
            Matrix matrix = new Matrix();
            matrix.Translate(xOffset, yOffset);
            _graphics.Transform = matrix;
        }

        protected void Close()
        {
            if (_fontFamily != null)
            {
                _fontFamily.Dispose();
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
            matrix.Translate(_xOffset + (float)translateX, _yOffset - (float)translateY);
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
            int lastIndex = _pathTypes!.Count - 1;
            byte pathType = _pathTypes[lastIndex];
            pathType |= (byte)PathPointType.CloseSubpath;
            _pathTypes[lastIndex] = pathType;
        }

        public override void MoveTo(double x, double y)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _pathPoints!.Add(new PointF((float)x, (float)y));
            _pathTypes!.Add((byte)PathPointType.Start);
        }

        public override void LineTo(double x, double y)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _pathPoints!.Add(new PointF((float)x, (float)y));
            _pathTypes!.Add((byte)PathPointType.Line);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;
            width *= _coordinateScale;
            height *= -_coordinateScale;

            _pathPoints!.Add(new PointF((float)x, (float)y));
            _pathTypes!.Add((byte)PathPointType.Start);
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

            _pathPoints!.Add(new PointF((float)x1, (float)y1));
            _pathTypes!.Add((byte)PathPointType.Bezier);
            _pathPoints.Add(new PointF((float)x2, (float)y2));
            _pathTypes.Add((byte)PathPointType.Bezier);
            _pathPoints.Add(new PointF((float)x, (float)y));
            _pathTypes.Add((byte)PathPointType.Bezier);
        }

        public override void FillPath(int color)
        {
            using SolidBrush brush = new SolidBrush(Color.FromArgb(color - 16777216));
            using GraphicsPath path = new GraphicsPath(_pathPoints!.ToArray(), _pathTypes!.ToArray(), FillMode.Winding);
            _graphics.FillPath(brush, path);
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            StrokePath(strokeWidth, color, LineStyle.Solid);
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle)
        {
            float width = (float)strokeWidth * _fontScale;

            using Pen pen = new Pen(Color.FromArgb(color - 16777216), width);
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

            using GraphicsPath path = new GraphicsPath(_pathPoints!.ToArray(), _pathTypes!.ToArray());
            _graphics.DrawPath(pen, path);
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            FontStyle style = isBold ? FontStyle.Bold : FontStyle.Regular;
            using Font font = new Font(_fontFamily, fontSize * _fontScale, style, GraphicsUnit.Pixel);
            float ascent = _fontFamily.GetCellAscent(style) / 2048.0f * fontSize * _fontScale;
            x *= _coordinateScale;
            y *= -_coordinateScale;
            y -= ascent;

            _graphics.DrawString(text, font, Brushes.Black, (float)x, (float)y, StringFormat.GenericTypographic);
        }
    }
}