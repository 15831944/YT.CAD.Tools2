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
using System.Data;
using DevExpress.XtraLayout;
using System.Collections.Specialized;
#endregion

#region PROJECT
#endregion

namespace YT_CAD_TOOL
{
    public partial class U003 : Form
    {
        #region 속성
        List<string> ISOLayers { get; set; }
        #endregion

        #region 싱글턴
        public static U003 Instance { get; set; }
        #endregion

        #region 생성자
        public U003()
        {
            InitializeComponent();

            #region 속성값
            this.TopMost = chk_Top.Checked;
            #endregion

            #region 이벤트
            this.Load += UI_Load;

            // Button
            this.btn_Close.Click += Btn_Close_Click;
            this.btn_SaveData.Click += Btn_SaveData_Click;
            this.btn_LoadData.Click += Btn_LoadData_Click;
            this.btn_SaveAsData.Click += Btn_SaveAsData_Click;

            this.barbtn_exportsetting.ItemClick += Barbtn_exportsetting_ItemClick;
            this.barbtn_importsetting.ItemClick += Barbtn_importsetting_ItemClick;
            this.barbtn_close.ItemClick += Barbtn_close_ItemClick;

            // CheckBox
            this.chk_Top.CheckedChanged += Chk_Top_CheckedChanged;

            // ListBox
            this.lst_L.MouseDown += lst_L_MouseDown;
            this.lst_L1.MouseDown += lst_L_MouseDown;
            this.lst_L2.MouseDown += lst_L_MouseDown;
            this.lst_L3.MouseDown += lst_L_MouseDown;
            this.lst_L4.MouseDown += lst_L_MouseDown;
            this.lst_L5.MouseDown += lst_L_MouseDown;
            this.lst_L6.MouseDown += lst_L_MouseDown;
            this.lst_L7.MouseDown += lst_L_MouseDown;
            this.lst_L8.MouseDown += lst_L_MouseDown;
            this.lst_L9.MouseDown += lst_L_MouseDown;
            this.lst_L10.MouseDown += lst_L_MouseDown;

            this.lst_L.MouseMove += lst_L_MouseMove;
            this.lst_L1.MouseMove += lst_L_MouseMove;
            this.lst_L2.MouseMove += lst_L_MouseMove;
            this.lst_L3.MouseMove += lst_L_MouseMove;
            this.lst_L4.MouseMove += lst_L_MouseMove;
            this.lst_L5.MouseMove += lst_L_MouseMove;
            this.lst_L6.MouseMove += lst_L_MouseMove;
            this.lst_L7.MouseMove += lst_L_MouseMove;
            this.lst_L8.MouseMove += lst_L_MouseMove;
            this.lst_L9.MouseMove += lst_L_MouseMove;
            this.lst_L10.MouseMove += lst_L_MouseMove;

            this.lst_L.DragOver += lst_L_DragOver;
            this.lst_L1.DragOver += lst_L_DragOver;
            this.lst_L2.DragOver += lst_L_DragOver;
            this.lst_L3.DragOver += lst_L_DragOver;
            this.lst_L4.DragOver += lst_L_DragOver;
            this.lst_L5.DragOver += lst_L_DragOver;
            this.lst_L6.DragOver += lst_L_DragOver;
            this.lst_L7.DragOver += lst_L_DragOver;
            this.lst_L8.DragOver += lst_L_DragOver;
            this.lst_L9.DragOver += lst_L_DragOver;
            this.lst_L10.DragOver += lst_L_DragOver;

            this.lst_L.DragDrop += lst_L_DragDrop;
            this.lst_L1.DragDrop += lst_L_DragDrop;
            this.lst_L2.DragDrop += lst_L_DragDrop;
            this.lst_L3.DragDrop += lst_L_DragDrop;
            this.lst_L4.DragDrop += lst_L_DragDrop;
            this.lst_L5.DragDrop += lst_L_DragDrop;
            this.lst_L6.DragDrop += lst_L_DragDrop;
            this.lst_L7.DragDrop += lst_L_DragDrop;
            this.lst_L8.DragDrop += lst_L_DragDrop;
            this.lst_L9.DragDrop += lst_L_DragDrop;
            this.lst_L10.DragDrop += lst_L_DragDrop;

            this.lst_L.DoubleClick += Lst_L_DoubleClick;
            this.lst_L1.DoubleClick += Lst_L_DoubleClick;
            this.lst_L2.DoubleClick += Lst_L_DoubleClick;
            this.lst_L3.DoubleClick += Lst_L_DoubleClick;
            this.lst_L4.DoubleClick += Lst_L_DoubleClick;
            this.lst_L5.DoubleClick += Lst_L_DoubleClick;
            this.lst_L6.DoubleClick += Lst_L_DoubleClick;
            this.lst_L7.DoubleClick += Lst_L_DoubleClick;
            this.lst_L8.DoubleClick += Lst_L_DoubleClick;
            this.lst_L9.DoubleClick += Lst_L_DoubleClick;
            this.lst_L10.DoubleClick += Lst_L_DoubleClick;

            // LayoutControlGroup
            this.LCG_Main.CustomButtonClick += LCG_Main_CustomButtonClick;
            this.LCG_0.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_1.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_2.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_3.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_4.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_5.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_6.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_7.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_8.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_9.CustomButtonClick += LCG_CustomButtonClick;
            this.LCG_10.CustomButtonClick += LCG_CustomButtonClick;

            this.LCG_1.DoubleClick += LCG_DoubleClick;
            this.LCG_2.DoubleClick += LCG_DoubleClick;
            this.LCG_3.DoubleClick += LCG_DoubleClick;
            this.LCG_4.DoubleClick += LCG_DoubleClick;
            this.LCG_5.DoubleClick += LCG_DoubleClick;
            this.LCG_6.DoubleClick += LCG_DoubleClick;
            this.LCG_7.DoubleClick += LCG_DoubleClick;
            this.LCG_8.DoubleClick += LCG_DoubleClick;
            this.LCG_9.DoubleClick += LCG_DoubleClick;
            this.LCG_10.DoubleClick += LCG_DoubleClick;

            #endregion

            #region 레이아웃
            LCG_1.Size = new Size(LCG_1.Size.Width, 250);
            LCG_2.Size = new Size(LCG_2.Size.Width, 250);
            #endregion
        }
        #endregion


        #region Bar : 메뉴 : 파일
        // 메뉴바 > 설정 내보내기
        void Barbtn_exportsetting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (SaveFileDialog SFD = new SaveFileDialog())
            {
                SFD.Filter = "config(.config)|*.config";

                if (SFD.ShowDialog() != DialogResult.Cancel)
                {
                    string path = SFD.FileName;

                    System.Configuration.ConfigurationManager.OpenExeConfiguration
                        (System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal).SaveAs(path);
                }
            }
        }
        // 메뉴바 > 설정 가져오기
        void Barbtn_importsetting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog())
            {
                OFD.Filter = "config(.config)|*.config";

                if (OFD.ShowDialog() != DialogResult.Cancel)
                {
                    string from_path = OFD.FileName;
                    string to_path = System.Configuration.ConfigurationManager.OpenExeConfiguration
                        (System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

                    File.Delete(to_path);
                    File.Copy(from_path, to_path);

                    Properties.Settings.Default.Reload();
                }
            }

            Initialization();
        }
        // 메뉴바 > 닫기
        void Barbtn_close_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        #endregion


        // 초기화
        void Initialization()
        {
            #region 초기화
            ISOLayers = new List<string>();

            lst_L1.Items.Clear();
            lst_L2.Items.Clear();
            lst_L3.Items.Clear();
            lst_L4.Items.Clear();
            lst_L5.Items.Clear();
            lst_L6.Items.Clear();
            lst_L7.Items.Clear();
            lst_L8.Items.Clear();
            lst_L9.Items.Clear();
            lst_L10.Items.Clear();

            if (Properties.Settings.Default.layers01 == null || Properties.Settings.Default.layers02 == null ||
                Properties.Settings.Default.layers03 == null || Properties.Settings.Default.layers04 == null ||
                Properties.Settings.Default.layers05 == null || Properties.Settings.Default.layers06 == null ||
                Properties.Settings.Default.layers07 == null || Properties.Settings.Default.layers08 == null ||
                Properties.Settings.Default.layers09 == null || Properties.Settings.Default.layers10 == null)
            {
                Properties.Settings.Default.layers01 = new StringCollection();
                Properties.Settings.Default.layers02 = new StringCollection();
                Properties.Settings.Default.layers03 = new StringCollection();
                Properties.Settings.Default.layers04 = new StringCollection();
                Properties.Settings.Default.layers05 = new StringCollection();
                Properties.Settings.Default.layers06 = new StringCollection();
                Properties.Settings.Default.layers07 = new StringCollection();
                Properties.Settings.Default.layers08 = new StringCollection();
                Properties.Settings.Default.layers09 = new StringCollection();
                Properties.Settings.Default.layers10 = new StringCollection();

            }
            if (Properties.Settings.Default.saveNames == null || Properties.Settings.Default.saveData1 == null ||
                Properties.Settings.Default.saveData2 == null || Properties.Settings.Default.saveData3 == null ||
                Properties.Settings.Default.saveData4 == null || Properties.Settings.Default.saveData5 == null ||
                Properties.Settings.Default.saveData6 == null || Properties.Settings.Default.saveData7 == null ||
                Properties.Settings.Default.saveData8 == null || Properties.Settings.Default.saveData9 == null ||
                Properties.Settings.Default.saveData10 == null)
            {
                Properties.Settings.Default.saveNames = new StringCollection();
                Properties.Settings.Default.saveNames.Add("Default");
                Properties.Settings.Default.saveData1 = new StringCollection();
                Properties.Settings.Default.saveData2 = new StringCollection();
                Properties.Settings.Default.saveData3 = new StringCollection();
                Properties.Settings.Default.saveData4 = new StringCollection();
                Properties.Settings.Default.saveData5 = new StringCollection();
                Properties.Settings.Default.saveData6 = new StringCollection();
                Properties.Settings.Default.saveData7 = new StringCollection();
                Properties.Settings.Default.saveData8 = new StringCollection();
                Properties.Settings.Default.saveData9 = new StringCollection();
                Properties.Settings.Default.saveData10 = new StringCollection();
            }
            #endregion

            #region 콩보상자
            cbo_SaveName.Properties.Items.Clear();
            cbo_SaveName.Properties.Items.AddRange(Properties.Settings.Default.saveNames);
            cbo_SaveName.Properties.Sorted = true;
            cbo_SaveName.Text = "Default";
            #endregion

            #region 리스트 상자
            var AllLayers = CADUtil.GetAllLayerNames();

            var L1 = Properties.Settings.Default.layers01.Cast<object>();
            var L2 = Properties.Settings.Default.layers02.Cast<object>();
            var L3 = Properties.Settings.Default.layers03.Cast<object>();
            var L4 = Properties.Settings.Default.layers04.Cast<object>();
            var L5 = Properties.Settings.Default.layers05.Cast<object>();
            var L6 = Properties.Settings.Default.layers06.Cast<object>();
            var L7 = Properties.Settings.Default.layers07.Cast<object>();
            var L8 = Properties.Settings.Default.layers08.Cast<object>();
            var L9 = Properties.Settings.Default.layers09.Cast<object>();
            var L10 = Properties.Settings.Default.layers10.Cast<object>();

            var L = (from a in AllLayers
                     where !L1.Contains(a)
                     where !L2.Contains(a)
                     where !L3.Contains(a)
                     where !L4.Contains(a)
                     where !L5.Contains(a)
                     where !L6.Contains(a)
                     where !L7.Contains(a)
                     where !L8.Contains(a)
                     where !L9.Contains(a)
                     where !L10.Contains(a)
                     let b = a as object
                     select b).ToArray();

            lst_L.Items.AddRange(L.Where(a => !lst_L.Items.Contains(a)).ToArray());

            lst_L1.Items.AddRange(L1.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L2.Items.AddRange(L2.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L3.Items.AddRange(L3.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L4.Items.AddRange(L4.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L5.Items.AddRange(L5.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L6.Items.AddRange(L6.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L7.Items.AddRange(L7.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L8.Items.AddRange(L8.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L9.Items.AddRange(L9.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());
            lst_L10.Items.AddRange(L10.Where(a => AllLayers.Contains(a) && !L.Contains(a)).Distinct().ToArray());

            #endregion
        }

        #region 이벤트

        #region 이벤트 : 폼
        void UI_Load(object sender, EventArgs e)
        {
            Instance = this;

            Initialization();
        }
        #endregion

        #region 이벤트 : 버튼
        // 버튼 > LayerISO
        void Btn_LayerISO_Click(object sender, EventArgs e)
        {
            ReadData();

            CADUtil.TurnOffAllLayers();
            //CAD.TurnOnLayer(cbo_Layer1.Text);
            //CAD.TurnOnLayer(cbo_Layer2.Text);
            //CAD.TurnOnLayer(cbo_Layer3.Text);
            //CAD.TurnOnLayer(cbo_Layer4.Text);
        }

        // 버튼 > LayerOn
        void Btn_LayerOn_Click(object sender, EventArgs e)
        {
            ReadData();

            CADUtil.TurnOnAllLayers();
        }

        // 버튼 > LayerOff
        void Btn_LayerOff_Click(object sender, EventArgs e)
        {
            ReadData();

            CADUtil.TurnOffAllLayers();
        }

        // 버튼 > Save
        void Btn_SaveData_Click(object sender, EventArgs e)
        {
            var D1 = Properties.Settings.Default.saveData1;
            var D2 = Properties.Settings.Default.saveData2;
            var D3 = Properties.Settings.Default.saveData3;
            var D4 = Properties.Settings.Default.saveData4;
            var D5 = Properties.Settings.Default.saveData5;
            var D6 = Properties.Settings.Default.saveData6;
            var D7 = Properties.Settings.Default.saveData7;
            var D8 = Properties.Settings.Default.saveData8;
            var D9 = Properties.Settings.Default.saveData9;
            var D10 = Properties.Settings.Default.saveData10;

            var name = cbo_SaveName.Text;

            var lst1 = lst_L1.Items.Cast<string>().ToList();
            var lst2 = lst_L2.Items.Cast<string>().ToList();
            var lst3 = lst_L3.Items.Cast<string>().ToList();
            var lst4 = lst_L4.Items.Cast<string>().ToList();
            var lst5 = lst_L5.Items.Cast<string>().ToList();
            var lst6 = lst_L5.Items.Cast<string>().ToList();
            var lst7 = lst_L5.Items.Cast<string>().ToList();
            var lst8 = lst_L5.Items.Cast<string>().ToList();
            var lst9 = lst_L5.Items.Cast<string>().ToList();
            var lst10 = lst_L5.Items.Cast<string>().ToList();

            Properties.Settings.Default.saveData1 = setSaveData(D1, name, lst1);
            Properties.Settings.Default.saveData2 = setSaveData(D2, name, lst2);
            Properties.Settings.Default.saveData3 = setSaveData(D3, name, lst3);
            Properties.Settings.Default.saveData4 = setSaveData(D4, name, lst4);
            Properties.Settings.Default.saveData5 = setSaveData(D5, name, lst5);
            Properties.Settings.Default.saveData6 = setSaveData(D6, name, lst6);
            Properties.Settings.Default.saveData7 = setSaveData(D7, name, lst7);
            Properties.Settings.Default.saveData8 = setSaveData(D8, name, lst8);
            Properties.Settings.Default.saveData9 = setSaveData(D9, name, lst9);
            Properties.Settings.Default.saveData10 = setSaveData(D10, name, lst10);

            Properties.Settings.Default.Save();

            StringCollection setSaveData(StringCollection SC, string Name, List<string> NewData)
            {
                var Return = new StringCollection();

                var matchedData = SC.Cast<string>().ToList().Where(a => a.Split(':').First() == Name);

                if (matchedData.Any())
                {
                    SC.Remove(matchedData.First());

                    string newData = Name + ":";

                    for (int i = 0; i < NewData.Count; i++)
                    {
                        newData += NewData[i] + ":";
                    }

                    SC.Add(newData);
                }

                Return = SC;

                return Return;
            }
        }

        // 버튼 > Load
        void Btn_LoadData_Click(object sender, EventArgs e)
        {
            var D1 = Properties.Settings.Default.saveData1;
            var D2 = Properties.Settings.Default.saveData2;
            var D3 = Properties.Settings.Default.saveData3;
            var D4 = Properties.Settings.Default.saveData4;
            var D5 = Properties.Settings.Default.saveData5;
            var D6 = Properties.Settings.Default.saveData5;
            var D7 = Properties.Settings.Default.saveData5;
            var D8 = Properties.Settings.Default.saveData5;
            var D9 = Properties.Settings.Default.saveData5;
            var D10 = Properties.Settings.Default.saveData5;

            lst_L1.Items.Clear();
            lst_L2.Items.Clear();
            lst_L3.Items.Clear();
            lst_L4.Items.Clear();
            lst_L5.Items.Clear();
            lst_L6.Items.Clear();
            lst_L7.Items.Clear();
            lst_L8.Items.Clear();
            lst_L9.Items.Clear();
            lst_L10.Items.Clear();

            var name = cbo_SaveName.Text;

            lst_L1.Items.AddRange(getSaveData(D1, name).Cast<object>().ToArray());
            lst_L2.Items.AddRange(getSaveData(D2, name).Cast<object>().ToArray());
            lst_L3.Items.AddRange(getSaveData(D3, name).Cast<object>().ToArray());
            lst_L4.Items.AddRange(getSaveData(D4, name).Cast<object>().ToArray());
            lst_L5.Items.AddRange(getSaveData(D5, name).Cast<object>().ToArray());
            lst_L6.Items.AddRange(getSaveData(D6, name).Cast<object>().ToArray());
            lst_L7.Items.AddRange(getSaveData(D7, name).Cast<object>().ToArray());
            lst_L8.Items.AddRange(getSaveData(D8, name).Cast<object>().ToArray());
            lst_L9.Items.AddRange(getSaveData(D9, name).Cast<object>().ToArray());
            lst_L10.Items.AddRange(getSaveData(D10, name).Cast<object>().ToArray());

            ResetListBox();

            List<string> getSaveData(StringCollection SC, string Name)
            {
                var Return = new List<string>();

                var matchedData = SC.Cast<string>().ToList().Where(a => a.Split(':').First() == Name);

                if (matchedData.Any())
                {
                    var datas = matchedData.First().Split(':');

                    for (int i = 1; i < datas.Count(); i++)
                    {
                        if (datas[i] != "" && datas[i] != null)
                        {
                            Return.Add(datas[i]);
                        }
                    }
                }

                return Return;
            }
        }

        // 버튼 > SaveAs
        void Btn_SaveAsData_Click(object sender, EventArgs e)
        {
            #region 판별
            if (txt_SaveName.Text == "" || txt_SaveName.Text == null)
            {
                MessageBox.Show("저장할 이름을 입력하세요.");

                return;
            }
            if (Properties.Settings.Default.saveNames.Contains(txt_SaveName.Text))
            {
                MessageBox.Show(txt_SaveName.Text + "가 이미 존재합니다.");

                return;
            }
            #endregion

            #region 새 이름으로 저장
            var name = txt_SaveName.Text;

            var D1 = name + ":";
            var D2 = name + ":";
            var D3 = name + ":";
            var D4 = name + ":";
            var D5 = name + ":";
            var D6 = name + ":";
            var D7 = name + ":";
            var D8 = name + ":";
            var D9 = name + ":";
            var D10 = name + ":";

            lst_L1.Items.Cast<string>().ToList().ForEach(a => { D1 += a + ":"; });
            lst_L2.Items.Cast<string>().ToList().ForEach(a => { D2 += a + ":"; });
            lst_L3.Items.Cast<string>().ToList().ForEach(a => { D3 += a + ":"; });
            lst_L4.Items.Cast<string>().ToList().ForEach(a => { D4 += a + ":"; });
            lst_L5.Items.Cast<string>().ToList().ForEach(a => { D5 += a + ":"; });
            lst_L6.Items.Cast<string>().ToList().ForEach(a => { D6 += a + ":"; });
            lst_L7.Items.Cast<string>().ToList().ForEach(a => { D7 += a + ":"; });
            lst_L8.Items.Cast<string>().ToList().ForEach(a => { D8 += a + ":"; });
            lst_L9.Items.Cast<string>().ToList().ForEach(a => { D9 += a + ":"; });
            lst_L10.Items.Cast<string>().ToList().ForEach(a => { D10 += a + ":"; });

            Properties.Settings.Default.saveNames.Add(txt_SaveName.Text);
            Properties.Settings.Default.saveData1.Add(D1);
            Properties.Settings.Default.saveData2.Add(D2);
            Properties.Settings.Default.saveData3.Add(D3);
            Properties.Settings.Default.saveData4.Add(D4);
            Properties.Settings.Default.saveData5.Add(D5);
            Properties.Settings.Default.saveData6.Add(D6);
            Properties.Settings.Default.saveData7.Add(D7);
            Properties.Settings.Default.saveData8.Add(D8);
            Properties.Settings.Default.saveData9.Add(D9);
            Properties.Settings.Default.saveData10.Add(D10);

            Properties.Settings.Default.Save();

            cbo_SaveName.Properties.Items.Add(name);
            cbo_SaveName.Properties.Sorted = true;
            #endregion
        }

        // 버튼 > 닫기
        void Btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 이벤트 : LayoutControlGroup
        void LCG_DoubleClick(object sender, EventArgs e)
        {
            var lcg = sender as LayoutControlGroup;

            lcg.Size = new Size(lcg.Size.Width, 250);
        }
        #endregion

        #region 이벤트 : 체크박스
        // 체크상자 > 항상위
        void Chk_Top_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chk_Top.Checked ? true : false;

            Properties.Settings.Default.c_top3 = chk_Top.Checked;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region DRAG & DROP 이벤트
        Point p = Point.Empty;

        DevExpress.XtraEditors.ListBoxControl lstBox_From;
        DevExpress.XtraEditors.ListBoxControl lstBox_To;

        void lst_L_MouseDown(object sender, MouseEventArgs e)
        {
            lstBox_From = sender as DevExpress.XtraEditors.ListBoxControl;

            p = new Point(e.X, e.Y);

            int selectedIndex = lstBox_From.IndexFromPoint(p);

            if (selectedIndex == -1)
            {
                p = Point.Empty;
            }
        }

        void lst_L_MouseMove(object sender, MouseEventArgs e)
        {
            lstBox_From = sender as DevExpress.XtraEditors.ListBoxControl;

            if (e.Button == MouseButtons.Left)
            {
                if ((p != Point.Empty) &&
                    (Math.Abs(e.X - p.X) > SystemInformation.DragSize.Width) || (Math.Abs(e.Y) - p.Y) > SystemInformation.DragSize.Height)
                {
                    if (lstBox_From != null) lstBox_From.DoDragDrop(sender, DragDropEffects.Move);
                }
            }
        }


        void lst_L_DragOver(object sender, DragEventArgs e)
        {
            lstBox_To = sender as DevExpress.XtraEditors.ListBoxControl;

            e.Effect = DragDropEffects.Move;
        }

        void lst_L_DragDrop(object sender, DragEventArgs e)
        {
            lstBox_To = sender as DevExpress.XtraEditors.ListBoxControl;

            Point newPoint = new Point(e.X, e.Y);

            newPoint = lstBox_To.PointToClient(newPoint);

            int selectedIndex = lstBox_To.IndexFromPoint(newPoint);

            if (lstBox_From.Equals(lstBox_To))
                return;

            lstBox_From.SelectedItems.ToList().ForEach(a =>
            {
                if (!lstBox_To.Items.Contains(a)) lstBox_To.Items.Add(a);
                if (lstBox_From.Items.Contains(a)) lstBox_From.Items.Remove(a);
            });

            lstBox_To.SortOrder = SortOrder.Ascending;
        }
        #endregion

        #region 레이아웃 컨트롤 그룹 커스텀 버튼

        // ListBox > 더블클릭
        void Lst_L_DoubleClick(object sender, EventArgs e)
        {
            var LBC = sender as DevExpress.XtraEditors.ListBoxControl;

            string LayerName = LBC.SelectedItem as string;

            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.LayerName), LayerName),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            var selectFilter = new SelectionFilter(tvs);
            #endregion

            var PSR = AC.Editor.SelectAll(selectFilter);

            if (PSR.Status == PromptStatus.OK)
            {
                CADUtil.SetSelected(PSR);

                AC.Editor.WriteMessage("\n'" + LayerName + "' 레이어 객체" + PSR.Value.Count.ToString() + "개 선택. \n");
            }
            else
            {
                AC.Editor.WriteMessage("\n'" + LayerName + "' 레이어 객체가 없습니다. \n");
            }


            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
        }

        // 모든 레이어 켜기 / 끄기 / All Clear / Fitting
        void LCG_Main_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            ReadData();

            switch (e.Button.Properties.Caption)
            {
                case "모든 레이어 켜기": CADUtil.TurnOnAllLayers(); break;

                case "모든 레이어 끄기": CADUtil.TurnOffAllLayers(); break;

                case "C":
                    {
                        ClearListBox(lst_L1);
                        ClearListBox(lst_L2);
                        ClearListBox(lst_L3);
                        ClearListBox(lst_L4);
                        ClearListBox(lst_L5);
                        ClearListBox(lst_L6);
                        ClearListBox(lst_L7);
                        ClearListBox(lst_L8);
                        ClearListBox(lst_L9);
                        ClearListBox(lst_L10);

                    }
                    break;

                case "F":
                    {
                        int n = 0;
                        if (LCG_1.Expanded) n++;
                        if (LCG_2.Expanded) n++;
                        if (LCG_3.Expanded) n++;
                        if (LCG_4.Expanded) n++;
                        if (LCG_5.Expanded) n++;
                        if (LCG_6.Expanded) n++;
                        if (LCG_7.Expanded) n++;
                        if (LCG_8.Expanded) n++;
                        if (LCG_9.Expanded) n++;
                        if (LCG_10.Expanded) n++;

                        int width = LCG_1.Size.Width;
                        int height = LCG_0.Height / n;

                        if (LCG_1.Expanded) LCG_1.Size = new Size(width, height);
                        if (LCG_2.Expanded) LCG_2.Size = new Size(width, height);
                        if (LCG_3.Expanded) LCG_3.Size = new Size(width, height);
                        if (LCG_4.Expanded) LCG_4.Size = new Size(width, height);
                        if (LCG_5.Expanded) LCG_5.Size = new Size(width, height);
                        if (LCG_6.Expanded) LCG_6.Size = new Size(width, height);
                        if (LCG_7.Expanded) LCG_7.Size = new Size(width, height);
                        if (LCG_8.Expanded) LCG_8.Size = new Size(width, height);
                        if (LCG_9.Expanded) LCG_9.Size = new Size(width, height);
                        if (LCG_10.Expanded) LCG_10.Size = new Size(width, height);

                    }
                    break;
            }
        }

        // View / Hide / Clear / Select / Reset
        void LCG_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            ReadData();

            var lcg = sender as LayoutControlGroup;

            ISOLayers = new List<string>();

            var lstBox = new DevExpress.XtraEditors.ListBoxControl();
            var lstLayers = new StringCollection();

            switch (lcg.Items[0].CustomizationFormText)
            {
                case "AllLayers": lstBox = lst_L; break;
                case "Layer 1": lstBox = lst_L1; break;
                case "Layer 2": lstBox = lst_L2; break;
                case "Layer 3": lstBox = lst_L3; break;
                case "Layer 4": lstBox = lst_L4; break;
                case "Layer 5": lstBox = lst_L5; break;
                case "Layer 6": lstBox = lst_L6; break;
                case "Layer 7": lstBox = lst_L7; break;
                case "Layer 8": lstBox = lst_L8; break;
                case "Layer 9": lstBox = lst_L9; break;
                case "Layer 10": lstBox = lst_L10; break;
            }

            ISOLayers.AddRange(lstBox.Items.Cast<string>());

            //MessageBox.Show(ISOLayers[0] + "\n" + e.Button.Properties.Caption);

            if (e.Button.Properties.Caption == "V")
            {
                // View
                // 레이어 켜기
                ISOLayers.ForEach(layer =>
                {
                    Utils.Layer.TurnOff(layer, false);
                    //CAD.TurnOnLayer(layer);
                });
            }
            else if (e.Button.Properties.Caption == "H")
            {
                // Hide
                // 레이어 끄기
                ISOLayers.ForEach(layer =>
                {
                    Utils.Layer.TurnOff(layer, true);
                    //CAD.TurnOffLayer(layer);
                });
            }
            else if (e.Button.Properties.Caption == "I")
            {
                // Isolate
                // 레이어 ISO
                Utils.Layer.ISO(ISOLayers, true);
            }
            else if (e.Button.Properties.Caption == "C")
            {
                // Clear
                // 리스트 Clear
                lst_L.Items.AddRange(ISOLayers.Cast<object>().ToArray());
                lst_L.SortOrder = SortOrder.Ascending;

                lstBox.Items.Clear();
            }
            else if (e.Button.Properties.Caption == "S")
            {
                // Select
                // 객체 선택
                Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

                bool B = true;

                while (B)
                {
                    var Ids = select.MultiObjs().GetObjectIds().ToList();

                    var LayerNames = Get.Entities(Ids).Select(x => x.Layer).Distinct().ToList();

                    //var LayerNames = CADUtil.SelectObjectLayers();

                    if (LayerNames.Count == 0)
                        B = false;

                    LayerNames.ForEach(layer =>
                    {
                        if (lst_L.Items.Contains(layer)) lst_L.Items.Remove(layer);
                        if (!lstBox.Items.Contains(layer))
                        {
                            lstBox.Items.Add(layer);
                            ISOLayers.Add(layer);
                        }
                    });
                }
            }
            else if (e.Button.Properties.Caption == "L")
            {

            }

            else if (e.Button.Properties.Caption == "R")
            {
                // Reset
                ResetListBox();
            }

            ReadData();
        }
        void ClearListBox(DevExpress.XtraEditors.ListBoxControl LBC)
        {
            var layers = from a in LBC.Items.Cast<object>().ToList()
                         where !lst_L.Items.Contains(a)
                         select a;

            if (layers.Any())
            {
                lst_L.Items.AddRange(layers.ToArray());
                lst_L.SortOrder = SortOrder.Ascending;
            }

            LBC.Items.Clear();
        }
        #endregion

        #endregion


        #region PUBLIC 메서드

        #endregion

        #region PRIVATE 메서드

        void ReadData()
        {
            // 두번 클릭 필요없이 바로 선택가능하게 해줌
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            //Properties.Settings.Default.layer01 = cbo_Layer1.Text;
            //Properties.Settings.Default.layer02 = cbo_Layer2.Text;
            //Properties.Settings.Default.layer03 = cbo_Layer3.Text;
            //Properties.Settings.Default.layer04 = cbo_Layer4.Text;

            Properties.Settings.Default.layers01.Clear();
            Properties.Settings.Default.layers02.Clear();
            Properties.Settings.Default.layers03.Clear();
            Properties.Settings.Default.layers04.Clear();
            Properties.Settings.Default.layers05.Clear();
            Properties.Settings.Default.layers06.Clear();
            Properties.Settings.Default.layers07.Clear();
            Properties.Settings.Default.layers08.Clear();
            Properties.Settings.Default.layers09.Clear();
            Properties.Settings.Default.layers10.Clear();

            Properties.Settings.Default.layers01.AddRange(lst_L1.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers02.AddRange(lst_L2.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers03.AddRange(lst_L3.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers04.AddRange(lst_L4.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers05.AddRange(lst_L5.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers06.AddRange(lst_L6.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers07.AddRange(lst_L7.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers08.AddRange(lst_L8.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers09.AddRange(lst_L9.Items.Cast<string>().ToArray());
            Properties.Settings.Default.layers10.AddRange(lst_L10.Items.Cast<string>().ToArray());

            Properties.Settings.Default.Save();
        }

        void ResetListBox()
        {
            ReadData();

            var AllLayers = CADUtil.GetAllLayerNames();

            var L1 = Properties.Settings.Default.layers01.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L2 = Properties.Settings.Default.layers02.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L3 = Properties.Settings.Default.layers03.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L4 = Properties.Settings.Default.layers04.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L5 = Properties.Settings.Default.layers05.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L6 = Properties.Settings.Default.layers05.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L7 = Properties.Settings.Default.layers05.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L8 = Properties.Settings.Default.layers05.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L9 = Properties.Settings.Default.layers05.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));
            var L10 = Properties.Settings.Default.layers05.Cast<object>().Where(a => AllLayers.Contains(a.ToString()));

            var L = (from a in AllLayers
                     where !L1.Contains(a)
                     where !L2.Contains(a)
                     where !L3.Contains(a)
                     where !L4.Contains(a)
                     where !L5.Contains(a)
                     where !L6.Contains(a)
                     where !L7.Contains(a)
                     where !L8.Contains(a)
                     where !L9.Contains(a)
                     where !L10.Contains(a)
                     let b = a as object
                     orderby b
                     select b);

            lst_L.Items.Clear();
            lst_L1.Items.Clear();
            lst_L2.Items.Clear();
            lst_L3.Items.Clear();
            lst_L4.Items.Clear();
            lst_L5.Items.Clear();
            lst_L6.Items.Clear();
            lst_L7.Items.Clear();
            lst_L8.Items.Clear();
            lst_L9.Items.Clear();
            lst_L10.Items.Clear();

            lst_L.Items.AddRange(L.ToArray());
            lst_L1.Items.AddRange(L1.ToArray());
            lst_L2.Items.AddRange(L2.ToArray());
            lst_L3.Items.AddRange(L3.ToArray());
            lst_L4.Items.AddRange(L4.ToArray());
            lst_L5.Items.AddRange(L5.ToArray());
            lst_L6.Items.AddRange(L6.ToArray());
            lst_L7.Items.AddRange(L7.ToArray());
            lst_L8.Items.AddRange(L8.ToArray());
            lst_L9.Items.AddRange(L9.ToArray());
            lst_L10.Items.AddRange(L10.ToArray());
        }

        #endregion
    }
}
