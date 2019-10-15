namespace YT_CAD_TOOL
{
    partial class U_Layer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U_Layer));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.GC1 = new DevExpress.XtraGrid.GridControl();
            this.GV1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.sb_Export = new DevExpress.XtraEditors.SimpleButton();
            this.sb_Ok = new DevExpress.XtraEditors.SimpleButton();
            this.sb_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GC1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.GC1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(468, 534);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // GC1
            // 
            this.GC1.Location = new System.Drawing.Point(4, 4);
            this.GC1.MainView = this.GV1;
            this.GC1.Name = "GC1";
            this.GC1.Size = new System.Drawing.Size(460, 526);
            this.GC1.TabIndex = 4;
            this.GC1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GV1});
            // 
            // GV1
            // 
            this.GV1.GridControl = this.GC1;
            this.GV1.Name = "GV1";
            this.GV1.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroup1.Size = new System.Drawing.Size(468, 534);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.GC1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(464, 530);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.sb_Export);
            this.panelControl1.Controls.Add(this.sb_Ok);
            this.panelControl1.Controls.Add(this.sb_Cancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 534);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(468, 35);
            this.panelControl1.TabIndex = 1;
            // 
            // sb_Export
            // 
            this.sb_Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sb_Export.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Export.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbSelect.ImageOptions.Image")));
            this.sb_Export.Location = new System.Drawing.Point(12, 6);
            this.sb_Export.Name = "sb_Export";
            this.sb_Export.Size = new System.Drawing.Size(89, 23);
            this.sb_Export.TabIndex = 5;
            this.sb_Export.Text = "Export";
            // 
            // sb_Ok
            // 
            this.sb_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sb_Ok.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Ok.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbOk.ImageOptions.Image")));
            this.sb_Ok.Location = new System.Drawing.Point(330, 7);
            this.sb_Ok.Name = "sb_Ok";
            this.sb_Ok.Size = new System.Drawing.Size(60, 23);
            this.sb_Ok.TabIndex = 4;
            this.sb_Ok.Text = "확인";
            // 
            // sb_Cancel
            // 
            this.sb_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sb_Cancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Cancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbCancel.ImageOptions.Image")));
            this.sb_Cancel.Location = new System.Drawing.Point(396, 7);
            this.sb_Cancel.Name = "sb_Cancel";
            this.sb_Cancel.Size = new System.Drawing.Size(60, 23);
            this.sb_Cancel.TabIndex = 3;
            this.sb_Cancel.Text = "취소";
            // 
            // U_Layer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 569);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.panelControl1);
            this.Name = "U_Layer";
            this.Text = "U_Layer";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GC1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl GC1;
        private DevExpress.XtraGrid.Views.Grid.GridView GV1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton sb_Export;
        private DevExpress.XtraEditors.SimpleButton sb_Ok;
        private DevExpress.XtraEditors.SimpleButton sb_Cancel;
    }
}