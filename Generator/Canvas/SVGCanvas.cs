//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Canvas for generating SVG files
    /// </summary>
    public class SVGCanvas : AbstractCanvas
    {
        private MemoryStream buffer;
        private StreamWriter stream;
        private bool isInGroup;
        private bool isFirstMoveInPath;
        private double lastPositionX;
        private double lastPositionY;
        private int approxPathLength;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public SVGCanvas()
        {
            // no further initialization needed here
        }

        public override void SetupPage(double width, double height, string fontFamilyList)
        {
            SetupFontMetrics(fontFamilyList);

            buffer = new MemoryStream();
            stream = new StreamWriter(buffer, Encoding.UTF8, 1024);
            stream.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n"
                    + "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n"
                    + "<svg width=\"");
            stream.Write(FormatNumber(width));
            stream.Write("mm\" height=\"");
            stream.Write(FormatNumber(height));
            stream.Write("mm\" version=\"1.1\" viewBox=\"0 0 ");
            stream.Write(FormatCoordinate(width));
            stream.Write(" ");
            stream.Write(FormatCoordinate(height));
            stream.Write("\" xmlns=\"http://www.w3.org/2000/svg\">\n");
            stream.Write("<g font-family=\"");
            stream.Write(EscapeXML(fontMetrics.FontFamilyList));
            stream.Write("\" transform=\"translate(0 ");
            stream.Write(FormatCoordinate(height));
            stream.Write(")\">\n");
            stream.Write("<title>Swiss QR Bill</title>\n");
        }

        protected void Close()
        {
            if (isInGroup)
            {
                stream.Write("</g>\n");
                isInGroup = false;
            }
            if (stream != null)
            {
                stream.Write("</g>\n");
                stream.Write("</svg>\n");
                stream.Close();
                stream = null;
            }
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

        public override void StartPath()
        {
            stream.Write("<path d=\"");
            isFirstMoveInPath = true;
            approxPathLength = 0;
        }

        public override void MoveTo(double x, double y)
        {
            y = -y;
            if (isFirstMoveInPath)
            {
                stream.Write("M");
                stream.Write(FormatCoordinate(x));
                stream.Write(",");
                stream.Write(FormatCoordinate(y));
                isFirstMoveInPath = false;
            }
            else
            {
                AddPathNewlines(16);
                stream.Write("m");
                stream.Write(FormatCoordinate(x - lastPositionX));
                stream.Write(",");
                stream.Write(FormatCoordinate(y - lastPositionY));
            }
            lastPositionX = x;
            lastPositionY = y;
            approxPathLength += 16;
        }

        public override void LineTo(double x, double y)
        {
            y = -y;
            AddPathNewlines(16);
            stream.Write("l");
            stream.Write(FormatCoordinate(x - lastPositionX));
            stream.Write(",");
            stream.Write(FormatCoordinate(y - lastPositionY));
            lastPositionX = x;
            lastPositionY = y;
            approxPathLength += 16;
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            y1 = -y1;
            y2 = -y2;
            y = -y;
            AddPathNewlines(48);
            stream.Write("c");
            stream.Write(FormatCoordinate(x1 - lastPositionX));
            stream.Write(",");
            stream.Write(FormatCoordinate(y1 - lastPositionY));
            stream.Write(",");
            stream.Write(FormatCoordinate(x2 - lastPositionX));
            stream.Write(",");
            stream.Write(FormatCoordinate(y2 - lastPositionY));
            stream.Write(",");
            stream.Write(FormatCoordinate(x - lastPositionX));
            stream.Write(",");
            stream.Write(FormatCoordinate(y - lastPositionY));
            lastPositionX = x;
            lastPositionY = y;
            approxPathLength += 48;
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            AddPathNewlines(40);
            MoveTo(x, y + height);
            stream.Write("h");
            stream.Write(FormatCoordinate(width));
            stream.Write("v");
            stream.Write(FormatCoordinate(height));
            stream.Write("h");
            stream.Write(FormatCoordinate(-width));
            stream.Write("z");
            approxPathLength += 24;
        }

        public override void CloseSubpath()
        {
            AddPathNewlines(1);
            stream.Write("z");
            approxPathLength += 1;
        }

        private void AddPathNewlines(int expectedLength)
        {
            if (approxPathLength + expectedLength > 255)
            {
                stream.Write("\n");
                approxPathLength = 0;
            }
        }

        public override void FillPath(int color)
        {
            stream.Write("\" fill=\"#");
            stream.Write(FormatColor(color));
            stream.Write("\"/>\n");
            isFirstMoveInPath = true;
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            stream.Write("\" stroke=\"#");
            stream.Write(FormatColor(color));
            if (strokeWidth != 1)
            {
                stream.Write("\" stroke-width=\"");
                stream.Write(FormatNumber(strokeWidth));
            }
            stream.Write("\" fill=\"none\"/>\n");
            isFirstMoveInPath = true;
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            y = -y;
            stream.Write("<text x=\"");
            stream.Write(FormatCoordinate(x));
            stream.Write("\" y=\"");
            stream.Write(FormatCoordinate(y));
            stream.Write("\" font-size=\"");
            stream.Write(FormatNumber(fontSize));
            if (isBold)
            {
                stream.Write("\" font-weight=\"bold");
            }

            stream.Write("\">");
            stream.Write(EscapeXML(text));
            stream.Write("</text>\n");
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            if (isInGroup)
            {
                stream.Write("</g>\n");
                isInGroup = false;
            }
            if (translateX != 0 || translateY != 0 || scaleX != 1 || scaleY != 1)
            {
                stream.Write("<g transform=\"translate(");
                stream.Write(FormatCoordinate(translateX));
                stream.Write(" ");
                stream.Write(FormatCoordinate(-translateY));
                if (rotate != 0)
                {
                    stream.Write(") rotate(");
                    stream.Write((-rotate / Math.PI * 180).ToString("#.#####", CultureInfo.InvariantCulture.NumberFormat));
                }
                if (scaleX != 1 || scaleY != 1)
                {
                    stream.Write(") scale(");
                    stream.Write(FormatNumber(scaleX));
                    if (scaleX != scaleY)
                    {
                        stream.Write(" ");
                        stream.Write(FormatNumber(scaleY));
                    }
                }
                stream.Write(")\">\n");
                isInGroup = true;
            }
        }

        public override byte[] Result
        {
            get
            {
                Close();
                return buffer.ToArray();
            }
        }

        private static string FormatNumber(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatCoordinate(double value)
        {
            return (value * MmToPt).ToString("0.###", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatColor(int color)
        {
            return $"{color:X6}";
        }

        private static string EscapeXML(string text)
        {
            int length = text.Length;
            int lastCopiedPosition = 0;
            StringBuilder result = null;
            for (int i = 0; i < length; i++)
            {
                char ch = text[i];
                if (ch == '<' || ch == '>' || ch == '&' || ch == '\'' || ch == '"')
                {
                    if (result == null)
                    {
                        result = new StringBuilder(length + 10);
                    }

                    if (i > lastCopiedPosition)
                    {
                        result.Append(text, lastCopiedPosition, i - lastCopiedPosition);
                    }

                    string entity;
                    switch (ch)
                    {
                        case '<':
                            entity = "&lt;";
                            break;
                        case '>':
                            entity = "&gt;";
                            break;
                        case '&':
                            entity = "&amp;";
                            break;
                        case '\'':
                            entity = "&apos;";
                            break;
                        default:
                            entity = "&quot;";
                            break;
                    }
                    result.Append(entity);
                    lastCopiedPosition = i + 1;
                }
            }

            if (result == null)
            {
                return text;
            }

            if (length > lastCopiedPosition)
            {
                result.Append(text, lastCopiedPosition, length - lastCopiedPosition);
            }

            return result.ToString();
        }
    }
}
