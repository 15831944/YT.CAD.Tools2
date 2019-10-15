namespace YT_CAD_TOOL
{
    partial class U_Msgbox
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.sb_cancle = new DevExpress.XtraEditors.SimpleButton();
            this.sb_ok = new DevExpress.XtraEditors.SimpleButton();
            this.lc_msg = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelControl1.Controls.Add(this.sb_cancle);
            this.panelControl1.Controls.Add(this.sb_ok);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 62);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(3);
            this.panelControl1.Size = new System.Drawing.Size(234, 40);
            this.panelControl1.TabIndex = 2;
            // 
            // sb_cancle
            // 
            this.sb_cancle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sb_cancle.Location = new System.Drawing.Point(149, 6);
            this.sb_cancle.Name = "sb_cancle";
            this.sb_cancle.Size = new System.Drawing.Size(75, 23);
            this.sb_cancle.TabIndex = 1;
            this.sb_cancle.Text = "취소";
            // 
            // sb_ok
            // 
            this.sb_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sb_ok.Location = new System.Drawing.Point(68, 7);
            this.sb_ok.Name = "sb_ok";
            this.sb_ok.Size = new System.Drawing.Size(75, 23);
            this.sb_ok.TabIndex = 0;
            this.sb_ok.Text = "확인";
            // 
            // lc_msg
            // 
            this.lc_msg.Location = new System.Drawing.Point(12, 26);
            this.lc_msg.Name = "lc_msg";
            this.lc_msg.Size = new System.Drawing.Size(70, 14);
            this.lc_msg.TabIndex = 3;
            this.lc_msg.Text = "labelControl1";
            // 
            // U_Msgbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 102);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.lc_msg);
            this.Location = new System.Drawing.Point(12, 27);
            this.Name = "U_Msgbox";
            this.Text = "MessageBox";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton sb_cancle;
        private DevExpress.XtraEditors.SimpleButton sb_ok;
        private DevExpress.XtraEditors.LabelControl lc_msg;
    }
}