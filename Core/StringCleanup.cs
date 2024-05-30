//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Text;
using static Codecrete.SwissQRBill.Generator.Payments;

namespace Codecrete.SwissQRBill.Generator
{
    internal static class StringCleanup
    {
        // sorted by code point (BMP only, no surrogates, resulting in a single character valid for Latin-1 subset)
        private static readonly char[] QuickReplacementsFrom =
                "¨¯¸ÃÅÕÝãåõÿĀāĂăĄąĆćĈĉĊċČčĎďĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĨĩĪīĬĭĮįİĴĵĶķĹĺĻļĽľŃńŅņŇňŌōŎŏŐőŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžƠơƯưǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǞǟǠǡǦǧǨǩǪǫǬǭǰǴǵǸǹǺǻȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȞȟȦȧȨȩȪȫȬȭȮȯȰȱȲȳ˘˙˚˛˜˝ͺ΄΅ḀḁḂḃḄḅḆḇḈḉḊḋḌḍḎḏḐḑḒḓḔḕḖḗḘḙḚḛḜḝḞḟḠḡḢḣḤḥḦḧḨḩḪḫḬḭḮḯḰḱḲḳḴḵḶḷḸḹḺḻḼḽḾḿṀṁṂṃṄṅṆṇṈṉṊṋṌṍṎṏṐṑṒṓṔṕṖṗṘṙṚṛṜṝṞṟṠṡṢṣṤṥṦṧṨṩṪṫṬṭṮṯṰṱṲṳṴṵṶṷṸṹṺṻṼṽṾṿẀẁẂẃẄẅẆẇẈẉẊẋẌẍẎẏẐẑẒẓẔẕẖẗẘẙẛẠạẢảẤấẦầẨẩẪẫẬậẮắẰằẲẳẴẵẶặẸẹẺẻẼẽẾếỀềỂểỄễỆệỈỉỊịỌọỎỏỐốỒồỔổỖỗỘộỚớỜờỞởỠỡỢợỤụỦủỨứỪừỬửỮữỰựỲỳỴỵỶỷỸỹ᾽᾿῀῁῍῎῏῝῞῟῭΅´῾‗‾Å≠≮≯﹉﹊﹋﹌￣"
                        .ToCharArray();
        private static readonly char[] QuickReplacementsTo =
                "   AAOYaaoyAaAaAaCcCcCcCcDdEeEeEeEeEeGgGgGgGgHhIiIiIiIiIJjKkLlLlLlNnNnNnOoOoOoRrRrRrSsSsSsSsTtTtUuUuUuUuUuUuWwYyYZzZzZzOoUuAaIiOoUuUuUuUuUuAaAaGgKkOoOojGgNnAaAaAaEeEeIiIiOoOoRrRrUuUuSsTtHhAaEeOoOoOoOoYy         AaBbBbBbCcDdDdDdDdDdEeEeEeEeEeFfGgHhHhHhHhHhIiIiKkKkKkLlLlLlLlMmMmMmNnNnNnNnOoOoOoOoPpPpRrRrRrRrSsSsSsSsSsTtTtTtTtUuUuUuUuUuVvVvWwWwWwWwWwXxXxYyZzZzZzhtwysAaAaAaAaAaAaAaAaAaAaAaAaEeEeEeEeEeEeEeEeIiIiOoOoOoOoOoOoOoOoOoOoOoOoUuUuUuUuUuUuUuYyYyYyYy                A=<>     "
                        .ToCharArray();

        // additional replacements, not covered by Unicode decomposition
        // (from a single character to possibly multiple characters)
        private static readonly string[] AdditionalReplacementPairs = new string[] {
            "Œ", "OE",
            "œ", "oe",
            "Æ", "AE",
            "æ", "ae",
            "Ǣ", "AE",
            "ǣ", "ae",
            "Ǽ", "AE",
            "ǽ", "ae",
            "Ǿ", "O",
            "ǿ", "o",
            "ȸ", "db",
            "ȹ", "qp",
            "Ø", "O",
            "ø", "o",
            "€", "E",
            "^", ".",
            "¡", "! ",
            "¢", "c",
            "¤", " ",
            "¥", "Y",
            "¦", "/",
            "§", "S",
            "©", "(c)",
            "«", "<<",
            "¬", "-",
            "\u00AD", "", // soft hyphen
            "®", "(r)",
            "°", "o",
            "±", "+-",
            "µ", "u",
            "¶", "P",
            "·", "-",
            "»", ">>",
            "¿", "? ",
            "Ð", "D",
            "×", "x",
            "Þ", "TH",
            "ð", "d",
            "þ", "th",
            "Đ", "D",
            "đ", "d",
            "Ħ", "H",
            "ħ", "h",
            "ı", "i",
            "ĸ", "k",
            "Ŀ", "L",
            "ŀ", "l",
            "Ł", "L",
            "ł", "l",
            "ŉ", "n",
            "Ŋ", "N",
            "ŋ", "n",
            "Ŧ", "T",
            "ŧ", "t",
            "⁄", "/", // fraction slash
        };

        // additional replacements, not covered by Unicode decomposition (code point to string)
        private static readonly Dictionary<int, string> AdditionalReplacements = new Dictionary<int, string>();

        static StringCleanup()
        {
            for (int i = 0; i < AdditionalReplacementPairs.Length; i += 2)
            {
                AdditionalReplacements.Add(char.ConvertToUtf32(AdditionalReplacementPairs[i], 0), AdditionalReplacementPairs[i + 1]);
            }
        }

        /// <summary>
        /// Returns a cleaned text valid according to the specified character set.
        /// <para>
        /// Unsupported characters are replaced with supported characters, either with the same character without accent
        /// (e.g. A instead of Ă), with characters of similar meaning (e.g. TM instead of ™, ij instead of ĳ),
        /// with a space (for unsupported whitespace characters) or with a dot.
        /// </para>
        /// <para>
        /// Some valid letters can be represented either with a single Unicode code point or with two code points,
        /// e.g. the letter A with umlaut can be represented either with the single code point U+00C4 (latin capital
        /// letter A with diaeresis) or with the two code points U+0041 U+0308 (latin capital letter A,
        /// combining diaeresis). This will be recognized and converted to the valid single code point form.
        /// </para>
        /// <para>
        /// If {@code text} is {@code null} or the resulting string would be empty, {@code null} is returned.
        /// </para>
        /// </summary>
        /// 
        /// <param name="text">string to clean</param>
        /// <param name="characterSet">character set specifying valid characters</param>
        /// <returns>valid text for Swiss payments</returns>
        internal static string CleanedText(string text, SpsCharacterSet characterSet)
        {
            CleanText(text, characterSet, false, out CleaningResult result);
            return result.CleanedString;
        }

        /// <summary>
        /// Returns a cleaned and trimmed text valid according to the specified character set.
        /// <para>
        /// Unsupported characters are replaced with supported characters, either with the same character without accent
        /// (e.g. A instead of Ă), with characters of similar meaning (e.g. TM instead of ™, ij instead of ĳ),
        /// with a space (for unsupported whitespace characters) or with a dot.
        /// </para>
        /// <para>
        /// Leading and trailing whitespace is removed. Multiple consecutive spaces are replaced with a single whitespace.
        /// </para>
        /// <para>
        /// Some valid letters can be represented either with a single Unicode code point or with two code points,
        /// e.g. the letter A with umlaut can be represented either with the single code point U+00C4 (latin capital
        /// letter A with diaeresis) or with the two code points U+0041 U+0308 (latin capital letter A,
        /// combining diaeresis). This will be recognized and converted to the valid single code point form.
        /// </para>
        /// <para>
        /// If {@code text} is {@code null} or the resulting string would be empty, {@code null} is returned.
        /// </para>
        /// </summary>
        /// 
        /// <param name="text">string to clean</param>
        /// <param name="characterSet">character set specifying valid characters</param>
        /// <returns>valid text for Swiss payments</returns>
        internal static string CleanedAndTrimmedText(string text, SpsCharacterSet characterSet)
        {
            CleanText(text, characterSet, true, out CleaningResult result);
            return result.CleanedString;
        }

        /// <summary>
        /// Indicates if the text consists only of characters allowed in the specified character set.
        /// <para>
        /// This method does not attempt to deal with accents and umlauts built from two code points. It will
        /// return <c>false</c> if the text contains such characters.
        /// </para>
        /// </summary>
        /// 
        /// <param name="text">text to check, possibly <c>null</c></param>
        /// <param name="characterSet">character set specifying valid characters</param>
        /// <returns><c>true</c> if the text is valid, <c>false</c> otherwise</returns>
        internal static bool IsValidText(String text, SpsCharacterSet characterSet)
        {
            if (text == null)
                return true;

            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                if (!characterSet.Contains(text[i]))
                    return false;
            }
            return true;
        }

        internal static void CleanText(string text, SpsCharacterSet characterSet, bool trimWhitespace, out CleaningResult result)
        {
            result = new CleaningResult
            {
                CleanedString = null,
                ReplacedUnsupportedChars = false
            };

            if (text == null)
                return;

            // step 1: quick test for valid text
            bool isValidString = IsValidText(text, characterSet);

            if (!isValidString)
            {
                // step 2: normalize string (to deal with accents built from two code points) and test again
                if (!text.IsNormalized(NormalizationForm.FormC))
                {
                    text = text.Normalize(NormalizationForm.FormC);
                    isValidString = IsValidText(text, characterSet);
                }

                // step 3: replace characters
                if (!isValidString)
                {
                    text = ReplaceCharacters(text, characterSet);
                    result.ReplacedUnsupportedChars = true;
                }
            }

            if (trimWhitespace)
                text = text.SpacesCleaned();
            if (text.Length == 0)
                text = null;
            result.CleanedString = text;
        }

        private static string ReplaceCharacters(string text, SpsCharacterSet characterSet)
        {
            StringBuilder sb = new StringBuilder();
            int len = text.Length;
            int offset = 0;
            bool inFallback = false;
            while (offset < len)
            {
                int codePoint = char.ConvertToUtf32(text, offset);

                if (characterSet.Contains(codePoint))
                {
                    // valid code point
                    sb.AppendCodePoint(codePoint);
                    inFallback = false;
                }
                else if (ReplaceCodePoint(text, offset, characterSet, sb))
                {
                    // good replacement
                    inFallback = false;
                }
                else if (!inFallback)
                {
                    // no replacement found and not consecutive fallback
                    sb.Append('.');
                    inFallback = true;
                }

                offset += StringExtensions.GetUtf16SequenceLength((uint)codePoint);
            }
            return sb.ToString();
        }

        private static bool ReplaceCodePoint(string text, int offset, SpsCharacterSet characterSet, StringBuilder sb)
        {
            // whitespace is replaced with a space
            if (char.IsWhiteSpace(text, offset))
            {
                sb.Append(' ');
                return true;
            }

            // check if there is a quick replacement (precomputed case)
            bool isSurrogatePair = char.IsSurrogatePair(text, offset);
            if (!isSurrogatePair)
            {
                int pos = Array.BinarySearch(QuickReplacementsFrom, text[offset]);
                if (pos >= 0)
                {
                    sb.Append(QuickReplacementsTo[pos]);
                    return true;
                }
            }

            string codePointString = text.Substring(offset, isSurrogatePair ? 2 : 1);

            // check if canonical decomposition yields a valid string
            string canonical = DecomposedString(codePointString, characterSet, NormalizationForm.FormD);
            if (canonical != null)
            {
                sb.Append(canonical);
                return true;
            }

            // check if compatibility decomposition yields a valid string
            string compatibility = DecomposedString(codePointString, characterSet, NormalizationForm.FormKD);
            if (compatibility != null)
            {
                sb.Append(compatibility);
                return true;
            }

            // check for additional replacements
            if (AdditionalReplacements.TryGetValue(char.ConvertToUtf32(text, offset), out var replacement))
            {
                sb.Append(replacement);
                return true;
            }

            // no good replacement
            return false;
        }

        private static string DecomposedString(string codePointString, SpsCharacterSet characterSet, NormalizationForm form)
        {
            // decompose string
            string decomposedString = codePointString.Normalize(form);

            bool hasFractionSlash = false;

            // check for valid characters
            int len = decomposedString.Length;
            for (int i = 0; i < len; i += 1)
            {
                if (!characterSet.Contains(decomposedString[i]))
                {
                    // check if decomposition consists one or more valid characters
                    // and combining diacritical mark at the end
                    if (i == len - 1 && IsCombiningDiacriticalMark(decomposedString[i]))
                    {
                        return decomposedString.Substring(0, i);
                    }

                    // check for fraction slash (U+2044)
                    if (decomposedString[i] == '⁄')
                    {
                        hasFractionSlash = true;
                    }
                    else
                    {
                        // return null if no valid decomposition is available
                        return null;
                    }
                }
            }

            return hasFractionSlash ? decomposedString.Replace('⁄', '/') : decomposedString;
        }

        private static bool IsCombiningDiacriticalMark(char ch)
        {
            return ch >= '\u0300' && ch <= '\u036F';
        }
    }
}
