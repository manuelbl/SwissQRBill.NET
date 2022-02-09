﻿//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.IO;

namespace libQrCodeGenerator.SwissQRBill.Generator.PDF
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
            return new Font
            {
                Subtype = "Type1",
                Name = fontname,
                Encoding = "WinAnsiEncoding"
            };
        }

        void IWritable.Write(StreamWriter writer)
        {
            GeneralDict dict = new GeneralDict("Font");
            dict.Add("Subtype", new Name(Subtype));
            dict.Add("BaseFont", new Name(Name));
            dict.Add("Encoding", new Name(Encoding));
            ((IWritable)dict).Write(writer);
        }
    }
}
