using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

namespace YT_CAD_TOOL
{
    public partial class U_Layer : Form
    {
        #region 생성자
        public U_Layer()
        {
            InitializeComponent();

            Load += U_Layer_Load;
            sb_Export.Click += Sb_Export_Click;
            sb_Ok.Click += Sb_Ok_Click;
            sb_Cancel.Click += Sb_Cancel_Click;
        }

        private void Sb_Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();

            if (SFD.ShowDialog() == DialogResult.OK)
            {
                GV1.ExportToXlsx(SFD.FileName);
            }
        }

        private void Sb_Ok_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Sb_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        public U_Layer(List<LayerTableRecord> Layers) : this()
        {
            var A = new List<D1>();
            var L = new List<string>();

            Layers.ForEach(l =>
            {
                if (!L.Contains(l.Name))
                {
                    L.Add(l.Name);
                    var color = l.Color.ColorValue;
                    A.Add(new D1(A.Count + 1, l.Name, GetRGB(color), l.Color.ColorIndex, color.R, color.G, color.B));
                }
            });

            GC1.DataSource = A;
        }
        #endregion

        #region 초기화
        private void U_Layer_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region PUBLIC

        #endregion

        #region PRIVATE
        private string GetRGB(Color C)
        {
            string R = C.R.ToString();
            string G = C.G.ToString();
            string B = C.B.ToString();

            return string.Format("{0}, {1}, {2}", R, G, B);
        }
        #endregion

        public class D1
        {
            #region 속성
            public int No { get; set; }
            public string Name { get; set; }
            public string RGB { get; set; }
            public int INDEX { get; set; }
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
            #endregion

            #region 생성자
            public D1()
            {
                No = 0;
                Name = "";
                RGB = "0,0,0";
                INDEX = 0;
                R = 0;
                G = 0;
                B = 0;
            }
            public D1(int no, string name, string rgb, int index, int r, int g, int b)
            {
                this.No = no;
                this.Name = name;
                this.RGB = rgb;
                this.INDEX = index;
                this.R = r;
                this.G = g;
                this.B = b;
            }
            #endregion
        }
    }
}
