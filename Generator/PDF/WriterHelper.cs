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

        internal static void WriteString(StreamWriter writer, string str)
        {
            writer.Write("(");
            writer.Write(EscapeString(str));
            writer.Write(")");
        }

        internal static void WriteNumber(StreamWriter writer, float num)
        {
            writer.Write(num.ToString("0.###", CultureInfo.InvariantCulture.NumberFormat));
        }

        internal static void WriteList<T>(StreamWriter writer, IEnumerable<T> list)
        {
            writer.Write("[ ");
            foreach (T e in list)
            {
                WriteObject(writer, e);
                writer.Write(" ");
            }
            writer.Write("]");
        }

        internal static string EscapeString(string text)
        {
            int length = text.Length;
            int lastCopiedPosition = 0;
            StringBuilder result = null;
            for (int i = 0; i < length; i++)
            {
                char ch = text[i];
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
                        replacement = "\\(";
                        break;
                    case ')':
                        replacement = "\\)";
                        break;
                    case '\\':
                        replacement = "\\\\";
                        break;
                    case '\n':
                        replacement = "\\n";
                        break;
                    case '\r':
                        replacement = "\\r";
                        break;
                    case '\t':
                        replacement = "\\t";
                        break;
                    default:
                        replacement = "000" + Convert.ToString(ch, 8);
                        replacement = replacement.Substring(replacement.Length - 3);
                        break;
                }
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
