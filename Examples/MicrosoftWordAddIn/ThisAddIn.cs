//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2022 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Windows;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace Codecrete.SwissQRBill.WordAddIn
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        public void InsertQrBill()
        {
            // Setup bill data
            Bill bill = new Bill
            {
                // creditor data
                Account = "CH4431999123000889012",
                Creditor = new Address
                {
                    Name = "Robert Schneider AG",
                    Street = "Rue du Lac",
                    HouseNo = "1268/2/22",
                    PostalCode = "2501",
                    Town = "Biel",
                    CountryCode = "CH"
                },

                // payment data
                Amount = 199.95m,
                Currency = "CHF",

                // debtor data
                Debtor = new Address
                {
                    Name = "Pia-Maria Rutschmann-Schnyder",
                    Street = "Grosse Marktgasse",
                    HouseNo = "28",
                    PostalCode = "9400",
                    Town = "Rorschach",
                    CountryCode = "CH"
                },

                // more payment data
                Reference = "210000000003139471430009017",
                UnstructuredMessage = "Abonnement für 2020",


                // output format
                Format = new BillFormat
                {
                    Language = Generator.Language.DE,
                    OutputSize = OutputSize.QrBillExtraSpace
                }
            };

            // create EMF file
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (MetafileCanvas canvas = new MetafileCanvas(210, 110, "Arial"))
            {
                QRBill.Draw(bill, canvas);
                File.WriteAllBytes(path, canvas.ToByteArray());
            }

            // insert EMF file into document (at bottom of page)
            var shape = Application.ActiveDocument.Shapes.AddPicture(path,
                Left: 0, Top: (297 - 110) / 25.4f * 72,
                Width: 210 / 25.4f * 72, Height: 110 / 25.4f * 72);
            shape.RelativeHorizontalPosition = WdRelativeHorizontalPosition.wdRelativeHorizontalPositionPage;
            shape.Left = -999998;
            shape.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionPage;
            shape.Top = -999997;
            shape.Select(true);

            // delete temporary file
            File.Delete(path);
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
