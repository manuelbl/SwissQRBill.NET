//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System;
using System.IO;

namespace Codecrete.SwissQRBill.Examples.Basic
{
    /// <summary>
    /// Console application generating a QR bill.
    /// <para>
    /// The QR bill is saved in the working directory.
    /// The path to the file is printed to the console.
    /// </para>
    /// </summary>
    class Program
    {
        /// <summary>
        /// Generates a QR bill and saves it as a SVG file.
        /// </summary>
        static void Main()
        {
            // Setup bill data
            Bill bill = new Bill
            {
                // creditor data
                Account = "CH4431999123000889012",
                Creditor = new Address
                {
                    Name = "Robert Schneider AG",
                    AddressLine1 = "Rue du Lac 1268/2/22",
                    AddressLine2 = "2501 Biel",
                    CountryCode = "CH"
                },

                // payment data
                Amount = 199.95m,
                Currency = "CHF",

                // debtor data
                Debtor = new Address
                {
                    Name = "Pia-Maria Rutschmann-Schnyder",
                    AddressLine1 = "Grosse Marktgasse 28",
                    AddressLine2 = "9400 Rorschach",
                    CountryCode = "CH"
                },

                // more payment data
                Reference = "210000000003139471430009017",
                UnstructuredMessage = "Abonnement für 2020",

                // output format
                Format = new BillFormat
                {
                    Language = Language.DE,
                    GraphicsFormat = GraphicsFormat.SVG,
                    OutputSize = OutputSize.QrBillOnly
                }
            };

            // Generate QR bill as SVG
            byte[] svg = QRBill.Generate(bill);

            // Save generated SVG file
            const string path = "qrbill.svg";
            File.WriteAllBytes(path, svg);
            Console.WriteLine($"SVG QR bill saved at { Path.GetFullPath(path) }");

            // Generate QR bill as PNG
            bill.Format.GraphicsFormat = GraphicsFormat.PNG;
            byte[] png = QRBill.Generate(bill);

            // Save generated PNG file
            const string pngPath = "qrbill.png";
            File.WriteAllBytes(pngPath, png);
            Console.WriteLine($"PNG QR bill saved at { Path.GetFullPath(pngPath) }");
        }
    }
}
