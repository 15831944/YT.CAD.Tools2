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

namespace YT_CAD_TOOL.Utils
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
        //public static Vector3d CrossProduct(Point3d A, Point3d B, Point3d C)
        //{
        //    //var V1 = Vector.Get(B, A);
        //    //var V2 = Vector.Get(B, C);

        //    //return V1.CrossProduct(V2);

        //    var Ax = A.X;
        //    var Ay = A.Y;
        //    var Bx = B.X;
        //    var By = B.Y;
        //    var Cx = C.X;
        //    var Cy = C.Y;

        //    double BAx = Ax - Bx;
        //    double BAy = Ay - By;
        //    double BCx = Cx - Bx;
        //    double BCy = Cy - By;

        //    return  (BAx * BCy - BAy * BCx);
        //}

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



        public static Point3dCollection PointCollection(Polyline acPoly)
        {
            var PoColl = new Point3dCollection();

            var A = new List<Point3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                try
                {
                    var a = acPoly.GetPoint3dAt(i);

                    var x = Math.Round(a.X, 2);
                    var y = Math.Round(a.Y, 2);
                    var z = Math.Round(a.Z, 2);

                    A.Add(new Point3d(x, y, z));
                }
                catch { }
            }

            var B = A.Distinct(new Point3dComparer()).ToList();

            if (!B.First().Equals(B.Last()))
            {
                B.Add(B.First());
            }

            for (int i = 0; i < B.Count - 1; i++)
            {
                if (B[i].DistanceTo(B[i + 1]) == 0) continue;

                PoColl.Add(B[i]);
            }

            PoColl.Add(B.Last());

            return PoColl;
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

        public static List<Entity> Entity(SelectionSet acSSet, Type ty)
        {
            var Return = new List<Entity>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var acEnts = from id in acSSet.GetObjectIds()
                             let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                             where acEnt.GetType() == ty
                             select acEnt;

                if (acEnts.Any()) Return.AddRange(acEnts);
            }

            return Return;
        }

        public static T GetObject<T>(ObjectId id)
        {
            using (var t = AC.DB.TransactionManager.StartTransaction())
            {
                var entity = (Entity)t.GetObject(id, OpenMode.ForWrite);

                if (!(entity is T)) return default(T);

                return (T)Convert.ChangeType(entity, typeof(T));
            }

            return default(T);
        }

        public static Entity GetObject(ObjectId id)
        {
            using (var t = AC.DB.TransactionManager.StartTransaction())
            {
                var entity = (Entity)t.GetObject(id, OpenMode.ForWrite);

                return entity;
            }

            //return null;
        }

        public static List<T> GetEntity<T>(SelectionSet acSSet)
        {
            var R = new List<T>();

            if (acSSet == null) return R;

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var acEnts = from id in acSSet.GetObjectIds()
                             let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                             where acEnt is T
                             select (T)Convert.ChangeType(acEnt, typeof(T));

                if (acEnts.Any()) R.AddRange(acEnts);
            }

            return R;
        }

        public static List<T> GetAllObject<T>()
        {
            var R = new List<T>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acEnts = from id in BTR.Cast<ObjectId>().ToList()
                             let acEnt = t.GetObject(id, OpenMode.ForWrite) as Entity
                             where acEnt is T
                             select (T)Convert.ChangeType(acEnt, typeof(T));

                if (acEnts.Any()) R.AddRange(acEnts);
            }

            return R;
        }

        public static List<LineSegment3d> GetLineSegment3Ds(Polyline acPoly)
        {
            var R = new List<LineSegment3d>();

            for (int i = 0; i < acPoly.NumberOfVertices; i++)
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

            AC.DB.TryGetObjectId(handle, out R);

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

        public static Handle ObjHandle(ObjectId objId)
        {
            return GetObject<Entity>(objId).Handle;
        }
        public static List<Handle> ObjHandles(List<ObjectId> objIds)
        {
            var R = new List<Handle>();

            objIds.ForEach(objId =>
            {
                if (objId != ObjectId.Null)
                {
                    var E = GetObject(objId);

                    if (E != null)
                    {
                        R.Add(E.Handle);
                    }
                }
            });

            return R;
        }

        public static bool IsExist(ObjectId id)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var A = t.GetObject(id, OpenMode.ForRead);

                return A == null ? false : true;
            }
        }
    }
}
