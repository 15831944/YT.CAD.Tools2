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

#region PROJECT
#endregion

namespace YT_CAD_TOOL
{
    public class CreateDimensions
    {
        #region 속성
        #endregion

        #region 생성자
        public CreateDimensions()
        {
        }
        #endregion

        #region PUBLIC 메서드
        public void CreatePolylineDimensions()
        {
            #region 선택
            var acPolylines = new List<Polyline>();

            var acSSet = SelectPolylines();

            if (acSSet == null)
            {
                AC.Editor.PostCommandPrompt();
                return;
            }

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var Polylines = from id in acSSet.GetObjectIds()
                                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                where acEnt is Polyline
                                let acPolyLine = acEnt as Polyline
                                select acPolyLine;

                if (!Polylines.Any())
                    return;

                acPolylines.AddRange(Polylines.ToArray());
            }
            #endregion

            acPolylines.ForEach(acPolyline =>
            {
                CrateDimension(acPolyline);
            });
        }
        #endregion

        #region PRIVATE 메서드

        private SelectionSet SelectPolylines()
        {
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            TypedValue[] tvs = new TypedValue[]
            {
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionResult acPSR = acDocEd.GetSelection(oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                acDocEd.WriteMessage("\n선택이 취소되었습니다.");
                acDocEd.PostCommandPrompt();
                return null;
            }

            return acPSR.Value;
        }

        public void CreateDimensionStyle()
        {
            var StyleName = "Temp_01";
            var dimstyleID = Utils.DIMUtil.Create_DimensionStyle(StyleName);

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var DST = T.GetObject(AC.DB.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
                var DSTR = T.GetObject(dimstyleID, OpenMode.ForWrite) as DimStyleTableRecord;

                DSTR.Dimasz = 50;
                DSTR.Dimtxt = 50;
                DSTR.Dimexo = 0;
                DSTR.Dimgap = 50;
                DSTR.Dimclrt = ColorIndex.Yellow;
                DSTR.Dimclrd = ColorIndex.Yellow;
                DSTR.Dimclre = ColorIndex.Gray;
                DSTR.Dimtad = 0;
                DSTR.Dimtoh = false;
                DSTR.Dimtih = false;
                DSTR.Dimdec = 0;

                T.Commit();
            }

            //using (Transaction T = acDb.TransactionManager.StartTransaction())
            //{
            //    DimStyleTable DST = T.GetObject(acDb.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;

            //    ObjectId dimId = ObjectId.Null;

            //    string dimName = "Test7";

            //    if (!DST.Has(dimName))
            //    {
            //        DST.UpgradeOpen();

            //        var dstr = new DimStyleTableRecord()
            //        {
            //            Name = dimName,                 // 스타일 이름
            //            Dimasz = 100,                   // 화살표 크기
            //            Dimtxt = 50,                   // 문자 크기
            //            Dimexo = 50,                   // 원점에서 간격띄우기 값
            //            Dimclrt = ColorIndex.Yellow,    // 문자 색상
            //            Dimclrd = ColorIndex.Yellow,    // 치수선 생상
            //            Dimtad = 0,                     // 1: 치수선 위에 입력, 0: 치수선 상에 입력
            //            Dimtoh = false,                 // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
            //            Dimtih = false,                 // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
            //            Dimdec = 2,                     // 소수점 자릿수
            //        };

            //        dimId = DST.Add(dstr);

            //        T.AddNewlyCreatedDBObject(dstr, true);
            //    }
            //    else
            //    {
            //        dimId = DST[dimName];

            //        var dstr = T.GetObject(dimId, OpenMode.ForWrite) as DimStyleTableRecord;

            //        dstr.Name = dimName;
            //        dstr.Dimasz = 50;
            //        dstr.Dimtxt = 50;
            //        dstr.Dimexo = 0;
            //        dstr.Dimgap = 50;
            //        dstr.Dimclrt = ColorIndex.Yellow;
            //        dstr.Dimclrd = ColorIndex.Yellow;
            //        dstr.Dimclre = ColorIndex.Gray;
            //        dstr.Dimtad = 0;
            //        dstr.Dimtoh = false;
            //        dstr.Dimtih = false;
            //        dstr.Dimdec = 0;
            //    }

            //    var DSTR = T.GetObject(dimId, OpenMode.ForRead) as DimStyleTableRecord;

            //    if (DSTR.ObjectId != acDb.Dimstyle)
            //    {
            //        acDb.Dimstyle = DSTR.ObjectId;
            //        acDb.SetDimstyleData(DSTR);
            //    }

            //    T.Commit();
            //}
        }

        private void CrateDimension(Polyline acPolyline)
        {
            var dimensionLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Green);

            for (int i = 0; i < acPolyline.NumberOfVertices; i++)
            {
                try
                {
                    var acLine = acPolyline.GetLineSegmentAt(i);

                    Utils.DIMUtil.Create(acLine, 50, AC.DB.Dimstyle, dimensionLayerId);
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
