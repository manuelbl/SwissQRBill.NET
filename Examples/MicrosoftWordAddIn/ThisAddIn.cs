//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2022 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.SystemDrawing;
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
                    Language = Generator.Language.DE,
                    OutputSize = OutputSize.QrBillExtraSpace
                }
            };

            // create EMF file
            var path = Path.GetTempFileName();
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
            shape.LeftRelative = 0;
            shape.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionPage;
            shape.Top = (297 - 110) / 25.4f * 72;
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
