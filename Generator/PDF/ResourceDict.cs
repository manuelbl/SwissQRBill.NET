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
        private readonly GeneralDict resources;
        private readonly Dictionary<Font, Name> fontNames;
        private readonly Document document;

        internal ResourceDict(Document document)
        {
            this.document = document;
            resources = new GeneralDict("Resources");
            fontNames = new Dictionary<Font, Name>();
        }

        internal Name AddFont(Font font)
        {
            if (fontNames.ContainsKey(font))
            {
                return fontNames[font];
            }

            string fname = $"F{fontNames.Count + 1}";
            Name name = new Name(fname);
            fontNames.Add(font, name);
            document.GetOrCreateFontReference(font);
            return name;
        }

        void IWritable.Write(StreamWriter writer)
        {
            GeneralDict fontDict = new GeneralDict();
            foreach (KeyValuePair<Font, Name> e in fontNames)
            {
                fontDict.Add(e.Value.Value, document.GetOrCreateFontReference(e.Key));
            }
            resources.Add("Font", fontDict);

            (resources as IWritable).Write(writer);
        }
    }
}
