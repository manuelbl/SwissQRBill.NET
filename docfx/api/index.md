# Swiss QR Bill for .NET Reference Documentation

## Reference Documentation

**[Codecrete.SwissQRBill.Generator.QRBill](xref:Codecrete.SwissQRBill.Generator.QRBill)**: Generates Swiss QR bill (receipt and payment part). Also validates the bill data and encode and decode the text embedded in the QR code.

**[Codecrete.SwissQRBill.Generator.Bill](xref:Codecrete.SwissQRBill.Generator.Bill)**: QR bill data as input for generation or output from decoding

## Namespaces

- [Codecrete.SwissQRBill.Generator](xref:Codecrete.SwissQRBill.Generator) (platform-independent core, part of Codecrete.SwissQRBill.Core NuGet package)
- [Codecrete.SwissQRBill.Generator.Canvas](xref:Codecrete.SwissQRBill.Generator.Canvas) (platform-independent graphics generation, part of Codecrete.SwissQRBill.Core NuGet package)
- [Codecrete.SwissQRBill.Generator.PDF](xref:Codecrete.SwissQRBill.Generator.PDF) (minimal PDF generator, part of Codecrete.SwissQRBill.Core NuGet package)
- [Codecrete.SwissQRBill.PixelCanvas](xref:Codecrete.SwissQRBill.PixelCanvas) (platform-independent, Skia-based PNG generation, part of Codecrete.SwissQRBill.Generator NuGet package)
- [Codecrete.SwissQRBill.Windows](xref:Codecrete.SwissQRBill.Windows) (Windows-specific PNG and metafile generation, part of Codecrete.SwissQRBill.Windows NuGet package)

## Requirements

Swiss QR Bill for .NET requires .NET Standard 2.0 or higher, i.e. any of:

- .NET Core 2.0 or higher
- .NET Framework 4.6.1 or higher
- Mono 5.4 or higher
- Universal Windows Platform 10.0.16299 or higher
- Xamarin
- The [`Codecrete.SwissQRBill.Windows` classes](xref:Codecrete.SwissQRBill.Windows) are Windows specific and run on Windows only
