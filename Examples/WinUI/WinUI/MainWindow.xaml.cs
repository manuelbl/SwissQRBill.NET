//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2022 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Printing;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics.Printing;

namespace Codecrete.SwissQRBill.Examples.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly Bill bill;
        private CanvasPrintDocument printDocument;

        public MainWindow()
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
                //Amount = 199.95m,
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

            Title = "Swiss QR Bill";
        }

        // Called to draw to QR bill in the UI
        private void DrawBillImage(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var session = args.DrawingSession;

            var scale = (float)Math.Min(sender.ActualWidth / 210, sender.ActualHeight / 105);
            var width = 210 * scale;
            var height = 105 * scale;
            var xOffset = ((float)sender.ActualWidth - width) / 2;
            var yOffset = ((float)sender.ActualHeight - height) / 2;

            QrBillImage.DrawQrBill(bill, session, xOffset, yOffset, scale);
        }


        // --- Printing ---

        // Called when the "Print" button is clicked
        private async void OnPrintClicked(object sender, RoutedEventArgs e)
        {
            // Dispose of all print document
            if (printDocument != null)
            {
                printDocument.Dispose();
            }

            // Create new print document
            printDocument = new CanvasPrintDocument();
            printDocument.Preview += OnPreview;
            printDocument.Print += OnPrint;

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var printManager = PrintManagerInterop.GetForWindow(hWnd);

            printManager.PrintTaskRequested += OnPrintTaskRequested;
            try
            {
                await PrintManagerInterop.ShowPrintUIForWindowAsync(hWnd);
            }
            finally
            {
                printManager.PrintTaskRequested -= OnPrintTaskRequested;
            }
        }

        private void OnPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var printTask = args.Request.CreatePrintTask("Swiss QR Bill", (createPrintTaskArgs) =>
            {
                createPrintTaskArgs.SetSource(printDocument);
            });

            // configure initial options
            printTask.Options.MediaSize = PrintMediaSize.IsoA4;
            printTask.Options.Orientation = PrintOrientation.Portrait;
        }

        private void OnPreview(CanvasPrintDocument sender, CanvasPreviewEventArgs args)
        {
            // draw preview
            DrawPage(args.DrawingSession);
        }

        private void OnPrint(CanvasPrintDocument sender, CanvasPrintEventArgs args)
        {
            // draw document (single page only)
            using var session = args.CreateDrawingSession();
            DrawPage(session);
        }

        private void DrawPage(CanvasDrawingSession session)
        {
            var dipToMm = 96f / 25.4f;

            var textFormat = new CanvasTextFormat()
            {
                FontFamily = "Arial",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                HorizontalAlignment = CanvasHorizontalAlignment.Left
            };
            session.DrawText("Swiss QR Bill", 5 * dipToMm, 12 * dipToMm, Colors.Black, textFormat);

            QrBillImage.DrawQrBillPage(bill, session, 0, 0, dipToMm);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (printDocument != null)
            {
                printDocument.Dispose();
                printDocument = null;
            }
        }
    }
}
