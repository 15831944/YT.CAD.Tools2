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
    public partial class U_Msgbox : Form
    {
        public U_Msgbox()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            sb_ok.Click += Sb_ok_Click;
            sb_cancle.Click += Sb_cancle_Click;
        }

        public U_Msgbox(string text, string msg) : this()
        {
            Text = text;
            lc_msg.Text = msg;
        }

        private void Sb_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void Sb_cancle_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
