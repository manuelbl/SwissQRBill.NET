//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2019 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.IO;
using Codecrete.SwissQRBill.Generator;


namespace Codecrete.SwissQRBill.Examples.IText7
{
    /// <summary>
    /// Sample program showing how to use the IText7Canvas for adding QR bills
    /// to existing PDF documents.
    /// <para>
    /// Note that the iText7 PDF library requires a license for commercial use.</para>
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Generate QR bill
            string path = "qrbill.pdf";
            Bill bill = CreateBillData();
            using (IText7Canvas canvas = new IText7Canvas(path, 210, 297))
            {
                QRBill.Draw(bill, canvas);
            }

            Console.WriteLine($"QR bill saved at {Path.GetFullPath(path)}");
        }

        private static Bill CreateBillData()
        {
            Bill bill = new Bill
            {
                Format = new BillFormat
                {
                    Language = Language.DE,
                    OutputSize = OutputSize.A4PortraitSheet
                },
                Account = "CH48 0900 0000 8575 7337 2",
                Creditor = new Address
                {
                    Name = "Omnia Trading AG",
                    AddressLine1 = "Allmendweg 30",
                    AddressLine2 = "4528 Zuchwil",
                    CountryCode = "CH"
                },
                Amount = 1756.05m,
                Currency = "CHF",
                Debtor = new Address
                {
                    Name = "Machina Futura AG",
                    AddressLine1 = "Alte Fabrik 3A",
                    AddressLine2 = "8400 Winterthur",
                    CountryCode = "CH"
                },
                UnstructuredMessage = "Auftrag 2830188 / Rechnung 2021007834"
            };

            bill.CreateAndSetCreditorReference("2021007834");
            return bill;
        }
    }
}
