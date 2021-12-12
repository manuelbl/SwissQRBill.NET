# Example for *Windows Forms* and `System.Drawing`

This example for Windows show various ways of working with QR bills. It is useful Windows Forms applications and other .NET applications running on Windows. It heavily depends on the `System.Drawing` classes, which have become a Windows only technology startring with .NET 6.

It is recommend to use this code example with the smaller `Codecrete.SwissQRBill.Core` NuGet package / assembly instead of the bigger one called `Codecrete.SwissQRBill.Generator`.

Useful classes are:

- [PngCanvas.cs](SystemDrawing/PngCanvas.cs) is a canvas class generating PNG files. It is a `System.Drawing`-based alternative for the *SkiaSharp*-based PNG generation in `Codecrete.SwissQRBill.Generator`. To simplify it's use, use the `PngCanvasFactory`.
- [PngCanvasFactory.cs](SystemDrawing/PngCanvasFactory.cs) is a factory for `PngCanvas`. Call `PngCanvasFactory.Register()` to enable the PNG generation without explicitly creating canvases.
- [BitmapCanvas.cs](SystemDrawing/BitmapCanvas.cs) is a canvas class for generating `System.Drawing.Bitmap` instances from QR bills.
- [MetafileCanvas.cs](SystemDrawing/MetafileCanvas.cs) is a canvas class for generating metafiles from QR bills.
- [QrBillControl.cs](QrBillControl.cs) is a Windows Forms compatible control class for display a QR bill. It automatically adapts to the size of the control.

The sample Windows Forms application shows how to display a QR bill, how to print it and to copy it to the clipboard (as both a resolution-independent metafile and a bitmap).
