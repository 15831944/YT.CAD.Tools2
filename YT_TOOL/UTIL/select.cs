using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_CAD_TOOL
{
    public class select
    {
        public static T Object<T>(string text) where T : new()
        {
            AC.Editor.SetImpliedSelection(new ObjectId[0]);

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            var opt = new PromptSelectionOptions();
            opt.MessageForAdding = text;
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            var set = AC.Editor.GetSelection(opt);

            if (set.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return default(T);
            }

            if (set.Value == null) return default(T);

            using (Transaction t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                var A = from id in set.Value.GetObjectIds()
                        let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                        where acEnt is T
                        select (T)Convert.ChangeType(acEnt, typeof(T));

                return A.Any() ? A.First() : default(T);
            }
        }

        public static List<T> Objects<T>() where T : new()
        {
            AC.Editor.SetImpliedSelection(new ObjectId[0]);

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            var opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";

            var set = AC.Editor.GetSelection(opt);

            if (set.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return default(List<T>);
            }

            if (set.Value == null) return default(List<T>);

            using (Transaction t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                var A = from id in set.Value.GetObjectIds()
                        let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                        where acEnt is T
                        select (T)Convert.ChangeType(acEnt, typeof(T));

                return A.Any() ? A.ToList() : default(List<T>);
            }
        }

        public static SelectionSet SingleObj()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet SingleObj(string Msg)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet SingleObj(SelectionFilter filter)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, filter);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }

        public static SelectionSet MultiObjs()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet MultiObjs(string Msg)
        {
            AC.Editor.SetImpliedSelection(new ObjectId[0]);

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPSR.Value;
        }
        public static List<ObjectId> MultiObjIDs(string Msg)
        {
            var A = MultiObjs(Msg);

            return A != null ? A.GetObjectIds().ToList() : new List<ObjectId>();
        }
        public static SelectionSet MultiObjs(SelectionFilter filter)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, filter);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet MultiObjs(string Msg, PromptSelectionOptions Opt)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            Opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(Opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet MultiObjs(string Msg, SelectionFilter filter)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, filter);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }

        public static List<Polyline> SetMultiObjs(string Msg)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            #region 키워드 받기
            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
            pKeyOpts.Message = "\n도면 번호를 입력하세요.";
            pKeyOpts.Keywords.Add("P01");
            pKeyOpts.Keywords.Add("P02");
            pKeyOpts.Keywords.Add("P03");
            pKeyOpts.Keywords.Add("P04");
            pKeyOpts.Keywords.Add("P05");
            pKeyOpts.Keywords.Add("P06");
            pKeyOpts.Keywords.Add("P07");
            pKeyOpts.Keywords.Add("P08");
            pKeyOpts.Keywords.Add("P09");
            pKeyOpts.Keywords.Add("P10");
            pKeyOpts.Keywords.Add("P11");
            pKeyOpts.Keywords.Add("P12");
            pKeyOpts.Keywords.Add("Test");

            pKeyOpts.AllowNone = false;
            pKeyOpts.AppendKeywordsToMessage = true;

            PromptResult pKeyRes = AC.Editor.GetKeywords(pKeyOpts);
            #endregion

            #region 폴리라인 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;
            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);
            #endregion

            #region 입력
            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return new List<Polyline>();
            }
            else
            {
                var A = acPSR.Value;
                var B = Get.Objects<Polyline>(A);
                //var R = from b in B select new iRoom(b);

                //if (R.Any())
                //{
                //    //switch (pKeyRes.StringResult)
                //    //{
                //    //    case "P01":
                //    //        PLANDB.P01.Add(R.ToList()); break;
                //    //    case "P02":
                //    //        PLANDB.P02.Add(R.ToList()); break;
                //    //    case "P03":
                //    //        PLANDB.P03.Add(R.ToList()); break;
                //    //    case "P04":
                //    //        PLANDB.P04.Add(R.ToList()); break;
                //    //    case "P05":
                //    //        PLANDB.P05.Add(R.ToList()); break;
                //    //    case "P06":
                //    //        PLANDB.P06.Add(R.ToList()); break;
                //    //    case "P07":
                //    //        PLANDB.P07.Add(R.ToList()); break;
                //    //    case "P08":
                //    //        PLANDB.P08.Add(R.ToList()); break;
                //    //    case "P09":
                //    //        PLANDB.P09.Add(R.ToList()); break;
                //    //    case "P10":
                //    //        PLANDB.P10.Add(R.ToList()); break;
                //    //    case "P11":
                //    //        PLANDB.P11.Add(R.ToList()); break;
                //    //    case "P12":
                //    //        PLANDB.P12.Add(R.ToList()); break;
                //    //}
                //}

                return B;
            }
            #endregion
        }

        public static List<Polyline> Polylines()
        {
            var SS = MultiObjs();

            if (SS == null) return null;

            return Get.Objects<Polyline>(SS);
        }

        public static List<ObjectId> InPolylineObjs(string Msg)
        {
            var R = new List<ObjectId>();

            #region 폴리라인 선택
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return R;
            }

            var acPolys = Get.Objects<Polyline>(acPSR.Value);
            #endregion

            #region 폴리라인 내부의 객체 받기
            using (Transaction t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                acPolys.ForEach(acPoly =>
                {
                    var PoColl = to.Point3dCollection(acPoly);

                    var acCPSSet = AC.Editor.SelectCrossingPolygon(PoColl).Value;

                    #region 텍스트
                    var acEnts1 = from id in acCPSSet.GetObjectIds()
                                  let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                                  where acEnt.GetType() == typeof(DBText)
                                  select acEnt.ObjectId;
                    if (acEnts1.Any()) R.AddRange(acEnts1);
                    #endregion

                    #region 치수선
                    var acEnts2 = from id in acCPSSet.GetObjectIds()
                                  let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                                  where acEnt.GetType().BaseType == typeof(Dimension)
                                  let dim = acEnt as Dimension
                                  where point.IsInsidePolyline(acPoly, dim.TextPosition, false)
                                  select acEnt.ObjectId;
                    if (acEnts2.Any()) R.AddRange(acEnts2);
                    #endregion
                });
            }
            #endregion

            return R;
        }

        public static string GetKeyword(PromptKeywordOptions Opt)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            var acPR = AC.Editor.GetKeywords(Opt);

            if (acPR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPR.StringResult;
        }

        public static List<T> IntersectedObjs<T>(Point3d P1, Point3d P2)
        {
            var PoColl = new Point3dCollection();

            PoColl.Add(P1);
            PoColl.Add(P2);
            PoColl.Add(point.Move(P2, 0, 1));
            PoColl.Add(point.Move(P1, 0, 1));

            var PSR = AC.Editor.SelectCrossingPolygon(PoColl);

            if (PSR.Status != PromptStatus.OK) return null;

            return Get.Objects<T>(PSR.Value.GetObjectIds().ToList());
        }
        public static SelectionSet IntersectedObjs(Point3d P1, Point3d P2)
        {
            var PoColl = new Point3dCollection();

            PoColl.Add(P1);
            PoColl.Add(P2);
            PoColl.Add(point.Move(P2, 0, 1));
            PoColl.Add(point.Move(P1, 0, 1));

            var PSR = AC.Editor.SelectCrossingPolygon(PoColl);

            if (PSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return PSR.Value;
        }

        public static void SetImpliedSelection(PromptSelectionResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            AC.Editor.SetImpliedSelection(result.Value.GetObjectIds());
        }
        public static void SetImpliedSelection(List<Entity> entities)
        {
            AC.Editor.SetImpliedSelection(entities.Select(x => x.Id).ToArray());
        }
        public static void SetImpliedSelection(List<Polyline> polylines)
        {
            var IDs = from a in polylines select a.Id;

            if (IDs.Any())
            {
                AC.Editor.SetImpliedSelection(IDs.ToArray());
            }
        }
        public static void SetImpliedSelection(Polyline polyline)
        {
            var IDs = new List<ObjectId>();
            IDs.Add(polyline.ObjectId);

            AC.Editor.SetImpliedSelection(IDs.ToArray());
        }
        public static void SetImpliedSelection(long? handle)
        {
            if (to.ObjectId(handle).IsNull) return;

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            AC.Editor.SetImpliedSelection(new ObjectId[] { to.ObjectId(handle) });
        }

        private bool IsInsideInPolyline(Polyline acPolyline, Point3d p)
        {
            #region 경계선에 걸리는 경우
            //for (int i = 0; i < acPolyline.NumberOfVertices; i++)
            //{
            //    try
            //    {
            //        var l = acPolyline.GetLineSegmentAt(i);

            //        if (l.GetDistanceTo(p) < 0.00001) return true;
            //    }
            //    catch
            //    {
            //    }
            //}
            #endregion

            #region 점이 폴리라인 내부에 있는지 판별
            Point3d Min_P = acPolyline.GetPoint3dAt(0);
            Point3d Max_P = acPolyline.GetPoint3dAt(acPolyline.NumberOfVertices - 1);

            double total_Ang = Utils.Get.Angle(Max_P, p, Min_P);

            for (int i = 0; i < acPolyline.NumberOfVertices - 1; i++)
            {
                var line = acPolyline.GetLineSegmentAt(i);
                var SP = line.StartPoint;
                var EP = line.EndPoint;

                total_Ang += Utils.Get.Angle(SP, p, EP);
            }

            return Math.Abs(total_Ang) > 0.0000000001;
            #endregion
        }

        //public static SelectionSet CurrentViewObjects()
        //{
        //    ViewTableRecord VTR = AC.Editor.GetCurrentView();

        //    var cen = (Point3d)Application.GetSystemVariable("VIEWCTR");
        //    var W = VTR.Width;
        //    var H = VTR.Height;
        //    var min = new Point3d(cen.X - W / 2, cen.Y - H / 2, 0);
        //    var max = new Point3d(cen.X + W / 2, cen.Y + H / 2, 0);

        //    var SEL = AC.Editor.SelectCrossingPolygon(cad.to.Point3dCollection(min, max));

        //    return SEL.Value;
        //}

        //public static List<Entity> CurrentViewEntities()
        //{
        //    var Return = new List<Entity>();

        //    ViewTableRecord VTR = AC.Editor.GetCurrentView();

        //    var cen = (Point3d)Application.GetSystemVariable("VIEWCTR");
        //    var W = VTR.Width;
        //    var H = VTR.Height;
        //    var min = new Point3d(cen.X - W / 2, cen.Y - H / 2, 0);
        //    var max = new Point3d(cen.X + W / 2, cen.Y + H / 2, 0);

        //    var SEL = AC.Editor.SelectCrossingPolygon(cad.to.Point3dCollection(min, max));
        //    if (SEL.Value == null) return new List<Entity>();

        //    return Get.Entities(SEL.Value.GetObjectIds().ToList());
        //}

        public static List<T> Bundary<T>(List<Polyline> polylines, double d = 500)
        {
            if (polylines.Count == 0) return default(List<T>);

            #region 입력
            var minP = Get.MinPoint(polylines);
            var maxP = Get.MaxPoint(polylines);
            #endregion

            #region 보정
            minP = point.Move(minP, -d, -d);
            maxP = point.Move(maxP, d, d);
            #endregion

            #region 줌인
            cad.ZoomIn(minP, maxP);
            #endregion

            #region 선택
            var val = AC.Editor.SelectCrossingWindow(minP, maxP).Value;
            if (val == null) return default(List<T>);
            #endregion

            return Get.Objects<T>(val.GetObjectIds().ToList());
        }

    }
}
