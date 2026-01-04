//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2025 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Codecrete.SwissQRBill.Generator.PDF
{
    internal class StreamData : IWritable
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        private readonly Dictionary<string, object> _dict;
        private readonly byte[] _data;
        private readonly string _resourceName;

        internal StreamData(Dictionary<string, object> dict, byte[] data)
        {
            _dict = dict;
            _data = data;
        }

        internal StreamData(Dictionary<string, object> dict, string resourceName)
        {
            _dict = dict;
            _resourceName = resourceName;
        }

        void IWritable.Write(StreamWriter writer)
        {
            writer.Write("<<\n");
            foreach (var entry in _dict)
            {
                writer.Write($"/{entry.Key} ");
                WriterHelper.WriteObject(writer, entry.Value);
                writer.Write("\n");
            }
            writer.Write(">>\n");

            writer.Write("stream\r\n");
            writer.Flush();

            if (_data != null)
            {
                writer.BaseStream.Write(_data, 0, _data.Length);
            }
            else
            {
                using (var stream = _assembly.GetManifestResourceStream("Codecrete.SwissQRBill.Generator.Fonts." + _resourceName))
                {
                    stream.CopyTo(writer.BaseStream);
                }
            }

            writer.Write("\r\nendstream\n");
        }
    }
}
