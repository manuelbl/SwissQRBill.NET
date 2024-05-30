//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        /// <summary>
        /// Create a new instance with the specified title.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        public Document(string title)
        {
            _references = new List<Reference>();
            CreateReference(null); // dummy reference with index 0

            var catalog = new GeneralDict("Catalog");
            catalog.Add("Version", new Name("1.4"));
            _catalogRef = CreateReference(catalog);

            var documentInfo = new GeneralDict();
            _documentInfoRef = CreateReference(documentInfo);
            documentInfo.Add("Title", title);

            _pages = new PageCollection(this);
            _pagesRef = CreateReference(_pages);
            catalog.Add("Pages", _pagesRef);

            _fontReferences = new Dictionary<Font, Reference>();
        }

        /// <summary>
        /// Creates a new page with the specified dimensions.
        /// </summary>
        /// <param name="width">The width, in point.</param>
        /// <param name="height">The height, in point.</param>
        /// <returns></returns>
        public Page CreatePage(float width, float height)
        {
            var page = new Page(this, _pagesRef, width, height);
            _pages.Add(page);
            return page;
        }

        /// <summary>
        /// Saves the document to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
            using (var writer = new StreamWriter(stream, GetCodepage1252()))
            {
                writer.Write("%PDF-1.4\n");
                writer.Write("%öäüß\n");
                WriteBody(writer, stream);
                writer.Flush();
                var xrefOffset = stream.Length;
                WriteCrossReferenceTable(writer);
                WriteTrailer(writer, xrefOffset);
                writer.Flush();
            }
        }

        internal Reference CreateReference(object target)
        {
            var index = _references.Count;
            var reference = new Reference(index, target);
            _references.Add(reference);
            return reference;
        }

        internal Reference GetOrCreateFontReference(Font font)
        {
            if (_fontReferences.ContainsKey(font))
                return _fontReferences[font];

            var reference = CreateReference(font);
            _fontReferences.Add(font, reference);
            return reference;
        }

        private void WriteBody(StreamWriter writer, Stream stream)
        {
            foreach (var reference in _references)
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
            foreach (var reference in _references)
            {
                reference.WriteCrossReference(writer);
            }
        }

        private void WriteTrailer(StreamWriter writer, long xrefOffset)
        {
            var dict = new GeneralDict();
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
                return Encoding.GetEncoding(1252, new EncoderExceptionFallback(), new DecoderExceptionFallback());
            }
            catch (NotSupportedException)
            {
                // fall through
            }
            catch (ArgumentException)
            {
                // fall through
            }

            // `CodePagesEncodingProvider.Instance` is not part of .NET Standard 2.0 but is available (built-in) on all
            // supported versions of .NET Core, see https://apisof.net/catalog/653e5d69-d3b8-cf7c-2875-7f1822ab1354
            // So we register the CodePagesEncodingProvider.Instance through reflexion in order to avoid
            // taking a superfluous dependency on the System.Text.Encoding.CodePages NuGet package.
            // On .NET Framework, this issue is moot since Encoding.GetEncoding(1252) always succeeds.
            var encodingProvider = Type.GetType("System.Text.CodePagesEncodingProvider, System.Text.Encoding.CodePages");
            var instanceProperty = encodingProvider?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty?.GetValue(null) is EncodingProvider instance)
            {
                Encoding.RegisterProvider(instance);
            }
            return Encoding.GetEncoding(1252, new EncoderExceptionFallback(), new DecoderExceptionFallback());
        }
    }
}
