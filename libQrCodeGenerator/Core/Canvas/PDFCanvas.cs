﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using libQrCodeGenerator.SwissQRBill.Generator.PDF;
using System.IO;

namespace libQrCodeGenerator.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Canvas for generating PDF files.
    /// <para>
    /// The PDF generator currently only supports the Helvetica font.
    /// </para>
    /// </summary>
    public class PDFCanvas : AbstractCanvas
    {
        private const float ColorScale = 1f / 255;
        private Document _document;
        private ContentStream _contentStream;
        private int _lastStrokingColor;
        private int _lastNonStrokingColor;
        private double _lastLineWidth = 1;
        private bool _hasSavedGraphicsState;
        private Font _lastFont;
        private float _lastFontSize;
        private LineStyle _lastLineStyle;

        /// <summary>
        /// Initializes a new instance of the PDF canvas with the specified page size.
        /// <para>
        /// A PDF with a single page of the specified size will be created. The QR bill
        /// will be drawn in the bottom left corner of the page.
        /// </para>
        /// </summary>
        /// <param name="width">The page width, in mm.</param>
        /// <param name="height">The page height, in mm.</param>
        public PDFCanvas(double width, double height)
        {
            SetupFontMetrics("Helvetica");
            _document = new Document("Swiss QR Bill");
            Page page = _document.CreatePage((float)(width * MmToPt), (float)(height * MmToPt));
            _contentStream = page.Contents;
        }

        /// <inheritdoc />
        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            translateX *= MmToPt;
            translateY *= MmToPt;

            if (_hasSavedGraphicsState)
            {
                _contentStream.RestoreGraphicsState();
                _lastStrokingColor = 0;
                _lastNonStrokingColor = 0;
                _lastLineWidth = 1;
            }

            _lastFont = null;
            _lastFontSize = 0;

            _contentStream.SaveGraphicsState();
            _hasSavedGraphicsState = true;

            TransformationMatrix matrix = new TransformationMatrix();
            matrix.Translate(translateX, translateY);
            matrix.Rotate(rotate);
            matrix.Scale(scaleX, scaleY);

            _contentStream.Transform(matrix);
        }

        private void SetFont(bool isBold, int fontSize)
        {
            Font font = isBold ? Font.HelveticaBold : Font.Helvetica;
            if (font == _lastFont && fontSize == _lastFontSize)
            {
                return;
            }

            _contentStream.SetFont(font, fontSize);
            _lastFont = font;
            _lastFontSize = fontSize;
        }

        /// <inheritdoc />
        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            x *= MmToPt;
            y *= MmToPt;

            SetFont(isBold, fontSize);

            _contentStream.BeginText();
            _contentStream.NewLineAtOffset((float)x, (float)y);
            _contentStream.ShowText(text);
            _contentStream.EndText();
        }

        /// <inheritdoc />
        public override void PutTextLines(string[] lines, double x, double y, int fontSize, double leading)
        {
            x *= MmToPt;
            y *= MmToPt;
            float lineHeight = (float)((FontMetrics.LineHeight(fontSize) + leading) * MmToPt);

            SetFont(false, fontSize);

            _contentStream.BeginText();
            _contentStream.NewLineAtOffset((float)x, (float)y);
            bool isFirstLine = true;
            foreach (string line in lines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                }
                else
                {
                    _contentStream.NewLineAtOffset(0, -lineHeight);
                }
                _contentStream.ShowText(line);
            }
            _contentStream.EndText();
        }

        /// <inheritdoc />
        public override void StartPath()
        {
            // path is start implicitly
        }

        /// <inheritdoc />
        public override void MoveTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            _contentStream.MoveTo((float)x, (float)y);
        }

        /// <inheritdoc />
        public override void LineTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            _contentStream.LineTo((float)x, (float)y);
        }

        /// <inheritdoc />
        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            x1 *= MmToPt;
            y1 *= MmToPt;
            x2 *= MmToPt;
            y2 *= MmToPt;
            x *= MmToPt;
            y *= MmToPt;
            _contentStream.CurveTo((float)x1, (float)y1, (float)x2, (float)y2, (float)x, (float)y);
        }

        /// <inheritdoc />
        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= MmToPt;
            y *= MmToPt;
            width *= MmToPt;
            height *= MmToPt;
            _contentStream.AddRect((float)x, (float)y, (float)width, (float)height);
        }

        /// <inheritdoc />
        public override void CloseSubpath()
        {
            _contentStream.ClosePath();
        }

        /// <inheritdoc />
        public override void FillPath(int color, bool smoothing)
        {
            if (color != _lastNonStrokingColor)
            {
                _lastNonStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 0) & 0xff);
                _contentStream.SetNonStrokingColor(r, g, b);
            }
            _contentStream.Fill();
        }

        /// <inheritdoc />
        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle, bool smoothing)
        {
            if (color != _lastStrokingColor)
            {
                _lastStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 0) & 0xff);
                _contentStream.SetStrokingColor(r, g, b);
            }
            if (lineStyle != _lastLineStyle
                || (lineStyle != LineStyle.Solid && strokeWidth != _lastLineWidth))
            {
                _lastLineStyle = lineStyle;
                float[] pattern;
                switch (lineStyle)
                {
                    case LineStyle.Dashed:
                        pattern = new float[] { 4 * (float)strokeWidth };
                        break;
                    case LineStyle.Dotted:
                        pattern = new float[] { 0, 3 * (float)strokeWidth };
                        break;
                    default:
                        pattern = new float[] { };
                        break;
                }
                _contentStream.SetLineCapStyle(lineStyle == LineStyle.Dotted ? 1 : 0);
                _contentStream.SetLineDashPattern(pattern, 0);
            }
            if (strokeWidth != _lastLineWidth)
            {
                _lastLineWidth = strokeWidth;
                _contentStream.SetLineWidth((float)(strokeWidth));
            }
            _contentStream.Stroke();
        }

        /// <summary>
        /// Gets the resulting graphics as a PDF document in a byte array.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <returns>The byte array containing the PDF document</returns>
        public override byte[] ToByteArray()
        {
            MemoryStream buffer = new MemoryStream();
            _document.Save(buffer);
            Close();
            return buffer.ToArray();
        }

        /// <summary>
        /// Writes the resulting graphics as a PDF document to the specified stream.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            _document.Save(stream);
            Close();
        }

        /// <summary>
        /// Writes the resulting graphics as a PDF document to the specified file path.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="path">The path (file name) to write to.</param>
        public void SaveAs(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                _document.Save(fs);
            }
            Close();
        }

        /// <inheritdoc />
        protected void Close()
        {
            _contentStream = null;
            _document = null;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            Close();
        }
    }
}
