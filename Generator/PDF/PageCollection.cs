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
    internal class PageCollection : IWritable
    {
        private readonly List<Reference> pages;
        private readonly Document document;

        internal PageCollection(Document document)
        {
            pages = new List<Reference>();
            this.document = document;
        }

        internal void Add(object node)
        {
            Reference pageRef = document.CreateReference(node);
            pages.Add(pageRef);
        }

        void IWritable.Write(StreamWriter writer)
        {
            GeneralDict dict = new GeneralDict("Pages");
            dict.Add("Count", pages.Count);
            dict.Add("Kids", pages);
            ((IWritable)dict).Write(writer);
        }
    }
}
