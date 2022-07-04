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
    /// Page in PDF document.
    /// </summary>
    public class Page : IWritable
    {
        private readonly GeneralDict _dict;

        /// <summary>
        /// Gets the content stream for the page contents.
        /// </summary>
        public ContentStream Contents { get; }

        internal Page(Document document, Reference parent, float width, float height)
        {
            _dict = new GeneralDict("Page");
            _dict.Add("Parent", parent);
            _dict.Add("MediaBox", new List<float> { 0, 0, width, height });

            var resources = new ResourceDict(document);
            var resourcesRef = document.CreateReference(resources);
            _dict.Add("Resources", resourcesRef);

            Contents = new ContentStream(resources);
            var contentsRef = document.CreateReference(Contents);
            _dict.Add("Contents", contentsRef);
        }

        void IWritable.Write(StreamWriter writer)
        {
            ((IWritable)_dict).Write(writer);
        }
    }
}
