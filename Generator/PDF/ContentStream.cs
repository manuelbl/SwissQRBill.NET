//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using System.IO;
using System.IO.Compression;
using static System.FormattableString;

namespace Codecrete.SwissQRBill.Generator.PDF
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

        public void SaveGraphicsState()
        {
            WriteOperator("q");
        }

        public void RestoreGraphicsState()
        {
            WriteOperator("Q");
        }

        public void Transform(TransformationMatrix matrix)
        {
            foreach (double f in matrix.Elements)
            {
                WriteOperand((float)f);
            }

            WriteOperator("cm");
        }

        public void SetStrokingColor(float red, float green, float blue)
        {
            WriteOperand(red);
            WriteOperand(green);
            WriteOperand(blue);
            WriteOperator("RG");
        }

        public void SetNonStrokingColor(float red, float green, float blue)
        {
            WriteOperand(red);
            WriteOperand(green);
            WriteOperand(blue);
            WriteOperator("rg");
        }

        public void SetLineWidth(float width)
        {
            WriteOperand(width);
            WriteOperator("w");
        }

        public void SetLineCapStyle(int style)
        {
            WriteOperand(style);
            WriteOperator("J");
        }

        public void SetLineDashPattern(float[] pattern, float offset)
        {
            WriteOperand(pattern);
            WriteOperand(offset);
            WriteOperator("d");
        }

        public void MoveTo(float x, float y)
        {
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("m");
        }

        public void LineTo(float x, float y)
        {
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("l");
        }

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

        public void CurveTo2(float x2, float y2, float x, float y)
        {
            WriteOperand(x2);
            WriteOperand(y2);
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("v");
        }

        public void CurveTo1(float x1, float y1, float x, float y)
        {
            WriteOperand(x1);
            WriteOperand(y1);
            WriteOperand(x);
            WriteOperand(y);
            WriteOperator("y");
        }

        public void AddRect(float x, float y, float width, float height)
        {
            WriteOperand(x);
            WriteOperand(y);
            WriteOperand(width);
            WriteOperand(height);
            WriteOperator("re");
        }

        public void ClosePath()
        {
            WriteOperator("h");
        }

        public void Stroke()
        {
            WriteOperator("S");
        }

        public void CloseAndStroke()
        {
            WriteOperator("s");
        }

        public void Fill()
        {
            WriteOperator("f");
        }

        public void FillEvenOdd()
        {
            WriteOperator("f*");
        }

        public void FillAndStroke()
        {
            WriteOperator("B");
        }

        public void FillEvenOddAndStroke()
        {
            WriteOperator("B*");
        }

        public void CloseAndFillAndStroke()
        {
            WriteOperator("b");
        }

        public void CloseAndFillEvenOddAndStroke()
        {
            WriteOperator("b*");
        }

        public void SetFont(Font font, float fontSize)
        {
            Name fontName = _resources.AddFont(font);
            WriteOperand(fontName);
            WriteOperand(fontSize);
            WriteOperator("Tf");
        }

        public void BeginText()
        {
            WriteOperator("BT");
        }

        public void EndText()
        {
            WriteOperator("ET");
        }

        public void NewLineAtOffset(float tx, float ty)
        {
            WriteOperand(tx);
            WriteOperand(ty);
            WriteOperator("Td");
        }

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
            _contentWriter.Write(Invariant($"{val:0.###} "));
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
