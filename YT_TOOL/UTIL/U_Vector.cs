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
    public class Vector
    {
        /// <summary>
        /// 두 점을 지나는 벡터
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static Vector2d Get(Point2d P1, Point2d P2)
            => new LineSegment2d(P1, P2).Direction;
        /// <summary>
        /// 두 점을 지나는 벡터
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static Vector3d Get(Point3d P1, Point3d P2)
            => new LineSegment3d(P1, P2).Direction;
        //public static Vector3d Get(Point3d P1, Point3d P2)
        //{
        //    return new LineSegment3d(P1, P2).Direction;
        //}

        /// <summary>
        /// 폴리라인의 긴 축의 방향
        /// </summary>
        /// <param name="acPoly"></param>
        /// <returns></returns>
        public static Vector3d GetByLong(Polyline acPoly)
        {
            var linesegments = Utils.Get.GetLineSegment3Ds(acPoly);

            if (!linesegments.Any()) return new Vector3d();

            var longline = from a in linesegments
                           orderby a.Length descending
                           select a;

            return longline.Any() ? longline.First().Direction : new Vector3d();
        }
        /// <summary>
        /// 폴리라인의 짧은 축의 방향
        /// </summary>
        /// <param name="acPoly"></param>
        /// <returns></returns>
        public static Vector3d GetByShort(Polyline acPoly)
        {
            var linesegments = Utils.Get.GetLineSegment3Ds(acPoly);

            if (!linesegments.Any()) return new Vector3d();

            var longline = from a in linesegments
                           orderby a.Length ascending
                           select a;

            return longline.Any() ? longline.First().Direction : new Vector3d();
        }


        /// <summary>
        /// Curve2d의 벡터
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Vector2d Get(Curve2d C)
        {
            return Get(C.StartPoint, C.EndPoint);
        }
        /// <summary>
        /// Line의 벡터
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public static Vector3d Get(Line L)
        {
            return Get(L.StartPoint, L.EndPoint);
        }

        /// <summary>
        /// 2d 벡터를 3d 벡터로 변경
        /// </summary>
        /// <param name="V"></param>
        /// <returns></returns>
        public static Vector3d To3D(Vector2d V)
        {
            return new Vector3d(V.X, V.Y, 0);
        }
        /// <summary>
        /// 3d 벡터를 2d 벡터로 변경
        /// </summary>
        /// <param name="V"></param>
        /// <returns></returns>
        public static Vector2d To2D(Vector3d V)
        {
            return new Vector2d(V.X, V.Y);
        }


        public static double GetAngle(Vector3d V, Vector3d v)
        {
            return V.GetAngleTo(v);
        }



        public static Vector3d GetReflectedVector(Vector3d FV, Vector3d IV)
        {
            var Return = new Vector3d();

            var axisVector = FV.GetAngleTo(IV) > Math.PI ? Vector3d.ZAxis : -Vector3d.ZAxis;

            var VV = FV.CrossProduct(axisVector);   // Vertical Vector

            var V_Ang = Math.Atan2(VV.Y, VV.X);     // Vector Angle

            var N_Ang = V_Ang + new Random().Next(0, 90) / Math.PI;   // New Angle

            Return = new Vector3d(Math.Cos(N_Ang), Math.Sin(N_Ang), 0);

            return Return;
        }

        public static Vector3d RefineVector(Vector3d V, int d = 2)
        {
            return new Vector3d(Math.Round(V.X, d), Math.Round(V.Y, d), Math.Round(V.Z, d));
        }

    }
}
