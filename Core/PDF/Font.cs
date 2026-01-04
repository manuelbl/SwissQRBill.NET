//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.IO;

namespace Codecrete.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// Font for PDF document.
    /// </summary>
    public class Font : IWritable
    {
        /// <summary>
        /// Helvetica regular base font.
        /// </summary>
        public static readonly Font Helvetica = CreateBaseFont("Helvetica");
        /// <summary>
        /// Helvetica bold base font.
        /// </summary>
        public static readonly Font HelveticaBold = CreateBaseFont("Helvetica-Bold");
        /// <summary>
        /// Helvetica oblique base font.
        /// </summary>
        public static readonly Font HelveticaOblique = CreateBaseFont("Helvetica-Oblique");
        /// <summary>
        /// Helvetica bold oblique base font.
        /// </summary>
        public static readonly Font HelveticaBoldOblique = CreateBaseFont("Helvetica-BoldOblique");
        /// <summary>
        /// Times Roman base font.
        /// </summary>
        public static readonly Font TimesRoman = CreateBaseFont("Times-Roman");
        /// <summary>
        /// Times bold base font.
        /// </summary>
        public static readonly Font TimesBold = CreateBaseFont("Times-Bold");
        /// <summary>
        /// Times italic base font.
        /// </summary>
        public static readonly Font TimesItalic = CreateBaseFont("Times-Italic");
        /// <summary>
        /// Times bold italic base font.
        /// </summary>
        public static readonly Font TimesBoldItalic = CreateBaseFont("Times-BoldItalic");
        /// <summary>
        /// Courier regular base font.
        /// </summary>
        public static readonly Font Courier = CreateBaseFont("Courier");
        /// <summary>
        /// Courier bold base font.
        /// </summary>
        public static readonly Font CourierBold = CreateBaseFont("Courier-Bold");
        /// <summary>
        /// Courier oblique base font.
        /// </summary>
        public static readonly Font CourierOblique = CreateBaseFont("Courier-Oblique");
        /// <summary>
        /// Courier bold oblique base font.
        /// </summary>
        public static readonly Font CourierBoldOblique = CreateBaseFont("Courier-BoldOblique");
        /// <summary>
        /// Symbol base font.
        /// </summary>
        public static readonly Font Symbol = CreateBaseFont("Symbol");
        /// <summary>
        /// Zapf Dingbats base font.
        /// </summary>
        public static readonly Font ZapfDingbats = CreateBaseFont("ZapfDingbats");

        /// <summary>
        /// Gets the font subtype.
        /// </summary>
        public string Subtype { get; internal set; }
        /// <summary>
        /// Gets the font name.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Gets the font encoding.
        /// </summary>
        public string Encoding { get; internal set; }

        private GeneralDict _dictionary;

        private delegate byte[] TextEncoder(string text);
        private TextEncoder _encodeText;

        /// <summary>
        /// Creates a base font for the specified font name.
        /// <para>
        /// Font subtype "Type1" and WinAnsi encoding is assumed.
        /// </para>
        /// </summary>
        /// <param name="fontname">The font name.</param>
        /// <returns>Font instance.</returns>
        private static Font CreateBaseFont(string fontname)
        {
            var dict = new GeneralDict("Font");
            var font = new Font
            {
                Subtype = "Type1",
                Name = fontname,
                Encoding = "WinAnsiEncoding",
                _dictionary = dict,
                _encodeText = EncodeWinAnsi
            };
            dict.Add("Subtype", new Name(font.Subtype));
            dict.Add("BaseFont", new Name(font.Name));
            dict.Add("Encoding", new Name(font.Encoding));

            return font;
        }

        /// <summary>
        /// Creates a new font that will be embedded in the PDF document.
        /// <para>
        /// Depending on the <paramref name="isBold"/> parameter, either the Liberation Sans Regular
        /// or Liberation Sans Bold font is created. The regular font includes the glyphs valid
        /// in Swiss payments (Basic Latin, Latin-1 Supplement, Latin Extended-A and a few additional
        /// characters. The bold font includes the glyphs A-Z, a-z and some French and German accented
        /// characters.
        /// </para>
        /// </summary>
        /// <param name="isBold">Indictes if the bold font should be created.</param>
        /// <param name="document">The PDF document to add the font to.</param>
        /// <returns></returns>
        public static Font CreateEmbeddedFont(bool isBold, Document document)
        {
            var dict = isBold ? EmbeddedFonts.CreateLiberationSansBold(document)
                : EmbeddedFonts.CreateLiberationSansRegular(document);
            var font = new Font
            {
                Subtype = "Type0",
                Name = isBold ? "LiberationSans-Bold" : "LiberationSans",
                _dictionary = dict,
                _encodeText = isBold ? (TextEncoder)EmbeddedFonts.EncodeTextLiberationSansBold
                    : EmbeddedFonts.EncodeTextLiberationSansRegular

            };
            return font;
        }

        /// <summary>
        /// Encodes the specified text for use with this font.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <returns>The encoded text.</returns>
        public byte[] EncodeText(string text)
        {
            return _encodeText(text);
        }

        void IWritable.Write(StreamWriter writer)
        {
            ((IWritable)_dictionary).Write(writer);
        }

        private static byte[] EncodeWinAnsi(string text)
        {
            var encoding = Document.GetCodepage1252();
            return encoding.GetBytes(text);
        }
    }
}
