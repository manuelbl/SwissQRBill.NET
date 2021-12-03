//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Codecrete.SwissQRBill.Examples.Wpf
{
    /// <summary>
    /// Canvas for Windows Presentation Foundation
    /// <para>
    /// Either a <see cref="DrawingImage"/> can be created, which can be used
    /// as a source for an image control, or the canvas can be used to draw
    /// to a <see cref="DrawingContext"/>.
    /// </para>
    /// </summary>
    internal class QrBillImage : AbstractCanvas
    {
        private const double scale = 96.0 / 25.4;
        private const double fontScale = 25.4 / 72.0;
        private const string fontFamilyName = "Arial";
        private static readonly FontFamily fontFamily = new FontFamily(fontFamilyName);

        private DrawingContext _context;
        private PathGeometry _geometry;
        private PathFigure _figure;

        /// <summary>
        /// Creates an image for the specified QR bill.
        /// <para>
        /// The image will only comprise the QR bill itself and have a size of 210 by 105 mm.
        /// </para>
        /// <para>
        /// If the QR bill contains invalid data, the resulting image will be empty
        /// (except for possibly the background).
        /// </para>
        /// </summary>
        /// <param name="bill">QR bill</param>
        /// <param name="transparent">indicates if the images should have a transparent or white background</param>
        /// <returns></returns>
        public static DrawingImage CreateImage(Bill bill)
        {
            var bounds = new Rect(0, 0, 210 * scale, 105 * scale);

            DrawingGroup group = new DrawingGroup();
            using (DrawingContext dc = group.Open())
            {
                // draw white background
                dc.DrawRectangle(Brushes.White, null, bounds);

                using var canvas = new QrBillImage(dc, 105);
                var savedOutputSize = bill.Format.OutputSize;
                bill.Format.OutputSize = OutputSize.QrBillOnly;
                try
                {
                    QRBill.Draw(bill, canvas);
                }
                catch (QRBillValidationException)
                {
                    // ignore
                }
                bill.Format.OutputSize = savedOutputSize;
            }

            var image = new DrawingImage(group);
            image.Freeze();
            return image;
        }

        /// <summary>
        /// Creates a canvas for the specified drawing context.
        /// </summary>
        /// <param name="dc">drawing context</param>
        /// <param name="height">The height of the canvas, in mm</param>
        public QrBillImage(DrawingContext dc, double height)
        {
            SetupFontMetrics(fontFamilyName);
            _context = dc;

            // Basic transformation so mm can be used instead of dip,
            // and to put origin in bottom left corner
            Matrix matrix = new Matrix();
            matrix.Translate(0, height);
            matrix.Scale(scale, scale);
            dc.PushTransform(new MatrixTransform(matrix));

            // Dummy inner transformation
            dc.PushTransform(new MatrixTransform(new Matrix()));
        }

        protected void Close()
        {
            _context = null;
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            Matrix matrix = new Matrix();

            matrix.Translate(translateX, -translateY);

            if (rotate != 0)
            {
                matrix.RotatePrepend(-rotate / Math.PI * 180);
            }

            if (scaleX != 1 || scaleY != 1)
            {
                matrix.ScalePrepend(scaleX, scaleY);
            }

            _context.Pop();
            _context.PushTransform(new MatrixTransform(matrix));
        }

        public override void StartPath()
        {
            _geometry = new PathGeometry();
        }

        public override void CloseSubpath()
        {
            _figure.IsClosed = true;
        }

        public override void MoveTo(double x, double y)
        {
            _figure = new PathFigure();
            _geometry.Figures.Add(_figure);
            _figure.StartPoint = new Point(x, -y);
        }

        public override void LineTo(double x, double y)
        {
            _figure.Segments.Add(new LineSegment(new Point(x, -y), false));
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            _figure.Segments.Add(new BezierSegment(new Point(x1, -y1), new Point(x2, -y2), new Point(x, -y), false));
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            _figure = new PathFigure();
            _geometry.Figures.Add(_figure);
            _figure.StartPoint = new Point(x, -y);
            _figure.Segments.Add(new LineSegment(new Point(x + width, -y), false));
            _figure.Segments.Add(new LineSegment(new Point(x + width, -y - height), false));
            _figure.Segments.Add(new LineSegment(new Point(x, -y - height), false));
            _figure.IsClosed = true;
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            StrokePath(strokeWidth, color, LineStyle.Solid);
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle)
        {
            // all segements in all figures need to be enabled for stroking
            foreach (var figure in _geometry.Figures)
            {
                foreach (var segment in figure.Segments)
                    segment.IsStroked = true;
            }

            Pen pen = new Pen(BrushFromRgb(color), strokeWidth * fontScale);

            switch (lineStyle)
            {
                case LineStyle.Dashed:
                    pen.DashStyle = new DashStyle(new double[] { 4 }, 0);
                    break;

                case LineStyle.Dotted:
                    pen.DashStyle = new DashStyle(new double[] { 0, 3 }, 0);
                    pen.DashCap = PenLineCap.Round;
                    break;

                default:
                    break;
            }

            _context.DrawGeometry(null, pen, _geometry);
            _geometry = null;
            _figure = null;
        }

        public override void FillPath(int color)
        {
            _figure.IsClosed = true;
            _geometry.FillRule = FillRule.Nonzero;
            _context.DrawGeometry(BrushFromRgb(color), null, _geometry);
            _geometry = null;
            _figure = null;
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            double ascent = fontFamily.Baseline * fontSize * fontScale;
            var typeface = new Typeface(fontFamily, FontStyles.Normal, isBold ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal);
            var formattedText = new FormattedText(text, CultureInfo.GetCultureInfo("de-fr"), FlowDirection.LeftToRight,
                typeface, fontSize * fontScale, Brushes.Black, 1);
            _context.DrawText(formattedText, new Point(x, -y - ascent));
        }

        private static Brush BrushFromRgb(int color)
        {
            if (color == 0)
                return Brushes.Black;
            if (color == 0xffffff)
                return Brushes.White;
            return new SolidColorBrush(Color.FromRgb((byte)(color >> 16), (byte)((color >> 8) & 0xff), (byte)(color & 0xff)));
        }
    }
}
