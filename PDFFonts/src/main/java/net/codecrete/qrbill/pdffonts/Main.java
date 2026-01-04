//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2025 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

package net.codecrete.qrbill.pdffonts;

import org.apache.pdfbox.pdmodel.PDDocument;
import org.apache.pdfbox.pdmodel.PDDocumentInformation;
import org.apache.pdfbox.pdmodel.PDPage;
import org.apache.pdfbox.pdmodel.PDPageContentStream;
import org.apache.pdfbox.pdmodel.common.PDRectangle;
import org.apache.pdfbox.pdmodel.font.PDFont;
import org.apache.pdfbox.pdmodel.font.PDType0Font;

import java.io.IOException;
import java.io.InputStream;
import java.io.PrintWriter;

/**
 * Creates font subsets, C# code and a reference PDF document.
 */
public class Main {
    private Main() {
    }

    static void main() throws IOException {
        createSubsets();
        createReferenceDocument();
    }

    private static void createSubsets() throws IOException {
        var writer = new PrintWriter(System.out);
        try (PDDocument doc = new PDDocument()) {
            var fonts = loadFonts(doc);

            var regular = new Subsetter(fonts[0]);
            regular.addCharacters(0x0020, 0x007E);
            regular.addCharacters(0x00A0, 0x00FE);
            regular.addCharacters(0x0100, 0x017F);
            regular.addCharacters(0x0218, 0x021C);
            regular.addCharacters(0x20AC, 0x20AC);
            regular.subset(writer, "LiberationSansRegular");
            regular.printCode(writer, "LiberationSansRegular");

            var bold = new Subsetter(fonts[1]);
            bold.addCharacters('A', 'Z');
            bold.addCharacters('a', 'z');
            bold.addCharacters(0x0020, 0x0020); // space
            bold.addCharacters(0x002F, 0x002F); // slash
            bold.addCharacters(0x00E0, 0x00E0); // à
            bold.addCharacters(0x00E4, 0x00E4); // ä
            bold.addCharacters(0x00E7, 0x00E9); // çèé
            bold.addCharacters(0x00F4, 0x00F4); // ô
            bold.addCharacters(0x00F6, 0x00F6); // ö
            bold.addCharacters(0x00FC, 0x00FC); // ü
            bold.subset(writer, "LiberationSansBold");
            bold.printCode(writer, "LiberationSansBold");
        }
    }

    private static PDType0Font[] loadFonts(PDDocument doc) throws IOException {
        try (InputStream fontStreamRegular = PDDocument.class.getResourceAsStream("/LiberationSans-Regular.ttf");
             InputStream fontStreamBold = PDDocument.class.getResourceAsStream("/LiberationSans-Bold.ttf")) {
            assert fontStreamRegular != null;
            assert fontStreamBold != null;
            PDType0Font fontRegular = PDType0Font.load(doc, fontStreamRegular, true);
            PDType0Font fontBold = PDType0Font.load(doc, fontStreamBold, true);
            return new PDType0Font[]{fontRegular, fontBold};
        }
    }

    private static void createReferenceDocument() throws IOException {
        try (PDDocument doc = new PDDocument()) {
            var info = new PDDocumentInformation();
            info.setTitle("Font Embedding");
            doc.setDocumentInformation(info);
            var fonts = loadFonts(doc);
            PDFont fontRegular = fonts[0];
            PDFont fontBold = fonts[1];

            PDPage page = new PDPage(PDRectangle.A4);
            doc.addPage(page);

            try (PDPageContentStream contentStream = new PDPageContentStream(doc,
                    page, PDPageContentStream.AppendMode.OVERWRITE, false)) {

                contentStream.setFont(fontRegular, 20.0f);
                contentStream.beginText();
                contentStream.newLineAtOffset(60, 700);
                drawTextLine(contentStream, 0x0020, 0x003F);
                drawTextLine(contentStream, 0x0040, 0x005F);
                drawTextLine(contentStream, 0x0060, 0x007E);
                drawTextLine(contentStream, 0x00A0, 0x00BF);
                drawTextLine(contentStream, 0x00C0, 0x00DF);
                drawTextLine(contentStream, 0x00E0, 0x00FE);
                drawTextLine(contentStream, 0x0100, 0x011F);
                drawTextLine(contentStream, 0x0120, 0x013F);
                drawTextLine(contentStream, 0x0140, 0x015F);
                drawTextLine(contentStream, 0x0160, 0x017F);
                drawTextLine(contentStream, 0x0218, 0x021C);
                drawTextLine(contentStream, 0x20AC, 0x20AC);
                contentStream.endText();

                contentStream.setFont(fontBold, 20.0f);
                contentStream.beginText();
                contentStream.newLineAtOffset(60, 400);
                drawTextLine(contentStream, 'A', 'Z');
                drawTextLine(contentStream, 'a', 'z');
                contentStream.newLineAtOffset(0, -24f);
                contentStream.showText("äöüàçéèô");
                contentStream.endText();
            }

            doc.save("text.pdf");
        }
    }

    private static void drawTextLine(PDPageContentStream contentStream, int firstChar, int lastChar) throws IOException {
        contentStream.newLineAtOffset(0, -24f);
        var chars = new char[lastChar - firstChar + 1];
        for (int i = firstChar; i <= lastChar; i++)
            chars[i - firstChar] = (char) i;
        contentStream.showText(new String(chars));
    }
}
