#region .NET
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion

#region CAD
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;
#endregion

namespace YT_CAD_TOOL
{
    public partial class MenuPalette : DevExpress.XtraEditors.XtraUserControl
    {
        //private Page_Tool m_Page_Tool = null;
        private Page_1_Tool m_Page_1_Tool = null;

        //public Page_Tool Page_Tool { get { return m_Page_Tool; } }
        public Page_1_Tool Page_Tool1 { get { return m_Page_1_Tool; } }

        public MenuPalette()
        {
            AC.Initialize();
            DB.Initialize();

            InitializeComponent();

            m_Page_1_Tool = new Page_1_Tool();

            this.xtraTabPage1.Controls.Add(m_Page_1_Tool);
            m_Page_1_Tool.Parent = this.xtraTabPage1;
            m_Page_1_Tool.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            m_Page_1_Tool.VerticalScroll.Enabled = true;

            m_Page_1_Tool.Show();
        }
    }
}
