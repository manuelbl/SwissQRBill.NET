//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Robert Hegner
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Reflection;

namespace PDFsharp
{
    /// <summary>
    /// Console application demonstrating the use of SwissQRBill.NET with PDFsharp
    /// </summary>
    class Program
    {
        /// <summary>
        /// Creates two PDF documents with a QR bill.
        /// <para>
        /// The first document consist of static text and the QR bill. It is
        /// entirely created by this program.
        /// </para>
        /// <para>
        /// The second document mainly consist of content from a template. The content is copied,
        /// and the QR bill is added on the last page.
        /// </para>
        /// </summary>
        static void Main()
        {

            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            // create new PDF file with QR bill
            using (var doc = new PdfDocument())
            {
                var page = doc.AddPage();
                page.Size = PageSize.A4;

                // static text on page
                using (var graphic = XGraphics.FromPdfPage(page, XGraphicsUnit.Millimeter))
                {
                    graphic.DrawString("Test Invoice", new XFont("Arial", 20.0), XBrushes.Black, 20.0, 150.0);
                }

                // add QR bill
                using (var canvas = new PdfSharpCanvas(page, "Arial"))
                {
                    QRBill.Draw(Bill, canvas);
                }

                // save document
                doc.Save("Output1.pdf");
            }

            // append QR bill to existing PDF file (on last page)
            using (var templateDoc = PdfReader.Open(OpenInvoice("invoice.pdf"), PdfDocumentOpenMode.Import))
            using (var doc = new PdfDocument())
            {
                // copy all pages
                foreach (var page in templateDoc.Pages)
                {
                    doc.AddPage(page);
                }

                var lastPage = doc.Pages[doc.Pages.Count - 1];
                using (var canvas = new PdfSharpCanvas(lastPage, "Arial"))
                {
                    QRBill.Draw(Bill, canvas);
                }

                doc.Save("Output2.pdf");
            }
        }

        static readonly Bill Bill = new Bill
        {
            // creditor data
            Account = "CH9600781622484102000",
            Creditor = new Address
            {
                Name = "Seemannschaft Rapperswil",
                PostalCode = "8640",
                Town = "Rapperswil",
                CountryCode = "CH"
            },

            // payment data
            Amount = null,
            Currency = "CHF",

            // more payment data
            UnstructuredMessage = "Spende",

            // format
            Format = new BillFormat
            {
                Language = Language.DE,
                SeparatorType = SeparatorType.DashedLineWithScissors,
                OutputSize = OutputSize.A4PortraitSheet,
                GraphicsFormat = GraphicsFormat.PDF,
                FontFamily = "Arial"
            }
        };


        private static Stream OpenInvoice(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = assembly.GetManifestResourceStream(typeof(Program), $"Templates.{filename}");
            if (resourceStream == null)
            {
                throw new FileNotFoundException($"Resource not found: Templates/{filename}");
            }

            return resourceStream;
        }
    }
}
