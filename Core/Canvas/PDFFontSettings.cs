//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2026 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

namespace Codecrete.SwissQRBill.Generator.Canvas
{
    /// <summary>
    /// Sets the font to use for a PDF canvas.
    /// <para>
    /// To render a QR bill to a PDF document, a regular and a bold font face are needed. According to the Swiss
    /// QR bill specification, only the non-serif fonts Helvetica, Arial, Liberation Sans and Frutiger are allowed.
    /// </para>
    /// <para>
    /// There are two options for fonts:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// Use the standard Helvetica font. It does not need to be embedded in the PDF document
    /// as it is supported by all PDF viewers. However, it only covers the WinANSI character set.
    /// This is sufficient for the <i>Latin 1 Subset</i> character set but not for the
    /// <i>Extended Latin</i> character set.
    /// </item>
    /// <item>
    /// Use the Liberation Sans font bundled with this library. It covers a wide range of characters and
    /// is sufficient for the <i>Extended Latin</i> character set. A subset of characters will be
    /// embedded in the PDF document. The font has been published under SIL OPEN FONT LICENSE Version 1.1
    /// and is free for use.
    /// </item>
    /// </list>
    /// </summary>
#pragma warning disable S101 // Types should be named in PascalCase
    public class PDFFontSettings
#pragma warning restore S101 // Types should be named in PascalCase
    {
        /// <summary>
        /// Creates a font settings instance for the standard Helvetica font.
        /// </summary>
        /// <returns>A new font settings instance.</returns>
        public static PDFFontSettings StandardHelvetica()
        {
            return new PDFFontSettings()
            {
                FontEmbedding = FontEmbedding.StandardHelvetica,
                FontFamily = "Helvetica"
            };
        }

        /// <summary>
        /// Creates a font settings instance for the Liberation Sans font.
        /// </summary>
        /// <returns>A new font settings instance.</returns>
        public static PDFFontSettings EmbeddedLiberationSans()
        {
            return new PDFFontSettings()
            {
                FontEmbedding = FontEmbedding.EmbeddedLiberationSans,
                FontFamily = "Liberation Sans"
            };
        }


        /// <summary>
        /// Gets the font embedding type.
        /// </summary>
        public FontEmbedding FontEmbedding { get; private set; }

        /// <summary>
        /// Gets the font family name.
        /// </summary>
        public string FontFamily { get; private set; }


        private PDFFontSettings()
        {
        }
    }


    /// <summary>
    /// Font embedding options.
    /// </summary>
    public enum FontEmbedding
    {
        /// <summary>
        /// Standard Helvetica font, without embedding.
        /// </summary>
        StandardHelvetica,
        /// <summary>
        /// Liberation Sans font included in this library, with embedding into the PDF document.
        /// </summary>
        EmbeddedLiberationSans,
    }
}