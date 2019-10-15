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
    public class Block
    {
        public static BlockReference Create(string BlockName, Point3d P)
        {
            BlockReference Return;

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var bID = ObjectId.Null;

                if (!BT.Has(BlockName)) return null;

                bID = BT[BlockName];

                if (bID == ObjectId.Null) return null;

                var BR = new BlockReference(P, bID);

                var BTR = t.GetObject(AC.DB.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                Return = BR;

                BTR.AppendEntity(BR);
                t.AddNewlyCreatedDBObject(BR, true);

                t.Commit();
            }

            return Return;
        }

        public static BlockReference Create(string BlockName, Point3d P, Vector3d Vec)
        {
            BlockReference Return;

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var bID = ObjectId.Null;

                if (!BT.Has(BlockName)) return null;

                bID = BT[BlockName];

                if (bID == ObjectId.Null) return null;

                var BR = new BlockReference(P, bID);
                BR.Rotation = Utils.Vector.To2D(Vec).Angle;

                var BTR = t.GetObject(AC.DB.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                Return = BR;

                BTR.AppendEntity(BR);
                t.AddNewlyCreatedDBObject(BR, true);

                t.Commit();
            }

            return Return;
        }

    }
}
