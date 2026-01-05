//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Codecrete.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// Helper class for writing content.
    /// </summary>
    internal static class WriterHelper
    {
        internal static void WriteObject(StreamWriter writer, object obj)
        {
            switch (obj)
            {
                case IWritable writable:
                    writable.Write(writer);
                    break;
                case IEnumerable<object> list:
                    WriteList(writer, list);
                    break;
                case IEnumerable<float> floatList:
                    WriteList(writer, floatList);
                    break;
                case IEnumerable<int> intList:
                    WriteList(writer, intList);
                    break;
                case string str:
                    WriteString(writer, str);
                    break;
                case float flt:
                    WriteNumber(writer, flt);
                    break;
                default:
                    writer.Write(obj.ToString());
                    break;
            }
        }

        private static void WriteString(TextWriter writer, string str)
        {
            // ensure it is suitable for WinAnsi encoding
            Document.GetCodepage1252().GetBytes(str);
            WriteLiteralString(writer, str);
        }

        private static void WriteNumber(TextWriter writer, float num)
        {
            writer.Write(num.ToString("0.###", CultureInfo.InvariantCulture.NumberFormat));
        }

        private static void WriteList<T>(StreamWriter writer, IEnumerable<T> list)
        {
            writer.Write("[ ");
            foreach (var e in list)
            {
                WriteObject(writer, e);
                writer.Write(" ");
            }
            writer.Write("]");
        }

        internal static void WriteLiteralString(TextWriter writer, string text)
        {
            writer.Write("(");

            var length = text.Length;
            var lastCopiedPosition = 0;
            for (var i = 0; i < length; i++)
            {
                var ch = text[i];
                if (ch >= ' ' && ch != '(' && ch != ')' && ch != '\\')
                {
                    continue;
                }

                if (i > lastCopiedPosition)
                {
                    writer.Write(text.Substring(lastCopiedPosition, i - lastCopiedPosition));
                }

                string replacement;
                switch (ch)
                {
                    case '(':
                        replacement = "(";
                        break;
                    case ')':
                        replacement = ")";
                        break;
                    case '\\':
                        replacement = "\\";
                        break;
                    case '\n':
                        replacement = "n";
                        break;
                    case '\r':
                        replacement = "r";
                        break;
                    case '\t':
                        replacement = "t";
                        break;
                    default:
                        replacement = "000" + Convert.ToString(ch, 8);
                        replacement = replacement.Substring(replacement.Length - 3);
                        break;
                }
                writer.Write('\\');
                writer.Write(replacement);
                lastCopiedPosition = i + 1;
            }

            if (length > lastCopiedPosition)
            {
                writer.Write(text.Substring(lastCopiedPosition, length - lastCopiedPosition));
            }

            writer.Write(")");
        }

        internal static void WriteHexadecimalString(TextWriter writer, byte[] bytes)
        {
            writer.Write("<");
            var chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i += 1)
            {
                var b = bytes[i];
                chars[i * 2] = GetHexChar(b >> 4);
                chars[i * 2 + 1] = GetHexChar(b & 0x0F);
            }
            writer.Write(chars);
            writer.Write(">");
        }

        private static char GetHexChar(int value)
        {
            if (value < 10)
                return (char)('0' + value);
            else
                return (char)('A' + (value - 10));
        }
    }
}
