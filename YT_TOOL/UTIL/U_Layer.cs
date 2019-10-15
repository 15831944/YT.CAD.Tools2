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
    public class Layer
    {
        /// <summary>
        /// 레이어 생성
        /// </summary>
        /// <param name="LayerName"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static ObjectId Create(string LayerName, Color C)
        {
            ObjectId Return = ObjectId.Null;

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(LayerName))
                {
                    Return = acLT[LayerName];

                    var acLTR = T.GetObject(Return, OpenMode.ForWrite) as LayerTableRecord;

                    if (acLTR.Color != C) acLTR.Color = C;

                    //acLT.UpgradeOpen();
                }
                else
                {
                    using (var acLTR = new LayerTableRecord())
                    {
                        acLTR.Name = LayerName;
                        acLTR.Color = C;

                        acLT.UpgradeOpen();

                        acLT.Add(acLTR);
                        T.AddNewlyCreatedDBObject(acLTR, true);
                    }
                }

                Return = acLT[LayerName];

                T.Commit();
            }

            return Return;
        }

        /// <summary>
        /// 모든 레이어 ID 받아오기
        /// </summary>
        /// <returns></returns>
        public static List<ObjectId> GetAllIDs()
        {
            var Return = new List<ObjectId>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                var temp = (from id in acLT.Cast<ObjectId>().ToList()
                            let acLTR = T.GetObject(id, OpenMode.ForRead) as LayerTableRecord
                            orderby acLTR.Name
                            select acLTR.ObjectId);

                if (temp.Any()) Return.AddRange(temp);
            }

            return Return;
        }
        /// <summary>
        /// 레이어 ID 받아오기
        /// </summary>
        /// <returns></returns>
        public static ObjectId GetID(string LayerName)
        {
            var Return = ObjectId.Null;

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(LayerName))
                {
                    Return = acLT[LayerName];
                }
            }

            return Return;
        }
        public static string GetName(ObjectId id)
        {
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                var A = T.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;
                return A.Name;
            }
        }

        /// <summary>
        /// 모든 레이어 이름 받아오기
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllNames()
        {
            var Return = new List<string>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                var temp = (from id in acLT.Cast<ObjectId>().ToList()
                            let acLTR = T.GetObject(id, OpenMode.ForRead) as LayerTableRecord
                            orderby acLTR.Name
                            select acLTR.Name);

                if (temp.Any()) Return.AddRange(temp);
            }

            return Return;
        }

        /// <summary>
        /// 레이어 끄기
        /// </summary>
        /// <param name="layerName">레이어 이름</param>
        /// <param name="B">true = 끄기, false = 켜기</param>
        public static void TurnOff(string layerName, bool B)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(layerName) == true)
                {
                    LayerTableRecord acLTR = T.GetObject(acLT[layerName], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = B;

                        AC.Editor.WriteMessage(layerName + " was truned off");

                        T.Commit();
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(layerName + " could not be turned off");
                    }
                    //acDocEd.PostCommandPrompt();
                }
            }
            #endregion
        }
        public static void TurnOff(ObjectId ObjId, bool B)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(ObjId) == true)
                {
                    LayerTableRecord acLTR = T.GetObject(ObjId, OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = B;

                        AC.Editor.WriteMessage(acLTR.Name + " was truned off");

                        T.Commit();
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(acLTR.Name + " could not be turned off");
                    }
                    AC.Editor.PostCommandPrompt();
                }
            }
            #endregion
        }
        public static void TurnOffAll(bool B)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId id in acLT)
                {
                    LayerTableRecord acLTR = T.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = B;
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(acLTR.Name + " could not be turned off");
                    }
                }

                AC.Editor.WriteMessage("all layers were truned off.");

                T.Commit();
            }
            AC.Editor.PostCommandPrompt();
            #endregion
        }

        /// <summary>
        /// 선택된 레이어빼고 모든 레이어 숨기기
        /// </summary>
        /// <param name="LayerName"></param>
        /// <param name="B">true = 선택된 레이어빼고 모두 숨기기, false = 선택된레이어만 숨기고 모두 표시</param>
        public static void ISO(string LayerName, bool B)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(LayerName) == true)
                {
                    foreach (ObjectId id in acLT)
                    {
                        LayerTableRecord acLTR = T.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;

                        acLTR.IsOff = acLTR.Name == LayerName ? !B : B;
                    }

                    AC.Editor.PostCommandPrompt();
                }

                T.Commit();
            }
            #endregion
        }
        /// <summary>
        /// 선택된 레이어빼고 모든 레이어 숨기기
        /// </summary>
        /// <param name="LayerName"></param>
        /// <param name="B">true = 선택된 레이어빼고 모두 숨기기, false = 선택된레이어만 숨기고 모두 표시</param>
        public static void ISO(List<string> LayerNames, bool B)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId id in acLT)
                {
                    LayerTableRecord acLTR = T.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;

                    acLTR.IsOff = LayerNames.Contains(acLTR.Name) ? !B : B;
                }

                AC.Editor.PostCommandPrompt();

                T.Commit();
            }
            #endregion
        }

        /// <summary>
        /// 레이어 잠그기
        /// </summary>
        /// <param name="layerName"></param>
        public static void Lock(string layerName)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(layerName) == true)
                {
                    LayerTableRecord acLTR = T.GetObject(acLT[layerName], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsLocked = true;

                        AC.Editor.WriteMessage(layerName + " was locked");

                        T.Commit();
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(layerName + " could not be locked");
                    }
                    AC.Editor.PostCommandPrompt();
                }
            }
            #endregion
        }



        /// <summary>
        /// 선택된 객체들이 정의된 레이어 이름 목록
        /// </summary>
        /// <param name="acSSet"></param>
        /// <returns></returns>
        public static List<string> Get_Selected_Obj_Layer_Names(SelectionSet acSSet)
        {
            var Return = new List<string>();

            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var acLayers = from id in acSSet.GetObjectIds()
                               let acEnt = T.GetObject(id, OpenMode.ForRead) as Entity
                               orderby acEnt.Layer
                               select acEnt.Layer;

                if (acLayers.Any())
                {
                    Return = acLayers.Distinct().ToList();
                }
            }
            #endregion

            return Return;
        }
        /// <summary>
        /// 선택된 객체들이 정의된 레이어 ID 목록
        /// </summary>
        /// <param name="acSSet"></param>
        /// <returns></returns>
        public static List<ObjectId> Get_Selected_Obj_Layer_IDs(SelectionSet acSSet)
        {
            var Return = new List<ObjectId>();

            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var acLayers = from id in acSSet.GetObjectIds()
                               let acEnt = T.GetObject(id, OpenMode.ForRead) as Entity
                               orderby acEnt.Layer
                               select acEnt.LayerId;

                if (acLayers.Any())
                {
                    Return = acLayers.Distinct().ToList();
                }
            }
            #endregion

            return Return;
        }


        public static void SetLayer(List<Entity> acEnts, string LayerName)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                acEnts.ForEach(acEnt =>
                {
                    var A = t.GetObject(acEnt.ObjectId, OpenMode.ForWrite) as Entity;

                    A.Layer = LayerName;
                });

                t.Commit();
            }
        }

        public static void SetLayer(Entity acEnt, string LayerName)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var A = t.GetObject(acEnt.ObjectId, OpenMode.ForWrite) as Entity;

                A.Layer = LayerName;

                t.Commit();
            }
        }
    }
}
