namespace YT_CAD_TOOL
{
    partial class MenuPalette
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.sidePanel1 = new DevExpress.XtraEditors.SidePanel();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.sidePanel2 = new DevExpress.XtraEditors.SidePanel();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.sidePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidePanel1
            // 
            this.sidePanel1.Controls.Add(this.pictureEdit1);
            this.sidePanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sidePanel1.Location = new System.Drawing.Point(0, 720);
            this.sidePanel1.Name = "sidePanel1";
            this.sidePanel1.Size = new System.Drawing.Size(320, 30);
            this.sidePanel1.TabIndex = 0;
            this.sidePanel1.Text = "sidePanel1";
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEdit1.EditValue = global::YT_CAD_TOOL.Properties.Resources.연우테크놀로지_로고;
            this.pictureEdit1.Location = new System.Drawing.Point(0, 1);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.PictureAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.pictureEdit1.Size = new System.Drawing.Size(320, 29);
            this.pictureEdit1.TabIndex = 0;
            // 
            // sidePanel2
            // 
            this.sidePanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.sidePanel2.Location = new System.Drawing.Point(0, 0);
            this.sidePanel2.MaximumSize = new System.Drawing.Size(0, 30);
            this.sidePanel2.MinimumSize = new System.Drawing.Size(320, 30);
            this.sidePanel2.Name = "sidePanel2";
            this.sidePanel2.Size = new System.Drawing.Size(320, 30);
            this.sidePanel2.TabIndex = 1;
            this.sidePanel2.Text = "sidePanel2";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 30);
            this.xtraTabControl1.LookAndFeel.SkinName = "Office 2013 Dark Gray";
            this.xtraTabControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(320, 690);
            this.xtraTabControl1.TabIndex = 2;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Appearance.PageClient.BackColor = System.Drawing.Color.White;
            this.xtraTabPage1.Appearance.PageClient.BackColor2 = System.Drawing.Color.Black;
            this.xtraTabPage1.Appearance.PageClient.BorderColor = System.Drawing.Color.LightSteelBlue;
            this.xtraTabPage1.Appearance.PageClient.ForeColor = System.Drawing.Color.Black;
            this.xtraTabPage1.Appearance.PageClient.Options.UseBackColor = true;
            this.xtraTabPage1.Appearance.PageClient.Options.UseBorderColor = true;
            this.xtraTabPage1.Appearance.PageClient.Options.UseForeColor = true;
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(318, 664);
            this.xtraTabPage1.Text = "YT_CAD_TOOL";
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(318, 664);
            this.xtraTabPage2.Text = "xtraTabPage2";
            // 
            // MenuPalette
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.sidePanel2);
            this.Controls.Add(this.sidePanel1);
            this.Name = "MenuPalette";
            this.Size = new System.Drawing.Size(320, 750);
            this.sidePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SidePanel sidePanel1;
        private DevExpress.XtraEditors.SidePanel sidePanel2;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
    }
}
