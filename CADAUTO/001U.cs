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

#region PROJECT
#endregion

namespace CADAUTO
{
    public partial class YT_CAD_TOOL : Form
    {
        #region 필드
        Document acDoc;
        Database acDb;
        DocumentLock acDL;
        #endregion

        #region 속성
        double Min;
        double Max;
        #endregion

        #region 생성자 | 초기화
        public YT_CAD_TOOL()
        {
            InitializeComponent();
        }

        private void _001U_Load(object sender, EventArgs e)
        {
            txt_Min.Text = Properties.Settings.Default.t_min;
            txt_Max.Text = Properties.Settings.Default.t_max;

            chk_Top.Checked = Properties.Settings.Default.c_top;
        }
        #endregion

        #region 이벤트

        #region 버튼 이벤트
        // 버튼 > 실행
        private void btn_Play_Click(object sender, EventArgs e)
        {
            ReadData();

            this.Play();
        }
        // 버튼 > 닫기
        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void chk_Top_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_Top.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        #endregion


        #region PUBLIC 메서드

        public void Play()
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDb = acDoc.Database;
            acDL = acDoc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);

            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult acPSR = acDocEd.GetSelection();

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

                            usedCurve.Add(CAD.GetCenterPoint2d(c));
                        }
                    }
                });
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }
        }

        #endregion


        #region PRIVATE 메서드

        private void ReadData()
        {
            this.Min = double.Parse(txt_Min.Text);
            this.Max = double.Parse(txt_Max.Text);

            Properties.Settings.Default.c_top = chk_Top.Checked;
            Properties.Settings.Default.t_min = txt_Min.Text;
            Properties.Settings.Default.t_max = txt_Max.Text;

            Properties.Settings.Default.Save();
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

        #endregion


    }
}
