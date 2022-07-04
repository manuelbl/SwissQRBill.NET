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

            var fname = $"F{_fontNames.Count + 1}";
            var name = new Name(fname);
            _fontNames.Add(font, name);
            _document.GetOrCreateFontReference(font);
            return name;
        }

        void IWritable.Write(StreamWriter writer)
        {
            var fontDict = new GeneralDict();
            foreach (var e in _fontNames)
            {
                fontDict.Add(e.Value.Value, _document.GetOrCreateFontReference(e.Key));
            }
            _resources.Add("Font", fontDict);

            (_resources as IWritable).Write(writer);
        }
    }
}
