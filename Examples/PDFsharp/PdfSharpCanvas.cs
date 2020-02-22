//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Robert Hegner
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PDFsharp
{
    class PdfSharpCanvas : AbstractCanvas
    {

        private readonly XGraphics XGraphics;
        private readonly string FontFamily;

        private GraphicsPath CurrentPath;

        public PdfSharpCanvas(PdfPage page, string fontFamily)
        {
            FontFamily = fontFamily;
            SetupFontMetrics(FontFamily);

            XGraphics = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append, XGraphicsUnit.Millimeter, XPageDirection.Downwards);

            // The origin of an XGraphics canvas is in the top left corner, with the y axis pointing downwards.
            // This transform mirrors the y axis. Unfortunately this also mirrors text output (see workaround in PutText).
            // XGraphics.FromPdfPage can be called with XPageDirection.Upwards, which is marked as obsolete because it is not implemented properly
            // it suffers from the same problem as our matrix below.
            XGraphics.MultiplyTransform(new XMatrix(1.0, 0.0, 0.0, -1.0, 0.0, XGraphics.PageSize.Height));

            // Save the current state so that we can always revert any transformations we apply.
            XGraphics.Save();
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            // Revert to original state, and immediately put this state on the stack again.
            XGraphics.Restore();
            XGraphics.Save();

            XGraphics.TranslateTransform(translateX, translateY);
            if (rotate != 0.0)
                XGraphics.RotateTransform(rotate / System.Math.PI * 180.0);
            if (scaleX != 1.0 || scaleY != 1.0)
                XGraphics.ScaleTransform(scaleX, scaleY);           
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            var font = new XFont(FontFamily, XUnit.FromPoint(fontSize).Millimeter, isBold ? XFontStyle.Bold : XFontStyle.Regular);

            // With our transformation to mirror the y axis, text printed using DrawString also gets mirrored.
            // As a workaround we move our current transformation to the x/y position and then mirror the y axis again, and then restore the previous transformation.
            XGraphics.Save();
            XGraphics.TranslateTransform(x, y);
            XGraphics.MultiplyTransform(new XMatrix(1.0, 0.0, 0.0, -1.0, 0.0, 0.0));
            XGraphics.DrawString(text, font, XBrushes.Black, 0.0, 0.0);
            XGraphics.Restore();
        }

        public override void StartPath()
        {
            CurrentPath = new GraphicsPath();
        }

        public override void MoveTo(double x, double y)
        {
            CurrentPath.MoveTo(new XPoint(x, y));
        }

        public override void LineTo(double x, double y)
        {
            CurrentPath.LineTo(new XPoint(x, y));
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            var controlPoint1 = new XPoint(x1, y1);
            var controlPoint2 = new XPoint(x2, y2);
            var point = new XPoint(x, y);
            CurrentPath.CubicCurveTo(controlPoint1, controlPoint2, point);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            CurrentPath.AddRectangle(new XPoint(x, y), width, height);
        }

        public override void CloseSubpath()
        {
            CurrentPath.CloseSubpath();
        }

        public override void FillPath(int color)
        {
            CurrentPath.Fill(XGraphics, new XSolidBrush(XColor.FromArgb(color)));
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            var pen = new XPen(XColor.FromArgb(color), XUnit.FromPoint(strokeWidth).Millimeter);
            CurrentPath.Stroke(XGraphics, pen);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                XGraphics.Dispose();
        }
    }
}
