#region .NET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public partial class U004 : Form
    {
        #region 필드
        Document acDoc;
        Database acDb;
        Editor acDocEd;
        DocumentLock acDL;
        #endregion

        #region 속성
        //public string Txt { get; set; }
        //public string T_Size { get; set; }
        //public string T_angle { get; set; }
        //public TextPosition T_Position { get; set; }
        //public bool rotate { get; set; }
        //public bool rotate2 { get; set; }
        //public bool rotate3 { get; set; }

        //public bool move { get; set; }
        //public bool shape { get; set; }
        #endregion

        #region 싱글턴
        public static U004 Instance { get; set; }
        #endregion

        #region 생성자 | 초기화
        public U004()
        {
            InitializeComponent();

            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDb = acDoc.Database;
            acDocEd = acDoc.Editor;

            //acDb.ObjectModified += AcDb_ObjectModified;

            //acDb.ObjectAppended -= new ObjectEventHandler(AcDb_ObjectAppended);

            //acDb.ObjectAppended += new ObjectEventHandler(AcDb_ObjectAppended);

            Instance = this;

            chk_event.CheckedChanged += Chk_event_CheckedChanged;
        }

        private void AcDb_ObjectModified(object sender, ObjectEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Chk_event_CheckedChanged(object sender, EventArgs e)
        {
            var checkEdit = sender as DevExpress.XtraEditors.CheckEdit;

            if (checkEdit.CheckState == CheckState.Checked)
            {
                acDb.ObjectAppended += new ObjectEventHandler(AcDb_ObjectAppended);
            }
            else
            {
                acDb.ObjectAppended -= new ObjectEventHandler(AcDb_ObjectAppended);
            }

            MessageBox.Show(checkEdit.Checked.ToString());
        }

        private void U004_Load(object sender, EventArgs e)
        {
            this.TopMost = chk_Top.Checked;
        }
        #endregion

        #region PUBLIC 메서드

        #endregion

        #region PRIVATE 메서드
        private void ReadData()
        {
            // 두번 클릭 필요없이 바로 선택가능하게 해줌
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            // 속성값 세팅
            Properties.Settings.Default.c_top4 = chk_Top.Checked;
            Properties.Settings.Default.Save();
        }
        #endregion



        // 버튼 > 바운더리 테스트
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.ReflectVector();
        }
        // 버튼 > 면적 입력
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            ReadData();

            HDC.CreateArea Command = new HDC.CreateArea();

            Command.CreateSingleObj();
        }
        // 버튼 > 면적 일괄 입력
        private void simpleButton8_Click(object sender, EventArgs e)
        {
            ReadData();

            HDC.CreateArea Command = new HDC.CreateArea();

            Command.CreateMultiObj();
        }
        // 버튼 > 치수선 입력
        private void simpleButton9_Click(object sender, EventArgs e)
        {
            ReadData();

            HDC.CreateDimension Command = new HDC.CreateDimension();

            Command.CreateAtPolyline();
        }
        // 버튼 > 문자 스타일 생성
        private void simpleButton10_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateDimensions Command = new CreateDimensions();

            Command.CreateDimensionStyle();
        }

        // 버튼 > 벽 생성
        private void simpleButton11_Click(object sender, EventArgs e)
        {
            ReadData();

            HDC.CreateWalls Command = new HDC.CreateWalls();

            Command.Create_by_Polyline();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void btn_Play7_Click(object sender, EventArgs e)
        {

        }

        // 버튼 > 닫기
        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 버튼 > 테스트
        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            CreateBoundary Command = new CreateBoundary();

            Command.Create_Boundaries();


            #region 초기 테스트
            //var acSSet = Util.Select.MultiObjs();

            //var acEnts = Util.Get.Entity(acSSet, typeof(Polyline));

            //var acLines = new List<LineSegment3d>();

            //var acTempLines = new List<Line>();

            //acEnts.ForEach(acEnt =>
            //{
            //    var acEntsTemp = acEnts.Where(x => x != acEnt);
            //    var acPoly = acEnt as Polyline;

            //    for (int i = 0; i < acPoly.NumberOfVertices; i++)
            //    {
            //        try
            //        {
            //            var acLine = acPoly.GetLineSegmentAt(i);

            //            if (!acLines.Contains(acLine))
            //            {
            //                acLines.Add(acLine);
            //            }

            //            var Di = acLine.Direction;
            //            var D = 300;

            //            var sp = acLine.StartPoint;
            //            var ep = acLine.EndPoint;
            //            var SP = Util.P.Move(acLine.StartPoint, -Di * D);
            //            var EP = Util.P.Move(acLine.EndPoint, Di * D);

            //            LineSegment3d LS1 = new LineSegment3d(sp, SP);
            //            LineSegment3d LS2 = new LineSegment3d(ep, EP);

            //            var IP1 = CAD.IntersectedLine(acEntsTemp.ToList(), LS1, sp, 500);
            //            var IP2 = CAD.IntersectedLine(acEntsTemp.ToList(), LS2, ep, 500);

            //            if (IP1 != new Point3d())
            //            {
            //                //if (sp.DistanceTo(IP1) < 500)
            //                //{
            //                //}
            //                var L1 = CAD.CreateLine(sp, IP1, ColorIndex.DarkGray);
            //                acTempLines.Add(L1);

            //                if (!acLines.Contains(LS1)) acLines.Add(LS1);
            //            }
            //            if (IP2 != new Point3d())
            //            {
            //                //if (ep.DistanceTo(IP2) < 500)
            //                //{
            //                //}
            //                var L2 = CAD.CreateLine(ep, IP2, ColorIndex.DarkGray);
            //                acTempLines.Add(L2);

            //                if (!acLines.Contains(LS2)) acLines.Add(LS2);
            //            }
            //        }
            //        catch
            //        {
            //        }
            //    }
            //});

            //var P_X = (from a in acLines
            //           let p = a.StartPoint
            //           orderby p.X
            //           select p.X);

            //var P_Y = (from a in acLines
            //           let p = a.StartPoint
            //           orderby p.Y
            //           select p.Y);


            //var _Points = (from a in acLines
            //               let p = a.StartPoint
            //               orderby p.X, p.Y
            //               select p);

            //var _acLines = (from a in acLines
            //                let p = a.StartPoint
            //                orderby p.X, p.Y
            //                select a);

            //var Min_P = new Point3d(P_X.First() - 1000, P_Y.First() - 1000, 0);
            //var Max_P = new Point3d(P_X.Last() + 1000, P_Y.Last() + 1000, 0);

            //var Box = CAD.CreateRectangle(Min_P, Max_P);

            //var P1 = Util.P.Move(Min_P, 100, 100);

            //var objColl = acDocEd.TraceBoundary(P1, true);

            //var acObjs = from a in objColl.Cast<Entity>().ToList()
            //             select a;

            //if (acObjs.Any())
            //{
            //    using (Transaction T = acDb.TransactionManager.StartTransaction())
            //    {
            //        var BT = T.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;
            //        var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

            //        acObjs.ToList().ForEach(a =>
            //        {
            //            var acPoly = a as Polyline;

            //            if (Math.Abs(acPoly.Area - Box.Area) > 1)
            //            {
            //                BTR.AppendEntity(acPoly);
            //                T.AddNewlyCreatedDBObject(acPoly, true);
            //            }
            //        });

            //        T.Commit();
            //    }
            //}

            ////using (Transaction T = acDb.TransactionManager.StartTransaction())
            ////{

            ////    object[] dataArray = new object[1];

            ////    var P = Util.P.Move(Min_P, 100, 100);
            ////    var str = P.X + "," + P.Y;
            ////    dataArray[0] = "-boundary " + str + "  ";

            ////    B = true;

            ////    //acDoc.SendStringToExecute(dataArray[0].ToString(), true, true, false);
            ////    //acDoc.SendStringToExecute(dataArray[0].ToString(), true, false, false);

            ////    //acDocEd.Command("boundary", " ", P, " ");

            ////    T.Commit();
            ////}


            ////MessageBox.Show(Box.Id.ObjectClass.DxfName);

            //CAD.Erase(Box.Id);

            //acTempLines.ForEach(a => CAD.Erase(a.Id));

            #endregion
        }
        private bool B = false;

        private void AcDb_ObjectAppended(object sender, ObjectEventArgs e)
        {
            //if (e.DBObject.GetType() == typeof(Polyline))
            //{
            //    string s = e.DBObject.Id.ToString() + "\n" + e.DBObject.ToString() + "\n" + e.DBObject.AcadObject.ToString();

            //    MessageBox.Show(s);
            //}
        }

        static ObjectIdCollection collection = new ObjectIdCollection();
        static string commandName = "";

        private static bool IsCommandActive()
        {
            String str = (String)Application.GetSystemVariable("CMDNAMES");

            if (String.Compare(commandName, str, true) != 0)
            {
                return true;
            }

            return false;
        }
        static void Database_ObjectAppended(object sender, ObjectEventArgs e)
        {

            collection.Add(e.DBObject.ObjectId);

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            var acSSet = Utils.Select.SingleObj();

            if (acSSet == null)
            {
                B = false;

                return;
            }
            var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));

            using (Transaction T = acDb.TransactionManager.StartTransaction())
            {
                //var BT = T.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                acEnts.ForEach(acEnt =>
                {
                    var ent = T.GetObject(acEnt.Id, OpenMode.ForWrite) as Entity;

                    ent.Visible = false;
                });

                T.Commit();
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            //var acSSet = AC.Editor.SelectImplied().Value;
            var acSSet = Utils.Select.SingleObj();

            if (acSSet == null)
            {
                B = false;

                return;
            }

            var polylines = Utils.Get.GetEntity<Polyline>(acSSet);
            var texts = Utils.Get.GetEntity<DBText>(acSSet);

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    polylines.ForEach(pl =>
                    {
                        var acPoly = t.GetObject(pl.Id, OpenMode.ForWrite) as Polyline;

                        acPoly.Color = ColorIndex.Blue;
                    });

                    t.Commit();
                }
            }


            MessageBox.Show(polylines.Count.ToString() + "\n" + texts.Count.ToString());



            //var acPolies = Util.Get.Entity(acSSet, typeof(Polyline));
            //var acTexts = Util.Get.Entity(acSSet, typeof(DBText));

            //MessageBox.Show(acPolies.Count.ToString() + "\n" + acTexts.Count.ToString());





            ////var BT = T.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;

            //acEnts.ForEach(acEnt =>
            //{
            //    using (Transaction T = acDb.TransactionManager.StartTransaction())
            //    {
            //        T.GetObject(acEnt.Id, OpenMode.ForWrite).Erase(true);

            //        T.Commit();
            //    }
            //});

        }
    }
}
