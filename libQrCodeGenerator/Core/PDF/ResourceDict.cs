//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System.Collections.Generic;
using System.IO;

namespace libQrCodeGenerator.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// Dictionary for resources
    /// </summary>
    internal class ResourceDict : IWritable
    {
        private readonly GeneralDict _resources;
        private readonly Dictionary<Font, Name> _fontNames;
        private readonly Document _document;

        internal ResourceDict(Document document)
        {
            _document = document;
            _resources = new GeneralDict("Resources");
            _fontNames = new Dictionary<Font, Name>();
        }

        internal Name AddFont(Font font)
        {
            if (_fontNames.ContainsKey(font))
            {
                return _fontNames[font];
            }

            string fname = $"F{_fontNames.Count + 1}";
            Name name = new Name(fname);
            _fontNames.Add(font, name);
            _document.GetOrCreateFontReference(font);
            return name;
        }

        void IWritable.Write(StreamWriter writer)
        {
            GeneralDict fontDict = new GeneralDict();
            foreach (KeyValuePair<Font, Name> e in _fontNames)
            {
                fontDict.Add(e.Value.Value, _document.GetOrCreateFontReference(e.Key));
            }
            _resources.Add("Font", fontDict);

            (_resources as IWritable).Write(writer);
        }
    }
}
