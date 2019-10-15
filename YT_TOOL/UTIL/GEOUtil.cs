using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using ClipperLib;

namespace YT_CAD_TOOL
{
    public class GEOUtil
    {
        public static Point3dCollection GetUnionPolygons(List<Polyline> polylines)
        {
            var R = new Point3dCollection();

            var A = (from polyline in polylines
                    let B = (from p in Utils.Get.PointList(polyline) select new IntPoint(p.X, p.Y)).ToList()
                    select B).ToList();

            var C = new Clipper();

            for (int i = 0; i < A.Count; i++)
            {
                if (i == 0)
                {
                    C.AddPath(A[i], PolyType.ptSubject, true);
                }
                else
                {
                    C.AddPath(A[i], PolyType.ptClip, true);
                }
            }

            var U = new List<List<IntPoint>>();

            C.Execute(ClipType.ctUnion, U, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            if (!U.Any()) return R;

            U.First().ForEach(p => R.Add(new Point3d(p.X, p.Y, 0)));

            return R;
        }

    }
}
