//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2025 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

package net.codecrete.qrbill.pdffonts;

import org.apache.fontbox.ttf.gsub.GsubWorkerFactory;
import org.apache.pdfbox.pdmodel.font.PDType0Font;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.List;
import java.util.Set;

import static java.lang.String.format;

/**
 * Creates a font subst.
 */
public class Subsetter {

    private static final String INDENT = "            ";

    private final PDType0Font font;
    private final GsubWorkerFactory gsubWorkerFactory = new GsubWorkerFactory();
    private final List<GlyphRange> glyphRanges = new ArrayList<>();


    public Subsetter(PDType0Font font) {
        this.font = font;
    }

    public void addCharacters(int firstChar, int lastChar) {
        var cmapLookup = font.getCmapLookup();
        var gsubData = font.getGsubData();
        var gsubWorker = gsubWorkerFactory.getGsubWorker(font.getCmapLookup(), gsubData);

        var glyphArray = new int[lastChar - firstChar + 1];
        for (var codePoint = firstChar; codePoint <= lastChar; codePoint++) {
            var glphyId = cmapLookup.getGlyphId(codePoint);

            var glyphIds = gsubWorker.applyTransforms(List.of(glphyId));
            if (glyphIds.size() != 1)
                throw new IllegalStateException(format("Unsupported font: it requires multiple glyphs for character U+%04x", codePoint));

            var bytes = font.encodeGlyphId(glyphIds.getFirst());
            if (bytes.length != 2)
                throw new IllegalStateException(format("Unsupported font: glyph is not encoded into 2 bytes (character U+%04x", codePoint));

            font.addGlyphsToSubset(Set.of(glyphIds.getFirst()));
            font.addToSubset(codePoint);

            glyphArray[codePoint - firstChar] = glyphIds.getFirst();
        }

        glyphRanges.add(new GlyphRange(firstChar, lastChar, glyphArray));
    }

    public void subset(PrintWriter printer, String fontName) throws IOException {
        font.subset();
        font.getCOSObject().accept(new CsharpCodeWriter(printer, fontName));
    }

    public void printCode(PrintWriter printer, String fontName) {
        printGlyphIndexes(printer, fontName);
        printIntro(printer, fontName);

        for (int i = 0; i < glyphRanges.size(); i++) {
            var firstCodePoint = glyphRanges.get(i).first;
            var lastCodePoint = glyphRanges.get(i).last;
            printer.printf("""
                    %s%sif (ch >= 0x%04x && ch <= 0x%04x)
                    %s{
                    %s    glyphIndexArray = GlyphIndexes%s_%04x_%04x;
                    %s    glyphIndex = ch - 0x%04x;
                    %s}
                    """, INDENT, i > 0 ? "else " : "", firstCodePoint, lastCodePoint, INDENT,
                    INDENT, fontName, firstCodePoint, lastCodePoint, INDENT, firstCodePoint, INDENT);
        }

        printer.printf("""
                %selse
                %s{
                %s    throw new ArgumentException($"Character U+{(int)ch:X4} is not part of the font %s (text: \\"{text}\\")");
                %s}
                """, INDENT, INDENT, INDENT, fontName, INDENT);

        printOutro(printer);
        printer.flush();
    }

    private void printGlyphIndexes(PrintWriter printer, String fontName) {
        printer.printf("        // Generated code. Do not modify. See the associated PDFFonts projects.%n");
        for (var range : glyphRanges) {
            var firstCodePoint = range.first;
            var lastCodePoint = range.last;
            printer.printf("        internal static readonly char[] GlyphIndexes%s_%04x_%04x = new char[] { ", fontName, firstCodePoint, lastCodePoint);
            for (var glyphId : range.glyphIds) {
                printer.printf("'\\x%04X', ", glyphId);
            }
            printer.printf("};%n");
        }
        printer.println();
    }

    private void printIntro(PrintWriter printer, String fontName) {
        printer.printf("""
                        // Generated code. Do not modify. See the associated PDFFonts projects.
                        internal static byte[] EncodeText%s(string text)
                        {
                            var bytes = new byte[text.Length * 2];
                            for (var i = 0; i < text.Length; i++)
                            {
                                var ch = text[i];
                                char[] glyphIndexArray;
                                int glyphIndex;
                """, fontName);
    }

    private void printOutro(PrintWriter printer) {
        printer.print("""
                                bytes[i * 2] = (byte)(glyphIndexArray[glyphIndex] >> 8);
                                bytes[i * 2 + 1] = (byte)glyphIndexArray[glyphIndex];
                            }
                            return bytes;
                        }
                
                
                """);
    }

    @SuppressWarnings("java:S6218")
    record GlyphRange(int first, int last, int[] glyphIds) {
    }
}
