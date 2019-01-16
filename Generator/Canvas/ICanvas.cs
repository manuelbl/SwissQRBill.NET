//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Common graphics interface for drawing
    /// <para>
    /// The coordinate system is initialized by <see cref="SetupPage(double, double, string)"/>. It's origin is
    /// initially in the bottom left corner of the pages and extends in x-direction
    /// to the right and in y-direction to the top.
    /// </para>
    /// The font family is specified at initialization and then used for the entire
    /// lifecycle.
    /// <para>
    /// </para>
    /// <para>
    /// A canvas may only be used to generate a single page. After the result has
    /// been retrieved, the instance must not be used anymore.
    /// </para>
    /// </summary>
    public interface ICanvas : IDisposable
    {
        /// <summary>
        /// Sets up the page.
        /// <para>
        /// The page (and graphics context) is not valid until this method has been
        /// called.
        /// </para>
        /// <para>
        /// The font family can be specified as a comma separated list, e.g. "Helvetica,Arial,sans". The first
        /// family will be used to calculate text widths. For formats that support it (e.g. SVG), the entire list
        /// will be used in the generated graphics file. Other format will just use the first one.
        /// </para>
        /// </summary>
        /// <param name="width">The width of page (in mm).</param>
        /// <param name="height">The height of page (in mm).</param>
        /// <param name="fontFamily">The font family to use.</param>
        void SetupPage(double width, double height, string fontFamily);

        /// <summary>
        /// Sets a translation, rotation and scaling for the subsequent operations.
        /// <para>
        /// Before a new translation is applied, the coordinate system is reset to it's
        /// original state after page setup (see <see cref="SetupPage(double, double, string)"/>).
        /// </para>
        /// <para>
        /// The transformations are applied in the order translation, rotation, scaling.
        /// </para>
        /// </summary>
        /// <param name="translateX">The translation in x direction (in mm).</param>
        /// <param name="translateY">The translation in y direction (in mm).</param>
        /// <param name="rotate">The rotation angle, in radians.</param>
        /// <param name="scaleX">The scale factor in x direction (1.0 = no scaling).</param>
        /// <param name="scaleY">The scale factor in y direction (1.0 = no scaling).</param>
        void SetTransformation(double translateX, double translateY, double rotate, double scaleX, double scaleY);

        /// <summary>
        /// Draws text to the graphics.
        /// <para>
        /// The text position refers to the left most point on the text's baseline.
        /// </para>
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="x">The x position of the text's start (in mm).</param>
        /// <param name="y">The y position of the text's top (in mm).</param>
        /// <param name="fontSize">Thhe font size (in pt).</param>
        /// <param name="isBold">Flag indicating if the text is in bold or regular weight.</param>
        void PutText(string text, double x, double y, int fontSize, bool isBold);

        /// <summary>
        /// Adds several lines of text to the graphics.
        /// <para>
        /// The text position refers to the left most point on the baseline of the first
        /// text line. Additional lines then follow below.
        /// </para>
        /// </summary>
        /// <param name="lines">The text lines to draw.</param>
        /// <param name="x">The x position of the text's start (in mm).</param>
        /// <param name="y">The y position of the text's top (in mm).</param>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <param name="leading">The amount of additional vertical space between text lines (in mm).</param>
        void PutTextLines(string[] lines, double x, double y, int fontSize, double leading);

        /// <summary>
        /// Starts a path that can be filled or stroked.
        ///</summary>
        void StartPath();

        /// <summary>
        /// Moves the current point of the open path to the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of position.</param>
        /// <param name="y">The y-coordinate of position.</param>
        void MoveTo(double x, double y);

        /// <summary>
        /// Adds a line segment to the open path from the previous point to the speicifed
        /// position.
        /// </summary>
        /// <param name="x">The x-coordinate of position.</param>
        /// <param name="y">The y-coordinate of position.</param>
        void LineTo(double x, double y);

        /// <summary>
        /// Adds a cubic Beziér curve to the open path going from the previous point to the speicifed
        /// position. Two control points control the curve.
        /// </summary>
        /// <param name="x1">The x-coordinate of first control point.</param>
        /// <param name="y1">The y-coordinate of first control point.</param>
        /// <param name="x2">The x-coordinate of second control point.</param>
        /// <param name="y2">The y-coordinate of second control point.</param>
        /// <param name="x">The x-coordinate of position.</param>
        /// <param name="y">The y-coordinate of position.</param>
        void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y);

        /// <summary>
        /// Adds a rectangle to the current path.
        ///</summary>
        /// <param name="x">The rectangle's left position (in mm).</param>
        /// <param name="y">The rectangle's top position (in mm).</param>
        /// <param name="width">The rectangle's width (in mm).</param>
        /// <param name="height">Trectangle's height (in mm).</param>
        void AddRectangle(double x, double y, double width, double height);

        /// <summary>
        /// Closes the current subpath.
        /// </summary>
        void CloseSubpath();

        /// <summary>
        /// Fills the current path and ends it.
        /// </summary>
        /// <param name="color">The fill color (expressed similar to HTML, e.g. 0xffffff for white).</param>
        void FillPath(int color);

        /// <summary>
        /// Strokes the current path and ends it.
        /// </summary>
        /// <param name="strokeWidth">The stroke width (in pt).</param>
        /// <param name="color">The stroke color (expressed similar to HTML, e.g. 0xffffff for white).</param>
        void StrokePath(double strokeWidth, int color);

        /// <summary>
        /// Gets the distance between baseline and top of highest letter.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        double Ascender(int fontSize);

        /// <summary>
        /// Gets the distance between baseline and bottom of letter extending the farest below the
        /// baseline.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        double Descender(int fontSize);

        /// <summary>
        /// Gets the distance between the baselines of two consecutive text lines.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        double LineHeight(int fontSize);

        /// <summary>
        /// Measures the width of the specified text for the specified font size.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The  text's font size (in pt).</param>
        /// <param name="isBold">Flag indicating if the text is in bold or regular weight.</param>
        /// <returns>The measured width (in mm).</returns>
        double TextWidth(string text, int fontSize, bool isBold);

        /// <summary>
        /// Splits the text into lines.
        /// <para>
        /// If a line would exceed the specified maximum length, line breaks are
        /// inserted. Newlines are treated as fixed line breaks.
        /// </para>
        /// </summary>
        /// <param name="text">The text to split into lines.</param>
        /// <param name="maxLength">The maximum line length (in pt).</param>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The resulting array of text lines.</returns>
        string[] SplitLines(string text, double maxLength, int fontSize);

        /// <summary>
        /// Returns the generated graphics as a byte array.
        /// <para>
        /// After this method was called, the page is no longer valid.
        /// </para>
        /// </summary>>
        /// <returns>The generated images as a byte array.</returns>
        byte[] GetResult();
    }
}
