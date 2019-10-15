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
using Autodesk.Windows;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;
#endregion

namespace YT_CAD_TOOL.Utils
{
    public class DIMUtil
    {
        public static ObjectId Create(LineSegment3d L, double H, ObjectId DimStyleId, ObjectId LayerId)
        {
            ObjectId Return = ObjectId.Null;

            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var V = L.Direction;
                var cross_V = V.CrossProduct(Vector3d.ZAxis);

                var CP = CADUtil.GetCenterPoint3d(L);       // center point
                var DP = CADUtil.MoveP(CP, cross_V, H);     // dimension point

                var acDim = new AlignedDimension(L.StartPoint, L.EndPoint, DP, string.Empty, DimStyleId);

                acDim.DimensionStyle = DimStyleId;
                acDim.LayerId = LayerId;

                BTR.AppendEntity(acDim);
                T.AddNewlyCreatedDBObject(acDim, true);
                Return = acDim.Id;

                T.Commit();
            }
            #endregion

            return Return;
        }

        public static List<ObjectId> Create(Polyline acPoly, double H, bool updown, ObjectId DimStyleId, ObjectId LayerId)
        {
            List<ObjectId> Return = new List<ObjectId>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                for (int i = 0; i < acPoly.NumberOfVertices; i++)
                {
                    try
                    {
                        var L = acPoly.GetLineSegmentAt(i);
                        var V = L.Direction;
                        var cross_V = V.CrossProduct(Vector3d.ZAxis);
                        var MP = Utils.PointUtil.GetCenterP(L);          // Middle point

                        #region 폴리라인 내부에 입력되어야하는 경우
                        // updown이 true일 경우 폴리라인 외부, false인 경우 폴리라인 내부
                        if (!updown)
                        {
                            var CP = Utils.PointUtil.GetCenterP(acPoly);     // Center point

                            var p = Utils.PointUtil.Move(MP, cross_V);

                            var b = Utils.PointUtil.IsInsidePolyline(acPoly, p, false);

                            if (!b) cross_V = -cross_V;
                        }
                        #endregion

                        var DP = CADUtil.MoveP(MP, cross_V, H);     // dimension point

                        var acDim = new AlignedDimension(L.StartPoint, L.EndPoint, DP, string.Empty, DimStyleId);

                        Return.Add(acDim.Id);

                        acDim.DimensionStyle = DimStyleId;
                        acDim.LayerId = LayerId;

                        BTR.AppendEntity(acDim);
                        T.AddNewlyCreatedDBObject(acDim, true);
                    }
                    catch
                    {
                    }
                }

                T.Commit();
            }

            return Return;
        }
        public static List<ObjectId> Create(List<Point3d> points, double H, bool updown, ObjectId DimStyleId, ObjectId LayerId)
        {
            List<ObjectId> Return = new List<ObjectId>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                for (int i = 0; i < points.Count - 1; i++)
                {
                    try
                    {
                        var L = new LineSegment3d(points[i], points[i + 1]);
                        var V = L.Direction;
                        var cross_V = V.CrossProduct(Vector3d.ZAxis);
                        var MP = Utils.PointUtil.GetCenterP(L);          // Middle point

                        #region 폴리라인 내부에 입력되어야하는 경우
                        // updown이 true일 경우 폴리라인 외부, false인 경우 폴리라인 내부
                        if (!updown)
                        {
                            var CP = Utils.PointUtil.GetCenterP(points);     // Center point

                            var p = Utils.PointUtil.Move(MP, cross_V);

                            var P = new Polyline();

                            for (int j = 0; j < points.Count; j++)
                            {
                                P.AddVertexAt(j, PointUtil.To2D(points[j]), 0, 0, 0);
                            }

                            var b = Utils.PointUtil.IsInsidePolyline(P, p, false);

                            if (!b) cross_V = -cross_V;
                        }
                        #endregion

                        var DP = CADUtil.MoveP(MP, cross_V, H);     // dimension point

                        var acDim = new AlignedDimension(L.StartPoint, L.EndPoint, DP, string.Empty, DimStyleId);

                        acDim.DimensionStyle = DimStyleId;
                        acDim.LayerId = LayerId;

                        BTR.AppendEntity(acDim);
                        T.AddNewlyCreatedDBObject(acDim, true);

                        Return.Add(acDim.Id);
                    }
                    catch
                    {
                    }
                }

                T.Commit();
            }

            return Return;
        }
        /// <summary>
        /// 치수선 스타일 만들기
        /// </summary>
        /// <param name="StyleName"></param>
        public static ObjectId Create_DimensionStyle(string StyleName)
        {
            var DimId = ObjectId.Null;

            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                DimStyleTable DST = T.GetObject(AC.DB.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;

                if (!DST.Has(StyleName))
                {
                    #region 생성
                    DST.UpgradeOpen();

                    var dstr = new DimStyleTableRecord()
                    {
                        Name = StyleName,                 // 스타일 이름
                        Dimasz = 100,                   // 화살표 크기
                        Dimtxt = 50,                    // 문자 크기
                        Dimexo = 50,                    // 원점에서 간격띄우기 값
                        Dimclrt = ColorIndex.Yellow,    // 문자 색상
                        Dimclrd = ColorIndex.Yellow,    // 치수선 생상
                        Dimclre = ColorIndex.Gray,      // 치수보조선 색생
                        Dimtad = 0,                     // 1: 치수선 위에 입력, 0: 치수선 상에 입력
                        Dimtoh = false,                 // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
                        Dimtih = false,                 // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
                        Dimdec = 0,                     // 소수점 자릿수
                    };

                    DimId = DST.Add(dstr);

                    T.AddNewlyCreatedDBObject(dstr, true);
                    #endregion
                }
                else
                {
                    #region 수정
                    DimId = DST[StyleName];

                    var dstr = T.GetObject(DimId, OpenMode.ForWrite) as DimStyleTableRecord;

                    dstr.Name = StyleName;
                    dstr.Dimasz = 50;
                    dstr.Dimtxt = 50;
                    dstr.Dimexo = 0;
                    dstr.Dimgap = 50;
                    dstr.Dimclrt = ColorIndex.Yellow;
                    dstr.Dimclrd = ColorIndex.Yellow;
                    dstr.Dimclre = ColorIndex.Gray;
                    dstr.Dimtad = 0;
                    dstr.Dimtoh = false;
                    dstr.Dimtih = false;
                    dstr.Dimdec = 0;
                    #endregion
                }

                T.Commit();
            }
            #endregion

            return DimId;
        }

        public static ObjectId Create_HDC_DimensionStyle(string StyleName)
        {
            var DimId = ObjectId.Null;

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                DimStyleTable DST = T.GetObject(AC.DB.DimStyleTableId, OpenMode.ForWrite) as DimStyleTable;

                switch (StyleName)
                {
                    case "HDC_Dim_Style":

                        #region 생성
                        if (!DST.Has(StyleName))
                        {
                            DST.UpgradeOpen();

                            var dstr = new DimStyleTableRecord()
                            {
                                Name = StyleName,                 // 스타일 이름
                                Dimasz = 0,                   // 화살표 크기
                                Dimtxt = 100,                    // 문자 크기
                                Dimexo = 100,                    // 원점에서 간격띄우기 값
                                Dimclrt = ColorIndex.Yellow,    // 문자 색상
                                Dimclrd = ColorIndex.Yellow,    // 치수선 생상
                                Dimclre = ColorIndex.Gray,      // 치수보조선 색생
                                Dimtad = 0,                     // 1: 치수선 위에 입력, 0: 치수선 상에 입력
                                Dimtoh = false,                 // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
                                Dimtih = false,                 // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
                                Dimdec = 0,                     // 소수점 자릿수
                                Dimsd1 = true,                 // 치수선1 억제
                                Dimsd2 = true,                 // 치수선2 억제
                                Dimse1 = true,                 // 치수보조선1 억제
                                Dimse2 = true,                 // 치수보조선1 억제
                                Dimtix = true,
                            };

                            DimId = DST.Add(dstr);
                            T.AddNewlyCreatedDBObject(dstr, true);
                        }
                        #endregion

                        #region 수정
                        else
                        {
                            DimId = DST[StyleName];

                            var dstr = T.GetObject(DimId, OpenMode.ForWrite) as DimStyleTableRecord;

                            dstr.Name = StyleName;
                            dstr.Dimasz = 0;
                            dstr.Dimtxt = 100;
                            dstr.Dimexo = 100;
                            dstr.Dimgap = 50;
                            dstr.Dimclrt = ColorIndex.Yellow;
                            dstr.Dimclrd = ColorIndex.Yellow;
                            dstr.Dimclre = ColorIndex.Gray;
                            dstr.Dimtad = 0;
                            dstr.Dimtoh = false;
                            dstr.Dimtih = false;
                            dstr.Dimdec = 0;
                            dstr.Dimsd1 = true;                 // 치수선1 억제
                            dstr.Dimsd2 = true;                 // 치수선2 억제
                            dstr.Dimse1 = true;                 // 치수보조선1 억제
                            dstr.Dimse2 = true;                 // 치수보조선1 억제
                            dstr.Dimtix = true;
                        }
                        #endregion

                        T.Commit();
                        break;

                    case "HDC_도면 치수선":

                        #region 생성
                        if (!DST.Has(StyleName))
                        {
                            DST.UpgradeOpen();

                            var dstr = new DimStyleTableRecord();
                            dstr.Name = StyleName;                          // 스타일 이름

                            dstr.Dimasz = 10;                              // 화살표 크기
                            dstr.Dimblk = GetArrowObjectId("DIMBLK", "_ARCHTICK");
                            dstr.Dimblk1 = GetArrowObjectId("DIMBLK1", "_ARCHTICK");// 화살표 모양 (_ARCHTICK : 건축 눈금)
                            dstr.Dimblk2 = GetArrowObjectId("DIMBLK2", "_ARCHTICK");// 화살표 모양 (_ARCHTICK : 건축 눈금)

                            dstr.Dimtxt = 100;                              // 문자 크기
                            dstr.Dimexo = 150;                              // 원점에서 간격띄우기 값

                            dstr.Dimclrt = ColorIndex.Green;                // 문자 색상
                            dstr.Dimclrd = ColorIndex.RGB(255, 63, 0);       // 치수선 생상
                            dstr.Dimclre = ColorIndex.RGB(255, 63, 0);      // 치수보조선 색생

                            dstr.Dimtad = 1;                    // 1: 치수선 위에 입력, 0: 치수선 상에 입력
                            dstr.Dimtoh = false;               // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
                            dstr.Dimtih = false;                // 문자 정렬(On/On: 수평, Off/Off: 치수선, On/Off: ISO)
                            dstr.Dimdec = 0;                    // 소수점 자릿수
                            dstr.Dimsd1 = false;               // 치수선1 억제
                            dstr.Dimsd2 = false;               // 치수선2 억제
                            dstr.Dimse1 = false;                 // 치수보조선1 억제
                            dstr.Dimse2 = false;                // 치수보조선1 억제

                            DimId = DST.Add(dstr);

                            T.AddNewlyCreatedDBObject(dstr, true);
                        }
                        #endregion

                        #region 수정
                        else
                        {
                            DimId = DST[StyleName];

                            var dstr = T.GetObject(DimId, OpenMode.ForWrite) as DimStyleTableRecord;

                            dstr.Name = StyleName;

                            dstr.Dimasz = 10;
                            dstr.Dimblk = GetArrowObjectId("DIMBLK", "_ARCHTICK");
                            dstr.Dimblk1 = GetArrowObjectId("DIMBLK1", "_ARCHTICK");
                            dstr.Dimblk2 = GetArrowObjectId("DIMBLK2", "_ARCHTICK");

                            dstr.Dimtxt = 100;
                            dstr.Dimexo = 150;
                            dstr.Dimgap = 50;

                            dstr.Dimclrt = ColorIndex.Green;
                            dstr.Dimclrd = ColorIndex.RGB(255, 63, 0);
                            dstr.Dimclre = ColorIndex.RGB(255, 63, 0);

                            dstr.Dimtad = 1;
                            dstr.Dimtoh = false;
                            dstr.Dimtih = false;
                            dstr.Dimdec = 0;
                            dstr.Dimsd1 = false;                 // 치수선1 억제
                            dstr.Dimsd2 = false;                 // 치수선2 억제
                            dstr.Dimse1 = false;                 // 치수보조선1 억제
                            dstr.Dimse2 = false;                 // 치수보조선1 억제
                                                                 //dstr.Dimfxlen = 700;
                                                                 //dstr.Dimtix = true;
                        }
                        #endregion

                        T.Commit();
                        break;
                }
            }

            return DimId;
        }

        public static ObjectId GetArrowObjectId(string arrow)
        {
            ObjectId Return = ObjectId.Null;

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForWrite) as BlockTable;

                if (BT.Has(arrow))
                {
                    Return = BT[arrow];
                }
            }

            return Return;
        }

        public static ObjectId GetArrowObjectId(string arrow, string newArrName)
        {
            var Return = ObjectId.Null;

            string oldArrName = Application.GetSystemVariable(arrow).ToString();

            Application.SetSystemVariable(arrow, newArrName);

            if (oldArrName.Length != 0)
            {
                Application.SetSystemVariable(arrow, oldArrName);
            }

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForWrite) as BlockTable;

                //ObjectId d = new ObjectId(new IntPtr(165522592));
                //var A = t.GetObject(d, OpenMode.ForRead) as BlockTableRecord;
                //MessageBox.Show(A.Name);

                Return = BT[newArrName];
                t.Commit();
            }

            return Return;
        }

        /// <summary>
        /// 치수선 스타일 ID 받아오기
        /// </summary>
        /// <param name="StyleName"></param>
        /// <returns></returns>
        public static ObjectId GetDimStyleId(string StyleName)
        {
            var styleID = ObjectId.Null;

            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                DimStyleTable DST = T.GetObject(AC.DB.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;

                if (DST.Has(StyleName))
                {
                    styleID = DST[StyleName];
                }
            }
            #endregion

            return styleID;
        }

        public static Vector3d GetVector(AlignedDimension dim)
        {
            var p1 = dim.XLine1Point;
            var p2 = dim.XLine2Point;
            var ls = new LineSegment3d(p1, p2);
            var tp = dim.TextPosition;
            var Vec = Utils.Vector.Get(p1, p2);

            var rotatedVec = Vec.RotateBy(Math.PI / 2, Vector3d.ZAxis);

            var ip = CADUtil.GetIntersectedPoint(tp, rotatedVec, ls);

            return Utils.Vector.Get(ip, tp);
        }

        public static List<ObjectId> ReCreate(Polyline acPoly, ObjectId LayerID, ObjectId StyleID)
        {
            List<ObjectId> Return = new List<ObjectId>();

            #region 객체 받기
            var A = Utils.Get.PointCollection(acPoly);

            var acCPSSet = AC.Editor.SelectCrossingPolygon(A).Value;
            if (acCPSSet.Count == 0) return new List<ObjectId>();

            //var Texts = Utils.Get.GetEntity<DBText>(acCPSSet);
            var Dims = Utils.Get.GetEntity<AlignedDimension>(acCPSSet);
            #endregion

            #region 치수선 지우기 & 치수선 생성
            if (Dims.Any())
            {
                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    Dims.ToList().ForEach(Dim =>
                    {
                        var acDim = t.GetObject(Dim.Id, OpenMode.ForWrite) as AlignedDimension;

                        if (Utils.PointUtil.IsInsidePolyline(acPoly, acDim.TextPosition, true))
                        {
                            acDim.Erase(true);
                        }
                    });

                    t.Commit();
                }
            }

            Return.AddRange(Utils.DIMUtil.Create(A.Cast<Point3d>().ToList(), 100, false, StyleID, LayerID));
            #endregion

            return Return;
        }

        public static List<ObjectId> Create2(Polyline acPoly, ObjectId LayerID, ObjectId StyleID)
        {
            List<ObjectId> Return = new List<ObjectId>();

            #region 객체 받기
            var A = Utils.Get.PointCollection(acPoly);

            var acCPSSet = AC.Editor.SelectCrossingPolygon(A).Value;
            if (acCPSSet.Count == 0) return new List<ObjectId>();

            var Texts = Utils.Get.GetEntity<DBText>(acCPSSet);
            var Dims = Utils.Get.GetEntity<AlignedDimension>(acCPSSet);
            #endregion

            #region 면적 지우기
            if (Texts.Any())
            {
                var acTexts = Texts.Where(x => Utils.Get.IsDouble(x.TextString)).ToList();

                if (acTexts.Count == 1)
                {
                    using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                    {
                        acTexts.ForEach(text =>
                        {
                            var B = t.GetObject(text.Id, OpenMode.ForWrite) as DBText;

                            B.Erase(true);

                            t.Commit();
                        });
                    }
                }
            }
            #endregion

            #region 치수선 지우기 & 치수선 생성
            if (Dims.Any())
            {
                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    Dims.ToList().ForEach(Dim =>
                    {
                        var acDim = t.GetObject(Dim.Id, OpenMode.ForWrite) as AlignedDimension;

                        if (Utils.PointUtil.IsInsidePolyline(acPoly, acDim.TextPosition, true))
                        {
                            acDim.Erase(true);
                        }
                    });

                    t.Commit();
                }
            }

            Return.AddRange(Utils.DIMUtil.Create(A.Cast<Point3d>().ToList(), 100, false, StyleID, LayerID));
            #endregion

            #region 새로 면적 입력
            var p = Utils.PointUtil.Move(Utils.PointUtil.GetCenterP(acPoly), 0, -100);

            var PL_Area = acPoly.Area;  // mm²

            var Area = Math.Round(PL_Area / 1000000, 3); // m²

            var acText = CADUtil.CreateText(p, 200, Area.ToString(), ColorIndex.Red);
            //var acText = CAD.CreateText(p, 100, Area.ToString() + "m²", TxtStyleId, ColorIndex.Green);

            Return.Add(acText.Id);
            #endregion

            return Return;
        }
    }


    public class Point3dComparer : IEqualityComparer<Point3d>
    {
        public bool Equals(Point3d x, Point3d y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            return x != null && y != null &&
                   x.X == y.X &&
                   x.Y == y.Y &&
                   x.Z == y.Z;
        }

        public int GetHashCode(Point3d obj)
        {
            return (obj.X.Equals(null) ? 0 : obj.X.GetHashCode()) ^
                   (obj.Y.Equals(null) ? 0 : obj.Y.GetHashCode()) ^
                   (obj.Z.Equals(null) ? 0 : obj.Z.GetHashCode());
        }
    }
}



