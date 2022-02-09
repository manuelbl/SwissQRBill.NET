//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using libQrCodeGenerator.SwissQRBill.Generator.Canvas;
using System;
using System.IO;
using System.IO.Compression;
// using static System.FormattableString;

namespace libQrCodeGenerator.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// Stream of PDF commands (contents of a page).
    /// </summary>
    public class ContentStream : IWritable
    {
        private readonly MemoryStream _buffer;
        private StreamWriter _contentWriter;
        private readonly GeneralDict _dict;
        private readonly ResourceDict _resources;

        internal ContentStream(ResourceDict resources)
        {
            _resources = resources;
            _buffer = new MemoryStream();
            _buffer.WriteByte(0x78);
            _buffer.WriteByte(0xDA);
            DeflateStream deflateStream = new DeflateStream(_buffer, CompressionMode.Compress, true);
            _contentWriter = new StreamWriter(deflateStream, Document.GetCodepage1252());
            _dict = new GeneralDict();
        }

        /// <summary>
        /// Saves the graphics state.
        /// </summary>
        public void SaveGraphicsState()
        {
            WriteOperator("q");
        }

        /// <summary>
        /// Restores the graphics state.
        /// </summary>
        public void RestoreGraphicsState()
        {
            WriteOperator("Q");
        }

        /// <summary>
        /// Sets the transfomation matrix.
        /// </summary>
        /// <param name="matrix">The transformation matrix.</param>
        public void Transform(TransformationMatrix matrix)
        {
            foreach (double f in matrix.Elements)
            {
                WriteOperand((float)f);
            }

            WriteOperator("cm");
        }

        /// <summary>
        /// Sets the stroking color.
        /// </summary>
        /// <param name="red">Red color component (between 0.0 and 1.0)</param>
        /// <param name="green">Green color component (between 0.0 and 1.0)</param>
        /// <param name="blue">Blue Color component (between 0.0 and 1.0)</param>
        public void SetStrokingColor(float red, float green, float blue)
        {
            WriteOperand(red);
            WriteOperand(green);
            WriteOperand(blue);
            WriteOperator("RG");
        }

        /// <summary>
        /// Sets the non-stroking color.
        /// </summary>
        /// <param name="red">Red color component (between 0.0 and 1.0)</param>
        /// <param name="green">Green color component (between 0.0 and 1.0)</param>
        /// <param name="blue">Blue Color component (between 0.0 and 1.0)</param>
        public void SetNonStrokingColor(float red, float green, float blue)
        {
            WriteOperand(red);
            WriteOperand(green);
            WriteOperand(blue);
            WriteOperator("rg");
        }

        /// <summary>
        /// Sets the line width.
        /// </summary>
        /// <param name="width">Line width, in point.</param>
        public void SetLineWidth(float width)
        {
            WriteOperand(width);
            WriteOperator("w");
        }

        /// <summary>
        /// Sets the line cap style.
        /// </summary>
        /// <param name="style">Line cap style (see PDF reference).</param>
        public void SetLineCapStyle(int style)
        {
            WriteOperand(style);
            WriteOperator("J");
        }

        /// <summary>
        /// Sets the line dash pattern.
        /// </summary>
        /// <param name="pattern">Array of on and off length.</param>
        /// <param name="offset">Offset to first on element.</param>
        public void SetLineDashPattern(float[] pattern, float offset)
        {
            WriteOperand(pattern);
            WriteOperand(offset);
            WriteOperator("d");
        }

        /// <summary>
        /// Moves the current point of the current path.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public void MoveTo(float x, float y)
        {
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("m");
        }

        /// <summary>
        /// Adds a straight line to the current path.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public void LineTo(float x, float y)
        {
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("l");
        }

        /// <summary>
        /// Adds a bezier curve to the current path.
        /// </summary>
        /// <param name="x1">x-coordinate of control point 1</param>
        /// <param name="y1">y-coordinate of control point 1</param>
        /// <param name="x2">x-coordinate of control point 2</param>
        /// <param name="y2">y-coordinate of control point 2</param>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public void CurveTo(float x1, float y1, float x2, float y2, float x, float y)
        {
            WriteOperand(x1);
            WriteOperand(y1);
            WriteOperand(x2);
            WriteOperand(y2);
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("c");
        }

        /// <summary>
        /// Adds a closed rectangle to the current paht.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AddRect(float x, float y, float width, float height)
        {
            WriteOperand(x);
            WriteOperand(y);
            WriteOperand(width);
            WriteOperand(height);
            WriteOperator("re");
        }

        /// <summary>
        /// Closes the current path.
        /// </summary>
        public void ClosePath()
        {
            WriteOperator("h");
        }

        /// <summary>
        /// Stores the current path.
        /// </summary>
        public void Stroke()
        {
            WriteOperator("S");
        }

        /// <summary>
        /// Closes and strokes the current path.
        /// </summary>
        public void CloseAndStroke()
        {
            WriteOperator("s");
        }

        /// <summary>
        /// Fills the current path using the non-zero winding rule.
        /// </summary>
        public void Fill()
        {
            WriteOperator("f");
        }

        /// <summary>
        /// Closes the current path using the even-odd rule.
        /// </summary>
        public void FillEvenOdd()
        {
            WriteOperator("f*");
        }

        /// <summary>
        /// Fills and strokes the current path using the non-zero winding rule.
        /// </summary>
        public void FillAndStroke()
        {
            WriteOperator("B");
        }

        /// <summary>
        /// Closes and strokes the current path using the even-odd rule.
        /// </summary>
        public void FillEvenOddAndStroke()
        {
            WriteOperator("B*");
        }

        /// <summary>
        /// Closes, fills and strokes the current path using the non-zero winding rule.
        /// </summary>
        public void CloseAndFillAndStroke()
        {
            WriteOperator("b");
        }

        /// <summary>
        /// Closes, filles and strokes the current path using the even-odd rule.
        /// </summary>
        public void CloseAndFillEvenOddAndStroke()
        {
            WriteOperator("b*");
        }

        /// <summary>
        /// Sets the current font.
        /// </summary>
        /// <param name="font">the font.</param>
        /// <param name="fontSize">The font size.</param>
        public void SetFont(Font font, float fontSize)
        {
            Name fontName = _resources.AddFont(font);
            WriteOperand(fontName);
            WriteOperand(fontSize);
            WriteOperator("Tf");
        }

        /// <summary>
        /// Begins a text object.
        /// </summary>
        public void BeginText()
        {
            WriteOperator("BT");
        }

        /// <summary>
        /// Ends a text object.
        /// </summary>
        public void EndText()
        {
            WriteOperator("ET");
        }

        /// <summary>
        /// Moves to the next line, offset by the specified distance from the current one.
        /// </summary>
        /// <param name="tx">x-distance</param>
        /// <param name="ty">y-distance</param>
        public void NewLineAtOffset(float tx, float ty)
        {
            WriteOperand(tx);
            WriteOperand(ty);
            WriteOperator("Td");
        }

        /// <summary>
        /// Add the specified text to the curren text object.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ShowText(string text)
        {
            WriteTextOperand(text);
            WriteOperator("Tj");
        }

        private void WriteOperand(int val)
        {
            _contentWriter.Write($"{val} ");
        }

        private void WriteOperand(float val)
        {
            if (Math.Abs(val) < 0.0005)
            {
                _contentWriter.Write("0 ");
            }
            else
            {
                // _contentWriter.Write(Invariant($"{val:0.###} "));
                _contentWriter.Write(string.Format(System.Globalization.CultureInfo.InvariantCulture, $"{val:0.###} "));
            }
        }

        private void WriteOperand(float[] array)
        {
            _contentWriter.Write("[");
            foreach (float val in array)
            {
                WriteOperand(val);
            }
            _contentWriter.Write("] ");
        }

        private void WriteOperand(Name name)
        {
            _contentWriter.Write($"/{name.Value} ");
        }

        private void WriteTextOperand(string text)
        {
            text = WriterHelper.EscapeString(text);
            _contentWriter.Write($"({text}) ");
        }

        private void WriteOperator(string oper)
        {
            _contentWriter.Write($"{oper}\n");
        }

        void IWritable.Write(StreamWriter writer)
        {
            _contentWriter.Close();

            _dict.Add("Length", _buffer.Length);
            _dict.Add("Filter", new Name("FlateDecode"));
            ((IWritable)_dict).Write(writer);

            writer.Write("stream\r\n");
            writer.Flush();
            _buffer.Seek(0, SeekOrigin.Begin);
            _buffer.CopyTo(writer.BaseStream);
            writer.Write("\r\nendstream\n");

            _contentWriter.Dispose();
            _contentWriter = null;
        }
    }
}
