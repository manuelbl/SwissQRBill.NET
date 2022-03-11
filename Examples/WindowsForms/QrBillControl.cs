//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2021 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Codecrete.SwissQRBill.Generator;
using Codecrete.SwissQRBill.Windows;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Codecrete.SwissQRBill.Examples.WindowsForms
{
    /// <summary>
    /// Custom control for Windows Forms displaying a Swiss QR Bill
    /// </summary>
    public class QrBillControl : Control
    {

        private Bill _bill;

        public QrBillControl()
        {
            ResizeRedraw = true;

            // create sample data
            Bill = new Bill
            {
                // creditor data
                Account = "CH1909000000690028219",
                Creditor = new Address
                {
                    Name = "Croce Rossa Svizzera - Sezione del Sottoceneri",
                    AddressLine1 = "Via alla Campagna",
                    AddressLine2 = "6900 Lugano",
                    CountryCode = "CH"
                },

                // payment data
                Currency = "CHF"
            };
        }

        /// <summary>
        /// Bill data to display.
        /// <para>
        /// Changing bill properties will not update the control.
        /// To trigger an update, assign the bill instance again,
        /// even if it is the same instance.
        /// </para>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Bill Bill
        {
            get
            {
                return _bill;
            }
            set
            {
                _bill = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // set to QR bill only, i.e. 210 by 105 mm
            OutputSize savedOutputSize = Bill.Format.OutputSize;
            Bill.Format.OutputSize = OutputSize.QrBillOnly;

            // find smaller dimension
            float scale = MathF.Min(Size.Width / 210f, Size.Height / 105f);
            float xPadding = (Size.Width - scale * 210f) / 2f;
            float yPadding = (Size.Height - scale * 105f) / 2f;

            // draw white background
            RectangleF billBounds = new RectangleF(xPadding, yPadding, scale * 210f, scale * 105f);
            e.Graphics.FillRectangle(Brushes.White, billBounds);

            // draw QR bill
            using SystemDrawingCanvas canvas = new SystemDrawingCanvas(e.Graphics, xPadding, Size.Height - yPadding, scale, "Arial");
            try
            {
                QRBill.Draw(Bill, canvas);
            }
            catch (QRBillValidationException)
            {
                // ignore invalid bill data
            }

            Bill.Format.OutputSize = savedOutputSize;
        }
    }
}
