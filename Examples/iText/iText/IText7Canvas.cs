using System.IO;
using Codecrete.SwissQRBill.Generator.Canvas;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace Codecrete.SwissQRBill.Examples.IText7
{
    class IText7Canvas : AbstractCanvas
    {
        private const float ColorScale = 1f / 255;
        private PdfDocument _document;
        private PdfCanvas _canvas;
        private int _lastStrokingColor;
        private int _lastNonStrokingColor;
        private double _lastLineWidth = 1;
        private PdfFont _lastFont;
        private float _lastFontSize;

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
            if (color != _lastStrokingColor)
            {
                _lastStrokingColor = color;
                float r = ColorScale * ((color >> 16) & 0xff);
                float g = ColorScale * ((color >> 8) & 0xff);
                float b = ColorScale * ((color >> 8) & 0xff);
                _canvas.SetStrokeColorRgb(r, g, b);
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
