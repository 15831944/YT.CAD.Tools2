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
    public partial class U_LogIn : Form
    {
        #region 속성
        #endregion

        #region 생성자
        public U_LogIn()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            Load += U_LogIn_Load;
            sb_login.Click += Sb_login_Click;
        }
        #endregion

        #region 초기화
        private void U_LogIn_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region 이벤트
        private void Sb_login_Click(object sender, EventArgs e)
        {
            login();
        }
        #endregion

        #region PUBLIC

        #endregion

        #region PRIVATE
        private void login()
        {
            var id = Convert.ToString(te_id.Text);
            var pw = Convert.ToString(te_pw.Text);

            HDCDataContext db = new HDCDataContext();

            var F = from a in db.S_USER.AsEnumerable()
                    where a.LoginID.Equals(id)
                    where a.LoginPW.Equals(pw)
                    select a;

            //if (id == "SL" && pw == "1234")
            if (F.Any())
            {
                DialogResult = DialogResult.OK;

                Close();
            }
            else
            {
                MessageBox.Show("시스템관리자에게 문의하세요", "로그인 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                te_id.Focus();
            }
        }
        #endregion
    }
}
