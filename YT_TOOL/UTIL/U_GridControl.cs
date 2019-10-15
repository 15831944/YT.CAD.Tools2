using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraGrid.Views.Grid;

namespace YT_CAD_TOOL.Utils
{
    public class GC
    {
        public static List<T> GetSelectedRows<T>(GridView view)
        {
            var R = new List<T>();

            view.GetSelectedRows().ToList().ForEach(r =>
            {
                R.Add((T)view.GetRow(r));
            });

            return R;
        }
    }
}
