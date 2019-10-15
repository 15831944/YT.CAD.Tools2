namespace YT_CAD_TOOL
{
    partial class U_Room
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U_Room));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.sb_Setting = new DevExpress.XtraEditors.SimpleButton();
            this.sb_Refresh = new DevExpress.XtraEditors.SimpleButton();
            this.sb_Link = new DevExpress.XtraEditors.SimpleButton();
            this.ce_Refresh = new DevExpress.XtraEditors.CheckEdit();
            this.GC = new DevExpress.XtraGrid.GridControl();
            this.GV = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.beLink = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.sb_Select = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ce_Refresh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beLink)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.sb_Select);
            this.panelControl1.Controls.Add(this.simpleButton2);
            this.panelControl1.Controls.Add(this.sb_Setting);
            this.panelControl1.Controls.Add(this.sb_Refresh);
            this.panelControl1.Controls.Add(this.sb_Link);
            this.panelControl1.Controls.Add(this.ce_Refresh);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(3);
            this.panelControl1.Size = new System.Drawing.Size(338, 40);
            this.panelControl1.TabIndex = 4;
            // 
            // simpleButton2
            // 
            this.simpleButton2.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.simpleButton2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton2.ImageOptions.Image")));
            this.simpleButton2.Location = new System.Drawing.Point(6, 9);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(23, 23);
            this.simpleButton2.TabIndex = 4;
            // 
            // sb_Setting
            // 
            this.sb_Setting.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Setting.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sb_Setting.ImageOptions.Image")));
            this.sb_Setting.Location = new System.Drawing.Point(35, 9);
            this.sb_Setting.Name = "sb_Setting";
            this.sb_Setting.Size = new System.Drawing.Size(23, 23);
            this.sb_Setting.TabIndex = 3;
            // 
            // sb_Refresh
            // 
            this.sb_Refresh.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Refresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sb_Refresh.ImageOptions.Image")));
            this.sb_Refresh.Location = new System.Drawing.Point(154, 9);
            this.sb_Refresh.Name = "sb_Refresh";
            this.sb_Refresh.Size = new System.Drawing.Size(69, 23);
            this.sb_Refresh.TabIndex = 2;
            this.sb_Refresh.Text = "Refresh";
            // 
            // sb_Link
            // 
            this.sb_Link.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Link.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("sb_Link.ImageOptions.Image")));
            this.sb_Link.Location = new System.Drawing.Point(93, 9);
            this.sb_Link.Name = "sb_Link";
            this.sb_Link.Size = new System.Drawing.Size(55, 23);
            this.sb_Link.TabIndex = 1;
            this.sb_Link.Text = "Link";
            // 
            // ce_Refresh
            // 
            this.ce_Refresh.Location = new System.Drawing.Point(233, 11);
            this.ce_Refresh.Name = "ce_Refresh";
            this.ce_Refresh.Properties.Caption = "Auto Refresh";
            this.ce_Refresh.Size = new System.Drawing.Size(97, 19);
            this.ce_Refresh.TabIndex = 0;
            // 
            // GC
            // 
            this.GC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GC.Location = new System.Drawing.Point(0, 40);
            this.GC.MainView = this.GV;
            this.GC.Name = "GC";
            this.GC.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.beLink});
            this.GC.Size = new System.Drawing.Size(338, 522);
            this.GC.TabIndex = 3;
            this.GC.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GV});
            // 
            // GV
            // 
            this.GV.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6});
            this.GV.GridControl = this.GC;
            this.GV.Name = "GV";
            this.GV.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
            this.GV.OptionsSelection.MultiSelect = true;
            this.GV.OptionsView.ColumnAutoWidth = false;
            this.GV.OptionsView.ShowFooter = true;
            this.GV.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = " ";
            this.gridColumn3.FieldName = "IsLink";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.FixedWidth = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 30;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "번호";
            this.gridColumn1.FieldName = "No";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 50;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "실명";
            this.gridColumn2.FieldName = "Name";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "Name", "실 개수 : {0}")});
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 100;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "가로";
            this.gridColumn4.FieldName = "Width";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "세로";
            this.gridColumn5.FieldName = "Height";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "면적";
            this.gridColumn6.DisplayFormat.FormatString = "{0:N4}";
            this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn6.FieldName = "Area";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Area", "{0:N2}")});
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 3;
            this.gridColumn6.Width = 80;
            // 
            // beLink
            // 
            this.beLink.AutoHeight = false;
            this.beLink.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.beLink.Name = "beLink";
            this.beLink.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // sb_Select
            // 
            this.sb_Select.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.sb_Select.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.sb_Select.Location = new System.Drawing.Point(64, 9);
            this.sb_Select.Name = "sb_Select";
            this.sb_Select.Size = new System.Drawing.Size(23, 23);
            this.sb_Select.TabIndex = 5;
            // 
            // U_Room
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 562);
            this.Controls.Add(this.GC);
            this.Controls.Add(this.panelControl1);
            this.Name = "U_Room";
            this.Text = "실 면적 검토";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ce_Refresh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beLink)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton sb_Refresh;
        private DevExpress.XtraEditors.SimpleButton sb_Link;
        private DevExpress.XtraEditors.CheckEdit ce_Refresh;
        private DevExpress.XtraGrid.GridControl GC;
        private DevExpress.XtraGrid.Views.Grid.GridView GV;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit beLink;
        private DevExpress.XtraEditors.SimpleButton sb_Setting;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton sb_Select;
    }
}