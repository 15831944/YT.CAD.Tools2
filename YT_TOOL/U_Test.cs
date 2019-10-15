using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#region AUTOCAD
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;
using Autodesk.Windows;
#endregion
namespace YT_CAD_TOOL
{
    public partial class U_Test : Form
    {
        public List<Temp> Temps { get; set; }

        public U_Test()
        {
            InitializeComponent();
        }

        public U_Test(List<Temp> temps)
        {
            InitializeComponent();

            this.Temps = new List<Temp>();
            this.Temps = temps;

            this.TopMost = true;

            this.Load += U_Test_Load;
            this.GV.SelectionChanged += GV_SelectionChanged;
            this.GV.FocusedRowChanged += GV_FocusedRowChanged;
        }

        private void GV_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var R = GV.GetRow(e.FocusedRowHandle) as Temp;

            var handle = new Handle(long.Parse(R.Handle.ToString()));

            var objID = new ObjectId(); AC.DB.TryGetObjectId(handle, out objID);

            if (objID == ObjectId.Null) return;

            var objIDs = new ObjectId[] { objID };

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                AC.Editor.SetImpliedSelection(objIDs);

                t.Commit();
            }

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
        }

        private void GV_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //var R = GV.GetRow(e.ControllerRow) as DataRow;

            //var handle = new Handle(long.Parse(R["Handle"].ToString()));

            //MessageBox.Show(handle.Value.ToString());

            //var objID = AC.DB.GetObjectId(false, handle, 0);

            //var objIDs = new ObjectId[] { objID };

            //MessageBox.Show("a");

            //using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            //{
            //    AC.Editor.SetImpliedSelection(objIDs);
            //    t.Commit();
            //}
        }

        private void U_Test_Load(object sender, EventArgs e)
        {
            GC.DataSource = Temps;
            GC.Update();
        }
    }
}
