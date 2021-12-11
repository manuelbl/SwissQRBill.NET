//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Codecrete.SwissQRBill.Examples.Wpf
{
    /// <summary>
    /// Sample class for printing a document with a Swiss QR bill
    /// </summary>
    internal class QrBillPrinting
    {
        /// <summary>
        /// Print sample document with the specified QR bill
        /// </summary>
        /// <param name="bill">QR bill to print</param>
        internal static void Print(Bill bill)
        {
            // show print dialog
            var pd = new PrintDialog();
            pd.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            pd.PrintTicket.PageOrientation = PageOrientation.Portrait;
            if (pd.ShowDialog() != true)
                return;

            // create page
            var pageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);
            var page = new FixedPage
            {
                Width = pageSize.Width,
                Height = pageSize.Height
            };

            // add title
            var text = new TextBlock
            {
                Text = "Swiss QR Bill",
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(20 / 25.4 * 96)
            };
            page.Children.Add(text);

            // add QR bill to the bottom
            var qrBill = new Image
            {
                Source = QrBillImage.CreateImage(bill)
            };
            FixedPage.SetBottom(qrBill, 0);
            page.Children.Add(qrBill);

            // create document
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;
            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            document.Pages.Add(pageContent);

            // print document
            pd.PrintDocument(document.DocumentPaginator, "Invoice");
        }
    }
}
