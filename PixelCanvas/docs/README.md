# Swiss QR Bill for .NET

Open-source .NET library to generate Swiss QR bills. Try it yourself and [create a QR bill](https://www.codecrete.net/qrbill).

## Introduction

The Swiss QR bill is the new QR code based payment format that started on 30 June, 2020. The old payment slip will no longer be accepted after 30 September 2022.

The new payment slip will be sent electronically in most cases. But it can still be printed at the bottom of an invoice or added to the invoice on a separate sheet. The payer scans the QR code with his/her mobile banking app to initiate the payment. The payment just needs to be confirmed.

If the invoicing party adds structured bill information (VAT rates, payment conditions etc.) to the QR bill, posting in accounts payable can be automated. The invoicing party can also automate the accounts receivable processing as the payment includes all relevant data including a reference number. The Swiss QR bill is convenient for the payer and payee.

![QR Bill](https://raw.githubusercontent.com/wiki/manuelbl/SwissQRBill/images/qr-invoice-e1.svg?sanitize=true)

*More [examples](https://github.com/manuelbl/SwissQRBill/wiki/Swiss-QR-Invoice-Examples) can be found in the [Wiki](https://github.com/manuelbl/SwissQRBill/wiki)*

## Features

The Swiss QR bill library:

- generates QR bills as PDF, SVG and PNG files (see home page for Windows-specific EMF)
- generates payment slips (105mm by 210mm), A4 sheets or QR codes
- multilingual: German, French, Italian, English, Romansh
- validates the invoice data and provides detailed validation information
- adds or retrieves structured bill information (according to Swico S1)
- parses the invoice data embedded in the QR code
- is easy to use (see example below)
- is small and fast
- is free – even for commercial use (MIT License)
- is built for .NET Standard 2.0, i.e. it runs with .NET Core 2.0 or higher, .NET Framework 4.6.1 or higher, Mono 5.4 or higher, Universal Windows Platform 10.0.16299 or higher, Xamarin etc.
- this library version uses SkiaSharp for platform-independent PNG file generation
- see [home page](https://github.com/manuelbl/SwissQRBill.NET) for more examples and other library versions including EMF generation


## Getting started

1. Create a new Visual Studio project for .NET Core 3.x (*File > New > Project...* / *Visual C# > .NET Core > Console App (.NET Core)*)

2. Add the library via NuGet:

   Either via *Project > Manage NuGet Packages...* / *Browse* / search for *qrbill* / *Install*
   
   Or by running a command in the Package Manager Console

```
Install-Package Codecrete.SwissQRBill.Generator -Version 3.1.1
```

3. Add the code:

```c#
using Codecrete.SwissQRBill.Generator;
using System;
using System.IO;

namespace Codecrete.SwissQRBill.Examples.Basic
{
    class Program
    {
        static void Main(string[] args)
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

            // Generate QR bill
            byte[] svg = QRBill.Generate(bill);

            // Save generated SVG file
            const string path = "qrbill.svg";
            File.WriteAllBytes(path, svg);
            Console.WriteLine($"QR bill saved at { Path.GetFullPath(path) }");
        }
    }
}
```

4. Run it

## API documention

See DocFX [API Documentation](https://codecrete.net/SwissQRBill.NET/api/index.html)

## Further topics

 - [PNG generation](https://github.com/manuelbl/SwissQRBill.NET#png-generation)
 - [More PDF generation options](https://github.com/manuelbl/SwissQRBill.NET#pdf-generation)
 - [Code examples](https://github.com/manuelbl/SwissQRBill.NET#code-examples)
 - [Wiki](https://github.com/manuelbl/SwissQRBill/wiki)
 - [Java version](https://github.com/manuelbl/SwissQRBill)
