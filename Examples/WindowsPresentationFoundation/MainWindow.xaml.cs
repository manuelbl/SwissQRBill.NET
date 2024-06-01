//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using System.Windows;

namespace Codecrete.SwissQRBill.Examples.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bill bill;

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

            billImage.Source = QrBillImage.CreateImage(bill);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            QrBillPrinting.Print(bill);
        }
    }
}
