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
        private const int PpLabelPrefFontSize = 8; // pt
        private const int PpTextPrefFontSize = 10; // pt
        private const int PpTextMinFontSize = 8; // pt
        private const int RcLabelPrefFontSize = 6; // pt
        private const int RcTextPrefFontSize = 8; // pt
        private const double SlipWidth = 210; // mm
        private const double SlipHeight = 105; // mm
        private const double PaymentPartWidth = 148; // mm
        private const double ReceiptWidth = SlipWidth - PaymentPartWidth; // mm
        private const double Margin = 5; // mm
        private static readonly double QrCodeSize = QRCode.Size; // 46 mm
        private const double InfoSectionWidth = 81; // mm (must not be smaller than 65)
        private const double CurrencyAmountBaseLine = 32; // mm (from to bottom)
        private const double CurrencWidthPp = 15; // mm
        private const double CurrencyWidthRc = 13; // mm
        private const double AmountBoWidthPp = 40; // mm
        private const double AmountBoxHeightPp = 15; // mm
        private const double AmountBoxWidthRc = 30; // mm
        private const double AmountBoxHeightRc = 10; // mm
        private const double DebtorBoxHeightPp = 25; // mm (must not be smaller than 25)
        private const double DebtorBoxHeightRc = 25; // mm (must not be smaller than 25)
        private const double AlternativeSchemesHeight = 6; // mm
        private const double InfoSectionMaxHeight = SlipHeight - AlternativeSchemesHeight - 3 * Margin;
        private const double ReceiptMaxHeight = SlipHeight - CurrencyAmountBaseLine - 7 - 0.5 - Margin;
        private const double LeadingPref = 0.2; // relative to font size
        private const double PaddingPref = 0.5; // relative to font size
        private const double PaddingMin = 0.2; // relative to font size
        private const double EllipsisWidth = 0.3528; // mm * font size


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
        private int _textFontSize;
        private double _labelLeading;
        private double _textLeading;
        private double _textBottomPadding;


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

            _labelFontSize = PpLabelPrefFontSize;
            _textFontSize = PpTextPrefFontSize;

            while (true)
            {
                BreakLines(InfoSectionWidth);
                bool isTooTight = ComputePaymentPartLeading();
                if (!isTooTight || _textFontSize == PpTextMinFontSize)
                {
                    break;
                }

                _labelFontSize--;
                _textFontSize--;
            }
            DrawPaymentPart();

            // receipt

            _labelFontSize = RcLabelPrefFontSize;
            _textFontSize = RcTextPrefFontSize;
            BreakLines(ReceiptWidth - 2 * Margin);
            ComputeReceiptLeading();
            DrawReceipt();

            // border
            DrawBorder();
        }

        private void DrawPaymentPart()
        {
            // QR code section
            _qrCode.Draw(_graphics, ReceiptWidth + Margin, SlipHeight - 17 - QrCodeSize);

            // "Payment part" title
            _graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);
            _yPos = SlipHeight - Margin - _graphics.Ascender(FontSizeTitle);
            _graphics.PutText(GetText(MultilingualText.KeyPaymentPart), 0,
                        _yPos, FontSizeTitle, true);

            // currency
            _yPos = CurrencyAmountBaseLine + _graphics.Ascender(_labelFontSize);
            DrawLabelAndText(MultilingualText.KeyCurrency, _bill.Currency);

            // amount
            _graphics.SetTransformation(ReceiptWidth + Margin + CurrencWidthPp, 0, 0, 1, 1);
            _yPos = CurrencyAmountBaseLine + _graphics.Ascender(_labelFontSize);
            if (_amount != null)
            {
                DrawLabelAndText(MultilingualText.KeyAmount, _amount);
            }
            else
            {
                DrawLabel(MultilingualText.KeyAmount);
                DrawCorners(0, _yPos - AmountBoxHeightPp, AmountBoWidthPp, AmountBoxHeightPp);
            }

            // information section
            _graphics.SetTransformation(SlipWidth - InfoSectionWidth - Margin, 0, 0, 1, 1);
            _yPos = SlipHeight - Margin;

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
                _yPos -= DebtorBoxHeightPp;
                DrawCorners(0, _yPos, InfoSectionWidth, DebtorBoxHeightPp);
            }

            // alternative schemes
            DrawAlternativeSchemes();
        }

        private void DrawReceipt()
        {
            // "Receipt" title
            _graphics.SetTransformation(Margin, 0, 0, 1, 1);
            _yPos = SlipHeight - Margin - _graphics.Ascender(FontSizeTitle);
            _graphics.PutText(GetText(MultilingualText.KeyReceipt), 0,
                        _yPos, FontSizeTitle, true);

            // account and creditor
            _yPos = SlipHeight - Margin - 7;
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
                _yPos -= DebtorBoxHeightRc;
                DrawCorners(0, _yPos, ReceiptWidth - 2 * Margin, DebtorBoxHeightRc);
            }

            // currency
            _yPos = CurrencyAmountBaseLine + _graphics.Ascender(_labelFontSize);
            DrawLabelAndText(MultilingualText.KeyCurrency, _bill.Currency);

            // amount
            _graphics.SetTransformation(Margin + CurrencyWidthRc, 0, 0, 1, 1);
            _yPos = CurrencyAmountBaseLine + _graphics.Ascender(_labelFontSize);
            if (_amount != null)
            {
                DrawLabelAndText(MultilingualText.KeyAmount, _amount);
            }
            else
            {
                DrawLabel(MultilingualText.KeyAmount);
                _graphics.SetTransformation(0, 0, 0, 1, 1);
                DrawCorners(ReceiptWidth - Margin - AmountBoxWidthRc,
                        CurrencyAmountBaseLine + 2 - AmountBoxHeightRc,
                        AmountBoxWidthRc, AmountBoxHeightRc);
            }

            // acceptance point
            _graphics.SetTransformation(0, 0, 0, 1, 1);
            string label = GetText(MultilingualText.KeyAcceptancePoint);
            double w = _graphics.TextWidth(label, _labelFontSize, true);
            _graphics.PutText(label, ReceiptWidth - Margin - w, 21, _labelFontSize, true);
        }

        private void DrawAlternativeSchemes()
        {
            if (_bill.AlternativeSchemes == null || _bill.AlternativeSchemes.Count == 0)
            {
                return;
            }

            _graphics.SetTransformation(ReceiptWidth + Margin, 0, 0, 1, 1);
            double y = 11 - _graphics.Ascender(7);
            double maxWidth = PaymentPartWidth - 2 * Margin;

            foreach (AlternativeScheme scheme in _bill.AlternativeSchemes)
            {
                string boldText = $"{scheme.Name}: ";
                double boldTextWidth = _graphics.TextWidth(boldText, 7, true);
                _graphics.PutText(boldText, 0, y, 7, true);

                string normalText = TrunacateText(scheme.Instruction, maxWidth - boldTextWidth, 7);
                _graphics.PutText(normalText, boldTextWidth, y, 7, false);
                y -= _graphics.LineHeight(7) * 1.2;
            }
        }

        private bool ComputePaymentPartLeading()
        {
            // The same line spacing (incl. leading) is used for the smaller label and text lines
            int numTextLines = 0;
            int numPaddings = 0;
            double height = 0;

            numTextLines += 1 + _accountPayableToLines.Length;
            height -= _graphics.Ascender(_textFontSize) - _graphics.Ascender(_labelFontSize);
            if (_reference != null)
            {
                numPaddings++;
                numTextLines += 2;
            }
            if (_additionalInfo != null)
            {
                numPaddings++;
                numTextLines += 1 + _additionalInfoLines.Length;
            }
            numPaddings++;
            if (_payableBy != null)
            {
                numTextLines += 1 + _payableByLines.Length;
            }
            else
            {
                numTextLines += 1;
                height += DebtorBoxHeightPp;
            }

            height += numTextLines * _graphics.LineHeight(_textFontSize);

            return ComputeLeading(height, InfoSectionMaxHeight, numTextLines, numPaddings);
        }

        private void ComputeReceiptLeading()
        {
            // The same line spacing (incl. leading) is used for the smaller label and text lines
            int numTextLines = 0;
            int numPaddings = 1;
            double height = 0;

            numTextLines += 1 + _accountPayableToLines.Length;
            height -= _graphics.Ascender(_textFontSize) - _graphics.Ascender(_labelFontSize);
            if (_reference != null)
            {
                numPaddings++;
                numTextLines += 2;
            }
            numPaddings++;
            if (_payableBy != null)
            {
                numTextLines += 1 + _payableByLines.Length;
            }
            else
            {
                numTextLines += 1;
                height += DebtorBoxHeightRc;
            }

            height += numTextLines * _graphics.LineHeight(_textFontSize);

            ComputeLeading(height, ReceiptMaxHeight, numTextLines, numPaddings);
        }

        private bool ComputeLeading(double height, double maxHeight, int numTextLines, int numPaddings)
        {
            bool isTooTight = false;
            _textLeading = LeadingPref * _textFontSize * PtToMm;
            if (height + numTextLines * _textLeading > maxHeight)
            {
                isTooTight = true;
                if (height > maxHeight)
                {
                    _textLeading = 0;
                    _labelLeading = 0;
                }
                else
                {
                    _textLeading = (maxHeight - height) / numTextLines;
                    _labelLeading = _textLeading + _graphics.Descender(_textFontSize) - _graphics.Descender(_labelFontSize);
                }
            }
            else
            {
                _labelLeading = _textLeading + _graphics.Descender(_textFontSize) - _graphics.Descender(_labelFontSize);
            }

            double prefPadding = _textFontSize * PaddingPref * PtToMm;
            double minPadding = _textFontSize * PaddingMin * PtToMm;
            _textBottomPadding = (maxHeight - height - numTextLines * _textLeading) / numPaddings;
            if (_textBottomPadding > prefPadding)
            {
                _textBottomPadding = prefPadding;
            }
            else if (_textBottomPadding < minPadding)
            {
                isTooTight = true;
                if (_textBottomPadding < 0)
                {
                    _textBottomPadding = 0;
                }
            }

            return isTooTight;
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

        // Draws a label at (0, yPos) and advances vertically
        private void DrawLabel(string labelKey)
        {
            _yPos -= _graphics.Ascender(_labelFontSize);
            _graphics.PutText(GetText(labelKey), 0, _yPos, _labelFontSize, true);
            _yPos -= _graphics.Descender(_labelFontSize) + _labelLeading;
        }

        // Draws a label and a single line of text at (0, yPos) and advances vertically
        private void DrawLabelAndText(string labelKey, string text)
        {
            DrawLabel(labelKey);
            _yPos -= _graphics.Ascender(_textFontSize);
            _graphics.PutText(text, 0, _yPos, _textFontSize, false);
            _yPos -= _graphics.Descender(_textFontSize) + _textLeading + _textBottomPadding;
        }

        // Draws a label and a multiple lines of text at (0, yPos) and advances
        // vertically
        private void DrawLabelAndTextLines(string labelKey, string[] textLines)
        {
            DrawLabel(labelKey);
            _yPos -= _graphics.Ascender(_textFontSize);
            _graphics.PutTextLines(textLines, 0, _yPos, _textFontSize, _textLeading);
            _yPos -= textLines.Length * (_graphics.LineHeight(_textFontSize) + _textLeading)
                        - _graphics.Ascender(_textFontSize) + _textBottomPadding;
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
            double lwh = CornerStrokeWidth * 0.5 / 72 * 25.4;
            double s = 3;

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
                return Payments.FormatIban(refNo);
            }

            return Payments.FormatQrReferenceNumber(refNo);
        }

        private string TrunacateText(string text, double maxWidth, int fontSize)
        {
            if (_graphics.TextWidth(text, fontSize, false) < maxWidth)
            {
                return text;
            }

            string[] lines = _graphics.SplitLines(text, maxWidth - fontSize * EllipsisWidth, fontSize);
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
