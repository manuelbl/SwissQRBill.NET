namespace Codecrete.SwissQRBill.WordAddIn
{
    partial class QrBillRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public QrBillRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.qrBillTab = this.Factory.CreateRibbonTab();
            this.qrBillGroup = this.Factory.CreateRibbonGroup();
            this.insertBillButton = this.Factory.CreateRibbonButton();
            this.qrBillTab.SuspendLayout();
            this.qrBillGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // qrBillTab
            // 
            this.qrBillTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.qrBillTab.Groups.Add(this.qrBillGroup);
            this.qrBillTab.Label = "TabAddIns";
            this.qrBillTab.Name = "qrBillTab";
            // 
            // qrBillGroup
            // 
            this.qrBillGroup.Items.Add(this.insertBillButton);
            this.qrBillGroup.Label = "QR Bill";
            this.qrBillGroup.Name = "qrBillGroup";
            // 
            // insertBillButton
            // 
            this.insertBillButton.Label = "Insert Bill";
            this.insertBillButton.Name = "insertBillButton";
            this.insertBillButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.InsertBillButton_Click);
            // 
            // QrBillRibbon
            // 
            this.Name = "QrBillRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.qrBillTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.QrBillRibbon_Load);
            this.qrBillTab.ResumeLayout(false);
            this.qrBillTab.PerformLayout();
            this.qrBillGroup.ResumeLayout(false);
            this.qrBillGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab qrBillTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup qrBillGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton insertBillButton;
    }

    partial class ThisRibbonCollection
    {
        internal QrBillRibbon QrBillRibbon
        {
            get { return this.GetRibbon<QrBillRibbon>(); }
        }
    }
}
