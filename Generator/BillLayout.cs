//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2018 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator.Canvas;
using System;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Resources;
using System.Text;
using static Codecrete.SwissQRBill.Generator.Address;

namespace Codecrete.SwissQRBill.Generator
{
    /// <summary>
    /// Layouting and drawing of QR bill payment slip
    /// </summary>
    internal class BillLayout
    {
        private static readonly double PtToMm = 25.4 / 72;
        private static readonly double MmToPt = 72 / 25.4;
        private static readonly int FontSizeTitle = 11; // pt
        private static readonly int PpLabelPrefFontSize = 8; // pt
        private static readonly int PpTextPrefFontSize = 10; // pt
        private static readonly int PpTextMinFontSize = 8; // pt
        private static readonly int RcLabelPrefFontSize = 6; // pt
        private static readonly int RcTextPrefFontSize = 8; // pt
        private static readonly double SlipWidth = 210; // mm
        private static readonly double SlipHeight = 105; // mm
        private static readonly double PaymentPartWidth = 148; // mm
        private static readonly double ReceiptWidth = SlipWidth - PaymentPartWidth; // mm
        private static readonly double Margin = 5; // mm
        private static readonly double QrCodeSize = QRCode.SIZE; // 46 mm
        private static readonly double InfoSectionWidth = 81; // mm (must not be smaller than 65)
        private static readonly double CurrencyAmountBaseLine = 32; // mm (from to bottom)
        private static readonly double CurrencWidthPp = 15; // mm
        private static readonly double CurrencyWidthRc = 13; // mm
        private static readonly double AmountBoWidthPp = 40; // mm
        private static readonly double AmountBoxHeightPp = 15; // mm
        private static readonly double AmountBoxWidthRc = 30; // mm
        private static readonly double AmountBoxHeightRc = 10; // mm
        private static readonly double DebtorBoxHeightPp = 25; // mm (must not be smaller than 25)
        private static readonly double DebtorBoxHeightRc = 25; // mm (must not be smaller than 25)
        private static readonly double AlternativeSchemesHeight = 6; // mm
        private static readonly double InfoSectionMaxHeight = SlipHeight - AlternativeSchemesHeight - 3 * Margin;
        private static readonly double ReceiptMaxHeight = SlipHeight - CurrencyAmountBaseLine - 7 - 0.5 - Margin;
        private static readonly double LeadingPref = 0.2; // relative to font size
        private static readonly double PaddingPref = 0.5; // relative to font size
        private static readonly double PaddingMin = 0.2; // relative to font size
        private static readonly double EllipsisWidth = 0.3528; // mm * font size


        private Bill bill;
        private QRCode qrCode;
        private ICanvas graphics;
        private ResourceSet resourceSet;

        private string accountPayableTo;
        private string reference;
        private string additionalInfo;
        private string payableBy;
        private string amount;

        private string[] accountPayableToLines;
        private string[] additionalInfoLines;
        private string[] payableByLines;

        private double yPos;

        private int labelFontSize;
        private int textFontSize;
        private double labelLeading;
        private double textLeading;
        private double textBottomPadding;


        internal BillLayout(Bill bill, ICanvas graphics)
        {
            this.bill = bill;
            qrCode = new QRCode(bill);
            qrCode = new QRCode(bill);
            this.graphics = graphics;
            resourceSet = MultilingualText.GetResourceSet(bill.Format.Language);
        }

        internal void Draw()
        {
            PrepareText();

            // payment part

            labelFontSize = PpLabelPrefFontSize;
            textFontSize = PpTextPrefFontSize;

            while (true)
            {
                BreakLines(InfoSectionWidth);
                bool isTooTight = ComputePaymentPartLeading();
                if (!isTooTight || textFontSize == PpTextMinFontSize)
                {
                    break;
                }

                labelFontSize--;
                textFontSize--;
            }
            DrawPaymentPart();

            // receipt

            labelFontSize = RcLabelPrefFontSize;
            textFontSize = RcTextPrefFontSize;
            BreakLines(ReceiptWidth - 2 * Margin);
            ComputeReceiptLeading();
            DrawReceipt();

            // border
            DrawBorder();
        }

        private void DrawPaymentPart()
        {
            // QR code section
            qrCode.Draw(graphics, ReceiptWidth + Margin, SlipHeight - 17 - QrCodeSize);

            // "Payment part" title
            graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);
            yPos = SlipHeight - Margin - graphics.Ascender(FontSizeTitle);
            graphics.PutText(GetText(MultilingualText.KeyPaymentPart), 0,
                        yPos, FontSizeTitle, true);

            // currency
            yPos = CurrencyAmountBaseLine + graphics.Ascender(labelFontSize);
            DrawLabelAndText(MultilingualText.KeyCurrency, bill.Currency);

            // amount
            graphics.SetTransformation(ReceiptWidth + Margin + CurrencWidthPp, 0, 0, 1, 1);
            yPos = CurrencyAmountBaseLine + graphics.Ascender(labelFontSize);
            if (amount != null)
            {
                DrawLabelAndText(MultilingualText.KeyAmount, amount);
            }
            else
            {
                DrawLabel(MultilingualText.KeyAmount);
                DrawCorners(0, yPos - AmountBoxHeightPp, AmountBoWidthPp, AmountBoxHeightPp);
            }

            // information section
            graphics.SetTransformation(SlipWidth - InfoSectionWidth - Margin, 0, 0, 1, 1);
            yPos = SlipHeight - Margin;

            // account and creditor
            DrawLabelAndTextLines(MultilingualText.KeyAccountPayableTo, accountPayableToLines);

            // reference
            if (reference != null)
            {
                DrawLabelAndText(MultilingualText.KeyReference, reference);
            }

            // additional information
            if (additionalInfo != null)
            {
                DrawLabelAndTextLines(MultilingualText.KeyAdditionalInformation, additionalInfoLines);
            }

            // payable by
            if (payableBy != null)
            {
                DrawLabelAndTextLines(MultilingualText.KeyPayableBy, payableByLines);
            }
            else
            {
                DrawLabel(MultilingualText.KeyPayableByNameAddr);
                yPos -= DebtorBoxHeightPp;
                DrawCorners(0, yPos, InfoSectionWidth, DebtorBoxHeightPp);
            }

            // alternative schemes
            DrawAlternativeSchemes();
        }

        private void DrawReceipt()
        {
            // "Receipt" title
            graphics.SetTransformation(Margin, 0, 0, 1, 1);
            yPos = SlipHeight - Margin - graphics.Ascender(FontSizeTitle);
            graphics.PutText(GetText(MultilingualText.KeyReceipt), 0,
                        yPos, FontSizeTitle, true);

            // account and creditor
            yPos = SlipHeight - Margin - 7;
            DrawLabelAndTextLines(MultilingualText.KeyAccountPayableTo, accountPayableToLines);

            // reference
            if (reference != null)
            {
                DrawLabelAndText(MultilingualText.KeyReference, reference);
            }

            // payable by
            if (payableBy != null)
            {
                DrawLabelAndTextLines(MultilingualText.KeyPayableBy, payableByLines);
            }
            else
            {
                DrawLabel(MultilingualText.KeyPayableByNameAddr);
                yPos -= DebtorBoxHeightRc;
                DrawCorners(0, yPos, ReceiptWidth - 2 * Margin, DebtorBoxHeightRc);
            }

            // currency
            yPos = CurrencyAmountBaseLine + graphics.Ascender(labelFontSize);
            DrawLabelAndText(MultilingualText.KeyCurrency, bill.Currency);

            // amount
            graphics.SetTransformation(Margin + CurrencyWidthRc, 0, 0, 1, 1);
            yPos = CurrencyAmountBaseLine + graphics.Ascender(labelFontSize);
            if (amount != null)
            {
                DrawLabelAndText(MultilingualText.KeyAmount, amount);
            }
            else
            {
                DrawLabel(MultilingualText.KeyAmount);
                graphics.SetTransformation(0, 0, 0, 1, 1);
                DrawCorners(ReceiptWidth - Margin - AmountBoxWidthRc,
                        CurrencyAmountBaseLine + 2 - AmountBoxHeightRc,
                        AmountBoxWidthRc, AmountBoxHeightRc);
            }

            // acceptance point
            graphics.SetTransformation(0, 0, 0, 1, 1);
            string label = GetText(MultilingualText.KeyAcceptancePoint);
            double w = graphics.TextWidth(label, labelFontSize, true);
            graphics.PutText(label, ReceiptWidth - Margin - w, 21, labelFontSize, true);
        }

        private void DrawAlternativeSchemes()
        {
            if (bill.AlternativeSchemes == null || bill.AlternativeSchemes.Count == 0)
            {
                return;
            }

            graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);
            double y = 11 - graphics.Ascender(7);
            double maxWidth = PaymentPartWidth - 2 * Margin;

            foreach (AlternativeScheme scheme in bill.AlternativeSchemes)
            {
                string boldText = $"{scheme.Name}: ";
                double boldTextWidth = graphics.TextWidth(boldText, 7, true);
                graphics.PutText(boldText, 0, y, 7, true);

                string normalText = TrunacateText(scheme.Instruction, maxWidth - boldTextWidth, 7);
                graphics.PutText(normalText, boldTextWidth, y, 7, false);
                y -= graphics.LineHeight(7) * 1.2;
            }
        }

        private bool ComputePaymentPartLeading()
        {
            // The same line spacing (incl. leading) is used for the smaller label and text lines
            int numTextLines = 0;
            int numPaddings = 0;
            double height = 0;

            numTextLines += 1 + accountPayableToLines.Length;
            height -= graphics.Ascender(textFontSize) - graphics.Ascender(labelFontSize);
            if (reference != null)
            {
                numPaddings++;
                numTextLines += 2;
            }
            if (additionalInfo != null)
            {
                numPaddings++;
                numTextLines += 1 + additionalInfoLines.Length;
            }
            numPaddings++;
            if (payableBy != null)
            {
                numTextLines += 1 + payableByLines.Length;
            }
            else
            {
                numTextLines += 1;
                height += DebtorBoxHeightPp;
            }

            height += numTextLines * graphics.LineHeight(textFontSize);

            return ComputeLeading(height, InfoSectionMaxHeight, numTextLines, numPaddings);
        }

        private void ComputeReceiptLeading()
        {
            // The same line spacing (incl. leading) is used for the smaller label and text lines
            int numTextLines = 0;
            int numPaddings = 1;
            double height = 0;

            numTextLines += 1 + accountPayableToLines.Length;
            height -= graphics.Ascender(textFontSize) - graphics.Ascender(labelFontSize);
            if (reference != null)
            {
                numPaddings++;
                numTextLines += 2;
            }
            numPaddings++;
            if (payableBy != null)
            {
                numTextLines += 1 + payableByLines.Length;
            }
            else
            {
                numTextLines += 1;
                height += DebtorBoxHeightRc;
            }

            height += numTextLines * graphics.LineHeight(textFontSize);

            ComputeLeading(height, ReceiptMaxHeight, numTextLines, numPaddings);
        }

        private bool ComputeLeading(double height, double maxHeight, int numTextLines, int numPaddings)
        {
            bool isTooTight = false;
            textLeading = LeadingPref * textFontSize * PtToMm;
            if (height + numTextLines * textLeading > maxHeight)
            {
                isTooTight = true;
                if (height > maxHeight)
                {
                    textLeading = 0;
                    labelLeading = 0;
                }
                else
                {
                    textLeading = (maxHeight - height) / numTextLines;
                    labelLeading = textLeading + graphics.Descender(textFontSize) - graphics.Descender(labelFontSize);
                }
            }
            else
            {
                labelLeading = textLeading + graphics.Descender(textFontSize) - graphics.Descender(labelFontSize);
            }

            double prefPadding = textFontSize * PaddingPref * PtToMm;
            double minPadding = textFontSize * PaddingMin * PtToMm;
            textBottomPadding = (maxHeight - height - numTextLines * textLeading) / numPaddings;
            if (textBottomPadding > prefPadding)
            {
                textBottomPadding = prefPadding;
            }
            else if (textBottomPadding < minPadding)
            {
                isTooTight = true;
                if (textBottomPadding < 0)
                {
                    textBottomPadding = 0;
                }
            }

            return isTooTight;
        }

        private void DrawBorder()
        {
            SeparatorType separatorType = bill.Format.SeparatorType;
            OutputSize outputSize = bill.Format.OutputSize;

            if (separatorType == SeparatorType.None)
            {
                return;
            }

            graphics.SetTransformation(0, 0, 0, 1, 1);

            // Draw vertical separator line between receipt and payment part
            graphics.StartPath();
            graphics.MoveTo(ReceiptWidth, 0);
            if (separatorType == SeparatorType.SolidLineWithScissors)
            {
                graphics.LineTo(ReceiptWidth, SlipHeight - 8);
                graphics.MoveTo(ReceiptWidth, SlipHeight - 5);
            }
            graphics.LineTo(ReceiptWidth, SlipHeight);

            // Draw horizontal separator line between bill and rest of A4 sheet
            if (outputSize != OutputSize.QRBillOnly)
            {
                graphics.MoveTo(0, SlipHeight);
                if (separatorType == SeparatorType.SolidLineWithScissors)
                {
                    graphics.LineTo(5, SlipHeight);
                    graphics.MoveTo(8, SlipHeight);
                }
                graphics.LineTo(SlipWidth, SlipHeight);
            }
            graphics.StrokePath(0.5, 0);

            // Draw scissors
            if (separatorType == SeparatorType.SolidLineWithScissors)
            {
                DrawScissors(ReceiptWidth, SlipHeight - 5, 3, 0);
                if (outputSize != OutputSize.QRBillOnly)
                {
                    DrawScissors(5, SlipHeight, 3, Math.PI / 2.0);
                }
            }
        }

        private void DrawScissors(double x, double y, double size, double angle)
        {
            DrawScissorsBlade(x, y, size, angle, false);
            DrawScissorsBlade(x, y, size, angle, true);
        }

        private void DrawScissorsBlade(double x, double y, double size, double angle, bool mirrored)
        {
            double scale = size / 476.0;
            double xOffset = 0.36 * size;
            double yOffset = -1.05 * size;
            using (Matrix matrix = new Matrix())
            {
                matrix.Translate((float)x, (float)y);
                matrix.Rotate((float)(angle / Math.PI * 180));
                matrix.Translate(mirrored ? (float)xOffset : (float)-xOffset, (float)yOffset);
                matrix.Scale(mirrored ? (float)-scale : (float)scale, (float)scale);
                graphics.SetTransformation(matrix.OffsetX, matrix.OffsetY, angle, mirrored ? -scale : scale, scale);
            }

            graphics.StartPath();
            graphics.MoveTo(46.48, 126.784);
            graphics.CubicCurveTo(34.824, 107.544, 28.0, 87.924, 28.0, 59.0);
            graphics.CubicCurveTo(28.0, 36.88, 33.387, 16.436, 42.507, -0.124);
            graphics.LineTo(242.743, 326.63);
            graphics.CubicCurveTo(246.359, 332.53, 254.836, 334.776, 265.31, 328.678);
            graphics.CubicCurveTo(276.973, 321.89, 290.532, 318.0, 305.0, 318.0);
            graphics.CubicCurveTo(348.63, 318.0, 384.0, 353.37, 384.0, 397.0);
            graphics.CubicCurveTo(384.0, 440.63, 348.63, 476.0, 305.0, 476.0);
            graphics.CubicCurveTo(278.066, 476.0, 254.28, 462.521, 240.02, 441.94);
            graphics.LineTo(46.48, 126.785);
            graphics.CloseSubpath();
            graphics.MoveTo(303.5, 446.0);
            graphics.CubicCurveTo(330.286, 446.0, 352.0, 424.286, 352.0, 397.5);
            graphics.CubicCurveTo(352.0, 370.714, 330.286, 349.0, 303.5, 349.0);
            graphics.CubicCurveTo(276.714, 349.0, 255.0, 370.714, 255.0, 397.5);
            graphics.CubicCurveTo(255.0, 424.286, 276.714, 446.0, 303.5, 446.0);
            graphics.CloseSubpath();
            graphics.FillPath(0);
        }

        // Draws a label at (0, yPos) and advances vertically
        private void DrawLabel(string labelKey)
        {
            yPos -= graphics.Ascender(labelFontSize);
            graphics.PutText(GetText(labelKey), 0, yPos, labelFontSize, true);
            yPos -= graphics.Descender(labelFontSize) + labelLeading;
        }

        // Draws a label and a single line of text at (0, yPos) and advances vertically
        private void DrawLabelAndText(string labelKey, string text)
        {
            DrawLabel(labelKey);
            yPos -= graphics.Ascender(textFontSize);
            graphics.PutText(text, 0, yPos, textFontSize, false);
            yPos -= graphics.Descender(textFontSize) + textLeading + textBottomPadding;
        }

        // Draws a label and a multiple lines of text at (0, yPos) and advances
        // vertically
        private void DrawLabelAndTextLines(string labelKey, string[] textLines)
        {
            DrawLabel(labelKey);
            yPos -= graphics.Ascender(textFontSize);
            graphics.PutTextLines(textLines, 0, yPos, textFontSize, textLeading);
            yPos -= textLines.Length * (graphics.LineHeight(textFontSize) + textLeading)
                        - graphics.Ascender(textFontSize) + textBottomPadding;
        }

        // Prepare the formatted text
        private void PrepareText()
        {
            string account = Payments.FormatIBAN(bill.Account);
            accountPayableTo = account + "\n" + FormatPersonForDisplay(bill.Creditor);

            reference = FormatReferenceNumber(bill.Reference);

            string info = bill.UnstructuredMessage;
            if (bill.BillInformation != null)
            {
                if (info == null)
                {
                    info = bill.BillInformation;
                }
                else
                {
                    info = info + "\n" + bill.BillInformation;
                }
            }
            if (info != null)
            {
                additionalInfo = info;
            }

            if (bill.Debtor != null)
            {
                payableBy = FormatPersonForDisplay(bill.Debtor);
            }

            if (bill.Amount != null)
            {
                amount = FormatAmountForDisplay(bill.Amount.Value);
            }
        }

        // Prepare the text (by breaking it into lines where necessary)
        private void BreakLines(double maxWidth)
        {
            accountPayableToLines = graphics.SplitLines(accountPayableTo, maxWidth * MmToPt, textFontSize);
            if (additionalInfo != null)
            {
                additionalInfoLines = graphics.SplitLines(additionalInfo, maxWidth * MmToPt, textFontSize);
            }

            if (payableBy != null)
            {
                payableByLines = graphics.SplitLines(payableBy, maxWidth * MmToPt, textFontSize);
            }
        }


        private static readonly double CornerStrokeWidth = 0.75;

        private void DrawCorners(double x, double y, double width, double height)
        {
            double lwh = CornerStrokeWidth * 0.5 / 72 * 25.4;
            double s = 3;

            graphics.StartPath();

            graphics.MoveTo(x + lwh, y + s);
            graphics.LineTo(x + lwh, y + lwh);
            graphics.LineTo(x + s, y + lwh);

            graphics.MoveTo(x + width - s, y + lwh);
            graphics.LineTo(x + width - lwh, y + lwh);
            graphics.LineTo(x + width - lwh, y + s);

            graphics.MoveTo(x + width - lwh, y + height - s);
            graphics.LineTo(x + width - lwh, y + height - lwh);
            graphics.LineTo(x + width - s, y + height - lwh);

            graphics.MoveTo(x + s, y + height - lwh);
            graphics.LineTo(x + lwh, y + height - lwh);
            graphics.LineTo(x + lwh, y + height - s);

            graphics.StrokePath(CornerStrokeWidth, 0);
        }

        private static string FormatAmountForDisplay(decimal amount)
        {

            return amount.ToString(AmountNumberInfo);
        }

        private static string FormatPersonForDisplay(Address address)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(address.Name);

            if (address.Type == AddressType.Structured)
            {
                string street = address.Street;
                if (street != null)
                {
                    sb.Append("\n");
                    sb.Append(street);
                }
                string houseNo = address.HouseNo;
                if (houseNo != null)
                {
                    sb.Append(street != null ? " " : "\n");
                    sb.Append(houseNo);
                }
                sb.Append("\n");
                sb.Append(address.PostalCode);
                sb.Append(" ");
                sb.Append(address.Town);

            }
            else if (address.Type == AddressType.CombinedElements)
            {
                if (address.AddressLine1 != null)
                {
                    sb.Append("\n");
                    sb.Append(address.AddressLine1);
                }
                sb.Append("\n");
                sb.Append(address.AddressLine2);
            }
            return sb.ToString();
        }

        private static string FormatReferenceNumber(string refNo)
        {
            if (refNo == null)
            {
                return null;
            }

            refNo = refNo.Trim();
            int len = refNo.Length;
            if (len == 0)
            {
                return null;
            }

            if (refNo.StartsWith("RF"))
            {
                // same format as IBAN
                return Payments.FormatIBAN(refNo);
            }

            return Payments.FormatQRReferenceNumber(refNo);
        }

        private string TrunacateText(string text, double maxWidth, int fontSize)
        {
            if (graphics.TextWidth(text, fontSize, false) < maxWidth)
            {
                return text;
            }

            string[] lines = graphics.SplitLines(text, maxWidth - fontSize * EllipsisWidth, fontSize);
            return lines[0] + "…";
        }

        private string GetText(string name)
        {
            return resourceSet.GetString(name);
        }

        private static NumberFormatInfo AmountNumberInfo;

        static BillLayout()
        {
            AmountNumberInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            AmountNumberInfo.NumberDecimalDigits = 2;
            AmountNumberInfo.NumberDecimalSeparator = ".";
            AmountNumberInfo.NumberGroupSeparator = " ";
        }
    }
}
