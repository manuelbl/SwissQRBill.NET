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
    /// Common interface for all output formats to draw the QR bill.
    /// <para>
    /// The coordinate system has its origin in the bottom left corner.
    /// The y-axis extends from the bottom to the top.
    /// </para>
    /// <para>
    /// The graphics model is similar to the one used by PDF, in particular with
    /// regards to the orientation of the y axis, the concept of a current path,
    /// and using the baseline for positioning text.
    /// </para>
    /// <para>
    /// Instance of this class are expected to use a single font family for
    /// the QR bill (regular and bold font weight).
    /// </para>
    /// </summary>
    public interface ICanvas : IDisposable
    {
        /// <summary>
        /// Returns the result as a byte array.
        /// </summary>
        /// <returns>The result.</returns>
        byte[] ToByteArray();
        /// <summary>
        /// Sets a translation, rotation and scaling for the subsequent operations.
        /// <para>
        /// Before a new translation is applied, the coordinate system is reset to it's
        /// original state.
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
        /// Draws text to the canvas.
        /// <para>
        /// The text position refers to the left most point on the text's baseline.
        /// </para>
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="x">The x position of the text's start (in mm).</param>
        /// <param name="y">The y position of the text's top (in mm).</param>
        /// <param name="fontSize">The font size (in pt).</param>
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
        /// Adds a line segment to the open path from the previous point to the specified
        /// position.
        /// </summary>
        /// <param name="x">The x-coordinate of position.</param>
        /// <param name="y">The y-coordinate of position.</param>
        void LineTo(double x, double y);

        /// <summary>
        /// Adds a cubic Beziér curve to the open path going from the previous point to the specified
        /// position. Two control points determine the curve.
        /// </summary>
        /// <param name="x1">The x-coordinate of first control point.</param>
        /// <param name="y1">The y-coordinate of first control point.</param>
        /// <param name="x2">The x-coordinate of second control point.</param>
        /// <param name="y2">The y-coordinate of second control point.</param>
        /// <param name="x">The x-coordinate of position.</param>
        /// <param name="y">The y-coordinate of position.</param>
        void CubicCurveTo(double x1, double y1, double x2, double y2, double x, double y);

        /// <summary>
        /// Adds a rectangle to the open path.
        ///</summary>
        /// <param name="x">The rectangle's left position (in mm).</param>
        /// <param name="y">The rectangle's top position (in mm).</param>
        /// <param name="width">The rectangle's width (in mm).</param>
        /// <param name="height">The rectangle's height (in mm).</param>
        void AddRectangle(double x, double y, double width, double height);

        /// <summary>
        /// Closes the current subpath. The next path operation will implicitly
        /// open a new subpath.
        /// </summary>
        void CloseSubpath();

        /// <summary>
        /// Fills the current path and discards it.
        /// </summary>
        /// <param name="color">The fill color (expressed similar to HTML, e.g. 0xffffff for white).</param>
        void FillPath(int color);

        /// <summary>
        /// Strokes the current path and discards it.
        /// </summary>
        /// <para>
        /// The path is stroked with a solid line.
        /// </para>
        /// <param name="strokeWidth">The stroke width (in pt).</param>
        /// <param name="color">The stroke color (expressed similar to HTML, e.g. 0xffffff for white).</param>
        void StrokePath(double strokeWidth, int color);

        /// <summary>
        /// Strokes the current path and discards it.
        /// </summary>
        /// <param name="strokeWidth">The stroke width (in pt).</param>
        /// <param name="color">The stroke color (expressed similar to HTML, e.g. 0xffffff for white).</param>
        /// <param name="lineStyle">The line style</param>
        void StrokePath(double strokeWidth, int color, LineStyle lineStyle);

        /// <summary>
        /// Gets the distance between the baseline and the top of the tallest letter.
        /// </summary>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The distance (in mm).</returns>
        double Ascender(int fontSize);

        /// <summary>
        /// Gets the distance between the baseline and the bottom of the letter extending the farthest below the
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
        /// The text is split such that no line is wider the specified maximum width.
        /// If possible, the text is split at whitespace characters. If a word is wider than
        /// the specified maximum width, the word is split and put onto two or more lines.
        /// The text is always split at newlines.
        /// </para>
        /// </summary>
        /// <param name="text">The text to split into lines.</param>
        /// <param name="maxLength">The maximum line length (in pt).</param>
        /// <param name="fontSize">The font size (in pt).</param>
        /// <returns>The resulting array of text lines.</returns>
        string[] SplitLines(string text, double maxLength, int fontSize);
    }
}
