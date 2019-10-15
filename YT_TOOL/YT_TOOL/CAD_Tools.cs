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
    public class CAD_Tools
    {
        #region 속성
        List<Entity> acXEnt;
        #endregion

        #region 생성자
        public CAD_Tools()
        {
            acXEnt = new List<Entity>();
        }
        #endregion

        #region PUBLIC 메서드

        /// <summary>
        /// 선택한 객체 타입 출력
        /// </summary>
        public void PrintObjType()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    #region 선택
                    var acSSet = SelectObjs();

                    var acEnts = from id in acSSet.GetObjectIds()
                                 let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                 select acEnt;

                    if (!acEnts.Any())
                        return;
                    #endregion

                    #region 출력
                    string s = "";
                    acEnts.ToList().ForEach(ent =>
                    {
                        s += "Entity: " + ent.ToString() + "\n";
                        s += "ID:" + ent.Id.ToString() + "\n";
                        s += "ClassID:" + ent.ClassID.ToString() + "\n";
                        s += "ObjectId:" + ent.ObjectId.ToString() + "\n";
                        //s += "BlockId:" + ent.BlockId.ToString() + "\n";
                        //s += "HashCode:" + ent.GetHashCode().ToString() + "\n";
                        //s += "Guid:" + new Guid(ent.Id.ToString()).ToString() + "\n";
                    });

                    MessageBox.Show(s);
                    #endregion
                }
            }

            AC.Editor.PostCommandPrompt();
        }

        /// <summary>
        /// 블럭 뿌수기!!!
        /// </summary>
        public void ExplodeBlocks()
        {
            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    #region 선택
                    var acSSet = SelectBlocks();

                    var acEnts = from id in acSSet.GetObjectIds()
                                 let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                 select acEnt;

                    if (!acEnts.Any())
                        return;
                    #endregion

                    #region 분해
                    acEnts.ToList().ForEach(ent =>
                    {
                        BreakBlock(ent);
                    });

                    AC.Editor.WriteMessage("\n" + acEnts.Count().ToString() + "개의 블럭을 분해함" + "\n" + acXEnt.Count.ToString() + "개의 객체 생성");
                    #endregion

                    #region 선택 객체 지우기
                    acEnts.ToList().ForEach(ent =>
                    {
                        ent.Erase(true);
                    });
                    #endregion

                    #region 입력
                    foreach (Entity ent in acXEnt)
                    {
                        BTR.AppendEntity(ent);
                        T.AddNewlyCreatedDBObject(ent, true);
                    }
                    #endregion

                    T.Commit();
                }
            }

            AC.Editor.WriteMessage("\n선택된 Block분해가 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }
        #endregion

        #region PRIVATE 메서드

        private SelectionSet SelectObjs()
        {
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            //SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionResult acPSR = acDocEd.GetSelection();

            acDocEd.WriteMessage("Block 객체 선택");

            if (acPSR.Status != PromptStatus.OK)
            {
                acDocEd.WriteMessage("\nError in getting selections");

                return acPSR.Value;
            }

            AC.Doc.GetAcadDocument();

            return acPSR.Value;
        }

        private SelectionSet SelectBlocks()
        {
            RXClass Rxclass = RXClass.GetClass(typeof(BlockReference));

            TypedValue[] tvs = new TypedValue[]
            {
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                        new TypedValue(Convert.ToInt32(DxfCode.Start), "INSERT"),   // 블럭
                        new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionResult acPSR = AC.Editor.GetSelection(oSf);

            AC.Editor.WriteMessage("Block 객체 선택");

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\nError in getting selections");
                return acPSR.Value;
            }

            AC.Doc.GetAcadDocument();

            return acPSR.Value;
        }


        private void BreakBlock(Entity acEnt)
        {
            var BreakedEntity = new List<Entity>();

            var entities = BreakEntity(acEnt);

            #region 블럭 없어질때 까지 분해
            do
            {
                BreakedEntity = entities;

                if (BreakedEntity.Count != 0)
                {
                    entities = new List<Entity>();

                    BreakedEntity.ForEach(a =>
                    {
                        var breaked = BreakEntity(a);

                        entities.AddRange(breaked);

                    });
                }
            } while (entities.Count != 0);
            #endregion
        }

        /// <summary>
        /// Entity 분해하기
        /// </summary>
        /// <param name="acEnt"></param>
        /// <returns></returns>
        private List<Entity> BreakEntity(Entity acEnt)
        {
            var acDBObjColl = new DBObjectCollection();

            acEnt.Explode(acDBObjColl);

            #region 분해된 객체
            var acObjs = from a in acDBObjColl.Cast<Entity>().ToList()
                         where a.GetRXClass().DxfName != RXClass.GetClass(typeof(BlockReference)).DxfName
                         select a;

            if (acObjs.Any()) acXEnt.AddRange(acObjs);
            #endregion

            #region 분해 안된 Block 안의 또다른 Block들
            var BlockObjs = from a in acDBObjColl.Cast<Entity>().ToList()
                            where a.GetRXClass().DxfName == RXClass.GetClass(typeof(BlockReference)).DxfName
                            select a;
            #endregion

            return BlockObjs.Any() ? BlockObjs.ToList() : new List<Entity>();
        }
        #endregion
    }
}
