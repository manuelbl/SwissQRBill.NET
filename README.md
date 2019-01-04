# Swiss QR Bill for .NET

Open-source .NET library to generate Swiss QR bills (jointly developed with the [Java version](https://github.com/manuelbl/SwissQRBill)).

Try it yourself and [create a QR bill](https://www.codecrete.net/qrbill). 

## Introduction

The Swiss QR bill is the new QR code based payment format that will replace the current payment slip starting at 30 June, 2020. The new payment slip will in most cases be sent electronically. But it can still be printed at the bottom of an invoice or added to the invoice on a separate sheet. The payer scans the QR code with his/her mobile banking app to initiate the payment. The payment just needs to be confirmed.

The invoicing party can easily synchronize the received payment with the accounts-receivable accounting as the payment comes with a full set of data including the reference number used on the invoice. The Swiss QR bill is convenient for the payer and payee.

![QR Bill](https://raw.githubusercontent.com/wiki/manuelbl/SwissQRBill/images/qr-invoice-e1.svg?sanitize=true)

*More [examples](https://github.com/manuelbl/SwissQRBill/wiki/Swiss-QR-Invoice-Examples) can be found in the [Wiki](https://github.com/manuelbl/SwissQRBill/wiki)*

## Features

The Swiss QR bill library:

- generates PDF, SVG and PNG files
- generates payment slip (105mm by 210mm), A4 sheets or QR code only
- multilingual: German, French, Italian, English
- validates the invoice data and provides detailed validation information
- parses the invoice data embedded in the QR code
- is easy to use (see example below)
- is small and fast
- is free – even for commecial use (MIT License)
- is built for .NET Standard 2.0, i.e. it runs with .NET Core 2.0 or higher, .NET Framework 4.6.1 or higher, Mono 5.4 or higher, Universal Windows Platform 10.0.16299 or higher, Xamarin etc.
- has a single dependency (QRCoder)
- will be available as a NuGet package soon

## Getting started

To be added soon...

## API Documention

See DocFX [API Documentation](https://codecrete.net/SwissQRBill.NET/api/index.html)

## More information

This library is the .NET version of Swiss QR Bill. There is also a [Java version](https://github.com/manuelbl/SwissQRBill) with the same features. More information about both libraries can be found in the [Wiki](https://github.com/manuelbl/SwissQRBill/wiki).

## Other programming languages

If you are looking for a library for yet another programming language or for a library with professional services, you might want to check out [Services & Tools](https://www.moneytoday.ch/iso20022/movers-shakers/software-hersteller/services-tools/) on [MoneyToday.ch](https://www.moneytoday.ch).
