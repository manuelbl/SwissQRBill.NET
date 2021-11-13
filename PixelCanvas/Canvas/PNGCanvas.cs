//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Codecrete.SwissQRBill.PixelCanvas
{
    /// <summary>
    /// Canvas for generating PNG files.
    /// </summary>
    /// <remarks>
    /// PNGs are not an optimal file format for QR bills. Vector formats such a SVG
    /// or PDF are of better quality and use far less processing power to generate
    /// </remarks>
    public class PNGCanvas : AbstractCanvas
    {
        private readonly int _dpi;
        private readonly float _coordinateScale;
        private readonly float _fontScale;
        private SKCanvas _canvas;
        private SKPath _path;
        private SKPaint _strokePaint;
        private SKPaint _fillPaint;
        private SKPaint _textPaint;
        private SKBitmap _bitmap;
        private SKTypeface _regularTypeface;
        private SKTypeface _boldTypeface;


        /// <summary>
        /// Initializes a new instance of a PNG canvas with the given size, resolution and font family.
        /// <para>
        /// The QR bill will be drawn in the bottom left corner of the image.
        /// </para>
        /// <para>
        /// It is recommended to use at least 144 dpi for a readable result.
        /// </para>
        /// </summary>
        /// <param name="width">The image width, in mm.</param>
        /// <param name="height">The image height, in mm.</param>
        /// <param name="resolution">The resolution of the image to generate (in pixels per inch).</param>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        public PNGCanvas(double width, double height, int resolution, string fontFamilyList)
        {
            _dpi = resolution;

            // setup font metrics
            SetupFontMetrics(FindInstalledFontFamily(fontFamilyList));
            _regularTypeface = SKTypeface.FromFamilyName(FontMetrics.FirstFontFamily);
            _boldTypeface = SKTypeface.FromFamilyName(FontMetrics.FirstFontFamily, SKFontStyle.Bold);

            // create paints
            _fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Black,
                IsAntialias = true
            };

            _strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                IsAntialias = true
            };

            _textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Black,
                IsAntialias = true,
                SubpixelText = true,
                Typeface = _regularTypeface
            };

            // create image
            _coordinateScale = (float)(resolution / 25.4);
            _fontScale = (float)(resolution / 72.0);
            int w = (int)(width * _coordinateScale + 0.5);
            int h = (int)(height * _coordinateScale + 0.5);
            _bitmap = new SKBitmap(w, h, SKColorType.Rgb888x, SKAlphaType.Opaque);

            // create canvas
            _canvas = new SKCanvas(_bitmap);
            _canvas.Clear(SKColors.White);

            // initialize transformation
            _canvas.Translate(0, _bitmap.Height);
        }
        /// <summary>
        /// Finds the first font family from the specified list that is installed and not replaced with an alternative font.
        /// </summary>
        /// <param name="fontFamilyList">A list font family names, separated by comma (same syntax as for CSS). The first font family will be used.</param>
        /// <returns>font family name (if font is installed), or unchanged font family list (if none of the fonts is found)</returns>
        private static string FindInstalledFontFamily(string fontFamilyList)
        {
            foreach (string fontFamily in SplitCommaSeparated(fontFamilyList))
            {
                string family = fontFamily.Trim();
                using (SKTypeface typeface = SKTypeface.FromFamilyName(family, SKFontStyle.Normal))
                {
                    if (typeface.FamilyName.Equals(family, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return family;
                    }
                }
            }

            return fontFamilyList;
        }

        private static readonly Regex quotedSplitter = new Regex("(?:^|,)(\"[^\"]*\"|[^,]*)", RegexOptions.Compiled);

        /// <summary>
        /// Splits the comma separated list into its components.
        /// <para>
        /// A component may use double quotes (similar to CSV formats).
        /// </para>
        /// </summary>
        /// <param name="input">comma separated list</param>
        /// <returns>list of components</returns>
        private static IEnumerable<string> SplitCommaSeparated(string input)
        {
            foreach (Match match in quotedSplitter.Matches(input))
            {
                string component = match.Groups[1].Value;
                if (component[0] == '"' && component[component.Length - 1] == '"')
                {
                    component = component.Substring(1, component.Length - 2);
                }
                yield return component;
            }
        }

        /// <summary>
        /// Gets the resulting graphics encoded as a PNG image in a byte array.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <returns>The byte array containing the PNG image</returns>
        public override byte[] ToByteArray()
        {
            _canvas.Dispose();
            _canvas = null;
            byte[] result;

            using (SKImage image = SKImage.FromBitmap(_bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 90))
            using (Stream imageDataStream = data.AsStream())
            {
                MemoryStream buffer = new MemoryStream();
                PngProcessor.InsertDpi(imageDataStream, buffer, _dpi);
                result = buffer.ToArray();
            }

            Close();
            return result;
        }

        /// <summary>
        /// Writes the resulting graphics as a PNG image to the specified stream.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            _canvas.Dispose();
            _canvas = null;

            using (SKImage image = SKImage.FromBitmap(_bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 90))
            using (Stream imageDataStream = data.AsStream())
            {
                PngProcessor.InsertDpi(imageDataStream, stream, _dpi);
            }

            Close();
        }

        /// <summary>
        /// Writes the resulting graphics as a PNG image to the specified file path.
        /// <para>
        /// The canvas can no longer be used for drawing after calling this method.</para>
        /// </summary>
        /// <param name="path">The path (file name) to write to.</param>
        public void SaveAs(string path)
        {
            _canvas.Dispose();
            _canvas = null;

            using (SKImage image = SKImage.FromBitmap(_bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 90))
            using (Stream imageDataStream = data.AsStream())
            using (FileStream stream = File.OpenWrite(path))
            {
                PngProcessor.InsertDpi(imageDataStream, stream, _dpi);
            }

            Close();
        }

        protected void Close()
        {
            if (_canvas != null)
            {
                _canvas.Dispose();
                _canvas = null;
            }
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            if (_path != null)
            {
                _path.Dispose();
                _path = null;
            }
            if (_strokePaint != null)
            {
                _strokePaint.Dispose();
                _strokePaint = null;
                _fillPaint.Dispose();
                _fillPaint = null;
                _textPaint.Dispose();
                _textPaint = null;
                _regularTypeface.Dispose();
                _regularTypeface = null;
                _boldTypeface.Dispose();
                _boldTypeface = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }

        public override void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY)
        {
            // Our coordinate system extends from the bottom upwards. .NET's system
            // extends from the top downwards. So Y coordinates need to be treated specially.
            translateX *= _coordinateScale;
            translateY *= _coordinateScale;

            TransformationMatrix matrix = new TransformationMatrix();
            matrix.Translate(translateX, _bitmap.Height - translateY);
            matrix.Rotate(-rotate);
            matrix.Scale(scaleX, scaleY);
            double[] elems = matrix.Elements;
            SKMatrix skMatrix = new SKMatrix((float)elems[0], (float)elems[2], (float)elems[4], (float)elems[1], (float)elems[3], (float)elems[5], 0, 0, 1);
            _canvas.SetMatrix(skMatrix);
        }

        public override void StartPath()
        {
            if (_path != null)
                _path.Dispose();
            _path = new SKPath();
        }

        public override void CloseSubpath()
        {
            _path.Close();
        }

        public override void MoveTo(double x, double y)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _path.MoveTo((float)x, (float)y);
        }

        public override void LineTo(double x, double y)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _path.LineTo((float)x, (float)y);
        }

        public override void AddRectangle(double x, double y, double width, double height)
        {
            x *= _coordinateScale;
            y *= -_coordinateScale;
            width *= _coordinateScale;
            height *= -_coordinateScale;

            _path.AddRect(new SKRect((float)x, (float)(y + height), (float)(x + width), (float)y));
        }

        public override void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y)
        {
            x1 *= _coordinateScale;
            y1 *= -_coordinateScale;
            x2 *= _coordinateScale;
            y2 *= -_coordinateScale;
            x *= _coordinateScale;
            y *= -_coordinateScale;

            _path.CubicTo((float)x1, (float)y1, (float)x2, (float)y2, (float)x, (float)y);
        }

        public override void FillPath(int color, bool smoothing)
        {
            _path.Close();
            _fillPaint.IsAntialias = smoothing;
            _fillPaint.Color = new SKColor((uint)(color - 16777216));
            _canvas.DrawPath(_path, _fillPaint);
        }

        public override void StrokePath(double strokeWidth, int color, LineStyle lineStyle, bool smoothing)
        {
            float width = (float)strokeWidth * _fontScale;
            _strokePaint.Color = new SKColor((uint)(color - 16777216));
            _strokePaint.StrokeWidth = width;
            _strokePaint.IsAntialias = smoothing || lineStyle == LineStyle.Dotted;

            switch (lineStyle)
            {
                case LineStyle.Dashed:
                    _strokePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 4 * width, 4 * width }, 0);
                    _strokePaint.StrokeCap = SKStrokeCap.Round;
                    break;
                case LineStyle.Dotted:
                    _strokePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 0.01f * width, 2 * width }, 0);
                    _strokePaint.StrokeCap = SKStrokeCap.Butt;
                    break;
                default:
                    _strokePaint.PathEffect = null;
                    _strokePaint.StrokeCap = SKStrokeCap.Butt;
                    break;
            }

            _canvas.DrawPath(_path, _strokePaint);
        }

        public override void PutText(string text, double x, double y, int fontSize, bool isBold)
        {
            _textPaint.Typeface = isBold ? _boldTypeface : _regularTypeface;
            _textPaint.TextSize = fontSize * _fontScale;
            x *= _coordinateScale;
            y *= -_coordinateScale;
            _canvas.DrawText(text, (float)x, (float)y, _textPaint);
        }
    }
}
