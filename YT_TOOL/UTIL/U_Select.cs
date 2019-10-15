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
    public class Select
    {
        /// <summary>
        /// 단일 객체 선택
        /// </summary>
        /// <returns></returns>
        public static SelectionSet SingleObj()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }
        /// <summary>
        /// 단일 객체 선택
        /// </summary>
        /// <returns></returns>
        public static SelectionSet SingleObj(string Msg)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }

        /// <summary>
        /// 단일 객체 선택
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static SelectionSet SingleObj(SelectionFilter filter)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, filter);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }
        /// <summary>
        /// 복수 객체 선택
        /// </summary>
        /// <returns></returns>
        public static SelectionSet MultiObjs()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet MultiObjs(string Msg)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPSR.Value;
        }
        /// <summary>
        /// 복수 객체 선택
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static SelectionSet MultiObjs(SelectionFilter filter)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, filter);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }
        public static SelectionSet MultiObjs(string Msg, PromptSelectionOptions Opt)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            Opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(Opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPSR.Value;
        }

        public static string GetKeyword(PromptKeywordOptions Opt)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            var acPR = AC.Editor.GetKeywords(Opt);

            if (acPR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");
                return null;
            }

            return acPR.StringResult;
        }

        public static SelectionSet MultiObjs(string Msg, SelectionFilter filter)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = Msg;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, filter);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return acPSR.Value;
        }

        public static SelectionSet IntersectedObjs(Point3d P1, Point3d P2)
        {
            var PoColl = new Point3dCollection();

            PoColl.Add(P1);
            PoColl.Add(P2);
            PoColl.Add(Utils.PointUtil.Move(P2, 0, 1));
            PoColl.Add(Utils.PointUtil.Move(P1, 0, 1));

            var PSR = AC.Editor.SelectCrossingPolygon(PoColl);

            if (PSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n취소");

                return null;
            }

            return PSR.Value;
        }

        /// <summary>
        /// 객체를 선택된 상태로 만들기
        /// </summary>
        /// <param name="acPSR"></param>
        public static void SetSelectedState(PromptSelectionResult acPSR)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                if (acPSR.Status == PromptStatus.OK)
                {
                    AC.Editor.SetImpliedSelection(acPSR.Value.GetObjectIds());
                }

                T.Commit();
            }
            #endregion
        }

    }
}
