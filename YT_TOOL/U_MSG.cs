using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.Geometry;

namespace YT_CAD_TOOL
{
    public class MSGUtil
    {
        public static void Show(string msg)
        {
            MessageBox.Show(msg, "알림");
        }

        public static void Show(Point3dCollection array)
        {
            var sb = new StringBuilder();

            array.Cast<Point3d>().ToList().ForEach(p => sb.AppendLine(p.ToString()));

            Show(sb.ToString());
        }

        public static void Show(List<Point3d> array)
        {
            var sb = new StringBuilder();

            array.ForEach(p => sb.AppendLine(p.ToString()));

            Show(sb.ToString());
        }

        public static void Done(string msg)
        {
            MessageBox.Show(string.Format("{0} 완료", msg), "알림");
        }
    }
}
