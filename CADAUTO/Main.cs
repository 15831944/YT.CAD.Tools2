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
using Autodesk.Windows;
using System.Reflection;
using Microsoft.Win32;
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
#endregion

#region PROJECT
#endregion

[assembly: ExtensionApplication(null)]
[assembly: CommandClass(typeof(CADAUTO.Main))]

namespace CADAUTO
{
    public class Main
    {
        #region 생성자
        public Main()
        {

        }
        #endregion

        #region PUBLIC 메서드

        [CommandMethod("YT_CAD_TOOL")]
        public void YT_CAD_TOOL()
        {
            YT_CAD_TOOL UI = new YT_CAD_TOOL();

            UI.Show();
        }
        [CommandMethod("SSS")]
        public void SSS()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult acPSR = acDocEd.GetSelection();

            double Min = 100;
            double Max = 300;

            // 선택한 객체를 받음
            if (acPSR.Status == PromptStatus.OK)
            {
                var edgePtrs = new Curve2dCollection();
                var edgeTypes = new IntegerCollection();

                using (Transaction T = acDb.TransactionManager.StartTransaction())
                {
                    BlockTable BT = T.GetObject(acDb.BlockTableId, OpenMode.ForWrite) as BlockTable;
                    BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    SelectionSet acSSet = acPSR.Value;

                    foreach (var objID in acSSet.GetObjectIds())
                    {
                        var acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity;

                        if (acEnt is Polyline)
                        {
                            Polyline acPL = acEnt as Polyline;

                            GetEdgeInformation(acPL, ref edgePtrs, ref edgeTypes);

                        }
                    }
                }

                var acPolylines = from a in edgePtrs.Cast<Curve2d>()
                                  orderby a.StartPoint.GetDistanceTo(a.EndPoint) descending
                                  select a;

                //var usedCurve = new List<Curve2d>();
                var usedCurve = new List<Point2d>();

                acPolylines.Cast<Curve2d>().ToList().ForEach(c =>
                {
                    var CenterP = CAD.GetCenterPoint2d(c.StartPoint, c.EndPoint);

                    var curves = from a in edgePtrs.Cast<Curve2d>().ToList()
                                 where a != c
                                 select a;

                    // c와 평행한 선을 받음
                    var MatchedCurves = from a in curves
                                        let d = a.GetDistanceTo(c)
                                        where CAD.GetVector(a).GetNormal().IsEqualTo(-CAD.GetVector(c).GetNormal())
                                        where d > Min && d < Max
                                        let cp1 = CAD.GetCenterPoint2d(c)
                                        let cp2 = CAD.GetCenterPoint2d(a)
                                        orderby cp1.GetDistanceTo(cp2) ascending
                                        select a;

                    if (MatchedCurves.Any())
                    {
                        //CAD.CreateLine(c.StartPoint, c.EndPoint);

                        bool B = true;

                        MatchedCurves.ToList().ForEach(c1 =>
                        {
                            var cp1 = CAD.GetCenterPoint2d(c1);

                            usedCurve.ForEach(cp2 =>
                            {
                                if (cp1.IsEqualTo(cp2))
                                {
                                    B = false;
                                }
                            });
                        });

                        if (B)
                        {
                            CreateRectangle(c, MatchedCurves.ToList());
                            //CAD.CreateLine(CAD.ToPoint3D(c.StartPoint), CAD.ToPoint3D(c.EndPoint));
                            //CAD.CreateLine(CAD.ToPoint3D(MatchedCurves.First().StartPoint), CAD.ToPoint3D(MatchedCurves.First().EndPoint));

                            usedCurve.Add(CAD.GetCenterPoint2d(c));
                            //usedCurve.Add(CAD.GetCenterPoint2d(MatchedCurves.First()));
                        }
                    }
                });
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }

        private void GetEdgeInformation(Polyline pline, ref Curve2dCollection plCurves, ref IntegerCollection edgeTypes)
        {
            int segCount = pline.NumberOfVertices;

            for (int cnt = 0; cnt < segCount; cnt++)
            {
                SegmentType type = pline.GetSegmentType(cnt);

                switch (type)
                {

                    case SegmentType.Arc:

                        CircularArc2d arc2d = pline.GetArcSegment2dAt(cnt);

                        plCurves.Add(arc2d);

                        edgeTypes.Add((int)Enum.Parse(typeof(HatchEdgeType),

                                HatchEdgeType.CircularArc.ToString()));

                        break;

                    case SegmentType.Line:

                        LineSegment2d line2d = pline.GetLineSegment2dAt(cnt);

                        plCurves.Add(line2d);

                        edgeTypes.Add((int)Enum.Parse(typeof(HatchEdgeType),

                                       HatchEdgeType.Line.ToString()));

                        break;

                    case SegmentType.Coincident:

                        break;

                    case SegmentType.Empty:

                        break;

                    case SegmentType.Point:

                        break;

                }

            }
        }


        private void CreateRectangle(Curve2d C1, List<Curve2d> Cs)
        {
            var C2 = Cs.First();

            var sp1 = C1.StartPoint;
            var ep1 = C1.EndPoint;
            var Ps1 = new List<Point2d> { sp1, ep1 };

            var sp2 = C2.StartPoint;
            var ep2 = C2.EndPoint;
            var Ps2 = new List<Point2d> { sp2, ep2 };

            var Ps = from p1 in Ps1
                     from p2 in Ps2
                     let d = p1.GetDistanceTo(p2)
                     orderby d descending
                     select new List<Point2d> { p1, p2 };

            if (Ps.Any())
            {
                var Points = Ps.First();

                var Vec1 = CAD.GetVector(C1);
                var Vec2 = CAD.GetVector(Points[0], Points[1]);

                if (Vec1.GetAngleTo(Vec2) > Math.PI / 2)
                {
                    Vec1 = -Vec1;
                }

                double Ang = Math.Abs(Vec1.GetAngleTo(Vec2));

                var P1 = CAD.ToPoint3D(Points[0]);
                var P2 = CAD.ToPoint3D(Points[1]);

                CAD.CreateRectangle(P1, P2, Vec1, Ang);
            }
        }


        [CommandMethod("CadTest")]
        public void CadTest()
        {
            UI frm = new UI();

            frm.Show();
        }

        [CommandMethod("TabTest")]
        public void TabTest()
        {
            MessageBox.Show("sss");
        }


        [CommandMethod("YTBIM")]
        public void YTBIM()
        {
            // 상하관계 : 탭 > 패널 > 패널 소스 > 갤러리 > 버튼 > 툴팁
            // 생성순서 : 탭 -> 갤러리 -> 패널 소스 -> 버튼 -> 툴팁 -> 패널

            #region 탭 추가
            RibbonControl RC = ComponentManager.Ribbon;

            // 메뉴탭에 탭 추가
            RibbonTab RT = new RibbonTab();
            RT.Name = "YT.BIM";
            RT.Title = "YT.BIM";
            RT.Id = "YTBIM ID";
            RC.Tabs.Add(RT);
            #endregion

            #region 갤러리 추가
            // 버튼이 담기는 갤러리 추가
            RibbonGallery RG1 = new RibbonGallery();
            RG1.Name = "MyGallery";
            RG1.Id = "MyGalleryId";
            RG1.DisplayMode = GalleryDisplayMode.ComboBox;

            RibbonGallery RG2 = new RibbonGallery();
            RG2.Name = "MyGallery";
            RG2.Id = "MyGalleryId";
            RG2.DisplayMode = GalleryDisplayMode.LargeButton;

            RibbonGallery RG3 = new RibbonGallery();
            RG3.Name = "MyGallery";
            RG3.Id = "MyGalleryId";
            RG3.DisplayMode = GalleryDisplayMode.StandardButton;

            RibbonGallery RG4 = new RibbonGallery();
            RG4.Name = "MyGallery";
            RG4.Id = "MyGalleryId";
            RG4.DisplayMode = GalleryDisplayMode.Window;

            #endregion

            #region 패널 소스 추가
            // 패널 소스 추가
            RibbonPanelSource RPS = new RibbonPanelSource();
            RPS.Name = "YT AUTO BIM";
            RPS.Title = "YT AUTO BIM";
            RPS.Id = "MyPanelId";
            RPS.Items.Add(RG1);
            RPS.Items.Add(RG2);
            RPS.Items.Add(RG3);
            RPS.Items.Add(RG4);
            #endregion

            #region 버튼 추가
            // 버튼 1
            RibbonButton B1 = new RibbonButton();
            B1.Name = "기둥일람표 작성";
            B1.Text = "기둥 일람표";
            B1.Id = "MyButtonId1";
            B1.ShowText = true;
            B1.ShowImage = true;
            B1.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B1.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            //B1.Orientation = System.Windows.Controls.Orientation.Vertical;
            B1.Size = RibbonItemSize.Large;
            B1.CommandHandler = new RibbonCommandHandler();

            // 버튼 1의 툴팁
            RibbonToolTip rbnT1 = new RibbonToolTip();
            rbnT1.Command = "Create Column Schedule";
            rbnT1.Title = "YT AUTO COLUMN SCHEDULE";
            rbnT1.Content = "Create column schedule automatically";
            rbnT1.ExpandedContent = "In the opened window, input column data and click Just one button.";
            B1.ToolTip = rbnT1;

            RG1.Items.Add(B1);
            RG2.Items.Add(B1);
            RG3.Items.Add(B1);
            RG4.Items.Add(B1);

            // 버튼2
            RibbonButton B2 = new RibbonButton();
            B2.Name = "MyButton2";
            B2.Text = "My Button2";
            B2.Id = "MyButtonId2";
            B2.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B2.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B2.Size = RibbonItemSize.Large;
            B2.Orientation = System.Windows.Controls.Orientation.Vertical;
            B2.ShowText = true;
            RG1.Items.Add(B2);
            RG2.Items.Add(B2);
            RG3.Items.Add(B2);
            RG4.Items.Add(B2);

            // 버튼3
            RibbonButton B3 = new RibbonButton();
            B3.Name = "MyButton3";
            B3.Text = "My Button3";
            B3.Id = "MyButtonId3";
            B3.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B3.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B3.Size = RibbonItemSize.Large;
            B3.Orientation = System.Windows.Controls.Orientation.Vertical;
            B3.ShowText = true;
            RG1.Items.Add(B3);
            RG2.Items.Add(B3);
            RG3.Items.Add(B3);
            RG4.Items.Add(B3);

            // 버튼4
            RibbonButton B4 = new RibbonButton();
            B4.Name = "MyButton4";
            B4.Text = "My Button4";
            B4.Id = "MyButtonId4";
            B4.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B4.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B4.Size = RibbonItemSize.Large;
            B4.Orientation = System.Windows.Controls.Orientation.Vertical;
            B4.ShowText = true;
            RG1.Items.Add(B4);
            RG2.Items.Add(B4);
            RG3.Items.Add(B4);
            RG4.Items.Add(B4);
            #endregion

            #region 패널 추가
            RibbonRowPanel RRP = new RibbonRowPanel();
            RRP.Items.Add(B1);
            RRP.Items.Add(new RibbonSeparator());
            RRP.Items.Add(B2);
            RRP.Items.Add(B3);
            RRP.Items.Add(B4);
            RPS.Items.Add(RRP);

            // 탭에 패널 추가
            RibbonPanel RP = new RibbonPanel();
            RP.Source = RPS;
            RT.Panels.Add(RP);
            //RT.Panels.Add(RPS);
            #endregion

        }
        #endregion


        #region .NET Developer's Guide

        [CommandMethod("AdskGreeting")]
        public void AdskGreeting()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (MText objText = new MText())
                {
                    objText.Location = new Point3d(2, 2, 0);
                    objText.Contents = "Greetings, Welcome to AutoCAD .NET";
                    objText.TextStyleId = DB.Textstyle;

                    BTR.AppendEntity(objText);
                    T.AddNewlyCreatedDBObject(objText, true);
                }

                T.Commit();
            }
        }

        [CommandMethod("ListEntities")]
        public void ListEntities()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                int Cnt = 0;
                Doc.Editor.WriteMessage("\nModel space objects: ");

                foreach (ObjectId Obj in BTR)
                {
                    Doc.Editor.WriteMessage("\n" + Obj.ObjectClass.DxfName);

                    Cnt += 1;
                }

                if (Cnt == 0)
                {
                    Doc.Editor.WriteMessage("\n No objects found");
                }

                //T.Commit();
                Application.UpdateScreen();
            }
        }

        /// <summary>
        /// # 1
        /// </summary>
        #region [1 Basic of the AutoCAD .NET API]

        #region 1.3 Collection Objects

        #region 1.3.2 Add a New Member to a Collection Object
        [CommandMethod("AddMyLayer")]
        public void AddMyLayer()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                LayerTable LT = T.GetObject(DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (LT.Has("MyLayer") != true)
                {
                    T.GetObject(DB.LayerTableId, OpenMode.ForWrite);

                    using (LayerTableRecord LTR = new LayerTableRecord())
                    {
                        LTR.Name = "MyLayer";
                        LT.Add(LTR);
                        T.AddNewlyCreatedDBObject(LTR, true);
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #region 1.3.3 Iterate through a Collection Object 
        [CommandMethod("IterateLayers")]
        public void IterateLayers()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                LayerTable LT = T.GetObject(DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId objID in LT)
                {
                    LayerTableRecord LTR = T.GetObject(objID, OpenMode.ForRead) as LayerTableRecord;

                    Doc.Editor.WriteMessage("\n" + LTR.Name);
                }

                //T.Commit();
            }
        }

        [CommandMethod("FindMyLayer")]
        public void FindMyLayer()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                LayerTable LT = T.GetObject(DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (LT.Has("MyLayer") != true)
                {
                    Doc.Editor.WriteMessage("\n'MyLayer' does not exist");
                }
                else
                {
                    Doc.Editor.WriteMessage("\n'MyLayer' exists");
                }

                //T.Commit();
            }
        }
        #endregion

        #region 1.3.4 Erase a Member of a Collection Object 
        [CommandMethod("RemoveMyLayer")]
        public void RemoveMyLayer()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                LayerTable LT = T.GetObject(DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (LT.Has("MyLayer") == true)
                {
                    LayerTableRecord LTR = T.GetObject(LT["MyLayer"], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        LTR.Erase();

                        Doc.Editor.WriteMessage("\n'MyLayer' was erased");

                        T.Commit();
                    }
                    catch
                    {
                        Doc.Editor.WriteMessage("\n'MyLayer' could not be erased");
                    }

                }
                else
                {
                    Doc.Editor.WriteMessage("\n'MyLayer' exists");
                }

                //T.Commit();
            }
        }
        #endregion

        #endregion

        #endregion


        /// <summary>
        /// # 2
        /// </summary>
        #region [2 Control the AutoCAD Environment(.NET)]

        #region 2.1 Control the Application Window

        [CommandMethod("PositionApplicationWindow")]
        public void PositionApplicationWindow()
        {
            System.Windows.Point ptApp = new System.Windows.Point(0, 0);
            Application.MainWindow.DeviceIndependentLocation = ptApp;

            System.Windows.Size szApp = new System.Windows.Size(400, 400);
            Application.MainWindow.DeviceIndependentSize = szApp;
        }

        [CommandMethod("MinMaxApplicationWindow")]
        public void MinMaxApplicationWindow()
        {
            Application.MainWindow.WindowState = Autodesk.AutoCAD.Windows.Window.State.Minimized;

            MessageBox.Show("Minimized", "MinMax",
                MessageBoxButtons.OK,
                MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification);

            Application.MainWindow.WindowState = Autodesk.AutoCAD.Windows.Window.State.Maximized;

            MessageBox.Show("Maximized", "MinMax");
        }

        [CommandMethod("CurrentWindowState")]
        public void CurrentWindowState()
        {
            MessageBox.Show("The application window is " + Application.MainWindow.WindowState.ToString(), "Window State");
        }

        [CommandMethod("HideWindowState")]
        public void HideWindowState()
        {
            Application.MainWindow.Visible = false;
            MessageBox.Show("Invisible", "Show/Hide");

            Application.MainWindow.Visible = true;
            MessageBox.Show("Visible", "Show/Hide");
        }

        #endregion

        #region 2.2 Control the Drawing Windows(.NET)

        #region 2.2.1 Position and Size the Document

        [CommandMethod("SizeDocumentWindow")]
        public void SizeDocumentWindow()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            Doc.Window.WindowState = Autodesk.AutoCAD.Windows.Window.State.Normal;

            System.Windows.Point ptApp = new System.Windows.Point(0, 0);
            Doc.Window.DeviceIndependentLocation = ptApp;

            System.Windows.Size szApp = new System.Windows.Size(400, 400);
            Doc.Window.DeviceIndependentSize = szApp;
        }

        [CommandMethod("MinMaxDocumentWindow")]
        public void MinMaxDocumentWindow()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            Doc.Window.WindowState = Autodesk.AutoCAD.Windows.Window.State.Minimized;
            MessageBox.Show("Minimized", "MinMax");

            Doc.Window.WindowState = Autodesk.AutoCAD.Windows.Window.State.Maximized;
            MessageBox.Show("Maximized", "MinMax");
        }

        [CommandMethod("CurrentDocWindowState")]
        public void CurrentDocWindowState()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            MessageBox.Show("The document window is " + Doc.Window.WindowState.ToString(), "Window State");
        }

        #endregion

        #region 2.2.2 Zoom and Pan the Current View(.NET)

        [CommandMethod("Zoom_API")]
        public void Zoom(Point3d pMin, Point3d pMax, Point3d pCen, double dFactor)
        {
            //Point3d pMin = new Point3d();
            //Point3d pMax = new Point3d();
            //Point3d pCen = new Point3d();
            //double dFactor = 1.0;

            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            int nCurVport = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"));

            if (Db.TileMode == true)
            {
                if (pMin.Equals(new Point3d()) && pMax.Equals(new Point3d()))
                {
                    pMin = Db.Extmin;
                    pMax = Db.Extmax;
                }
            }
            else
            {
                if (nCurVport == 1)
                {
                    if (pMin.Equals(new Point3d()) && pMax.Equals(new Point3d()))
                    {
                        pMin = Db.Pextmin;
                        pMax = Db.Pextmax;
                    }
                }
                else
                {
                    if (pMin.Equals(new Point3d()) && pMax.Equals(new Point3d()))
                    {
                        pMin = Db.Extmin;
                        pMax = Db.Extmax;
                    }
                }
            }

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                using (ViewTableRecord VTR = Doc.Editor.GetCurrentView())
                {
                    Extents3d eExtents;

                    // Translate WCT coordinates to DCS
                    Matrix3d mat3d;
                    mat3d = Matrix3d.PlaneToWorld(VTR.ViewDirection);
                    mat3d = Matrix3d.Displacement(VTR.Target - Point3d.Origin) * mat3d;
                    mat3d = Matrix3d.Rotation(-VTR.ViewTwist, VTR.ViewDirection, VTR.Target) * mat3d;

                    //pCen = new Point3d(VTR.CenterPoint.X, VTR.CenterPoint.Y, 0);

                    if (pCen.DistanceTo(Point3d.Origin) != 0)
                    {
                        pMin = new Point3d(pCen.X - (VTR.Width / 2), pCen.Y - (VTR.Height / 2), 0);
                        pMin = new Point3d(pCen.X + (VTR.Width / 2), pCen.Y + (VTR.Height / 2), 0);
                    }

                    using (Line L = new Line(pMin, pMax))
                    {
                        eExtents = new Extents3d(L.Bounds.Value.MinPoint, L.Bounds.Value.MaxPoint);
                    }

                    double dViewRatio = VTR.Width / VTR.Height;

                    mat3d = mat3d.Inverse();
                    eExtents.TransformBy(mat3d);

                    double dWidth;
                    double dHeight;

                    Point2d pNewCen;

                    if (pCen.DistanceTo(Point3d.Origin) != 0)
                    {
                        dWidth = VTR.Width;
                        dHeight = VTR.Height;

                        if (dFactor == 0)
                        {
                            pCen = pCen.TransformBy(mat3d);
                        }

                        pNewCen = new Point2d(pCen.X, pCen.Y);
                    }
                    else
                    {
                        dWidth = eExtents.MaxPoint.X - eExtents.MinPoint.X;
                        dHeight = eExtents.MaxPoint.Y - eExtents.MinPoint.Y;

                        pNewCen = new Point2d((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5, (eExtents.MaxPoint.Y + eExtents.MinPoint.Y) * 0.5);
                    }

                    if (dWidth > (dHeight * dViewRatio)) dHeight = dWidth / dViewRatio;

                    if (dFactor != 0)
                    {
                        VTR.Height = dHeight * dFactor;
                        VTR.Width = dWidth * dFactor;
                    }

                    VTR.CenterPoint = pNewCen;
                    
                    Doc.Editor.SetCurrentView(VTR);
                }

                T.Commit();
            }

        }

        [CommandMethod("DefineToWindow")]
        public void DefineToWindow()
        {
            Point3d pMin = new Point3d(1.3, 7.8, 0);
            Point3d pMax = new Point3d(13.7, -2.6, 0);

            Zoom(pMin, pMax, new Point3d(), 1);
        }

        [CommandMethod("ScaleAView")]
        public void ScaleAView()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            using (ViewTableRecord VTR = Doc.Editor.GetCurrentView())
            {
                Point3d pCen = new Point3d(VTR.CenterPoint.X, VTR.CenterPoint.Y, 0);

                double dScale = 0.5;

                Zoom(new Point3d(), new Point3d(), pCen, 1 / dScale);
            }
        }

        [CommandMethod("ZoomCenter")]
        public void ZoomCenter()
        {
            Zoom(new Point3d(), new Point3d(), new Point3d(5, 5, 0), 1);
        }

        [CommandMethod("ZoomExtents")]
        public void ZoomExtents()
        {
            Zoom(new Point3d(), new Point3d(), new Point3d(), 1.01075);
        }

        [CommandMethod("ZoomLimits")]
        public void ZoomLimits()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            Zoom(new Point3d(Db.Limmin.X, Db.Limmin.Y, 0), new Point3d(Db.Limmax.X, Db.Limmax.Y, 0), new Point3d(), 1);
        }

        #endregion

        #region 2.2.3 Use Named Views

        [CommandMethod("CreateNamedView")]
        public void CreateNamedView()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                ViewTable VT;
                VT = T.GetObject(Db.ViewTableId, OpenMode.ForRead) as ViewTable;

                string ViewName = "View1";

                if (!VT.Has(ViewName))
                {
                    T.GetObject(Db.ViewTableId, OpenMode.ForWrite);

                    using (ViewTableRecord VTR = new ViewTableRecord())
                    {
                        VTR.Name = ViewName;

                        VT.Add(VTR);
                        T.AddNewlyCreatedDBObject(VTR, true);

                        Doc.Editor.SetCurrentView(VTR);
                    }
                }

                T.Commit();
            }
        }

        [CommandMethod("EraseNamedView")]
        public void EraseNamedView()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                ViewTable VT;
                VT = T.GetObject(Db.ViewTableId, OpenMode.ForRead) as ViewTable;

                string ViewName = "View1";

                if (VT.Has(ViewName))
                {
                    T.GetObject(Db.ViewTableId, OpenMode.ForWrite);

                    ViewTableRecord VTR;
                    VTR = T.GetObject(VT[ViewName], OpenMode.ForWrite) as ViewTableRecord;

                    VTR.Erase();

                    T.Commit();
                }
            }
        }

        #endregion

        #region 2.2.4 Use Tiled Viewports

        #region 2.2.4.1 Identify and Manipulate the Active Viewport

        [CommandMethod("CreateModelViewport")]
        public void CreateModelViewport()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                ViewportTable VpT;
                VpT = T.GetObject(Db.ViewportTableId, OpenMode.ForRead) as ViewportTable;

                string ViewPortName = "TEST_VIEWPORT";

                if (VpT.Has(ViewPortName))
                {
                    T.GetObject(Db.ViewportTableId, OpenMode.ForWrite);

                    using (ViewportTableRecord VTRup = new ViewportTableRecord())
                    {
                        VpT.Add(VTRup);
                        T.AddNewlyCreatedDBObject(VTRup, true);

                        VTRup.Name = ViewPortName;
                        VTRup.LowerLeftCorner = new Point2d(0, 0.5);
                        VTRup.UpperRightCorner = new Point2d(1, 1);

                        foreach (ObjectId ID in VpT)
                        {
                            ViewportTableRecord VTR;
                            VTR = T.GetObject(ID, OpenMode.ForRead) as ViewportTableRecord;

                            if (VTR.Name == "*Active")
                            {
                                T.GetObject(ID, OpenMode.ForWrite);
                                VTR.Erase();
                            }
                        }

                        foreach (ObjectId ID in VpT)
                        {
                            ViewportTableRecord VTR;
                            VTR = T.GetObject(ID, OpenMode.ForRead) as ViewportTableRecord;

                            if (VTR.Name == ViewPortName)
                            {
                                ViewportTableRecord VTRClone;
                                VTRClone = VTR.Clone() as ViewportTableRecord;

                                VpT.Add(VTRClone);
                                VTRClone.Name = "*Active";
                                T.AddNewlyCreatedDBObject(VTRClone, true);
                            }
                        }

                        Doc.Editor.UpdateTiledViewportsFromDatabase();
                    }
                }

                T.Commit();
            }
        }

        #endregion

        #region 2.2.4.2 Make A Tiled Viewport Current

        [CommandMethod("SplitAndIterateModelViewports")]
        public void SplitAndIterateModelViewports()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                ViewportTable VT;
                VT = T.GetObject(Db.ViewportTableId, OpenMode.ForWrite) as ViewportTable;

                ViewportTableRecord VTR;
                VTR = T.GetObject(Doc.Editor.ActiveViewportId, OpenMode.ForWrite) as ViewportTableRecord;

                using (ViewportTableRecord vtr = new ViewportTableRecord())
                {
                    VT.Add(vtr);
                    T.AddNewlyCreatedDBObject(vtr, true);

                    vtr.Name = "*Active";

                    vtr.LowerLeftCorner = VTR.LowerLeftCorner;

                    vtr.UpperRightCorner = new Point2d(VTR.UpperRightCorner.X,
                                                       VTR.LowerLeftCorner.Y + (VTR.UpperRightCorner.Y - VTR.LowerLeftCorner.Y) / 2);

                    VTR.LowerLeftCorner = new Point2d(VTR.LowerLeftCorner.X, vtr.UpperRightCorner.Y);

                    Doc.Editor.UpdateTiledViewportsFromDatabase();

                    foreach (ObjectId ID in VT)
                    {
                        ViewportTableRecord acVportTblRecCur;
                        acVportTblRecCur = T.GetObject(ID, OpenMode.ForRead) as ViewportTableRecord;

                        if (acVportTblRecCur.Name == "*Active")
                        {
                            Application.SetSystemVariable("CVPORT", acVportTblRecCur.Number);

                            Application.ShowAlertDialog("Viewport: " + acVportTblRecCur.Number +
                                " is now active." +
                                "\nLower left corner: " +
                                acVportTblRecCur.LowerLeftCorner.X + ", " +
                                acVportTblRecCur.LowerLeftCorner.Y +
                                "\nUpper right corner: " +
                                acVportTblRecCur.UpperRightCorner.X + ", " +
                                acVportTblRecCur.UpperRightCorner.Y);
                        }
                    }
                }

                T.Commit();
            }
        }

        #endregion

        #endregion

        #endregion

        #region 2.3 Create, Open, Save, and Close Drawings

        #region 2.3.1 Create and Close a Drawing
        [CommandMethod("NewDrawing", CommandFlags.Session)]
        public void NewDrawing()
        {
            string strTemplatePath = "acad.dwt";

            DocumentCollection DocMgr = Application.DocumentManager;
            Document Doc = DocMgr.Add(strTemplatePath);

            DocMgr.MdiActiveDocument = Doc;
        }

        [CommandMethod("OpenDrawing", CommandFlags.Session)]
        public void OpenDrawing()
        {
            string strFileName = @"C:\campus.dwg";

            DocumentCollection DocMgr = Application.DocumentManager;

            if (File.Exists(strFileName))
            {
                DocMgr.Open(strFileName, false);
            }
            else
            {
                DocMgr.MdiActiveDocument.Editor.WriteMessage("File " + strFileName + " does not exist.");
            }
        }
        #endregion

        #region 2.3.2 Save and Close a Drawing
        [CommandMethod("SaveActiveDrawing")]
        public void SaveActiveDrawing()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            string strDWGName = Doc.Name;

            object obj = Application.GetSystemVariable("DWGTITLED");

            if (System.Convert.ToInt16(obj) == 0)
            {
                strDWGName = @"c:\MyDrawing.dwg";
            }

            Doc.Database.SaveAs(strDWGName, true, DwgVersion.Current, Doc.Database.SecurityParameters);
        }

        [CommandMethod("DrawingSaved")]
        public void DrawingSaved()
        {
            object obj = Application.GetSystemVariable("DBMOD");

            if (System.Convert.ToInt16(obj) != 0)
            {
                if (System.Windows.Forms.MessageBox.Show("Do you wish to save this drawing", "Save Drawing",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Document Doc = Application.DocumentManager.MdiActiveDocument;

                    Doc.Database.SaveAs(Doc.Name, true, DwgVersion.Current, Doc.Database.SecurityParameters);
                }
            }
        }
        #endregion

        #region 2.3.3 Work With No Documents Open
        //public class MyCommandHandler : System.Windows.Input.ICommand
        //{

        //}
        #endregion

        #endregion

        #region 2.4 Lock and Unlock a Document
        [CommandMethod("LockDoc", CommandFlags.Session)]
        public void LockDoc()
        {
            DocumentCollection DocMgr = Application.DocumentManager;
            Document NewDoc = DocMgr.Add("acad.dwt");
            Database Db = NewDoc.Database;

            using (DocumentLock LckDoc = NewDoc.LockDocument())
            {
                using (Transaction T = Db.TransactionManager.StartTransaction())
                {
                    BlockTable BT;
                    BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord BTR;
                    BTR = T.GetObject(Db.BlockTableId, OpenMode.ForWrite) as BlockTableRecord;

                    using (Circle C = new Circle())
                    {
                        C.Center = new Point3d(5, 5, 0);
                        C.Radius = 3;

                        BTR.AppendEntity(C);
                        T.AddNewlyCreatedDBObject(C, true);
                    }

                    T.Commit();
                }
            }

            DocMgr.MdiActiveDocument = NewDoc;
        }
        #endregion

        #region 2.5 Set AutoCAD Preferences

        /// Autodesk.AutoCAD.Interop 을 추가
        [CommandMethod("PrefsSetCursor")]
        public void PrefsSetCursor()
        {
            AcadPreferences acPrefComObj = (AcadPreferences)Application.Preferences;

            acPrefComObj.Display.CursorSize = 100;
        }

        [CommandMethod("PrefsSetDisplay")]
        public void PrefsSetDisplay()
        {
            AcadPreferences acPrefComObj = (AcadPreferences)Application.Preferences;

            acPrefComObj.Display.DisplayScrollBars = false;
        }

        #endregion

        #region 2.6 Set and Return System Variables

        #endregion

        #region 2.7 Draw With Precision

        #region 2.7.1 Adjust Snap and Grid Alignment

        [CommandMethod("ChangeGridAndSnap")]
        public void ChangeGridAndSnap()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                ViewportTableRecord VTR;
                VTR = T.GetObject(Doc.Editor.ActiveViewportId, OpenMode.ForWrite) as ViewportTableRecord;

                VTR.GridEnabled = true;
                VTR.GridIncrements = new Point2d(1, 1);
                VTR.SnapEnabled = true;
                VTR.SnapIncrements = new Point2d(0.5, 0.5);
                VTR.SnapBase = new Point2d(1, 1);
                VTR.SnapAngle = 0.524;

                Doc.Editor.UpdateTiledViewportsFromDatabase();

                T.Commit();
            }
        }

        #endregion

        #region 2.7.2 Use Ortho Mode
        public void OrthoMode()
        {
            // 직각 모드
            Application.DocumentManager.MdiActiveDocument.Database.Orthomode = true;
        }
        #endregion

        #region 2.7.3 Calculate Points and Values

        [CommandMethod("AngleFromXAxis")]
        public void AngleFromXAxis()
        {
            Point2d pt1 = new Point2d(2, 5);
            Point2d pt2 = new Point2d(5, 2);

            Application.ShowAlertDialog("Angle from XAxis: " + pt1.GetVectorTo(pt2).Angle.ToString());
        }

        #endregion

        #region 2.7.4 Calculate Areas

        [CommandMethod("CalculateDefinedArea")]
        public void CalculateDefinedArea()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            PromptPointResult pPtRes;
            Point2dCollection colPt = new Point2dCollection();
            PromptPointOptions pPtOpts = new PromptPointOptions("");

            pPtOpts.Message = "\nSpecify first point: ";
            pPtRes = Doc.Editor.GetPoint(pPtOpts);
            colPt.Add(new Point2d(pPtRes.Value.X, pPtRes.Value.Y));

            if (pPtRes.Status == PromptStatus.Cancel) return;

            int nCounter = 1;

            while (nCounter <= 4)
            {
                switch (nCounter)
                {
                    case 1: pPtOpts.Message = "\nSpecify second point: "; break;
                    case 2: pPtOpts.Message = "\nSpecify third point: "; break;
                    case 3: pPtOpts.Message = "\nSpecify fourth point: "; break;
                    case 4: pPtOpts.Message = "\nSpecify fifth point: "; break;
                }
            }

            pPtOpts.UseBasePoint = true;
            pPtOpts.BasePoint = pPtRes.Value;

            pPtRes = Doc.Editor.GetPoint(pPtOpts);
            colPt.Add(new Point2d(pPtRes.Value.X, pPtRes.Value.Y));

            if (pPtRes.Status == PromptStatus.Cancel) return;

            nCounter = nCounter + 1;

            using (Polyline acPoly = new Polyline())
            {
                acPoly.AddVertexAt(0, colPt[0], 0, 0, 0);
                acPoly.AddVertexAt(1, colPt[1], 0, 0, 0);
                acPoly.AddVertexAt(2, colPt[2], 0, 0, 0);
                acPoly.AddVertexAt(3, colPt[3], 0, 0, 0);
                acPoly.AddVertexAt(4, colPt[4], 0, 0, 0);

                Application.ShowAlertDialog("Area of polyline: " + acPoly.Area.ToString());
            }
        }

        #endregion

        #endregion

        #region 2.8 Prompt for User Input

        #region 2.8.1 GetString Method
        [CommandMethod("GetStringFromUser")]
        public void GetStringFromUser()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            PromptStringOptions pStrOpts = new PromptStringOptions("\nEnter your name: ");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = Doc.Editor.GetString(pStrOpts);

            Application.ShowAlertDialog("The name entered was: " + pStrRes.StringResult);

        }
        #endregion

        #region 2.8.2 GetPoint Method
        [CommandMethod("GetPointFromUser")]
        public void GetPointFromUser()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");

            pPtOpts.Message = "\nEnter the start point of the line: ";
            pPtRes = Doc.Editor.GetPoint(pPtOpts);
            Point3d ptStart = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.Cancel) return;

            pPtOpts.Message = "\nEnter the end point of the line: ";
            pPtOpts.UseBasePoint = true;
            pPtOpts.BasePoint = ptStart;
            pPtRes = Doc.Editor.GetPoint(pPtOpts);
            Point3d ptEnd = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.Cancel) return;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BlockTableRecord BTR;

                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Line acLine = new Line(ptStart, ptEnd))
                {
                    BTR.AppendEntity(acLine);
                    T.AddNewlyCreatedDBObject(acLine, true);
                }

                Doc.SendStringToExecute("zoom e ", true, false, true);

                T.Commit();
            }
        }
        #endregion

        #region 2.8.3 GetKeywords Method
        [CommandMethod("GetKeywordFromUser")]
        public void GetKeywordFromUser()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
            pKeyOpts.Message = "\nEnter an option ";
            pKeyOpts.Keywords.Add("Line");
            pKeyOpts.Keywords.Add("Circle");
            pKeyOpts.Keywords.Add("Arc");
            pKeyOpts.AllowNone = false;

            PromptResult pKeyRes = Doc.Editor.GetKeywords(pKeyOpts);

            Application.ShowAlertDialog("Entered keyword: " + pKeyRes.StringResult);
        }

        [CommandMethod("GetKeywordFromUser2")]
        public void GetKeywordFromUser2()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
            pKeyOpts.Message = "\nEnter an options ";
            pKeyOpts.Keywords.Add("Line");
            pKeyOpts.Keywords.Add("Circle");
            pKeyOpts.Keywords.Add("Arc");
            pKeyOpts.Keywords.Default = "Arc";
            pKeyOpts.AllowNone = true;

            PromptResult pKeyRes = Doc.Editor.GetKeywords(pKeyOpts);

            Application.ShowAlertDialog("Entered keyword: " + pKeyRes.StringResult);
        }
        #endregion

        #region 2.8.4 Control User Input
        [CommandMethod("GetIntegerOrKeywordFromUser")]
        public void GetIntegerOrKeywordFromUser()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            PromptIntegerOptions pIntOpts = new PromptIntegerOptions("");
            pIntOpts.Message = "\nEnter the size or ";

            pIntOpts.AllowZero = false;
            pIntOpts.AllowNegative = false;

            pIntOpts.Keywords.Add("Big");
            pIntOpts.Keywords.Add("Small");
            pIntOpts.Keywords.Add("Regular");
            pIntOpts.Keywords.Default = "Regular";
            pIntOpts.AllowNone = true;

            PromptIntegerResult pIntRes = Doc.Editor.GetInteger(pIntOpts);

            if (pIntRes.Status == PromptStatus.Keyword)
            {
                Application.ShowAlertDialog("Entered keyword: " + pIntRes.StringResult);
            }
            else
            {
                Application.ShowAlertDialog("Entered keyword: " + pIntRes.Value.ToString());
            }
        }
        #endregion

        #endregion

        #region 2.9 Access the Command Line
        [CommandMethod("SendACommandToAutoCAD")]
        public void SendACommandToAutoCAD()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;

            Doc.SendStringToExecute("circle 2,2,0 4 ", true, false, false);
            Doc.SendStringToExecute("zoom all ", true, false, false);
        }
        #endregion

        #endregion


        /// <summary>
        /// # 3
        /// </summary>
        #region [3 Create and Edit AutoCAD Entities]

        #region 3.1 Open and Close Objects

        #region 3.1.1 Work With ObjectIds
        public void WorkWithObjectIds()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            #region Type 1
            {
                Transaction T = Db.TransactionManager.StartTransaction();
                LayerTableRecord LTR;
                LTR = T.GetObject(Db.LayerZero, OpenMode.ForRead) as LayerTableRecord;

                T.Dispose();
            }
            #endregion

            #region Type 2
            {
                using (Transaction T = Db.TransactionManager.StartTransaction())
                {
                    LayerTableRecord LTR;
                    LTR = T.GetObject(Db.LayerZero, OpenMode.ForRead) as LayerTableRecord;
                }
            }
            #endregion
        }
        #endregion

        #region 3.1.2 Use Transactions With the Transaction Manager

        #region 3.1.2.1 Use Trancactions to Access and Create Objects
        [CommandMethod("StartTransactionManager")]
        public void StartTransactionManager()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId Id in BTR)
                {
                    Doc.Editor.WriteMessage("\nDXF name: " + Id.ObjectClass.DxfName);
                    Doc.Editor.WriteMessage("\nObjectID: " + Id.ToString());
                    Doc.Editor.WriteMessage("\nHandle: " + Id.Handle.ToString());
                    Doc.Editor.WriteMessage("\n");
                }
            }
        }

        [CommandMethod("AddNewCircleTransaction")]
        public void AddNewCircleTransaction()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                using (Circle Circ = new Circle())
                {
                    Circ.Center = new Point3d(5, 5, 0);
                    Circ.Radius = 3;

                    BTR.AppendEntity(Circ);
                    T.AddNewlyCreatedDBObject(Circ, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.1.2.2 Commit and Rollback Changes
        /// Commit the changes made within the transaction
        // <Transaction>.Commit() => Commit
        /// Abort the transaction and rollback to the previous state
        // <Transaction>.Abort()  => Rollback
        #endregion

        #region 3.1.2.3 Dispose Objects
        /// Dispose an object with the using startment
        /// using (Transaction T = ....)
        /// {
        ///     // Do something 
        ///     
        ///     T.Dispose();
        /// }
        #endregion

        #region 3.1.2.4 Nest Transactions
        [CommandMethod("NestedTransactions")]
        public void NestedTransactions()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = Doc.Database;

            Autodesk.AutoCAD.DatabaseServices.TransactionManager TM = Doc.TransactionManager;

            using (Transaction T1 = TM.StartTransaction())
            {
                Doc.Editor.WriteMessage("\nNumber of transactions active: " + TM.NumberOfActiveTransactions.ToString());

                BlockTable BT;
                BT = T1.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T1.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle Circ = new Circle())
                {
                    Circ.Center = new Point3d(5, 5, 0);
                    Circ.Radius = 3;

                    BTR.AppendEntity(Circ);
                    T1.AddNewlyCreatedDBObject(Circ, true);

                    using (Transaction T2 = TM.StartTransaction())
                    {
                        Doc.Editor.WriteMessage("\nNumber of transactions active: " + TM.NumberOfActiveTransactions.ToString());

                        Circ.ColorIndex = 5;

                        using (Line L = new Line(new Point3d(2, 5, 0), new Point3d(10, 7, 0)))
                        {
                            L.ColorIndex = 3;
                            
                            BTR.AppendEntity(L);
                            T2.AddNewlyCreatedDBObject(L, true);
                        }

                        using (Transaction T3 = TM.StartTransaction())
                        {
                            Doc.Editor.WriteMessage("\nNumber of transactions active: " + TM.NumberOfActiveTransactions.ToString());

                            Circ.ColorIndex = 3;

                            Doc.Editor.WriteMessage("\n");
                            Doc.Editor.Regen();

                            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
                            pKeyOpts.Message = "\nKeep color chage ";
                            pKeyOpts.Keywords.Add("Yes");
                            pKeyOpts.Keywords.Add("No");
                            pKeyOpts.Keywords.Default = "No";
                            pKeyOpts.AllowNone = true;

                            PromptResult pKeyRes = Doc.Editor.GetKeywords(pKeyOpts);

                            if (pKeyRes.StringResult == "No")
                            {
                                T3.Abort();
                            }
                            else
                            {
                                T3.Commit();
                            }
                        }

                        Doc.Editor.WriteMessage("\nNumber of transactions active: " + TM.NumberOfActiveTransactions.ToString());

                        T2.Commit();
                    }
                }

                Doc.Editor.WriteMessage("\nNumber of transactions active: " + TM.NumberOfActiveTransactions.ToString());

                T1.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.1.3 Open and Close Objects Without the Transaction Manager
        [CommandMethod("OpenCloseObjectId")]
        public void OpenCloseObjectId()
        {
            /// Transaction 없이 Object를 열고 닫는 방법
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            BlockTable BT = null;

            try
            {
                BT = Db.BlockTableId.Open(OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR = null;

                try
                {
                    BTR = BT[BlockTableRecord.ModelSpace].Open(OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId Id in BTR)
                    {
                        Doc.Editor.WriteMessage("\nDXF name: " + Id.ObjectClass.DxfName);
                        Doc.Editor.WriteMessage("\nObjectID: " + Id.ToString());
                        Doc.Editor.WriteMessage("\nHandle: " + Id.Handle.ToString());
                        Doc.Editor.WriteMessage("\n");
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (!BTR.ObjectId.IsNull)
                    {
                        BTR.Close();
                        BTR.Dispose();
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (!BT.ObjectId.IsNull)
                {
                    BT.Clone();
                    BT.Dispose();
                }
            }
        }

        [CommandMethod("OpenCloseObjectIdWithUsing")]
        public void OpenCloseObjectIdWithUsing()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (BlockTable BT = Db.BlockTableId.Open(OpenMode.ForRead) as BlockTable)
            {
                using (BlockTableRecord BTR = BT[BlockTableRecord.ModelSpace].Open(OpenMode.ForRead) as BlockTableRecord)
                {
                    foreach (ObjectId Id in BTR)
                    {
                        Doc.Editor.WriteMessage("\nDXF name: " + Id.ObjectClass.DxfName);
                        Doc.Editor.WriteMessage("\nObjectID: " + Id.ToString());
                        Doc.Editor.WriteMessage("\nHandle: " + Id.Handle.ToString());
                        Doc.Editor.WriteMessage("\n");
                    }
                }
            }
        }

        [CommandMethod("AddNewCircleOpenClose")]
        public void AddNewCircleOpenClose()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (BlockTable BT = Db.BlockTableId.Open(OpenMode.ForRead) as BlockTable)
            {
                using (BlockTableRecord BTR = BT[BlockTableRecord.ModelSpace].Open(OpenMode.ForRead) as BlockTableRecord)
                {
                    using (Circle Circ = new Circle())
                    {
                        Circ.Center = new Point3d(5, 5, 0);
                        Circ.Radius = 3;

                        BTR.AppendEntity(Circ);
                    }
                }
            }
        }
        #endregion

        #region 3.1.4 Upgrade and Downgrade Open Objects
        [CommandMethod("FreezeDoorLayer")]
        public void FreezeDoorLayer()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                LayerTable LT = null;
                LT = T.GetObject(Db.LayerTableId, OpenMode.ForRead) as LayerTable;

                //var ltr = from a in LT.Cast<ObjectId>().ToList()
                //          let b = T.GetObject(a, OpenMode.ForRead) as LayerTableRecord
                //          where b.Name.StartsWith("Door", StringComparison.OrdinalIgnoreCase)
                //          select b;

                foreach (ObjectId Id in LT)
                {
                    LayerTableRecord LTR = null;
                    LTR = T.GetObject(Id, OpenMode.ForRead) as LayerTableRecord;

                    if (LTR.Name.StartsWith("Door", StringComparison.OrdinalIgnoreCase))
                    {
                        if (LTR.ObjectId != Db.Clayer)
                        {
                            T.GetObject(Id, OpenMode.ForWrite);

                            LTR.IsFrozen = true;
                        }
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.1.5 About Using the Dynamic Language Runtime
        /// Dynamic Language Runtime 사용법

        #endregion

        #endregion

        #region 3.2 Create Objects

        #region 3.2.1 Determine the Parent Objectt
        [CommandMethod("AccessSpace")]
        public void AccessSpace()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT = null;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR = null;

                PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nEnter which space to create the line in ";
                pKeyOpts.Keywords.Add("Model");
                pKeyOpts.Keywords.Add("Paper");
                pKeyOpts.Keywords.Add("Current");
                pKeyOpts.AllowNone = false;
                pKeyOpts.AppendKeywordsToMessage = true;

                PromptResult pKeyRes = Doc.Editor.GetKeywords(pKeyOpts);

                if (pKeyRes.StringResult == "Model")
                {
                    BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                }
                else if (pKeyRes.StringResult == "Paper")
                {
                    BTR = T.GetObject(BT[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;
                }
                else
                {
                    BTR = T.GetObject(Db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                }

                using (Line L = new Line(new Point3d(2, 5, 0), new Point3d(10, 7, 0)))
                {
                    BTR.AppendEntity(L);

                    T.AddNewlyCreatedDBObject(L, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.2.2 Create Lines

        #region 3.2.2.1 Create a Line Object
        [CommandMethod("AddLine")]
        public void AddLine()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Line L = new Line(new Point3d(5, 5, 0), new Point3d(12, 3, 0)))
                {
                    BTR.AppendEntity(L);

                    T.AddNewlyCreatedDBObject(L, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.2.2.2 Create a Polyline Object
        [CommandMethod("AddLightweightPolyline")]
        public void AddLightweightPolyline()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline Poly = new Polyline())
                {
                    Poly.AddVertexAt(0, new Point2d(2, 4), 0, 0, 0);
                    Poly.AddVertexAt(1, new Point2d(2, 2), 0, 0, 0);
                    Poly.AddVertexAt(2, new Point2d(6, 4), 0, 0, 0);

                    BTR.AppendEntity(Poly);
                    T.AddNewlyCreatedDBObject(Poly, true);
                }

                T.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.2.3 Create Curved Objects

        #region 3.2.3.1 Create a Circle Object
        [CommandMethod("AddCircle")]
        public void AddCircle()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle Circ = new Circle())
                {
                    Circ.Center = new Point3d(2, 3, 0);
                    Circ.Radius = 4.25;

                    BTR.AppendEntity(Circ);
                    T.AddNewlyCreatedDBObject(Circ, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.2.3.2 Create an Arc Object
        [CommandMethod("AddArc")]
        public void AddArc()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Arc acArc = new Arc(new Point3d(6.25, 9.125, 0), 6, 1.117, 3.5605))
                {
                    BTR.AppendEntity(acArc);
                    T.AddNewlyCreatedDBObject(acArc, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.2.3.3 Create a Spline Object
        [CommandMethod("AddSpline")]
        public void AddSpline()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point3dCollection ptColl = new Point3dCollection();
                ptColl.Add(new Point3d(0, 0, 0));
                ptColl.Add(new Point3d(5, 5, 0));
                ptColl.Add(new Point3d(10, 0, 0));

                Vector3d vecTan = new Point3d(0.5, 0.5, 0).GetAsVector();

                using (Spline acSpline = new Spline(ptColl, vecTan, vecTan, 4, 0.0))
                {
                    BTR.AppendEntity(acSpline);
                    T.AddNewlyCreatedDBObject(acSpline, true);
                }

                T.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.2.4 Create Point Objects
        [CommandMethod("AddPointAndSetPointStyle")]
        public void AddPointAndSetPointStyle()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (DBPoint acPoint = new DBPoint(new Point3d(4, 3, 0)))
                {
                    BTR.AppendEntity(acPoint);
                    T.AddNewlyCreatedDBObject(acPoint, true);
                }

                Db.Pdmode = 34;
                Db.Pdsize = 1;

                T.Commit();
            }
        }
        #endregion

        #region 3.2.5 Create Solid-Filled Areas
        [CommandMethod("Add2DSolid")]
        public void Add2DSolid()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Solid ac2DSolidBow = new Solid(new Point3d(0, 0, 0), new Point3d(5, 0, 0), new Point3d(5, 8, 0), new Point3d(0, 8, 0)))
                {
                    BTR.AppendEntity(ac2DSolidBow);
                    T.AddNewlyCreatedDBObject(ac2DSolidBow, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.2.6 Work With Regions

        #region 3.2.6.1 Create Regions
        [CommandMethod("AddRegion")]
        public void AddRegion()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle Circ = new Circle())
                {
                    Circ.Center = new Point3d(2, 2, 0);
                    Circ.Radius = 5;

                    DBObjectCollection acDBObjColl = new DBObjectCollection();
                    acDBObjColl.Add(Circ);

                    DBObjectCollection myRegionColl = new DBObjectCollection();
                    myRegionColl = Autodesk.AutoCAD.DatabaseServices.Region.CreateFromCurves(acDBObjColl);

                    var acRegion = myRegionColl[0] as Autodesk.AutoCAD.DatabaseServices.Region;

                    BTR.AppendEntity(acRegion);
                    T.AddNewlyCreatedDBObject(acRegion, true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.2.6.2 Create Composite Regions
        [CommandMethod("CreateCompositeRegions")]
        public void CreateCompositeRegions()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle Circ1 = new Circle())
                {
                    Circ1.Center = new Point3d(4, 4, 0);
                    Circ1.Radius = 2;

                    using (Circle Circ2 = new Circle())
                    {
                        Circ2.Center = new Point3d(4, 4, 0);
                        Circ2.Radius = 1;

                        DBObjectCollection acDBObjColl = new DBObjectCollection();
                        acDBObjColl.Add(Circ1);
                        acDBObjColl.Add(Circ2);

                        DBObjectCollection myRegionColl = new DBObjectCollection();
                        myRegionColl = Autodesk.AutoCAD.DatabaseServices.Region.CreateFromCurves(acDBObjColl);
                        var acRegion1 = myRegionColl[0] as Autodesk.AutoCAD.DatabaseServices.Region;
                        var acRegion2 = myRegionColl[1] as Autodesk.AutoCAD.DatabaseServices.Region;

                        if (acRegion1.Area > acRegion2.Area)
                        {
                            acRegion1.BooleanOperation(BooleanOperationType.BoolSubtract, acRegion2);
                            acRegion2.Dispose();

                            BTR.AppendEntity(acRegion1);
                            T.AddNewlyCreatedDBObject(acRegion1, true);
                        }
                        else
                        {
                            acRegion2.BooleanOperation(BooleanOperationType.BoolSubtract, acRegion1);
                            acRegion1.Dispose();

                            BTR.AppendEntity(acRegion2);
                            T.AddNewlyCreatedDBObject(acRegion2, true);
                        }
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.2.7 Create Hatches

        #region 3.2.7.1 Create a Hatch Object
        /// you specify the hatch pattern type, the hatch pattern name, and the associativity.
        /// Once a Hatch object has been created, you will not be able to change the hatch associativity. 
        #endregion

        #region 3.2.7.2 Associate the Hatch Pattern Type
        /// Associative hatches are linked to their boundaries and updated when the boundaries are modified. 
        /// Nonassociative hatches are independent of their boundaries. 
        /// 
        /// To make a hatch associative, set the Associative property of the hatch object created to TRUE. 
        /// To make a hatch nonassociative, set the Associative property to FALSE. 
        /// 
        #endregion

        #region 3.2.7.3 Assign the Hatch Pattern Type
        /// HatchPatternType.PreDefined
        /// Selects the pattern name from those defined in the acad.pat  or acadiso.pat files. 
        /// 
        /// HatchPatternType.UserDefined
        /// Defines a pattern of lines using the current linetype.
        /// 
        /// HatchPatternType.CustomDefined
        /// Selects the pattern name from a PAT other than the  acad.pat or  acadiso.pat files. 

        #endregion

        #region 3.2.7.4 Define the Hatch Boundaries
        [CommandMethod("AddHatch")]
        public void AddHatch()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle Circ = new Circle())
                {
                    Circ.Center = new Point3d(3, 3, 0);
                    Circ.Radius = 1;

                    BTR.AppendEntity(Circ);
                    T.AddNewlyCreatedDBObject(Circ, true);

                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    acObjIdColl.Add(Circ.ObjectId);

                    using (Hatch acHatch = new Hatch())
                    {
                        BTR.AppendEntity(acHatch);
                        T.AddNewlyCreatedDBObject(acHatch, true);

                        acHatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
                        acHatch.Associative = true;
                        acHatch.AppendLoop(HatchLoopTypes.Outermost, acObjIdColl);
                        acHatch.EvaluateHatch(true);
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #endregion

        #endregion

        #region 3.3 Work With Selection Sets

        #region 3.3.1 Obtain the PickFirst Selection Set
        [CommandMethod("CheckForPickfirstSelection", CommandFlags.UsePickSet)]
        public void CheckForPickfirstSelection()
        {
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult acPSR;
            acPSR = acDocEd.SelectImplied();

            SelectionSet acSSet;
            // 선택되어 있는 객체를 받음
            if (acPSR.Status == PromptStatus.OK)
            {
                acSSet = acPSR.Value;

                Application.ShowAlertDialog("Number of objects in Pickfirst selection: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects in pickfirst selection: 0");
            }

            ObjectId[] idArrayEmpty = new ObjectId[0];
            acDocEd.SetImpliedSelection(idArrayEmpty);

            acPSR = acDocEd.GetSelection();
            // 선택한 객체를 받음
            if (acPSR.Status == PromptStatus.OK)
            {
                acSSet = acPSR.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }

        }
        #endregion

        #region 3.3.2 Select Objects in the Drawing Area
        [CommandMethod("SelectObjectsOnscreen")]
        public void SelectObjectsOnscreen()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                PromptSelectionResult acSSPrompt = Doc.Editor.GetSelection();

                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPrompt.Value;

                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        if (acSSObj != null)
                        {
                            Entity acEnt = T.GetObject(acSSObj.ObjectId, OpenMode.ForWrite) as Entity;

                            if (acEnt != null)
                            {
                                acEnt.ColorIndex = 3;
                            }
                        }
                    }
                }

                T.Commit();
            }
        }

        [CommandMethod("SelectObjectsByCrossingWindow")]
        public void SelectObjectsByCrossingWindow()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.SelectCrossingWindow(new Point3d(2, 2, 0), new Point3d(10, 8, 0));

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }
        #endregion

        #region 3.3.3 Selection Set Keywords
        private void SelectionKeywordInputHandler(object sender, SelectionTextInputEventArgs eSelectionInput)
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            PromptSelectionResult acSSPrompt = null;
            SelectionSet acSSet = null;
            ObjectId[] acObjIds = null;

            switch (eSelectionInput.Input)
            {
                case "myFence":
                    Point3dCollection ptsFence = new Point3dCollection();
                    ptsFence.Add(new Point3d(5, 5, 0));
                    ptsFence.Add(new Point3d(13, 15, 0));
                    ptsFence.Add(new Point3d(12, 9, 0));
                    ptsFence.Add(new Point3d(5, 5, 0));
                    acSSPrompt = acDocEd.SelectFence(ptsFence);
                    break;

                case "myWindow":
                    acSSPrompt = acDocEd.SelectWindow(new Point3d(1, 1, 0), new Point3d(30, 20, 0));
                    break;

                case "myWPoly":
                    Point3dCollection ptsPolygon = new Point3dCollection();
                    ptsPolygon.Add(new Point3d(5, 5, 0));
                    ptsPolygon.Add(new Point3d(13, 15, 0));
                    ptsPolygon.Add(new Point3d(12, 9, 0));
                    ptsPolygon.Add(new Point3d(5, 5, 0));
                    acSSPrompt = acDocEd.SelectWindowPolygon(ptsPolygon);
                    break;

                case "myLastSel":
                    acSSPrompt = acDocEd.SelectLast();
                    break;

                case "myPrevSel":
                    acSSPrompt = acDocEd.SelectPrevious();
                    break;
            }

            if (acSSPrompt != null)
            {
                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    acSSet = acSSPrompt.Value;
                    acObjIds = acSSet.GetObjectIds();
                    eSelectionInput.AddObjects(acObjIds);
                }
            }
        }
        [CommandMethod("SelectionKeywordInput")]
        public void SelectionKeywordInput()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            PromptSelectionOptions acKeywordOpts = new PromptSelectionOptions();
            acKeywordOpts.Keywords.Add("myFence");
            acKeywordOpts.Keywords.Add("myWindow");
            acKeywordOpts.Keywords.Add("myWPoly");
            acKeywordOpts.Keywords.Add("myLastSel");
            acKeywordOpts.Keywords.Add("myPrevSel");

            acKeywordOpts.KeywordInput += new SelectionTextInputEventHandler(SelectionKeywordInputHandler);

            PromptSelectionResult acSSPrompt = acDocEd.GetSelection(acKeywordOpts);

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                ObjectId[] acObjIds = acSSet.GetObjectIds();
                Database Db = Doc.Database;

                using (Transaction T = Db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        foreach (ObjectId acObjId in acObjIds)
                        {
                            Entity acEnt = (Entity)T.GetObject(acObjId, OpenMode.ForWrite, true);
                            acDocEd.WriteMessage("\nObject selected: " + acEnt.GetType().FullName);
                        }
                    }
                    finally
                    {
                        T.Dispose();
                    }
                }
            }
        }
        #endregion

        #region 3.3.4 Add To or Merge Multiple Selection Sets
        [CommandMethod("MergeSelectionSets")]
        public void MergeSelectionSets()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            ObjectIdCollection acObjIdColl = new ObjectIdCollection();

            #region 선택 1
            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.GetSelection();    // 선택받기

            SelectionSet acSSet1;

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                acSSet1 = acSSPrompt.Value;
                acObjIdColl = new ObjectIdCollection(acSSet1.GetObjectIds());
            }
            #endregion

            #region 선택 2
            acSSPrompt = acDocEd.GetSelection();    // 선택받기

            SelectionSet acSSet2;

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                acSSet2 = acSSPrompt.Value;

                if (acObjIdColl.Count == 0)
                {
                    acObjIdColl = new ObjectIdCollection(acSSet2.GetObjectIds());
                }
                else
                {
                    foreach (ObjectId acObjId in acSSet2.GetObjectIds())
                    {
                        acObjIdColl.Add(acObjId);
                    }
                }
            }
            #endregion

            Application.ShowAlertDialog("Number of objects selected: " + acObjIdColl.Count.ToString());
        }
        #endregion

        #region 3.3.5 Define Rules for Selection Filters

        #endregion

        #region 3.3.6 Remove Objects From a Selection

        #region 3.3.6.1 Use Selection Filters to Define Selection Set Rules
        [CommandMethod("FilterSelectionSet")]
        public void FilterSelectionSet()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            TypedValue[] acTypValAr = new TypedValue[1];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "CIRCLE"), 0);

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.GetSelection(acSelFtr);

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }
        #endregion

        #region 3.3.6.2 Specify Multiple Criteria in a Selection Filter
        [CommandMethod("FilterBlueCircleOnLayer0")]
        public void FilterBlueCircleOnLayer0()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            TypedValue[] acTypValAr = new TypedValue[3];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Color, 5), 0);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "CIRCLE"), 1);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.LayerName, "0"), 2);

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.GetSelection(acSelFtr);

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }
        #endregion

        #region 3.3.6.3  Add Complexity to Your Filter List Conditions
        [CommandMethod("FilterRelational")]
        public void FilterRelational()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            TypedValue[] acTypValAr = new TypedValue[3];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "CIRCLE"), 0);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Operator, ">="), 1);
            acTypValAr.SetValue(new TypedValue(40, 5), 2);

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.GetSelection(acSelFtr);

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }
        #endregion

        #region 3.3.6.4 Use Wild-Card Patterns in Selection Set Filter Criteria
        [CommandMethod("FilterMtextWildcard")]
        public void FilterMtextWildcard()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            TypedValue[] acTypValAr = new TypedValue[2];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Text, "*The*"), 1);

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.GetSelection(acSelFtr);

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }
        #endregion

        #region 3.3.6.5 Filter for Extended Data
        [CommandMethod("FilterXdata")]
        public void FilterXdata()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = Doc.Editor;

            TypedValue[] acTypValAr = new TypedValue[2];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "Circle"), 0);
            acTypValAr.SetValue(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "MY_APP"), 1);

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDocEd.GetSelection(acSelFtr);

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;

                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }
        #endregion

        #endregion

        #endregion

        #region 3.4 Edit Named and 2D Objects

        #region 3.4.1 Work With Named Objects

        #region 3.4.1.1 Purge Unreferenced Named Objects
        [CommandMethod("PurgeUnreferencedLayers")]
        public void PurgeUnreferencedLayers()
        {
            // 사용되지 않은 레이어 제거(소거)
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                LayerTable LT;
                LT = T.GetObject(Db.LayerTableId, OpenMode.ForRead) as LayerTable;

                ObjectIdCollection acObjIdColl = new ObjectIdCollection();

                foreach (ObjectId acObjId in LT)
                {
                    acObjIdColl.Add(acObjId);
                }

                Db.Purge(acObjIdColl);

                foreach (ObjectId acObjId in acObjIdColl)
                {
                    SymbolTableRecord acSymTblRec;
                    acSymTblRec = T.GetObject(acObjId, OpenMode.ForWrite) as SymbolTableRecord;

                    try
                    {
                        acSymTblRec.Erase(true);
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                    {
                        Application.ShowAlertDialog("Error:\n" + Ex.Message);
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.4.1.2 Rename Objects
        [CommandMethod("RenameLayer")]
        public void RenameLayer()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                LayerTable LT;
                LT = T.GetObject(Db.LayerTableId, OpenMode.ForWrite) as LayerTable;

                // 레이어 복제하고 새로 생성
                LayerTableRecord LTR;
                LTR = T.GetObject(LT["0"], OpenMode.ForRead).Clone() as LayerTableRecord;
                // 레이어 이름 변경
                LTR.Name = "MyLayer";

                LT.Add(LTR);
                T.AddNewlyCreatedDBObject(LTR, true);

                T.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.4.2 Erase Objects
        [CommandMethod("EraseObject")]
        public void EraseObject()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(2, 4), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(4, 2), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(6, 4), 0, 0, 0);

                    BTR.AppendEntity(acPoly);
                    T.AddNewlyCreatedDBObject(acPoly, true);

                    Doc.Editor.Regen();
                    Application.ShowAlertDialog("Erase the newly added polyline");

                    acPoly.Erase(true);
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.4.3 Copy Objects

        #region 3.4.3.1 Copy an Object
        [CommandMethod("SingleCopy")]
        public void SingleCopy()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(2, 3, 0);
                    acCirc.Radius = 4.25;

                    BTR.AppendEntity(acCirc);
                    T.AddNewlyCreatedDBObject(acCirc, true);

                    Circle acCircClone = acCirc.Clone() as Circle;
                    acCirc.Radius = 1;

                    BTR.AppendEntity(acCircClone);
                    T.AddNewlyCreatedDBObject(acCircClone, true);
                }

                T.Commit();
            }
        }

        [CommandMethod("MultipleCopy")]
        public void MultipleCopy()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc1 = new Circle())
                {
                    acCirc1.Center = new Point3d(0, 0, 0);
                    acCirc1.Radius = 5;

                    BTR.AppendEntity(acCirc1);
                    T.AddNewlyCreatedDBObject(acCirc1, true);

                    using (Circle acCirc2 = new Circle())
                    {
                        acCirc2.Center = new Point3d(0, 0, 0);
                        acCirc2.Radius = 7;

                        BTR.AppendEntity(acCirc2);
                        T.AddNewlyCreatedDBObject(acCirc2, true);

                        DBObjectCollection acDBObjColl = new DBObjectCollection();
                        acDBObjColl.Add(acCirc1);
                        acDBObjColl.Add(acCirc2);

                        foreach (Entity acEnt in acDBObjColl)
                        {
                            Entity acEntClone;
                            acEntClone = acEnt.Clone() as Entity;
                            acEntClone.ColorIndex = 1;

                            acEntClone.TransformBy(Matrix3d.Displacement(new Vector3d(15, 0, 0)));

                            BTR.AppendEntity(acEntClone);
                            T.AddNewlyCreatedDBObject(acEntClone, true);
                        }
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.4.3.2 Copy Objects Between Databases
        [CommandMethod("CopyObjectsBetweenDatabases", CommandFlags.Session)]
        public void CopyObjectsBetweenDatabases()
        {
            // 다른 문서에 객체 복사해서 넣기
            ObjectIdCollection acObjIdColl = new ObjectIdCollection();

            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (DocumentLock acLckDocCur = Doc.LockDocument())
            {
                using (Transaction T = Db.TransactionManager.StartTransaction())
                {
                    BlockTable BT;
                    BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord BTR;
                    BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    using (Circle acCirc1 = new Circle())
                    {
                        acCirc1.Center = new Point3d(0, 0, 0);
                        acCirc1.Radius = 5;

                        BTR.AppendEntity(acCirc1);
                        T.AddNewlyCreatedDBObject(acCirc1, true);

                        using (Circle acCirc2 = new Circle())
                        {
                            acCirc2.Center = new Point3d(0, 0, 0);
                            acCirc2.Radius = 7;

                            BTR.AppendEntity(acCirc2);
                            T.AddNewlyCreatedDBObject(acCirc2, true);

                            acObjIdColl = new ObjectIdCollection();
                            acObjIdColl.Add(acCirc1.ObjectId);
                            acObjIdColl.Add(acCirc2.ObjectId);
                        }
                    }

                    T.Commit();
                }
            }

            string sLocalRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
            Application.ShowAlertDialog(sLocalRoot);

            string sTemplatePath = sLocalRoot + "Template\\acad.dwt";

            DocumentCollection DocMgr = Application.DocumentManager;
            Document NewDoc = DocMgr.Add(sTemplatePath);
            Database DbNewDoc = NewDoc.Database;

            using (DocumentLock LckDoc = NewDoc.LockDocument())
            {
                using (Transaction T = DbNewDoc.TransactionManager.StartTransaction())
                {
                    BlockTable BTNewDoc;
                    BTNewDoc = T.GetObject(DbNewDoc.BlockTableId, OpenMode.ForRead) as BlockTable;

                    BlockTableRecord BTRNewDoc;
                    BTRNewDoc = T.GetObject(BTNewDoc[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    IdMapping acIdMap = new IdMapping();
                    Db.WblockCloneObjects(acObjIdColl, BTRNewDoc.ObjectId, acIdMap, DuplicateRecordCloning.Ignore, false);

                    T.Commit();
                }
            }

            DocMgr.MdiActiveDocument = NewDoc;
        }
        #endregion

        #endregion

        #region 3.4.4 Offset Objects
        [CommandMethod("OffsetObject")]
        public void OffsetObject()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction T = Db.TransactionManager.StartTransaction())
            {
                BlockTable BT;
                BT = T.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR;
                BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(3, 2), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(4, 5), 0, 0, 0);
                    acPoly.AddVertexAt(5, new Point2d(4, 1), 0, 0, 0);

                    BTR.AppendEntity(acPoly);
                    T.AddNewlyCreatedDBObject(acPoly, true);

                    DBObjectCollection acDbObjColl = acPoly.GetOffsetCurves(0.25);

                    foreach (Entity acEnt in acDbObjColl)
                    {
                        BTR.AppendEntity(acEnt);
                        T.AddNewlyCreatedDBObject(acEnt, true);
                    }
                }

                T.Commit();
            }
        }
        #endregion

        #region 3.4.5 Transform Objects

        #region 3.4.5.1 Move Object
        [CommandMethod("MoveObject")]
        public void MoveObject()
        {
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Database Db = Doc.Database;

            using (Transaction acTrans = Db.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(Db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(2, 2, 0);
                    acCirc.Radius = 0.5;

                    Point3d acPt3d = new Point3d(0, 0, 0);
                    Vector3d acVec3d = acPt3d.GetVectorTo(new Point3d(2, 0, 0));

                    acCirc.TransformBy(Matrix3d.Displacement(acVec3d));

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.5.2 Rotate Object
        [CommandMethod("RotateObject")]
        public void RotateObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(1, 2), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(1, 3), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(2, 3), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(3, 3), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(4, 4), 0, 0, 0);
                    acPoly.AddVertexAt(5, new Point2d(4, 2), 0, 0, 0);

                    acPoly.Closed = true;

                    Matrix3d curUCSMatrix = acDoc.Editor.CurrentUserCoordinateSystem;
                    CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;

                    acPoly.TransformBy(Matrix3d.Rotation(0.7854, curUCS.Zaxis, new Point3d(4, 4.25, 0)));

                    acBlkTblRec.AppendEntity(acPoly);
                    acTrans.AddNewlyCreatedDBObject(acPoly, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.5.3 Mirror Object
        [CommandMethod("MirrorObject")]
        public void MirrorObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(3, 2), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(4, 4), 0, 0, 0);
                    acPoly.AddVertexAt(5, new Point2d(4, 1), 0, 0, 0);

                    acPoly.SetBulgeAt(1, -2);

                    acPoly.Closed = true;

                    acBlkTblRec.AppendEntity(acPoly);
                    acTrans.AddNewlyCreatedDBObject(acPoly, true);

                    Polyline acPolyMirCopy = acPoly.Clone() as Polyline;
                    acPolyMirCopy.ColorIndex = 5;

                    Point3d acPtFrom = new Point3d(0, 4.25, 0);
                    Point3d acPtTo = new Point3d(4, 4.25, 0);
                    Line3d acLine3d = new Line3d(acPtFrom, acPtTo);

                    acPolyMirCopy.TransformBy(Matrix3d.Mirroring(acLine3d));

                    acBlkTblRec.AppendEntity(acPolyMirCopy);
                    acTrans.AddNewlyCreatedDBObject(acPolyMirCopy, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.5.4 Scale Object
        [CommandMethod("ScaleObject")]
        public void ScaleObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(1, 2), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(1, 3), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(2, 3), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(3, 3), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(4, 4), 0, 0, 0);
                    acPoly.AddVertexAt(5, new Point2d(4, 2), 0, 0, 0);

                    acPoly.Closed = true;

                    acBlkTblRec.AppendEntity(acPoly);
                    acTrans.AddNewlyCreatedDBObject(acPoly, true);

                    var acPolyClone = acPoly.Clone() as Polyline;

                    acPolyClone.TransformBy(Matrix3d.Scaling(0.5, new Point3d(4, 4.25, 0)));

                    acBlkTblRec.AppendEntity(acPolyClone);
                    acTrans.AddNewlyCreatedDBObject(acPolyClone, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.4.6 Array Objects

        static Point2d PolarPoints(Point2d pPt, double dAng, double dDist)
        {
            return new Point2d(pPt.X + dDist * Math.Cos(dAng), pPt.Y + dDist * Math.Sin(dAng));
        }

        #region 3.4.6.1 Create Polar Arrays

        [CommandMethod("PolarArrayObject")]
        public void PolarArrayObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(2, 2, 0);
                    acCirc.Radius = 1;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);

                    int nCount = 1;

                    // Set a value in radians for 60 degrees
                    double dAng = 1.0472;

                    Point2d acPt2dArrayBase = new Point2d(4, 4);

                    while (nCount < 6)
                    {
                        Entity acEntClone = acCirc.Clone() as Entity;

                        Extents3d acExts;
                        Point2d acPtObjBase;

                        Circle acCircArrObj = acEntClone as Circle;

                        if (acCircArrObj != null)
                        {
                            acPtObjBase = new Point2d(acCircArrObj.Center.X, acCircArrObj.Center.Y);
                        }
                        else
                        {
                            acExts = acEntClone.Bounds.GetValueOrDefault();
                            acPtObjBase = new Point2d(acExts.MinPoint.X, acExts.MaxPoint.Y);
                        }

                        double dDist = acPt2dArrayBase.GetDistanceTo(acPtObjBase);
                        double dAngFromX = acPt2dArrayBase.GetVectorTo(acPtObjBase).Angle;

                        Point2d acPt2dTo = PolarPoints(acPt2dArrayBase, (nCount * dAng) + dAngFromX, dDist);

                        Vector2d acVec2d = acPtObjBase.GetVectorTo(acPt2dTo);
                        Vector3d acVec3d = new Vector3d(acVec2d.X, acVec2d.Y, 0);
                        acEntClone.TransformBy(Matrix3d.Displacement(acVec3d));

                        /*
                        // The following code demonstrates how to rotate each object like
                        // the ARRAY command does.
                        acExts = acEntClone.Bounds.GetValueOrDefault();
                        acPtObjBase = new Point2d(acExts.MinPoint.X,
                                                    acExts.MaxPoint.Y);

                        // Rotate the cloned entity around its upper-left extents point
                        Matrix3d curUCSMatrix = acDoc.Editor.CurrentUserCoordinateSystem;
                        CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;
                        acEntClone.TransformBy(Matrix3d.Rotation(nCount * dAng,
                                                                    curUCS.Zaxis,
                                                                    new Point3d(acPtObjBase.X,
                                                                                acPtObjBase.Y, 0)));
                        */

                        acBlkTblRec.AppendEntity(acEntClone);
                        acTrans.AddNewlyCreatedDBObject(acEntClone, true);

                        nCount = nCount + 1;
                    }
                }

                acTrans.Commit();
            }
        }

        #endregion

        #region 3.4.6.2 Create Rectangular Arrays

        [CommandMethod("RectangularArrayObject")]
        public void RectangularArrayObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc = new Circle())
                {
                    #region 원 생성
                    acCirc.Center = new Point3d(2, 2, 0);
                    acCirc.Radius = 0.5;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                    #endregion

                    #region 배열 기준표
                    int nRows = 5;
                    int nColumns = 5;

                    double dRowOffset = 1;
                    double dColumnOffset = 1;
                    double dArrayAng = 0;
                    #endregion

                    Matrix3d curUCSMatrix = acDoc.Editor.CurrentUserCoordinateSystem;
                    CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;
                    Vector2d acVec2dAng = new Vector2d(curUCS.Xaxis.X, curUCS.Xaxis.Y);

                    dArrayAng = dArrayAng + acVec2dAng.Angle;

                    Extents3d acExts = acCirc.Bounds.GetValueOrDefault();
                    Point2d acPt2dArrayBase = new Point2d(acExts.MinPoint.X, acExts.MaxPoint.Y);

                    DBObjectCollection acDBObjCollCols = new DBObjectCollection();
                    acDBObjCollCols.Add(acCirc);

                    // 열 입력
                    int nColumnsCount = 1;
                    while (nColumns > nColumnsCount)
                    {
                        Entity acEntClone = acCirc.Clone() as Entity;
                        acDBObjCollCols.Add(acEntClone);

                        Point2d acPt2dTo = PolarPoints(acPt2dArrayBase, dArrayAng, dColumnOffset * nColumnsCount);

                        Vector2d acVec2d = acPt2dArrayBase.GetVectorTo(acPt2dTo);
                        Vector3d acVec3d = new Vector3d(acVec2d.X, acVec2d.Y, 0);
                        acEntClone.TransformBy(Matrix3d.Displacement(acVec3d));

                        acBlkTblRec.AppendEntity(acEntClone);
                        acTrans.AddNewlyCreatedDBObject(acEntClone, true);

                        nColumnsCount = nColumnsCount + 1;
                    }

                    double dAng = Math.PI / 2;

                    DBObjectCollection acDBObjCollLvls = new DBObjectCollection();

                    foreach (DBObject acObj in acDBObjCollCols)
                    {
                        acDBObjCollLvls.Add(acObj);
                    }

                    foreach (Entity acEnt in acDBObjCollCols)
                    {
                        int nRowsCount = 1;

                        // 행 입력
                        while (nRows > nRowsCount)
                        {
                            Entity acEntClone = acEnt.Clone() as Entity;
                            acDBObjCollLvls.Add(acEntClone);

                            Point2d acPt2dTo = PolarPoints(acPt2dArrayBase, dArrayAng + dAng, dRowOffset * nRowsCount);

                            Vector2d acVec2d = acPt2dArrayBase.GetVectorTo(acPt2dTo);
                            Vector3d acVec3d = new Vector3d(acVec2d.X, acVec2d.Y, 0);
                            acEntClone.TransformBy(Matrix3d.Displacement(acVec3d));

                            acBlkTblRec.AppendEntity(acEntClone);
                            acTrans.AddNewlyCreatedDBObject(acEntClone, true);

                            nRowsCount = nRowsCount + 1;
                        }
                    }
                }

                acTrans.Commit();
            }
        }

        #endregion

        #endregion

        #region 3.4.7 Extend and Trim Objects
        [CommandMethod("ExtendObject")]
        public void ExtendObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Line acLine = new Line(new Point3d(4, 4, 0), new Point3d(7, 7, 0)))
                {
                    acBlkTblRec.AppendEntity(acLine);
                    acTrans.AddNewlyCreatedDBObject(acLine, true);

                    acDoc.Editor.Regen();

                    Application.ShowAlertDialog("Before extend");
                    // acLine.Delta => (3, 3, 0)
                    acLine.EndPoint = acLine.EndPoint + acLine.Delta;

                    //acTrans.Commit();
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.8 Explode Objects
        [CommandMethod("ExplodeObject")]
        public void ExplodeObject()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(3, 2), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(4, 4), 0, 0, 0);
                    acPoly.AddVertexAt(5, new Point2d(4, 1), 0, 0, 0);
                    acPoly.Closed = true;

                    acPoly.SetBulgeAt(3, -0.5);

                    DBObjectCollection acDBObjColl = new DBObjectCollection();
                    acPoly.Explode(acDBObjColl);

                    string s = "";

                    foreach (Entity acEnt in acDBObjColl)
                    {
                        if (acEnt is Line)
                        {
                            Line acLine = acEnt as Line;
                            s += acLine.StartPoint.ToString() + ", " + acLine.EndPoint.ToString() + "\n";
                        }

                        acBlkTblRec.AppendEntity(acEnt);
                        acTrans.AddNewlyCreatedDBObject(acEnt, true);
                    }
                    //Application.ShowAlertDialog(s);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.9 Edit Polylines
        [CommandMethod("EditPolyline")]
        public void EditPolyline()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Polyline acPoly = new Polyline())
                {
                    acPoly.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(3, 2), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(4, 4), 0, 0, 0);

                    acBlkTblRec.AppendEntity(acPoly);
                    acTrans.AddNewlyCreatedDBObject(acPoly, true);

                    acPoly.SetBulgeAt(3, -0.5);

                    acPoly.AddVertexAt(5, new Point2d(4, 1), 0, 0, 0);

                    acPoly.SetStartWidthAt(4, 0.1);
                    acPoly.SetEndWidthAt(4, 0.5);

                    acPoly.Closed = true;
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.10 Edit Splines
        [CommandMethod("EditSpline")]
        public void EditSpline()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point3dCollection acPt3dColl = new Point3dCollection();
                acPt3dColl.Add(new Point3d(1, 1, 0));
                acPt3dColl.Add(new Point3d(5, 5, 0));
                acPt3dColl.Add(new Point3d(10, 0, 0));

                Vector3d acStartTan = new Vector3d(0.5, 0.5, 0);
                Vector3d acEndTan = new Vector3d(0.5, 0.5, 0);

                using (Spline acSpline = new Spline(acPt3dColl, acStartTan, acEndTan, 4, 0))
                {
                    acSpline.SetControlPointAt(0, new Point3d(0, 3, 0));

                    acBlkTblRec.AppendEntity(acSpline);
                    acTrans.AddNewlyCreatedDBObject(acSpline, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.11 Edit Hatches

        #region 3.4.11.1 Edit Hatches
        [CommandMethod("EditHatchAppendLoop")]
        public void EditHatchAppendLoop()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Arc acArc = new Arc(new Point3d(5, 3, 0), 3, 0, 3.141592))
                {
                    acBlkTblRec.AppendEntity(acArc);
                    acTrans.AddNewlyCreatedDBObject(acArc, true);

                    using (Line acLine = new Line(acArc.StartPoint, acArc.EndPoint))
                    {
                        acBlkTblRec.AppendEntity(acLine);
                        acTrans.AddNewlyCreatedDBObject(acLine, true);

                        ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                        acObjIdColl.Add(acArc.ObjectId);
                        acObjIdColl.Add(acLine.ObjectId);

                        using (Hatch acHatch = new Hatch())
                        {
                            acBlkTblRec.AppendEntity(acHatch);
                            acTrans.AddNewlyCreatedDBObject(acHatch, true);

                            // Set the properties of the hatch object
                            // Associative must be set after the hatch object is appended to the 
                            // block table record and before AppendLoop
                            acHatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
                            acHatch.Associative = true;
                            acHatch.AppendLoop(HatchLoopTypes.Outermost, acObjIdColl);

                            // Create a circle object for the inner boundary of the hatch
                            using (Circle acCirc = new Circle())
                            {
                                acCirc.Center = new Point3d(5, 4.5, 0);
                                acCirc.Radius = 1;

                                acBlkTblRec.AppendEntity(acCirc);
                                acTrans.AddNewlyCreatedDBObject(acCirc, true);

                                // Adds the circle to an object id collection
                                acObjIdColl.Clear();
                                acObjIdColl.Add(acCirc.ObjectId);

                                // Append the circle as the inner loop of the hatch and evaluate it
                                acHatch.AppendLoop(HatchLoopTypes.Default, acObjIdColl);
                                acHatch.EvaluateHatch(true);
                            }
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.4.11.2 Edit Hatch Patterns
        [CommandMethod("EditHatchPatternScale")]
        public void EditHatchPatternScale()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(5, 3, 0);
                    acCirc.Radius = 3;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);

                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    acObjIdColl.Add(acCirc.ObjectId);

                    using (Hatch acHatch = new Hatch())
                    {
                        acBlkTblRec.AppendEntity(acHatch);
                        acTrans.AddNewlyCreatedDBObject(acHatch, true);

                        // Set the properties of the hatch object
                        // Associative must be set after the hatch object is appended to the 
                        // block table record and before AppendLoop
                        acHatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
                        acHatch.Associative = true;
                        acHatch.AppendLoop(HatchLoopTypes.Outermost, acObjIdColl);

                        // Evaluate the hatch
                        acHatch.EvaluateHatch(true);

                        // Increase the pattern scale by 2 and re-evaluate the hatch
                        acHatch.PatternScale = acHatch.PatternScale + 2;
                        acHatch.SetHatchPattern(acHatch.PatternType, acHatch.PatternName);
                        acHatch.EvaluateHatch(true);
                    }
                }

                // Save the new object to the database
                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #endregion

        #region 3.5 Use Layers, Colors, and Linetypes

        #region 3.5.1 Work With Layers

        #region 3.5.1.1 Sort Layers and Linetypes
        [CommandMethod("DisplayLayerNames")]
        public void DisplayLayerNames()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                string sLayerNames = "";

                foreach (ObjectId acObjId in acLyrTbl)
                {
                    LayerTableRecord acLyrTblRec;
                    acLyrTblRec = acTrans.GetObject(acObjId, OpenMode.ForRead) as LayerTableRecord;

                    sLayerNames = sLayerNames + "\n" + acLyrTblRec.Name;
                }

                Application.ShowAlertDialog("The layers in this drawing are: " + sLayerNames);
            }
        }
        #endregion

        #region 3.5.1.2 Create and Name Layers
        [CommandMethod("CreateAndAssignALayer")]
        public void CreateAndAssignALayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                PromptStringOptions pStrOpts = new PromptStringOptions("\nEnter layer Name: ");
                pStrOpts.AllowSpaces = true;
                PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);

                string sLayerName = pStrRes.StringResult;

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 3);
                        acLyrTblRec.Name = sLayerName;

                        acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                }

                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(2, 2, 0);
                    acCirc.Radius = 1;
                    acCirc.Layer = sLayerName;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.1.3 Make a Layer Current
        [CommandMethod("SetLayerCurrent")]
        public void SetLayerCurrent()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                PromptKeywordOptions acPrmpKeyOpt = new PromptKeywordOptions("\nSelect Layer to set current: ");
                acPrmpKeyOpt.AppendKeywordsToMessage = true;

                string s = "";

                foreach (ObjectId acLyrId in acLyrTbl)
                {
                    LayerTableRecord LTR = acTrans.GetObject(acLyrId, OpenMode.ForRead) as LayerTableRecord;

                    acPrmpKeyOpt.Keywords.Add(LTR.Name);

                    s += LTR.Name + "\n";
                }

                //Application.ShowAlertDialog(s);

                PromptResult pKeyRes = acDoc.Editor.GetKeywords(acPrmpKeyOpt);

                //Application.ShowAlertDialog(pKeyRes.StringResult);

                string sLayerName = pKeyRes.StringResult;

                if (acLyrTbl.Has(sLayerName) == true)
                {
                    // current layer
                    acCurDb.Clayer = acLyrTbl[sLayerName];

                    acTrans.Commit();
                }
            }
        }
        #endregion

        #region 3.5.1.4 Turn Layers On and Off
        [CommandMethod("TurnLayerOff")]
        public void TurnLayerOff()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                string sLayerName = "ABC";

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        // Assign the layer a name
                        acLyrTblRec.Name = sLayerName;

                        // Turn the layer off
                        acLyrTblRec.IsOff = true;

                        // Upgrade the Layer table for write
                        acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                        // Append the new layer to the Layer table and the transaction
                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                }
                else
                {
                    LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName], OpenMode.ForWrite) as LayerTableRecord;

                    // Turn the layer off
                    acLyrTblRec.IsOff = true;
                }

                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a circle object
                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(2, 2, 0);
                    acCirc.Radius = 1;
                    acCirc.Layer = sLayerName;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.1.5 Freeze and Thaw Layers
        [CommandMethod("FreezeLayer")]
        public void FreezeLayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                string sLayerName = "ABC";

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        // Assign the layer a name
                        acLyrTblRec.Name = sLayerName;

                        // Freeze the layer
                        acLyrTblRec.IsFrozen = true;

                        // Upgrade the Layer table for write
                        acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                        // Append the new layer to the Layer table and the transaction
                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                }
                else
                {
                    LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName], OpenMode.ForWrite) as LayerTableRecord;

                    // Freeze the layer
                    acLyrTblRec.IsFrozen = true;
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.1.6 Lock and Unlock Layers
        [CommandMethod("LockLayer")]
        public void LockLayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                string sLayerName = "ABC";

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        // Assign the layer a name
                        acLyrTblRec.Name = sLayerName;

                        // Lock the layer
                        acLyrTblRec.IsLocked = true;

                        // Upgrade the Layer table for write
                        acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                        // Append the new layer to the Layer table and the transaction
                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                }
                else
                {
                    LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName], OpenMode.ForWrite) as LayerTableRecord;

                    // Lock the layer
                    acLyrTblRec.IsLocked = true;
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.1.7 Assign Color to a Layer
        [CommandMethod("SetLayerColor")]
        public void SetLayerColor()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                // Define an array of layer names
                string[] sLayerNames = new string[3];
                sLayerNames[0] = "ACIRed";
                sLayerNames[1] = "TrueBlue";
                sLayerNames[2] = "ColorBookYellow";

                // Define an array of colors for the layers
                Color[] acColors = new Color[3];
                acColors[0] = Color.FromColorIndex(ColorMethod.ByAci, 1);
                acColors[1] = Color.FromRgb(23, 54, 232);
                acColors[2] = Color.FromNames("PANTONE Yellow 0131 C", "PANTONE+ Pastels & Neons Coated");

                int nCnt = 0;

                // Add or change each layer in the drawing
                foreach (string sLayerName in sLayerNames)
                {
                    if (acLyrTbl.Has(sLayerName) == false)
                    {
                        using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                        {
                            // Assign the layer a name
                            acLyrTblRec.Name = sLayerName;

                            // Set the color of the layer
                            acLyrTblRec.Color = acColors[nCnt];

                            // Upgrade the Layer table for write
                            if (acLyrTbl.IsWriteEnabled == false) acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                            // Append the new layer to the Layer table and the transaction
                            acLyrTbl.Add(acLyrTblRec);
                            acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                        }
                    }
                    else
                    {
                        // Open the layer if it already exists for write
                        LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName], OpenMode.ForWrite) as LayerTableRecord;

                        // Set the color of the layer
                        acLyrTblRec.Color = acColors[nCnt];
                    }

                    nCnt = nCnt + 1;
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.1.8 Assign a Linetype to a Layer
        [CommandMethod("SetLayerLinetype")]
        public void SetLayerLinetype()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                string sLayerName = "ABC";

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        acLyrTblRec.Name = sLayerName;

                        LinetypeTable acLinTbl;
                        acLinTbl = acTrans.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                        if (acLinTbl.Has("Center") == true)
                        {
                            // Set the linetype for the layer
                            acLyrTblRec.LinetypeObjectId = acLinTbl["Center"];
                        }

                        // Upgrade the Layer table for write
                        acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                        // Append the new layer to the Layer table and the transaction
                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                }
                else
                {
                    LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLayerName], OpenMode.ForRead) as LayerTableRecord;

                    // Open the Layer table for read
                    LinetypeTable acLinTbl;
                    acLinTbl = acTrans.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                    if (acLinTbl.Has("Center") == true)
                    {
                        // Upgrade the Layer Table Record for write
                        acTrans.GetObject(acLyrTbl[sLayerName], OpenMode.ForWrite);

                        // Set the linetype for the layer
                        acLyrTblRec.LinetypeObjectId = acLinTbl["Center"];
                    }
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.1.9 Erase Layers
        [CommandMethod("EraseLayer")]
        public void EraseLayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                string sLayerName = "ABC";

                if (acLyrTbl.Has(sLayerName) == true)
                {
                    // Check to see if it is safe to erase layer
                    ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                    acObjIdColl.Add(acLyrTbl[sLayerName]);
                    acCurDb.Purge(acObjIdColl);

                    if (acObjIdColl.Count > 0)
                    {
                        LayerTableRecord acLyrTblRec;
                        acLyrTblRec = acTrans.GetObject(acObjIdColl[0], OpenMode.ForWrite) as LayerTableRecord;

                        try
                        {
                            // Erase the unreferenced layer
                            acLyrTblRec.Erase(true);

                            // Save the changes and dispose of the transaction
                            acTrans.Commit();
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                        {
                            // Layer could not be deleted
                            Application.ShowAlertDialog("Error:\n" + Ex.Message);
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        #region 3.5.2 Work With Colors

        #region 3.5.2.1 Assign a Color Value to an Object
        [CommandMethod("SetObjectColor")]
        public static void SetObjectColor()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                Color[] acColors = new Color[3];
                acColors[0] = Color.FromColorIndex(ColorMethod.ByAci, 1);
                acColors[1] = Color.FromRgb(23, 54, 232);
                acColors[2] = Color.FromRgb(23, 54, 232);

                //acColors[2] = Color.FromNames("PANTONE Yellow 0131 C", "PANTONE(R) pastel coated");

                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a circle object and assign it the ACI value of 4
                Point3d acPt = new Point3d(0, 3, 0);
                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = acPt;
                    acCirc.Radius = 1;
                    acCirc.ColorIndex = 4;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);

                    int nCnt = 0;

                    while (nCnt < 3)
                    {
                        // Create a copy of the circle
                        Circle acCircCopy;
                        acCircCopy = acCirc.Clone() as Circle;

                        // Shift the copy along the Y-axis
                        acPt = new Point3d(acPt.X, acPt.Y + 3, acPt.Z);
                        acCircCopy.Center = acPt;

                        // Assign the new color to the circle
                        acCircCopy.Color = acColors[nCnt];

                        acBlkTblRec.AppendEntity(acCircCopy);
                        acTrans.AddNewlyCreatedDBObject(acCircCopy, true);

                        nCnt = nCnt + 1;
                    }
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.2.2 Make a Color Current Through the Database
        [CommandMethod("SetColorCurrent")]
        public void SetColorCurrent()
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            // Set the current color
            acDoc.Database.Cecolor = Color.FromColorIndex(ColorMethod.ByLayer, 256);
        }
        #endregion

        #region 3.5.2.3 Make a Color Current With the CECOLOR System Variable
        public void SetCurrentColor()
        {
            Application.SetSystemVariable("CECOLOR", "1");
        }
        #endregion

        #endregion

        #region 3.5.3 Work With Linetypes

        #region 3.5.3.1 Make a Linetype Active

        #region 3.5.3.1.1 Assign a Linetype to an Object 
        [CommandMethod("SetObjectLinetype")]
        public void SetObjectLinetype()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LinetypeTable acLineTypTbl;
                acLineTypTbl = acTrans.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                string sLineTypName = "Center";

                if (acLineTypTbl.Has(sLineTypName) == false)
                {
                    acCurDb.LoadLineTypeFile(sLineTypName, "acad.lin");
                }

                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a circle object
                using (Circle acCirc = new Circle())
                {
                    acCirc.Center = new Point3d(2, 2, 0);
                    acCirc.Radius = 1;
                    acCirc.Linetype = sLineTypName;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.3.1.2 Make a Linetype Current Through the Database
        [CommandMethod("SetLinetypeCurrent")]
        public void SetLinetypeCurrent()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LinetypeTable acLineTypTbl;
                acLineTypTbl = acTrans.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                string sLineTypName = "Center";

                if (acLineTypTbl.Has(sLineTypName) == true)
                {
                    // Set the linetype Center current
                    acCurDb.Celtype = acLineTypTbl[sLineTypName];

                    // Save the changes
                    acTrans.Commit();
                }
            }
        }
        #endregion

        #region 3.5.3.1.3 Make a Linetype Current With the CELTYPE System Variable
        public void SetLinetype()
        {
            Application.SetSystemVariable("CELTYPE", "Center"); ;
        }
        #endregion

        #endregion

        #region 3.5.3.2 Rename Linetypes

        #endregion

        #region 3.5.3.3 Delete Linetypes

        #endregion

        #region 3.5.3.4 Change Linetype Descriptions
        [CommandMethod("ChangeLinetypeDescription")]
        public void ChangeLinetypeDescription()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LinetypeTableRecord acLineTypTblRec;
                acLineTypTblRec = acTrans.GetObject(acCurDb.Celtype, OpenMode.ForWrite) as LinetypeTableRecord;

                // Change the description of the current linetype
                acLineTypTblRec.AsciiDescription = "Exterior Wall";

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.5.3.5 Specify Linetype Scale
        [CommandMethod("SetObjectLinetypeScale")]
        public void SetObjectLinetypeScale()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Save the current linetype
                ObjectId acObjId = acCurDb.Celtype;

                // Set the global linetype scale
                acCurDb.Ltscale = 3;

                LinetypeTable acLineTypTbl;
                acLineTypTbl = acTrans.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                string sLineTypName = "Border";

                if (acLineTypTbl.Has(sLineTypName) == false)
                {
                    acCurDb.LoadLineTypeFile(sLineTypName, "acad.lin");
                }

                // Set the Border linetype current
                acCurDb.Celtype = acLineTypTbl[sLineTypName];

                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a circle object and set its linetype
                // scale to half of full size
                using (Circle acCirc1 = new Circle())
                {
                    acCirc1.Center = new Point3d(2, 2, 0);
                    acCirc1.Radius = 4;
                    acCirc1.LinetypeScale = 0.5;

                    acBlkTblRec.AppendEntity(acCirc1);
                    acTrans.AddNewlyCreatedDBObject(acCirc1, true);

                    // Create a second circle object
                    using (Circle acCirc2 = new Circle())
                    {
                        acCirc2.Center = new Point3d(12, 2, 0);
                        acCirc2.Radius = 4;

                        acBlkTblRec.AppendEntity(acCirc2);
                        acTrans.AddNewlyCreatedDBObject(acCirc2, true);
                    }
                }

                // Restore the original active linetype
                acCurDb.Celtype = acObjId;

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #endregion

        #region 3.6 Save and Restore Layer States

        #region 3.6.1 Understand How AutoCAD Saves Layer States
        [CommandMethod("ListLayerStates")]
        public void ListLayerStates()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                LayerStateManager acLyrStMan;
                acLyrStMan = acCurDb.LayerStateManager;

                DBDictionary acDbDict;
                acDbDict = acTrans.GetObject(acLyrStMan.LayerStatesDictionaryId(true), OpenMode.ForRead) as DBDictionary;

                string sLayerStateNames = "";

                foreach (DBDictionaryEntry acDbDictEnt in acDbDict)
                {
                    sLayerStateNames = sLayerStateNames + "\n" + acDbDictEnt.Key;
                }

                Application.ShowAlertDialog("The saved layer settings in this drawing are:" + sLayerStateNames);
            }
        }
        #endregion

        #region 3.6.2 Use the LayerStateManager to Manage Layer States

        /// <summary>
        /// 레이어 상태 정보를 통째로 저장
        /// </summary>
        #region 3.6.2.1 Save Layer States
        [CommandMethod("SaveLayerColorAndLinetype")]
        public void SaveLayerColorAndLinetype()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            string sLyrStName = "ColorLinetype";

            if (acLyrStMan.HasLayerState(sLyrStName) == false)
            {
                acLyrStMan.SaveLayerState(sLyrStName,
                                          LayerStateMasks.Color |
                                          LayerStateMasks.LineType,
                                          ObjectId.Null);
            }
        }
        #endregion

        #region 3.6.2.2 Rename Layer States
        [CommandMethod("RenameLayerState")]
        public void RenameLayerState()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            string sLyrStName = "ColorLinetype";
            string sLyrStNewName = "OldColorLinetype";

            if (acLyrStMan.HasLayerState(sLyrStName) == true &&
                acLyrStMan.HasLayerState(sLyrStNewName) == false)
            {
                acLyrStMan.RenameLayerState(sLyrStName, sLyrStNewName);
            }
        }
        #endregion

        #region 3.6.2.3 Delete Layer States
        [CommandMethod("RemoveLayerState")]
        public void RemoveLayerState()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            string sLyrStName = "ColorLinetype";

            if (acLyrStMan.HasLayerState(sLyrStName) == true)
            {
                acLyrStMan.DeleteLayerState(sLyrStName);
            }
        }
        #endregion

        #region 3.6.2.4 Restore Layer States
        [CommandMethod("RestoreLayerState")]
        public void RestoreLayerState()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            string sLyrStName = "ColorLinetype";

            if (acLyrStMan.HasLayerState(sLyrStName) == true)
            {
                acLyrStMan.RestoreLayerState(sLyrStName,
                                             ObjectId.Null,
                                             1,
                                             LayerStateMasks.Color |
                                             LayerStateMasks.LineType);
            }
        }
        #endregion

        #region 3.6.2.5 Export and Import Saved Layer States
        [CommandMethod("ExportLayerState")]
        public void ExportLayerState()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            string sLyrStName = "ColorLinetype";

            if (acLyrStMan.HasLayerState(sLyrStName) == true)
            {
                acLyrStMan.ExportLayerState(sLyrStName, "c:\\my documents\\" + sLyrStName + ".las");
            }
        }

        [CommandMethod("ImportLayerState")]
        public void ImportLayerState()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            string sLyrStFileName = "c:\\my documents\\ColorLinetype.las";

            if (System.IO.File.Exists(sLyrStFileName))
            {
                try
                {
                    acLyrStMan.ImportLayerState(sLyrStFileName);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Application.ShowAlertDialog(ex.Message);
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region 3.7 Add Text to Drawings

        #region 3.7.1 Use Multiline Text

        #region 3.7.1.1 Create Multiline Text
        [CommandMethod("CreateMText")]
        public void CreateMText()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a multiline text object
                using (MText acMText = new MText())
                {
                    acMText.Location = new Point3d(2, 2, 0);
                    acMText.Width = 4;
                    // Width 범위를 초과하면 다음줄로 넘어감
                    acMText.Contents = "This is a text string for the MText object.";

                    acBlkTblRec.AppendEntity(acMText);
                    acTrans.AddNewlyCreatedDBObject(acMText, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.7.1.2 Format Multiline Text
        [CommandMethod("FormatMText")]
        public void FormatMText()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a multiline text object
                using (MText acMText = new MText())
                {
                    acMText.Location = new Point3d(2, 2, 0);
                    acMText.Width = 4.5;
                    // \\으로 문자 구분
                    acMText.Contents = "{{\\H1.5x; Big text}\\A2; over text\\A1;/\\A0;under text}";

                    acBlkTblRec.AppendEntity(acMText);
                    acTrans.AddNewlyCreatedDBObject(acMText, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #region 3.7.2 Use Single-Line Text

        #region 3.7.2.1 Create Single-Line Text
        [CommandMethod("CreateText")]
        public void CreateText()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a single-line text object
                using (DBText acText = new DBText())
                {
                    acText.Position = new Point3d(2, 2, 0);
                    acText.Height = 0.5;
                    acText.TextString = "Hello, World.";

                    acBlkTblRec.AppendEntity(acText);
                    acTrans.AddNewlyCreatedDBObject(acText, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.7.2.2 Change Single-Line Text

        #endregion

        #region 3.7.2.3 Set Text Height

        #endregion

        #region 3.7.2.4 Format Single-Line Text

        #region 3.7.2.4.1 Set Obliquing Angle
        [CommandMethod("ObliqueText")]
        public void ObliqueText()
        {
            // 글자 기울이기
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a single-line text object
                using (DBText acText = new DBText())
                {
                    acText.Position = new Point3d(3, 3, 0);
                    acText.Height = 0.5;
                    acText.TextString = "Hello, World.";

                    // Change the oblique angle of the text object to 45 degrees(0.707 in radians)
                    acText.Oblique = 0.707;

                    acBlkTblRec.AppendEntity(acText);
                    acTrans.AddNewlyCreatedDBObject(acText, true);
                }

                acTrans.Commit();
            }
        }
        #endregion

        #region 3.7.2.4.2 Align Single-Line Text
        [CommandMethod("TextAlignment")]
        public void TextAlignment()
        {
            // 왼쪽정렬 / 가운데정렬/ 오른쪽정렬
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                string[] textString = new string[3];
                textString[0] = "Left";
                textString[1] = "Center";
                textString[2] = "Right";

                int[] textAlign = new int[3];
                textAlign[0] = (int)TextHorizontalMode.TextLeft;
                textAlign[1] = (int)TextHorizontalMode.TextCenter;
                textAlign[2] = (int)TextHorizontalMode.TextRight;

                Point3d acPtIns = new Point3d(3, 3, 0);
                Point3d acPtAlign = new Point3d(3, 3, 0);

                int nCnt = 0;

                foreach (string strVal in textString)
                {
                    // Create a single-line text object
                    using (DBText acText = new DBText())
                    {
                        acText.Position = acPtIns;
                        acText.Height = 0.5;
                        acText.TextString = strVal;

                        // Set the alignment for the text
                        acText.HorizontalMode = (TextHorizontalMode)textAlign[nCnt];

                        if (acText.HorizontalMode != TextHorizontalMode.TextLeft)
                        {
                            acText.AlignmentPoint = acPtAlign;
                        }

                        acBlkTblRec.AppendEntity(acText);
                        acTrans.AddNewlyCreatedDBObject(acText, true);
                    }

                    // Create a point over the alignment point of the text
                    using (DBPoint acPoint = new DBPoint(acPtAlign))
                    {
                        acPoint.ColorIndex = 1;

                        acBlkTblRec.AppendEntity(acPoint);
                        acTrans.AddNewlyCreatedDBObject(acPoint, true);

                        // Adjust the insertion and alignment points
                        acPtIns = new Point3d(acPtIns.X, acPtIns.Y + 3, 0);
                        acPtAlign = acPtIns;
                    }

                    nCnt = nCnt + 1;
                }

                // Set the point style to crosshair
                Application.SetSystemVariable("PDMODE", 2);

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.7.2.4.3 Set Text Generation Flag
        [CommandMethod("BackwardsText")]
        public void BackwardsText()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a single-line text object
                using (DBText acText = new DBText())
                {
                    acText.Position = new Point3d(3, 3, 0);
                    acText.Height = 0.5;
                    acText.TextString = "Hello, World.";

                    // Display the text backwards
                    acText.IsMirroredInX = true;
                    acText.IsMirroredInY = true;
                    acBlkTblRec.AppendEntity(acText);
                    acTrans.AddNewlyCreatedDBObject(acText, true);
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #endregion

        #region 3.7.3 Work With Text Styles

        #region 3.7.3.1 Create and Modify Text Styles

        #endregion

        #region 3.7.3.2 Assign Fonts
        [CommandMethod("UpdateTextFont")]
        public void UpdateTextFont()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                TextStyleTableRecord acTextStyleTblRec;
                acTextStyleTblRec = acTrans.GetObject(acCurDb.Textstyle, OpenMode.ForWrite) as TextStyleTableRecord;

                // Get the current font settings
                Autodesk.AutoCAD.GraphicsInterface.FontDescriptor acFont;
                acFont = acTextStyleTblRec.Font;

                // Update the text style's typeface with "PlayBill"
                Autodesk.AutoCAD.GraphicsInterface.FontDescriptor acNewFont;
                acNewFont = new Autodesk.AutoCAD.GraphicsInterface.FontDescriptor("PlayBill",
                                                                                    acFont.Bold,
                                                                                    acFont.Italic,
                                                                                    acFont.CharacterSet,
                                                                                    acFont.PitchAndFamily);

                acTextStyleTblRec.Font = acNewFont;

                acDoc.Editor.Regen();

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.7.3.3 Use TrueType Fonts

        #endregion

        #region 3.7.3.4 Use Unicode and Big Fonts
        [CommandMethod("ChangeFontFiles")]
        public void ChangeFontFiles()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the current text style for write
                TextStyleTableRecord acTextStyleTblRec;
                acTextStyleTblRec = acTrans.GetObject(acCurDb.Textstyle, OpenMode.ForWrite) as TextStyleTableRecord;

                // Change the font files used for both Big and Regular fonts
                acTextStyleTblRec.BigFontFileName = "C:/AutoCAD/Fonts/bigfont.shx";
                acTextStyleTblRec.FileName = "C:/AutoCAD/Fonts/italic.shx";

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 3.7.3.5 Substitute Fonts

        #endregion

        #endregion

        #region 3.7.4 Use Unicode Characters, Control Codes, and Special Characters

        #endregion

        #region 3.7.5 Check Spelling

        #endregion

        #endregion

        #endregion


        /// <summary>
        /// # 4
        /// </summary>
        #region [4 Dimensions and Tolerances]

        #region 4.1 Dimensioning Concepts

        #region 4.1.1 Parts of a Dimension

        #endregion

        #region 4.1.2 Deine the Dimension System Variables

        #endregion

        #region 4.1.3 Set Dimension Text Styles

        #endregion

        #region 4.1.4 Understand Leader Lines

        #endregion

        #region 4.1.5 Understand Associative Dimensions

        #endregion

        #endregion

        #region 4.2 Create Dimensions

        #region 4.2.1 Create Linear Dimensions
        [CommandMethod("CreateRotatedDimension")]
        public void CreateRotatedDimension()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create the rotated dimension
                using (RotatedDimension acRotDim = new RotatedDimension())
                {
                    acRotDim.XLine1Point = new Point3d(0, 0, 0);
                    acRotDim.XLine2Point = new Point3d(6, 3, 0);
                    acRotDim.Rotation = 0.707;
                    acRotDim.DimLinePoint = new Point3d(0, 5, 0);
                    acRotDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acRotDim);
                    acTrans.AddNewlyCreatedDBObject(acRotDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.2.2 Create Radial Dimensions
        [CommandMethod("CreateRadialDimension")]
        public void CreateRadialDimension()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create the radial dimension
                using (RadialDimension acRadDim = new RadialDimension())
                {
                    acRadDim.Center = new Point3d(0, 0, 0);
                    acRadDim.ChordPoint = new Point3d(5, 5, 0);
                    acRadDim.LeaderLength = 5;
                    acRadDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acRadDim);
                    acTrans.AddNewlyCreatedDBObject(acRadDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.2.3 Create Angular Dimensions
        [CommandMethod("CreateAngularDimension")]
        public void CreateAngularDimension()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create an angular dimension
                using (LineAngularDimension2 acLinAngDim = new LineAngularDimension2())
                {
                    acLinAngDim.XLine1Start = new Point3d(0, 5, 0);
                    acLinAngDim.XLine1End = new Point3d(1, 7, 0);
                    acLinAngDim.XLine2Start = new Point3d(0, 5, 0);
                    acLinAngDim.XLine2End = new Point3d(1, 3, 0);
                    acLinAngDim.ArcPoint = new Point3d(3, 5, 0);

                    acLinAngDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acLinAngDim);
                    acTrans.AddNewlyCreatedDBObject(acLinAngDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.2.4 Create Jogged Radius Dimensions
        [CommandMethod("CreateJoggedDimension")]
        public void CreateJoggedDimension()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // Create a large radius dimension
                using (RadialDimensionLarge acRadDimLrg = new RadialDimensionLarge())
                {
                    acRadDimLrg.Center = new Point3d(-3, -4, 0);
                    acRadDimLrg.ChordPoint = new Point3d(2, 7, 0);
                    acRadDimLrg.OverrideCenter = new Point3d(0, 2, 0);
                    acRadDimLrg.JogPoint = new Point3d(1, 4.5, 0);
                    acRadDimLrg.JogAngle = 0.707;
                    acRadDimLrg.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acRadDimLrg);
                    acTrans.AddNewlyCreatedDBObject(acRadDimLrg, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.2.5 Create Arc Length Dimensions
        [CommandMethod("CreateArcLengthDimension")]
        public void CreateArcLengthDimension()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create an arc length dimension
                /// 1: Arc 중심점
                /// 2: Arc 시작점
                /// 3: Arc 끝점
                /// 4: Arc 위의 한점
                using (ArcDimension acArcDim = new ArcDimension(new Point3d(4.5, 1.5, 0),
                                                                new Point3d(8, 4.25, 0),
                                                                new Point3d(0, 2, 0),
                                                                new Point3d(5, 7, 0),
                                                                "<>",
                                                                acCurDb.Dimstyle))
                {

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acArcDim);
                    acTrans.AddNewlyCreatedDBObject(acArcDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.2.6 Create Ordinate Dimensions
        [CommandMethod("CreateOrdinateDimension")]
        public void CreateOrdinateDimension()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create an ordinate dimension
                using (OrdinateDimension acOrdDim = new OrdinateDimension())
                {
                    acOrdDim.UsingXAxis = true;
                    acOrdDim.DefiningPoint = new Point3d(5, 5, 0);
                    acOrdDim.LeaderEndPoint = new Point3d(10, 5, 0);
                    acOrdDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acOrdDim);
                    acTrans.AddNewlyCreatedDBObject(acOrdDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #region 4.3 Edit Dimensions

        #region 4.3.1 Override Dimension Text

        [CommandMethod("OverrideDimensionText")]
        public static void OverrideDimensionText()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the aligned dimension
                using (AlignedDimension acAliDim = new AlignedDimension())
                {
                    acAliDim.XLine1Point = new Point3d(5, 3, 0);
                    acAliDim.XLine2Point = new Point3d(10, 3, 0);
                    acAliDim.DimLinePoint = new Point3d(7.5, 5, 0);
                    acAliDim.DimensionStyle = acCurDb.Dimstyle;

                    // Override the dimension text
                    acAliDim.DimensionText = "The value is <>";

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acAliDim);
                    acTrans.AddNewlyCreatedDBObject(acAliDim, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }

        #endregion

        #endregion

        #region 4.4 Work With Dimension Styles

        #region 4.4.1 Create, Modify, and Copy Dimension Styles
        [CommandMethod("CopyDimStyles")]
        public void CopyDimStyles()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for read
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForRead) as BlockTableRecord;

                object acObj = null;
                foreach (ObjectId acObjId in acBlkTblRec)
                {
                    // Get the first object in Model space
                    acObj = acTrans.GetObject(acObjId,
                                                OpenMode.ForRead);

                    break;
                }

                // Open the DimStyle table for read
                DimStyleTable acDimStyleTbl;
                acDimStyleTbl = acTrans.GetObject(acCurDb.DimStyleTableId,
                                                    OpenMode.ForRead) as DimStyleTable;

                string[] strDimStyleNames = new string[3];
                strDimStyleNames[0] = "Style 1 copied from a dim";
                strDimStyleNames[1] = "Style 2 copied from Style 1";
                strDimStyleNames[2] = "Style 3 copied from the running drawing values";

                int nCnt = 0;

                // Keep a reference of the first dimension style for later
                DimStyleTableRecord acDimStyleTblRec1 = null;

                // Iterate the array of dimension style names
                foreach (string strDimStyleName in strDimStyleNames)
                {
                    DimStyleTableRecord acDimStyleTblRec;
                    DimStyleTableRecord acDimStyleTblRecCopy = null;

                    // Check to see if the dimension style exists or not
                    if (acDimStyleTbl.Has(strDimStyleName) == false)
                    {
                        if (acDimStyleTbl.IsWriteEnabled == false) acTrans.GetObject(acCurDb.DimStyleTableId, OpenMode.ForWrite);

                        acDimStyleTblRec = new DimStyleTableRecord();
                        acDimStyleTblRec.Name = strDimStyleName;

                        acDimStyleTbl.Add(acDimStyleTblRec);
                        acTrans.AddNewlyCreatedDBObject(acDimStyleTblRec, true);
                    }
                    else
                    {
                        acDimStyleTblRec = acTrans.GetObject(acDimStyleTbl[strDimStyleName],
                                                                OpenMode.ForWrite) as DimStyleTableRecord;
                    }

                    // Determine how the new dimension style is populated
                    switch ((int)nCnt)
                    {
                        // Assign the values of the dimension object to the new dimension style
                        case 0:
                            try
                            {
                                // Cast the object to a Dimension
                                Dimension acDim = acObj as Dimension;

                                // Copy the dimension style data from the dimension and
                                // set the name of the dimension style as the copied settings
                                // are unnamed.
                                acDimStyleTblRecCopy = acDim.GetDimstyleData();
                                acDimStyleTblRec1 = acDimStyleTblRec;
                            }
                            catch
                            {
                                // Object was not a dimension
                            }

                            break;

                        // Assign the values of the dimension style to the new dimension style
                        case 1:
                            acDimStyleTblRecCopy = acDimStyleTblRec1;
                            break;
                        // Assign the values of the current drawing to the dimension style
                        case 2:
                            acDimStyleTblRecCopy = acCurDb.GetDimstyleData();
                            break;
                    }

                    // Copy the dimension settings and set the name of the dimension style
                    acDimStyleTblRec.CopyFrom(acDimStyleTblRecCopy);
                    acDimStyleTblRec.Name = strDimStyleName;

                    // Dispose of the copied dimension style
                    acDimStyleTblRecCopy.Dispose();

                    nCnt = nCnt + 1;
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.4.2 Override the Dimension Styles
        [CommandMethod("AddDimensionTextSuffix")]
        public static void AddDimensionTextSuffix()
        {
            // '치수 꼬리말' 넣기
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the aligned dimension
                using (AlignedDimension acAliDim = new AlignedDimension())
                {
                    acAliDim.XLine1Point = new Point3d(0, 5, 0);
                    acAliDim.XLine2Point = new Point3d(5, 5, 0);
                    acAliDim.DimLinePoint = new Point3d(5, 7, 0);
                    acAliDim.DimensionStyle = acCurDb.Dimstyle;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acAliDim);
                    acTrans.AddNewlyCreatedDBObject(acAliDim, true);

                    // Append a suffix to the dimension text
                    PromptStringOptions pStrOpts = new PromptStringOptions("");

                    pStrOpts.Message = "\nEnter a new text suffix for the dimension: ";
                    pStrOpts.AllowSpaces = true;
                    PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);

                    if (pStrRes.Status == PromptStatus.OK)
                    {
                        // 치수 꼬리말 입력
                        acAliDim.Suffix = pStrRes.StringResult;
                    }
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #endregion

        #region 4.5 Dimension in Model Space and Paper Space

        #endregion

        #region 4.6 Create Leaders and Annotation

        #region 4.6.1 Create Leader Lines
        [CommandMethod("CreateLeader")]
        public void CreateLeader()
        {
            /// 지시선 생성
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the leader
                using (Leader acLdr = new Leader())
                {
                    acLdr.AppendVertex(new Point3d(0, 0, 0));
                    acLdr.AppendVertex(new Point3d(4, 4, 0));
                    acLdr.AppendVertex(new Point3d(4, 5, 0));
                    acLdr.HasArrowHead = true;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acLdr);
                    acTrans.AddNewlyCreatedDBObject(acLdr, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.6.2 Add Annotation to a Leader

        #endregion

        #region 4.6.3 Leader Associativity
        [CommandMethod("AddLeaderAnnotation")]
        public void AddLeaderAnnotation()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the MText annotation
                using (MText acMText = new MText())
                {
                    acMText.Contents = "Hello, World.";
                    acMText.Location = new Point3d(5, 5, 0);
                    acMText.Width = 2;

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acMText);
                    acTrans.AddNewlyCreatedDBObject(acMText, true);

                    // Create the leader with annotation
                    using (Leader acLdr = new Leader())
                    {
                        acLdr.AppendVertex(new Point3d(0, 0, 0));
                        acLdr.AppendVertex(new Point3d(4, 4, 0));
                        acLdr.AppendVertex(new Point3d(4, 5, 0));
                        acLdr.HasArrowHead = true;

                        // Add the new object to Model space and the transaction
                        acBlkTblRec.AppendEntity(acLdr);
                        acTrans.AddNewlyCreatedDBObject(acLdr, true);

                        // Attach the annotation after the leader object is added
                        acLdr.Annotation = acMText.ObjectId;
                        acLdr.EvaluateLeader();
                    }
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.6.4 Edit Leader Associativity

        #endregion

        #region 4.6.5 Edit Leaders

        #endregion

        #endregion

        #region 4.7 Use Geometric Tolerances

        #region 4.7.1 Create Geometric Tolerances
        [CommandMethod("CreateGeometricTolerance")]
        public void CreateGeometricTolerance()
        {
            // Get the current database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the Geometric Tolerance (Feature Control Frame)
                using (FeatureControlFrame acFcf = new FeatureControlFrame())
                {
                    // %%v => 칸 나누기
                    // %%v + 문자 => 사각형 안에 문자
                    // {\\Fgdt; + '문자'} => 특수기호
                    // {\\Fgdt;j} => 원형에 십자가 모양
                    // {\\Fgdt;n} => 파이
                    acFcf.Text = "{\\Fgdt;j}%%v{\\Fgdt;n}0.001%%v%%v%%v%%v{\\Fgdt;k}%%v1234%%v5678%%v1111";
                    acFcf.Location = new Point3d(5, 5, 0);

                    // Add the new object to Model space and the transaction
                    acBlkTblRec.AppendEntity(acFcf);
                    acTrans.AddNewlyCreatedDBObject(acFcf, true);
                }

                // Commit the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
        #endregion

        #region 4.7.2 Edit Geometric Tolerances

        #endregion

        #endregion

        #endregion


        /// <summary>
        /// # 5
        /// </summary>
        #region [5 Work in Three-Dimensional Space]

        #endregion


        /// <summary>
        /// # 6
        /// </summary>
        #region [6 Define Layouts and Plot(.NET)]

        #region 6.1 Model Space and Paper Space

        #endregion

        #region 6.2 Layouts(.NET)

        // List all the layouts in the current drawing
        [CommandMethod("ListLayouts")]
        public void ListLayouts()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Get the layout dictionary of the current database
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                DBDictionary lays =
                    acTrans.GetObject(acCurDb.LayoutDictionaryId,
                        OpenMode.ForRead) as DBDictionary;

                acDoc.Editor.WriteMessage("\nLayouts:");

                // Step through and list each named layout and Model
                foreach (DBDictionaryEntry item in lays)
                {
                    acDoc.Editor.WriteMessage("\n  " + item.Key);
                }

                // Abort the changes to the database
                acTrans.Abort();
            }
        }

        // Create a new layout with the LayoutManager
        [CommandMethod("CreateLayout")]
        public void CreateLayout()
        {
            // 레이아웃(Layout) 생성
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Get the layout and plot settings of the named pagesetup
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Reference the Layout Manager
                LayoutManager acLayoutMgr = LayoutManager.Current;

                // Create the new layout with default settings
                ObjectId objID = acLayoutMgr.CreateLayout("newLayout");

                // Open the layout
                Layout acLayout = acTrans.GetObject(objID, OpenMode.ForRead) as Layout;

                // Set the layout current if it is not already
                if (acLayout.TabSelected == false)
                {
                    acLayoutMgr.CurrentLayout = acLayout.LayoutName;
                }

                // Output some information related to the layout object
                acDoc.Editor.WriteMessage("\nTab Order: " + acLayout.TabOrder +
                                          "\nTab Selected: " + acLayout.TabSelected +
                                          "\nBlock Table Record ID: " +
                                          acLayout.BlockTableRecordId.ToString());

                // Save the changes made
                acTrans.Commit();
            }
        }


        // Import a layout from an external drawing
        [CommandMethod("ImportLayout")]
        public void ImportLayout()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Specify the layout name and drawing file to work with
            string layoutName = "MAIN AND SECOND FLOOR PLAN";
            string filename = "C:\\AutoCAD\\Sample\\Sheet Sets\\Architectural\\A-01.dwg";

            // Create a new database object and open the drawing into memory
            Database acExDb = new Database(false, true);
            acExDb.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, true, "");

            // Create a transaction for the external drawing
            using (Transaction acTransEx = acExDb.TransactionManager.StartTransaction())
            {
                // Get the layouts dictionary
                DBDictionary layoutsEx =
                    acTransEx.GetObject(acExDb.LayoutDictionaryId,
                                        OpenMode.ForRead) as DBDictionary;

                // Check to see if the layout exists in the external drawing
                if (layoutsEx.Contains(layoutName) == true)
                {
                    // Get the layout and block objects from the external drawing
                    Layout layEx =
                        layoutsEx.GetAt(layoutName).GetObject(OpenMode.ForRead) as Layout;
                    BlockTableRecord blkBlkRecEx =
                        acTransEx.GetObject(layEx.BlockTableRecordId,
                                            OpenMode.ForRead) as BlockTableRecord;

                    // Get the objects from the block associated with the layout
                    ObjectIdCollection idCol = new ObjectIdCollection();
                    foreach (ObjectId id in blkBlkRecEx)
                    {
                        idCol.Add(id);
                    }

                    // Create a transaction for the current drawing
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        // Get the block table and create a new block
                        // then copy the objects between drawings
                        BlockTable blkTbl =
                            acTrans.GetObject(acCurDb.BlockTableId,
                                              OpenMode.ForWrite) as BlockTable;

                        using (BlockTableRecord blkBlkRec = new BlockTableRecord())
                        {
                            int layoutCount = layoutsEx.Count - 1;

                            blkBlkRec.Name = "*Paper_Space" + layoutCount.ToString();
                            blkTbl.Add(blkBlkRec);
                            acTrans.AddNewlyCreatedDBObject(blkBlkRec, true);
                            acExDb.WblockCloneObjects(idCol,
                                                      blkBlkRec.ObjectId,
                                                      new IdMapping(),
                                                      DuplicateRecordCloning.Ignore,
                                                      false);

                            // Create a new layout and then copy properties between drawings
                            DBDictionary layouts =
                                acTrans.GetObject(acCurDb.LayoutDictionaryId,
                                                  OpenMode.ForWrite) as DBDictionary;

                            using (Layout lay = new Layout())
                            {
                                lay.LayoutName = layoutName;
                                lay.AddToLayoutDictionary(acCurDb, blkBlkRec.ObjectId);
                                acTrans.AddNewlyCreatedDBObject(lay, true);
                                lay.CopyFrom(layEx);

                                DBDictionary plSets =
                                    acTrans.GetObject(acCurDb.PlotSettingsDictionaryId,
                                                      OpenMode.ForRead) as DBDictionary;

                                // Check to see if a named page setup was assigned to the layout,
                                // if so then copy the page setup settings
                                if (lay.PlotSettingsName != "")
                                {
                                    // Check to see if the page setup exists
                                    if (plSets.Contains(lay.PlotSettingsName) == false)
                                    {
                                        acTrans.GetObject(acCurDb.PlotSettingsDictionaryId, OpenMode.ForWrite);

                                        using (PlotSettings plSet = new PlotSettings(lay.ModelType))
                                        {
                                            plSet.PlotSettingsName = lay.PlotSettingsName;
                                            plSet.AddToPlotSettingsDictionary(acCurDb);
                                            acTrans.AddNewlyCreatedDBObject(plSet, true);

                                            DBDictionary plSetsEx =
                                                acTransEx.GetObject(acExDb.PlotSettingsDictionaryId,
                                                                    OpenMode.ForRead) as DBDictionary;

                                            PlotSettings plSetEx =
                                                plSetsEx.GetAt(lay.PlotSettingsName).GetObject(
                                                               OpenMode.ForRead) as PlotSettings;

                                            plSet.CopyFrom(plSetEx);
                                        }
                                    }
                                }
                            }
                        }

                        // Regen the drawing to get the layout tab to display
                        acDoc.Editor.Regen();

                        // Save the changes made
                        acTrans.Commit();
                    }
                }
                else
                {
                    // Display a message if the layout could not be found in the specified drawing
                    acDoc.Editor.WriteMessage("\nLayout '" + layoutName +
                                              "' could not be imported from '" + filename + "'.");
                }

                // Discard the changes made to the external drawing file
                acTransEx.Abort();
            }

            // Close the external drawing file
            acExDb.Dispose();
        }

        #region 6.2.1 Layouts and Blocks

        #endregion

        #region 6.2.2 Layout Settings

        #region 6.2.2.1 Paper Size and Units
        // Lists the available local media names for a specified plot configuration (PC3) file
        [CommandMethod("PlotterLocalMediaNameList")]
        public static void PlotterLocalMediaNameList()
        {
            /// 플롯 세팅
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            using (PlotSettings plSet = new PlotSettings(true))
            {
                PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;

                // Set the Plotter and page size
                // ['PlotSettings'], [플룻 이름], [용지 크기]
                acPlSetVdr.SetPlotConfigurationName(plSet, "DWF6 ePlot.pc3",
                                                    "ANSI_A_(8.50_x_11.00_Inches)");

                acDoc.Editor.WriteMessage("\nCanonical and Local media names: ");

                int cnt = 0;

                string s = "";

                foreach (string mediaName in acPlSetVdr.GetCanonicalMediaNameList(plSet))
                {
                    // Output the names of the available media for the specified device
                    acDoc.Editor.WriteMessage("\n  " + mediaName + " | " +
                                              acPlSetVdr.GetLocaleMediaName(plSet, cnt));
                    s += "\n  " + mediaName + " | " +
                                              acPlSetVdr.GetLocaleMediaName(plSet, cnt);
                    cnt = cnt + 1;
                }

                MessageBox.Show(s);
            }
        }
        #endregion

        #region 6.2.2.2 Plot Origin

        #endregion

        #region 6.2.2.3 Plot Area

        #endregion

        #region 6.2.2.4 Plot Scale

        #endregion

        #region 6.2.2.5 Lineweight Scale

        #endregion

        #region 6.2.2.6 Plot Device
        // Lists the available plotters (plot configuration [PC3] files)
        [CommandMethod("PlotterList")]
        public void PlotterList()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            acDoc.Editor.WriteMessage("\nPlot devices: ");

            string s = "";

            foreach (string plotDevice in PlotSettingsValidator.Current.GetPlotDeviceList())
            {
                // Output the names of the available plotter devices
                acDoc.Editor.WriteMessage("\n  " + plotDevice);
                s += plotDevice + "\n";
            }
            MessageBox.Show(s);
        }
        #endregion

        #region 6.2.2.7 Query and Set Layout Settings
        // Changes the plot settings for a layout directly
        [CommandMethod("ChangeLayoutPlotSettings")]
        public static void ChangeLayoutPlotSettings()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Reference the Layout Manager
                LayoutManager acLayoutMgr = LayoutManager.Current;

                // Get the current layout and output its name in the Command Line window
                Layout acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout),
                                                    OpenMode.ForRead) as Layout;

                // Output the name of the current layout and its device
                acDoc.Editor.WriteMessage("\nCurrent layout: " + acLayout.LayoutName);

                acDoc.Editor.WriteMessage("\nCurrent device name: " + acLayout.PlotConfigurationName);

                // Get a copy of the PlotSettings from the layout
                using (PlotSettings acPlSet = new PlotSettings(acLayout.ModelType))
                {
                    acPlSet.CopyFrom(acLayout);

                    // Update the PlotConfigurationName property of the PlotSettings object
                    PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;
                    acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG To PDF.pc3", "ANSI_B_(11.00_x_17.00_Inches)");

                    // Zoom to show the whole paper
                    acPlSetVdr.SetZoomToPaperOnUpdate(acPlSet, true);

                    // Update the layout
                    acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForWrite);
                    acLayout.CopyFrom(acPlSet);
                }

                // Output the name of the new device assigned to the layout
                acDoc.Editor.WriteMessage("\nNew device name: " + acLayout.PlotConfigurationName);

                // Save the new objects to the database
                acTrans.Commit();
            }

            // Update the display
            acDoc.Editor.Regen();
        }
        #endregion


        #endregion

        #endregion

        #region 6.3 Viewports(.NET)

        #region 6.3.1 Floating Viewports
        [CommandMethod("ToggleSpace")]
        public void ToggleSpace()
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            // Get the current values of CVPORT and TILEMODE
            object oCvports = Application.GetSystemVariable("CVPORT");
            object oTilemode = Application.GetSystemVariable("TILEMODE");

            // Check to see if the Model layout is active, TILEMODE is 1 when
            // the Model layout is active
            if (System.Convert.ToInt16(oTilemode) == 0)
            {
                // Check to see if Model space is active in a viewport,
                // CVPORT is 2 if Model space is active
                if (System.Convert.ToInt16(oCvports) == 2)
                {
                    acDoc.Editor.SwitchToPaperSpace();
                }
                else
                {
                    acDoc.Editor.SwitchToModelSpace();
                }
            }
            else
            {
                // Switch to the previous Paper space layout
                Application.SetSystemVariable("TILEMODE", 0);
            }
        }
        #endregion

        #region 6.3.2 Create Paper Space Viewports

        #endregion

        #region 6.3.3 Scale Views Relative to Paper Space

        #endregion

        #region 6.3.4 Scale Pattern Linetypes in Paper Space

        #endregion

        #region 6.3.5 Use Shaded Viewports

        #endregion

        #endregion

        #region 6.4 Plot Styles(.NET)

        #endregion

        #region 6.5 Visual Styles(.NET)

        #endregion

        #region 6.6 Render Presets(.NET)

        #endregion

        #region 6.7 Plot Settings and Page Setups(.NET)

        #endregion

        #region 6.8 Plot Your Drawing(.NET)

        #endregion

        #region 6.9 Publish Layouts(.NET)

        #endregion

        #endregion


        /// <summary>
        /// # 7
        /// </summary>
        #region [7 Advanced Drawing and Organizational(.NET)]

        #region 7.1 Work With Raster Images

        #region 7.1.1 Attach and Scale a Raster Image

        [CommandMethod("AttachRasterImage")]
        public void AttachRasterImage()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Define the name and image to use
                string strImgName = "test2312";         // RasterImage 이름 지정
                string strFileName = @"D:\test.JPG";    // RasterImage로 사용할 이미지 경로
                //string strImgName = "WorldMap";
                //string strFileName = "C:\\AutoCAD\\Sample\\VBA\\WorldMap.TIF";


                RasterImageDef acRasterDef;
                bool bRasterDefCreated = false;
                ObjectId acImgDefId;

                // Get the image dictionary
                ObjectId acImgDctID = RasterImageDef.GetImageDictionary(acCurDb);

                // Check to see if the dictionary does not exist, it not then create it
                if (acImgDctID.IsNull)
                {
                    acImgDctID = RasterImageDef.CreateImageDictionary(acCurDb);
                }

                // Open the image dictionary
                DBDictionary acImgDict = acTrans.GetObject(acImgDctID, OpenMode.ForRead) as DBDictionary;

                // Check to see if the image definition already exists
                if (acImgDict.Contains(strImgName))
                {
                    acImgDefId = acImgDict.GetAt(strImgName);

                    acRasterDef = acTrans.GetObject(acImgDefId, OpenMode.ForWrite) as RasterImageDef;
                }
                else
                {
                    // Create a raster image definition
                    RasterImageDef acRasterDefNew = new RasterImageDef();

                    // Set the source for the image file
                    acRasterDefNew.SourceFileName = strFileName;

                    // Load the image into memory
                    acRasterDefNew.Load();

                    // Add the image definition to the dictionary
                    acTrans.GetObject(acImgDctID, OpenMode.ForWrite);
                    acImgDefId = acImgDict.SetAt(strImgName, acRasterDefNew);

                    acTrans.AddNewlyCreatedDBObject(acRasterDefNew, true);

                    acRasterDef = acRasterDefNew;

                    bRasterDefCreated = true;
                }

                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create the new image and assign it the image definition
                using (RasterImage acRaster = new RasterImage())
                {
                    acRaster.ImageDefId = acImgDefId;

                    // Use ImageWidth and ImageHeight to get the size of the image in pixels (1024 x 768).
                    // Use ResolutionMMPerPixel to determine the number of millimeters in a pixel so you 
                    // can convert the size of the drawing into other units or millimeters based on the 
                    // drawing units used in the current drawing.

                    // Define the width and height of the image
                    Vector3d width;
                    Vector3d height;

                    // Check to see if the measurement is set to English (Imperial) or Metric units
                    if (acCurDb.Measurement == MeasurementValue.English)
                    {
                        width = new Vector3d((acRasterDef.ResolutionMMPerPixel.X * acRaster.ImageWidth) / 25.4, 0, 0);
                        height = new Vector3d(0, (acRasterDef.ResolutionMMPerPixel.Y * acRaster.ImageHeight) / 25.4, 0);
                    }
                    else
                    {
                        width = new Vector3d(acRasterDef.ResolutionMMPerPixel.X * acRaster.ImageWidth, 0, 0);
                        height = new Vector3d(0, acRasterDef.ResolutionMMPerPixel.Y * acRaster.ImageHeight, 0);
                    }

                    // Define the position for the image 
                    Point3d insPt = new Point3d(5.0, 5.0, 0.0);

                    // Define and assign a coordinate system for the image's orientation
                    CoordinateSystem3d coordinateSystem = new CoordinateSystem3d(insPt, width * 2, height * 2);
                    acRaster.Orientation = coordinateSystem;

                    // Set the rotation angle for the image
                    acRaster.Rotation = 0;

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acRaster);
                    acTrans.AddNewlyCreatedDBObject(acRaster, true);

                    // Connect the raster definition and image together so the definition
                    // does not appear as "unreferenced" in the External References palette.
                    RasterImage.EnableReactors(true);
                    acRaster.AssociateRasterDef(acRasterDef);

                    if (bRasterDefCreated)
                    {
                        acRasterDef.Dispose();
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }

        #endregion

        #region 7.1.2 Manage Raster Images

        #endregion

        #region 7.1.3 Modify Images and Image Boundaries

        #endregion

        #region 7.1.4 Change the Clipping Boundary of an Image

        #endregion



        #endregion

        #region 7.2 Use Blocks and Attributes

        #region 7.2.1 Work with Blocks

        #region 7.2.1.1 Define Blocks
        [CommandMethod("CreatingABlock")]
        public void CreatingABlock()
        {
            /// 블럭 생성하기
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        #endregion

        #region 7.2.1.2 Insert Blocks
        [CommandMethod("InsertingABlock")]
        public void InsertingABlock()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;

                if (!acBlkTbl.Has("CircleBlock2"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock2";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlock"];
                }

                // Insert the block into the current space
                if (blkRecId != ObjectId.Null)
                {
                    using (BlockReference acBlkRef = new BlockReference(new Point3d(0, 0, 0), blkRecId))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        #endregion

        #region 7.2.1.3 Explode a Block Reference
        [CommandMethod("ExplodingABlock")]
        public void ExplodingABlock()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;

                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlock"];
                }

                // Insert the block into the current space
                if (blkRecId != ObjectId.Null)
                {
                    using (BlockReference acBlkRef = new BlockReference(new Point3d(0, 0, 0), blkRecId))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                        using (DBObjectCollection dbObjCol = new DBObjectCollection())
                        {
                            // 블럭 분해
                            acBlkRef.Explode(dbObjCol);

                            foreach (DBObject dbObj in dbObjCol)
                            {
                                Entity acEnt = dbObj as Entity;

                                acCurSpaceBlkTblRec.AppendEntity(acEnt);
                                acTrans.AddNewlyCreatedDBObject(dbObj, true);

                                acEnt = acTrans.GetObject(dbObj.ObjectId, OpenMode.ForWrite) as Entity;

                                acEnt.ColorIndex = 1;
                                Application.ShowAlertDialog("Exploded Object: " + acEnt.GetRXClass().DxfName);
                            }
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        #endregion

        #region 7.2.1.4 Redefine a Block
        [CommandMethod("RedefiningABlock")]
        public void RedefiningABlock()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);

                            // Insert the block into the current space
                            using (BlockReference acBlkRef = new BlockReference(new Point3d(0, 0, 0), acBlkTblRec.Id))
                            {
                                BlockTableRecord acModelSpace;
                                acModelSpace = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                                acModelSpace.AppendEntity(acBlkRef);
                                acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                                Application.ShowAlertDialog("CircleBlock has been created.");
                            }
                        }
                    }
                }
                else
                {
                    // Redefine the block if it exists
                    BlockTableRecord acBlkTblRec =
                        acTrans.GetObject(acBlkTbl["CircleBlock"], OpenMode.ForWrite) as BlockTableRecord;

                    // Step through each object in the block table record
                    foreach (ObjectId objID in acBlkTblRec)
                    {
                        DBObject dbObj = acTrans.GetObject(objID, OpenMode.ForRead) as DBObject;

                        // Revise the circle in the block
                        if (dbObj is Circle)
                        {
                            Circle acCirc = dbObj as Circle;

                            acTrans.GetObject(objID, OpenMode.ForWrite);
                            acCirc.Radius = acCirc.Radius * 2;
                        }
                    }

                    // Update existing block references
                    foreach (ObjectId objID in acBlkTblRec.GetBlockReferenceIds(false, true))
                    {
                        BlockReference acBlkRef = acTrans.GetObject(objID, OpenMode.ForWrite) as BlockReference;
                        acBlkRef.RecordGraphicsModified(true);
                    }

                    Application.ShowAlertDialog("CircleBlock has been revised.");
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        #endregion

        #endregion

        #region 7.2.2 Work with Attributes

        #region 7.2.2.1 Create Attribute Definitions and Attribute References

        [CommandMethod("AddingAttributeToABlock")]
        public void AddingAttributeToABlock()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (!acBlkTbl.Has("CircleBlockWithAttributes"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlockWithAttributes";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            // Add an attribute definition to the block
                            using (AttributeDefinition acAttDef = new AttributeDefinition())
                            {
                                acAttDef.Position = new Point3d(0, 0, 0);
                                acAttDef.Verifiable = true;
                                acAttDef.Prompt = "Door #: ";
                                acAttDef.Tag = "Door#";
                                acAttDef.TextString = "DXX";
                                acAttDef.Height = 1;
                                acAttDef.Justify = AttachmentPoint.MiddleCenter;

                                acBlkTblRec.AppendEntity(acAttDef);

                                acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                                acBlkTbl.Add(acBlkTblRec);
                                acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                            }
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }


        [CommandMethod("InsertingBlockWithAnAttribute")]
        public void InsertingBlockWithAnAttribute()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;

                if (!acBlkTbl.Has("CircleBlockWithAttributes"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlockWithAttributes";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            // Add an attribute definition to the block
                            using (AttributeDefinition acAttDef = new AttributeDefinition())
                            {
                                acAttDef.Position = new Point3d(0, 0, 0);
                                acAttDef.Prompt = "Door #: ";
                                acAttDef.Tag = "Door#";
                                acAttDef.TextString = "DXX";
                                acAttDef.Height = 1;
                                acAttDef.Justify = AttachmentPoint.MiddleCenter;
                                acBlkTblRec.AppendEntity(acAttDef);

                                acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                                acBlkTbl.Add(acBlkTblRec);
                                acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                            }
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlockWithAttributes"];
                }

                // Insert the block into the current space
                if (blkRecId != ObjectId.Null)
                {
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;

                    // Create and insert the new block reference
                    using (BlockReference acBlkRef = new BlockReference(new Point3d(2, 2, 0), blkRecId))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                        // Verify block table record has attribute definitions associated with it
                        if (acBlkTblRec.HasAttributeDefinitions)
                        {
                            // Add attributes from the block table record
                            foreach (ObjectId objID in acBlkTblRec)
                            {
                                DBObject dbObj = acTrans.GetObject(objID, OpenMode.ForRead) as DBObject;

                                if (dbObj is AttributeDefinition)
                                {
                                    AttributeDefinition acAtt = dbObj as AttributeDefinition;

                                    if (!acAtt.Constant)
                                    {
                                        using (AttributeReference acAttRef = new AttributeReference())
                                        {
                                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);

                                            acAttRef.TextString = acAtt.TextString;

                                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);

                                            acTrans.AddNewlyCreatedDBObject(acAttRef, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }

        #endregion

        #region 7.2.2.2 Edit Attribute Definiitions
        [CommandMethod("RedefiningAnAttribute")]
        public void RedefiningAnAttribute()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;

                if (!acBlkTbl.Has("CircleBlockWithAttributes2"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlockWithAttributes2";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            // Add an attribute definition to the block
                            using (AttributeDefinition acAttDef = new AttributeDefinition())
                            {
                                acAttDef.Position = new Point3d(0, 0, 0);
                                acAttDef.Prompt = "Door #: ";
                                acAttDef.Tag = "Door#";
                                acAttDef.TextString = "DXX";
                                acAttDef.Height = 1;
                                acAttDef.Justify = AttachmentPoint.MiddleCenter;
                                acBlkTblRec.AppendEntity(acAttDef);

                                acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                                acBlkTbl.Add(acBlkTblRec);
                                acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                            }
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlockWithAttributes"];
                }

                // Create and insert the new block reference
                if (blkRecId != ObjectId.Null)
                {
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;

                    using (BlockReference acBlkRef = new BlockReference(new Point3d(2, 2, 0), blkRecId))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                        // Verify block table record has attribute definitions associated with it
                        if (acBlkTblRec.HasAttributeDefinitions)
                        {
                            // Add attributes from the block table record
                            foreach (ObjectId objID in acBlkTblRec)
                            {
                                DBObject dbObj = acTrans.GetObject(objID, OpenMode.ForRead) as DBObject;

                                if (dbObj is AttributeDefinition)
                                {
                                    AttributeDefinition acAtt = dbObj as AttributeDefinition;

                                    if (!acAtt.Constant)
                                    {
                                        using (AttributeReference acAttRef = new AttributeReference())
                                        {
                                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);

                                            acAttRef.TextString = acAtt.TextString;

                                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
                                            acTrans.AddNewlyCreatedDBObject(acAttRef, true);
                                        }
                                    }

                                    // Change the attribute definition to be displayed as backwards
                                    acTrans.GetObject(objID, OpenMode.ForWrite);
                                    acAtt.IsMirroredInX = true;
                                }
                            }
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        #endregion

        #region 7.2.2.3 Extract Attribute Information
        [CommandMethod("GettingAttributes")]
        public void GettingAttributes()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;

                if (!acBlkTbl.Has("TESTBLOCK"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "TESTBLOCK";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add an attribute definition to the block
                        using (AttributeDefinition acAttDef = new AttributeDefinition())
                        {
                            acAttDef.Position = new Point3d(5, 5, 0);
                            acAttDef.Prompt = "Attribute Prompt";
                            acAttDef.Tag = "AttributeTag";
                            acAttDef.TextString = "Attribute Value";
                            acAttDef.Height = 1;
                            acAttDef.Justify = AttachmentPoint.MiddleCenter;
                            acBlkTblRec.AppendEntity(acAttDef);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlockWithAttributes"];
                }

                // Create and insert the new block reference
                if (blkRecId != ObjectId.Null)
                {
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(blkRecId, OpenMode.ForRead) as BlockTableRecord;

                    using (BlockReference acBlkRef = new BlockReference(new Point3d(5, 5, 0), acBlkTblRec.Id))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);

                        // Verify block table record has attribute definitions associated with it
                        if (acBlkTblRec.HasAttributeDefinitions)
                        {
                            // Add attributes from the block table record
                            foreach (ObjectId objID in acBlkTblRec)
                            {
                                DBObject dbObj = acTrans.GetObject(objID, OpenMode.ForRead) as DBObject;

                                if (dbObj is AttributeDefinition)
                                {
                                    AttributeDefinition acAtt = dbObj as AttributeDefinition;

                                    if (!acAtt.Constant)
                                    {
                                        using (AttributeReference acAttRef = new AttributeReference())
                                        {
                                            acAttRef.SetAttributeFromBlock(acAtt, acBlkRef.BlockTransform);
                                            acAttRef.Position = acAtt.Position.TransformBy(acBlkRef.BlockTransform);

                                            acAttRef.TextString = acAtt.TextString;

                                            acBlkRef.AttributeCollection.AppendAttribute(acAttRef);
                                            acTrans.AddNewlyCreatedDBObject(acAttRef, true);
                                        }
                                    }
                                }
                            }

                            // Display the tags and values of the attached attributes
                            string strMessage = "";
                            AttributeCollection attCol = acBlkRef.AttributeCollection;

                            foreach (ObjectId objID in attCol)
                            {
                                DBObject dbObj = acTrans.GetObject(objID, OpenMode.ForRead) as DBObject;

                                AttributeReference acAttRef = dbObj as AttributeReference;

                                strMessage = strMessage + "Tag: " + acAttRef.Tag + "\n" +
                                                "Value: " + acAttRef.TextString + "\n";

                                // Change the value of the attribute
                                acAttRef.TextString = "NEW VALUE!";
                            }

                            Application.ShowAlertDialog("The attributes for blockReference " + acBlkRef.Name + " are:\n" + strMessage);

                            strMessage = "";
                            foreach (ObjectId objID in attCol)
                            {
                                DBObject dbObj = acTrans.GetObject(objID, OpenMode.ForRead) as DBObject;

                                AttributeReference acAttRef = dbObj as AttributeReference;

                                strMessage = strMessage + "Tag: " + acAttRef.Tag + "\n" +
                                                "Value: " + acAttRef.TextString + "\n";
                            }

                            Application.ShowAlertDialog("The attributes for blockReference " + acBlkRef.Name + " are:\n" + strMessage);
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        #endregion

        #endregion

        #endregion

        #region 7.3 Use External References

        #endregion



        #endregion


        /// <summary>
        /// # 8
        /// </summary>
        #region [8 Use Events(.NET)]

        #region 8.1 Understand AutoCAD Events

        #endregion

        #region 8.2 Guidelines for Event Handlers

        #endregion

        #region 8.3 Register and Unregister Events

        #region 8.3.1 Handle Application Events
        [CommandMethod("AddAppEvent")]
        public void AddAppEvent()
        {
            Application.SystemVariableChanged +=
                new Autodesk.AutoCAD.ApplicationServices.
                    SystemVariableChangedEventHandler(appSysVarChanged);
        }

        [CommandMethod("RemoveAppEvent")]
        public void RemoveAppEvent()
        {
            Application.SystemVariableChanged -=
                new Autodesk.AutoCAD.ApplicationServices.
                    SystemVariableChangedEventHandler(appSysVarChanged);
        }

        public void appSysVarChanged(object senderObj, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs sysVarChEvtArgs)
        {
            object oVal = Application.GetSystemVariable(sysVarChEvtArgs.Name);

            // Display a message box with the system variable name and the new value
            Application.ShowAlertDialog(sysVarChEvtArgs.Name + " was changed." + "\nNew value: " + oVal.ToString());
        }
        #endregion

        #region 8.3.2 Handle DocumentCollection Events
        [CommandMethod("AddDocColEvent")]
        public void AddDocColEvent()
        {
            Application.DocumentManager.DocumentActivated +=
                new DocumentCollectionEventHandler(docColDocAct);
        }

        [CommandMethod("RemoveDocColEvent")]
        public void RemoveDocColEvent()
        {
            Application.DocumentManager.DocumentActivated -=
                new DocumentCollectionEventHandler(docColDocAct);
        }

        public void docColDocAct(object senderObj, DocumentCollectionEventArgs docColDocActEvtArgs)
        {
            Application.ShowAlertDialog(docColDocActEvtArgs.Document.Name + " was activated.");
        }
        #endregion

        #region 8.3.3 Handle Document Events
        [CommandMethod("AddDocEvent")]
        public void AddDocEvent()
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            acDoc.BeginDocumentClose +=
                new DocumentBeginCloseEventHandler(docBeginDocClose);
        }

        [CommandMethod("RemoveDocEvent")]
        public void RemoveDocEvent()
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            acDoc.BeginDocumentClose -=
                new DocumentBeginCloseEventHandler(docBeginDocClose);
        }

        public void docBeginDocClose(object senderObj, DocumentBeginCloseEventArgs docBegClsEvtArgs)
        {
            // Display a message box prompting to continue closing the document
            if (MessageBox.Show("The document is about to be closed." +
                                 "\nDo you want to continue?",
                                 "Close Document",
                                 MessageBoxButtons.YesNo) ==
                                 DialogResult.No)
            {
                docBegClsEvtArgs.Veto();
            }
        }
        #endregion

        #region 8.3.4 Handle Object Events
        // Global variable for polyline object
        Polyline acPoly = null;

        [CommandMethod("AddPlObjEvent")]
        public void AddPlObjEvent()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table record for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create a closed polyline
                acPoly = new Polyline();
                acPoly.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                acPoly.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                acPoly.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                acPoly.AddVertexAt(3, new Point2d(3, 3), 0, 0, 0);
                acPoly.AddVertexAt(4, new Point2d(3, 2), 0, 0, 0);
                acPoly.Closed = true;

                // Add the new object to the block table record and the transaction
                acBlkTblRec.AppendEntity(acPoly);
                acTrans.AddNewlyCreatedDBObject(acPoly, true);

                acPoly.Modified += new EventHandler(acPolyMod);

                // Save the new object to the database
                acTrans.Commit();
            }
        }

        [CommandMethod("RemovePlObjEvent")]
        public void RemovePlObjEvent()
        {
            if (acPoly != null)
            {
                // Get the current document and database, and start a transaction
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the polyline for read
                    acPoly = acTrans.GetObject(acPoly.ObjectId,
                                               OpenMode.ForRead) as Polyline;

                    if (acPoly.IsWriteEnabled == false)
                    {
                        acTrans.GetObject(acPoly.ObjectId, OpenMode.ForWrite);
                    }

                    acPoly.Modified -= new EventHandler(acPolyMod);
                    acPoly = null;
                }
            }
        }

        public void acPolyMod(object senderObj, EventArgs evtArgs)
        {
            Application.ShowAlertDialog("The area of " + acPoly.ToString() + " is: " + acPoly.Area);
        }
        #endregion

        #endregion

        #region 8.4 Register COM Based Events
        // Global variable for AddCOMEvent and RemoveCOMEvent commands
        AcadApplication acAppCom;

        [CommandMethod("AddCOMEvent")]
        public void AddCOMEvent()
        {
            // Set the global variable to hold a reference to the application and
            // register the BeginFileDrop COM event
            acAppCom = Application.AcadApplication as AcadApplication;
            acAppCom.BeginFileDrop +=
                new _DAcadApplicationEvents_BeginFileDropEventHandler(appComBeginFileDrop);
        }

        [CommandMethod("RemoveCOMEvent")]
        public void RemoveCOMEvent()
        {
            // Unregister the COM event handle
            acAppCom.BeginFileDrop -=
                new _DAcadApplicationEvents_BeginFileDropEventHandler(appComBeginFileDrop);
            acAppCom = null;
        }

        public void appComBeginFileDrop(string strFileName, ref bool bCancel)
        {
            // Display a message box prompting to continue inserting the DWG file
            if (MessageBox.Show("AutoCAD is about to load " + strFileName +
                "\nDo you want to continue loading this file?",
                "DWG File Dropped", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                bCancel = true;
            }
        }
        #endregion

        #endregion


        /// <summary>
        /// # 9
        /// </summary>
        #region [9 Develop Applications with VB.NET and C#(.NET)]

        #region 9.1 Handle Errors

        #region 9.1.1 Define Application Error Types

        #endregion

        #region 9.1.2 Trap Runtime Errors

        #region 9.1.2.1 Use Try Statements
        [CommandMethod("NoErrorHandler")]
        public void NoErrorHandler()
        {
            // Create a new database with no document window
            using (Database acDb = new Database(false, true))
            {
                // Read the drawing file named "Drawing123.dwg" on the C: drive.
                // If the "Drawing123.dwg" file does not exist, an eFileNotFound
                // exception is tossed and the program halts.
                acDb.ReadDwgFile("c:\\Drawing123.dwg",
                                 System.IO.FileShare.None, false, "");
            }

            // Message will not be displayed since the exception caused by
            // ReadDwgFile is not handled.
            Application.ShowAlertDialog("End of command reached");
        }

        [CommandMethod("ErrorTryCatchFinally")]
        public void ErrorTryCatchFinally()
        {
            // Create a new database with no document window
            using (Database acDb = new Database(false, true))
            {
                try
                {
                    // Read the drawing file named "Drawing123.dwg" on the C: drive.
                    // If the "Drawing123.dwg" file does not exist, an eFileNotFound
                    // exception is tossed and the catch statement handles the error.
                    acDb.ReadDwgFile("c:\\Drawing123.dwg",
                                     System.IO.FileShare.None, false, "");
                }
                catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                {
                    Application.ShowAlertDialog("The following exception was caught:\n" +
                                                Ex.Message);
                }
                finally
                {
                    // Message is displayed since the exception caused
                    // by ReadDwgFile is handled.
                    Application.ShowAlertDialog("End of command reached");
                }
            }
        }
        #endregion

        #region 9.1.2.2 Use the Exception Object

        #endregion

        #region 9.1.2.3 Using the VB.NET On Error Statements

        #endregion

        #region 9.1.2.4 Compare Error Handlers in VBA or VB to VB.NET

        #endregion

        #endregion

        #region 9.1.3 Respond to User Input Errors

        #endregion

        #endregion

        #region 9.2 Distribute Your Application
        //[CommandMethod("RegisterMyApp")]
        //public void RegisterMyApp()
        //{
        //    // Get the AutoCAD Applications key
        //    string sProdKey = HostApplicationServices.Current.UserRegistryProductRootKey;
        //    string sAppName = "MyApp";

        //    RegistryKey regAcadProdKey = Registry.CurrentUser.OpenSubKey(sProdKey);
        //    RegistryKey regAcadAppKey = regAcadProdKey.OpenSubKey("Applications", true);

        //    // Check to see if the "MyApp" key exists
        //    string[] subKeys = regAcadAppKey.GetSubKeyNames();
        //    foreach (string subKey in subKeys)
        //    {
        //        // If the application is already registered, exit
        //        if (subKey.Equals(sAppName))
        //        {
        //            regAcadAppKey.Close();
        //            return;
        //        }
        //    }

        //    // Get the location of this module
        //    string sAssemblyPath = Assembly.GetExecutingAssembly().Location;

        //    // Register the application
        //    RegistryKey regAppAddInKey = regAcadAppKey.CreateSubKey(sAppName);
        //    regAppAddInKey.SetValue("DESCRIPTION", sAppName, RegistryValueKind.String);
        //    regAppAddInKey.SetValue("LOADCTRLS", 14, RegistryValueKind.DWord);
        //    regAppAddInKey.SetValue("LOADER", sAssemblyPath, RegistryValueKind.String);
        //    regAppAddInKey.SetValue("MANAGED", 1, RegistryValueKind.DWord);

        //    regAcadAppKey.Close();
        //}

        //[CommandMethod("UnregisterMyApp")]
        //public void UnregisterMyApp()
        //{
        //    // Get the AutoCAD Applications key
        //    string sProdKey = HostApplicationServices.Current.RegistryProductRootKey;
        //    string sAppName = "MyApp";

        //    RegistryKey regAcadProdKey = Registry.CurrentUser.OpenSubKey(sProdKey);
        //    RegistryKey regAcadAppKey = regAcadProdKey.OpenSubKey("Applications", true);

        //    // Delete the key for the application
        //    regAcadAppKey.DeleteSubKeyTree(sAppName);
        //    regAcadAppKey.Close();
        //}
        #endregion

        #region 9.3 Securing Managed .NET Applications

        #endregion

        #endregion



        /// <summary>
        /// # 10
        /// </summary>
        #region [10 Customize User Interface (CUI) Managed API(.NET)]
        #endregion



        /// <summary>
        /// # 11
        /// </summary>
        #region [11 VBA/VB to VB.NET and C# Comparison(.NET)]

        #endregion


        /// <summary>
        /// # DockPanelTest
        /// </summary>
        #region DockPanelTest
        [CommandMethod("DockPanelTest")]
        public void DockPanelTest()
        {
            /// 블럭 생성하기
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;
            
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            acBlkTblRec.AppendEntity(acCirc);

                            acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite);
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }








        #endregion

        #endregion
    }

    public class RibbonCommandHandler : System.Windows.Input.ICommand
    {
        public Action<String> CallBack;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is RibbonButton)
            {
                RibbonButton button = parameter as RibbonButton;
                CallBack(button.Id);
            }
        }
    }
    public class Images
    {
        public static BitmapImage getBitmap(Bitmap image)
        {
            var stream = new MemoryStream();

            image.Save(stream, ImageFormat.Png);

            var bmp = new BitmapImage();

            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();

            return bmp;
        }
    }
}

