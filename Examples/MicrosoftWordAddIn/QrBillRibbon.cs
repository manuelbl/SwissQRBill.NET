//
// Swiss QR Bill Generator for .NET
// Copyright (c) 2022 Manuel Bleichenbacher
// Licensed under MIT License
// https://opensource.org/licenses/MIT
//

using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codecrete.SwissQRBill.WordAddIn
{
    public partial class QrBillRibbon
    {
        private void QrBillRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void InsertBillButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.InsertQrBill();
        }
    }
}
