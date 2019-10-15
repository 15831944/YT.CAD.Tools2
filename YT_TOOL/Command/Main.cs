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
using Aspose.CAD;
#endregion

//[assembly: ExtensionApplication(null)]
//[assembly: CommandClass(typeof(YT_CAD_TOOL.YT_Main))]
[assembly: CommandClass(typeof(YT_CAD_TOOL.Command_Palette))]
[assembly: CommandClass(typeof(YT_CAD_TOOL.YT_Main))]

namespace YT_CAD_TOOL
{
    public class YT_Main
    {
        [CommandMethod("YTTOOL")]
        public void YT_CAD_TOOL()
        {
            AC.Initialize_YT();

            U001 UI = new U001();

            UI.Show();
        }
        //------------------------------------------------------------------------------

        [CommandMethod("YTUTIL")]
        public void U002()
        {
            AC.Initialize_YT();

            U002 UI = new U002();

            UI.Show();
        }
        //------------------------------------------------------------------------------

        [CommandMethod("YTLAYER")]
        public void U003()
        {
            AC.Initialize_YT();

            U003 UI = new U003();

            UI.Show();
        }
        //------------------------------------------------------------------------------

        [CommandMethod("HDCTEST")]
        public void U004()
        {
            AC.Initialize_YT();

            U004 UI = new U004();

            UI.Show();
        }
        //------------------------------------------------------------------------------
    }

    public class HDC_Main
    {
        /// <summary>
        /// 실 정보 입력하기
        /// </summary>
        [CommandMethod("IR")]
        public void InsertRoom()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            DB.Initialize();

            new U_Room().Show();
        }


        /// <summary>
        /// 도면상의 실 이름을 DB에 받아옴
        /// </summary>
        [CommandMethod("RR")]
        public void ReadRoom()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();

            var A = Utils.Get.GetEntity<DBText>(Utils.Select.MultiObjs("실 이름을 선택해주세요."));
            if (A.Count == 0) return;

            var B = from a in A
                    let b = a.TextString
                    orderby b
                    select b;

            if (!B.Any()) return;

            DB.ROOM.Clear();

            int i = 1;

            DB.ROOMs.ForEach(r =>
            {
                var C = from b in B
                        where b.Contains(r)
                        select b;

                if (!C.Any()) return;

                C.ToList().ForEach(c =>
                {
                    DB.ROOM.Add(new Room(i++, c, 0, 0, 0, false));
                });
            });
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 벽체 분할 바운더리 생성
        /// </summary>
        [CommandMethod("CB")]
        public void Create_Boundaries()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();

            var Boundaries = new List<Polyline>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                #region 선택
                var acSSet = Utils.Select.MultiObjs();

                if (acSSet == null) return;

                var acPolylines = Utils.Get.GetEntity<Polyline>(acSSet)
                                           .Where(a => DB.LAYER.Contains(a.Layer)).ToList();
                #endregion

                #region 설비실 해치생성
                //string layerName = "00_덕트";

                //var acPolys = Util.Get.GetEntity<Polyline>(acSSet).Where(a => a.Layer == layerName);

                //acPolys.ToList().ForEach(acPoly =>
                //{
                //    CAD.CreateHatch(acPoly.ObjectId, "ANSI31", layerName, 500);
                //});
                #endregion

                #region ZOOM IN
                var P_X = (from a in acPolylines
                           let p = a.Bounds.Value.MinPoint
                           orderby p.X
                           select p.X);

                var P_Y = (from a in acPolylines
                           let p = a.Bounds.Value.MaxPoint
                           orderby p.Y
                           select p.Y);

                var Min_P = new Point3d(P_X.First() - 10, P_Y.First() - 10, 0);
                var Max_P = new Point3d(P_X.Last() + 10, P_Y.Last() + 10, 0);

                //CAD.ZoomExtents(Min_P, Max_P, 0.8);
                #endregion

                #region 레이어 숨기기
                Utils.Layer.ISO(DB.LAYER, true);
                #endregion

                #region 폴리라인 연장시켜 벽 공간 나누기
                double Dist = 700;      // 점과 점 사이의 간격(최대 벽체 두께)

                var separate_lines = new List<Line>();          // 공간을 나누는 선분

                acPolylines.ForEach(pl =>
                {
                    var comparePolylines = acPolylines.Where(a => a != pl);

                    for (int i = 0; i < pl.NumberOfVertices; i++)
                    {
                        try
                        {
                            var acLine = pl.GetLineSegmentAt(i);
                            var Di = acLine.Direction;

                            var sp = acLine.StartPoint;
                            var ep = acLine.EndPoint;

                            var moved_sp = Utils.PointUtil.Move(acLine.StartPoint, -Di);
                            var moved_ep = Utils.PointUtil.Move(acLine.StartPoint, Di);

                            var LS1 = new LineSegment3d(sp, moved_sp);      // Line에서 연장된 선분
                            var LS2 = new LineSegment3d(ep, moved_ep);      // Line에서 연장된 선분

                            var IP1 = CADUtil.IntersectedLine(comparePolylines.ToList(), LS1, sp, Dist);
                            var IP2 = CADUtil.IntersectedLine(comparePolylines.ToList(), LS2, ep, Dist);

                            if (IP1 != new Point3d())
                            {
                                var L1 = CADUtil.CreateLine(sp, IP1, ColorIndex.Gray);
                                separate_lines.Add(L1);
                            }
                            if (IP2 != new Point3d())
                            {
                                var L2 = CADUtil.CreateLine(ep, IP2, ColorIndex.Gray);
                                separate_lines.Add(L2);
                            }
                        }
                        catch
                        {
                        }
                    }
                });

                AC.Editor.UpdateScreen();
                #endregion

                #region TEXT 숨기기
                var texts = Utils.Get.GetAllObject<DBText>();
                var dims = Utils.Get.GetAllObject<AlignedDimension>();

                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    texts.ForEach(text =>
                    {
                        (t.GetObject(text.Id, OpenMode.ForWrite) as Entity).Visible = false;
                    });
                    dims.ForEach(dim =>
                    {
                        (t.GetObject(dim.Id, OpenMode.ForWrite) as Entity).Visible = false;
                    });

                    t.Commit();
                }
                #endregion

                #region Boundary 공간(벽체) 생성
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Utils.Layer.Create("00_벽체", ColorIndex.Gray);

                    separate_lines.ForEach(l =>
                    {
                        var direction = Utils.Vector.Get(l);
                        var cross_direction = direction.CrossProduct(Vector3d.ZAxis).GetNormal();
                        var cp = Utils.PointUtil.GetCenterP(l);

                        #region 바운더리 생성 및 기준 검토
                        var sp1 = Utils.PointUtil.Move(cp, cross_direction, 10);
                        var sp2 = Utils.PointUtil.Move(cp, -cross_direction, 10);

                        #region 포인트 표시
                        //var dbPoint1 = new DBPoint(sp1);
                        //var dbPoint2 = new DBPoint(sp2);

                        //BTR.AppendEntity(dbPoint1);
                        //BTR.AppendEntity(dbPoint2);
                        //T.AddNewlyCreatedDBObject(dbPoint1, true);
                        //T.AddNewlyCreatedDBObject(dbPoint2, true);

                        //AC.DB.Pdmode = 34;
                        //AC.DB.Pdsize = 5;
                        #endregion

                        // 바운더리 생성
                        var acObjs1 = AC.Editor.TraceBoundary(sp1, true).Cast<Polyline>().ToList();
                        var acObjs2 = AC.Editor.TraceBoundary(sp2, true).Cast<Polyline>().ToList();

                        var boundLines = new List<Polyline>();

                        if (acObjs1.Any()) boundLines.AddRange(acObjs1);
                        if (acObjs2.Any()) boundLines.AddRange(acObjs2);

                        boundLines.ForEach(acPoly =>
                        {
                            // 기준 검토
                            if (!IsContainPolyline(Boundaries, acPoly) && IsWallPolyline(acPoly, Dist))
                            {
                                acPoly.Layer = "00_벽체";

                                BTR.AppendEntity(acPoly);
                                T.AddNewlyCreatedDBObject(acPoly, true);

                                Boundaries.Add(acPoly);
                            }
                        });
                        #endregion
                    });

                    T.Commit();
                }
                #endregion

                #region TEXT 보이기
                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    texts.ForEach(text => (t.GetObject(text.Id, OpenMode.ForWrite) as Entity).Visible = true);

                    dims.ForEach(dim => (t.GetObject(dim.Id, OpenMode.ForWrite) as Entity).Visible = true);

                    t.Commit();
                }
                #endregion

                #region 레이어 보이기
                Utils.Layer.TurnOffAll(false);
                #endregion

                #region 벽체 번호 생성
                int n = 1;

                Boundaries.ForEach(pl =>
                {
                    var center = Utils.PointUtil.GetCenterP(pl);

                    var dbText = CADUtil.CreateText(center, 30, (n++).ToString());

                    Utils.Layer.SetLayer(dbText, "00_Temp");
                });
                #endregion

                #region 벽체 해치 생성
                var rooms = acPolylines.Where(x => x.Layer == "00_실면적" || x.Layer == "00_외벽라인" || x.Layer == "00_덕트");

                //var rooms = Util.Get.GetAllObject<Polyline>()
                //                .Where(x => x.Layer == "00_실면적" || x.Layer == "00_외벽라인" || x.Layer == "00_덕트");

                if (rooms.Any())
                {
                    Boundaries.ForEach(pl =>
                    {
                        #region 벽체 폴리라인 주변에 있는 룸 폴리라인을 받아옴
                        var IntersectedRooms = new List<Polyline>();

                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            var p = pl.GetPoint3dAt(i);

                            var Rs = from r in rooms
                                     where Utils.PointUtil.InOnPolyline(r, p)
                                     where !IsContainPolyline(IntersectedRooms, r)
                                     select r;

                            if (Rs.Any()) IntersectedRooms.AddRange(Rs.ToArray());
                        }
                        #endregion

                        if (IntersectedRooms.Any())
                        {
                            var RoomNames = new List<string>();

                            #region 룸 이름 받아오기
                            IntersectedRooms.ForEach(r =>
                            {
                                var pointColl = new Point3dCollection();

                                for (int i = 0; i < r.NumberOfVertices; i++)
                                {
                                    pointColl.Add(r.GetPoint3dAt(i));
                                }

                                var A = AC.Editor.SelectCrossingPolygon(pointColl);

                                if (A.Value != null)
                                {
                                    var B = Utils.Get.GetEntity<DBText>(A.Value)
                                                    .Where(x => DB.ROOM.Select(a => a.Name).Contains(x.TextString))
                                                    .Select(x => x.TextString);

                                    if (B.Any())
                                    {
                                        if (B.Count() > 1)
                                        {
                                            RoomNames.Add("외부");
                                        }
                                        else if (B.Count() == 1)
                                        {
                                            RoomNames.AddRange(B);
                                        }
                                    }
                                }
                            });
                            #endregion

                            #region 룸 정보로 레이어 생성 및 해치 생성
                            //string LayerName = "01_Wall";

                            //if (RoomNames.Any())
                            //{
                            //    RoomNames.ToList().ForEach(a => LayerName += "_" + a);
                            //}

                            //Util.Layer.Create(LayerName, ColorIndex.DarkGray);  // 레이어 생성

                            //CAD.CreateHatch(pl.ObjectId, "NET", LayerName);     // 해치 생성
                            #endregion

                            //Util.Layer.SetLayer(pl, LayerName);
                        }
                    });
                }
                #endregion

                #region Boundary 공간 맨뒤로 보내기
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    var DOT = T.GetObject(BTR.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;

                    var objToMove = new ObjectIdCollection();

                    Boundaries.Where(a => a.ObjectId != ObjectId.Null)
                          .Select(a => a.ObjectId).Distinct()
                          .ToList().ForEach(id => objToMove.Add(id));

                    if (objToMove.Count != 0) DOT.MoveToTop(objToMove);

                    T.Commit();
                }
                #endregion

                #region 제거
                separate_lines.ForEach(a => CADUtil.Erase(a.Id));
                #endregion
            }
        }
        private bool IsContainPolyline(List<Polyline> Polylines, Polyline polyline)
        {
            var Return = false;

            Polylines.ForEach(pl =>
            {
                if (Math.Round(pl.Area, 6) == Math.Round(polyline.Area, 6))
                {
                    if (Utils.PointUtil.GetCenterP(pl).DistanceTo(Utils.PointUtil.GetCenterP(polyline)) < 1)
                    {
                        Return = true;
                        return;
                    }
                }
            });

            return Return;
        }
        private bool IsWallPolyline(Polyline polyline, Double D = 500)
        {
            var n = polyline.NumberOfVertices;

            if (n > 6) return false;

            var m = 0;

            for (int i = 0; i < n; i++)
            {
                var linesegment = polyline.GetLineSegmentAt(i);

                if (linesegment.Length < D)
                {
                    m++;
                }
            }

            if (m >= 2) return true;

            return false;
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 폴리라인 결합
        /// </summary>
        [CommandMethod("PM")]
        public void Merge_Polygons()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            var Polylines = Utils.Get.GetEntity<Polyline>(Utils.Select.MultiObjs());
            if (Polylines == null) return;

            var A = GEOUtil.GetUnionPolygons(Polylines);
            CADUtil.CreatePolyline(A);

            var B = Polylines.Cast<Polyline>().Select(x => x.Id).ToList();
            CADUtil.Erases(B);
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 벽체가 포함된 실 이름 출력
        /// </summary>
        [CommandMethod("FR")]
        public void FindRoom()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            DB.Initialize();

            var acSSet = Utils.Select.SingleObj();

            if (acSSet == null) return;

            var acPoly = Utils.Get.GetEntity<Polyline>(acSSet).First();

            var NearPolylines = GetNearPolylines(acPoly);

            var RoomNames = GetRoomNames(NearPolylines);

            string s = "";

            RoomNames.ForEach(a => { s += a + "\n"; });

            MessageBox.Show(s);
        }
        private List<Polyline> GetNearPolylines(Polyline acPoly)
        {
            var R = new List<Polyline>();

            var rooms = Utils.Get.GetAllObject<Polyline>().Where(a => DB.LAYER.Contains(a.Layer));
            //.Where(x => x.Layer == "00_실면적" || x.Layer == "00_외벽라인" || x.Layer == "00_덕트");

            if (!rooms.Any()) return R;

            // 벽체 폴리라인 주변에 있는 룸 폴리라인을 받아옴
            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                var p = acPoly.GetPoint3dAt(i);

                var Rs = from r in rooms
                         where Utils.PointUtil.InOnPolyline(r, p)
                         where !IsContainPolyline(R, r)
                         select r;

                if (Rs.Any()) R.AddRange(Rs.ToArray());
            }

            return R;
        }
        private List<Polyline> GetNearPolylines(List<Polyline> acPolys, Polyline acPoly)
        {
            var R = new List<Polyline>();

            var rooms = acPolys.Where(x => x.Layer == "00_실면적" || x.Layer == "00_외벽라인" || x.Layer == "00_덕트");

            if (!rooms.Any()) return R;

            // 벽체 폴리라인 주변에 있는 룸 폴리라인을 받아옴
            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                var p = acPoly.GetPoint3dAt(i);

                var Rs = from r in rooms
                         where Utils.PointUtil.InOnPolyline(r, p)
                         where !IsContainPolyline(R, r)
                         select r;

                if (Rs.Any()) R.AddRange(Rs.ToArray());
            }

            return R;
        }

        /// 룸 폴리라인의 룸 이름을 출력
        private List<string> GetRoomNames(List<Polyline> Rooms)
        {
            var R = new List<string>();

            string msg = string.Empty;

            Rooms.ForEach(r =>
            {
                var psr = AC.Editor.SelectCrossingPolygon(Utils.Get.PointCollection(r));

                var RoomNames = Utils.Get.GetEntity<DBText>(psr.Value)
                                         .Select(a => a.TextString)
                                         .Where(a => DB.ROOM.Select(b => b.Name).Contains(a))
                                         .Where(a => !Utils.Get.IsDouble(a)).Distinct();

                if (RoomNames.Any())
                {
                    if (RoomNames.Count() > 1)
                    {
                        R.Add("외부");
                        msg += "외부" + "\n";
                    }
                    else if (RoomNames.Count() == 1)
                    {
                        R.Add(RoomNames.First());
                        msg += RoomNames.First() + "\n";
                    }
                }
            });

            //MessageBox.Show(msg);

            //AC.Editor.SetImpliedSelection(Rooms.Select(x => x.ObjectId).ToArray());

            return R;
        }
        private List<string> GetRoomNames(List<DBText> acTexts, List<Polyline> Rooms)
        {
            var R = new List<string>();

            Rooms.ForEach(r =>
            {
                var value = r.Bounds.Value;
                var RoomNames = acTexts.Where(x => DB.ROOM.Select(a => a.Name).Contains(x.TextString))
                                       .Where(x => Utils.PointUtil.IsInsidePolyline(r, x.Position, true))
                                       .Select(x => x.TextString);

                if (RoomNames.Any())
                {
                    if (RoomNames.Count() > 1)
                    {
                        R.Add("외부");
                    }
                    else if (RoomNames.Count() == 1)
                    {
                        R.Add(RoomNames.First());
                    }
                }
            });

            return R;
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 블럭 삽입하기
        /// </summary>
        [CommandMethod("IB")]
        public void InsertBlock()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();

            var BlockName = "WallLayer";
            //var BlockName = "PW248_800_2400";

            var acPPR = AC.Editor.GetPoint("점 위치 입력");
            if (acPPR.Status != PromptStatus.OK) return;

            var acPoint = acPPR.Value;

            var BR = Utils.Block.Create(BlockName, acPoint, Vector3d.XAxis);

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var br = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

                var coll = br.DynamicBlockReferencePropertyCollection;

                foreach (DynamicBlockReferenceProperty dbrp in coll)
                {
                    switch (dbrp.PropertyName)
                    {
                        //case "반전 상태1": dbrp.Value = 0; break;

                        case "공차1": dbrp.Value = 5.0; break;
                        case "석고보드1": dbrp.Value = 25.0; break;
                        case "스터드1": dbrp.Value = 35.0; break;
                        case "스터드2": dbrp.Value = 35.0; break;
                        case "석고보드2": dbrp.Value = 25.0; break;
                        case "공차2": dbrp.Value = 5.0; break;
                    }
                }

                t.Commit();
            }
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 창 입력
        /// </summary>
        [CommandMethod("IW")]
        public void InsertWindow()
        {
            #region PRE
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();
            #endregion

            #region 선택
            var acSSet = Utils.Select.MultiObjs();
            if (acSSet == null) return;

            var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);
            if (acPolys.Count == 0) return;
            #endregion

            acPolys.Where(a => a.Layer == "00_벽체").ToList().ForEach(acPoly =>
            {
                #region 인접해있는 실 이름 받아오기
                var p1 = acPoly.Bounds.Value.MinPoint;
                var p2 = acPoly.Bounds.Value.MaxPoint;
                var psr = AC.Editor.SelectCrossingWindow(p1, p2);

                var NearPolylines = Utils.Get.GetEntity<Polyline>(psr.Value)
                                         .Where(a => DB.LAYER.Contains(a.Layer)).ToList();

                var RoomNames = GetRoomNames(NearPolylines);

                if (RoomNames.Count != 2) return;
                #endregion

                #region DB에서 벽체에 맞는 창 블럭 받기
                var Room1 = RoomNames[0];
                var Room2 = RoomNames[1];

                var A = DB.WINDOW.Where(a => (a.Room1 == Room1 && a.Room2 == Room2) || (a.Room1 == Room2 && a.Room2 == Room1));

                if (!A.Any()) return;

                var Window = A.First();
                #endregion

                #region 블럭 생성[BR]
                var longL = Utils.Get.LongLine(acPoly);
                var shortL = Utils.Get.ShortLine(acPoly);

                if (Window.Thick != shortL) return;
                if (Window.Width > longL) return;

                var p = Utils.PointUtil.GetCenterP(acPoly);
                var BR = Utils.Block.Create(Window.Name, p, Vector3d.XAxis);
                #endregion

                #region 파라미터 값 입력
                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    var br = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

                    var coll = br.DynamicBlockReferencePropertyCollection;

                    foreach (DynamicBlockReferenceProperty dbrp in coll)
                    {
                        switch (dbrp.PropertyName)
                        {
                            case "벽두께": dbrp.Value = Window.Thick; break;
                            case "창너비": dbrp.Value = Window.Width; break;
                        }
                    }

                    t.Commit();
                }
                #endregion
            });
        }
        //------------------------------------------------------------------------------

        /// <summary>
        /// 문 입력
        /// </summary>
        [CommandMethod("ISD")]
        public void InsertDoor()
        {
            #region PRE
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();
            #endregion

            #region 선택
            var acSSet = Utils.Select.MultiObjs();
            if (acSSet == null) return;

            var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);
            if (acPolys.Count == 0) return;
            #endregion

            acPolys.Where(a => a.Layer == "00_벽체").ToList().ForEach(acPoly =>
            {
                #region 인접해있는 실 이름 받아오기
                var p1 = acPoly.Bounds.Value.MinPoint;
                var p2 = acPoly.Bounds.Value.MaxPoint;
                var psr = AC.Editor.SelectCrossingWindow(p1, p2);

                var NearPolylines = Utils.Get.GetEntity<Polyline>(psr.Value)
                                         .Where(a => DB.LAYER.Contains(a.Layer)).ToList();

                var RoomNames = GetRoomNames(NearPolylines);

                if (RoomNames.Count != 2) return;
                #endregion

                #region DB에서 벽체에 맞는 문 블럭 받기
                var Room1 = RoomNames[0];
                var Room2 = RoomNames[1];

                var A = DB.DOOR.Where(a => (a.Room1 == Room1 && a.Room2 == Room2) || (a.Room1 == Room2 && a.Room2 == Room1));

                if (!A.Any()) return;

                var Door = A.First();
                #endregion

                #region 생성
                var longL = Utils.Get.LongLine(acPoly);
                var shortL = Utils.Get.ShortLine(acPoly);

                if (Door.Distance > longL) return;

                var p = Utils.PointUtil.GetCenterP(acPoly);
                var BR = Utils.Block.Create(Door.Name, p, Vector3d.XAxis);
                #endregion

                #region 파라미터 값 입력
                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    var br = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

                    var coll = br.DynamicBlockReferencePropertyCollection;

                    foreach (DynamicBlockReferenceProperty dbrp in coll)
                    {
                        switch (dbrp.PropertyName)
                        {
                            case "벽두께": dbrp.Value = Door.Distance; break;
                            case "거리1": dbrp.Value = Door.Distance; break;
                        }
                    }

                    t.Commit();
                }
                #endregion
            });
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 기계실 해치 입력
        /// </summary>
        [CommandMethod("DH")]
        public void DuctHatch()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();

            var acSSet = Utils.Select.MultiObjs();

            if (acSSet == null) return;

            string layerName = "00_덕트";

            var acPolys = Utils.Get.GetEntity<Polyline>(acSSet).Where(a => a.Layer == layerName);

            acPolys.ToList().ForEach(acPoly =>
            {
                CADUtil.CreateHatch(acPoly.ObjectId, "ANSI31", layerName, 500);

                var p = Utils.PointUtil.GetCenterP(acPoly);

                CADUtil.CreateText(p, 50, "덕트");
            });


        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 객체 정보 출력
        /// </summary>
        [CommandMethod("IID")]
        public void GetInfo()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            var temps = new List<Temp>();

            var acSSet = Utils.Select.SingleObj("ID를 추출할 객체를 선택하시오.");
            if (acSSet == null) return;

            string Msg = "";

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var acEnts = from id in acSSet.GetObjectIds()
                             let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                             where acEnt is Entity
                             select acEnt;

                if (acEnts.Any())
                {
                    acEnts.ToList().ForEach(acEnt =>
                    {
                        var type = acEnt.GetType().Name;
                        var basetype = acEnt.GetType().BaseType.Name;
                        var h = acEnt.Handle;
                        var o = new ObjectId(); AC.DB.TryGetObjectId(h, out o);
                        var l = acEnt.Layer;
                        var layer = t.GetObject(acEnt.LayerId, OpenMode.ForRead) as LayerTableRecord;
                        var c = acEnt.ColorIndex == 256 ? layer.Color.ColorIndex : acEnt.ColorIndex;
                        var color = acEnt.ColorIndex == 256 ? layer.Color.ColorValue : acEnt.Color.ColorValue;
                        var rgb = string.Format("R:{0}, G:{1}, B:{2}", color.R, color.G, color.B);
                        string point = "";

                        #region #1 POLYLINE
                        if (acEnt is Polyline)
                        {
                            var acPoly = acEnt as Polyline;

                            for (int i = 0; i < acPoly.NumberOfVertices; i++)
                            {
                                try
                                {
                                    var p = acPoly.GetPoint3dAt(i);

                                    point += string.Format("Point {0}: {1}\n", i + 1, p.ToString());
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region #2 LINE
                        else if (acEnt is Line)
                        {
                            var acLine = acEnt as Line;

                            point += string.Format("Start Point: {0}\nEnd Point: {1}"
                                , acLine.StartPoint.ToString(), acLine.EndPoint.ToString());
                        }
                        #endregion

                        #region #3 AlignedDimension
                        else if (acEnt is AlignedDimension)
                        {
                            var acDim = acEnt as AlignedDimension;


                            point += string.Format("DP: {0}\nP1: {1}\nP2: {2}\n"
                                , acDim.DimLinePoint, acDim.XLine1Point, acDim.XLine2Point);
                        }

                        #endregion

                        Msg += string.Format("Type: {0}\nBaseType: {7}\nHandle: {1}\nObjectID: {2}\nLayer: {3}\nColor: {4} ({5})\n{6}\n"
                                , type, h.Value, o, l, c, rgb, point, basetype);
                    });
                }
            }

            MessageBox.Show(Msg);
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 치수선 지우기
        /// </summary>
        [CommandMethod("RD")]
        public void RemoveDim()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();

            TypedValue[] tvs = new TypedValue[]
            {
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "DIMENSION"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "TEXT"),
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);

            var acSSet = Utils.Select.MultiObjs("삭제할 치수선들을 선택해주세요.", oSf);

            if (acSSet == null) return;

            acSSet.GetObjectIds().ToList().ForEach(id =>
            {
                CADUtil.Erase(id);
            });

            AC.Editor.PostCommandPrompt();
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 치수선 생성
        /// </summary>
        [CommandMethod("CD")]
        public void CreateDim()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();

            var acSSet = Utils.Select.MultiObjs("치수선을 입력할 선을 선택하세요.");
            if (acSSet == null) return;

            var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));

            acEnts.ForEach(acEnt =>
            {
                var acPoly = acEnt as Polyline;

                #region 객체 받기
                var PoColl = Utils.Get.PointCollection(acPoly);

                var acCPSSet = AC.Editor.SelectCrossingPolygon(PoColl).Value;
                if (acCPSSet.Count == 0) return;

                var Texts = Utils.Get.GetEntity<DBText>(acCPSSet);
                var Dims = Utils.Get.GetEntity<AlignedDimension>(acCPSSet)
                                    .Where(a => Utils.PointUtil.IsInsidePolyline(acPoly, a.TextPosition, false))
                                    .Where(a => Utils.Get.Distance(acPoly, a.TextPosition) <= 150)
                                    .ToList();
                #endregion

                #region 치수선 지우기
                if (Dims.Any())
                {
                    Dims.ForEach(Dim => CADUtil.Erase(Dim.Id));
                }
                #endregion

                #region 치수선 생성
                var acDimIDs = Utils.DIMUtil.Create(acPoly, 100, false, DB.DimStyleId, DB.DimLayerId);
                #endregion
            });
        }
        private List<ObjectId> CreateDim(Polyline acPoly)
        {
            var acDimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Red);    // 레이어 생성
            var acDimStyleId = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_Dim_Style");             // 치수 스타일 생성

            return Utils.DIMUtil.Create(acPoly, 100, false, acDimStyleId, acDimLayerId);
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 실 치수선 및 면적 생성 / 갱신
        /// </summary>
        [CommandMethod("CA")]
        public void CreateArea()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            var acSSet = Utils.Select.MultiObjs("면적을 입력할 폴리라인을 선택하시오.");
            if (acSSet == null) return;

            var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);

            acPolys.ForEach(acPoly =>
            {
                #region 객체 받기
                var PoColl = Utils.Get.PointCollection(acPoly);

                var acCPSSet = AC.Editor.SelectCrossingPolygon(PoColl).Value;
                if (acCPSSet.Count == 0) return;

                //var acTexts = Util.Get.GetEntity<DBText>(acCPSSet).Where(x => x.TextString.Contains("m²")).ToList();
                var Texts = Utils.Get.GetEntity<DBText>(acCPSSet);
                var Dims = Utils.Get.GetEntity<AlignedDimension>(acCPSSet)
                                    .Where(a => Utils.PointUtil.IsInsidePolyline(acPoly, a.TextPosition, false))
                                    .Where(a => Utils.Get.Distance(acPoly, a.TextPosition) <= 150)
                                    .ToList();
                #endregion

                #region 면적 지우기
                if (Texts.Any())
                {
                    var acTexts = Texts.Where(x => Utils.Get.IsDouble(x.TextString)).ToList();

                    if (acTexts.Count == 1)
                    {
                        acTexts.ForEach(txt => CADUtil.Erase(txt.Id));
                    }
                }
                #endregion

                #region 치수선 지우기
                if (Dims.Any())
                {
                    Dims.ForEach(Dim => CADUtil.Erase(Dim.Id));
                }
                #endregion

                #region 치수선 생성
                var acDimIDs = Utils.DIMUtil.Create(acPoly, 100, false, DB.DimStyleId, DB.DimLayerId);
                #endregion

                #region 면적 생성
                var p = Utils.PointUtil.Move(Utils.PointUtil.GetCenterP(acPoly), 0, -150);

                var PL_Area = acPoly.Area;  // mm²

                var Area = Math.Round(PL_Area / 1000000, 4); // m²

                var acText = CADUtil.CreateText(p, 200, Area.ToString(), ColorIndex.Red);
                #endregion
            });
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 객체 좌표 정제하기
        /// </summary>
        [CommandMethod("RF")]
        public void RefineElements()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            var acSSet = Utils.Select.MultiObjs("좌표 오차를 맞출 객체들을 선택하시오.");
            if (acSSet == null) return;

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                #region #1 POLYLINE
                var acPolys = from a in acSSet.GetObjectIds()
                              let b = t.GetObject(a, OpenMode.ForWrite) as Polyline
                              where b != null
                              select b;

                if (acPolys.Any())
                {
                    acPolys.ToList().ForEach(acPoly =>
                    {
                        for (int i = 0; i < acPoly.NumberOfVertices; i++)
                        {
                            try
                            {
                                var p = acPoly.GetPoint3dAt(i);
                                var x = Math.Round(p.X);
                                var y = Math.Round(p.Y);

                                acPoly.SetPointAt(i, new Point2d(x, y));
                            }
                            catch
                            {
                            }
                        }
                    });
                }
                #endregion

                #region #2 LINE
                var acLines = from a in acSSet.GetObjectIds()
                              let b = t.GetObject(a, OpenMode.ForWrite) as Line
                              where b != null
                              select b;

                if (acLines.Any())
                {
                    acLines.ToList().ForEach(acLine =>
                    {
                        acLine.StartPoint = SetPoint(acLine.StartPoint);
                        acLine.EndPoint = SetPoint(acLine.EndPoint);
                    });
                }
                #endregion

                #region #3 Dimension
                var acDimensions = from a in acSSet.GetObjectIds()
                                   let b = t.GetObject(a, OpenMode.ForWrite)
                                   where b.GetType().BaseType == typeof(Dimension)
                                   select b;

                if (acDimensions.Any())
                {
                    acDimensions.ToList().ForEach(dim =>
                    {
                        if (dim.GetType() == typeof(AlignedDimension))
                        {
                            var acAlDim = dim as AlignedDimension;

                            acAlDim.TextPosition = SetPoint(acAlDim.TextPosition);
                            acAlDim.DimLinePoint = SetPoint(acAlDim.DimLinePoint);
                            acAlDim.XLine1Point = SetPoint(acAlDim.XLine1Point);
                            acAlDim.XLine2Point = SetPoint(acAlDim.XLine2Point);
                        }
                        else if (dim.GetType() == typeof(ArcDimension))
                        {
                            var acArDim = dim as ArcDimension;

                            acArDim.ArcPoint = SetPoint(acArDim.ArcPoint);
                            acArDim.CenterPoint = SetPoint(acArDim.CenterPoint);
                            acArDim.Leader1Point = SetPoint(acArDim.Leader1Point);
                            acArDim.Leader2Point = SetPoint(acArDim.Leader2Point);
                            acArDim.XLine1Point = SetPoint(acArDim.XLine1Point);
                            acArDim.XLine2Point = SetPoint(acArDim.XLine2Point);
                            acArDim.TextPosition = SetPoint(acArDim.TextPosition);
                        }
                        else if (dim.GetType() == typeof(DiametricDimension))
                        {
                            var acDiDim = dim as DiametricDimension;

                            acDiDim.TextPosition = SetPoint(acDiDim.TextPosition);
                            acDiDim.ChordPoint = SetPoint(acDiDim.ChordPoint);
                            acDiDim.FarChordPoint = SetPoint(acDiDim.FarChordPoint);
                        }
                        else if (dim.GetType() == typeof(OrdinateDimension))
                        {
                            var acOrDim = dim as OrdinateDimension;

                            acOrDim.TextPosition = SetPoint(acOrDim.TextPosition);
                            acOrDim.DefiningPoint = SetPoint(acOrDim.DefiningPoint);
                            acOrDim.LeaderEndPoint = SetPoint(acOrDim.LeaderEndPoint);
                        }
                        else if (dim.GetType() == typeof(Point3AngularDimension))
                        {
                            var acPoDim = dim as Point3AngularDimension;

                            acPoDim.TextPosition = SetPoint(acPoDim.TextPosition);
                            acPoDim.ArcPoint = SetPoint(acPoDim.ArcPoint);
                            acPoDim.CenterPoint = SetPoint(acPoDim.CenterPoint);
                            acPoDim.XLine1Point = SetPoint(acPoDim.XLine1Point);
                            acPoDim.XLine2Point = SetPoint(acPoDim.XLine2Point);
                        }
                        else if (dim.GetType() == typeof(RadialDimension))
                        {
                            var acRaDim = dim as RadialDimension;

                            acRaDim.TextPosition = SetPoint(acRaDim.TextPosition);
                            acRaDim.ChordPoint = SetPoint(acRaDim.ChordPoint);
                        }
                        else if (dim.GetType() == typeof(RotatedDimension))
                        {
                            var acRoDim = dim as RotatedDimension;

                            acRoDim.TextPosition = SetPoint(acRoDim.TextPosition);
                            acRoDim.DimLinePoint = SetPoint(acRoDim.DimLinePoint);
                            acRoDim.XLine1Point = SetPoint(acRoDim.XLine1Point);
                            acRoDim.XLine2Point = SetPoint(acRoDim.XLine2Point);
                        }
                    });
                }
                #endregion

                #region #4 CIRCLE
                var acCircles = from a in acSSet.GetObjectIds()
                                let b = t.GetObject(a, OpenMode.ForWrite) as Circle
                                where b != null
                                select b;

                if (acCircles.Any())
                {
                    acCircles.ToList().ForEach(acCircle =>
                    {
                        acCircle.Center = SetPoint(acCircle.Center);
                        acCircle.Radius = Math.Round(acCircle.Radius, 2);
                    });
                }
                #endregion

                #region #5 ARC
                var acArcs = from a in acSSet.GetObjectIds()
                             let b = t.GetObject(a, OpenMode.ForWrite) as Arc
                             where b != null
                             select b;

                if (acArcs.Any())
                {
                    acArcs.ToList().ForEach(acArc =>
                    {
                        acArc.Center = SetPoint(acArc.Center);
                        acArc.Radius = Math.Round(acArc.Radius, 2);
                        acArc.StartAngle = To.ToRadian(Math.Round(To.ToDegree(acArc.StartAngle)));
                        acArc.EndAngle = To.ToRadian(Math.Round(To.ToDegree(acArc.EndAngle)));
                    });
                }
                #endregion

                t.Commit();
            }
        }
        private Point3d SetPoint(Point3d P)
        {
            return new Point3d(Math.Round(P.X), Math.Round(P.Y), 0);
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 치수선 일괄 생성
        /// </summary>
        [CommandMethod("CCD")]
        public void CreateCombinedDimension()
        {
            #region 초기화
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();
            #endregion

            #region 첫번째 점
            var opt1 = new PromptPointOptions("첫번째 지점을 클릭 하시오");
            var PPR1 = AC.Editor.GetPoint(opt1);
            if (PPR1.Status != PromptStatus.OK) return;
            var P1 = PPR1.Value;
            #endregion

            #region 두번째 점
            var opt2 = new PromptPointOptions("두번째 지점을 클릭 하시오");
            opt2.UseBasePoint = true;
            opt2.BasePoint = P1;
            opt2.UseDashedLine = true;
            var PPR2 = AC.Editor.GetPoint(opt2);
            if (PPR2.Status != PromptStatus.OK) return;
            var P2 = PPR2.Value;
            #endregion

            #region 첫번째 점과 두번째 점을 읻는 선이 교차되는 객체
            var acSSet = Utils.Select.IntersectedObjs(P1, P2);
            if (acSSet == null) return;

            var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);
            #endregion

            #region 교차점 받아오기
            var IPs = new List<Point3d>();
            var curve = CADUtil.CreateCurve(P1, P2);

            acPolys.ToList().ForEach(acPoly =>
            {
                IPs.AddRange(Utils.Get.IntersectedPoints(acPoly, curve));
            });

            CADUtil.Erase(curve.Id);

            var vector = Utils.Vector.Get(P1, P2);
            var points = vector.Y == 0 ? IPs.OrderBy(x => x.X).ToList() : IPs.OrderBy(x => -x.Y).ToList();
            #endregion

            //#region 레이어 및 치수 스타일 생성
            //var acDimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Red);    // 레이어 생성
            //var acDimStyleId = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_도면 치수선");             // 치수 스타일 생성
            //#endregion

            #region 치수선 생성
            var ObjIDs = new List<ObjectId>();

            var CrossVec = Vector3d.XAxis;

            double h = 800;             // 포인트로부터의 간격
            double textsize = 100;      // 텍스트 크기

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                for (int i = 0; i < points.Count - 1; i++)
                {
                    var L = new LineSegment3d(points[i], points[i + 1]);

                    if (L.Length > 1)
                    {
                        #region 치수선 포인트 위치 지정
                        CrossVec = L.Direction.CrossProduct(Vector3d.ZAxis);

                        var MP = Utils.PointUtil.GetCenterP(L);            // Middle point(중심점)
                        var DP = CADUtil.MoveP(MP, -CrossVec, h);     // dimension point(텍스트가 입력될 점)

                        //텍스트가 입력되는 높이만큼 다시 옮겨줘야 위치가 맞다.
                        if (CrossVec.GetAngleTo(Vector3d.XAxis) == Math.PI / 2)
                        {
                            DP = CADUtil.MoveP(DP, -CrossVec, -textsize);
                        }
                        else if (CrossVec.GetAngleTo(Vector3d.XAxis) == Math.PI)
                        {
                            DP = CADUtil.MoveP(DP, -CrossVec, textsize);
                        }
                        #endregion

                        #region 치수선 생성
                        var acDim = new AlignedDimension(L.StartPoint, L.EndPoint, DP, string.Empty, DB.DimStyleId2);

                        acDim.DimensionStyle = DB.DimStyleId2;    // 치수 스타일
                        acDim.LayerId = DB.DimLayerId;           // 레이어
                        acDim.Dimtxt = textsize;                // 텍스트 크기

                        BTR.AppendEntity(acDim);
                        T.AddNewlyCreatedDBObject(acDim, true);

                        if (acDim.Id != ObjectId.Null) ObjIDs.Add(acDim.Id);
                        #endregion
                    }
                }
                T.Commit();
            }
            #endregion

            #region 세번째 점
            var P3 = new Point3d();
            var opt3 = new PromptPointOptions("세번째 지점을 클릭 하시오");
            opt3.UseBasePoint = true;
            opt3.BasePoint = P2;
            opt3.UseDashedLine = true;
            var PPR3 = AC.Editor.GetPoint(opt3);
            if (PPR3.Status != PromptStatus.OK) return;
            P3 = PPR3.Value;
            #endregion

            #region 치수선 좌표이동
            double length = P3.DistanceTo(P2);

            var vector2 = Utils.Vector.Get(P2, P3);

            double ang = Utils.Get.Angle(Vector3d.XAxis, vector2);

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                ObjIDs.ForEach(ObjId =>
                {
                    var acDim = t.GetObject(ObjId, OpenMode.ForWrite) as AlignedDimension;

                    if (ang > Math.PI / 2 || ang < 0)
                    {
                        acDim.TextPosition = Utils.PointUtil.Move(acDim.TextPosition, CrossVec, h * 2);
                        acDim.DimLinePoint = Utils.PointUtil.Move(acDim.DimLinePoint, CrossVec, h * 2);
                    }

                    acDim.TextPosition = Utils.PointUtil.Move(acDim.TextPosition, vector2, length);
                    acDim.DimLinePoint = Utils.PointUtil.Move(acDim.DimLinePoint, vector2, length);
                    acDim.XLine1Point = Utils.PointUtil.Move(acDim.XLine1Point, vector2, length);
                    acDim.XLine2Point = Utils.PointUtil.Move(acDim.XLine2Point, vector2, length);
                });

                t.Commit();
            }
            #endregion
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 치수선 합치기
        /// </summary>
        [CommandMethod("MD")]
        public void MergeDimension()
        {
            #region 초기화
            AC.Initialize(); if (!AC.IsLogIn) return;
            DB.Initialize();
            #endregion

            #region 치수선 선택
            var acSSet = Utils.Select.MultiObjs("합칠 치수선들을 선택해주세요.");
            if (acSSet == null) return;

            var Dims = Utils.Get.GetEntity<AlignedDimension>(acSSet);

            var Vecs = from a in Dims
                       let v = Utils.DIMUtil.GetVector(a)
                       select Utils.Vector.RefineVector(v);

            if (!Vecs.Any()) return;
            #endregion

            Vecs.Distinct().ToList().ForEach(vec =>
            {
                #region 선택된 치수선들의 시작점, 끝점 찾기 [Min, Max]
                var dims = from a in Dims
                           let v = Utils.DIMUtil.GetVector(a)
                           where vec == Utils.Vector.RefineVector(v)
                           select a;

                if (!dims.Any()) return;

                var points = new List<Point3d>();

                dims.ToList().ForEach(dimm =>
                {
                    points.Add(dimm.XLine1Point);
                    points.Add(dimm.XLine2Point);
                });

                points = points.Distinct().ToList();
                points = Math.Round(vec.Y) == 0 ? points.OrderBy(x => x.Y).ToList() : points.OrderBy(x => x.X).ToList();

                var Min = points.First();
                var Max = points.Last();
                #endregion

                //#region 레이어 및 치수 스타일 생성
                //var acDimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Red);    // 레이어 생성
                //var acDimStyleId = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_도면 치수선");             // 치수 스타일 생성
                //#endregion

                #region 치수선 생성
                var dim = dims.First();

                var tp = dim.TextPosition;
                var ls = new LineSegment3d(Min, Max);

                double h = ls.GetDistanceTo(tp) + 500;             // 포인트로부터의 간격
                double textsize = 100;      // 텍스트 크기

                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    if (ls.Length > 1)
                    {
                        #region 치수선 포인트 지정
                        var DP = CADUtil.MoveP(ls.MidPoint, vec, h);         // dimension point(텍스트가입력될 점)
                        #endregion

                        #region 치수선 생성
                        var acDim = new AlignedDimension(Min, Max, DP, string.Empty, DB.DimStyleId2);

                        acDim.DimensionStyle = DB.DimStyleId2;    // 치수 스타일
                        acDim.LayerId = DB.DimLayerId;           // 레이어
                        acDim.Dimtxt = textsize;                // 텍스트 크기

                        BTR.AppendEntity(acDim);
                        T.AddNewlyCreatedDBObject(acDim, true);
                        #endregion
                    }
                    T.Commit();
                }
                #endregion
            });
        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 그리기 순서 지정 및 폴리라인 전역폭 설정
        /// </summary>
        [CommandMethod("PPO")]
        public void PolylineOrder()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            var A = Utils.Get.GetEntity<Polyline>(Utils.Select.MultiObjs("폴리라인을 선택하시오."));

            if (A.Count == 0) return;

            var layernames = (from a in A select a.Layer).ToList().Distinct().ToList();

            var UI = new U_List(layernames);

            if (UI.ShowDialog() != DialogResult.OK) return;

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var DOT = t.GetObject(BTR.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;

                var objToMove = new ObjectIdCollection();

                #region 초기 세팅
                A.Where(a => a.ObjectId != ObjectId.Null)
                 .Select(a => a.ObjectId).Distinct()
                 .ToList().ForEach(id => objToMove.Add(id));

                if (objToMove.Count != 0) DOT.MoveToBottom(objToMove);

                A.ForEach(a =>
                {
                    var b = t.GetObject(a.Id, OpenMode.ForWrite) as Polyline;
                    b.ConstantWidth = 0;
                });
                #endregion

                #region 선택 레이어 정렬
                var acPolys = from a in A
                              where a.Layer == UI.item
                              select a;

                if (acPolys.Any())
                {
                    objToMove.Clear();

                    acPolys.Where(a => !a.IsErased).Select(a => a.ObjectId).Distinct().ToList().ForEach(id => objToMove.Add(id));

                    // 맨 위로 가져오기
                    if (objToMove.Count != 0) DOT.MoveToTop(objToMove);

                    acPolys.ToList().ForEach(a =>
                    {
                        var b = t.GetObject(a.Id, OpenMode.ForWrite) as Polyline;

                        b.ConstantWidth = UI.size;
                    });
                }
                #endregion

                t.Commit();
            }

        }
        //------------------------------------------------------------------------------


        /// <summary>
        /// 선택한 객체들의 레이어 받아오기
        /// </summary>
        [CommandMethod("GL")]
        public void GetLayer()
        {
            AC.Initialize(); if (!AC.IsLogIn) return;

            var A = Utils.Select.MultiObjs();

            if (A == null) return;

            var Layers = new List<LayerTableRecord>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                #region CAD 객체 레이어 담기
                var acLayers = from id in A.GetObjectIds()
                               let Ent = t.GetObject(id, OpenMode.ForRead) as Entity
                               let LTR = t.GetObject(Ent.LayerId, OpenMode.ForWrite) as LayerTableRecord
                               orderby LTR.Name
                               select LTR;

                if (!acLayers.Any()) return;

                var B = acLayers.ToList();
                #endregion

                #region CAD 블럭 객체에 포함된 레이어 담기
                //var acBlocks = from id in A.GetObjectIds()
                //               where (t.GetObject(id, OpenMode.ForWrite) as BlockReference) != null
                //               let BR = t.GetObject(id, OpenMode.ForRead) as BlockReference
                //               select BR;

                //if (acBlocks.Any())
                //{
                //    acBlocks.ToList().ForEach(a =>
                //    {
                //        var C = new DBObjectCollection();

                //        a.Explode(C);

                //        var D = from c in C.Cast<DBObject>()
                //                let ent = c as Entity
                //                where ent != null
                //                let LTR = t.GetObject(ent.LayerId, OpenMode.ForRead) as LayerTableRecord
                //                orderby LTR.Name
                //                where !B.Contains(LTR)
                //                select LTR;

                //        if (D.Any())
                //        {
                //            B.AddRange(D.ToArray());
                //        }
                //    });
                //}
                #endregion

                var selectedLayers = (from b in B orderby b.Name select b).Distinct().ToList();

                var U = new U_Layer(selectedLayers);

                U.Show();
            }
        }
        //------------------------------------------------------------------------------



        #region TEST
        //[CommandMethod("MM")]
        //public void MoveWall()
        //{
        //    AC.Initialize();

        //    #region 2개의 폴리라인 선택
        //    var acSSet = Util.Select.MultiObjs();

        //    PromptSelectionOptions opt1 = new PromptSelectionOptions();
        //    opt1.SingleOnly = false;
        //    opt1.MessageForAdding = "변경할 Polyline을 선택해주시오";

        //    var acPolylines = Util.Get.GetEntity<Polyline>(acSSet);
        //    #endregion

        //    #region 영역 설정
        //    var acSSet1 = AC.Editor.GetPoint("시작점 클릭");
        //    if (acSSet1.Status != PromptStatus.OK) return;
        //    var SP = acSSet1.Value;

        //    var opt2 = new PromptPointOptions("option");
        //    opt2.UseDashedLine = true;
        //    opt2.UseBasePoint = true;
        //    opt2.BasePoint = SP;
        //    opt2.AllowArbitraryInput = true;
        //    opt2.Message = "끝점 클릭";

        //    var acSSet2 = AC.Editor.GetPoint(opt2);
        //    if (acSSet2.Status != PromptStatus.OK) return;
        //    var EP = acSSet2.Value;
        //    #endregion

        //}

        //[CommandMethod("IB")]
        //public void InsertBlock()
        //{
        //    AC.Initialize();
        //    DB.Initialize();

        //    var BlockName = "PDset";
        //    //var BlockName = "PW248_800_2400";

        //    var acPPR = AC.Editor.GetPoint("점 위치 입력");
        //    if (acPPR.Status != PromptStatus.OK) return;

        //    var acPoint = acPPR.Value;

        //    var BR = Util.Block.Create(BlockName, acPoint, Vector3d.YAxis);

        //    using (Transaction t = AC.DB.TransactionManager.StartTransaction())
        //    {
        //        var br = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

        //        var coll = br.DynamicBlockReferencePropertyCollection;

        //        foreach (DynamicBlockReferenceProperty dbrp in coll)
        //        {
        //            switch (dbrp.PropertyName)
        //            {
        //                case "창너비": dbrp.Value = 1200.0; break;
        //                case "벽두께": dbrp.Value = 350.0; break;
        //            }
        //        }

        //        t.Commit();
        //    }
        //}

        //[CommandMethod("AAA")]
        //public void SelectTest()
        //{
        //    AC.Initialize();
        //    DB.Initialize();

        //    var acSSet = Util.Select.SingleObj();

        //    var acPoly = Util.Get.GetEntity<Polyline>(acSSet).First();

        //    var pointColl = new Point3dCollection();
        //    for (int i = 0; i < acPoly.NumberOfVertices; i++)
        //    {
        //        pointColl.Add(acPoly.GetPoint3dAt(i));
        //    }

        //    var A = AC.Editor.SelectCrossingPolygon(pointColl);

        //    MessageBox.Show(A.Value.Count.ToString());

        //    var B = Util.Get.GetEntity<DBText>(A.Value)
        //                    .Where(x => DB.ROOM.Contains(x.TextString))
        //                    .Select(x => x.TextString);

        //    MessageBox.Show(B.Count().ToString());
        //}

        //[CommandMethod("HTEST")]
        //public void HTEST()
        //{
        //    AC.Initialize();
        //    DB.Initialize();

        //    var temps = new List<Temp>();

        //    bool B = true;

        //    while (B)
        //    {
        //        var acSSet = Util.Select.SingleObj();
        //        if (acSSet == null)
        //        {
        //            B = false;
        //            break;
        //        }

        //        var acPoly = Util.Get.GetEntity<Polyline>(acSSet).First();
        //        if (acPoly == null)
        //        {
        //            B = false;
        //            break;
        //        }

        //        var handle = acPoly.Handle;

        //        var objID = new ObjectId(); AC.DB.TryGetObjectId(handle, out objID);

        //        var temp = new Temp((temps.Count + 1).ToString(), acSSet[0].GetType().Name, handle.Value.ToString(), objID.ToString());

        //        temps.Add(temp);
        //    }

        //    if (temps.Count > 0)
        //    {
        //        U_Test UI = new U_Test(temps);

        //        UI.Show();
        //    }
        //}
        #endregion

        #region TEST
        [CommandMethod("TEST02")]
        public void TEST02()
        {
            var OFD = new OpenFileDialog();

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var path = OFD.FileName;

                using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(path))
                {
                    Aspose.CAD.ImageOptions.CadRasterizationOptions opt = new Aspose.CAD.ImageOptions.CadRasterizationOptions();
                    opt.PageWidth = 1200;
                    opt.PageHeight = 1200;

                    ImageOptionsBase opts = new Aspose.CAD.ImageOptions.PdfOptions();

                    opts.VectorRasterizationOptions = opt;

                    image.Save(path + "_TEST.pdf");
                }
            }
        }
        #endregion

    }

    public class Temp
    {
        public string No { get; set; }
        public string Type { get; set; }
        public string Handle { get; set; }
        public string ObjectID { get; set; }

        public Temp()
        {

        }

        public Temp(string no, string type, string handle, string objectid)
        {
            this.No = no;
            this.Type = type;
            this.Handle = handle;
            this.ObjectID = objectid;
        }
    }

    public class WallLayer
    {
        #region 속성
        // Left(외부)
        public double LT { get; set; }
        public double LF1 { get; set; }
        public double LF2 { get; set; }
        public double LF3 { get; set; }
        public double LF4 { get; set; }
        public double LF5 { get; set; }

        // Right(내부)
        public double RT { get; set; }
        public double RF1 { get; set; }
        public double RF2 { get; set; }
        public double RF3 { get; set; }
        public double RF4 { get; set; }
        public double RF5 { get; set; }
        public double RF6 { get; set; }
        public double RF7 { get; set; }
        #endregion

        #region 생성자
        public WallLayer()
        {
            // Left(외부)
            this.LT = 0;
            this.LF1 = 0;
            this.LF2 = 0;
            this.LF3 = 0;
            this.LF4 = 0;
            this.LF5 = 0;

            // Right(내부)
            this.RT = 0;
            this.RF1 = 0;
            this.RF2 = 0;
            this.RF3 = 0;
            this.RF4 = 0;
            this.RF5 = 0;
            this.RF6 = 0;
            this.RF7 = 0;
        }

        public WallLayer(double lt, double lf1, double lf2, double lf3, double lf4, double lf5,
                         double rt, double rf1, double rf2, double rf3, double rf4, double rf5, double rf6, double rf7)
        {

        }
        #endregion
    }

    public class C_Wall
    {
        #region 속성
        // Left 공차
        public double LT { get; set; }

        public double MW { get; set; }

        // Right 공차
        public double RT { get; set; }
        // Right 마감1
        public double RF1 { get; set; }
        // Right 마감2
        public double RF2 { get; set; }
        // Right 마감3
        public double RF3 { get; set; }
        // Right 마감4
        public double RF4 { get; set; }
        // Right 마감5
        public double RF5 { get; set; }
        // Right 마감6
        public double RF6 { get; set; }

        public double SUM { get; set; }
        #endregion

        #region 생성자
        public C_Wall()
        {
            this.LT = 0;

            this.RT = 0;
            this.RF1 = 0;
            this.RF2 = 0;
            this.RF3 = 0;
            this.RF4 = 0;
            this.RF5 = 0;
            this.RF6 = 0;
        }

        public C_Wall(double lt, double mw, double rt, double rf1, double rf2, double rf3, double rf4, double rf5, double rf6, double rf7) : this()
        {
            this.LT = lt;

            this.MW = mw;

            this.RT = rt;
            this.RF1 = rf1;
            this.RF2 = rf2;
            this.RF3 = rf3;
            this.RF4 = rf4;
            this.RF5 = rf5;
            this.RF6 = rf6;

            this.SUM = lt + mw + rt + rf1 + rf2 + rf3 + rf4 + rf5 + rf6 + rf7;
        }
        #endregion

        #region PUBLIC 메서드

        #endregion

        #region PRIVATE 메서드

        #endregion
    }

    public class B_Wall
    {
        #region 속성
        // Left 공차
        public double LT { get; set; }
        // Left 마감1
        public double LF1 { get; set; }
        // Left 마감2
        public double LF2 { get; set; }
        // Left 마감3
        public double LF3 { get; set; }
        // Left 마감4
        public double LF4 { get; set; }
        // Left 마감5
        public double LF5 { get; set; }

        public double MW { get; set; }

        // Right 공차
        public double RT { get; set; }
        // Right 마감1
        public double RF1 { get; set; }
        // Right 마감2
        public double RF2 { get; set; }
        // Right 마감3
        public double RF3 { get; set; }
        // Right 마감4
        public double RF4 { get; set; }
        // Right 마감5
        public double RF5 { get; set; }
        // Right 마감6
        public double RF6 { get; set; }
        // Right 마감7
        public double RF7 { get; set; }

        public double SUM { get; set; }
        #endregion

        #region 생성자
        public B_Wall()
        {
            this.LT = 0;
            this.LF1 = 0;
            this.LF2 = 0;
            this.LF3 = 0;
            this.LF4 = 0;
            this.LF5 = 0;

            this.MW = 0;

            this.RT = 0;
            this.RF1 = 0;
            this.RF2 = 0;
            this.RF3 = 0;
            this.RF4 = 0;
            this.RF5 = 0;
            this.RF6 = 0;
            this.RF7 = 0;
        }

        public B_Wall(double lt, double lf1, double lf2, double lf3, double lf4, double lf5, double mw,
                      double rt, double rf1, double rf2, double rf3, double rf4, double rf5, double rf6, double rf7) : this()
        {
            this.LT = lt;
            this.LF1 = lf1;
            this.LF2 = lf2;
            this.LF3 = lf3;
            this.LF4 = lf4;
            this.LF5 = lf5;

            this.MW = mw;

            this.RT = rt;
            this.RF1 = rf1;
            this.RF2 = rf2;
            this.RF3 = rf3;
            this.RF4 = rf4;
            this.RF5 = rf5;
            this.RF6 = rf6;
            this.RF7 = rf7;

            this.SUM = lt + lf1 + lf2 + lf3 + lf4 + lf5 + mw + rt + rf1 + rf2 + rf3 + rf4 + rf5 + rf6 + rf7;
        }
        #endregion

        #region PUBLIC 메서드

        #endregion

        #region PRIVATE 메서드

        #endregion
    }

    public class L_Wall
    {
        #region 속성
        // Left 공차
        public double LT { get; set; }
        // Left 마감1
        public double LF1 { get; set; }
        // Left 마감2
        public double LF2 { get; set; }
        // Left 마감3
        public double LF3 { get; set; }
        // Left 마감4
        public double LF4 { get; set; }
        // Left 마감5
        public double LF5 { get; set; }

        public double MW { get; set; }

        // Right 공차
        public double RT { get; set; }
        // Right 마감1
        public double RF1 { get; set; }

        public double SUM { get; set; }
        #endregion

        #region 생성자
        public L_Wall()
        {
            this.LT = 0;
            this.LF1 = 0;
            this.LF2 = 0;
            this.LF3 = 0;
            this.LF4 = 0;
            this.LF5 = 0;

            this.MW = 0;

            this.RT = 0;
            this.RF1 = 0;
            this.SUM = 0;
        }

        public L_Wall(double lt, double lf1, double lf2, double lf3, double lf4, double lf5, double mw, double rt, double rf1) : this()
        {
            this.LT = lt;
            this.LF1 = lf1;
            this.LF2 = lf2;
            this.LF3 = lf3;
            this.LF4 = lf4;
            this.LF5 = lf5;

            this.MW = MW;

            this.RT = rt;
            this.RF1 = rf1;

            this.SUM = lt + lf1 + lf2 + lf3 + lf4 + lf5 + mw + rt + rf1;
        }
        #endregion

        #region PUBLIC 메서드

        #endregion

        #region PRIVATE 메서드

        #endregion
    }
}
