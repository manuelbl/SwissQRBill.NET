//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2025 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

package net.codecrete.qrbill.pdffonts;

import org.apache.pdfbox.cos.COSArray;
import org.apache.pdfbox.cos.COSBoolean;
import org.apache.pdfbox.cos.COSDictionary;
import org.apache.pdfbox.cos.COSDocument;
import org.apache.pdfbox.cos.COSFloat;
import org.apache.pdfbox.cos.COSInteger;
import org.apache.pdfbox.cos.COSName;
import org.apache.pdfbox.cos.COSNull;
import org.apache.pdfbox.cos.COSStream;
import org.apache.pdfbox.cos.COSString;
import org.apache.pdfbox.cos.ICOSVisitor;
import org.apache.pdfbox.io.IOUtils;

import java.io.ByteArrayOutputStream;
import java.io.CharArrayWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayDeque;
import java.util.Deque;

public class CsharpCodeWriter implements ICOSVisitor {

    private static final String INDENT = "            ";

    private final String fontName;
    private String propertyName;

    private final PrintWriter targetPrinter;
    private PrintWriter printer;
    private CharArrayWriter writer;
    private int arrayNesting = 0;

    private final Deque<PrintWriter> printerStack = new ArrayDeque<>();
    private final Deque<CharArrayWriter> writerStack = new ArrayDeque<>();
    private final Deque<Integer> arrayNestingStack = new ArrayDeque<>();

    public CsharpCodeWriter(PrintWriter targetPrinter, String fontName) {
        this.targetPrinter = targetPrinter;
        this.fontName = fontName;
        propertyName = "font";
        writer = new CharArrayWriter();
        printer = new PrintWriter(writer);

        printIntro();
    }

    private void printIntro() {
        targetPrinter.printf("""
                        // Generated code. Do not modify. See the associated PDFFonts projects.
                        internal static GeneralDict Create%s(Document document)
                        {
                """, fontName);
    }

    private void printOutro() {
        targetPrinter.printf("""
                            return font;
                        }
                
                """);
        targetPrinter.flush();
    }

    @Override
    public void visitFromDictionary(COSDictionary cosDictionary) throws IOException {
        var variableName = getVariableName(propertyName);

        startRecording();
        printer.printf("""
                %svar %s = new GeneralDict(new Dictionary<string, object>
                %s{
                """, INDENT, variableName, INDENT);

        printDictionary(cosDictionary);

        printer.printf("""
                %s});
                
                """, INDENT);

        printRecorded();

        printer.printf("document.CreateReference(%s)", variableName);
    }

    public void printDictionary(COSDictionary cosDictionary) throws IOException {
        for (var entry : cosDictionary.entrySet()) {
            propertyName = entry.getKey().getName();
            printer.printf("%s    { \"%s\", ", INDENT, propertyName);
            entry.getValue().accept(this);
            printer.printf(" },%n");
        }
    }

    @Override
    public void visitFromArray(COSArray cosArray) throws IOException {
        if (arrayNesting == 0) {
            printer.printf("new object[] {%n");
        }
        else
        {
            printer.print("new int[] { ");
        }

        for (var item : cosArray) {
            if (arrayNesting == 0) {
                printer.printf("%s        ", INDENT);
            }
            arrayNesting += 1;
            item.accept(this);
            arrayNesting -= 1;
            printer.printf(",%s", arrayNesting == 0 ? "\n" : " ");
        }

        if (arrayNesting == 0) {
            printer.printf("%s    ", INDENT);
        }
        printer.print('}');
    }

    @Override
    public void visitFromStream(COSStream cosStream) throws IOException {
        var streamPropertyName = propertyName;
        var variableName = getVariableName(propertyName);

        startRecording();

        printer.printf("""
                %svar %s = new StreamData(new Dictionary<string, object>
                %s{
                """, INDENT, variableName, INDENT);

        printDictionary(cosStream);

        var buffer = new ByteArrayOutputStream();
        IOUtils.copy(cosStream.createRawInputStream(), buffer);
        var bytes = buffer.toByteArray();

        printer.printf("%s}, ", INDENT);
        if (bytes.length < 256) {
            printer.print("new byte[] { ");
            for (var b : bytes) {
                printer.printf("0x%02X, ", b & 0xFF);
            }
            printer.print("}");
        } else {
            var resourceName = fontName + "-" + streamPropertyName + ".bin";
            Files.write(Path.of(resourceName), bytes);
            printer.printf("\"%s\"", resourceName);
        }
        printer.printf("""
                );
                
                
                """);
        printRecorded();

        printer.printf("document.CreateReference(%s)", variableName);
    }

    @Override
    public void visitFromFloat(COSFloat cosFloat) {
        printer.printf("%ff", cosFloat.floatValue());
    }

    @Override
    public void visitFromInt(COSInteger cosInteger) {
        printer.printf("%d", cosInteger.intValue());
    }

    @Override
    public void visitFromName(COSName cosName) {
        printer.printf("new Name(\"%s\")", cosName.getName());
    }

    @Override
    public void visitFromString(COSString cosString) {
        printer.printf("\"%s\"", cosString.getString());
    }

    @Override
    public void visitFromBoolean(COSBoolean cosBoolean) {
        assert false; // not used
    }

    @Override
    public void visitFromDocument(COSDocument cosDocument) {
        assert false; // not used
    }

    @Override
    public void visitFromNull(COSNull cosNull) {
        assert false; // not used
    }

    private void startRecording() {
        printer.flush();
        printerStack.push(printer);
        writerStack.push(writer);
        arrayNestingStack.push(arrayNesting);
        writer = new CharArrayWriter();
        printer = new PrintWriter(writer);
        arrayNesting = 0;
    }

    private void printRecorded() {
        printer.close();
        targetPrinter.write(writer.toCharArray());
        printer = printerStack.pop();
        writer = writerStack.pop();
        arrayNesting = arrayNestingStack.pop();

        if (writerStack.isEmpty())
            printOutro();
    }

    private static String getVariableName(String propertyName) {
        return Character.toLowerCase(propertyName.charAt(0)) + propertyName.substring(1);
    }
}
