//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    public abstract class AbstractCanvas : ICanvas
    {
        protected static readonly double MmToPt = 72 / 25.4;

        protected FontMetrics fontMetrics;

        public abstract byte[] Result { get; }

        protected void SetupFontMetrics(string fontFamilyList)
        {
            fontMetrics = new FontMetrics(fontFamilyList);
        }

        public void PutTextLines(string[] lines, double x, double y, int fontSize, double leading)
        {
            foreach (string line in lines)
            {
                PutText(line, x, y, fontSize, false);
                y -= fontMetrics.LineHeight(fontSize) + leading;
            }
        }

        public double Ascender(int fontSize)
        {
            return fontMetrics.Ascender(fontSize);
        }

        public double Descender(int fontSize)
        {
            return fontMetrics.Descender(fontSize);
        }

        public double LineHeight(int fontSize)
        {
            return fontMetrics.LineHeight(fontSize);
        }

        public double TextWidth(string text, int fontSize, bool isBold)
        {
            return fontMetrics.TextWidth(text, fontSize, isBold);
        }

        public string[] SplitLines(string text, double maxLength, int fontSize)
        {
            return fontMetrics.SplitLines(text, maxLength, fontSize);
        }

        public abstract void SetupPage(double width, double height, string fontFamily);
        public abstract void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY);
        public abstract void PutText(string text, double x, double y, int fontSize, bool isBold);
        public abstract void StartPath();
        public abstract void MoveTo(double x, double y);
        public abstract void LineTo(double x, double y);
        public abstract void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y);
        public abstract void AddRectangle(double x, double y, double width, double height);
        public abstract void CloseSubpath();
        public abstract void FillPath(int color);
        public abstract void StrokePath(double strokeWidth, int color);
        public abstract void Dispose();
    }
}
