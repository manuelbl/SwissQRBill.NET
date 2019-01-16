//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Codecrete.SwissQRBill.Generator.PDF
{
    /// <summary>
    /// PDF document.
    /// </summary>
    public class Document
    {
        private readonly List<Reference> _references;
        private readonly Reference _catalogRef;
        private readonly Reference _documentInfoRef;
        private readonly PageCollection _pages;
        private readonly Reference _pagesRef;
        private readonly Dictionary<Font, Reference> _fontReferences;

        public Document(string title)
        {
            _references = new List<Reference>();
            CreateReference(null); // dummy reference with index 0

            GeneralDict catalog = new GeneralDict("Catalog");
            catalog.Add("Version", new Name("1.4"));
            _catalogRef = CreateReference(catalog);

            GeneralDict documentInfo = new GeneralDict();
            _documentInfoRef = CreateReference(documentInfo);
            documentInfo.Add("Title", title);

            _pages = new PageCollection(this);
            _pagesRef = CreateReference(_pages);
            catalog.Add("Pages", _pagesRef);

            _fontReferences = new Dictionary<Font, Reference>();
        }

        public Page CreatePage(float width, float height)
        {
            Page page = new Page(this, _pagesRef, width, height);
            _pages.Add(page);
            return page;
        }

        public void Save(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream, GetCodepage1252()))
            {
                writer.Write("%PDF-1.4\n");
                writer.Write("%öäüß\n");
                WriteBody(writer, stream);
                writer.Flush();
                long xrefOffset = stream.Length;
                WriteCrossReferenceTable(writer);
                WriteTrailer(writer, xrefOffset);
                writer.Flush();
            }
        }

        internal Reference CreateReference(object target)
        {
            int index = _references.Count;
            Reference reference = new Reference(index, target);
            _references.Add(reference);
            return reference;
        }

        internal Reference GetOrCreateFontReference(Font font)
        {
            if (_fontReferences.ContainsKey(font))
                return _fontReferences[font];

            Reference reference = CreateReference(font);
            _fontReferences.Add(font, reference);
            return reference;
        }

        private void WriteBody(StreamWriter writer, Stream stream)
        {
            foreach (Reference reference in _references)
            {
                writer.Flush();
                reference.Offset = (int)stream.Length;
                reference.WriteDefinition(writer);
            }
        }

        private void WriteCrossReferenceTable(StreamWriter writer)
        {
            writer.Write("xref\n");
            writer.Write($"0 {_references.Count}\n");
            foreach (Reference reference in _references)
            {
                reference.WriteCrossReference(writer);
            }
        }

        private void WriteTrailer(StreamWriter writer, long xrefOffset)
        {
            GeneralDict dict = new GeneralDict();
            dict.Add("Root", _catalogRef);
            dict.Add("Info", _documentInfoRef);
            dict.Add("Size", _references.Count);
            writer.Write("trailer\n");
            ((IWritable)dict).Write(writer);
            writer.Write("startxref\n");
            writer.Write($"{xrefOffset}\n");
            writer.Write("%%EOF\n");
        }

        internal static Encoding GetCodepage1252()
        {
            try
            {
                return Encoding.GetEncoding(1252);
            }
            catch (NotSupportedException)
            {
                // fall through
            }
            catch (ArgumentException)
            {
                // fall through
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(1252);
        }
    }
}
