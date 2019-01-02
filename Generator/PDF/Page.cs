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
        private readonly Document document;
        private GeneralDict dict;
        private readonly Reference contentsRef;
        private readonly ResourceDict resources;
        private readonly Reference resourcesRef;

        public ContentStream Contents { get; }

        internal Page(Document document, Reference parent, float width, float height)
        {
            this.document = document;
            dict = new GeneralDict("Page");
            dict.Add("Parent", parent);
            dict.Add("MediaBox", new List<float> { 0, 0, width, height });

            resources = new ResourceDict(document);
            resourcesRef = document.CreateReference(resources);
            dict.Add("Resources", resourcesRef);

            Contents = new ContentStream(resources);
            contentsRef = document.CreateReference(Contents);
            dict.Add("Contents", contentsRef);
        }

        void IWritable.Write(StreamWriter writer)
        {
            ((IWritable)dict).Write(writer);
        }
    }
}
