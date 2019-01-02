//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Drawing.Drawing2D;
using System.IO;
using static System.FormattableString;

namespace Codecrete.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// Stream of PDF commands (contents of a page).
    /// </summary>
    public class ContentStream : IWritable
    {
        private MemoryStream buffer;
        private StreamWriter contentWriter;
        private GeneralDict dict;
        private readonly ResourceDict resources;

        internal ContentStream(ResourceDict resources)
        {
            this.resources = resources;
            buffer = new MemoryStream();
            contentWriter = new StreamWriter(buffer, Document.GetCodepage1252());
            dict = new GeneralDict();
        }

        public void SaveGraphicsState()
        {
            WriteOperator("q");
        }

        public void RestoreGraphicsState()
        {
            WriteOperator("Q");
        }

        public void Transform(Matrix matrix)
        {
            foreach (float f in matrix.Elements)
            {
                WriteOperand(f);
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
            Name fontName = resources.AddFont(font);
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

        private void WriteOperand(float val)
        {
            contentWriter.Write(Invariant($"{val:0.###} "));
        }

        private void WriteOperand(Name name)
        {
            contentWriter.Write($"/{name.Value} ");
        }

        private void WriteTextOperand(string text)
        {
            text = WriterHelper.EscapeString(text);
            contentWriter.Write($"({text}) ");
        }

        private void WriteOperator(string oper)
        {
            contentWriter.Write($"{oper}\n");
        }

        void IWritable.Write(StreamWriter writer)
        {
            contentWriter.Flush();

            dict.Add("Length", buffer.Length);
            ((IWritable)dict).Write(writer);

            writer.Write("stream\n");
            writer.Flush();
            buffer.Seek(0, SeekOrigin.Begin);
            buffer.CopyTo(writer.BaseStream);
            writer.Write("endstream\n");

            contentWriter.Dispose();
            contentWriter = null;
        }
    }
}
