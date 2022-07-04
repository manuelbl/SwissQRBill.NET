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
            writer.Write("(");
            writer.Write(EscapeString(str));
            writer.Write(")");
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

        internal static string EscapeString(string text)
        {
            var length = text.Length;
            var lastCopiedPosition = 0;
            StringBuilder result = null;
            for (var i = 0; i < length; i++)
            {
                var ch = text[i];
                if (ch >= ' ' && ch != '(' && ch != ')' && ch != '\\')
                {
                    continue;
                }

                if (result == null)
                {
                    result = new StringBuilder(length + 10);
                }

                if (i > lastCopiedPosition)
                {
                    result.Append(text, lastCopiedPosition, i - lastCopiedPosition);
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
                result.Append('\\');
                result.Append(replacement);
                lastCopiedPosition = i + 1;
            }

            if (result == null)
            {
                return text;
            }

            if (length > lastCopiedPosition)
            {
                result.Append(text, lastCopiedPosition, length - lastCopiedPosition);
            }

            return result.ToString();
        }
    }
}
