using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_CAD_TOOL
{
    public class to
    {
        public static string Str(object obj)
        {
            return obj == null || obj is DBNull ? "" : System.Convert.ToString(obj).Trim();
        }

        public static int Int(object obj)
        {
            if (obj == null || obj is DBNull || !obj.ToString().ToCharArray().Where(x => char.IsNumber(x)).Any()) return 0;

            return Convert.ToInt32(obj);
        }

        public static double Dbl(object obj)
        {
            if (obj == null) return 0;
            else if (obj is DBNull) return 0;
            //else if (!com.IsNumber(obj)) return 0;
            else if (string.IsNullOrEmpty(obj.ToString())) return 0;
            else return Convert.ToDouble(obj);

            //return obj == null || obj is DBNull || !com.IsNumber(obj) || string.IsNullOrEmpty(obj.ToString()) ? 0 : Convert.ToDouble(obj);
        }

        public static float Flt(object obj)
        {
            return obj == null || obj is DBNull ? 0 : (float)Dbl(obj);
        }

        public static Single Sgl(object obj)
        {
            return obj == null || obj is DBNull || string.IsNullOrEmpty(obj.ToString()) ? 0 : Convert.ToSingle(obj);
        }

        public static long Lng(object obj)
        {
            return obj == null || obj is DBNull ? 0 : (long)obj;
        }

        public static bool Bool(object obj)
        {
            return obj == null || obj is DBNull ? false : Convert.ToBoolean(obj);

        }

        //public static Color Colour(object edit)
        //{
        //    if (edit is null) return Color.FromArgb(255, 255, 255);
        //    if (!(edit is Color)) return Color.FromArgb(255, 255, 255);

        //    return (Color)edit;
        //}

        //public static DataTable Table<T>(IEnumerable<T> items)
        //{
        //    DataTable dt = new DataTable();

        //    dt.TableName = typeof(T).Name;

        //    #region 행
        //    var properties = typeof(T).GetProperties();

        //    for (int i = 0; i < properties.Length; i++)
        //    {
        //        var prop = properties[i];

        //        try
        //        {
        //            dt.Columns.Add(prop.Name, prop.PropertyType);
        //        }
        //        catch
        //        {
        //            dt.Columns.Add(prop.Name, typeof(string));
        //        }
        //    }
        //    #endregion

        //    if (items == null || items.Count() == 0) return dt;

        //    #region 열
        //    var A = items.ToArray<T>();

        //    for (int i = 0; i < A.Length; i++)
        //    {
        //        var item = A[i];

        //        if (item == null) continue;

        //        var r = dt.NewRow();

        //        for (int j = 0; j < properties.Length; j++)
        //        {
        //            var prop = properties[j];
        //            r[prop.Name] = prop.GetValue(item, null);
        //        }

        //        dt.Rows.Add(r);
        //    }
        //    #endregion

        //    return dt;
        //}

        //public static List<T> Generic<T>(DataTable table) where T : new()
        //{
        //    List<T> R = new List<T>();

        //    PropertyInfo[] properties = typeof(T).GetProperties();

        //    foreach (DataRow row in table.Rows)
        //    {
        //        T item = new T();
        //        foreach (var p in properties)
        //        {
        //            if (!table.Columns.Contains(p.Name)) continue;
        //            if (row[p.Name] is DBNull) continue;

        //            object val1 = row[p.Name];
        //            object val2 = DataType(val1, p.PropertyType);

        //            p.SetValue(item, val2, null);
        //        }

        //        R.Add(item);
        //    }

        //    return R;
        //}
        //public static List<T> Generic<T>(DataSet dataSet) where T : new()
        //{
        //    var sheet = TableMap.GetSheetNameByTableName(typeof(T).Name);

        //    if (dataSet.Tables[sheet] == null) return new List<T>();

        //    var table = dataSet.Tables[sheet];

        //    DBDataContext.Select<ColumnMap>().Where(x => x.TableName.Equals(typeof(T).Name)).ToList().ForEach(x =>
        //    {
        //        var c = table.Columns[x.ColumnName];
        //        if (c != null)
        //        {
        //            c.ColumnName = x.FieldName;
        //        }
        //    });

        //    return to.Generic<T>(table);
        //}

        //public static object DataType(object value, Type type)
        //{
        //    string typestring = type.IsGenericType ? type.GenericTypeArguments.First().Name : type.Name;

        //    switch (typestring)
        //    {
        //        case "String": return to.Str(value);
        //        case "Int32": return to.Int(value);
        //        case "Double": return to.Dbl(value);
        //        case "Single": return to.Sgl(value);
        //        case "Bool": return Convert.ToBoolean(value);
        //        case "Boolean": return to.Bool(value);
        //        case "Int64": return Convert.ToInt64(value);
        //    }

        //    return null;
        //}

        public static double Radian(double Degree)
        {
            return Degree * Math.PI / 180;
        }

        public static double Degree(double Radian)
        {
            return Radian * 180 / Math.PI;
        }

        public static ObjectId ObjectId(long? handle)
        {
            ObjectId id;

            AC.DB.TryGetObjectId(new Handle(Convert.ToInt64(handle)), out id);

            return id;
        }

        //public static List<T> List<T>(params T[] items)
        //{
        //    return items.ToList<T>();
        //}

        //public static string FormatString(int digit)
        //{
        //    var str = "0.";
        //    for (int i = 0; i < digit; i++) str += "0";

        //    return str;
        //}

        //public static T Round<T>(object val, int digit)
        //{
        //    return (T)Convert.ChangeType(Math.Round(to.Dbl(val), digit), typeof(T));
        //}
        //public static T Round<T>(object val)
        //{
        //    return (T)Convert.ChangeType(Math.Round(to.Dbl(val), current.Rounding), typeof(T));
        //}

        //public static T Meter<T>(object milimeter)
        //{
        //    return (T)Convert.ChangeType(to.Dbl(milimeter) / 1000, typeof(T));
        //}

        //public static T Meter2<T>(double milimeter)
        //{
        //    return (T)Convert.ChangeType(milimeter / 1000000, typeof(T));
        //}

        //public static T RoundMeter<T>(double millimeter)
        //{
        //    return to.Round<T>(millimeter / 1000, current.Rounding);
        //}
        //public static T RoundMeter2<T>(double millimeter)
        //{
        //    return to.Round<T>(millimeter / 1000000, current.Rounding);
        //}

        public static Point3dCollection Point3dCollection(Polyline polyline)
        {
            var PoColl = new Point3dCollection();

            #region A : 폴리곤 → 포인트
            var A = new List<Point3d>();

            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                try
                {
                    var a = polyline.GetPoint3dAt(i);

                    var x = Math.Round(a.X, 2);
                    var y = Math.Round(a.Y, 2);
                    var z = Math.Round(a.Z, 2);

                    A.Add(new Point3d(x, y, z));
                }
                catch { }
            }
            #endregion

            #region B : 중복제거 + 닫힌곡선
            var B = A.Distinct(new Point3dComparer()).ToList();

            if (!B.First().Equals(B.Last())) B.Add(B.First());
            #endregion

            #region C : 거리 0 제거
            var C = new Point3dCollection();
            for (int i = 0; i < B.Count - 1; i++)
            {
                if (B[i].DistanceTo(B[i + 1]) == 0) continue;

                C.Add(B[i]);
            }

            C.Add(B.Last());
            #endregion

            return C;
        }
        public static Point3dCollection Point3dCollection(Point3d min, Point3d max)
        {
            var Return = new Point3dCollection();

            double dx = max.X - min.X;
            double dy = max.Y - min.Y;

            return new Point3dCollection()
                {
                    new Point3d(min.X, min.Y, 0),
                    new Point3d(min.X + dx, min.Y, 0),
                    new Point3d(min.X + dx, min.Y + dy, 0),
                    new Point3d(min.X, min.Y + dy, 0),
                    new Point3d(min.X, min.Y, 0)
                };
        }

    }
}
