#region .NET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
#endregion

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

namespace YT_CAD_TOOL.HDC
{
    public class CreateWalls
    {
        #region 필드
        Document acDoc;
        Database acDb;
        Editor acDocEd;
        DocumentLock acDL;
        #endregion

        #region 속성

        #endregion

        #region 생성자
        public CreateWalls()
        {
            #region PRE
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDb = acDoc.Database;
            acDL = acDoc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            acDocEd = acDoc.Editor;
            #endregion
        }
        #endregion

        #region PUBLIC 메서드
        public void Create_by_Polyline()
        {
            ReadData();

            var acSSet = Utils.Select.MultiObjs();

            var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));

            var LayerId = Utils.Layer.Create("_UNIT_Wall", ColorIndex.Yellow);

            acEnts.ForEach(acEnt =>
            {
                var acPolyline = acEnt as Polyline;

                Cad.Wall.Create(acPolyline, 150, LayerId);
            });
        }
        #endregion

        #region PRIVATE 메서드
        private void ReadData()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

        }
        #endregion
    }
}
