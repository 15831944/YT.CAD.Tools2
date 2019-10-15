using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;

namespace CADAUTO
{
    public class Test
    {
        #region 필드
        Document Doc;
        Database DB;
        DocumentLock DL;
        Editor ED;
        #endregion

        #region 생성자
        public Test()
        {
            Doc = Application.DocumentManager.MdiActiveDocument;
            DB = Doc.Database;
            DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            ED = Doc.Editor;
        }
        #endregion

        #region PUBLIC 메서드
        public void Create()
        {
            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point3d SP = new Point3d();
                Point3d EP = new Point3d(100, 0, 0);

                Line L = CAD.CreateLine(SP, EP);

                BTR.AppendEntity(L);
                T.AddNewlyCreatedDBObject(L, true);

                T.Commit();
            }
        }
        #endregion

        #region PRIVATE 메서드

        #endregion
    }
}
