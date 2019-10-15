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

#region PROJECT
#endregion

namespace YT_CAD_TOOL
{
    public class CreateBoundary
    {
        #region 속성

        #endregion

        #region 생성자
        public CreateBoundary()
        {
        }
        #endregion

        #region PUBLIC 메서드
        public List<Polyline> Create_Boundaries()
        {
            var Return = new List<Polyline>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                #region 선택
                var acSSet = Utils.Select.MultiObjs();

                var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));
                #endregion

                #region 폴리라인 연장시켜 하나의 덩어리로 만들기
                var acLines = new List<LineSegment3d>();    // 선택된 Polyline의 모든 선분을 담음(최대, 최소 좌표를 얻기위해)
                var acTempLines = new List<Line>();         // 작업 완료 후 지울 보조선

                acEnts.Where(x => x is Polyline).ToList().ForEach(acEnt =>
                {
                    var acEntsCopy = acEnts.Where(x => x != acEnt);
                    var acPoly = acEnt as Polyline;

                    for (int i = 0; i < acPoly.NumberOfVertices; i++)
                    {
                        try
                        {
                            var acLine = acPoly.GetLineSegmentAt(i);

                            if (!acLines.Contains(acLine)) acLines.Add(acLine);

                            var Di = acLine.Direction;

                            var sp = acLine.StartPoint;
                            var ep = acLine.EndPoint;
                            var moved_sp = Utils.PointUtil.Move(acLine.StartPoint, -Di);
                            var moved_ep = Utils.PointUtil.Move(acLine.StartPoint, Di);

                            var LS1 = new LineSegment3d(sp, moved_sp);      // Line에서 연장된 선분
                            var LS2 = new LineSegment3d(ep, moved_ep);      // Line에서 연장된 선분

                            var IP1 = CADUtil.IntersectedLine(acEntsCopy.ToList(), LS1, sp, 500);
                            var IP2 = CADUtil.IntersectedLine(acEntsCopy.ToList(), LS2, ep, 500);

                            double d = 0;

                            if (IP1 != new Point3d())
                            {
                                var SP = Utils.PointUtil.Move(sp, Di * d);
                                var EP = Utils.PointUtil.Move(IP1, -Di * d);
                                var L1 = CADUtil.CreateLine(SP, EP, ColorIndex.DarkGray);
                                acTempLines.Add(L1);
                            }
                            if (IP2 != new Point3d())
                            {
                                var SP = Utils.PointUtil.Move(ep, -Di * d);
                                var EP = Utils.PointUtil.Move(IP2, Di * d);
                                var L2 = CADUtil.CreateLine(SP, EP, ColorIndex.DarkGray);
                                acTempLines.Add(L2);
                            }
                        }
                        catch { }
                    }
                });
                #endregion

                #region 폴리라인을 감싸는 Box객체를 생성
                var P_X = (from a in acLines
                           let p = a.StartPoint
                           orderby p.X
                           select p.X);

                var P_Y = (from a in acLines
                           let p = a.StartPoint
                           orderby p.Y
                           select p.Y);

                var Min_P = new Point3d(P_X.First() - 1000, P_Y.First() - 1000, 0);
                var Max_P = new Point3d(P_X.Last() + 1000, P_Y.Last() + 1000, 0);

                var Box = CADUtil.CreateRectangle(Min_P, Max_P);
                #endregion

                #region 밀폐된 공간에서 Boundary 추적
                var P1 = Utils.PointUtil.Move(Min_P, 500, 500);

                var objColl = AC.Editor.TraceBoundary(P1, true);

                var acObjs = from a in objColl.Cast<Entity>().ToList() select a;
                #endregion

                #region Boundary 생성
                if (acObjs.Any())
                {
                    using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                    {
                        var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                        var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        acObjs.ToList().ForEach(a =>
                        {
                            var acPoly = a as Polyline;

                            if (Math.Abs(acPoly.Area - Box.Area) > 1)
                            {
                                BTR.AppendEntity(acPoly);
                                T.AddNewlyCreatedDBObject(acPoly, true);

                                Return.Add(acPoly);
                            }
                        });

                        T.Commit();
                    }
                }
                #endregion

                #region 제거
                CADUtil.Erase(Box.Id);

                acTempLines.ForEach(a => CADUtil.Erase(a.Id));
                #endregion
            }

            return Return;
        }

        public List<Polyline> Create_Boundaries_Util(double Dis = 100)
        {
            var Return = new List<Polyline>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                #region 선택
                var acSSet = Utils.Select.MultiObjs();

                var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));
                #endregion

                #region 폴리라인 연장시켜 폴리라인 생성
                acEnts.Where(x => x is Polyline).ToList().ForEach(acEnt =>
                {
                    var acPoly = acEnt as Polyline;
                    var acTempLines = new List<Line>();         // 작업 완료 후 지울 보조선

                    #region 폴리라인 선분 연장선 생성
                    for (int i = 0; i < acPoly.NumberOfVertices; i++)
                    {
                        try
                        {
                            var acLine = acPoly.GetLineSegmentAt(i);
                            var sp = acLine.StartPoint;
                            var ep = acLine.EndPoint;
                            var Direction = acLine.Direction;

                            var SP = sp - Direction * Dis;
                            var EP = ep + Direction * Dis;

                            var L = CADUtil.CreateLine(SP, EP);

                            acTempLines.Add(L);
                        }
                        catch { }
                    }
                    #endregion

                    #region 선택된 폴리라인의 중앙점
                    var CP = Utils.PointUtil.GetCenterP(acPoly);
                    #endregion

                    #region 밀폐된 공간에서 Boundary 생성
                    // 선택된 객체 말고 모두 숨긴뒤에 실행해야 오류가 줄어듬


                    var objColl = AC.Editor.TraceBoundary(CP, true);

                    var acObjs = from a in objColl.Cast<Entity>().ToList() select a;

                    if (acObjs.Any())
                    {
                        using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                        {
                            var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                            var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                            acObjs.ToList().ForEach(a =>
                            {
                                var Poly = a as Polyline;

                                BTR.AppendEntity(Poly);
                                T.AddNewlyCreatedDBObject(Poly, true);

                                Return.Add(Poly);
                            });

                            T.Commit();
                        }
                    }
                    #endregion

                    #region 연장선 제거
                    acTempLines.ForEach(a => CADUtil.Erase(a.Id));
                    #endregion
                });
                #endregion
            }

            return Return;
        }

        #endregion

        #region PRIVATE 메서드

        #endregion
    }
}
