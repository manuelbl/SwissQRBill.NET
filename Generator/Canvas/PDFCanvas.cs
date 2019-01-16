//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.PDF;
using System;
using System.Drawing.Drawing2D;
using System.IO;

namespace Codecrete.SwissQRBill.Generator.Canvas
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

        public override void SetupPage(double width, double height, string fontFamily)
        {
            SetupFontMetrics("Helvetica");
            _document = new Document("Swiss QR Bill");
            Page page = _document.CreatePage((float)(width * MmToPt), (float)(height * MmToPt));
            _contentStream = page.Contents;
        }

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

            using (Matrix matrix = new Matrix())
            {
                matrix.Translate((float)translateX, (float)translateY);
                if (rotate != 0)
                {
                    matrix.Rotate((float)(rotate / Math.PI * 180));
                }

                if (scaleX != 1 || scaleY != 1)
                {
                    matrix.Scale((float)scaleX, (float)scaleY);
                }

                _contentStream.Transform(matrix);
            }
        }

        private void SetFont(bool isBold, int fontSize)
        {
            Font font = isBold ? Font.HelveticaBold : Font.Helvetica;
            if (font != _lastFont || fontSize != _lastFontSize)
            {
                _contentStream.SetFont(font, fontSize);
                _lastFont = font;
                _lastFontSize = fontSize;
            }
        }

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

        public override void StartPath()
        {
            // path is start implicitly
        }

        public override void MoveTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            _contentStream.MoveTo((float)x, (float)y);
        }

        public override void LineTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            _contentStream.LineTo((float)x, (float)y);
        }

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

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= MmToPt;
            y *= MmToPt;
            width *= MmToPt;
            height *= MmToPt;
            _contentStream.AddRect((float)x, (float)y, (float)width, (float)height);
        }

        public override void CloseSubpath()
        {
            _contentStream.ClosePath();
        }

        public override void FillPath(int color)
        {
            if (color != _lastNonStrokingColor)
            {
                _lastNonStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                _contentStream.SetNonStrokingColor(r, g, b);
            }
            _contentStream.Fill();
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            if (color != _lastStrokingColor)
            {
                _lastStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                _contentStream.SetStrokingColor(r, g, b);
            }
            if (strokeWidth != _lastLineWidth)
            {
                _lastLineWidth = strokeWidth;
                _contentStream.SetLineWidth((float)(strokeWidth));
            }
            _contentStream.Stroke();
        }

        public override byte[] GetResult()
        {
            MemoryStream buffer = new MemoryStream();
            _document.Save(buffer);
            Close();
            return buffer.ToArray();
        }

        protected void Close()
        {
            _contentStream = null;
            _document = null;
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }
    }
}
