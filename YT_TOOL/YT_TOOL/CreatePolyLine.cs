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
    public class CreatePolyLine
    {
        #region 필드
        #endregion

        #region 속성
        double Min;
        double Max;
        double extend;
        double distance;
        double distance2;
        #endregion

        #region 생성자
        public CreatePolyLine()
        {
            Min = double.Parse(Properties.Settings.Default.t_min);
            Max = double.Parse(Properties.Settings.Default.t_max);
            extend = double.Parse(Properties.Settings.Default.t_extend);
            distance = double.Parse(Properties.Settings.Default.t_distance);
            distance2 = double.Parse(Properties.Settings.Default.t_distance2);
        }
        #endregion

        #region PUBLIC 메서드

        public void CreateWallPolyLine()
        {
            bool B = true;

            do
            {
                try
                {
                    #region 선택한 폴리선에서 각 선들 받기 [edgePtrs]
                    var edgePtrs = new Curve2dCollection();
                    var edgeTypes = new IntegerCollection();

                    select.Objects<Polyline>().ForEach(p =>
                    {
                        GetEdgeInformation(p, ref edgePtrs, ref edgeTypes);
                    });

                    //using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                    //{
                    //SelectionSet acSSet = SelectPolylines();

                    //if (acSSet == null)
                    //{
                    //    AC.Editor.PostCommandPrompt();
                    //    return;
                    //}

                    //var Polylines = from id in acSSet.GetObjectIds()
                    //                let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                    //                where acEnt is Polyline
                    //                let acPolyLine = acEnt as Polyline
                    //                select acPolyLine;

                    //if (Polylines.Any())
                    //{
                    //    Polylines.ToList().ForEach(p =>
                    //    {
                    //        GetEdgeInformation(p, ref edgePtrs, ref edgeTypes);
                    //    });
                    //}
                    //}
                    #endregion

                    var acPolylines = from a in edgePtrs.Cast<Curve2d>()
                                      orderby a.StartPoint.GetDistanceTo(a.EndPoint) descending
                                      select a;

                    var usedCurve = new List<Point2d>();

                    using (DocumentLock DL = AC.Doc.LockDocument())
                    {
                        acPolylines.Cast<Curve2d>().ToList().ForEach(c =>
                        {
                            var CenterP = point.GetCenterP(c.StartPoint, c.EndPoint);

                            var curves = from a in edgePtrs.Cast<Curve2d>().ToList()
                                         where a != c
                                         select a;

                            // c와 평행한 선을 받음
                            var MatchedCurves = from a in curves
                                                let d = Math.Round(a.GetDistanceTo(c))
                                                where CADUtil.GetVector(a).GetNormal().IsEqualTo(-CADUtil.GetVector(c).GetNormal())
                                                where d >= Min && d <= Max
                                                let cp1 = CADUtil.GetCenterPoint2d(c)
                                                let cp2 = CADUtil.GetCenterPoint2d(a)
                                                orderby cp1.GetDistanceTo(cp2) ascending
                                                select a;

                            if (MatchedCurves.Any())
                            {
                                //CAD.CreateLine(c.StartPoint, c.EndPoint);

                                bool B1 = true;

                                MatchedCurves.ToList().ForEach(c1 =>
                                {
                                    var cp1 = CADUtil.GetCenterPoint2d(c1);

                                    usedCurve.ForEach(cp2 =>
                                    {
                                        if (cp1.IsEqualTo(cp2))
                                        {
                                            B1 = false;
                                        }
                                    });
                                });

                                if (B1)
                                {
                                    CreateRectangle(c, MatchedCurves.ToList());

                                    usedCurve.Add(CADUtil.GetCenterPoint2d(c));
                                }
                            }
                        });
                    }
                }
                catch
                {
                    B = false;
                }
            } while (B);

            AC.Editor.WriteMessage("\n벽 입력완료 ");
            AC.Editor.PostCommandPrompt();
        }

        public void CreateBeamPolyLine()
        {
            bool B = true;

            while (B)
            {
                var Lines = select.Objects<Line>();

                if (Lines == null)
                {
                    B = false;
                    return;
                }

                var usedLines = new List<Point3d>();

                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    Lines.ForEach(acLine =>
                    {
                        try
                        {
                            var lines = from l in Lines.ToList()
                                        where acLine != l
                                        select l;

                            if (lines.Any())
                            {
                                #region 규칙 적용
                                var matchedLines = from a in lines
                                                   let A1 = Math.Abs(Math.Round(CADUtil.GetVector(acLine).Angle / Math.PI * 180)) % 180
                                                   let A2 = Math.Abs(Math.Round(CADUtil.GetVector(a).Angle / Math.PI * 180)) % 180
                                                   where Math.Abs(A1 - A2) < 5
                                                   //where CAD.GetVector(a).IsParallelTo(CAD.GetVector(acLine))
                                                   //|| CAD.GetVector(a).IsParallelTo(-CAD.GetVector(acLine))
                                                   let curve = acLine as Curve
                                                   let d = Math.Round(acLine.GetGeCurve().GetDistanceTo(CADUtil.GetCenterPoint3d(a)), 0)
                                                   where d >= Min && d <= Max
                                                   select a;

                                //MessageBox.Show(matchedLines.Count().ToString());
                                #endregion

                                #region 생성
                                if (matchedLines.Any())
                                {
                                    bool B1 = true;

                                    #region 중복 판별
                                    matchedLines.ToList().ForEach(l1 =>
                                    {
                                        var cp1 = CADUtil.GetCenterPoint3d(l1);

                                        usedLines.ForEach(cp2 =>
                                        {
                                            if (cp1.IsEqualTo(cp2))
                                            {
                                                B1 = false;
                                            }
                                        });
                                    });
                                    #endregion

                                    if (B1)
                                    {
                                        CreateRectangle(acLine, matchedLines.ToList());

                                        usedLines.Add(CADUtil.GetCenterPoint3d(acLine));
                                    }
                                }
                                #endregion
                            }
                        }
                        catch { }
                    });
                }
            }

            AC.Editor.WriteMessage("\n보 입력완료 ");
            AC.Editor.PostCommandPrompt();
        }

        public void CreateColumnPolyLine()
        {
            bool B = true;

            while (B)
            {
                var Polylines = select.Objects<Polyline>();

                if (Polylines == null)
                {
                    B = false;
                    return;
                }

                #region 생성
                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    Polylines.ForEach(acPolyline =>
                    {
                        var Points = new List<Point3d>();

                        for (int i = 0; i < acPolyline.NumberOfVertices; i++)
                        {
                            Points.Add(acPolyline.GetPoint3dAt(i));
                        }

                        Points.Add(acPolyline.GetPoint3dAt(0));

                        var acNewPolyLine = CADUtil.CreateRectangle(Points);
                    });
                }
                #endregion
            }

            AC.Editor.WriteMessage("\n기둥 입력완료 ");
            AC.Editor.PostCommandPrompt();
        }

        public void CreateKeyLine2()
        {
            var selectedPolyLines = new List<Polyline>();
            var acPolylines = new List<Polyline>();

            #region 폴리라인 선택
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    SelectionSet acSSet = SelectLines();

                    var Lines = from id in acSSet.GetObjectIds()
                                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                where acEnt is Polyline
                                let acLine = acEnt as Polyline
                                select acLine;

                    if (!Lines.Any())
                        return;

                    selectedPolyLines.AddRange(Lines.ToArray());
                }
                catch (System.Exception ex)
                {
                    AC.Editor.WriteMessage(Convert.ToString(ex));
                }
            }
            #endregion

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                #region 입력
                selectedPolyLines.ForEach(pl =>
                {
                    #region 폴리라인에서 커브 담기
                    var curves = new List<Curve3d>();

                    for (int i = 0; i < pl.NumberOfVertices; i++)
                    {
                        try
                        {
                            var l = pl.GetLineSegmentAt(i).Length;

                            if (l >= Min && l <= Max)
                            {
                                curves.Add(pl.GetLineSegmentAt(i));
                            }
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                    #endregion

                    if (curves.Count == 2)
                    {
                        var cv1 = curves[0];
                        var cv2 = curves[1];

                        var p1 = CADUtil.GetCenterPoint3d(cv1);
                        var p2 = CADUtil.GetCenterPoint3d(cv2);

                        var vec = CADUtil.GetVector(p1, p2);

                        var X = (vec * extend).X;
                        var Y = (vec * extend).Y;

                        var sp = CADUtil.MoveP(p1, -X, -Y, 0);
                        var ep = CADUtil.MoveP(p2, X, Y, 0);

                        var acPolyline = CADUtil.CreatePolyline(sp, ep);

                        acPolylines.Add(acPolyline);
                    }
                });
                #endregion

                #region 검토 & 연속된 선 연결
                // 제거할 선들
                var deletePolylines = new List<Polyline>();

                acPolylines.ForEach(pl1 =>
                {
                    var direction = pl1.GetLineSegmentAt(0).Direction;
                    direction = direction.X < 0 ? -direction : direction;
                    var Ang = Math.Round(Math.Atan2(direction.Y, direction.X) / Math.PI * 180) % 180;
                    var A = Math.Abs(Ang) == 90 || Ang == 0 ? 0 : Math.Tan(Ang / 180 * Math.PI);

                    //MessageBox.Show(Ang + "\n" + A.ToString());

                    // 같은 2차 그래프에 존재하는 선들
                    var parallelPolylines = from pl2 in acPolylines
                                            where !deletePolylines.Contains(pl2)
                                            let line1 = pl1.GetLineSegmentAt(0)
                                            let line2 = pl2.GetLineSegmentAt(0)
                                            where line1.IsParallelTo(line2, new Tolerance(1, 1))
                                            //let b1 = line1.StartPoint.Y - A * line1.StartPoint.X
                                            //let b2 = line2.StartPoint.Y - A * line2.StartPoint.X
                                            //let sp1 = line1.StartPoint
                                            //let sp2 = line2.StartPoint
                                            //where (Ang == 0 && IsAlmostEqual(sp1.Y, sp2.Y)) ||
                                            //      (Math.Abs(Ang) == 90 && IsAlmostEqual(sp1.X, sp2.X)) ||
                                            //      (Ang != 0 && Math.Abs(Ang) != 90 && IsAlmostEqual(line1,line2))
                                            where IsAlmostEqual(line1, line2)
                                            select pl2;

                    if (parallelPolylines.Any())
                    {
                        var Ps = CADUtil.getFurPoints(parallelPolylines.ToList());

                        var acPolyline = CADUtil.CreatePolyline(Ps[0], Ps[1]);

                        deletePolylines.AddRange(parallelPolylines);
                    }
                });

                #endregion

                #region 제거
                using (OpenCloseTransaction T = AC.DB.TransactionManager.StartOpenCloseTransaction())
                {
                    deletePolylines.ForEach(a =>
                    {
                        var entity = T.GetObject(a.Id, OpenMode.ForWrite) as Entity;

                        if (!entity.IsErased)
                        {
                            entity.Erase(true);
                        }
                    });

                    T.Commit();
                }
                #endregion
            }

            AC.Editor.WriteMessage("\n키라인 입력완료 ");
            AC.Editor.PostCommandPrompt();
        }

        public void CreateKeyLine()
        {
            bool B = true;

            while (B)
            {                
                //#region PRE
                //SelectionSet acSSet = SelectLines();

                //if (acSSet == null)
                //{
                //    B = false;
                //    return;
                //}
                //#endregion

                //#region 폴리라인 선택
                //var selectedPolyLines = new List<Polyline>();
                //var acPolylines = new List<Polyline>();

                //using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                //{
                //    var Lines = from id in acSSet.GetObjectIds()
                //                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                //                where acEnt is Polyline
                //                let acLine = acEnt as Polyline
                //                select acLine;

                //    if (!Lines.Any())
                //        return;

                //    selectedPolyLines.AddRange(Lines.ToArray());
                //}
                //#endregion

                var Polylines = select.Objects<Polyline>();
                var acPolylines = new List<Polyline>();

                #region 폴리라인 정리
                Polylines.ForEach(pl =>
                {
                    #region 폴리라인에서 커브 담기
                    var curves = new List<Curve3d>();

                    for (int i = 0; i < pl.NumberOfVertices; i++)
                    {
                        try
                        {
                            var l = pl.GetLineSegmentAt(i).Length;

                            if (l >= Min && l <= Max)
                            {
                                curves.Add(pl.GetLineSegmentAt(i));
                            }
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                    #endregion

                    #region 폴리라인 장축 중심을 읻는 선
                    if (curves.Count == 2)
                    {
                        var cv1 = curves[0];
                        var cv2 = curves[1];

                        var p1 = CADUtil.GetCenterPoint3d(cv1);
                        var p2 = CADUtil.GetCenterPoint3d(cv2);

                        var vec = CADUtil.GetVector(p1, p2);

                        var X = (vec * extend).X;
                        var Y = (vec * extend).Y;

                        var sp = CADUtil.MoveP(p1, -X, -Y, 0);
                        var ep = CADUtil.MoveP(p2, X, Y, 0);

                        var acPolyline = CADUtil.CreatePolyline(sp, ep);

                        acPolylines.Add(acPolyline);
                    }
                    #endregion
                });
                #endregion

                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    #region 검토 & 연속된 선 연결
                    // 제거할 선들
                    var deletePolylines = new List<Polyline>();

                    acPolylines.ForEach(pl1 =>
                    {
                        var contiPolylines = new List<Polyline>();

                        // 평행하고 같은 선상에 있는 선들
                        var paralPolylines = from pl2 in acPolylines
                                             where !deletePolylines.Contains(pl2)
                                             let line1 = pl1.GetLineSegmentAt(0)
                                             let line2 = pl2.GetLineSegmentAt(0)
                                             where line1.IsParallelTo(line2, new Tolerance(1, 1))
                                             where IsAlmostEqual(line1, line2)
                                             select pl2;

                        // 평행하고 같은 선상에 있는 선들 개수
                        int N = paralPolylines.Count();

                        var paralNearPolylines = from pl2 in paralPolylines
                                                 let line1 = pl1.GetLineSegmentAt(0)
                                                 let line2 = pl2.GetLineSegmentAt(0)
                                                 where CADUtil.getNearDistance(line1, line2) < distance
                                                 select pl2;

                        if (paralNearPolylines.Any())
                        {
                            contiPolylines.AddRange(paralNearPolylines);

                            for (int j = 0; j < N; j++)
                            {
                                contiPolylines.ToList().ForEach(a =>
                                {
                                    var srtPolylines = from pl2 in acPolylines
                                                       where !contiPolylines.Contains(pl2)
                                                       let line1 = a.GetLineSegmentAt(0)
                                                       let line2 = pl2.GetLineSegmentAt(0)
                                                       where line1.IsParallelTo(line2, new Tolerance(1, 1))
                                                       where IsAlmostEqual(line1, line2)
                                                       where CADUtil.getNearDistance(line1, line2) < 2 * extend + distance
                                                       select pl2;

                                    if (srtPolylines.Any())
                                    {
                                        contiPolylines.AddRange(srtPolylines);
                                    }
                                });
                            }
                        }

                        if (contiPolylines.Any())
                        {
                            var Ps = CADUtil.getFurPoints(contiPolylines.ToList());

                            var acPolyline = CADUtil.CreatePolyline(Ps[0], Ps[1]);

                            deletePolylines.AddRange(contiPolylines);
                        }
                    });

                    #endregion

                    #region 제거
                    using (OpenCloseTransaction T = AC.DB.TransactionManager.StartOpenCloseTransaction())
                    {
                        deletePolylines.ForEach(a =>
                        {
                            var entity = T.GetObject(a.Id, OpenMode.ForWrite) as Entity;

                            if (!entity.IsErased)
                            {
                                entity.Erase(true);
                            }
                        });

                        T.Commit();
                    }
                    #endregion
                }
            }

            AC.Editor.WriteMessage("\n키라인 입력완료 ");
            AC.Editor.PostCommandPrompt();
        }

        #endregion

        #region PRIVATE 메서드
        private SelectionSet SelectPolylines()
        {
            // create the typevalue (criteria what should be selected)
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

            PromptSelectionResult acPSR = AC.Editor.GetSelection(oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }

            AC.Doc.GetAcadDocument();

            return acPSR.Value;
        }

        private SelectionSet SelectLines()
        {
            // create the typevalue (criteria what should be selected)
            TypedValue[] tvs = new TypedValue[]
            {
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionResult acPSR = AC.Editor.GetSelection(oSf);

            //acDocEd.WriteMessage("선택");
            //acDocEd.UpdateScreen();

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }

            AC.Doc.GetAcadDocument();

            return acPSR.Value;
        }

        private void GetEdgeInformation(Polyline pline, ref Curve2dCollection plCurves, ref IntegerCollection edgeTypes)
        {
            int segCount = pline.NumberOfVertices;

            for (int cnt = 0; cnt < segCount; cnt++)
            {
                SegmentType type = pline.GetSegmentType(cnt);

                switch (type)
                {

                    case SegmentType.Arc:

                        CircularArc2d arc2d = pline.GetArcSegment2dAt(cnt);

                        plCurves.Add(arc2d);

                        edgeTypes.Add((int)Enum.Parse(typeof(HatchEdgeType),

                                HatchEdgeType.CircularArc.ToString()));

                        break;

                    case SegmentType.Line:

                        LineSegment2d line2d = pline.GetLineSegment2dAt(cnt);

                        plCurves.Add(line2d);

                        edgeTypes.Add((int)Enum.Parse(typeof(HatchEdgeType),

                                       HatchEdgeType.Line.ToString()));

                        break;

                    case SegmentType.Coincident:

                        break;

                    case SegmentType.Empty:

                        break;

                    case SegmentType.Point:

                        break;

                }

            }
        }

        private void CreateRectangle(Curve2d C1, List<Curve2d> Cs)
        {
            var C2 = Cs.First();

            var sp1 = C1.StartPoint;
            var ep1 = C1.EndPoint;
            var Ps1 = new List<Point2d> { sp1, ep1 };

            var sp2 = C2.StartPoint;
            var ep2 = C2.EndPoint;
            var Ps2 = new List<Point2d> { sp2, ep2 };

            var Ps = from p1 in Ps1
                     from p2 in Ps2
                     let d = p1.GetDistanceTo(p2)
                     orderby d descending
                     select new List<Point2d> { p1, p2 };

            if (Ps.Any())
            {
                var Points = Ps.First();

                var Vec1 = CADUtil.GetVector(C1);
                var Vec2 = CADUtil.GetVector(Points[0], Points[1]);

                if (Vec1.GetAngleTo(Vec2) > Math.PI / 2)
                {
                    Vec1 = -Vec1;
                }

                double Ang = Math.Abs(Vec1.GetAngleTo(Vec2));

                var P1 = CADUtil.ToPoint3D(Points[0]);
                var P2 = CADUtil.ToPoint3D(Points[1]);

                CADUtil.CreateRectangle(P1, P2, Vec1, Ang);
            }
        }

        private void CreateRectangle(Line L1, List<Line> Ls)
        {
            var C2 = Ls.First();

            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;
            var Ps1 = new List<Point3d> { sp1, ep1 };

            var sp2 = C2.StartPoint;
            var ep2 = C2.EndPoint;
            var Ps2 = new List<Point3d> { sp2, ep2 };

            var Ps = from p1 in Ps1
                     from p2 in Ps2
                     let d = p1.DistanceTo(p2)
                     orderby d descending
                     select new List<Point3d> { p1, p2 };

            if (Ps.Any())
            {
                var Points = Ps.First();

                var P1 = new Point3d(Points[0].X, Points[0].Y, 0);
                var P2 = new Point3d(Points[1].X, Points[1].Y, 0);

                var Vec1 = CADUtil.GetVector(L1);
                var Vec2 = CADUtil.GetVector(P1, P2);

                if (Vec1.GetAngleTo(Vec2) > Math.PI / 2)
                {
                    Vec1 = -Vec1;
                }

                double Ang = Math.Abs(Vec1.GetAngleTo(Vec2));

                var acPolyline = CADUtil.CreateRectangle(P1, P2, Vec1, Ang);

                //MessageBox.Show(P1.ToString() + "\n" + P2.ToString() + "\n" + Vec1.ToString() + "\n" + Ang.ToString());

                //MessageBox.Show(acPolyline.StartPoint.ToString() + "\n" + acPolyline.EndPoint.ToString());
            }
        }

        private bool IsAlmostEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < distance2;
        }
        private bool IsAlmostEqual(Line L1, Line L2)
        {
            var p1 = L1.StartPoint;
            var p2 = L1.EndPoint;
            var p3 = L2.StartPoint;

            double a = p1.Y - p2.Y;
            double b = p2.X - p1.X;
            double c = p1.X * p2.Y - p2.X * p1.Y;

            double d = Math.Abs(a * p3.X + b * p3.Y + c) / Math.Sqrt(a * a + b * b);

            return d < distance2 ? true : false;
        }
        private bool IsAlmostEqual(LineSegment3d L1, LineSegment3d L2)
        {
            var p1 = L1.StartPoint;
            var p2 = L1.EndPoint;
            var p3 = L2.StartPoint;

            double a = p1.Y - p2.Y;
            double b = p2.X - p1.X;
            double c = p1.X * p2.Y - p2.X * p1.Y;

            double d = Math.Abs(a * p3.X + b * p3.Y + c) / Math.Sqrt(a * a + b * b);

            return d < distance2 ? true : false;
        }
        #endregion

    }
}
