namespace YT_CAD_TOOL
{
    partial class U005
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U005));
            this.GC = new DevExpress.XtraGrid.GridControl();
            this.GV = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.sbOk = new DevExpress.XtraEditors.SimpleButton();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbSelect = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.GC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GC
            // 
            this.GC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GC.Location = new System.Drawing.Point(0, 0);
            this.GC.MainView = this.GV;
            this.GC.Name = "GC";
            this.GC.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinEdit1});
            this.GC.Size = new System.Drawing.Size(283, 418);
            this.GC.TabIndex = 0;
            this.GC.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GV});
            // 
            // GV
            // 
            this.GV.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.GV.GridControl = this.GC;
            this.GV.Name = "GV";
            this.GV.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
            this.GV.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "No.";
            this.gridColumn1.FieldName = "P1";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.FixedWidth = true;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 50;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "실명";
            this.gridColumn2.FieldName = "P2";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 68;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "개수";
            this.gridColumn3.ColumnEdit = this.repositoryItemSpinEdit1;
            this.gridColumn3.FieldName = "P3";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.FixedWidth = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 70;
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.sbSelect);
            this.panelControl1.Controls.Add(this.sbOk);
            this.panelControl1.Controls.Add(this.sbCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 418);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(3);
            this.panelControl1.Size = new System.Drawing.Size(283, 40);
            this.panelControl1.TabIndex = 1;
            // 
            // sbOk
            // 
            this.sbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbOk.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sbOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbOk.ImageOptions.Image")));
            this.sbOk.Location = new System.Drawing.Point(149, 8);
            this.sbOk.Name = "sbOk";
            this.sbOk.Size = new System.Drawing.Size(60, 23);
            this.sbOk.TabIndex = 1;
            this.sbOk.Text = "확인";
            // 
            // sbCancel
            // 
            this.sbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sbCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sbCancel.ImageOptions.Image")));
            this.sbCancel.Location = new System.Drawing.Point(215, 8);
            this.sbCancel.Name = "sbCancel";
            this.sbCancel.Size = new System.Drawing.Size(60, 23);
            this.sbCancel.TabIndex = 0;
            this.sbCancel.Text = "취소";
            // 
            // sbSelect
            // 
            this.sbSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbSelect.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sbSelect.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.sbSelect.Location = new System.Drawing.Point(12, 8);
            this.sbSelect.Name = "sbSelect";
            this.sbSelect.Size = new System.Drawing.Size(60, 23);
            this.sbSelect.TabIndex = 2;
            this.sbSelect.Text = "선택";
            // 
            // U005
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 458);
            this.Controls.Add(this.GC);
            this.Controls.Add(this.panelControl1);
            this.Name = "U005";
            this.Text = "U005";
            ((System.ComponentModel.ISupportInitialize)(this.GC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl GC;
        private DevExpress.XtraGrid.Views.Grid.GridView GV;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton sbCancel;
        private DevExpress.XtraEditors.SimpleButton sbOk;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraEditors.SimpleButton sbSelect;
    }
}