using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YT_CAD_TOOL
{
    public class Get
    {
        /// <summary>
        /// 세 점이 이루는 각도 구하기
        /// </summary>
        /// <param name="A">시작점</param>
        /// <param name="B">중간점</param>
        /// <param name="C">끝점</param>
        /// <returns></returns>
        public static double Angle(Point3d A, Point3d B, Point3d C)
        {
            //var V1 = CAD.GetVector3d(B, A);
            //var V2 = CAD.GetVector3d(B, C);

            //return V1.GetAngleTo(V2);

            double dot_product = DotProduct(A, B, C);
            //double cross_product = CrossProduct(A, B, C).Length;
            double cross_product = CrossProductLength(A, B, C);

            return Math.Atan2(cross_product, dot_product);
        }
        /// <summary>
        /// 두 벡터의 각도를 구함 (+ / -)
        /// </summary>
        /// <param name="V1"></param>
        /// <param name="V2"></param>
        /// <returns></returns>
        public static double Angle(Vector3d V1, Vector3d V2)
        {
            var A = new Point3d(V1.X, V1.Y, V1.Z);
            var B = new Point3d(0, 0, 0);
            var C = new Point3d(V2.X, V2.Y, V2.Z);

            double dot_product = DotProduct(A, B, C);

            double cross_product = CrossProductLength(A, B, C);

            return Math.Atan2(cross_product, dot_product);
        }

        /// <summary>
        /// 내적 구하기
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static double DotProduct(Point3d A, Point3d B, Point3d C)
        {
            //var V1 = Vector.Get(B, A);
            //var V2 = Vector.Get(B, C);

            //return V1.DotProduct(V2);

            var Ax = A.X;
            var Ay = A.Y;
            var Bx = B.X;
            var By = B.Y;
            var Cx = C.X;
            var Cy = C.Y;

            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            return (BAx * BCx + BAy * BCy);
        }

        /// <summary>
        /// 외적 구하기
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static double CrossProductLength(Point3d A, Point3d B, Point3d C)
        {
            //var V1 = Vector.Get(B, A);
            //var V2 = Vector.Get(B, C);

            //return V1.CrossProduct(V2);

            var Ax = A.X;
            var Ay = A.Y;
            var Bx = B.X;
            var By = B.Y;
            var Cx = C.X;
            var Cy = C.Y;

            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            return (BAx * BCy - BAy * BCx);
        }

        public static double LongLine(Polyline acPoly)
        {
            double Length = 0;

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                try
                {
                    var linesegment = acPoly.GetLineSegmentAt(i);
                    if (Length < linesegment.Length)
                    {
                        Length = linesegment.Length;
                    }
                }
                catch { }
            }
            return Length;
        }

        public static double ShortLine(Polyline acPoly)
        {
            double Length = double.MaxValue;

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                try
                {
                    var linesegment = acPoly.GetLineSegmentAt(i);
                    if (Length > linesegment.Length)
                    {
                        Length = linesegment.Length;
                    }
                }
                catch { }
            }
            return Length;
        }

        public static double Distance(Polyline acPoly, Point3d P)
        {
            var R = double.MaxValue;

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                try
                {
                    var l = acPoly.GetLineSegmentAt(i);

                    var d = l.GetDistanceTo(P);

                    if (R > d)
                    {
                        R = d;
                    }
                }
                catch { }
            }

            return R;
        }

        public static List<Point3d> PointList(Polyline acPoly)
        {
            var PoColl = new List<Point3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                try
                {
                    PoColl.Add(acPoly.GetPoint3dAt(i));
                }
                catch { }
            }

            return PoColl;
        }

        public static T Object<T>(ObjectId id)
        {
            try
            {
                using (var t = AC.Doc.Database.TransactionManager.StartTransaction())
                {
                    var obj = t.GetObject(id, OpenMode.ForWrite);

                    if (obj is T) return (T)Convert.ChangeType(obj, typeof(T));
                }
            }
            catch (System.Exception)
            {
                //msg.show(ex.Message);
            }

            return default(T);
        }
        public static T Object<T>(long? handle)
        {
            return Object<T>(to.ObjectId(handle));
        }

        public static List<T> Objects<T>(List<ObjectId> ids)
        {
            using (var t = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    return (from id in ids
                            let obj = t.GetObject(id, OpenMode.ForWrite)
                            where obj is T
                            select (T)Convert.ChangeType(obj, typeof(T))).ToList();
                }
                catch (System.Exception)
                {
                    return default(List<T>);
                }
            }
        }
        public static List<T> Objects<T>(List<long> handles)
        {
            return Objects<T>(handles.Select(handle => to.ObjectId(handle)).ToList());
        }
        public static List<T> Objects<T>(SelectionSet set) where T : new()
        {
            if (set == null) return default(List<T>);

            using (Transaction t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                return (from id in set.GetObjectIds()
                        let obj = t.GetObject(id, OpenMode.ForWrite)
                        where obj is T
                        select (T)Convert.ChangeType(obj, typeof(T))).ToList();
            }
        }


        public static List<Entity> Entities(SelectionSet acSSet, Type ty)
        {
            var Return = new List<Entity>();

            using (Transaction T = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                var acEnts = from id in acSSet.GetObjectIds()
                             let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                             where acEnt.GetType() == ty
                             select acEnt;

                if (acEnts.Any()) Return.AddRange(acEnts);
            }

            return Return;
        }
        public static Entity Entity(ObjectId id)
        {
            using (var t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                var entity = (Entity)t.GetObject(id, OpenMode.ForWrite);

                return entity;
            }
        }
        public static List<Entity> Entities(List<ObjectId> ids)
        {
            var Return = new List<Entity>();

            using (var t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                ids.Where(x => !x.IsErased).ToList().ForEach(id =>
                {
                    var entity = (Entity)t.GetObject(id, OpenMode.ForWrite);

                    Return.Add(entity);
                });
            }

            return Return;
        }


        public static List<T> GetAllObject<T>()
        {
            var R = new List<T>();

            using (Transaction t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.Doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acEnts = from id in BTR.Cast<ObjectId>().ToList()
                             let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                             where acEnt is T
                             select (T)Convert.ChangeType(acEnt, typeof(T));

                if (acEnts.Any()) R.AddRange(acEnts);
            }

            return R;
        }
        public static List<LineSegment3d> LineSegment3Ds(Polyline acPoly)
        {
            var R = new List<LineSegment3d>();

            var N = acPoly.Closed ? acPoly.NumberOfVertices : acPoly.NumberOfVertices - 1;

            for (int i = 0; i < N; i++)
            {
                try
                {
                    var l = acPoly.GetLineSegmentAt(i);

                    R.Add(l);
                }
                catch { }
            }

            return R;
        }

        public static Point3d IntersectedPoint(Line line1, Line line2)
        {
            var Return = new Point3d();

            var pts3D = new Point3dCollection();

            line1.IntersectWith(line2, Intersect.OnBothOperands, pts3D, IntPtr.Zero, IntPtr.Zero);
       
            for (int i = 0; i < pts3D.Count; i++)
            {
                Return = pts3D[i];
            }

            return Return;
        }
        public static Point3dCollection IntersectedPoint3dCollection(Line line1, Line line2)
        {
            var Return = new Point3dCollection();

            line1.IntersectWith(line2, Intersect.OnBothOperands, Return, IntPtr.Zero, IntPtr.Zero);


            return Return;
        }
        public static List<Point3d> IntersectedPoints(Polyline acPoly, Curve curve)
        {
            var Return = new List<Point3d>();

            var pts3D = new Point3dCollection();

            acPoly.IntersectWith(curve, Intersect.OnBothOperands, pts3D, IntPtr.Zero, IntPtr.Zero);

            for (int i = 0; i < pts3D.Count; i++)
            {
                Return.Add(pts3D[i]);
            }

            return Return;
        }
        public static List<Polyline> IntersectedPolylines(List<Polyline> Polys, Polyline Poly)
        {
            List<Polyline> Return = new List<Polyline>();

            Polys.ForEach(acPoly =>
            {
                var pts3D = new Point3dCollection();

                acPoly.IntersectWith(Poly, Intersect.OnBothOperands, pts3D, IntPtr.Zero, IntPtr.Zero);

                if (pts3D.Count > 1)
                {
                    Return.Add(acPoly);
                }
            });

            return Return;
        }
        public static List<Polyline> IntersectedPolylines(Polyline polyline, ObjectId layerid)
        {
            var A = Get.Objects<Polyline>(AC.Editor.SelectCrossingPolygon(to.Point3dCollection(polyline)).Value)
                       .Where(x => x.LayerId == layerid);

            return A.Any() ? A.ToList() : null;
        }
        //public static Room IntersectedRoom(Polyline pl1, Room room)
        //{
        //    var pts3D = new Point3dCollection();

        //    var pl2 = Get.Object<Polyline>(room.Handle);

        //    pl1.IntersectWith(pl2, Intersect.OnBothOperands, pts3D, IntPtr.Zero, IntPtr.Zero);

        //    return pts3D.Count > 1 ? room : null;
        //}

        public static bool IsDouble(string s)
        {
            double A = 0;

            return double.TryParse(s, out A);
        }
        public static bool IsInt(string s)
        {
            int A = 0;

            return int.TryParse(s, out A);
        }

        public static ObjectId ObjectID(Handle handle)
        {
            var R = new ObjectId();

            AC.Doc.Database.TryGetObjectId(handle, out R);

            return R;
        }
        public static List<ObjectId> ObjectIDs(List<Handle> handles)
        {
            var R = new List<ObjectId>();

            handles.ForEach(handle =>
            {
                var ObjID = ObjectID(handle);

                if (ObjID != ObjectId.Null) R.Add(ObjID);
            });

            return R;
        }

        public static ObjectId ObjectID(long? handle)
        {
            var R = new ObjectId();

            AC.Doc.Database.TryGetObjectId(new Handle(Convert.ToInt64(handle)), out R);

            return R;
        }
        public static List<ObjectId> ObjectIDs(List<long?> handles)
        {
            var R = new List<ObjectId>();

            handles.ForEach(handle =>
            {
                var ObjID = ObjectID(handle);

                if (ObjID != ObjectId.Null) R.Add(ObjID);
            });

            return R;
        }

        public static Handle ObjHandle(ObjectId objId)
        {
            return Object<Entity>(objId).Handle;
        }
        public static List<Handle> ObjHandles(List<ObjectId> objIds)
        {
            var R = new List<Handle>();

            objIds.ForEach(objId =>
            {
                if (!objId.IsErased)
                {
                    if (objId != ObjectId.Null)
                    {
                        var E = Entity(objId);

                        if (E != null)
                        {
                            R.Add(E.Handle);
                        }
                    }
                }

            });

            return R;
        }

        public static bool IsExist(ObjectId id)
        {
            using (Transaction t = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                var A = t.GetObject(id, OpenMode.ForRead);

                return A == null ? false : true;
            }
        }

        public IList<ObjectId> GetIdsByType()
        {
            Func<Type, RXClass> getClass = RXObject.GetClass;

            // You can set this anywhere
            var acceptableTypes = new HashSet<RXClass>
            {
                getClass(typeof(Polyline)),
                getClass(typeof (Polyline2d)),
                getClass(typeof (Polyline3d))
            };

            using (var t = AC.Doc.TransactionManager.StartOpenCloseTransaction())
            {
                var modelspace = t.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(AC.Doc.Database), OpenMode.ForRead) as BlockTableRecord;

                var polylineIds = (from id in modelspace.Cast<ObjectId>()
                                   where acceptableTypes.Contains(id.ObjectClass)
                                   select id).ToList();

                t.Commit();

                return polylineIds;
            }
        }

        public static string RemoveNumber(string roomname)
        {
            return Regex.Replace(roomname, @"[\d-]", string.Empty);
        }

        public static T GetEnumVal<T>(string val)
        {
            var A = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            var B = A.Where(a => a.ToString() == val);

            return B.Any() ? (T)Convert.ChangeType(B.First(), typeof(T)) : (T)Convert.ChangeType(A.First(), typeof(T));
        }

        public static Point3d GetNearP(LineSegment3d line, Point3d point)
        {
            var sp = line.StartPoint;
            var ep = line.EndPoint;

            return sp.DistanceTo(point) < ep.DistanceTo(point) ? sp : ep;
        }
        public static Point3d GetNearP(Polyline polyline, LineSegment3d line)
        {
            var sp = line.StartPoint;
            var ep = line.EndPoint;

            var csp = polyline.GetClosestPointTo(sp, true);
            var cep = polyline.GetClosestPointTo(sp, true);

            return sp.DistanceTo(csp) < ep.DistanceTo(cep) ? sp : ep;
        }
        public static Point3d GetFurP(LineSegment3d line, Point3d point)
        {
            var sp = line.StartPoint;
            var ep = line.EndPoint;

            return sp.DistanceTo(point) > ep.DistanceTo(point) ? sp : ep;
        }

        public static Point3d GetFurP(Polyline polyline, LineSegment3d line)
        {
            var sp = line.StartPoint;
            var ep = line.EndPoint;

            var csp = polyline.GetClosestPointTo(sp, true);
            var cep = polyline.GetClosestPointTo(sp, true);

            return sp.DistanceTo(csp) > ep.DistanceTo(cep) ? sp : ep;
        }
        public static LineSegment3d GetParallel(Polyline polyline, LineSegment3d line)
        {
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                var l = polyline.GetLineSegmentAt(i);

                if (l.IsParallelTo(line) && l.GetDistanceTo(line) > 1) return l;
            }

            return null;
        }

        //public static double Width(Room room)
        //{
        //    var polyline = Get.Object<Polyline>(room.Handle);
        //    var bb = polyline.Bounds.Value;
        //    var min = bb.MinPoint;
        //    var max = bb.MaxPoint;

        //    return max.X - min.X;
        //}
        public static double Width(Polyline acPoly)
        {
            var max = acPoly.Bounds.Value.MaxPoint;
            var min = acPoly.Bounds.Value.MinPoint;

            return max.X - min.X;
        }
        public static double Width(BlockReference BR)
        {
            var A = BR.Bounds.Value;
            var Min = A.MinPoint;
            var Max = A.MaxPoint;
            double H = Max.Y - Min.Y;
            double W = Max.X - Min.X;

            return H > W ? Math.Round(H) : Math.Round(W);
        }

        //public static double Height(Room room)
        //{
        //    var polyline = Get.Object<Polyline>(room.Handle);
        //    var bb = polyline.Bounds.Value;
        //    var min = bb.MinPoint;
        //    var max = bb.MaxPoint;

        //    return max.Y - min.Y;
        //}
        public static double Height(Polyline acPoly)
        {
            var max = acPoly.Bounds.Value.MaxPoint;
            var min = acPoly.Bounds.Value.MinPoint;

            return max.Y - min.Y;
        }

        public static string BlockName(BlockReference BR)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                BR = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

                BlockTableRecord BTR = null;

                if (BR.IsDynamicBlock)
                {
                    BTR = t.GetObject(BR.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                }
                else
                {
                    BTR = t.GetObject(BR.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                }

                return BTR == null ? "" : BTR.Name;
            }
        }
        public static string BlockNameTest(BlockReference BR)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var sb = new StringBuilder();

                BR = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

                foreach (DynamicBlockReferenceProperty p in BR.DynamicBlockReferencePropertyCollection)
                {
                    sb.AppendLine(string.Format("{0} : {1}", p.PropertyName, p.Value));
                }

                msg.show(sb);

                t.Commit();
            }

            return string.Empty;
        }


        //public static string BlockType(string blockname)
        //{
        //    if (Utils.window.Default.Where(x => x.Block.Equals(blockname)).Any())
        //    {
        //        return "WINDOW";
        //    }
        //    else if (Utils.door.Default.Where(x => x.Block.Equals(blockname)).Any())
        //    {
        //        return "DOOR";
        //    }
        //    else if (Utils.furniture.Default.Where(x => x.block.Equals(blockname)).Any())
        //    {
        //        return "FURNITURE";
        //    }
        //    else if (Utils.bath.Default.Where(x => x.block.Equals(blockname)).Any())
        //    {
        //        return "BATH";
        //    }

        //    return string.Empty;
        //}

        public static Point3d MinPoint(List<Polyline> polylines)
        {


            var minX = (from a in polylines
                        let min = a.Bounds.Value.MinPoint.X
                        orderby min
                        select min).First();
            var minY = (from a in polylines
                        let min = a.Bounds.Value.MinPoint.Y
                        orderby min
                        select min).First();

            return new Point3d(minX, minY, 0);
        }
        public static Point3d MaxPoint(List<Polyline> polylines)
        {
            var maxX = (from a in polylines
                        let max = a.Bounds.Value.MaxPoint.X
                        orderby max descending
                        select max).First();
            var maxY = (from a in polylines
                        let max = a.Bounds.Value.MaxPoint.Y
                        orderby max descending
                        select max).First();

            return new Point3d(maxX, maxY, 0);
        }

        public static string DimensionText(BlockReference BR)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                BR = t.GetObject(BR.Id, OpenMode.ForRead) as BlockReference;

                var A = new DBObjectCollection();

                BR.Explode(A);

                var B = A.Cast<Entity>().ToList();

                if (B.Where(x => x.GetType() == typeof(DBText)).Any())
                {
                    var C = B.Where(x => x.GetType() == typeof(DBText));
                    if (!C.Any()) return string.Empty;

                    var dbtext = C.First() as DBText;

                    return dbtext.TextString;
                }
                else
                {
                    var C = B.Where(x => x.GetType().BaseType == typeof(Dimension));
                    if (!C.Any()) return string.Empty;

                    var dim = C.First() as Dimension;
                    var width = dim.Measurement;

                    return dim.DimensionText.Replace("<>", to.Str(width));
                }
            }
        }

        public static List<ObjectId> InsidePolyline(Polyline pline)
        {
            var Return = new List<ObjectId>();

            using (Transaction t = AC.Doc.TransactionManager.StartTransaction())
            {
                var PoColl = to.Point3dCollection(pline);

                var acCPSSet = AC.Editor.SelectCrossingPolygon(PoColl).Value;

                #region 텍스트
                var acEnts1 = from id in acCPSSet.GetObjectIds()
                              let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                              where acEnt.GetType() == typeof(DBText)
                              select acEnt.ObjectId;
                if (acEnts1.Any()) Return.AddRange(acEnts1);
                #endregion

                #region 치수선
                var acEnts2 = from id in acCPSSet.GetObjectIds()
                              let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                              where acEnt.GetType().BaseType == typeof(Dimension)
                              let dim = acEnt as Dimension
                              where point.IsInsidePolyline(pline, dim.TextPosition, false)
                              select acEnt.ObjectId;
                if (acEnts2.Any()) Return.AddRange(acEnts2);
                #endregion
            }

            return Return;
        }

        public static DateTime GetMSDateTime()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = request.GetResponse();
            var now = response.Headers["date"];
            response.Close();

            return DateTime.ParseExact(now, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);

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
