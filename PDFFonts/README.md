# PDF Fonts

This Java program creates the PDF object required to embed a subset
of the fonts Liberation Sans Regular and Liberation Sans Bold into
PDF files.

It writes C# code to standard out and creates several binary files in
the project directory. Both the C# code and the binary file need to be
copied to the *Core* .NET project. 


## Subsets

All PDF files will use the same subset. They do not determine the actually
used subset. Instead, the subset consists of:

### Liberation Sans Regular

- Basic Latin (Unicode codepoints U+0020 to U+007E)
- Latin1 Supplement (Unicode codepoints U+00A0 to U+00FF)
- Latin Extended A (Unicode codepoints U+0100 to U+017F)
- Additional characters: Unicode codepoint U+0218 to U+021B and U+20AC (euro sign)

These characters are the characters allowed for payments according to the
Swiss Payment Standard 2025

### Liberation Sans Bold

- Characters A to Z
- Characters a to z
- Characters äöüàçéèô
- Space and slash character

These characters are needed for the bold titles on the payment slip
incl. the names of alternative schemes.


## Limitations

This simplified approach only works for fonts using a single glyph for
each of the relevant characters. It does not support ligatures.


## Implementation

The program leverages Apache PDFBox for creating the subset.

The result is an COS object graph, which is converted to C# code and
binary snippets.
