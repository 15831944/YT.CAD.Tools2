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
    public class CreateDimension
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
        public CreateDimension()
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
        public void CreateAtPolyline()
        {
            #region 선택
            var acPolylines = new List<Polyline>();

            var acSSet = Utils.Select.MultiObjs(new SelectionFilter(new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            }));

            if (acSSet == null)
            {
                acDocEd.PostCommandPrompt();
                return;
            }

            var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));
            #endregion

            #region 생성
            var acDimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Green);    // 레이어 생성
            var acDimStyleId = Utils.DIMUtil.Create_DimensionStyle("HDC_Style");             // 치수 스타일 생성

            acEnts.ForEach(acEnt =>
            {
                var acPolyline = acEnt as Polyline;

                for (int i = 0; i < acPolyline.NumberOfVertices; i++)
                {
                    var acLine = acPolyline.GetLineSegmentAt(i);

                    Utils.DIMUtil.Create(acLine, 50, acDimStyleId, acDimLayerId);
                }
            });
            #endregion
        }
        #endregion

        #region PRIVATE 메서드
        #endregion
    }
}
