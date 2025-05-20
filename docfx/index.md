# Swiss QR Bill for .NET

Open-source .NET library to generate Swiss QR bills (jointly developed with the [Java version](https://github.com/manuelbl/SwissQRBill)).

Try it yourself and [create a QR bill](https://www.codecrete.net/qrbill). 

## Features

The Swiss QR bill library:

- generates QR bills as PDF, SVG, PNG and EMF files
- generates payment slips (210mm by 105mm), payment part (148mm by 105mm), A4 sheets or QR code only
- is multilingual: German, French, Italian, English, Romansh
- validates the invoice data and provides detailed validation information
- adds or retrieves structured bill information (according to Swico S1)
- parses the invoice data embedded in the QR code
- is easy to use (see example below)
- is small and fast
- is free – even for commercial use (MIT License)
- is built for .NET Standard 2.0, i.e. it runs with .NET Core 2.0 or higher, .NET Framework 4.6.1 or higher, Mono 5.4 or higher, Universal Windows Platform 10.0.16299 or higher, Xamarin etc.
- core library is light-weight and has a single dependency: `Net.Codecrete.QrCodeGenerator`
- enhanced version uses SkiaSharp for PNG file generation
- Windows version uses `System.Drawing` for generating PNG and EMF files
- available as a NuGet packages


## Getting Started

Find all the relevant information in the [Swiss QR Bill Wiki](https://github.com/manuelbl/SwissQRBill/wiki).

## .NET API Documention

[Swiss QR Bill .NET Reference Documentation](api/index.md)
