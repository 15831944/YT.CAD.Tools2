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

namespace YT_CAD_TOOL.Cad
{
    public class Wall
    {
        public static void Create(Line CenterLine, double Width)
        {
            #region PRE
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            DocumentLock acDL = acDoc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor acDocEd = acDoc.Editor;
            #endregion

            #region T
            using (Transaction T = acDb.TransactionManager.StartTransaction())
            {
                var BT = (BlockTable)T.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                var BTR = (BlockTableRecord)T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var ObjColl1 = CenterLine.GetOffsetCurves(Width / 2);
                var ObjColl2 = CenterLine.GetOffsetCurves(-Width / 2);

                foreach (Entity obj in ObjColl1)
                {
                    BTR.AppendEntity(obj);
                    T.AddNewlyCreatedDBObject(obj, true);
                }
                foreach (Entity obj in ObjColl2)
                {
                    BTR.AppendEntity(obj);
                    T.AddNewlyCreatedDBObject(obj, true);
                }

                T.Commit();
            }
            #endregion
        }
        public static void Create(Polyline CenterLine, double Width, Color C)
        {
            #region PRE
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            DocumentLock acDL = acDoc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor acDocEd = acDoc.Editor;
            #endregion

            #region T
            using (Transaction T = acDb.TransactionManager.StartTransaction())
            {
                var BT = (BlockTable)T.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                var BTR = (BlockTableRecord)T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var ObjColl1 = CenterLine.GetOffsetCurves(Width / 2);
                var ObjColl2 = CenterLine.GetOffsetCurves(-Width / 2);

                foreach (Entity obj in ObjColl1)
                {
                    obj.Color = C;
                    BTR.AppendEntity(obj);
                    T.AddNewlyCreatedDBObject(obj, true);
                }
                foreach (Entity obj in ObjColl2)
                {
                    obj.Color = C;
                    BTR.AppendEntity(obj);
                    T.AddNewlyCreatedDBObject(obj, true);
                }

                T.Commit();
            }
            #endregion
        }
        public static void Create(Polyline CenterLine, double Width, ObjectId LayerId)
        {
            #region PRE
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            DocumentLock acDL = acDoc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor acDocEd = acDoc.Editor;
            #endregion

            #region T
            using (Transaction T = acDb.TransactionManager.StartTransaction())
            {
                var BT = (BlockTable)T.GetObject(acDb.BlockTableId, OpenMode.ForRead);
                var BTR = (BlockTableRecord)T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var ObjColl1 = CenterLine.GetOffsetCurves(Width / 2);
                var ObjColl2 = CenterLine.GetOffsetCurves(-Width / 2);

                foreach (Entity obj in ObjColl1)
                {
                    obj.LayerId = LayerId;
                    BTR.AppendEntity(obj);
                    T.AddNewlyCreatedDBObject(obj, true);
                }
                foreach (Entity obj in ObjColl2)
                {
                    obj.LayerId = LayerId;
                    BTR.AppendEntity(obj);
                    T.AddNewlyCreatedDBObject(obj, true);
                }

                T.Commit();
            }
            #endregion
        }



        public static Polyline Offset(Polyline PL, double D)
        {
            #region PRE
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor ED = Doc.Editor;
            #endregion

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                PL.GetOffsetCurves(D);

                BTR.AppendEntity(PL);
                T.AddNewlyCreatedDBObject(PL, true);
                T.Commit();
            }

            return PL;
        }
        public static Circle Offset(Circle C, double D)
        {
            #region PRE
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor ED = Doc.Editor;
            #endregion

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                C.GetOffsetCurves(D);

                BTR.AppendEntity(C);
                T.AddNewlyCreatedDBObject(C, true);
                T.Commit();
            }

            return C;
        }
        public static Line Offset(Line L, double D)
        {
            #region PRE
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor ED = Doc.Editor;
            #endregion

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                L.GetOffsetCurves(D);

                BTR.AppendEntity(L);
                T.AddNewlyCreatedDBObject(L, true);
                T.Commit();
            }

            return L;
        }
        public static Line CreateLine(Point3d SP, Point3d EP)
        {
            #region PRE
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;
            DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor ED = Doc.Editor;
            #endregion

            var Return = new Line();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return = new Line(SP, EP);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
    }
}
