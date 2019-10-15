namespace CADAUTO
{
    partial class UI
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
            this.btn_Line = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Curve = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Rectangle = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Arc = new DevExpress.XtraEditors.SimpleButton();
            this.txt_W = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txt_H = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txt_D = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txt_R1 = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txt_R2 = new DevExpress.XtraEditors.TextEdit();
            this.btn_zoom = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txt_zoom = new DevExpress.XtraEditors.TextEdit();
            this.btn_Play = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txt_W.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_H.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_D.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_R1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_R2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_zoom.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Line
            // 
            this.btn_Line.Location = new System.Drawing.Point(426, 158);
            this.btn_Line.Name = "btn_Line";
            this.btn_Line.Size = new System.Drawing.Size(75, 23);
            this.btn_Line.TabIndex = 0;
            this.btn_Line.Text = "Line";
            this.btn_Line.Click += new System.EventHandler(this.btn_Line_Click);
            // 
            // btn_Curve
            // 
            this.btn_Curve.Location = new System.Drawing.Point(426, 187);
            this.btn_Curve.Name = "btn_Curve";
            this.btn_Curve.Size = new System.Drawing.Size(75, 23);
            this.btn_Curve.TabIndex = 1;
            this.btn_Curve.Text = "Curve";
            this.btn_Curve.Click += new System.EventHandler(this.btn_Curve_Click);
            // 
            // btn_Rectangle
            // 
            this.btn_Rectangle.Location = new System.Drawing.Point(426, 216);
            this.btn_Rectangle.Name = "btn_Rectangle";
            this.btn_Rectangle.Size = new System.Drawing.Size(75, 23);
            this.btn_Rectangle.TabIndex = 2;
            this.btn_Rectangle.Text = "Rectangle";
            this.btn_Rectangle.Click += new System.EventHandler(this.btn_Rectangle_Click);
            // 
            // btn_Arc
            // 
            this.btn_Arc.Location = new System.Drawing.Point(426, 245);
            this.btn_Arc.Name = "btn_Arc";
            this.btn_Arc.Size = new System.Drawing.Size(75, 23);
            this.btn_Arc.TabIndex = 3;
            this.btn_Arc.Text = "Arc";
            this.btn_Arc.Click += new System.EventHandler(this.btn_Arc_Click);
            // 
            // txt_W
            // 
            this.txt_W.EditValue = "100";
            this.txt_W.Location = new System.Drawing.Point(229, 159);
            this.txt_W.Name = "txt_W";
            this.txt_W.Size = new System.Drawing.Size(100, 20);
            this.txt_W.TabIndex = 4;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(165, 162);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(45, 14);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "Width:  ";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(165, 187);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(43, 14);
            this.labelControl2.TabIndex = 7;
            this.labelControl2.Text = "Heigh:  ";
            // 
            // txt_H
            // 
            this.txt_H.Location = new System.Drawing.Point(229, 184);
            this.txt_H.Name = "txt_H";
            this.txt_H.Size = new System.Drawing.Size(100, 20);
            this.txt_H.TabIndex = 6;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(165, 213);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(58, 14);
            this.labelControl3.TabIndex = 9;
            this.labelControl3.Text = "Distance:  ";
            // 
            // txt_D
            // 
            this.txt_D.Location = new System.Drawing.Point(229, 210);
            this.txt_D.Name = "txt_D";
            this.txt_D.Size = new System.Drawing.Size(100, 20);
            this.txt_D.TabIndex = 8;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(165, 239);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(26, 14);
            this.labelControl4.TabIndex = 11;
            this.labelControl4.Text = "R1:  ";
            // 
            // txt_R1
            // 
            this.txt_R1.Location = new System.Drawing.Point(229, 236);
            this.txt_R1.Name = "txt_R1";
            this.txt_R1.Size = new System.Drawing.Size(100, 20);
            this.txt_R1.TabIndex = 10;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(163, 265);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(26, 14);
            this.labelControl5.TabIndex = 13;
            this.labelControl5.Text = "R2:  ";
            // 
            // txt_R2
            // 
            this.txt_R2.Location = new System.Drawing.Point(229, 262);
            this.txt_R2.Name = "txt_R2";
            this.txt_R2.Size = new System.Drawing.Size(100, 20);
            this.txt_R2.TabIndex = 12;
            // 
            // btn_zoom
            // 
            this.btn_zoom.Location = new System.Drawing.Point(426, 274);
            this.btn_zoom.Name = "btn_zoom";
            this.btn_zoom.Size = new System.Drawing.Size(75, 23);
            this.btn_zoom.TabIndex = 14;
            this.btn_zoom.Text = "Zoom";
            this.btn_zoom.Click += new System.EventHandler(this.btn_zoom_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(163, 291);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(43, 14);
            this.labelControl6.TabIndex = 16;
            this.labelControl6.Text = "Zoom:  ";
            // 
            // txt_zoom
            // 
            this.txt_zoom.Location = new System.Drawing.Point(229, 288);
            this.txt_zoom.Name = "txt_zoom";
            this.txt_zoom.Size = new System.Drawing.Size(100, 20);
            this.txt_zoom.TabIndex = 15;
            // 
            // btn_Play
            // 
            this.btn_Play.Location = new System.Drawing.Point(426, 409);
            this.btn_Play.Name = "btn_Play";
            this.btn_Play.Size = new System.Drawing.Size(75, 23);
            this.btn_Play.TabIndex = 17;
            this.btn_Play.Text = "실행";
            this.btn_Play.Click += new System.EventHandler(this.btn_Play_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(507, 409);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(75, 23);
            this.btn_Close.TabIndex = 18;
            this.btn_Close.Text = "닫기";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 606);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Play);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.txt_zoom);
            this.Controls.Add(this.btn_zoom);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.txt_R2);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.txt_R1);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.txt_D);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txt_H);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txt_W);
            this.Controls.Add(this.btn_Arc);
            this.Controls.Add(this.btn_Rectangle);
            this.Controls.Add(this.btn_Curve);
            this.Controls.Add(this.btn_Line);
            this.Name = "UI";
            this.Text = "UI";
            this.Load += new System.EventHandler(this.UI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_W.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_H.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_D.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_R1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_R2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_zoom.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btn_Line;
        private DevExpress.XtraEditors.SimpleButton btn_Curve;
        private DevExpress.XtraEditors.SimpleButton btn_Rectangle;
        private DevExpress.XtraEditors.SimpleButton btn_Arc;
        private DevExpress.XtraEditors.TextEdit txt_W;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txt_H;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txt_D;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txt_R1;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txt_R2;
        private DevExpress.XtraEditors.SimpleButton btn_zoom;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txt_zoom;
        private DevExpress.XtraEditors.SimpleButton btn_Play;
        private DevExpress.XtraEditors.SimpleButton btn_Close;
    }
}