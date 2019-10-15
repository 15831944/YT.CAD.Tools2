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
    public class point
    {     /// <summary>
          /// 클릭한 점의 좌표를 받아옵니다.
          /// </summary>
          /// <returns></returns>
        public static Point3d Pick()
        {
            PromptPointResult PPR = AC.Editor.GetPoint("Pick Point");

            return (PPR.Status == PromptStatus.OK) ? PPR.Value : new Point3d();
        }
        public static Point3d Pick(string msg)
        {
            PromptPointResult PPR = AC.Editor.GetPoint(msg);

            return (PPR.Status == PromptStatus.OK) ? PPR.Value : new Point3d();
        }
        public static Point3d Pick(PromptPointOptions opt)
        {
            PromptPointResult PPR = AC.Editor.GetPoint(opt);

            return (PPR.Status == PromptStatus.OK) ? PPR.Value : new Point3d();
        }
        public static Point3d LinkedPick(Point3d sp, string msg)
        {
            var opt = new PromptPointOptions("두번째 지점을 클릭 하시오");
            opt.UseBasePoint = true;
            opt.BasePoint = sp;
            opt.UseDashedLine = true;
            return Pick(opt);
        }

        /// <summary>
        /// 3D에서 2D로 변환
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        public static Point2d To2D(Point3d P)
        {
            return new Point2d(P.X, P.Y);
        }
        /// <summary>
        /// 2D에서 3D로 변환
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        public static Point3d To3D(Point2d P, double H = 0)
        {
            return new Point3d(P.X, P.Y, H);
        }

        /// <summary>
        /// 중심점 구하기
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static Point2d GetCenterP(Point2d P1, Point2d P2)
        {
            return new Point2d((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
        }
        /// <summary>
        /// 줌심점 구하기
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        public static Point3d GetCenterP(Point3d P1, Point3d P2)
        {
            return new Point3d((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2, 0);
        }
        /// <summary>
        /// 중심점 구하기
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Point2d GetCenterP(Curve2d C)
        {
            return GetCenterP(C.StartPoint, C.EndPoint);
        }
        /// <summary>
        /// 중심점 구하기
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Point3d GetCenterP(Curve3d C)
        {
            return GetCenterP(C.StartPoint, C.EndPoint);
        }
        /// <summary>
        /// 중심점 구하기
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public static Point3d GetCenterP(Line L)
        {
            return GetCenterP(L.StartPoint, L.EndPoint);
        }
        /// <summary>
        /// 폴리라인의 중심점 구하기
        /// </summary>
        /// <param name="acPolyline"></param>
        /// <returns></returns>
        public static Point3d GetCenterP(Polyline polyline)
        {
            if (polyline == null) return new Point3d();

            #region 폴리라인 내부 점 받기
            // 폴리라인 내부에 n간격으로 점을 찍음
            var min_P = polyline.Bounds.Value.MinPoint;
            var max_P = polyline.Bounds.Value.MaxPoint;
            //CAD.CreateCircle(min_P, 50);
            //CAD.CreateCircle(max_P, 50);

            int n = 20;     // 정밀도

            double dis_X = (max_P.X - min_P.X) / n;
            double dis_Y = (max_P.Y - min_P.Y) / n;

            var Points = new List<Point3d>();

            for (int i = 0; i < n + 1; i++)
            {
                for (int j = 0; j < n + 1; j++)
                {
                    var p = point.Move(min_P, dis_X * j, dis_Y * i);

                    if (IsInsidePolyline(polyline, p, false))
                    {
                        //CAD.CreateCircle(p, 20);
                        Points.Add(p);
                    }
                }
            }
            #endregion

            #region 무게중심
            // 폴리라인 내부의 점들끼리의 거리의 합이 가장 큰 점이 가장 가운데에 위치해 있다
            double value = double.MaxValue;
            Point3d cp = new Point3d();

            Points.ForEach(p1 =>
            {
                double val = 0;
                Points.ForEach(p2 =>
                {
                    if (!p1.IsEqualTo(p2))
                    {
                        val += p1.DistanceTo(p2) * p1.DistanceTo(p2);
                    }
                });

                if (value > val)
                {
                    value = val;
                    cp = p1;
                }
            });

            return cp;
            #endregion
        }
        public static Point3d GetCenterP(List<Polyline> acPolys)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            acPolys.ForEach(poly =>
            {
                for (int i = 0; i < poly.NumberOfVertices; i++)
                {
                    var l = poly.GetLineSegmentAt(i);
                    var p = l.StartPoint;
                    var x = p.X;
                    var y = p.Y;

                    if (minX > x) minX = x;
                    if (minY > y) minY = y;
                    if (maxX < x) maxX = x;
                    if (maxY < y) maxY = y;
                }
            });

            var lx = maxX - minX;
            var ly = maxY - minY;

            double d = 0.08;

            var Min = new Point3d(minX - lx * d, minY - ly * d, 0);
            var Max = new Point3d(maxX + lx * d, maxY + ly * d, 0);

            return GetCenterP(Min, Max);
        }
        public static Point3d GetCenterP(List<Point3d> points)
        {
            var x = points.Select(p => p.X).Sum() / points.Count;
            var y = points.Select(p => p.Y).Sum() / points.Count;
            var z = points.Select(p => p.Z).Sum() / points.Count;

            return new Point3d(x, y, z);
        }


        /// <summary>
        /// 점 이동 시키기
        /// </summary>
        /// <param name="P"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <returns></returns>
        public static Point3d Move(Point3d P, double W, double H)
        {
            return new Point3d(P.X + W, P.Y + H, P.Z);
        }
        /// <summary>
        /// 점 이동 시키기
        /// </summary>
        /// <param name="P"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public static Point3d Move(Point3d P, double X, double Y, double Z = 0)
        {
            return new Point3d(P.X + X, P.Y + Y, P.Z + Z);
        }
        /// <summary>
        /// 점 이동 시키기
        /// </summary>
        /// <param name="P"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <returns></returns>
        public static Point2d Move(Point2d P, double W, double H)
        {
            return new Point2d(P.X + W, P.Y + H);
        }
        /// <summary>
        /// 점 이동 시키기
        /// </summary>
        /// <param name="P"></param>
        /// <param name="V"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Point2d Move(Point2d P, Vector3d V, double d = 1)
        {
            return new Point2d(P.X + V.X * d, P.Y + V.Y * d);
        }
        /// <summary>
        /// 점 이동 시키기
        /// </summary>
        /// <param name="P"></param>
        /// <param name="V"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Point3d Move(Point3d P, Vector3d V, double d = 1)
        {
            return new Point3d(P.X + V.X * d, P.Y + V.Y * d, P.Z + V.Z * d);
        }

        /// <summary>
        /// 점이 폴리라인 내부에 있는지 판별
        /// </summary>
        /// <param name="acPolyline">폴리라인</param>
        /// <param name="P">점</param>
        /// <param name="B">경계선 포함</param>
        /// <returns></returns>
        public static bool IsInsidePolyline(Polyline acPolyline, Point3d P, bool B)
        {
            #region 경계선에 걸리는 경우
            if (B)
            {
                for (int i = 0; i < acPolyline.NumberOfVertices; i++)
                {
                    try
                    {
                        var l = acPolyline.GetLineSegmentAt(i);

                        if (l.IsOn(P)) return true;

                        //if (l.GetDistanceTo(P) < 0.00001) return true;
                    }
                    catch
                    {
                    }
                }
            }
            #endregion

            #region 점이 폴리라인 내부에 있는지 판별
            //Point3d Min_P = acPolyline.Bounds.Value.MinPoint;
            //Point3d Max_P = acPolyline.Bounds.Value.MaxPoint;
            Point3d Min_P = acPolyline.GetPoint3dAt(0);
            Point3d Max_P = acPolyline.GetPoint3dAt(acPolyline.NumberOfVertices - 1);

            double total_Ang = Get.Angle(Max_P, P, Min_P);

            for (int i = 0; i < acPolyline.NumberOfVertices - 1; i++)
            {
                var line = acPolyline.GetLineSegmentAt(i);
                var SP = line.StartPoint;
                var EP = line.EndPoint;

                total_Ang += Get.Angle(SP, P, EP);
            }

            return Math.Abs(total_Ang) > 0.0000000001;
            #endregion
        }

        public static bool InOnPolyline(Polyline acPoly, Point3d P)
        {
            for (int i = 0; i < acPoly.NumberOfVertices; i++)
            {
                try
                {
                    var l = acPoly.GetLineSegmentAt(i);

                    if (l.IsOn(P)) return true;
                }
                catch { }
            }

            return false;
        }

        public static bool IsSame(Point3d P1, Point3d P2, double d1 = 1, double d2 = 1)
        {
            return P1.IsEqualTo(P2, new Tolerance(d1, d2));
        }

    }
}
