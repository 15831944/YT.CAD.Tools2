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
    public partial class Page_1_Tool : UserControl
    {

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
        public static Page_1_Tool Instance { get; set; }
        #endregion

        #region 생성자 | 초기화
        public Page_1_Tool()
        {
            InitializeComponent();

            Instance = this;

            this.Load += Page_Tool_Load;

            this.ce_move.CheckedChanged += Ce_move_CheckedChanged;
            this.ce_ShapeRotation.CheckedChanged += Ce_ShapeRotation_CheckedChanged;
            this.ce_AngRotation.CheckedChanged += Ce_Rotation_CheckedChanged;

            this.sb_ConnectLines.Click += Sb_ConnectLines_Click;
            this.sb_RefinePolyline.Click += Sb_RefinePolyline_Click;
            this.sb_AlignText.Click += Sb_AlignText_Click;
            this.sb_AlignTexts.Click += Sb_AlignTexts_Click;
            this.sb_InsertText.Click += Sb_InsertText_Click;
            this.sb_GetType.Click += Sb_GetType_Click;
            this.sb_ExplodeBlock.Click += Sb_ExplodeBlock_Click;
            this.sb_AttachPolyline.Click += Sb_AttachPolyline_Click;
            this.sb_AttachPolylines.Click += Sb_AttachPolylines_Click;
            this.sb_AttachPolylineToLine.Click += Sb_AttachPolylineToLine_Click;
            this.sb_CreateBoundary.Click += Sb_CreateBoundary_Click;
        }

        private void Page_Tool_Load(object sender, EventArgs e)
        {
            te_Size.Text = Properties.Settings.Default.t_size;
            txt_AllowDistance.Text = Properties.Settings.Default.t_AD;

            rotate = chk_rotate.Checked;
            rotate2 = ce_ShapeRotation.Checked;
            rotate2 = ce_AngRotation.Checked;

            move = ce_move.Checked;
            T_angle = te_angle.Text;
            shape = true;

            foreach (var item in Enum.GetNames(typeof(TextPosition)))
            {
                cbo_Position.Properties.Items.Add(item);
            }
        }
        #endregion

        #region 이벤트

        #region 버튼 이벤트

        // 버튼 > 연속된 Line을 Polyline으로 변환
        private void Sb_ConnectLines_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.CreatePolyline();
        }
        // 버튼 > 폴리라인의 불필요한 점 제거
        private void Sb_RefinePolyline_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.SimplifyPolyline();
        }

        // 버튼 > Polyline에 Text 입력
        private void Sb_InsertText_Click(object sender, EventArgs e)
        {
            string text = te_Text.Text;

            double size = double.Parse(te_Size.Text.ToString());

            var A = Enum.GetValues(typeof(TextPosition)).Cast<TextPosition>()
                        .Where(x => x.ToString() == cbo_Position.Text);

            var position = A.Any() ? A.First() : TextPosition.맞춤;

            InsertText.Command(text, size, position);
        }

        // 버튼 > Text를 Polyline에 맞게 정렬(단일객체)
        private void Sb_AlignText_Click(object sender, EventArgs e)
        {
            bool moveTocenter = ce_move.Checked;
            bool byShape = ce_ShapeRotation.Checked;
            bool byAngle = ce_AngRotation.Checked;
            string shape = rg_Shape.SelectedIndex == 0 ? "장축" : "단축";
            string angle = te_angle.Text;

            RotateText.Command_Single(moveTocenter, byShape, shape, byAngle, angle);            
        }
        // 버튼 > Text를 Polyline에 맞게 정렬(복수객체)
        private void Sb_AlignTexts_Click(object sender, EventArgs e)
        {
            bool moveTocenter = ce_move.Checked;
            bool byShape = ce_ShapeRotation.Checked;
            bool byAngle = ce_AngRotation.Checked;
            string shape = rg_Shape.SelectedIndex == 0 ? "장축" : "단축";
            string angle = te_angle.Text;

            RotateText.Command_Multi(moveTocenter, byShape, shape, byAngle, angle);
        }

        // 버튼 > 객체 타입 출력
        private void Sb_GetType_Click(object sender, EventArgs e)
        {
            CAD_Tools Command = new CAD_Tools();

            Command.PrintObjType();
        }
        // 버튼 > Block 분해!!!
        private void Sb_ExplodeBlock_Click(object sender, EventArgs e)
        {
            ReadData();

            CAD_Tools Command = new CAD_Tools();

            Command.ExplodeBlocks();
        }

        // 버튼 > 선택된 Polyline에 Polyline 붙이기
        private void Sb_AttachPolyline_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.AttachPolyline();
        }
        // 버튼 > 선택된 Polyline에 여러 Polyline 붙이기
        private void Sb_AttachPolylines_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.AttachAtSelectedPolyline();
        }
        // 버튼 > 사용자가 그린 선분에 Polyline 붙이기
        private void Sb_AttachPolylineToLine_Click(object sender, EventArgs e)
        {
            ReadData();

            ConnectPolyLine Command = new ConnectPolyLine();

            Command.AttachAtLine();
        }

        // 버튼 > 바운더리 생성
        private void Sb_CreateBoundary_Click(object sender, EventArgs e)
        {
            ReadData();

            CreateBoundary Command = new CreateBoundary();

            Command.Create_Boundaries_Util(double.Parse(AllowDistance));
        }

        #endregion

        #region 체크박스 이벤트
        // 체크박스 > 모양에 따라 회전
        private void Ce_ShapeRotation_CheckedChanged(object sender, EventArgs e)
        {
            if (ce_ShapeRotation.Checked && ce_AngRotation.Checked)
            {
                ce_AngRotation.Checked = false;
            }

            rg_Shape.Enabled = ce_ShapeRotation.Checked ? true : false;
        }
        // 체크박스 > 객체 중앙으로 이동
        private void Ce_move_CheckedChanged(object sender, EventArgs e)
        {
        }
        // 체크박스 > 각도 회전
        private void Ce_Rotation_CheckedChanged(object sender, EventArgs e)
        {
            if (ce_AngRotation.Checked && ce_ShapeRotation.Checked)
            {
                ce_ShapeRotation.Checked = false;
            }
        }
        #endregion

        #endregion

        #region PUBLIC 메서드
        #endregion

        #region PRIVATE 메서드
        private void ReadData()
        {
            // 두번 클릭 필요없이 바로 선택가능하게 해줌
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            Properties.Settings.Default.t_size = te_Size.Text;
            Properties.Settings.Default.t_AD = txt_AllowDistance.Text;

            Properties.Settings.Default.Save();

            AllowDistance = txt_AllowDistance.Text;
            Txt = te_Text.Text;
            T_Size = te_Size.Text;
            T_angle = te_angle.Text;

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
            rotate2 = ce_ShapeRotation.Checked;
            rotate3 = ce_AngRotation.Checked;

            move = ce_move.Checked;
            shape = rg_Shape.SelectedIndex == 0 ? true : false;
        }

        #endregion



    }
}
