using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_CAD_TOOL
{
    public class TrimSelectedPolylines
    {
        #region STATIC
        public static void Command()
        {
            // 선택
            var Lines = select.Objects<Line>();

            if (Lines == null)
                return;

            var eraseLineIDs = new List<ObjectId>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    Lines.ForEach(line =>
                    {
                        var L = t.GetObject(line.Id, OpenMode.ForWrite) as Line;

                        var sp = L.StartPoint;
                        L.StartPoint = new Point3d(sp.X, sp.Y, 0);

                        var ep = L.EndPoint;
                        L.EndPoint = new Point3d(ep.X, ep.Y, 0);
                    });
                }
                catch (Exception)
                {
                }
                finally
                {
                    t.Commit();
                }
            }

            // 교차
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    Lines.ForEach(line1 =>
                    {
                        var A = Lines.Where(x => !x.Id.Equals(line1.Id))
                                 .Select(x => Get.IntersectedPoint(line1, x))
                                 .Where(x => !x.IsEqualTo(line1.StartPoint) && !x.IsEqualTo(line1.EndPoint) && x != new Point3d());

                        if (A.Any())
                        {
                            var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                            var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                            var ptn3d = new Point3dCollection();

                            A.OrderBy(a => line1.StartPoint.DistanceTo(a)).ToList().ForEach(x => ptn3d.Add(x));

                            var B = line1.GetSplitCurves(ptn3d);

                            for (int i = 0; i < B.Count; i++)
                            {
                                var C = B[i] as Line;

                                var sp = C.StartPoint;
                                var ep = C.EndPoint;

                                var line = cad.CreateLine(sp, ep);

                                line.Layer = line1.Layer;
                            }

                            eraseLineIDs.Add(line1.Id);

                        }
                    });
                }
                catch (Exception)
                {
                }
                finally
                {
                    t.Commit();
                }
            }

            cad.Erases(eraseLineIDs);

            // 생성

            // 제거

        }
        #endregion

        #region PUBLIC

        #endregion

        #region PRIVATE

        #endregion
    }
}
