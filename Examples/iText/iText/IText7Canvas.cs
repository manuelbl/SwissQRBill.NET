//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2019 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.IO;
using Codecrete.SwissQRBill.Generator.Canvas;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace Codecrete.SwissQRBill.Examples.IText7
{
    public class IText7Canvas : AbstractCanvas
    {
        /// <summary>
        /// Page number for adding the QR bill to the last page of the PDF document.
        /// </summary>
        public const int LastPage = -1;

        /// <summary>
        /// Page number for appending a new page for the QR bill at the end of the PDF document.
        /// </summary>
        public const int NewPageAtEnd = -2;

        private const float ColorScale = 1f / 255;
        private PdfDocument _document;
        private PdfCanvas _canvas;
        private int _lastStrokingColor;
        private int _lastNonStrokingColor;
        private double _lastLineWidth = 1;
        private PdfFont _lastFont;
        private float _lastFontSize;
        private LineStyle _lastLineStyle;

        /// <summary>
        /// Initializes a new instance of the PDF canvas with the specified page size.
        /// <para>
        /// A PDF with a single page of the specified size will be created. The QR bill
        /// will be drawn in the bottom left corner of the page.
        /// </para>
        /// <para>
        /// The created PDF will be written to the specified path when the canvas
        /// is closed (<see cref="Close"/>) or disposed (<see cref="Dispose"/>).
        /// </para>
        /// </summary>
        /// <param name="path">path of PDF file to create</param>
        /// <param name="width">The page width, in mm.</param>
        /// <param name="height">The page height, in mm.</param>
        public IText7Canvas(string path, double width, double height)
        {
            PdfWriter writer = new PdfWriter(path);
            CreateDocument(writer, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the PDF canvas with the specified page size.
        /// <para>
        /// A PDF with a single page of the specified size will be created. The QR bill
        /// will be drawn in the bottom left corner of the page.
        /// </para>
        /// <para>
        /// The created PDF will be written to the specified stream when the canvas
        /// is closed (<see cref="Close"/>) or disposed (<see cref="Dispose"/>).
        /// </para>
        /// </summary>
        /// <param name="stream">stream to write the created PDF file to</param>
        /// <param name="width">The page width, in mm.</param>
        /// <param name="height">The page height, in mm.</param>
        public IText7Canvas(Stream stream, double width, double height)
        {
            PdfWriter writer = new PdfWriter(stream);
            CreateDocument(writer, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the PDF canvas for adding the QR bill to
        /// an existing PDF document.
        /// <para>
        /// The QR bill can either be added to an existing page by specifying the
        /// zero-based page number (or <see cref="LastPage"/>. Or a new page at
        /// the end of the document can be created (<see cref="NewPageAtEnd"/>).
        /// </para>
        /// <para>
        /// The created canvas assumes that the page with the QR bill has an A4 portrait format
        /// and adds the QR bil at the bottom of the page.</para>
        /// <para>
        /// The final PDF will be written to the specified path when the canvas
        /// is closed (<see cref="Close"/>) or disposed (<see cref="Dispose"/>).
        /// </para>
        /// </summary>
        /// <param name="sourcePath">path to existing PDF document</param>
        /// <param name="destPath">path for the new PDF document with the additional QR bill</param>
        /// <param name="pageNo">the zero-based number of the page for adding the QR bill</param>
        public IText7Canvas(string sourcePath, string destPath, int pageNo)
        {
            PdfReader reader = new PdfReader(sourcePath);
            PdfWriter writer = new PdfWriter(destPath);
            OpenDocument(reader, writer, pageNo);
        }

        /// <summary>
        /// Initializes a new instance of the PDF canvas for adding the QR bill to
        /// an existing PDF document.
        /// <para>
        /// The QR bill can either be added to an existing page by specifying the
        /// zero-based page number (or <see cref="LastPage"/>. Or a new page at
        /// the end of the document can be created (<see cref="NewPageAtEnd"/>).
        /// </para>
        /// <para>
        /// The created canvas assumes that the page with the QR bill has an A4 portrait format
        /// and adds the QR bil at the bottom of the page.</para>
        /// <para>
        /// The final PDF will be written to the specified stream when the canvas
        /// is closed (<see cref="Close"/>) or disposed (<see cref="Dispose"/>).
        /// </para>
        /// </summary>
        /// <param name="source">stream to read existing PDF document from</param>
        /// <param name="destination">stream to wirte the new PDF document with the additional QR bill to</param>
        /// <param name="pageNo">the zero-based number of the page for adding the QR bill</param>
        public IText7Canvas(Stream source, Stream destination, int pageNo)
        {
            PdfReader reader = new PdfReader(source);
            PdfWriter writer = new PdfWriter(destination);
            OpenDocument(reader, writer, pageNo);
        }

        private void CreateDocument(PdfWriter writer, double width, double height)
        {
            SetupFontMetrics("Helvetica");
            _document = new PdfDocument(writer);
            _document.GetDocumentInfo().SetTitle("Swiss QR Bill");
            PageSize pageSize = new PageSize((float)(width * MmToPt), (float)(height * MmToPt));
            PdfPage page = _document.AddNewPage(pageSize);
            _canvas = new PdfCanvas(page);
            _canvas.SaveState();
        }

        private void OpenDocument(PdfReader reader, PdfWriter writer, int pageNo)
        {
            SetupFontMetrics("Helvetica");
            _document = new PdfDocument(reader, writer);

            PdfPage page;
            if (pageNo == NewPageAtEnd)
            {
                PageSize pageSize = new PageSize((float)(210 * MmToPt), (float)(297 * MmToPt));
                page = _document.AddNewPage(pageSize);
            }
            else if (pageNo == LastPage)
            {
                page = _document.GetLastPage();
            }
            else
            {
                page = _document.GetPage(pageNo);
            }

            _canvas = new PdfCanvas(page);
            _canvas.SaveState();
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            translateX *= MmToPt;
            translateY *= MmToPt;

            _canvas.RestoreState();
            _lastStrokingColor = 0;
            _lastNonStrokingColor = 0;
            _lastLineWidth = 1;

            _lastFont = null;
            _lastFontSize = 0;

            _canvas.SaveState();

            AffineTransform matrix = new AffineTransform();
            matrix.Translate(translateX, translateY);
            if (rotate != 0)
            {
                matrix.Rotate(rotate);
            }

            if (scaleX != 1 || scaleY != 1)
            {
                matrix.Scale(scaleX, scaleY);
            }

            _canvas.ConcatMatrix(matrix);
        }

        private void SetFont(bool isBold, int fontSize)
        {
            PdfFont font = PdfFontFactory.CreateFont(isBold ? StandardFonts.HELVETICA_BOLD : StandardFonts.HELVETICA);
            if (font == _lastFont && fontSize == _lastFontSize)
            {
                return;
            }

            _canvas.SetFontAndSize(font, fontSize);
            _lastFont = font;
            _lastFontSize = fontSize;
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            x *= MmToPt;
            y *= MmToPt;

            SetFont(isBold, fontSize);

            _canvas.BeginText()
                .MoveText(x, y)
                .ShowText(text)
                .EndText();
        }

        public override void PutTextLines(string[] lines, double x, double y, int fontSize, double leading)
        {
            x *= MmToPt;
            y *= MmToPt;
            float lineHeight = (float)((FontMetrics.LineHeight(fontSize) + leading) * MmToPt);

            SetFont(false, fontSize);

            _canvas.BeginText()
                .BeginText()
                .MoveText(x, y)
                .SetLeading(lineHeight);

            bool isFirstLine = true;
            foreach (string line in lines)
            {
                if (isFirstLine)
                {
                    _canvas.ShowText(line);
                    isFirstLine = false;
                }
                else
                {
                    _canvas.NewlineShowText(line);
                }
            }

            _canvas.EndText();
        }

        public override void StartPath()
        {
            // path is start implicitly
        }

        public override void MoveTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            _canvas.MoveTo(x, y);
        }

        public override void LineTo(double x, double y)
        {
            x *= MmToPt;
            y *= MmToPt;
            _canvas.LineTo(x, y);
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            x1 *= MmToPt;
            y1 *= MmToPt;
            x2 *= MmToPt;
            y2 *= MmToPt;
            x *= MmToPt;
            y *= MmToPt;
            _canvas.CurveTo(x1, y1, x2, y2, x, y);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= MmToPt;
            y *= MmToPt;
            width *= MmToPt;
            height *= MmToPt;
            _canvas.Rectangle(x, y, width, height);
        }

        public override void CloseSubpath()
        {
            _canvas.ClosePath();
        }

        public override void FillPath(int color)
        {
            if (color != _lastNonStrokingColor)
            {
                _lastNonStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                _canvas.SetFillColorRgb(r, g, b);
            }

            _canvas.Fill();
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            StrokePath(strokeWidth, color, LineStyle.Solid);
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle)
        {
            if (color != _lastStrokingColor)
            {
                _lastStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                _canvas.SetStrokeColorRgb(r, g, b);
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
                _canvas.SetLineCapStyle(lineStyle == LineStyle.Dotted ? 1 : 0);
                _canvas.SetLineDash(pattern, 0);
            }
            if (strokeWidth != _lastLineWidth)
            {
                _lastLineWidth = strokeWidth;
                _canvas.SetLineWidth((float) strokeWidth);
            }

            _canvas.Stroke();
        }

        protected void Close()
        {
            if (_document != null)
            {
                _canvas = null;
                _document.Close();
                _document = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }
    }
}
