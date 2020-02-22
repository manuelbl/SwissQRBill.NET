//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2020 Robert Hegner
// Copyright (c) 2020 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;

namespace PDFsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var doc = new PdfDocument())
            {
                var page = doc.AddPage();
                using (var graphic = XGraphics.FromPdfPage(page, XGraphicsUnit.Millimeter))
                {
                    graphic.DrawString("Test Invoice", new XFont("Arial", 20.0), XBrushes.Black, 20.0, 150.0);
                }

                using (var canvas = new PdfSharpCanvas(page, "Arial"))
                {
                    QRBill.Draw(Bill, canvas);
                }
                doc.Save("Output.pdf");
            }

            File.WriteAllBytes("OutputOriginal.pdf", QRBill.Generate(Bill));
        }

        static readonly Bill Bill = new Bill
		{
			// creditor data
			Account = "CH9600781622484102000",
			Creditor = new Address
			{
				Name = "Seemannschaft Rapperswil",
				AddressLine2 = "8640 Rapperswil",
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
	}
}
