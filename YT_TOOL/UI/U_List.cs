using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT_CAD_TOOL
{
    public partial class U_List : Form
    {
        #region 속성
        public string item { get; set; }
        public double size { get; set; }
        #endregion

        #region 생성자
        public U_List()
        {
            InitializeComponent();

            Load += U_List_Load;
            sbOk.Click += SbOk_Click;
            sbCancel.Click += SbCancel_Click;

            LB.DoubleClick += LB_DoubleClick;

            StartPosition = FormStartPosition.CenterScreen;
        }

        public U_List(List<string> items) : this()
        {
            LB.Items.AddRange(items.ToArray());
        }
        #endregion

        #region 초기화
        private void U_List_Load(object sender, EventArgs e)
        {
            te_size.Text = "20";
            LB.SelectedIndex = 0;
        }
        #endregion

        #region 이벤트
        private void SbCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void SbOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            item = LB.SelectedItem.ToString();
            size = double.Parse(te_size.Text);

            Close();
        }

        private void LB_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            item = LB.SelectedItem.ToString();
            size = double.Parse(te_size.Text);

            Close();
        }
        #endregion
    }
}
