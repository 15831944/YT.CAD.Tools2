namespace YT_CAD_TOOL
{
    partial class U_List
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U_List));
            this.LB = new DevExpress.XtraEditors.ListBoxControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.te_size = new DevExpress.XtraEditors.TextEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.sbOk = new DevExpress.XtraEditors.SimpleButton();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.LB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.te_size.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // LB
            // 
            this.LB.Location = new System.Drawing.Point(4, 28);
            this.LB.Name = "LB";
            this.LB.Size = new System.Drawing.Size(206, 297);
            this.LB.StyleController = this.layoutControl1;
            this.LB.TabIndex = 0;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.te_size);
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.LB);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2364, 92, 650, 400);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(214, 366);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // te_size
            // 
            this.te_size.Location = new System.Drawing.Point(151, 4);
            this.te_size.Name = "te_size";
            this.te_size.Size = new System.Drawing.Size(59, 20);
            this.te_size.StyleController = this.layoutControl1;
            this.te_size.TabIndex = 5;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.sbOk);
            this.panelControl1.Controls.Add(this.sbCancel);
            this.panelControl1.Location = new System.Drawing.Point(4, 329);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(3);
            this.panelControl1.Size = new System.Drawing.Size(206, 33);
            this.panelControl1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroup1.Size = new System.Drawing.Size(214, 366);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.LB;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(54, 4);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(210, 301);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panelControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 325);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 37);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(5, 37);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(210, 37);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.te_size;
            this.layoutControlItem3.Location = new System.Drawing.Point(108, 0);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(93, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(102, 24);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "전역폭";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(36, 14);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(108, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // sbOk
            // 
            this.sbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbOk.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sbOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbOk.ImageOptions.Image")));
            this.sbOk.Location = new System.Drawing.Point(72, 5);
            this.sbOk.Name = "sbOk";
            this.sbOk.Size = new System.Drawing.Size(60, 23);
            this.sbOk.TabIndex = 3;
            this.sbOk.Text = "확인";
            // 
            // sbCancel
            // 
            this.sbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sbCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbCancel.ImageOptions.Image")));
            this.sbCancel.Location = new System.Drawing.Point(138, 5);
            this.sbCancel.Name = "sbCancel";
            this.sbCancel.Size = new System.Drawing.Size(60, 23);
            this.sbCancel.TabIndex = 2;
            this.sbCancel.Text = "취소";
            // 
            // U_List
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 366);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "U_List";
            this.Text = "U_List";
            ((System.ComponentModel.ISupportInitialize)(this.LB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.te_size.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl LB;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton sbOk;
        private DevExpress.XtraEditors.SimpleButton sbCancel;
        private DevExpress.XtraEditors.TextEdit te_size;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}