//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Abstract base class for simplified implementation of classes implementing <see cref="ICanvas"/>.
    /// <para>
    /// The class mainly implements text measurement and a helper for multi-line text.
    /// </para>
    /// </summary>
    public abstract class AbstractCanvas : ICanvas
    {
        /// <summary>
        /// Conversion factor from mm to point
        /// </summary>
        protected static readonly double MmToPt = 72 / 25.4;

        /// <summary>
        /// Font metrics used by this canvas.
        /// </summary>
        protected FontMetrics FontMetrics;

        /// <summary>
        /// Sets up the font metrics using the first font family in the specified list.
        /// </summary>
        /// <param name="fontFamilyList">The font family list.</param>
        protected void SetupFontMetrics(string fontFamilyList)
        {
            FontMetrics = new FontMetrics(fontFamilyList);
        }

        /// <inheritdoc />
        public virtual void PutTextLines(string[] lines, double x, double y, int fontSize, double leading)
        {
            foreach (var line in lines)
            {
                PutText(line, x, y, fontSize, false);
                y -= FontMetrics.LineHeight(fontSize) + leading;
            }
        }

        /// <inheritdoc />
        public double Ascender(int fontSize)
        {
            return FontMetrics.Ascender(fontSize);
        }

        /// <inheritdoc />
        public double Descender(int fontSize)
        {
            return FontMetrics.Descender(fontSize);
        }

        /// <inheritdoc />
        public double LineHeight(int fontSize)
        {
            return FontMetrics.LineHeight(fontSize);
        }

        /// <inheritdoc />
        public double TextWidth(string text, int fontSize, bool isBold)
        {
            return FontMetrics.TextWidth(text, fontSize, isBold);
        }

        /// <inheritdoc />
        public string[] SplitLines(string text, double maxLength, int fontSize)
        {
            return FontMetrics.SplitLines(text, maxLength, fontSize);
        }

        /// <inheritdoc />
        public abstract void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY);
        /// <inheritdoc />
        public abstract void PutText(string text, double x, double y, int fontSize, bool isBold);
        /// <inheritdoc />
        public abstract void StartPath();
        /// <inheritdoc />
        public abstract void MoveTo(double x, double y);
        /// <inheritdoc />
        public abstract void LineTo(double x, double y);
        /// <inheritdoc />
        public abstract void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y);
        /// <inheritdoc />
        public abstract void AddRectangle(double x, double y, double width, double height);
        /// <inheritdoc />
        public abstract void CloseSubpath();
        /// <inheritdoc />
        public abstract void FillPath(int color, bool smoothing = true);
        /// <inheritdoc />
        public abstract void StrokePath(double strokeWidth, int color, LineStyle lineStyle = LineStyle.Solid, bool smoothing = true);

        /// <inheritdoc />
        public abstract byte[] ToByteArray();

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees and releases resources.
        /// </summary>
        /// <param name="disposing">indicates whether the method is called from a <c>Dispose</c> method (<c>true</c>) or from a finalizer (<c>false</c>).</param>
        protected abstract void Dispose(bool disposing);
    }
}
