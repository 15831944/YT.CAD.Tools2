namespace CADAUTO
{
    partial class YT_CAD_TOOL
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
            this.txt_Min = new DevExpress.XtraEditors.TextEdit();
            this.txt_Max = new DevExpress.XtraEditors.TextEdit();
            this.btn_Play = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.chk_Top = new DevExpress.XtraEditors.CheckEdit();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Min.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Max.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chk_Top.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_Min
            // 
            this.txt_Min.Location = new System.Drawing.Point(224, 28);
            this.txt_Min.Name = "txt_Min";
            this.txt_Min.Size = new System.Drawing.Size(100, 20);
            this.txt_Min.TabIndex = 0;
            // 
            // txt_Max
            // 
            this.txt_Max.Location = new System.Drawing.Point(224, 54);
            this.txt_Max.Name = "txt_Max";
            this.txt_Max.Size = new System.Drawing.Size(100, 20);
            this.txt_Max.TabIndex = 1;
            // 
            // btn_Play
            // 
            this.btn_Play.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Play.Location = new System.Drawing.Point(174, 7);
            this.btn_Play.Name = "btn_Play";
            this.btn_Play.Size = new System.Drawing.Size(75, 23);
            this.btn_Play.TabIndex = 2;
            this.btn_Play.Text = "실행";
            this.btn_Play.Click += new System.EventHandler(this.btn_Play_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Close.Location = new System.Drawing.Point(255, 7);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(75, 23);
            this.btn_Close.TabIndex = 3;
            this.btn_Close.Text = "닫기";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(186, 31);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(29, 14);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "MIN: ";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(186, 57);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(32, 14);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "MAX: ";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.chk_Top);
            this.panelControl1.Controls.Add(this.btn_Close);
            this.panelControl1.Controls.Add(this.btn_Play);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 140);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(342, 35);
            this.panelControl1.TabIndex = 6;
            // 
            // chk_Top
            // 
            this.chk_Top.EditValue = true;
            this.chk_Top.Location = new System.Drawing.Point(12, 9);
            this.chk_Top.Name = "chk_Top";
            this.chk_Top.Properties.Caption = "항상위";
            this.chk_Top.Size = new System.Drawing.Size(65, 19);
            this.chk_Top.TabIndex = 6;
            this.chk_Top.CheckedChanged += new System.EventHandler(this.chk_Top_CheckedChanged);
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.xtraTabControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(342, 140);
            this.xtraTabControl1.TabIndex = 7;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.txt_Min);
            this.xtraTabPage1.Controls.Add(this.txt_Max);
            this.xtraTabPage1.Controls.Add(this.labelControl2);
            this.xtraTabPage1.Controls.Add(this.labelControl1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(340, 114);
            this.xtraTabPage1.Text = "AutoRectangle";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(340, 114);
            this.xtraTabPage2.Text = "None";
            // 
            // YT_CAD_TOOL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 175);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "YT_CAD_TOOL";
            this.Text = "YT_CAD_TOOL";
            this.Load += new System.EventHandler(this._001U_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_Min.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Max.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chk_Top.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.xtraTabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txt_Min;
        private DevExpress.XtraEditors.TextEdit txt_Max;
        private DevExpress.XtraEditors.SimpleButton btn_Play;
        private DevExpress.XtraEditors.SimpleButton btn_Close;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.CheckEdit chk_Top;
    }
}