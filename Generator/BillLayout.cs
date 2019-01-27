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
        private const double PtToMm = 25.4 / 72;
        private const double MmToPt = 72 / 25.4;
        private const int FontSizeTitle = 11; // pt
        private const double SlipWidth = 210; // mm
        private const double SlipHeight = 105; // mm
        private const double Margin = 5; // mm
        private const double ReceiptWidth = 62; // mm
        private const double ReceiptTextWidth = 52; // mm
        private const double PaymentPartWidth = 148; // mm
        private const double PpAmountSectionWidth = 46; // mm
        private const double PpInfoSectionWidth = 87; // mm
        private const double AmountSectionTop = 37; // mm (from bottom)
        private const double BoxTopPadding = 2 * PtToMm; // mm
        private const double DebtorBoxWidthPp = 65; // mm
        private const double DebtorBoxHeightPp = 25; // mm
        private const double DebtorBoxWidthRc = 52; // mm
        private const double DebtorBoxHeightRc = 20; // mm


        private readonly Bill _bill;
        private readonly QRCode _qrCode;
        private readonly ICanvas _graphics;
        private readonly ResourceSet _resourceSet;

        private string _accountPayableTo;
        private string _reference;
        private string _additionalInfo;
        private string _payableBy;
        private string _amount;

        private string[] _accountPayableToLines;
        private string[] _additionalInfoLines;
        private string[] _payableByLines;

        private double _yPos;

        private int _labelFontSize;
        private double _labelAscender;
        private int _textFontSize;
        private double _textAscender;
        private double _lineSpacing;
        private double _extraSpacing;


        internal BillLayout(Bill bill, ICanvas graphics)
        {
            _bill = bill;
            _qrCode = new QRCode(bill);
            _qrCode = new QRCode(bill);
            _graphics = graphics;
            _resourceSet = MultilingualText.GetResourceSet(bill.Format.Language);
        }

        internal void Draw()
        {
            PrepareText();

            // payment part

            const int ppLabelPrefFontSize = 8; // pt
            const int ppTextPrefFontSize = 10; // pt
            const int ppTextMinFontSize = 8; // pt

            _labelFontSize = ppLabelPrefFontSize;
            _textFontSize = ppTextPrefFontSize;

            bool isTooTight;
            while (true)
            {
                BreakLines(PpInfoSectionWidth);
                isTooTight = ComputePaymentPartSpacing();
                if (!isTooTight || _textFontSize == ppTextMinFontSize)
                {
                    break;
                }

                _labelFontSize--;
                _textFontSize--;
            }
            DrawPaymentPart();

            // receipt

            const int rcLabelPrefFontSize = 6; // pt
            const int rcTextPrefFontSize = 8; // pt

            _labelFontSize = rcLabelPrefFontSize;
            _textFontSize = rcTextPrefFontSize;
            BreakLines(ReceiptWidth - 2 * Margin);
            isTooTight = ComputeReceiptSpacing();
            if (isTooTight)
            {
                PrepareReducedReceiptText(false);
                BreakLines(ReceiptWidth - 2 * Margin);
                isTooTight = ComputeReceiptSpacing();
            }
            if (isTooTight)
            {
                PrepareReducedReceiptText(true);
                BreakLines(ReceiptWidth - 2 * Margin);
                ComputeReceiptSpacing();
            }
            DrawReceipt();

            // border
            DrawBorder();
        }

        private void DrawPaymentPart()
        {
            const double qrCodeBottom = 43.5; // mm

            // title section
            _graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);
            _yPos = SlipHeight - Margin - _graphics.Ascender(FontSizeTitle);
            _graphics.PutText(GetText(MultilingualText.KeyPaymentPart), 0, _yPos, FontSizeTitle, true);

            // Swiss QR code section
            _qrCode.Draw(_graphics, ReceiptWidth + Margin, qrCodeBottom);

            // amount section
            DrawPaymentPartAmountSection();

            // information section
            DrawPaymentPartInformationSection();

            // further information section
            DrawFurtherInformationSection();
        }

        private void DrawPaymentPartAmountSection()
        {
            const double currencyWidthPp = 15; // mm
            const double amountBoxWidthPp = 40; // mm
            const double amountBoxHeightPp = 15; // mm

            _graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);

            // currency
            double y = AmountSectionTop - _labelAscender;
            string label = GetText(MultilingualText.KeyCurrency);
            _graphics.PutText(label, 0, y, _labelFontSize, true);

            y -= (_textFontSize + 3) * PtToMm;
            _graphics.PutText(_bill.Currency, 0, y, _textFontSize, false);

            // amount
            y = AmountSectionTop - _labelAscender;
            label = GetText(MultilingualText.KeyAmount);
            _graphics.PutText(label, currencyWidthPp, y, _labelFontSize, true);

            y -= (_textFontSize + 3) * PtToMm;
            if (_amount != null)
            {
                _graphics.PutText(_amount, currencyWidthPp, y, _labelFontSize, true);
            }
            else
            {
                y -= -_textAscender + amountBoxHeightPp;
                DrawCorners(PpAmountSectionWidth + Margin - amountBoxWidthPp, y, amountBoxWidthPp, amountBoxHeightPp);
            }
        }

        private void DrawPaymentPartInformationSection()
        {
            _graphics.SetTransformation(SlipWidth - PpInfoSectionWidth - Margin, 0, 0, 1, 1);
            _yPos = SlipHeight - Margin - _labelAscender;

            // account and creditor
            DrawLabelAndTextLines(MultilingualText.KeyAccountPayableTo, _accountPayableToLines);

            // reference
            if (_reference != null)
            {
                DrawLabelAndText(MultilingualText.KeyReference, _reference);
            }

            // additional information
            if (_additionalInfo != null)
            {
                DrawLabelAndTextLines(MultilingualText.KeyAdditionalInformation, _additionalInfoLines);
            }

            // payable by
            if (_payableBy != null)
            {
                DrawLabelAndTextLines(MultilingualText.KeyPayableBy, _payableByLines);
            }
            else
            {
                DrawLabel(MultilingualText.KeyPayableByNameAddr);
                _yPos -= -_textAscender + BoxTopPadding;
                _yPos -= DebtorBoxHeightPp;
                DrawCorners(0, _yPos, DebtorBoxWidthPp, DebtorBoxHeightPp);
            }
        }

        private void DrawFurtherInformationSection()
        {
            const int fontSize = 7;
            const int lineSpacing = 8;
            const double furtherInformationSectionTop = 15; // mm

            if (_bill.AlternativeSchemes == null || _bill.AlternativeSchemes.Count == 0)
            {
                return;
            }

            _graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);
            double y = furtherInformationSectionTop - _graphics.Ascender(fontSize);
            const double maxWidth = PaymentPartWidth - 2 * Margin;

            foreach (AlternativeScheme scheme in _bill.AlternativeSchemes)
            {
                string boldText = $"{scheme.Name}: ";
                double boldTextWidth = _graphics.TextWidth(boldText, fontSize, true);
                _graphics.PutText(boldText, 0, y, fontSize, true);

                string normalText = TrunacateText(scheme.Instruction, maxWidth - boldTextWidth, fontSize);
                _graphics.PutText(normalText, boldTextWidth, y, fontSize, false);
                y -= lineSpacing * PtToMm;
            }
        }

        private void DrawReceipt()
        {
            // "Receipt" title
            _graphics.SetTransformation(Margin, 0, 0, 1, 1);
            _yPos = SlipHeight - Margin - _graphics.Ascender(FontSizeTitle);
            _graphics.PutText(GetText(MultilingualText.KeyReceipt), 0, _yPos, FontSizeTitle, true);

            // information section
            DrawReceiptInformationSection();

            // amount section
            DrawReceiptAmountSection();

            // acceptance point
            DrawReceiptAcceptancePointSection();
        }

        private void DrawReceiptInformationSection()
        {
            const double titleHeight = 7; // mm

            // payable to
            _yPos = SlipHeight - Margin - titleHeight - _labelAscender;
            DrawLabelAndTextLines(MultilingualText.KeyAccountPayableTo, _accountPayableToLines);

            // reference
            if (_reference != null)
            {
                DrawLabelAndText(MultilingualText.KeyReference, _reference);
            }

            // payable by
            if (_payableBy != null)
            {
                DrawLabelAndTextLines(MultilingualText.KeyPayableBy, _payableByLines);
            }
            else
            {
                DrawLabel(MultilingualText.KeyPayableByNameAddr);
                _yPos -= -_textAscender + BoxTopPadding;
                _yPos -= DebtorBoxHeightRc;
                DrawCorners(0, _yPos, DebtorBoxWidthRc, DebtorBoxHeightRc);
            }
        }

        private void DrawReceiptAmountSection()
        {
            const double currencyWidthRc = 12; // mm
            const double amountBoxWidthRc = 30; // mm
            const double amountBoxHeightRc = 10; // mm

            // currency
            double y = AmountSectionTop - _labelAscender;
            string label = GetText(MultilingualText.KeyCurrency);
            _graphics.PutText(label, 0, y, _labelFontSize, true);

            y -= (_textFontSize + 3) * PtToMm;
            _graphics.PutText(_bill.Currency, 0, y, _textFontSize, false);

            // amount
            y = AmountSectionTop - _labelAscender;
            label = GetText(MultilingualText.KeyAmount);
            _graphics.PutText(label, currencyWidthRc, y, _labelFontSize, true);

            if (_amount != null)
            {
                y -= (_textFontSize + 3) * PtToMm;
                _graphics.PutText(_amount, currencyWidthRc, y, _textFontSize, false);
            }
            else
            {
                DrawCorners(ReceiptTextWidth - amountBoxWidthRc,
                        AmountSectionTop - amountBoxHeightRc,
                        amountBoxWidthRc, amountBoxHeightRc);
            }
        }

        private void DrawReceiptAcceptancePointSection()
        {
            const double acceptancePointSectionTop = 23; // mm (from bottom)

            string label = GetText(MultilingualText.KeyAcceptancePoint);
            double y = acceptancePointSectionTop - _labelAscender;
            double w = _graphics.TextWidth(label, _labelFontSize, true);
            _graphics.PutText(label, ReceiptTextWidth - w, y, _labelFontSize, true);
        }


        private bool ComputePaymentPartSpacing()
        {
            const double ppInfoSectionMaxHeight = 85; // mm

            // numExtraLines: the number of lines between text blocks
            int numTextLines = 0;
            int numExtraLines = 0;
            double fixedHeight = 0;

            numTextLines += 1 + _accountPayableToLines.Length;
            if (_reference != null)
            {
                numExtraLines++;
                numTextLines += 2;
            }
            if (_additionalInfo != null)
            {
                numExtraLines++;
                numTextLines += 1 + _additionalInfoLines.Length;
            }
            numExtraLines++;
            if (_payableBy != null)
            {
                numTextLines += 1 + _payableByLines.Length;
            }
            else
            {
                numTextLines += 1;
                fixedHeight += DebtorBoxHeightPp;
            }

            // extra spacing line if there are alternative schemes
            if (_bill.AlternativeSchemes != null && _bill.AlternativeSchemes.Count > 0)
            {
                numExtraLines++;
            }

            return ComputeSpacing(ppInfoSectionMaxHeight, fixedHeight, numTextLines, numExtraLines);
        }

        private bool ComputeReceiptSpacing()
        {
            const double receiptMaxHeight = 56; // mm

            // numExtraLines: the number of lines between text blocks
            int numTextLines = 0;
            int numExtraLines = 0;
            double fixedHeight = 0;

            numTextLines += 1 + _accountPayableToLines.Length;
            if (_reference != null)
            {
                numExtraLines++;
                numTextLines += 2;
            }
            numExtraLines++;
            if (_payableBy != null)
            {
                numTextLines += 1 + _payableByLines.Length;
            }
            else
            {
                numTextLines += 1;
                fixedHeight += DebtorBoxHeightRc;
            }

            numExtraLines++;

            return ComputeSpacing(receiptMaxHeight, fixedHeight, numTextLines, numExtraLines);
        }

        private bool ComputeSpacing(double maxHeight, double fixedHeight, int numTextLines, int numExtraLines)
        {
            _lineSpacing = (_textFontSize + 1) * PtToMm;
            _extraSpacing = (maxHeight - fixedHeight - numTextLines * _lineSpacing) / numExtraLines;
            _extraSpacing = Math.Min(Math.Max(_extraSpacing, 0), _lineSpacing);

            _labelAscender = _graphics.Ascender(_labelFontSize);
            _textAscender = _graphics.Ascender(_textFontSize);

            return _extraSpacing / _lineSpacing < 0.8;
        }

        private void DrawBorder()
        {
            SeparatorType separatorType = _bill.Format.SeparatorType;
            OutputSize outputSize = _bill.Format.OutputSize;

            if (separatorType == SeparatorType.None)
            {
                return;
            }

            _graphics.SetTransformation(0, 0, 0, 1, 1);

            // Draw vertical separator line between receipt and payment part
            _graphics.StartPath();
            _graphics.MoveTo(ReceiptWidth, 0);
            if (separatorType == SeparatorType.SolidLineWithScissors)
            {
                _graphics.LineTo(ReceiptWidth, SlipHeight - 8);
                _graphics.MoveTo(ReceiptWidth, SlipHeight - 5);
            }
            _graphics.LineTo(ReceiptWidth, SlipHeight);

            // Draw horizontal separator line between bill and rest of A4 sheet
            if (outputSize != OutputSize.QrBillOnly)
            {
                _graphics.MoveTo(0, SlipHeight);
                if (separatorType == SeparatorType.SolidLineWithScissors)
                {
                    _graphics.LineTo(5, SlipHeight);
                    _graphics.MoveTo(8, SlipHeight);
                }
                _graphics.LineTo(SlipWidth, SlipHeight);
            }
            _graphics.StrokePath(0.5, 0);

            // Draw scissors
            if (separatorType == SeparatorType.SolidLineWithScissors)
            {
                DrawScissors(ReceiptWidth, SlipHeight - 5, 3, 0);
                if (outputSize != OutputSize.QrBillOnly)
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
                _graphics.SetTransformation(matrix.OffsetX, matrix.OffsetY, angle, mirrored ? -scale : scale, scale);
            }

            _graphics.StartPath();
            _graphics.MoveTo(46.48, 126.784);
            _graphics.CubicCurveTo(34.824, 107.544, 28.0, 87.924, 28.0, 59.0);
            _graphics.CubicCurveTo(28.0, 36.88, 33.387, 16.436, 42.507, -0.124);
            _graphics.LineTo(242.743, 326.63);
            _graphics.CubicCurveTo(246.359, 332.53, 254.836, 334.776, 265.31, 328.678);
            _graphics.CubicCurveTo(276.973, 321.89, 290.532, 318.0, 305.0, 318.0);
            _graphics.CubicCurveTo(348.63, 318.0, 384.0, 353.37, 384.0, 397.0);
            _graphics.CubicCurveTo(384.0, 440.63, 348.63, 476.0, 305.0, 476.0);
            _graphics.CubicCurveTo(278.066, 476.0, 254.28, 462.521, 240.02, 441.94);
            _graphics.LineTo(46.48, 126.785);
            _graphics.CloseSubpath();
            _graphics.MoveTo(303.5, 446.0);
            _graphics.CubicCurveTo(330.286, 446.0, 352.0, 424.286, 352.0, 397.5);
            _graphics.CubicCurveTo(352.0, 370.714, 330.286, 349.0, 303.5, 349.0);
            _graphics.CubicCurveTo(276.714, 349.0, 255.0, 370.714, 255.0, 397.5);
            _graphics.CubicCurveTo(255.0, 424.286, 276.714, 446.0, 303.5, 446.0);
            _graphics.CloseSubpath();
            _graphics.FillPath(0);
        }

        // Draws a label at (0, yPos) and advances vertically.
        // yPos is taken as the baseline for the text.
        private void DrawLabel(string labelKey)
        {
            _graphics.PutText(GetText(labelKey), 0, _yPos, _labelFontSize, true);
            _yPos -= _lineSpacing;
        }

        // Draws a label and a single line of text at (0, yPos) and advances vertically.
        // yPos is taken as the baseline for the text.
        private void DrawLabelAndText(string labelKey, string text)
        {
            DrawLabel(labelKey);
            _graphics.PutText(text, 0, _yPos, _textFontSize, false);
            _yPos -= _lineSpacing + _extraSpacing;
        }

        // Draws a label and a multiple lines of text at (0, yPos) and advances vertically.
        // yPos is taken as the baseline for the text.
        private void DrawLabelAndTextLines(string labelKey, string[] textLines)
        {
            DrawLabel(labelKey);
            double leading = _lineSpacing - _graphics.LineHeight(_textFontSize);
            _graphics.PutTextLines(textLines, 0, _yPos, _textFontSize, leading);
            _yPos -= textLines.Length * _lineSpacing + _extraSpacing;
        }

        // Prepare the formatted text
        private void PrepareText()
        {
            string account = Payments.FormatIban(_bill.Account);
            _accountPayableTo = account + "\n" + FormatPersonForDisplay(_bill.Creditor);

            _reference = FormatReferenceNumber(_bill.Reference);

            string info = _bill.UnstructuredMessage;
            if (_bill.BillInformation != null)
            {
                if (info == null)
                {
                    info = _bill.BillInformation;
                }
                else
                {
                    info = info + "\n" + _bill.BillInformation;
                }
            }
            if (info != null)
            {
                _additionalInfo = info;
            }

            if (_bill.Debtor != null)
            {
                _payableBy = FormatPersonForDisplay(_bill.Debtor);
            }

            if (_bill.Amount != null)
            {
                _amount = FormatAmountForDisplay(_bill.Amount.Value);
            }
        }

        private void PrepareReducedReceiptText(bool reduceBoth)
        {
            if (reduceBoth)
            {
                string account = Payments.FormatIban(_bill.Account);
                _accountPayableTo = account + "\n" + FormatPersonForDisplay(CreateReducedAddress(_bill.Creditor));
            }

            if (_bill.Debtor != null)
            {
                _payableBy = FormatPersonForDisplay(CreateReducedAddress(_bill.Debtor));
            }
        }

        private static Address CreateReducedAddress(Address address)
        {
            Address reducedAddress = new Address
            {
                Name = address.Name,
                CountryCode = address.CountryCode
            };

            switch (address.Type)
            {
                case AddressType.Structured:
                    reducedAddress.PostalCode = address.PostalCode;
                    reducedAddress.Town = address.Town;
                    break;
                case AddressType.CombinedElements:
                    reducedAddress.AddressLine2 = address.AddressLine2;
                    break;
            }

            return reducedAddress;
        }

        // Prepare the text (by breaking it into lines where necessary)
        private void BreakLines(double maxWidth)
        {
            _accountPayableToLines = _graphics.SplitLines(_accountPayableTo, maxWidth * MmToPt, _textFontSize);
            if (_additionalInfo != null)
            {
                _additionalInfoLines = _graphics.SplitLines(_additionalInfo, maxWidth * MmToPt, _textFontSize);
            }

            if (_payableBy != null)
            {
                _payableByLines = _graphics.SplitLines(_payableBy, maxWidth * MmToPt, _textFontSize);
            }
        }


        private const double CornerStrokeWidth = 0.75;

        private void DrawCorners(double x, double y, double width, double height)
        {
            const double lwh = CornerStrokeWidth * 0.5 / 72 * 25.4;
            const double s = 3; // mm

            _graphics.StartPath();

            _graphics.MoveTo(x + lwh, y + s);
            _graphics.LineTo(x + lwh, y + lwh);
            _graphics.LineTo(x + s, y + lwh);

            _graphics.MoveTo(x + width - s, y + lwh);
            _graphics.LineTo(x + width - lwh, y + lwh);
            _graphics.LineTo(x + width - lwh, y + s);

            _graphics.MoveTo(x + width - lwh, y + height - s);
            _graphics.LineTo(x + width - lwh, y + height - lwh);
            _graphics.LineTo(x + width - s, y + height - lwh);

            _graphics.MoveTo(x + s, y + height - lwh);
            _graphics.LineTo(x + lwh, y + height - lwh);
            _graphics.LineTo(x + lwh, y + height - s);

            _graphics.StrokePath(CornerStrokeWidth, 0);
        }

        private static string FormatAmountForDisplay(decimal amount)
        {

            return amount.ToString("N", AmountNumberInfo);
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
                if ("CH" != address.CountryCode && "LI" != address.CountryCode)
                {
                    sb.Append(address.CountryCode);
                    sb.Append(" - ");
                }
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
                if ("CH" != address.CountryCode && "LI" != address.CountryCode)
                {
                    sb.Append(address.CountryCode);
                    sb.Append(" ");
                }
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
                return Payments.FormatIban(refNo);
            }

            return Payments.FormatQrReferenceNumber(refNo);
        }

        private string TrunacateText(string text, double maxWidth, int fontSize)
        {
            const double ellipsisWidth = 0.3528; // mm * font size

            if (_graphics.TextWidth(text, fontSize, false) < maxWidth)
            {
                return text;
            }

            string[] lines = _graphics.SplitLines(text, maxWidth - fontSize * ellipsisWidth, fontSize);
            return lines[0] + "…";
        }

        private string GetText(string name)
        {
            return _resourceSet.GetString(name);
        }

        private static readonly NumberFormatInfo AmountNumberInfo = CreateAmountNumberInfo();

        private static NumberFormatInfo CreateAmountNumberInfo()
        {
            NumberFormatInfo numberInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberInfo.NumberDecimalDigits = 2;
            numberInfo.NumberDecimalSeparator = ".";
            numberInfo.NumberGroupSeparator = " ";
            return numberInfo;
        }
    }
}
