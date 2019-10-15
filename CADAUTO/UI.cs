using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADAUTO
{
    public partial class UI : Form
    {
        #region 필드
        double W;
        double H;
        double D;
        double R1;
        double R2;
        double Z;
        #endregion

        #region 생성자 | 초기화
        public UI()
        {
            InitializeComponent();
        }
        private void UI_Load(object sender, EventArgs e)
        {
            LoadValue();
        }
        #endregion

        #region 이벤트

        #region 버튼

        private void btn_Line_Click(object sender, EventArgs e)
        {
            LoadValue();

            var P1 = CAD.PickPoint();
            var P2 = CAD.PickPoint();

            var Line = CAD.CreateLine(P1, P2);
        }

        private void btn_Curve_Click(object sender, EventArgs e)
        {
            LoadValue();

            var P1 = CAD.PickPoint();

            var Arc = CAD.CreateArc(P1, D, R1, R2);
        }

        private void btn_Rectangle_Click(object sender, EventArgs e)
        {
            LoadValue();

            var P1 = CAD.PickPoint();

            var Line = CAD.CreateRectangle(P1, W, H);
        }

        private void btn_Arc_Click(object sender, EventArgs e)
        {
            LoadValue();

            var P1 = CAD.PickPoint();

            var Arc = CAD.CreateArc(P1, D, R1, R2);
        }

        private void btn_zoom_Click(object sender, EventArgs e)
        {
            LoadValue();

            CAD.ZoomExtents(new Autodesk.AutoCAD.Geometry.Point3d(), new Autodesk.AutoCAD.Geometry.Point3d(), Z);
        }
        #endregion

        #endregion

        #region PUBLIC 메서드

        #endregion

        #region PRIVATE 메서드
        private void LoadValue()
        {
            W = Util.ToDouble(txt_W);
            H = Util.ToDouble(txt_H.Text);
            D = Util.ToDouble(txt_D.Text);
            R1 = Util.ToDouble(txt_R1.Text);
            R2 = Util.ToDouble(txt_R2.Text);
            Z = Util.ToDouble(txt_zoom.Text);
        }
        #endregion

        private void btn_Play_Click(object sender, EventArgs e)
        {

        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
