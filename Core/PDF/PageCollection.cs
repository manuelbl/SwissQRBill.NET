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
        private readonly List<Reference> _pages;
        private readonly Document _document;

        internal PageCollection(Document document)
        {
            _pages = new List<Reference>();
            _document = document;
        }

        internal void Add(object node)
        {
            Reference pageRef = _document.CreateReference(node);
            _pages.Add(pageRef);
        }

        void IWritable.Write(StreamWriter writer)
        {
            GeneralDict dict = new GeneralDict("Pages");
            dict.Add("Count", _pages.Count);
            dict.Add("Kids", _pages);
            ((IWritable)dict).Write(writer);
        }
    }
}
