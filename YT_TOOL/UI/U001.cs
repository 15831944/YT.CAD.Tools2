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
    public partial class U001 : Form
    {
        #region 속성
        double Min;
        double Max;
        #endregion

        #region 생성자 | 초기화
        public U001()
        {
            InitializeComponent();

            AC.Initialize_YT();
            //Editor = Application.DocumentManager.MdiActiveDocument.Editor;
        }

        private void U001_Load(object sender, EventArgs e)
        {
            txt_Min.Text = Properties.Settings.Default.t_min;
            txt_Max.Text = Properties.Settings.Default.t_max;
            txt_extend.Text = Properties.Settings.Default.t_extend;
            txt_distance.Text = Properties.Settings.Default.t_distance;
            txt_dis2.Text = Properties.Settings.Default.t_distance2;

            chk_Top.Checked = Properties.Settings.Default.c_top;

            this.TopMost = chk_Top.Checked;
        }
        #endregion

        #region 이벤트

        #region 버튼 이벤트
        // 버튼 > 벽 입력
        private void btn_Play_Click(object sender, EventArgs e)
        {
            ReadData();

            CreatePolyLine Command = new CreatePolyLine();

            Command.CreateWallPolyLine();
        }
        // 버튼 > 보 입력
        private void btn_Beam_Click(object sender, EventArgs e)
        {
            ReadData();

            CreatePolyLine Command = new CreatePolyLine();

            Command.CreateBeamPolyLine();
        }
        // 버튼 > 기둥 입력
        private void btn_Column_Click(object sender, EventArgs e)
        {
            ReadData();

            CreatePolyLine Command = new CreatePolyLine();

            Command.CreateColumnPolyLine();
        }
        // 버튼 > 키라인 입력
        private void btn_keyline_Click(object sender, EventArgs e)
        {
            ReadData();

            CreatePolyLine Command = new CreatePolyLine();

            Command.CreateKeyLine();
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
            //Doc = Application.DocumentManager.MdiActiveDocument;
            //Db = Doc.Database;
            //DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);

            //Editor Editor = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult acPSR = AC.Editor.GetSelection();

            // 선택한 객체를 받음
            if (acPSR.Status == PromptStatus.OK)
            {
                var edgePtrs = new Curve2dCollection();
                var edgeTypes = new IntegerCollection();

                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForWrite) as BlockTable;
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
                    var CenterP = CADUtil.GetCenterPoint2d(c.StartPoint, c.EndPoint);

                    var curves = from a in edgePtrs.Cast<Curve2d>().ToList()
                                 where a != c
                                 select a;

                    // c와 평행한 선을 받음
                    var MatchedCurves = from a in curves
                                        let d = a.GetDistanceTo(c)
                                        where CADUtil.GetVector(a).GetNormal().IsEqualTo(-CADUtil.GetVector(c).GetNormal())
                                        where d > Min && d < Max
                                        let cp1 = CADUtil.GetCenterPoint2d(c)
                                        let cp2 = CADUtil.GetCenterPoint2d(a)
                                        orderby cp1.GetDistanceTo(cp2) ascending
                                        select a;

                    if (MatchedCurves.Any())
                    {
                        //CAD.CreateLine(c.StartPoint, c.EndPoint);

                        bool B = true;

                        MatchedCurves.ToList().ForEach(c1 =>
                        {
                            var cp1 = CADUtil.GetCenterPoint2d(c1);

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

                            usedCurve.Add(CADUtil.GetCenterPoint2d(c));
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
            Properties.Settings.Default.t_extend = txt_extend.Text;
            Properties.Settings.Default.t_distance = txt_distance.Text;
            Properties.Settings.Default.t_distance2 = txt_dis2.Text;

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

                var Vec1 = CADUtil.GetVector(C1);
                var Vec2 = CADUtil.GetVector(Points[0], Points[1]);

                if (Vec1.GetAngleTo(Vec2) > Math.PI / 2)
                {
                    Vec1 = -Vec1;
                }

                double Ang = Math.Abs(Vec1.GetAngleTo(Vec2));

                var P1 = CADUtil.ToPoint3D(Points[0]);
                var P2 = CADUtil.ToPoint3D(Points[1]);

                CADUtil.CreateRectangle(P1, P2, Vec1, Ang);
            }
        }




        #endregion
    }
}
