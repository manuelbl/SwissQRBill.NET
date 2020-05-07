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
        public static readonly Font Helvetica = CreateBaseFont("Helvetica");
        public static readonly Font HelveticaBold = CreateBaseFont("Helvetica-Bold");
        public static readonly Font HelveticaOblique = CreateBaseFont("Helvetica-Oblique");
        public static readonly Font HelveticaBoldOblique = CreateBaseFont("Helvetica-BoldOblique");
        public static readonly Font TimesRoman = CreateBaseFont("Times-Roman");
        public static readonly Font TimesBold = CreateBaseFont("Times-Bold");
        public static readonly Font TimesItalic = CreateBaseFont("Times-Italic");
        public static readonly Font TimesBoldItalic = CreateBaseFont("Times-BoldItalic");
        public static readonly Font Courier = CreateBaseFont("Courier");
        public static readonly Font CourierBold = CreateBaseFont("Courier-Bold");
        public static readonly Font CourierOblique = CreateBaseFont("Courier-Oblique");
        public static readonly Font CourierBoldOblique = CreateBaseFont("Courier-BoldOblique");
        public static readonly Font Symbol = CreateBaseFont("Symbol");
        public static readonly Font ZapfDingbats = CreateBaseFont("ZapfDingbats");

        public string Subtype { get; internal set; }
        public string Name { get; internal set; }
        public string Encoding { get; internal set; }

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
