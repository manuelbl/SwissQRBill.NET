//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;
using System.IO;

namespace Codecrete.SwissQRBill.Generator.PDF
{
    internal class GeneralDict : IWritable
    {
        private readonly Dictionary<string, object> dict;

        internal GeneralDict()
        {
            dict = new Dictionary<string, object>();
        }

        internal GeneralDict(string type)
        {
            dict = new Dictionary<string, object>
            {
                { "Type", new Name(type) }
            };
        }

        void IWritable.Write(StreamWriter writer)
        {
            writer.Write("<<\n");
            foreach (KeyValuePair<string, object> entry in dict)
            {
                writer.Write($"/{entry.Key} ");
                WriterHelper.WriteObject(writer, entry.Value);
                writer.Write("\n");
            }
            writer.Write(">>\n");
        }

        internal void Add(string key, object value)
        {
            dict.Add(key, value);
        }
    }
}
