namespace YT_CAD_TOOL
{
    partial class U001
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U001));
            this.txt_Min = new DevExpress.XtraEditors.TextEdit();
            this.txt_Max = new DevExpress.XtraEditors.TextEdit();
            this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Play = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.chk_Top = new DevExpress.XtraEditors.CheckEdit();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btn_keyline = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Column = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Beam = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.txt_extend = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txt_dis2 = new DevExpress.XtraEditors.TextEdit();
            this.txt_distance = new DevExpress.XtraEditors.TextEdit();
            this.groupControl10 = new DevExpress.XtraEditors.GroupControl();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Min.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Max.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chk_Top.Properties)).BeginInit();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_extend.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_dis2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_distance.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl10)).BeginInit();
            this.groupControl10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_Min
            // 
            this.txt_Min.Location = new System.Drawing.Point(103, 39);
            this.txt_Min.Name = "txt_Min";
            this.txt_Min.Size = new System.Drawing.Size(68, 20);
            this.txt_Min.TabIndex = 0;
            // 
            // txt_Max
            // 
            this.txt_Max.Location = new System.Drawing.Point(103, 65);
            this.txt_Max.Name = "txt_Max";
            this.txt_Max.Size = new System.Drawing.Size(68, 20);
            this.txt_Max.TabIndex = 1;
            // 
            // btn_Close
            // 
            this.btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Close.Location = new System.Drawing.Point(265, 7);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(109, 23);
            this.btn_Close.TabIndex = 3;
            this.btn_Close.Text = "닫기";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Play
            // 
            this.btn_Play.Location = new System.Drawing.Point(22, 33);
            this.btn_Play.Name = "btn_Play";
            this.btn_Play.Size = new System.Drawing.Size(109, 23);
            this.btn_Play.TabIndex = 2;
            this.btn_Play.Text = "벽 입력";
            this.btn_Play.Click += new System.EventHandler(this.btn_Play_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(63, 68);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(32, 14);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "MAX: ";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(66, 42);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(29, 14);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "MIN: ";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.chk_Top);
            this.panelControl1.Controls.Add(this.btn_Close);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 297);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(386, 35);
            this.panelControl1.TabIndex = 8;
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
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.groupControl2);
            this.xtraTabPage1.Controls.Add(this.groupControl1);
            this.xtraTabPage1.Controls.Add(this.groupControl10);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(384, 306);
            this.xtraTabPage1.Text = "폴리선";
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.btn_Play);
            this.groupControl2.Controls.Add(this.btn_keyline);
            this.groupControl2.Controls.Add(this.btn_Column);
            this.groupControl2.Controls.Add(this.btn_Beam);
            this.groupControl2.Location = new System.Drawing.Point(226, 17);
            this.groupControl2.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.groupControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(148, 243);
            this.groupControl2.TabIndex = 30;
            this.groupControl2.Text = "실행";
            // 
            // btn_keyline
            // 
            this.btn_keyline.Location = new System.Drawing.Point(22, 141);
            this.btn_keyline.Name = "btn_keyline";
            this.btn_keyline.Size = new System.Drawing.Size(109, 23);
            this.btn_keyline.TabIndex = 24;
            this.btn_keyline.Text = "키라인 입력";
            this.btn_keyline.Click += new System.EventHandler(this.btn_keyline_Click);
            // 
            // btn_Column
            // 
            this.btn_Column.Location = new System.Drawing.Point(22, 91);
            this.btn_Column.Name = "btn_Column";
            this.btn_Column.Size = new System.Drawing.Size(109, 23);
            this.btn_Column.TabIndex = 23;
            this.btn_Column.Text = "기둥 입력";
            this.btn_Column.Click += new System.EventHandler(this.btn_Column_Click);
            // 
            // btn_Beam
            // 
            this.btn_Beam.Location = new System.Drawing.Point(22, 62);
            this.btn_Beam.Name = "btn_Beam";
            this.btn_Beam.Size = new System.Drawing.Size(109, 23);
            this.btn_Beam.TabIndex = 22;
            this.btn_Beam.Text = "보 입력";
            this.btn_Beam.Click += new System.EventHandler(this.btn_Beam_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.txt_extend);
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.txt_dis2);
            this.groupControl1.Controls.Add(this.txt_distance);
            this.groupControl1.Location = new System.Drawing.Point(20, 122);
            this.groupControl1.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.groupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(200, 138);
            this.groupControl1.TabIndex = 29;
            this.groupControl1.Text = "키라인 설정";
            // 
            // txt_extend
            // 
            this.txt_extend.EditValue = "1000";
            this.txt_extend.Location = new System.Drawing.Point(103, 39);
            this.txt_extend.Name = "txt_extend";
            this.txt_extend.Size = new System.Drawing.Size(68, 20);
            this.txt_extend.TabIndex = 6;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(11, 94);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(84, 14);
            this.labelControl5.TabIndex = 28;
            this.labelControl5.Text = "평행 오차범위: ";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(39, 42);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(56, 14);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "연장길이: ";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(47, 68);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 14);
            this.labelControl4.TabIndex = 26;
            this.labelControl4.Text = "보 간격: ";
            // 
            // txt_dis2
            // 
            this.txt_dis2.EditValue = "10";
            this.txt_dis2.Location = new System.Drawing.Point(103, 91);
            this.txt_dis2.Name = "txt_dis2";
            this.txt_dis2.Size = new System.Drawing.Size(68, 20);
            this.txt_dis2.TabIndex = 27;
            // 
            // txt_distance
            // 
            this.txt_distance.EditValue = "1000";
            this.txt_distance.Location = new System.Drawing.Point(103, 65);
            this.txt_distance.Name = "txt_distance";
            this.txt_distance.Size = new System.Drawing.Size(68, 20);
            this.txt_distance.TabIndex = 25;
            // 
            // groupControl10
            // 
            this.groupControl10.Controls.Add(this.txt_Min);
            this.groupControl10.Controls.Add(this.txt_Max);
            this.groupControl10.Controls.Add(this.labelControl2);
            this.groupControl10.Controls.Add(this.labelControl1);
            this.groupControl10.Location = new System.Drawing.Point(20, 17);
            this.groupControl10.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.groupControl10.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupControl10.Name = "groupControl10";
            this.groupControl10.Size = new System.Drawing.Size(200, 99);
            this.groupControl10.TabIndex = 21;
            this.groupControl10.Text = "공통 너비 설정";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(384, 306);
            this.xtraTabPage2.Text = "None";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.xtraTabControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(386, 332);
            this.xtraTabControl1.TabIndex = 9;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // U001
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 332);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.xtraTabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "U001";
            this.Text = "U001";
            this.Load += new System.EventHandler(this.U001_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_Min.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Max.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chk_Top.Properties)).EndInit();
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_extend.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_dis2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_distance.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl10)).EndInit();
            this.groupControl10.ResumeLayout(false);
            this.groupControl10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txt_Min;
        private DevExpress.XtraEditors.TextEdit txt_Max;
        private DevExpress.XtraEditors.SimpleButton btn_Close;
        private DevExpress.XtraEditors.SimpleButton btn_Play;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckEdit chk_Top;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraEditors.GroupControl groupControl10;
        private DevExpress.XtraEditors.SimpleButton btn_Column;
        private DevExpress.XtraEditors.SimpleButton btn_Beam;
        private DevExpress.XtraEditors.SimpleButton btn_keyline;
        private DevExpress.XtraEditors.TextEdit txt_extend;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txt_distance;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txt_dis2;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
    }
}