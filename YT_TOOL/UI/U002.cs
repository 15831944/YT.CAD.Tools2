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
    public partial class U002 : Form
    {
        #region 필드
        Document acDoc;
        Database acDb;
        Editor acDocEd;
        DocumentLock acDL;
        #endregion

        #region 속성
        public string Txt { get; set; }
        public string T_Size { get; set; }
        public string T_angle { get; set; }
        public string AllowDistance { get; set; }
        public TextPosition T_Position { get; set; }
        public bool rotate { get; set; }
        public bool rotate2 { get; set; }
        public bool rotate3 { get; set; }

        public bool move { get; set; }
        public bool shape { get; set; }
        #endregion

        #region 싱글턴
        public static U002 Instance { get; set; }
        #endregion

        #region 생성자 | 초기화
        public U002()
        {
            InitializeComponent();

            Instance = this;

            #region 이벤트 : 버튼
            sbLineToPL.Click += SbLineToPL_Click;
            sbMergePL.Click += SbMergePL_Click;
            sbTrimPL.Click += sbTrimPL_Click;

            sbGetType.Click += SbGetType_Click;
            sbXBlock.Click += SbXBlock_Click;

            sbInsertText.Click += SbInsertText_Click;

            sbAlignText.Click += SbAlignText_Click;
            sbAlignTexts.Click += SbAlignTexts_Click;

            sbConnectPL.Click += SbConnectPL_Click;
            sbConnectPLtoSL.Click += SbConnectPLtoSL_Click;
            sbConnectPLtoDL.Click += SbConnectPLtoDL_Click;

            sbCreateBound.Click += SbCreateBound_Click;

            sbClose.Click += SbClose_Click;
            #endregion
        }


        void U002_Load(object sender, EventArgs e)
        {
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDb = acDoc.Database;
            acDocEd = acDoc.Editor;

            txt_Size.Text = Properties.Settings.Default.t_size;
            chk_Top.Checked = Properties.Settings.Default.c_top2;
            txt_AllowDistance.Text = Properties.Settings.Default.t_AD;

            rotate = chk_rotate.Checked;
            rotate2 = chk_rotate2.Checked;
            rotate2 = chk_rotate3.Checked;

            move = chk_move.Checked;
            T_angle = txt_angle.Text;
            this.TopMost = chk_Top.Checked;
            shape = true;

            foreach (var item in Enum.GetNames(typeof(TextPosition)))
            {
                cbo_Position.Properties.Items.Add(item);
            }
        }
        #endregion

        #region 이벤트

        #region 이벤트 : 버튼
        void SbMergePL_Click(object sender, EventArgs e)
        {
            ReadData();

            var Command = new ConnectPolyLine();

            Command.SimplifyPolyline();
        }

        void SbLineToPL_Click(object sender, EventArgs e)
        {
            ReadData();

            var Command = new ConnectPolyLine();

            Command.CreatePolyline();
        }

        void sbTrimPL_Click(object sender, EventArgs e)
        {
            ReadData();

            TrimSelectedPolylines.Command();

        }

        void SbXBlock_Click(object sender, EventArgs e)
        {
            ReadData();

            CAD_Tools Command = new CAD_Tools();

            Command.ExplodeBlocks();
        }

        void SbGetType_Click(object sender, EventArgs e)
        {
            CAD_Tools Command = new CAD_Tools();

            Command.PrintObjType();
        }

        void SbInsertText_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateText Command = new CreateText();

            Command.CreateText_Polyline_Center();
        }

        void SbAlignText_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateText Command = new CreateText();

            Command.RotateText_by_Polyline();
        }

        void SbAlignTexts_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateText Command = new CreateText();

            Command.RotateTexts_by_Polyline();
        }

        void SbConnectPL_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.AttachPolyline();
        }

        void SbConnectPLtoSL_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.AttachAtSelectedPolyline();
        }

        void SbConnectPLtoDL_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.AttachAtLine();
        }

        void SbCreateBound_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateBoundary Command = new CreateBoundary();

            Command.Create_Boundaries_Util(double.Parse(AllowDistance));
        }

        void SbClose_Click(object sender, EventArgs e)
        {
            ReadData();

            this.Close();
        }

        void btn_Play5_Click(object sender, EventArgs e)
        {
            // 버튼 > Polyline 내부의 점 확인
            ReadData();

            CreateText Command = new CreateText();

            Command.IsInsideAPolyline();
        }
        #endregion

        #region 이벤트 : 체크박스
        void chk_Top_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.c_top2 = chk_Top.Checked;

            Properties.Settings.Default.Save();

            if (chk_Top.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        void chk_rotate2_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_rotate2.Checked && chk_rotate3.Checked)
            {
                chk_rotate3.Checked = false;
            }

            rdoShape.Enabled = chk_rotate2.Checked ? true : false;
        }

        void chk_rotate3_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_rotate3.Checked && chk_rotate2.Checked)
            {
                chk_rotate2.Checked = false;
            }
        }
        #endregion

        #endregion

        #region PRIVATE 메서드
        void ReadData()
        {
            // 두번 클릭 필요없이 바로 선택가능하게 해줌
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            Properties.Settings.Default.c_top2 = chk_Top.Checked;
            Properties.Settings.Default.t_size = txt_Size.Text;
            Properties.Settings.Default.t_AD = txt_AllowDistance.Text;

            Properties.Settings.Default.Save();

            AllowDistance = txt_AllowDistance.Text;
            Txt = txt_PLineText.Text;
            T_Size = txt_Size.Text;
            T_angle = txt_angle.Text;

            var TPs = Enum.GetValues(typeof(TextPosition)).GetEnumerator();
            while (TPs.MoveNext())
            {
                var TP = TPs.Current;

                if (TP.ToString() == cbo_Position.Text)
                {
                    T_Position = (TextPosition)TP;
                }
            }

            rotate = chk_rotate.Checked;
            rotate2 = chk_rotate2.Checked;
            rotate3 = chk_rotate3.Checked;

            move = chk_move.Checked;
            shape = rdoShape.SelectedIndex == 0 ? true : false;
        }

        #endregion

        #region 이벤트 : 버튼(테스트)
        // 버튼 > 바운더리 테스트
        void simpleButton6_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.ReflectVector();
        }
        // 버튼 > 면적 입력
        void simpleButton7_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateText Command = new CreateText();

            Command.Create_PolylineAreaText();
        }
        // 버튼 > 면적 일괄 입력
        void simpleButton8_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateText Command = new CreateText();

            Command.Create_PolylineAreaTexts();
        }
        // 버튼 > 치수선 입력
        void simpleButton9_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateDimensions Command = new CreateDimensions();

            Command.CreatePolylineDimensions();
        }
        // 버튼 > 문자 스타일 생성
        void simpleButton10_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateDimensions Command = new CreateDimensions();

            Command.CreateDimensionStyle();
        }

        void simpleButton11_Click(object sender, EventArgs e)
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            //SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR1 = acDocEd.GetSelection(opt);

            Polyline L1 = new Polyline();

            var PLs = new List<Polyline>();

            #region 폴리라인 추출
            using (Transaction T = acDb.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var acLines1 = from objID in acPSR1.Value.GetObjectIds()
                               let acEnt = T.GetObject(objID, OpenMode.ForWrite) as Entity
                               where acEnt is Polyline
                               select acEnt as Polyline;

                if (acLines1.Any())
                {
                    L1 = acLines1.First();
                    PLs.AddRange(acLines1);
                }

                T.Commit();
            }
            #endregion

            var LayerId = Utils.Layer.GetID("_UNIT_Wall");

            PLs.ForEach(PL =>
            {
                Cad.Wall.Create(PL, 150, LayerId);
            });
        }
        #endregion
    }
}
