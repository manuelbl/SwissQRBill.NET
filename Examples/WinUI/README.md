# Sample project for WinUI applications

This sample project shows how to use the SwissQRBill.NET library in a WinUI application. It demonstrates how to display a QR bill and how to print it, using the [Win2D API](https://github.com/microsoft/Win2D).

- *Display*: For display, a [CanvasControl](https://microsoft.github.io/Win2D/WinUI3/html/T_Microsoft_Graphics_Canvas_UI_Xaml_CanvasControl.htm) is used. The drawing is triggered by the [Draw event](https://microsoft.github.io/Win2D/WinUI3/html/E_Microsoft_Graphics_Canvas_UI_Xaml_CanvasControl_Draw.htm).

- *Printing*: For printing, Win2D's [CanvasPrintDocument](https://microsoft.github.io/Win2D/WinUI3/html/T_Microsoft_Graphics_Canvas_Printing_CanvasPrintDocument.htm) and .NET's [PrintManager](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.printing.printmanager?view=winrt-22621) class are used.

The QR bill drawing is common to both output media. The code can be found in [`QrBillImage.cs`](WinUI/QrBillImage.cs).

