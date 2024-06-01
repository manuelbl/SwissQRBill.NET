//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Windows;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Codecrete.SwissQRBill.Examples.WindowsForms
{
    public partial class Form1 : Form
    {
        private readonly Bill bill;

        public Form1()
        {
            InitializeComponent();

            // create bill data
            bill = new Bill
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
                UnstructuredMessage = "Abonnement für 2020"
            };

            // set bill data for QR bill control
            qrBillControl1.Bill = bill;
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
                pd.Print();
                MessageBox.Show("QR bill successfully printed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            using Font font = new Font("Arial", 18, FontStyle.Bold);
            e.Graphics.DrawString("Swiss QR Bill", font, Brushes.Black, 30, 80, new StringFormat());

            PrintQRBill(e);
        }

        private void PrintQRBill(PrintPageEventArgs e)
        {
            // ouput size for QR bill: 210 x 297mm
            bill.Format.OutputSize = OutputSize.A4PortraitSheet;

            // print QR bill at bottom of page
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;            
            using SystemDrawingCanvas canvas = new SystemDrawingCanvas(e.Graphics, 0, 297, 1, "Arial");
            QRBill.Draw(bill, canvas);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            using Metafile metafile = CreateMetafile();
            using Bitmap bitmap = CreateBitmap();
            ClipboardMetafileHelper.PutOnClipboard(this.Handle, metafile, bitmap);
        }

        private Bitmap CreateBitmap()
        {
            // ouput size for QR bill only: 210 x 110mm
            bill.Format.OutputSize = OutputSize.QrBillExtraSpace;
            const int dpi = 192;
            using BitmapCanvas canvas = new BitmapCanvas((int)Math.Round(210 / 25.4 * dpi),
                (int)Math.Round(110 / 25.4 * dpi), dpi, "Arial");
            QRBill.Draw(bill, canvas);
            return canvas.ToBitmap();
        }

        private Metafile CreateMetafile()
        {
            // ouput size for QR bill only: 210 x 110mm
            bill.Format.OutputSize = OutputSize.QrBillExtraSpace;
            using MetafileCanvas canvas = new MetafileCanvas(210, 110, "Arial");
            QRBill.Draw(bill, canvas);
            return canvas.ToMetafile();
        }
    }
}

