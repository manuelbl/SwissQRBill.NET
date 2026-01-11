//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2025 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;

namespace Codecrete.SwissQRBill.Generator.PDF
{
#pragma warning disable S1192

    internal static class EmbeddedFonts
    {
        // Generated code. Do not modify. See the associated PDFFonts projects.
        internal static GeneralDict CreateLiberationSansRegular(Document document)
        {
            var cIDSystemInfo = new GeneralDict(new Dictionary<string, object>
            {
                { "Registry", "Adobe" },
                { "Ordering", "Identity" },
                { "Supplement", 0 },
            });

            var fontFile2 = new StreamData(new Dictionary<string, object>
            {
                { "Length", 29225 },
                { "Filter", new Name("FlateDecode") },
                { "Length1", 47712 },
            }, "LiberationSansRegular-FontFile2.bin");


            var cIDSet = new StreamData(new Dictionary<string, object>
            {
                { "Length", 14 },
                { "Filter", new Name("FlateDecode") },
            }, new byte[] { 0x78, 0x9C, 0xFB, 0xFF, 0x7F, 0x14, 0x10, 0x02, 0x7F, 0x00, 0xA1, 0xAC, 0x23, 0xE8, });


            var fontDescriptor = new GeneralDict(new Dictionary<string, object>
            {
                { "Type", new Name("FontDescriptor") },
                { "FontName", new Name("AAHBIE+LiberationSans") },
                { "Flags", 4 },
                { "FontWeight", 400.000000f },
                { "ItalicAngle", 0.000000f },
                { "FontBBox", new object[] {
                    -543.945313f,
                    -303.222656f,
                    1301.757813f,
                    979.980469f,
                } },
                { "Ascent", 905.273438f },
                { "Descent", -211.914063f },
                { "CapHeight", 687.988281f },
                { "XHeight", 528.320313f },
                { "StemV", 239.941391f },
                { "FontFile2", document.CreateReference(fontFile2) },
                { "CIDSet", document.CreateReference(cIDSet) },
            });

            var cIDToGIDMap = new StreamData(new Dictionary<string, object>
            {
                { "Length", 599 },
                { "Filter", new Name("FlateDecode") },
            }, "LiberationSansRegular-CIDToGIDMap.bin");


            var descendantFonts = new GeneralDict(new Dictionary<string, object>
            {
                { "Type", new Name("Font") },
                { "Subtype", new Name("CIDFontType2") },
                { "BaseFont", new Name("AAHBIE+LiberationSans") },
                { "CIDSystemInfo", document.CreateReference(cIDSystemInfo) },
                { "FontDescriptor", document.CreateReference(fontDescriptor) },
                { "W", new object[] {
                    0,
                    new int[] { 750, },
                    3,
                    new int[] { 278, 278, 355, 556, 556, 889, 667, 191, 333, 333, 389, 584, 278, 333, 278, 278, 556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 278, 278, 584, 584, 584, 556, 1015, 667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 278, 278, 278, 469, 556, 333, 556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556, 556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500, 334, 260, 334, 584, 278, 333, 556, 556, 556, 556, 260, 556, 333, 737, 370, 556, 584, 333, 737, 552, 400, 549, 333, 333, 333, 576, 537, 333, 333, 333, 365, 556, 834, 834, 834, 611, 667, 667, 667, 667, 667, 667, },
                    137,
                    new int[] { 722, 667, 667, 667, 667, 278, 278, 278, 278, 722, 722, 778, 778, 778, 778, 778, 584, 778, 722, 722, 722, 722, 667, 667, 611, 556, 556, 556, 556, 556, 556, 889, 500, 556, 556, 556, 556, 278, 278, 278, 278, 556, 556, 556, 556, 556, 556, 556, 549, 611, 556, 556, 556, 556, 500, 556, },
                    194,
                    new int[] { 667, 556, 667, 556, 667, 556, 722, 500, 722, 500, 722, 500, 722, 500, 722, 615, 722, 556, 667, 556, 667, 556, 667, 556, 667, 556, 667, 556, 778, 556, 778, 556, 778, 556, 778, 556, 722, 556, 722, 556, 278, 278, 278, 278, 278, 278, 278, 222, 278, 278, 735, 444, 500, 222, 667, 500, 500, 556, 222, 556, 222, 556, 292, 556, 334, 556, 222, 722, 556, 722, 556, 722, 556, 604, 723, 556, 778, 556, 778, 556, 778, 556, },
                    277,
                    new int[] { 944, 722, 333, 722, 333, 722, 333, 667, 500, 667, 500, 667, 500, 667, 500, 611, 278, 611, 375, 611, 278, 722, 556, 722, 556, 722, 556, 722, 556, 722, 556, 722, 556, 944, 722, 667, 500, 667, 611, 500, 611, 500, 611, 500, 222, },
                    474,
                    new int[] { 667, 500, 611, 278, 545, },
                    648,
                    new int[] { 333, 333, },
                    651,
                    new int[] { 333, },
                    666,
                    new int[] { 333, 333, 333, 333, 333, 333, },
                    2015,
                    new int[] { 556, },
                    2021,
                    new int[] { 222, },
                    2046,
                    new int[] { 167, },
                    2054,
                    new int[] { 430, },
                    2088,
                    new int[] { 556, },
                    2130,
                    new int[] { 278, },
                    2330,
                    new int[] { 333, },
                    2332,
                    new int[] { 222, 222, 294, 294, 324, 324, 316, 328, 398, 285, },
                } },
                { "CIDToGIDMap", document.CreateReference(cIDToGIDMap) },
            });

            var toUnicode = new StreamData(new Dictionary<string, object>
            {
                { "Length", 343 },
                { "Filter", new Name("FlateDecode") },
            }, "LiberationSansRegular-ToUnicode.bin");


            var font = new GeneralDict(new Dictionary<string, object>
            {
                { "Type", new Name("Font") },
                { "BaseFont", new Name("AAHBIE+LiberationSans") },
                { "Subtype", new Name("Type0") },
                { "Encoding", new Name("Identity-H") },
                { "DescendantFonts", new object[] {
                    document.CreateReference(descendantFonts),
                } },
                { "ToUnicode", document.CreateReference(toUnicode) },
            });

            return font;
        }

        // Generated code. Do not modify. See the associated PDFFonts projects.
        internal static readonly char[] GlyphIndexesLiberationSansRegular_0020_007e = new char[] { '\x0003', '\x0004', '\x0005', '\x0006', '\x0007', '\x0008', '\x0009', '\x000A', '\x000B', '\x000C', '\x000D', '\x000E', '\x000F', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001A', '\x001B', '\x001C', '\x001D', '\x001E', '\x001F', '\x0020', '\x0021', '\x0022', '\x0023', '\x0024', '\x0025', '\x0026', '\x0027', '\x0028', '\x0029', '\x002A', '\x002B', '\x002C', '\x002D', '\x002E', '\x002F', '\x0030', '\x0031', '\x0032', '\x0033', '\x0034', '\x0035', '\x0036', '\x0037', '\x0038', '\x0039', '\x003A', '\x003B', '\x003C', '\x003D', '\x003E', '\x003F', '\x0040', '\x0041', '\x0042', '\x0043', '\x0044', '\x0045', '\x0046', '\x0047', '\x0048', '\x0049', '\x004A', '\x004B', '\x004C', '\x004D', '\x004E', '\x004F', '\x0050', '\x0051', '\x0052', '\x0053', '\x0054', '\x0055', '\x0056', '\x0057', '\x0058', '\x0059', '\x005A', '\x005B', '\x005C', '\x005D', '\x005E', '\x005F', '\x0060', '\x0061', };
        internal static readonly char[] GlyphIndexesLiberationSansRegular_00a0_00fe = new char[] { '\x0062', '\x0063', '\x0064', '\x0065', '\x0066', '\x0067', '\x0068', '\x0069', '\x006A', '\x006B', '\x006C', '\x006D', '\x006E', '\x006F', '\x0070', '\x0071', '\x0072', '\x0073', '\x0074', '\x0075', '\x0076', '\x0077', '\x0078', '\x0079', '\x007A', '\x007B', '\x007C', '\x007D', '\x007E', '\x007F', '\x0080', '\x0081', '\x0082', '\x0083', '\x0084', '\x0085', '\x0086', '\x0087', '\x0088', '\x0089', '\x008A', '\x008B', '\x008C', '\x008D', '\x008E', '\x008F', '\x0090', '\x0091', '\x0092', '\x0093', '\x0094', '\x0095', '\x0096', '\x0097', '\x0098', '\x0099', '\x009A', '\x009B', '\x009C', '\x009D', '\x009E', '\x009F', '\x00A0', '\x00A1', '\x00A2', '\x00A3', '\x00A4', '\x00A5', '\x00A6', '\x00A7', '\x00A8', '\x00A9', '\x00AA', '\x00AB', '\x00AC', '\x00AD', '\x00AE', '\x00AF', '\x00B0', '\x00B1', '\x00B2', '\x00B3', '\x00B4', '\x00B5', '\x00B6', '\x00B7', '\x00B8', '\x00B9', '\x00BA', '\x00BB', '\x00BC', '\x00BD', '\x00BE', '\x00BF', '\x00C0', };
        internal static readonly char[] GlyphIndexesLiberationSansRegular_0100_017f = new char[] { '\x00C2', '\x00C3', '\x00C4', '\x00C5', '\x00C6', '\x00C7', '\x00C8', '\x00C9', '\x00CA', '\x00CB', '\x00CC', '\x00CD', '\x00CE', '\x00CF', '\x00D0', '\x00D1', '\x00D2', '\x00D3', '\x00D4', '\x00D5', '\x00D6', '\x00D7', '\x00D8', '\x00D9', '\x00DA', '\x00DB', '\x00DC', '\x00DD', '\x00DE', '\x00DF', '\x00E0', '\x00E1', '\x00E2', '\x00E3', '\x00E4', '\x00E5', '\x00E6', '\x00E7', '\x00E8', '\x00E9', '\x00EA', '\x00EB', '\x00EC', '\x00ED', '\x00EE', '\x00EF', '\x00F0', '\x00F1', '\x00F2', '\x00F3', '\x00F4', '\x00F5', '\x00F6', '\x00F7', '\x00F8', '\x00F9', '\x00FA', '\x00FB', '\x00FC', '\x00FD', '\x00FE', '\x00FF', '\x0100', '\x0101', '\x0102', '\x0103', '\x0104', '\x0105', '\x0106', '\x0107', '\x0108', '\x0109', '\x010A', '\x010B', '\x010C', '\x010D', '\x010E', '\x010F', '\x0110', '\x0111', '\x0112', '\x0113', '\x0114', '\x0115', '\x0116', '\x0117', '\x0118', '\x0119', '\x011A', '\x011B', '\x011C', '\x011D', '\x011E', '\x011F', '\x0120', '\x0121', '\x0122', '\x0123', '\x0124', '\x0125', '\x0126', '\x0127', '\x0128', '\x0129', '\x012A', '\x012B', '\x012C', '\x012D', '\x012E', '\x012F', '\x0130', '\x0131', '\x0132', '\x0133', '\x0134', '\x0135', '\x0136', '\x0137', '\x0138', '\x0139', '\x013A', '\x013B', '\x013C', '\x013D', '\x013E', '\x013F', '\x0140', '\x0141', };
        internal static readonly char[] GlyphIndexesLiberationSansRegular_0218_021c = new char[] { '\x01DA', '\x01DB', '\x01DC', '\x01DD', '\x01DE', };
        internal static readonly char[] GlyphIndexesLiberationSansRegular_20ac_20ac = new char[] { '\x0828', };
        internal static readonly char[] GlyphIndexesLiberationSansRegular_2013_2013 = new char[] { '\x07DF', };

        // Generated code. Do not modify. See the associated PDFFonts projects.
        internal static byte[] EncodeTextLiberationSansRegular(string text)
        {
            var bytes = new byte[text.Length * 2];
            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                char[] glyphIndexArray;
                int glyphIndex;
                if (ch >= 0x0020 && ch <= 0x007e)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansRegular_0020_007e;
                    glyphIndex = ch - 0x0020;
                }
                else if (ch >= 0x00a0 && ch <= 0x00fe)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansRegular_00a0_00fe;
                    glyphIndex = ch - 0x00a0;
                }
                else if (ch >= 0x0100 && ch <= 0x017f)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansRegular_0100_017f;
                    glyphIndex = ch - 0x0100;
                }
                else if (ch >= 0x0218 && ch <= 0x021c)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansRegular_0218_021c;
                    glyphIndex = ch - 0x0218;
                }
                else if (ch >= 0x20ac && ch <= 0x20ac)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansRegular_20ac_20ac;
                    glyphIndex = ch - 0x20ac;
                }
                else if (ch >= 0x2013 && ch <= 0x2013)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansRegular_2013_2013;
                    glyphIndex = ch - 0x2013;
                }
                else
                {
                    throw new ArgumentException($"Character U+{(int)ch:X4} is not part of the font LiberationSansRegular (text: \"{text}\")");
                }
                bytes[i * 2] = (byte)(glyphIndexArray[glyphIndex] >> 8);
                bytes[i * 2 + 1] = (byte)glyphIndexArray[glyphIndex];
            }
            return bytes;
        }


        // Generated code. Do not modify. See the associated PDFFonts projects.
        internal static GeneralDict CreateLiberationSansBold(Document document)
        {
            var cIDSystemInfo = new GeneralDict(new Dictionary<string, object>
            {
                { "Registry", "Adobe" },
                { "Ordering", "Identity" },
                { "Supplement", 0 },
            });

            var fontFile2 = new StreamData(new Dictionary<string, object>
            {
                { "Length", 12546 },
                { "Filter", new Name("FlateDecode") },
                { "Length1", 19028 },
            }, "LiberationSansBold-FontFile2.bin");


            var cIDSet = new StreamData(new Dictionary<string, object>
            {
                { "Length", 13 },
                { "Filter", new Name("FlateDecode") },
            }, new byte[] { 0x78, 0x9C, 0xFB, 0xFF, 0x9F, 0xCA, 0xA0, 0x01, 0x00, 0x3E, 0x4B, 0x51, 0x30, });


            var fontDescriptor = new GeneralDict(new Dictionary<string, object>
            {
                { "Type", new Name("FontDescriptor") },
                { "FontName", new Name("AAACTL+LiberationSans-Bold") },
                { "Flags", 4 },
                { "FontWeight", 700.000000f },
                { "ItalicAngle", 0.000000f },
                { "FontBBox", new object[] {
                    -481.933594f,
                    -376.464844f,
                    1304.199219f,
                    1033.203125f,
                } },
                { "Ascent", 905.273438f },
                { "Descent", -211.914063f },
                { "CapHeight", 687.988281f },
                { "XHeight", 528.320313f },
                { "StemV", 232.197250f },
                { "FontFile2", document.CreateReference(fontFile2) },
                { "CIDSet", document.CreateReference(cIDSet) },
            });

            var cIDToGIDMap = new StreamData(new Dictionary<string, object>
            {
                { "Length", 169 },
                { "Filter", new Name("FlateDecode") },
            }, new byte[] { 0x78, 0x9C, 0xED, 0xCE, 0xD7, 0x4E, 0x42, 0x51, 0x10, 0x05, 0xD0, 0x05, 0x2A, 0x76, 0x05, 0x11, 0x7B, 0x45, 0x54, 0x2C, 0x20, 0x0A, 0x0A, 0x36, 0x50, 0x11, 0x1B, 0x58, 0xFE, 0xFF, 0x6B, 0x34, 0x84, 0x87, 0x4B, 0xE2, 0x7D, 0x41, 0x1F, 0xEF, 0x4A, 0xE6, 0x4C, 0x66, 0x72, 0x76, 0x32, 0x74, 0xC5, 0xF4, 0x8B, 0x1B, 0x0A, 0x4C, 0xC3, 0x46, 0x24, 0x8C, 0x1A, 0x33, 0x6E, 0xC2, 0xA4, 0x29, 0xD3, 0x66, 0xCC, 0xF6, 0x25, 0x92, 0x52, 0xE6, 0xA4, 0xCD, 0xCB, 0x58, 0xB0, 0x68, 0xC9, 0xB2, 0x15, 0xAB, 0xD6, 0xAC, 0xDB, 0xB0, 0x69, 0xCB, 0xB6, 0xAC, 0x1D, 0x39, 0xBB, 0xF6, 0xEC, 0xCB, 0x3B, 0x08, 0xA4, 0x0F, 0x1D, 0x39, 0x56, 0x50, 0x74, 0xA2, 0xE4, 0xD4, 0x99, 0xB2, 0x8A, 0x73, 0x17, 0xAA, 0x6A, 0x2E, 0x5D, 0xB9, 0x76, 0xE3, 0x56, 0x5D, 0xC3, 0x9D, 0x7B, 0x0F, 0x9A, 0x1E, 0x85, 0x69, 0x85, 0xEC, 0x9F, 0x7A, 0xFD, 0x39, 0x34, 0x39, 0x98, 0x97, 0x5E, 0x7F, 0xED, 0xBE, 0x6F, 0xDA, 0x3A, 0xBF, 0xFE, 0x7B, 0xFF, 0xA9, 0x8F, 0xC0, 0xFC, 0xF9, 0xCF, 0x77, 0x44, 0x22, 0x91, 0xBF, 0xF9, 0xFA, 0x06, 0xB5, 0xD5, 0x0C, 0xA9, });


            var descendantFonts = new GeneralDict(new Dictionary<string, object>
            {
                { "Type", new Name("Font") },
                { "Subtype", new Name("CIDFontType2") },
                { "BaseFont", new Name("AAACTL+LiberationSans-Bold") },
                { "CIDSystemInfo", document.CreateReference(cIDSystemInfo) },
                { "FontDescriptor", document.CreateReference(fontDescriptor) },
                { "W", new object[] {
                    0,
                    new int[] { 750, },
                    3,
                    new int[] { 278, },
                    11,
                    new int[] { 333, 333, },
                    18,
                    new int[] { 278, 556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 333, },
                    36,
                    new int[] { 722, 722, 722, 722, 667, 611, 778, 722, 278, 556, 722, 611, 833, 722, 778, 667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, },
                    67,
                    new int[] { 333, 556, 611, 556, 611, 556, 333, 611, 611, 278, 278, 556, 278, 889, 611, 611, 611, 611, 389, 556, 333, 611, 556, 778, 556, 556, 500, },
                    106,
                    new int[] { 333, },
                    118,
                    new int[] { 333, },
                    122,
                    new int[] { 333, },
                    162,
                    new int[] { 556, },
                    166,
                    new int[] { 556, },
                    169,
                    new int[] { 556, 556, 556, },
                    182,
                    new int[] { 611, },
                    184,
                    new int[] { 611, },
                    190,
                    new int[] { 611, },
                    648,
                    new int[] { 333, },
                } },
                { "CIDToGIDMap", document.CreateReference(cIDToGIDMap) },
            });

            var toUnicode = new StreamData(new Dictionary<string, object>
            {
                { "Length", 329 },
                { "Filter", new Name("FlateDecode") },
            }, "LiberationSansBold-ToUnicode.bin");


            var font = new GeneralDict(new Dictionary<string, object>
            {
                { "Type", new Name("Font") },
                { "BaseFont", new Name("AAACTL+LiberationSans-Bold") },
                { "Subtype", new Name("Type0") },
                { "Encoding", new Name("Identity-H") },
                { "DescendantFonts", new object[] {
                    document.CreateReference(descendantFonts),
                } },
                { "ToUnicode", document.CreateReference(toUnicode) },
            });

            return font;
        }

        // Generated code. Do not modify. See the associated PDFFonts projects.
        internal static readonly char[] GlyphIndexesLiberationSansBold_0041_005a = new char[] { '\x0024', '\x0025', '\x0026', '\x0027', '\x0028', '\x0029', '\x002A', '\x002B', '\x002C', '\x002D', '\x002E', '\x002F', '\x0030', '\x0031', '\x0032', '\x0033', '\x0034', '\x0035', '\x0036', '\x0037', '\x0038', '\x0039', '\x003A', '\x003B', '\x003C', '\x003D', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_0061_007a = new char[] { '\x0044', '\x0045', '\x0046', '\x0047', '\x0048', '\x0049', '\x004A', '\x004B', '\x004C', '\x004D', '\x004E', '\x004F', '\x0050', '\x0051', '\x0052', '\x0053', '\x0054', '\x0055', '\x0056', '\x0057', '\x0058', '\x0059', '\x005A', '\x005B', '\x005C', '\x005D', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_0030_0039 = new char[] { '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001A', '\x001B', '\x001C', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_0020_0020 = new char[] { '\x0003', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_0028_0029 = new char[] { '\x000B', '\x000C', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_002f_002f = new char[] { '\x0012', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_003a_003a = new char[] { '\x001D', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_00e0_00e0 = new char[] { '\x00A2', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_00e4_00e4 = new char[] { '\x00A6', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_00e7_00e9 = new char[] { '\x00A9', '\x00AA', '\x00AB', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_00f4_00f4 = new char[] { '\x00B6', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_00f6_00f6 = new char[] { '\x00B8', };
        internal static readonly char[] GlyphIndexesLiberationSansBold_00fc_00fc = new char[] { '\x00BE', };

        // Generated code. Do not modify. See the associated PDFFonts projects.
#pragma warning disable S3776
        internal static byte[] EncodeTextLiberationSansBold(string text)
        {
            var bytes = new byte[text.Length * 2];
            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                char[] glyphIndexArray;
                int glyphIndex;
                if (ch >= 0x0041 && ch <= 0x005a)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_0041_005a;
                    glyphIndex = ch - 0x0041;
                }
                else if (ch >= 0x0061 && ch <= 0x007a)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_0061_007a;
                    glyphIndex = ch - 0x0061;
                }
                else if (ch >= 0x0030 && ch <= 0x0039)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_0030_0039;
                    glyphIndex = ch - 0x0030;
                }
                else if (ch >= 0x0020 && ch <= 0x0020)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_0020_0020;
                    glyphIndex = ch - 0x0020;
                }
                else if (ch >= 0x0028 && ch <= 0x0029)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_0028_0029;
                    glyphIndex = ch - 0x0028;
                }
                else if (ch >= 0x002f && ch <= 0x002f)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_002f_002f;
                    glyphIndex = ch - 0x002f;
                }
                else if (ch >= 0x003a && ch <= 0x003a)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_003a_003a;
                    glyphIndex = ch - 0x003a;
                }
                else if (ch >= 0x00e0 && ch <= 0x00e0)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_00e0_00e0;
                    glyphIndex = ch - 0x00e0;
                }
                else if (ch >= 0x00e4 && ch <= 0x00e4)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_00e4_00e4;
                    glyphIndex = ch - 0x00e4;
                }
                else if (ch >= 0x00e7 && ch <= 0x00e9)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_00e7_00e9;
                    glyphIndex = ch - 0x00e7;
                }
                else if (ch >= 0x00f4 && ch <= 0x00f4)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_00f4_00f4;
                    glyphIndex = ch - 0x00f4;
                }
                else if (ch >= 0x00f6 && ch <= 0x00f6)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_00f6_00f6;
                    glyphIndex = ch - 0x00f6;
                }
                else if (ch >= 0x00fc && ch <= 0x00fc)
                {
                    glyphIndexArray = GlyphIndexesLiberationSansBold_00fc_00fc;
                    glyphIndex = ch - 0x00fc;
                }
                else
                {
                    throw new ArgumentException($"Character U+{(int)ch:X4} is not part of the font LiberationSansBold (text: \"{text}\")");
                }
                bytes[i * 2] = (byte)(glyphIndexArray[glyphIndex] >> 8);
                bytes[i * 2 + 1] = (byte)glyphIndexArray[glyphIndex];
            }
            return bytes;
        }
#pragma warning restore S3776

    }

    }
