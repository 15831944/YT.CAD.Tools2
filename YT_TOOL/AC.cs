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

namespace YT_CAD_TOOL
{
    public class AC
    {
        #region 속성
        public static string ID { get; set; }
        public static string PW { get; set; }

        public static bool IsLogIn = false;

        public static Document Doc { get; set; }
        public static Database DB { get; set; }
        public static DocumentLock DL { get; set; }
        public static Editor Editor { get; set; }

        public static bool IsActivated = false;
        #endregion

        #region PUBLIC
        public static void Initialize_YT()
        {
            if (!IsActivated) Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;

            Doc = Application.DocumentManager.MdiActiveDocument;
            DB = Doc.Database;
            DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor = Doc.Editor;

            //Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
        }

        public static void Initialize()
        {
            if (!IsLogIn)
            {
                MessageBox.Show("시스템관리자에게 문의하세요", "로그인 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsActivated) Application.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;

            Doc = Application.DocumentManager.MdiActiveDocument;
            DB = Doc.Database;
            DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            Editor = Doc.Editor;
            //Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
        }

        private static void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (e.Document != null)
            {
                Doc = Application.DocumentManager.MdiActiveDocument;
                DB = Doc.Database;
                Editor = Doc.Editor;
                Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);

                IsActivated = true;
            }
        }
        #endregion
    }
}
