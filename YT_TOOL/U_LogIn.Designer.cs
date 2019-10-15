namespace YT_CAD_TOOL
{
    partial class U_LogIn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U_LogIn));
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.sb_login = new DevExpress.XtraEditors.SimpleButton();
            this.te_pw = new DevExpress.XtraEditors.TextEdit();
            this.te_id = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_pw.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_id.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = global::YT_CAD_TOOL.Properties.Resources.연우테크놀로지_로고;
            this.pictureEdit1.Location = new System.Drawing.Point(12, 12);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.pictureEdit1.Size = new System.Drawing.Size(246, 88);
            this.pictureEdit1.TabIndex = 10;
            // 
            // sb_login
            // 
            this.sb_login.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.sb_login.Appearance.Options.UseBackColor = true;
            this.sb_login.Location = new System.Drawing.Point(12, 158);
            this.sb_login.Name = "sb_login";
            this.sb_login.Size = new System.Drawing.Size(246, 35);
            this.sb_login.TabIndex = 9;
            this.sb_login.Text = "login";
            // 
            // te_pw
            // 
            this.te_pw.Location = new System.Drawing.Point(12, 132);
            this.te_pw.Name = "te_pw";
            this.te_pw.Properties.Appearance.Options.UseTextOptions = true;
            this.te_pw.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.te_pw.Properties.PasswordChar = '*';
            this.te_pw.Size = new System.Drawing.Size(246, 20);
            this.te_pw.TabIndex = 8;
            // 
            // te_id
            // 
            this.te_id.Location = new System.Drawing.Point(12, 106);
            this.te_id.Name = "te_id";
            this.te_id.Properties.Appearance.Options.UseTextOptions = true;
            this.te_id.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.te_id.Size = new System.Drawing.Size(246, 20);
            this.te_id.TabIndex = 7;
            // 
            // U_LogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 208);
            this.Controls.Add(this.pictureEdit1);
            this.Controls.Add(this.sb_login);
            this.Controls.Add(this.te_pw);
            this.Controls.Add(this.te_id);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "U_LogIn";
            this.Text = "LogIn";
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_pw.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_id.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.SimpleButton sb_login;
        private DevExpress.XtraEditors.TextEdit te_pw;
        private DevExpress.XtraEditors.TextEdit te_id;
    }
}