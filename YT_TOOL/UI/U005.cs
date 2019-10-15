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
    public partial class U005 : Form
    {
        #region 속성
        public List<Room> R { get; set; }
        #endregion

        #region 생성자
        public U005()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            Load += U005_Load;

            sbOk.Click += SbOk_Click;
            sbCancel.Click += SbCancel_Click;
            sbSelect.Click += SbSelect_Click;
        }

        #endregion

        #region 초기화
        private void U005_Load(object sender, EventArgs e)
        {
            Initialize();

            //R = new List<Room>();

            //var A = new List<D1>();
            //A.Add(new D1(1, "거실", 1, true));
            //A.Add(new D1(2, "주방", 1, true));
            //A.Add(new D1(3, "식당", 1, true));
            //A.Add(new D1(4, "현관", 1, true));
            //A.Add(new D1(5, "복도", 1, true));
            //A.Add(new D1(6, "욕실", 1, true));
            //A.Add(new D1(7, "안방", 1, true));
            //A.Add(new D1(8, "침실", 1, true));
            //A.Add(new D1(9, "발코니", 1, true));
            //A.Add(new D1(10, "다용도실", 1, true));
            //A.Add(new D1(11, "실외기실", 1, true));
            //A.Add(new D1(12, "대피공간", 1, true));
            //A.Add(new D1(13, "드레스룸", 1, true));
            //A.Add(new D1(14, "파우더룸", 1, true));
            //A.Add(new D1(15, "가족실", 1, true));
            //A.Add(new D1(16, "펜트리", 1, true));
            //A.Add(new D1(17, "덕트", 1, true));

            //GC.DataSource = A;
        }

        private void Initialize()
        {
            R = new List<Room>();

            var A = new List<D1>();
            A.Add(new D1(1, "거실", GetCount("거실"), true));
            A.Add(new D1(2, "주방", GetCount("주방"), true));
            A.Add(new D1(3, "식당", GetCount("식당"), true));
            A.Add(new D1(4, "현관", GetCount("현관"), true));
            A.Add(new D1(5, "복도", GetCount("복도"), true));
            A.Add(new D1(6, "욕실", GetCount("욕실"), true));
            A.Add(new D1(7, "안방", GetCount("안방"), true));
            A.Add(new D1(8, "침실", GetCount("침실"), true));
            A.Add(new D1(9, "발코니", GetCount("발코니"), true));
            A.Add(new D1(10, "다용도실", GetCount("다용도실"), true));
            A.Add(new D1(11, "실외기실", GetCount("실외기실"), true));
            A.Add(new D1(12, "대피공간", GetCount("대피공간"), true));
            A.Add(new D1(13, "드레스룸", GetCount("드레스룸"), true));
            A.Add(new D1(14, "파우더룸", GetCount("파우더룸"), true));
            A.Add(new D1(15, "가족실", GetCount("가족실"), true));
            A.Add(new D1(16, "펜트리", GetCount("펜트리"), true));
            A.Add(new D1(17, "덕트", GetCount("덕트"), true));

            GC.DataSource = A;
        }

        private int GetCount(string r)
        {
            return DB.ROOM.Where(a => a.Name.Contains(r)).Count();
        }
        #endregion

        #region 이벤트
        private void SbSelect_Click(object sender, EventArgs e)
        {
            var A = Utils.Get.GetEntity<DBText>(Utils.Select.MultiObjs("실 이름을 선택해주세요."));
            if (A.Count == 0) return;

            var B = from a in A
                    let b = a.TextString
                    orderby b
                    select b;

            if (!B.Any()) return;

            var R = new List<D1>();

            int i = 1;

            DB.ROOMs.ForEach(r =>
            {
                var C = B.Where(b => b.Contains(r)).Select(b => b);

                if (C.Any())
                {
                    R.Add(new D1(i++, r, C.Count(), true));
                }
                else
                {
                    R.Add(new D1(i++, r, 0, true));
                }
            });

            GC.DataSource = R;
        }

        private void SbCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SbOk_Click(object sender, EventArgs e)
        {
            SaveReturns();
        }
        #endregion

        #region PUBLIC

        #endregion

        #region PRIVATE
        void SaveReturns()
        {
            var A = GC.DataSource as List<D1>;

            var n = 1;
            for (int i = 0; i < A.Count; i++)
            {
                var a = A[i];

                if (a.P3 == 0) continue;

                if (a.P3 == 1)
                {
                    R.Add(new Room(n, a.P2, 0, 0, 0, false));
                    n++;
                }
                else
                {
                    for (int j = 0; j < a.P3; j++)
                    {
                        R.Add(new Room(n, string.Format("{0}{1}", a.P2, j + 1), 0, 0, 0, false));
                        n++;
                    }
                }
            }

            DialogResult = DialogResult.OK;
        }
        #endregion


        public class D1
        {
            public int P1 { get; set; }
            public string P2 { get; set; }
            public int P3 { get; set; }
            public bool P4 { get; set; }

            public D1()
            { }

            public D1(int p1, string p2, int p3, bool p4)
            {
                P1 = p1;
                P2 = p2;
                P3 = p3;
                P4 = p4;
            }
        }
    }
}
