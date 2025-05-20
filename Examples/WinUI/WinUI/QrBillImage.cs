//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2022 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Generator.Canvas;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Microsoft.UI.Text;
using System;
using System.Numerics;
using Windows.UI;

namespace Codecrete.SwissQRBill.Examples.WinUI
{
    /// <summary>
    /// Canvas for Win2D
    /// <para>
    /// Draws a Swiss QR bill into a <see cref="CanvasDrawingSession"/>.
    /// </para>
    /// </summary>
    internal partial class QrBillImage : AbstractCanvas
    {
        private const double ptToMm = 25.4 / 72.0;
        private const string fontFamilyName = "Arial";

        private CanvasDrawingSession? _session;
        private Matrix3x2 _baseTransform;
        private CanvasPathBuilder? _pathBuilder;
        private bool _isInFigure = false;

        /// <summary>
        /// Draws the specified QR bill into the Win2D drawing session.
        /// <para>
        /// The image will only comprise the QR bill itself and have a size of 210 by 105 mm.
        /// </para>
        /// <para>
        /// If the QR bill contains invalid data, the resulting image will be empty.
        /// </para>
        /// </summary>
        /// <param name="bill">QR bill</param>
        /// <param name="session">drawing session</param>
        /// <param name="xOffset">offset at the left of the QR bill, in the session's coordinate system</param>
        /// <param name="yOffset">offset at the top of the QR bill, in the session's coordinate system</param>
        /// <param name="scale">scaling factor from mm to the session's resolution</param>
        /// <returns></returns>
        public static void DrawQrBill(Bill bill, CanvasDrawingSession session, double xOffset, double yOffset, double scale)
        {
            session.FillRectangle((float)xOffset, (float)yOffset, (float)(210 * scale), (float)(105 * scale), Colors.White);

            using var canvas = new QrBillImage(session, xOffset, yOffset + 105 * scale, scale);
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

        /// <summary>
        /// Draws the specified QR bill into the Win2D drawing session.
        /// <para>
        /// The image will assume a full A4 page.
        /// </para>
        /// <para>
        /// If the QR bill contains invalid data, the resulting image will be empty.
        /// </para>
        /// </summary>
        /// <param name="bill">QR bill</param>
        /// <param name="session">drawing session</param>
        /// <param name="xOffset">offset at the left of the page, in the session's coordinate system</param>
        /// <param name="yOffset">offset at the top of the page, in the session's coordinate system</param>
        /// <param name="scale">scaling factor from mm to the session's resolution</param>
        /// <returns></returns>
        public static void DrawQrBillPage(Bill bill, CanvasDrawingSession session, double xOffset, double yOffset, double scale)
        {
            using var canvas = new QrBillImage(session, xOffset, yOffset + 297 * scale, scale);
            var savedOutputSize = bill.Format.OutputSize;
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;
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

        /// <summary>
        /// Creates a canvas for the specified Win2D drawing session.
        /// </summary>
        /// <param name="session">drawing session</param>
        /// <param name="xOffset">offset at the left of the QR bill, in the session's coordinate system</param>
        /// <param name="yOffset">offset at the top of the QR bill, in the session's coordinate system</param>
        /// <param name="scale">scaling factor from mm to the session's resolution</param>
        public QrBillImage(CanvasDrawingSession session, double xOffset, double yOffset, double scale)
        {
            SetupFontMetrics(fontFamilyName);
            _session = session;

            // Basic transformation so mm can be used instead of dip,
            // and to put origin in bottom left corner
            _baseTransform = Matrix3x2.CreateTranslation((float)xOffset, (float)yOffset);
            _baseTransform = Matrix3x2.Multiply(Matrix3x2.CreateScale((float)scale, (float)scale), _baseTransform);
            _session.Transform = _baseTransform;
        }

        protected void Close()
        {
            if (_pathBuilder != null)
            {
                _pathBuilder.Dispose();
                _pathBuilder = null;
            }
            _session = null;
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            var matrix = Matrix3x2.Multiply(Matrix3x2.CreateTranslation((float)translateX, (float)-translateY), _baseTransform);

            if (rotate != 0)
            {
                matrix = Matrix3x2.Multiply(Matrix3x2.CreateRotation((float)-rotate), matrix);
            }

            if (scaleX != 1 || scaleY != 1)
            {
                matrix = Matrix3x2.Multiply(Matrix3x2.CreateScale((float)scaleX, (float)scaleY), matrix);
            }

            _session!.Transform = matrix;
        }

        public override void StartPath()
        {
            _pathBuilder = new CanvasPathBuilder(_session!.Device);
            _isInFigure = false;
        }

        public override void CloseSubpath()
        {
            _pathBuilder!.EndFigure(CanvasFigureLoop.Closed);
            _isInFigure = false;
        }

        public override void MoveTo(double x, double y)
        {
            if (_isInFigure)
                _pathBuilder!.EndFigure(CanvasFigureLoop.Open);
            _pathBuilder!.BeginFigure((float)x, (float)-y);
            _isInFigure = true;
        }

        public override void LineTo(double x, double y)
        {
            _pathBuilder!.AddLine((float)x, (float)-y);
            _isInFigure = true;
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            _pathBuilder!.AddCubicBezier(new Vector2((float)x1, (float)-y1), new Vector2((float)x2, (float)-y2), new Vector2((float)x, (float)-y));
            _isInFigure = true;
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            if (_isInFigure)
                _pathBuilder!.EndFigure(CanvasFigureLoop.Open);
            _pathBuilder!.BeginFigure((float)x, (float)-y);
            _pathBuilder.AddLine((float)(x + width), (float)-y);
            _pathBuilder.AddLine((float)(x + width), (float)(-y - height));
            _pathBuilder.AddLine((float)x, (float)(-y - height));
            _pathBuilder.EndFigure(CanvasFigureLoop.Closed);
            _isInFigure = false;
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle = LineStyle.Solid, bool smoothing = true)
        {
            if (_isInFigure)
                _pathBuilder!.EndFigure(CanvasFigureLoop.Open);

            using var strokeStyle = new CanvasStrokeStyle();

            switch (lineStyle)
            {
                case LineStyle.Dashed:
                    strokeStyle.CustomDashStyle = [4];
                    break;

                case LineStyle.Dotted:
                    strokeStyle.CustomDashStyle = [0, 3];
                    strokeStyle.DashCap = CanvasCapStyle.Round;
                    break;

                default:
                    strokeStyle.DashStyle = CanvasDashStyle.Solid;
                    break;
            }

            _session!.DrawGeometry(CanvasGeometry.CreatePath(_pathBuilder), ColorFromRgb(color), (float)(strokeWidth * ptToMm), strokeStyle);
            _pathBuilder!.Dispose();
            _pathBuilder = null;
        }

        public override void FillPath(int color, bool smoothing = true)
        {
            if (_isInFigure)
                _pathBuilder!.EndFigure(CanvasFigureLoop.Closed);
            _session!.FillGeometry(CanvasGeometry.CreatePath(_pathBuilder), ColorFromRgb(color));
            _pathBuilder!.Dispose();
            _pathBuilder = null;
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            using var textFormat = new CanvasTextFormat()
            {
                FontFamily = fontFamilyName,
                FontSize = (float)(fontSize * ptToMm),
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal
            };

            double ascent = 0.921630859375 * fontSize * ptToMm;
            _session!.DrawText(text, (float)x, (float)(-y - ascent), Colors.Black, textFormat);
        }

        private static Color ColorFromRgb(int color)
        {
            return ColorHelper.FromArgb(255, (byte)((color >> 16) & 0xff),
                (byte)((color >> 8) & 0xff), (byte)(color & 0xff));
        }

        public override byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }
    }
}