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
    /// Canvas for generating SVG files.
    /// </summary>
    public class SVGCanvas : AbstractCanvas
    {
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        private readonly MemoryStream _buffer;
        private StreamWriter _stream;
        private bool _isInGroup;
        private bool _isFirstMoveInPath;
        private double _lastPositionX;
        private double _lastPositionY;
        private int _approxPathLength;


        /// <summary>
        /// Initializes a new SVG canvas with the specified size and font family.
        /// <para>
        /// The QR bill will be drawn in the bottom left corner of the SVG image.
        /// </para>
        /// </summary>
        /// <param name="width">The image width, in mm.</param>
        /// <param name="height">The image height, in mm.</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS).</param>
        public SVGCanvas(double width, double height, string fontFamilyList)
        {
            SetupFontMetrics(fontFamilyList);

            _buffer = new MemoryStream();
            _stream = new StreamWriter(_buffer, Utf8WithoutBom, 1024);
            _stream.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n"
                    + "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n"
                    + "<svg width=\"");
            _stream.Write(FormatNumber(width));
            _stream.Write("mm\" height=\"");
            _stream.Write(FormatNumber(height));
            _stream.Write("mm\" version=\"1.1\" viewBox=\"0 0 ");
            _stream.Write(FormatCoordinate(width));
            _stream.Write(" ");
            _stream.Write(FormatCoordinate(height));
            _stream.Write("\" xmlns=\"http://www.w3.org/2000/svg\">\n");
            _stream.Write("<g font-family=\"");
            _stream.Write(EscapeXml(FontMetrics.FontFamilyList));
            _stream.Write("\" transform=\"translate(0 ");
            _stream.Write(FormatCoordinate(height));
            _stream.Write(")\">\n");
            _stream.Write("<title>Swiss QR Bill</title>\n");
        }

        protected void Close()
        {
            if (_isInGroup)
            {
                _stream.Write("</g>\n");
                _isInGroup = false;
            }
            if (_stream != null)
            {
                _stream.Write("</g>\n");
                _stream.Write("</svg>\n");
                _stream.Close();
                _stream = null;
            }
        }

        /// <summary>
        /// Gets the resulting graphics as an SVG document in a byte array.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <returns>The byte array containing the SVG document</returns>
        public override byte[] ToByteArray()
        {
            Close();
            return _buffer.ToArray();
        }

        /// <summary>
        /// Writes the resulting graphics as an SVG image to the specified stream.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            long length = _buffer.Length;
            Close();
            byte[] data = _buffer.GetBuffer();
            stream.Write(data, 0, (int)length);
            stream.Flush();
        }

        /// <summary>
        /// Writes the resulting graphics as an SVG image to the specified file path.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="path">The path (file name) to write to.</param>
        public void SaveAs(string path)
        {
            Close();
            File.WriteAllBytes(path, _buffer.ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }

        public override void StartPath()
        {
            _stream.Write("<path d=\"");
            _isFirstMoveInPath = true;
            _approxPathLength = 0;
        }

        public override void MoveTo(double x, double y)
        {
            y = -y;
            if (_isFirstMoveInPath)
            {
                _stream.Write("M");
                _stream.Write(FormatCoordinate(x));
                _stream.Write(",");
                _stream.Write(FormatCoordinate(y));
                _isFirstMoveInPath = false;
            }
            else
            {
                AddPathNewlines(16);
                _stream.Write("m");
                _stream.Write(FormatCoordinate(x - _lastPositionX));
                _stream.Write(",");
                _stream.Write(FormatCoordinate(y - _lastPositionY));
            }
            _lastPositionX = x;
            _lastPositionY = y;
            _approxPathLength += 16;
        }

        public override void LineTo(double x, double y)
        {
            y = -y;
            AddPathNewlines(16);
            _stream.Write("l");
            _stream.Write(FormatCoordinate(x - _lastPositionX));
            _stream.Write(",");
            _stream.Write(FormatCoordinate(y - _lastPositionY));
            _lastPositionX = x;
            _lastPositionY = y;
            _approxPathLength += 16;
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            y1 = -y1;
            y2 = -y2;
            y = -y;
            AddPathNewlines(48);
            _stream.Write("c");
            _stream.Write(FormatCoordinate(x1 - _lastPositionX));
            _stream.Write(",");
            _stream.Write(FormatCoordinate(y1 - _lastPositionY));
            _stream.Write(",");
            _stream.Write(FormatCoordinate(x2 - _lastPositionX));
            _stream.Write(",");
            _stream.Write(FormatCoordinate(y2 - _lastPositionY));
            _stream.Write(",");
            _stream.Write(FormatCoordinate(x - _lastPositionX));
            _stream.Write(",");
            _stream.Write(FormatCoordinate(y - _lastPositionY));
            _lastPositionX = x;
            _lastPositionY = y;
            _approxPathLength += 48;
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            AddPathNewlines(40);
            MoveTo(x, y + height);
            _stream.Write("h");
            _stream.Write(FormatCoordinate(width));
            _stream.Write("v");
            _stream.Write(FormatCoordinate(height));
            _stream.Write("h");
            _stream.Write(FormatCoordinate(-width));
            _stream.Write("z");
            _approxPathLength += 24;
        }

        public override void CloseSubpath()
        {
            AddPathNewlines(1);
            _stream.Write("z");
            _approxPathLength += 1;
        }

        private void AddPathNewlines(int expectedLength)
        {
            if (_approxPathLength + expectedLength > 255)
            {
                _stream.Write("\n");
                _approxPathLength = 0;
            }
        }

        public override void FillPath(int color)
        {
            _stream.Write("\" fill=\"#");
            _stream.Write(FormatColor(color));
            _stream.Write("\"/>\n");
            _isFirstMoveInPath = true;
        }

        public override void StrokePath(double strokeWidth, int color)
        {
            StrokePath(strokeWidth, color, LineStyle.Solid);
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle)
        {
            _stream.Write("\" stroke=\"#");
            _stream.Write(FormatColor(color));
            if (strokeWidth != 1)
            {
                _stream.Write("\" stroke-width=\"");
                _stream.Write(FormatNumber(strokeWidth));
            }
            if (lineStyle == LineStyle.Dashed)
            {
                _stream.Write("\" stroke-dasharray=\"");
                _stream.Write(FormatNumber(strokeWidth * 4));
            }
            else if (lineStyle == LineStyle.Dotted)
            {
                _stream.Write("\" stroke-linecap=\"round\" stroke-dasharray=\"0 ");
                _stream.Write(FormatNumber(strokeWidth * 3));
            }
            _stream.Write("\" fill=\"none\"/>\n");
            _isFirstMoveInPath = true;
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            y = -y;
            _stream.Write("<text x=\"");
            _stream.Write(FormatCoordinate(x));
            _stream.Write("\" y=\"");
            _stream.Write(FormatCoordinate(y));
            _stream.Write("\" font-size=\"");
            _stream.Write(FormatNumber(fontSize));
            if (isBold)
            {
                _stream.Write("\" font-weight=\"bold");
            }

            _stream.Write("\">");
            _stream.Write(EscapeXml(text));
            _stream.Write("</text>\n");
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            if (_isInGroup)
            {
                _stream.Write("</g>\n");
                _isInGroup = false;
            }
            if (translateX != 0 || translateY != 0 || scaleX != 1 || scaleY != 1)
            {
                _stream.Write("<g transform=\"translate(");
                _stream.Write(FormatCoordinate(translateX));
                _stream.Write(" ");
                _stream.Write(FormatCoordinate(-translateY));
                if (rotate != 0)
                {
                    _stream.Write(") rotate(");
                    _stream.Write((-rotate / Math.PI * 180).ToString("#.#####", CultureInfo.InvariantCulture.NumberFormat));
                }
                if (scaleX != 1 || scaleY != 1)
                {
                    _stream.Write(") scale(");
                    _stream.Write(FormatNumber(scaleX));
                    if (scaleX != scaleY)
                    {
                        _stream.Write(" ");
                        _stream.Write(FormatNumber(scaleY));
                    }
                }
                _stream.Write(")\">\n");
                _isInGroup = true;
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

        private static string EscapeXml(string text)
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
