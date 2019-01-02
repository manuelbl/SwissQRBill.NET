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
    /// </summary>
    /// <remarks>
    /// The PDF generator currently only supports the Helvetica font.
    /// </remarks>
    public class PDFCanvas : AbstractCanvas
    {
        private static readonly float ColorScale = 1f / 255;
        private Document document;
        private ContentStream contentStream;
        private int lastStrokingColor = 0;
        private int lastNonStrokingColor = 0;
        private double lastLineWidth = 1;
        private bool hasSavedGraphicsState = false;
        private Font lastFont = null;
        private float lastFontSize = 0;

        public override void SetupPage(double width, double height, string fontFamilyList)
        {
            SetupFontMetrics("Helvetica");
            document = new Document("Swiss QR Bill");
            Page page = document.CreatePage((float)(width * MmToPt), (float)(height * MmToPt));
            contentStream = page.Contents;
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            translateX *= MmToPt;
            translateY *= MmToPt;

            if (hasSavedGraphicsState)
            {
                contentStream.RestoreGraphicsState();
                lastStrokingColor = 0;
                lastNonStrokingColor = 0;
                lastLineWidth = 1;
            }

            lastFont = null;
            lastFontSize = 0;

            contentStream.SaveGraphicsState();
            hasSavedGraphicsState = true;
            Matrix matrix = new Matrix();
            matrix.Translate((float)translateX, (float)translateY);
            if (rotate != 0)
            {
                matrix.Rotate((float)(rotate / Math.PI * 180));
            }

            if (scaleX != 1 || scaleY != 1)
            {
                matrix.Scale((float)scaleX, (float)scaleY);
            }

            contentStream.Transform(matrix);
        }

        private void SetFont(bool isBold, int fontSize)
        {
            Font font = isBold ? Font.HelveticaBold : Font.Helvetica;
            if (font != lastFont || fontSize != lastFontSize)
            {
                contentStream.SetFont(font, fontSize);
                lastFont = font;
                lastFontSize = fontSize;
            }
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            x *= MmToPt;
            y *= MmToPt;

            SetFont(isBold, fontSize);

            contentStream.BeginText();
            contentStream.NewLineAtOffset((float)x, (float)y);
            contentStream.ShowText(text);
            contentStream.EndText();
        }

        public override void PutTextLines(string[] lines, double x, double y, int fontSize, double leading)
        {
            x *= MmToPt;
            y *= MmToPt;
            float lineHeight = (float)((fontMetrics.LineHeight(fontSize) + leading) * MmToPt);

            SetFont(false, fontSize);

            contentStream.BeginText();
            contentStream.NewLineAtOffset((float)x, (float)y);
            bool isFirstLine = true;
            foreach (string line in lines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                }
                else
                {
                    contentStream.NewLineAtOffset(0, -lineHeight);
                }
                contentStream.ShowText(line);
            }
            contentStream.EndText();
        }

        public override void StartPath()
        {
            // path is start implicitly
        }

        public override void MoveTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            contentStream.MoveTo((float)x, (float)y);
        }

        public override void LineTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            contentStream.LineTo((float)x, (float)y);
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            x1 *= MmToPt;
            y1 *= MmToPt;
            x2 *= MmToPt;
            y2 *= MmToPt;
            x *= MmToPt;
            y *= MmToPt;
            contentStream.CurveTo((float)x1, (float)y1, (float)x2, (float)y2, (float)x, (float)y);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= MmToPt;
            y *= MmToPt;
            width *= MmToPt;
            height *= MmToPt;
            contentStream.AddRect((float)x, (float)y, (float)width, (float)height);
        }

        public override void CloseSubpath()
        {
            contentStream.ClosePath();
        }

        public override void FillPath(int color)
        {
            if (color != lastNonStrokingColor)
            {
                lastNonStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                contentStream.SetNonStrokingColor(r, g, b);
            }
            contentStream.Fill();
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            if (color != lastStrokingColor)
            {
                lastStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                contentStream.SetStrokingColor(r, g, b);
            }
            if (strokeWidth != lastLineWidth)
            {
                lastLineWidth = strokeWidth;
                contentStream.SetLineWidth((float)(strokeWidth));
            }
            contentStream.Stroke();
        }

        public override byte[] Result
        {
            get
            {
                MemoryStream buffer = new MemoryStream();
                document.Save(buffer);
                Close();
                return buffer.ToArray();

            }
        }

        protected void Close()
        {
            contentStream = null;
            document = null;
        }

        bool disposed = false;

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Close();
            }
            disposed = true;
        }
    }
}
