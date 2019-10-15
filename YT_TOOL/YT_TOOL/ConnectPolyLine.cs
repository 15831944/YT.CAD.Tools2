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

namespace YT_CAD_TOOL
{
    public class ConnectPolyLine
    {
        #region 속성
        #endregion

        #region 생성자
        public ConnectPolyLine()
        {
        }
        #endregion

        #region PUBLIC 메서드

        public void CreatePolyline()
        {
            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    try
                    {
                        SelectionSet ss = SelectLines();

                        AC.Editor.SetImpliedSelection(ss);

                        AC.Doc.SendStringToExecute("join ", true, false, false);

                        T.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        AC.Editor.WriteMessage(Convert.ToString(ex));
                    }
                }
            }

            AC.Editor.WriteMessage("\nPolyline 생성이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }


        public void SimplifyPolyline()
        {
            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    try
                    {
                        #region PRE
                        BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                        BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        SelectionSet acSSet = SelectLines();

                        var acPolylines = from objID in acSSet.GetObjectIds()
                                          let acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity
                                          where acEnt is Polyline
                                          select acEnt as Polyline;

                        if (!acPolylines.Any())
                            return;
                        #endregion

                        acPolylines.ToList().ForEach(acPL =>
                        {
                            #region 점, 문자
                            //for (int i = 0; i < acPL.NumberOfVertices; i++)
                            //{
                            //    using (DBPoint acPoint = new DBPoint(acPL.GetPoint3dAt(i)))
                            //    {
                            //        BTR.AppendEntity(acPoint);
                            //        T.AddNewlyCreatedDBObject(acPoint, true);
                            //    }
                            //    using (DBText acText = new DBText())
                            //    {
                            //        acText.TextString = i.ToString();
                            //        acText.Position = acPL.GetPoint3dAt(i);
                            //        acText.Height = 20;

                            //        BTR.AppendEntity(acText);
                            //        T.AddNewlyCreatedDBObject(acText, true);
                            //    }
                            //}
                            //acDb.Pdmode = 34;
                            //acDb.Pdsize = 150;
                            #endregion

                            #region 시작점이랑 끝점이 겹칠경우 끝점을 제거하고 Close해줌
                            if (acPL.GetPoint3dAt(0).DistanceTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)) < 0.1)
                            {
                                acPL.RemoveVertexAt(acPL.NumberOfVertices - 1);

                                acPL.Closed = true;
                            }
                            #endregion

                            #region #1 같은 지점에 중복되는 점이 있을 경우 점을 제거함
                            for (int i = 0; i < acPL.NumberOfVertices - 1; i++)
                            {
                                try
                                {
                                    #region Polyline이 열려있는 경우 마지막 선이 없다
                                    if (acPL.Closed == false && i == acPL.NumberOfVertices - 2)
                                    {
                                        i++;
                                        break;
                                    }
                                    #endregion

                                    var prePoint = acPL.GetPoint3dAt(i);
                                    var aftPoint = acPL.GetPoint3dAt(i + 1);

                                    #region 점 겹칠때
                                    if (IsSamePoint(prePoint, aftPoint))
                                    {
                                        acPL.RemoveVertexAt(i);

                                        i = i - 1;
                                        continue;
                                    }
                                    #endregion
                                }
                                catch { }
                            }
                            #endregion

                            #region #2 나란한 직선에 연속되는 선들을 합침
                            for (int i = 0; i < acPL.NumberOfVertices - 1; i++)
                            {
                                try
                                {
                                    #region Polyline이 열려있는 경우 마지막 선이 없다
                                    if (acPL.Closed == false && i == acPL.NumberOfVertices - 2)
                                    {
                                        i++;
                                        break;
                                    }
                                    #endregion

                                    var preLine = acPL.GetLineSegmentAt(i);
                                    var aftLine = acPL.GetLineSegmentAt(i + 1);

                                    #region [Case 1] i == 0 인경우
                                    if (i == 0 && acPL.Closed)
                                    {
                                        preLine = acPL.GetLineSegmentAt(acPL.NumberOfVertices - 1);
                                        aftLine = acPL.GetLineSegmentAt(0);

                                        if (IsParallel(preLine, aftLine) && IsIntersect(preLine, aftLine))
                                        {
                                            acPL.RemoveVertexAt(0);

                                            i = i - 1;

                                            continue;
                                        }
                                    }
                                    #endregion

                                    #region [Case 2] i != 0 인 경우

                                    if (IsParallel(preLine, aftLine) && IsIntersect(preLine, aftLine))
                                    {
                                        acPL.RemoveVertexAt(i + 1);

                                        i = i - 1;

                                        continue;
                                    }

                                    #endregion
                                }
                                catch { }
                            }
                            #endregion

                        });

                        T.Commit();
                    }
                    catch { }
                }
            }

            AC.Editor.WriteMessage("\nPolyline 단순화가 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }


        /// <summary>
        /// 폴리라인끼리 붙이기
        /// </summary>
        public void AttachPolyline()
        {
            bool B = true;

            while (B)
            {
                #region 폴리라인 선택
                var acPoly1 = SelectAPolyline("첫번째 폴리선 선택: "); if (acPoly1 == null) { B = false; continue; }
                var acPoly2 = SelectAPolyline("두번째 폴리선 선택: "); if (acPoly2 == null) { B = false; continue; }

                if (acPoly1.NumberOfVertices > 4 || acPoly2.NumberOfVertices > 4) continue;

                var acCenterLine1 = GetCenterLine(acPoly1);
                var acCenterLine2 = GetCenterLine(acPoly2);

                if (IsParallel(acCenterLine1, acCenterLine2)) continue;
                #endregion

                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                    {
                        var IP = GetIntersectPoint(acCenterLine1, acCenterLine2);   // 중심선 교차점

                        #region 연장되어야 하는 폴리라인 찾기
                        Polyline MovePoly = null;         // 연장되어야하는 폴리라인
                        Polyline BasePoly = null;     // 연장되는 선분이 닿아야하는 기준 폴리라인

                        LineSegment3d upLine = null;
                        LineSegment3d downLine = null;

                        if (IsPointOnLine(acCenterLine1, IP))
                        {
                            MovePoly = T.GetObject(acPoly2.Id, OpenMode.ForWrite) as Polyline;
                            BasePoly = acPoly1;

                            upLine = acCenterLine2;
                            downLine = acCenterLine2;
                        }
                        else if (IsPointOnLine(acCenterLine2, IP))
                        {
                            MovePoly = T.GetObject(acPoly1.Id, OpenMode.ForWrite) as Polyline;
                            BasePoly = acPoly2;

                            upLine = acCenterLine1;
                            downLine = acCenterLine1;
                        }
                        #endregion

                        #region 실행 - 양쪽다 이동해야하는 경우
                        if (MovePoly == null)
                        {
                            #region 폴리라인1 이동
                            acPoly1 = T.GetObject(acPoly1.Id, OpenMode.ForWrite) as Polyline;
                            var line1 = GetNearShortLine(acPoly1, IP);

                            for (int i = 0; i < acPoly1.NumberOfVertices; i++)
                            {
                                var L = acPoly1.GetLineSegmentAt(i);

                                if (IsSameLine(line1, L))
                                {
                                    var IC = new IntegerCollection() { i, i + 1 };
                                    var cp = CADUtil.GetCenterPoint3d(L);
                                    var v = CADUtil.GetVector3d(cp, IP);
                                    var d = cp.DistanceTo(IP);

                                    acPoly1.MoveStretchPointsAt(IC, v * d);
                                }
                            }
                            #endregion

                            #region 폴리라인2 이동
                            acPoly2 = T.GetObject(acPoly2.Id, OpenMode.ForWrite) as Polyline;
                            var line2 = GetNearShortLine(acPoly2, IP);

                            for (int i = 0; i < acPoly2.NumberOfVertices; i++)
                            {
                                var L = acPoly2.GetLineSegmentAt(i);

                                if (IsSameLine(line2, L))
                                {
                                    var IC = new IntegerCollection() { i, i + 1 };
                                    var cp = CADUtil.GetCenterPoint3d(L);
                                    var v = CADUtil.GetVector3d(cp, IP);
                                    var d = cp.DistanceTo(IP);

                                    acPoly2.MoveStretchPointsAt(IC, v * d);
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region 실행 - 한쪽만 이동하면 되는 경우
                        else
                        {
                            #region 움직여야하는 선분 & 고정되어야하는 선분 찾기
                            LineSegment3d moveLine = GetNearShortLine(MovePoly, IP);     // 교차점과 가장 가까운 선분 찾기
                            LineSegment3d baseLine = GetNearLongLine(BasePoly, IP);

                            var v = moveLine.Direction;
                            var d = moveLine.Length / 2;

                            upLine = CADUtil.MoveL(upLine, v * d);
                            downLine = CADUtil.MoveL(downLine, -v * d);
                            #endregion

                            if (baseLine != null || moveLine != null)
                            {
                                var ip1 = GetIntersectPoint(upLine, baseLine);
                                var ip2 = GetIntersectPoint(downLine, baseLine);
                                var d1 = moveLine.GetDistanceTo(ip1);
                                var d2 = moveLine.GetDistanceTo(ip2);

                                var distance = d1 > d2 ? d1 : d2;

                                var IC = new IntegerCollection();

                                for (int i = 0; i < MovePoly.NumberOfVertices; i++)
                                {
                                    var L = MovePoly.GetLineSegmentAt(i);

                                    if (IsSameLine(moveLine, L))
                                    {
                                        IC = new IntegerCollection() { i, i + 1 };

                                        break;
                                    }
                                }

                                if (IC.Count != 0)
                                {
                                    var cp = CADUtil.GetCenterPoint3d(moveLine);
                                    var vec = CADUtil.GetVector3d(cp, IP);

                                    if (cp.IsEqualTo(IP))
                                    {
                                        vec = CADUtil.GetVector3d(cp, GetCenterPoint(MovePoly));
                                    }

                                    MovePoly.MoveStretchPointsAt(IC, vec * distance);
                                }
                            }
                        }
                        #endregion

                        T.Commit();
                    }
                }
            }

            AC.Editor.WriteMessage("\nPolyline 연결이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }

        public void AttachPolylines()
        {
            #region 폴리라인 선택
            var selectedLines = SelectPolylines();
            #endregion

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                selectedLines.ForEach(acPoly1 =>
                {
                    selectedLines.Where(l => acPoly1 != l).ToList().ForEach(acPoly2 =>
                    {
                        var acCenterPoly1 = GetCenterLine(acPoly1);
                        var acCenterPoly2 = GetCenterLine(acPoly2);

                        if (IsParallel(acCenterPoly1, acCenterPoly2))
                            return;

                        using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                        {
                            var IP = GetIntersectPoint(acCenterPoly1, acCenterPoly2);

                            #region 연장되어야 하는 폴리라인 찾기
                            var acPoly = new Polyline();

                            if (IsPointOnLine(acCenterPoly1, IP))
                            {
                                acPoly = T.GetObject(acPoly2.Id, OpenMode.ForWrite) as Polyline;
                            }
                            else if (IsPointOnLine(acCenterPoly2, IP))
                            {
                                acPoly = T.GetObject(acPoly1.Id, OpenMode.ForWrite) as Polyline;
                            }

                            if (acPoly == new Polyline())
                                return;
                            #endregion

                            #region 교차점과 가장 가까운 선 찾기
                            var Lines = new List<LineSegment3d>();

                            for (int i = 0; i < acPoly.NumberOfVertices; i++)
                            {
                                Lines.Add(acPoly.GetLineSegmentAt(i));
                            }

                            var line = (from l in Lines
                                        let cp = CADUtil.GetCenterPoint3d(l)
                                        let d = cp.DistanceTo(IP)
                                        orderby d
                                        select l).First();
                            #endregion

                            #region 이동
                            for (int i = 0; i < acPoly.NumberOfVertices; i++)
                            {
                                var L = acPoly.GetLineSegmentAt(i);

                                if (IsSameLine(line, L))
                                {
                                    var IC = new IntegerCollection();
                                    IC.Add(i);
                                    IC.Add(i + 1);

                                    var cp = CADUtil.GetCenterPoint3d(L);
                                    var v = CADUtil.GetVector3d(cp, IP);
                                    var d = cp.DistanceTo(IP);

                                    acPoly.MoveStretchPointsAt(IC, v * d);
                                }
                            }
                            #endregion

                            T.Commit();
                        }
                    });
                });
            }

            AC.Editor.PostCommandPrompt();
        }

        public void AttachAtSelectedPolyline()
        {
            #region 폴리라인 선택
            var acPoly1 = SelectAPolyline("기준 폴리선 선택: "); if (acPoly1 == null) return;
            var Polys = SelectPolylines("연결할 폴리선 선택: ");

            if (acPoly1.NumberOfVertices < 4) return;
            #endregion

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    Polys.Where(a => a.NumberOfVertices > 3 && a != acPoly1).ToList().ForEach(acPoly2 =>
                    {
                        #region 기준선 | 움직이는선 | 중심선 | 교차점
                        acPoly2 = T.GetObject(acPoly2.Id, OpenMode.ForWrite) as Polyline;

                        var centerPoint1 = GetCenterPoint(acPoly1);
                        var centerPoint2 = GetCenterPoint(acPoly2);

                        var centerLine = new LineSegment3d(centerPoint1, centerPoint2);


                        var baseLine = GetIntersectLine(acPoly1, centerLine);
                        //var moveLine = GetIntersectLine(acPoly2, centerLine);     // 교차점과 가장 가까운 선분 찾기
                        var moveLine = GetNearShortLine(acPoly2, centerPoint1);

                        if (baseLine == null || moveLine == null) return;

                        centerLine = new LineSegment3d(centerPoint2, CADUtil.GetCenterPoint3d(moveLine));

                        var IP = GetIntersectPoint(centerLine, baseLine);
                        #endregion

                        #region 위 | 아래 선
                        var v = moveLine.Direction;
                        var d = moveLine.Length / 2;
                        LineSegment3d upLine = CADUtil.MoveL(centerLine, v * d);
                        LineSegment3d downLine = CADUtil.MoveL(centerLine, -v * d);
                        #endregion

                        #region 거리측정
                        var ip1 = GetIntersectPoint(upLine, baseLine);
                        var ip2 = GetIntersectPoint(downLine, baseLine);
                        var d1 = moveLine.GetDistanceTo(ip1);
                        var d2 = moveLine.GetDistanceTo(ip2);

                        var distance = d1 > d2 ? d1 : d2;
                        #endregion

                        #region 실행
                        for (int i = 0; i < acPoly2.NumberOfVertices; i++)
                        {
                            var L = acPoly2.GetLineSegmentAt(i);

                            if (IsSameLine(moveLine, L))
                            {
                                var IC = new IntegerCollection() { i, i + 1 };
                                var cp = CADUtil.GetCenterPoint3d(L);
                                var vec = CADUtil.GetVector3d(centerPoint2, cp);

                                acPoly2.MoveStretchPointsAt(IC, vec * distance);
                            }
                        }
                        #endregion
                    });

                    T.Commit();
                }
            }

            AC.Editor.WriteMessage("\nPolyline 연결이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }

        /// <summary>
        /// 선 긋고 선에 붙이기
        /// </summary>
        public void AttachAtLine()
        {
            bool B = true;

            while (B)
            {
                #region 점 | 폴리라인 선택
                var pickPoint1 = PickPoint("첫번째 점: "); if (pickPoint1 == new Point3d()) { B = false; continue; }
                var pickPoint2 = PickPoint("두번째 점: "); if (pickPoint2 == new Point3d()) { B = false; continue; }

                #region 가이드선 생성
                Line guideLine = null;

                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    guideLine = new Line(pickPoint1, pickPoint2);
                    guideLine.Color = ColorIndex.Green;

                    //LinetypeTable acLinTbl = T.GetObject(acDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                    //if (acLinTbl.Has("Center") == true)
                    //{
                    //    guideLine.LinetypeId = acLinTbl["Center"];
                    //}

                    BTR.AppendEntity(guideLine);
                    T.AddNewlyCreatedDBObject(guideLine, true);
                    T.Commit();
                }
                #endregion

                var acPoly = SelectAPolyline("기준 폴리선 선택: "); if (acPoly == null) { B = false; continue; }

                #region 가이드선 제거
                CADUtil.Erase(guideLine.Id);
                #endregion

                if (acPoly.NumberOfVertices < 4) continue;
                #endregion
                
                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                    {
                        #region 기준선 | 움직이는선 | 중심선 | 교차점
                        acPoly = T.GetObject(acPoly.Id, OpenMode.ForWrite) as Polyline;

                        var baseLine = new LineSegment3d(pickPoint1, pickPoint2);   // 기준 선분
                        var moveLine = GetNearShortLine(acPoly, baseLine);               // 움직여야하는 선분


                        if (baseLine == null || moveLine == null) continue;

                        var centerLine = GetCenterLine(acPoly);

                        if (IsParallel(centerLine, baseLine)) continue;       // 기준 선분과 장축이 수평인 경우

                        var IP = GetIntersectPoint(centerLine, baseLine);   // 기준 선분과 Polyline의 교차되는 점
                        #endregion

                        #region 위 | 아래 선
                        var v = moveLine.Direction;
                        var d = moveLine.Length / 2;
                        LineSegment3d upLine = CADUtil.MoveL(centerLine, v * d);
                        LineSegment3d downLine = CADUtil.MoveL(centerLine, -v * d);
                        #endregion

                        #region 거리 측정
                        var ip1 = GetIntersectPoint(upLine, baseLine);
                        var ip2 = GetIntersectPoint(downLine, baseLine);

                        var d1 = moveLine.GetDistanceTo(ip1);
                        var d2 = moveLine.GetDistanceTo(ip2);
                        var distance = d1 > d2 ? d1 : d2;   // 움직여야하는 거리
                        #endregion

                        #region 실행
                        int n = acPoly.NumberOfVertices;

                        for (int i = 0; i < n; i++)
                        {
                            var L = acPoly.GetLineSegmentAt(i);

                            if (IsSameLine(moveLine, L))
                            {
                                var IC = i == n - 1 ? new IntegerCollection() { i, 0 } : new IntegerCollection() { i, i + 1 };

                                var centerPoint = CADUtil.GetCenterPoint3d(moveLine);
                                var cp = CADUtil.GetCenterPoint3d(ip1, ip2);
                                var vec = CADUtil.GetVector3d(centerPoint, cp);

                                acPoly.MoveStretchPointsAt(IC, vec * distance);
                            }
                        }
                        #endregion

                        T.Commit();
                    }
                }
            }

            AC.Editor.WriteMessage("\nPolyline 연결이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }



        public void ReflectVector2()
        {
            var acLine1 = SelectALine("첫번째 선 선택: "); if (acLine1 == null) return;
            var acLine2 = SelectALine("두번째 선 선택: "); if (acLine2 == null) return;

            var sp1 = acLine1.StartPoint;
            var ep1 = acLine1.EndPoint;
            var sp2 = acLine2.StartPoint;
            var ep2 = acLine2.EndPoint;

            var L_segment1 = new LineSegment3d(sp1, ep1);
            var L_segment2 = new LineSegment3d(sp2, ep2);

            var d1 = L_segment2.GetDistanceTo(sp1);
            var d2 = L_segment2.GetDistanceTo(ep1);

            var V = d1 > d2 ? CADUtil.GetVector3d(sp1, ep1) : CADUtil.GetVector3d(ep1, sp1);
            var line1 = d1 > d2 ? new LineSegment3d(sp1, ep1) : new LineSegment3d(ep1, sp1);
            var line2 = new LineSegment3d(acLine2.StartPoint, acLine2.EndPoint);


            //var pp = CAD.GetIntersectedPoint(sp1, V);

            //CAD.CreateCircle(pp, 30);

        }

        public void ReflectVector()
        {
            bool B = true;

            while (B)
            {
                var PP = CADUtil.PickPoint();

                if (PP == new Point3d())
                {
                    B = false;
                }

                CADUtil.CreateCircle(PP, 50, ColorIndex.Yellow);    // 클릭점

                var acLines = GetNearLines(PP, 10000);  // 점 주변의 선들을 다 받아온다.

                var acPolylines = GetNearPolylines(PP, 10000); // 점 주변의 폴리라인들을 다 받아온다.

                var acEnts = new List<Entity>();
                acEnts.AddRange(acLines);
                acEnts.AddRange(acPolylines);

                double n = 10;

                for (int i = 0; i < n; i++)
                {
                    var degree = 360 / n * i;

                    var ang = (degree * Math.PI) / 180;

                    var vec = new Vector3d(Math.Cos(ang), Math.Sin(ang), 0);

                    //var d = vec.GetAngleTo(Vector3d.XAxis);

                    var IP = CADUtil.GetIntersectedPoint(acEnts, PP, vec);
                    if (IP != new Point3d()) CADUtil.CreateCircle(IP, 20);
                }
            }
        }




        public void SimplifyPolyline1()
        {
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    SelectionSet acSSet = SelectLines();

                    foreach (var objID in acSSet.GetObjectIds())
                    {
                        var acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity;

                        if (acEnt is Polyline)
                        {
                            Polyline acPL = acEnt as Polyline;

                            #region 점, 문자
                            //for (int i = 0; i < acPL.NumberOfVertices; i++)
                            //{
                            //    using (DBPoint acPoint = new DBPoint(acPL.GetPoint3dAt(i)))
                            //    {
                            //        BTR.AppendEntity(acPoint);
                            //        T.AddNewlyCreatedDBObject(acPoint, true);
                            //    }
                            //    using (DBText acText = new DBText())
                            //    {
                            //        acText.TextString = i.ToString();
                            //        acText.Position = acPL.GetPoint3dAt(i);
                            //        acText.Height = 300;

                            //        BTR.AppendEntity(acText);
                            //        T.AddNewlyCreatedDBObject(acText, true);
                            //    }
                            //}
                            //acDb.Pdmode = 34;
                            //acDb.Pdsize = 150;
                            #endregion

                            try
                            {
                                #region 시작점이랑 끝점이 겹칠경우 끝점을 제거하고 Close해줌
                                if (acPL.GetPoint3dAt(0).DistanceTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)) < 0.1)
                                {
                                    acPL.RemoveVertexAt(acPL.NumberOfVertices - 1);

                                    acPL.Closed = true;
                                }
                                #endregion

                                #region Vertieces에 점, 문자 입력
                                //for (int i = 0; i < acPL.NumberOfVertices; i++)
                                //{
                                //    using (DBPoint acPoint = new DBPoint(acPL.GetPoint3dAt(i)))
                                //    {
                                //        BTR.AppendEntity(acPoint);
                                //        T.AddNewlyCreatedDBObject(acPoint, true);
                                //    }
                                //    using (DBText acText = new DBText())
                                //    {
                                //        acText.TextString = i.ToString();
                                //        acText.Position = acPL.GetPoint3dAt(i);
                                //        acText.Height = 300;

                                //        BTR.AppendEntity(acText);
                                //        T.AddNewlyCreatedDBObject(acText, true);
                                //    }
                                //}
                                //acDb.Pdmode = 34;
                                //acDb.Pdsize = 150;
                                #endregion

                                #region 실행

                                for (int i = 0; i < acPL.NumberOfVertices - 1; i++)
                                {
                                    try
                                    {
                                        #region Polyline이 열려있는 경우 마지막 선이 없다
                                        if (acPL.Closed == false && i == acPL.NumberOfVertices - 2)
                                        {
                                            i++;
                                            break;
                                        }
                                        #endregion

                                        var preLine = acPL.GetLineSegmentAt(i);
                                        var aftLine = acPL.GetLineSegmentAt(i + 1);

                                        #region 벡터 문자 입력
                                        //using (DBText acText = new DBText())
                                        //{
                                        //    acText.TextString = i.ToString() + "백터";
                                        //    acText.Position = CAD.GetCenterPoint3d(preLine.StartPoint, preLine.EndPoint);
                                        //    acText.Height = 300;

                                        //    BTR.AppendEntity(acText);
                                        //    T.AddNewlyCreatedDBObject(acText, true);
                                        //}
                                        #endregion

                                        #region [Case 1] i == 0 인경우
                                        if (i == 0 && acPL.Closed)
                                        {
                                            #region 선이 평행
                                            preLine = acPL.GetLineSegmentAt(acPL.NumberOfVertices - 1);
                                            aftLine = acPL.GetLineSegmentAt(0);

                                            try
                                            {
                                                if (preLine.Direction.IsEqualTo(aftLine.Direction) && preLine.IntersectWith(aftLine).Any())
                                                {
                                                    acPL.RemoveVertexAt(0);
                                                }
                                            }
                                            catch { }
                                            #endregion

                                            #region 점 겹칠때
                                            if (acPL.GetPoint3dAt(0).IsEqualTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)))
                                            {
                                                acPL.RemoveVertexAt(0);
                                            }
                                            #endregion
                                        }

                                        preLine = acPL.GetLineSegmentAt(i);
                                        aftLine = acPL.GetLineSegmentAt(i + 1);
                                        #endregion

                                        #region [Case 2] i != 0 인 경우
                                        try
                                        {
                                            bool B = false;

                                            #region 선 평행
                                            if (preLine.Direction.IsEqualTo(aftLine.Direction) && preLine.IntersectWith(aftLine).Any())
                                            {
                                                acPL.RemoveVertexAt(i + 1);

                                                B = true;
                                            }
                                            #endregion

                                            #region 점 겹칠때
                                            if (acPL.GetPoint3dAt(i).IsEqualTo(acPL.GetPoint3dAt(i + 1)))
                                            {
                                                acPL.RemoveVertexAt(i);

                                                B = true;
                                            }
                                            #endregion

                                            if (B) i = -1;
                                        }
                                        catch { }
                                        #endregion

                                    }
                                    catch { }
                                }

                                #endregion
                            }
                            catch { }

                            #region 시작점과 끝점이 같으면 닫기
                            if (acPL.GetPoint3dAt(0).IsEqualTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)))
                            {
                                acPL.Closed = true;
                            }
                            #endregion
                        }
                    }

                    T.Commit();
                }
                catch (System.Exception ex)
                {
                    //acDocEd.WriteMessage(Convert.ToString(ex));
                }
            }

            AC.Editor.PostCommandPrompt();
        }

        public void SimplifyPolyline2()
        {
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    SelectionSet acSSet = SelectLines();

                    var acPolylines = from a in acSSet.GetObjectIds()
                                      let b = T.GetObject(a, OpenMode.ForWrite)
                                      where b is Polyline
                                      select b as Polyline;

                    if (!acPolylines.Any())
                        return;

                    var points = new List<Point3d>();

                    acPolylines.ToList().ForEach(a =>
                    {
                        int n = a.NumberOfVertices;

                        for (int i = 0; i < n; i++)
                        {
                            var p = a.GetPoint3dAt(i);

                            points.Add(p);
                        }
                    });

                    foreach (var objID in acSSet.GetObjectIds())
                    {
                        var acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity;

                        if (acEnt is Polyline)
                        {
                            Polyline acPL = acEnt as Polyline;

                            #region 점, 문자
                            //for (int i = 0; i < acPL.NumberOfVertices; i++)
                            //{
                            //    using (DBPoint acPoint = new DBPoint(acPL.GetPoint3dAt(i)))
                            //    {
                            //        BTR.AppendEntity(acPoint);
                            //        T.AddNewlyCreatedDBObject(acPoint, true);
                            //    }
                            //    using (DBText acText = new DBText())
                            //    {
                            //        acText.TextString = i.ToString();
                            //        acText.Position = acPL.GetPoint3dAt(i);
                            //        acText.Height = 300;

                            //        BTR.AppendEntity(acText);
                            //        T.AddNewlyCreatedDBObject(acText, true);
                            //    }
                            //}
                            //acDb.Pdmode = 34;
                            //acDb.Pdsize = 150;
                            #endregion

                            try
                            {
                                #region 시작점이랑 끝점이 겹칠경우 끝점을 제거하고 Close해줌
                                if (acPL.GetPoint3dAt(0).DistanceTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)) < 0.1)
                                {
                                    acPL.RemoveVertexAt(acPL.NumberOfVertices - 1);

                                    acPL.Closed = true;
                                }
                                #endregion

                                #region Vertieces에 점, 문자 입력
                                //for (int i = 0; i < acPL.NumberOfVertices; i++)
                                //{
                                //    using (DBPoint acPoint = new DBPoint(acPL.GetPoint3dAt(i)))
                                //    {
                                //        BTR.AppendEntity(acPoint);
                                //        T.AddNewlyCreatedDBObject(acPoint, true);
                                //    }
                                //    using (DBText acText = new DBText())
                                //    {
                                //        acText.TextString = i.ToString();
                                //        acText.Position = acPL.GetPoint3dAt(i);
                                //        acText.Height = 300;

                                //        BTR.AppendEntity(acText);
                                //        T.AddNewlyCreatedDBObject(acText, true);
                                //    }
                                //}
                                //acDb.Pdmode = 34;
                                //acDb.Pdsize = 150;
                                #endregion

                                #region 실행

                                for (int i = 0; i < acPL.NumberOfVertices - 1; i++)
                                {
                                    try
                                    {
                                        #region Polyline이 열려있는 경우 마지막 선이 없다
                                        if (acPL.Closed == false && i == acPL.NumberOfVertices - 2)
                                        {
                                            i++;
                                            break;
                                        }
                                        #endregion

                                        var preLine = acPL.GetLineSegmentAt(i);
                                        var aftLine = acPL.GetLineSegmentAt(i + 1);

                                        #region 벡터 문자 입력
                                        //using (DBText acText = new DBText())
                                        //{
                                        //    acText.TextString = i.ToString() + "백터";
                                        //    acText.Position = CAD.GetCenterPoint3d(preLine.StartPoint, preLine.EndPoint);
                                        //    acText.Height = 300;

                                        //    BTR.AppendEntity(acText);
                                        //    T.AddNewlyCreatedDBObject(acText, true);
                                        //}
                                        #endregion

                                        #region [Case 1] i == 0 인경우
                                        if (i == 0 && acPL.Closed)
                                        {
                                            #region 선이 평행
                                            preLine = acPL.GetLineSegmentAt(acPL.NumberOfVertices - 1);
                                            aftLine = acPL.GetLineSegmentAt(0);

                                            try
                                            {
                                                if (preLine.Direction.IsEqualTo(aftLine.Direction) && preLine.IntersectWith(aftLine).Any())
                                                {
                                                    acPL.RemoveVertexAt(0);
                                                }
                                            }
                                            catch
                                            {
                                            }
                                            #endregion

                                            #region 점 겹칠때
                                            if (acPL.GetPoint3dAt(0).IsEqualTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)))
                                            {
                                                acPL.RemoveVertexAt(0);
                                            }
                                            #endregion
                                        }

                                        preLine = acPL.GetLineSegmentAt(i);
                                        aftLine = acPL.GetLineSegmentAt(i + 1);
                                        #endregion

                                        #region [Case 2] i != 0 인 경우
                                        try
                                        {
                                            bool B = false;

                                            #region 선 평행
                                            if (preLine.Direction.IsEqualTo(aftLine.Direction) && preLine.IntersectWith(aftLine).Any())
                                            {
                                                acPL.RemoveVertexAt(i + 1);

                                                B = true;
                                            }
                                            #endregion

                                            #region 점 겹칠때
                                            if (acPL.GetPoint3dAt(i).IsEqualTo(acPL.GetPoint3dAt(i + 1)))
                                            {
                                                acPL.RemoveVertexAt(i);

                                                B = true;
                                            }
                                            #endregion

                                            if (B) i = -1;
                                        }
                                        catch
                                        {
                                        }
                                        #endregion

                                    }
                                    catch
                                    {
                                    }
                                }

                                #endregion
                            }
                            catch (System.Exception ex)
                            {
                                //MessageBox.Show(ex.ToString());
                            }

                            #region 시작점과 끝점이 같으면 닫기
                            if (acPL.GetPoint3dAt(0).IsEqualTo(acPL.GetPoint3dAt(acPL.NumberOfVertices - 1)))
                            {
                                acPL.Closed = true;
                            }
                            #endregion
                        }
                    }

                    T.Commit();
                }
                catch (System.Exception ex)
                {
                    //acDocEd.WriteMessage(Convert.ToString(ex));
                }
            }

            AC.Editor.PostCommandPrompt();
        }


        #endregion

        #region PRIVATE 메서드
        // 선 선택
        private Line SelectALine(string text)
        {
            Line Return;

            // create the typevalue (criteria what should be selected)
            TypedValue[] tvs = new TypedValue[]
            {
                        //new TypedValue(Convert.ToInt32(DxfCode.Operator), "<and"),
                        //new TypedValue(Convert.ToInt32(DxfCode.LayerName), "Test unlocked"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
                //new TypedValue(Convert.ToInt32(DxfCode.Operator), "and>")
            };

            SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = text;
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\nError in getting selections");

                return null;
            }

            #region 폴리라인 추출
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acLines = from objID in acPSR.Value.GetObjectIds()
                              let acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity
                              where acEnt is Line
                              select acEnt as Line;

                if (acLines.Any())
                {
                    Return = acLines.First();
                }
                else
                {
                    return null;
                }

                T.Commit();
            }
            #endregion

            return Return;
        }

        private SelectionSet SelectLines()
        {
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            // create the typevalue (criteria what should be selected)
            TypedValue[] tvs = new TypedValue[]
            {
                        //new TypedValue(Convert.ToInt32(DxfCode.Operator), "<and"),
                        //new TypedValue(Convert.ToInt32(DxfCode.LayerName), "Test unlocked"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
                //new TypedValue(Convert.ToInt32(DxfCode.Operator), "and>")
            };

            SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionResult acPSR = acDocEd.GetSelection(oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                acDocEd.WriteMessage("\nError in getting selections");
                return acPSR.Value;
            }


            return acPSR.Value;
        }

        private List<Polyline> SelectPolylines(string text = "폴리선 선택: ")
        {
            var Return = new List<Polyline>();

            #region 필터
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
            #endregion

            #region 선택
            PromptSelectionResult acPSR = AC.Editor.GetSelection(oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }
            #endregion

            #region 폴리라인 추출
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acPolylines = from objID in acPSR.Value.GetObjectIds()
                                  let acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity
                                  where acEnt is Polyline
                                  select acEnt as Polyline;

                if (acPolylines.Any())
                {
                    Return.AddRange(acPolylines);
                }
                else
                {
                    return null;
                }

                T.Commit();
            }
            #endregion

            return Return;
        }

        private Polyline SelectAPolyline(string text = "폴리선 선택: ")
        {
            var Return = new Polyline();

            #region 필터
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
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = text;
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }
            #endregion

            #region 폴리라인 추출
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acPolylines = from objID in acPSR.Value.GetObjectIds()
                                  let acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity
                                  where acEnt is Polyline
                                  select acEnt as Polyline;

                if (acPolylines.Any())
                {
                    Return = acPolylines.First();
                }
                else
                {
                    return null;
                }

                T.Commit();
            }
            #endregion

            return Return;
        }

        private Point3d PickPoint(string text = "점 임력: ")
        {
            PromptPointResult PPR = AC.Editor.GetPoint(text);

            return (PPR.Status == PromptStatus.OK) ? PPR.Value : new Point3d();
        }



        private LineSegment3d GetCenterLine(Polyline acPoly)
        {
            var Return = new LineSegment3d();

            #region 폴리라인에서 커브 담기
            var curves = new List<Curve3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                var l = acPoly.GetLineSegmentAt(i).Length;

                curves.Add(acPoly.GetLineSegmentAt(i));
            }
            #endregion

            #region 폴리라인 장축 중심을 읻는 선
            if (curves.Count >= 2)
            {
                var cv1 = curves[0];
                var cv2 = curves[2];

                var cv3 = curves[1];
                var cv4 = curves[3];

                var sp1 = CADUtil.GetCenterPoint3d(cv1);
                var ep1 = CADUtil.GetCenterPoint3d(cv2);

                var sp2 = CADUtil.GetCenterPoint3d(cv3);
                var ep2 = CADUtil.GetCenterPoint3d(cv4);

                var L1 = new LineSegment3d(sp1, ep1);
                var L2 = new LineSegment3d(sp2, ep2);

                Return = L1.Length > L2.Length ? L1 : L2;
            }
            #endregion

            return Return;
        }

        private Polyline GetCenterPolyLine(Polyline acPoly)
        {
            var Return = new Polyline();

            #region 폴리라인에서 커브 담기
            var curves = new List<Curve3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                var l = acPoly.GetLineSegmentAt(i).Length;

                curves.Add(acPoly.GetLineSegmentAt(i));
            }
            #endregion

            #region 폴리라인 장축 중심을 읻는 선
            if (curves.Count >= 2)
            {
                var cv1 = curves[0];
                var cv2 = curves[1];

                var sp = CADUtil.GetCenterPoint3d(cv1);
                var ep = CADUtil.GetCenterPoint3d(cv2);

                var acPolyline = CADUtil.CreatePolyline(sp, ep);

                Return = acPolyline;
            }
            #endregion

            return Return;
        }

        private LineSegment3d GetIntersectLine(Polyline acPoly, LineSegment3d L)
        {
            LineSegment3d Return = null;

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                var l = acPoly.GetLineSegmentAt(i);

                if (l.IntersectWith(L) != null)
                {
                    Return = l;       // 기준이 되는 선분
                }
            };

            return Return;
        }

        private Point3d GetIntersectPoint(LineSegment3d L1, LineSegment3d L2)
        {
            var Return = new Point3d();

            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;

            if (!IsParallel(L1, L2))
            {
                var x1 = sp1.X;
                var y1 = sp1.Y;
                var x2 = ep1.X;
                var y2 = ep1.Y;

                var x3 = sp2.X;
                var y3 = sp2.Y;
                var x4 = ep2.X;
                var y4 = ep2.Y;

                var X = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
                        ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                var Y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
                        ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                var Z = sp1.Z;

                Return = new Point3d(X, Y, Z);
            }

            return Return;
        }

        private bool IsSameLine(LineSegment3d L1, LineSegment3d L2, double d = 1)
        {
            var cp = CADUtil.GetCenterPoint3d(L1);

            var sp = L2.StartPoint;
            var ep = L2.EndPoint;

            return cp.DistanceTo(sp) + cp.DistanceTo(ep) - L2.Length < d ? true : false;
        }


        private LineSegment3d GetNearShortLine(Polyline acPoly, Point3d P)
        {
            var Lines = new List<LineSegment3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                Lines.Add(acPoly.GetLineSegmentAt(i));
            }

            #region 짧은 축만 담음
            var temp = (from a in Lines
                        orderby a.Length ascending
                        select a).ToList();

            var Ls = new List<LineSegment3d>();
            if (Lines.Count > 2)
            {
                Ls.Add(temp[0]);
                Ls.Add(temp[1]);
            }
            #endregion

            return (from l in Ls
                    orderby l.GetDistanceTo(P) ascending
                    select l).First();
        }
        private LineSegment3d GetNearLongLine(Polyline acPoly, Point3d P)
        {
            var Lines = new List<LineSegment3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                Lines.Add(acPoly.GetLineSegmentAt(i));
            }

            #region 짧은 축만 담음
            var temp = (from a in Lines
                        orderby a.Length descending
                        select a).ToList();

            var Ls = new List<LineSegment3d>();
            if (Lines.Count > 2)
            {
                Ls.Add(temp[0]);
                Ls.Add(temp[1]);
            }
            #endregion

            return (from l in Ls
                    orderby l.GetDistanceTo(P) ascending
                    select l).First();
        }

        private LineSegment3d GetNearShortLine(Polyline acPoly, LineSegment3d L)
        {
            var Lines = new List<LineSegment3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                Lines.Add(acPoly.GetLineSegmentAt(i));
            }

            #region 짧은 축만 담음
            var temp = (from a in Lines
                        orderby a.Length ascending
                        select a).ToList();

            var Ls = new List<LineSegment3d>();
            if (Lines.Count > 2)
            {
                Ls.Add(temp[0]);
                Ls.Add(temp[1]);
            }
            #endregion

            return (from l in Ls
                    let cp = CADUtil.GetCenterPoint3d(l)
                    let d = cp.DistanceTo(CADUtil.GetCenterPoint3d(L))
                    orderby d
                    select l).First();
        }
        private LineSegment3d GetNearLongLine(Polyline acPoly, LineSegment3d L)
        {
            var Lines = new List<LineSegment3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                Lines.Add(acPoly.GetLineSegmentAt(i));
            }

            #region 짧은 축만 담음
            var temp = (from a in Lines
                        orderby a.Length descending
                        select a).ToList();

            var Ls = new List<LineSegment3d>();
            if (Lines.Count > 2)
            {
                Ls.Add(temp[0]);
                Ls.Add(temp[1]);
            }
            #endregion

            return (from l in Ls
                    let cp = CADUtil.GetCenterPoint3d(l)
                    let d = cp.DistanceTo(CADUtil.GetCenterPoint3d(L))
                    orderby d
                    select l).First();
        }

        private bool IsPointOnLine(LineSegment3d L, Point3d P)
        {
            return Math.Round(L.GetDistanceTo(P)) == 0 ? true : false;
        }

        private Point3d GetCenterPoint(Polyline acPoly)
        {
            double x = 0;
            double y = 0;

            int n = acPoly.NumberOfVertices;

            for (int i = 0; i < n; i++)
            {
                var p = acPoly.GetPoint3dAt(i);

                x += p.X;
                y += p.Y;
            }


            return new Point3d(x / n, y / n, 0);
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


        private bool IsSamePoint(Point3d P1, Point3d P2)
        {
            return P1.IsEqualTo(P2, new Tolerance(0.1, 0.1));
        }

        //private bool IsParallel(LineSegment3d L1, LineSegment3d L2)
        //{
        //    return L1.Direction.IsParallelTo(L2.Direction, new Tolerance(0.1, 0.1));
        //}
        private bool IsParallel(LineSegment3d L1, LineSegment3d L2)
        {
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;

            var x1 = sp1.X;
            var y1 = sp1.Y;
            var x2 = ep1.X;
            var y2 = ep1.Y;

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;

            var x3 = sp2.X;
            var y3 = sp2.Y;
            var x4 = ep2.X;
            var y4 = ep2.Y;
            var A = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            return Math.Round((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)) == 0 ? true : false;
        }

        private bool IsIntersect(LineSegment3d L1, LineSegment3d L2)
        {
            return L1.IntersectWith(L2).Any();
        }

        private List<Point3d> GetIntersectedPoints(LineSegment3d L1, LineSegment3d L2)
        {
            return L1.IntersectWith(L2).ToList();
        }


        private List<Line> GetNearLines(Point3d CP, double d = 10000)
        {
            var Return = new List<Line>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acEnts = new List<Entity>();

                var acLines = from id in BTR.Cast<ObjectId>().ToList()
                              let line = T.GetObject(id, OpenMode.ForRead) as Line
                              where line != null
                              where line.GetGeCurve().GetDistanceTo(CP) < d
                              select line;

                if (acLines.Any()) Return.AddRange(acLines);
            }

            return Return;
        }
        private List<Polyline> GetNearPolylines(Point3d CP, double d = 10000)
        {
            var Return = new List<Polyline>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acEnts = new List<Entity>();

                var acPolylines = from id in BTR.Cast<ObjectId>().ToList()
                                  let line = T.GetObject(id, OpenMode.ForRead) as Polyline
                                  where line != null
                                  where line.GetGeCurve().GetDistanceTo(CP) < d
                                  select line;

                if (acPolylines.Any()) Return.AddRange(acPolylines);
            }

            return Return;
        }

        #endregion
    }
}
