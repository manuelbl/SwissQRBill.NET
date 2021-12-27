# Sample Code for PDFsharp

[iText](https://itextpdf.com/en/products/itext-7/itext-7-core) is one of the PDF libraries frequently used in .NET project. This sample code demonstrates how to generate PDF documents with QR bills using this library.

The *SwissQRBill.NET* comes with its own light-weight PDF generator. However, if an existing project already uses *iText*, this code can be helpful. Using *iText* can also be helpful if the QR bill needs to be added to an existing PDF file.

Note that you might need a commercial license for using *iText*.

Two cases are demonstrated:

- Copying an existing PDF file and adding the QR bill to it
- Creating a new PDF file with a QR bill
