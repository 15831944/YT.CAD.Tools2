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
using Autodesk.AutoCAD.Geometry;

namespace YT_CAD_TOOL
{
    public partial class U_Room : Form
    {
        #region 필드
        //ObjectId acDimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Red);                // 레이어 생성
        //ObjectId acDimStyleId = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_Dim_Style");           // 치수 스타일 생성

        bool IsRefresh = false;
        #endregion

        #region 속성

        #endregion

        #region 생성자
        public U_Room()
        {
            InitializeComponent();

            TopMost = true;

            Load += U_Room_Load;

            AC.DB.ObjectModified += DB_ObjectModified;
            FormClosing += U_Room_FormClosing;
            GV.DoubleClick += GV_DoubleClick;
            sb_Link.Click += Sb_Link_Click;
            sb_Refresh.Click += Sb_Refresh_Click;
            sb_Setting.Click += Sb_Setting_Click;
            sb_Select.Click += Sb_Select_Click;
        }
        #endregion

        #region 초기화
        private void U_Room_Load(object sender, EventArgs e)
        {
            DB.Initialize();

            GC.DataSource = DB.ROOM;
        }
        #endregion

        #region 이벤트

        private void Sb_Refresh_Click(object sender, EventArgs e)
        {
            var R = GC.DataSource as List<Room>;

            R.ForEach(r =>
            {
                if (r.Handle == new Handle()) return;

                var handle = r.Handle;

                // Children 제거
                Utils.Get.ObjectIDs(r.ChildrenH).Where(id => !id.IsErased).ToList().ForEach(id => CADUtil.Erase(id));

                r.Children.Clear();
                r.ChildrenH.Clear();

                var ObjId = new ObjectId();
                AC.DB.TryGetObjectId(handle, out ObjId);
                if (ObjId.IsErased)
                {
                    r.IsLink = false;
                    r.Area = 0;
                    return;
                }

                var acPoly = Utils.Get.GetObject<Polyline>(ObjId);
                if (acPoly == null) return;

                r.Area = Math.Round(acPoly.Area / 1000000, 4);

                var cen = Utils.PointUtil.GetCenterP(acPoly);
                var pos1 = new Point3d(cen.X, cen.Y + 150, cen.Z);
                var pos2 = new Point3d(cen.X, cen.Y - 150, cen.Z);

                // 이름 적기
                r.ChildrenH.Add(CADUtil.CreateText(pos1, 200, r.Name, ColorIndex.Red).Handle);

                // 길이 적기
                var objIds = Utils.DIMUtil.ReCreate(acPoly, DB.DimLayerId, DB.DimStyleId);
                r.ChildrenH.AddRange(Utils.Get.ObjHandles(objIds));

                // 면적 적기
                r.ChildrenH.Add(CADUtil.CreateText(pos2, 200, Math.Round(r.Area, 4).ToString(), ColorIndex.Red).Handle);
            });

            GV.RefreshData();

            DB.ROOM = GC.DataSource as List<Room>;
        }

        private void Sb_Setting_Click(object sender, EventArgs e)
        {
            var pu = new U005();

            if (pu.ShowDialog() != DialogResult.OK) return;

            var rooms = new List<Room>();

            pu.R.ForEach(r =>
            {
                var A = DB.ROOM.Where(a => a.Name == r.Name);

                if (A.Any())
                {
                    A.First().No = rooms.Count + 1;
                    rooms.Add(A.First());
                }
                else
                {
                    r.No = rooms.Count + 1;
                    rooms.Add(r);
                }
            });

            GC.DataSource = rooms;

            DB.ROOM = GC.DataSource as List<Room>;
        }

        private void Sb_Select_Click(object sender, EventArgs e)
        {
            var A = Utils.Get.GetEntity<Polyline>(Utils.Select.MultiObjs("실 공간을 선택해주세요."));

            if (A.Count == 0) return;

            A.Where(a => DB.LAYER.Contains(a.Layer)).ToList().ForEach(a=>
            {

            });


        }

        private void U_Room_FormClosing(object sender, FormClosingEventArgs e)
        {
            AC.DB.ObjectModified -= DB_ObjectModified;
        }

        private void DB_ObjectModified(object sender, Autodesk.AutoCAD.DatabaseServices.ObjectEventArgs e)
        {
            if (!ce_Refresh.Checked) return;
            if (e.DBObject.Id.IsErased) return;

            RefreshRoom(e.DBObject.Handle);
        }

        private void Sb_Link_Click(object sender, EventArgs e)
        {
            LinkRoom();
        }

        private void GV_DoubleClick(object sender, EventArgs e)
        {
            LinkRoom();
        }
        #endregion

        #region PUBLIC
        #endregion

        #region PRIVATE

        void LinkRoom()
        {
            // F = GC.SelectedRows
            // f = GC.GetFocusedRow();

            #region 선택
            var F = Utils.GC.GetSelectedRows<Room>(GV);
            if (F.Count == 0) return;
            #endregion

            #region 치수
            foreach (Room f in F)
            {
                var msg = string.Format("실 {0} 을/를 선택하세요", f.Name);

                if (new U_Msgbox("실(공간) 선택", msg).ShowDialog() != DialogResult.OK) break;

                var acSSet = Utils.Select.SingleObj(string.Format("실 {0} 를 선택하세요", f.Name));
                if (acSSet == null) continue;

                var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);
                if (acPolys.Count == 0) continue;

                var acPoly = acPolys.First();
                if (acPoly.Id == ObjectId.Null) continue;

                f.Id = acPoly.Id;
                f.Handle = acPoly.Handle;
                f.IsLink = true;
                f.Area = Math.Round(acPoly.Area / 1000000, 4);

                var A = F.Where(a => a.Handle.Equals(f.Handle));

                if (A.Any())
                {
                    RefreshRoom(f.Handle);
                }
                else
                {
                    var cen = Utils.PointUtil.GetCenterP(acPoly);
                    var pos1 = new Point3d(cen.X, cen.Y + 150, cen.Z);
                    var pos2 = new Point3d(cen.X, cen.Y - 150, cen.Z);

                    // 이름 적기
                    f.ChildrenH.Add(CADUtil.CreateText(pos1, 200, f.Name, ColorIndex.Red).Handle);

                    // 길이 적기
                    var objIds = Utils.DIMUtil.ReCreate(acPoly, DB.DimLayerId, DB.DimStyleId);
                    f.ChildrenH.AddRange(Utils.Get.ObjHandles(objIds));

                    // 면적 적기
                    f.ChildrenH.Add(CADUtil.CreateText(pos2, 200, Math.Round(f.Area, 4).ToString(), ColorIndex.Red).Handle);
                }
            }
            #endregion

            GV.RefreshData();

            DB.ROOM = GC.DataSource as List<Room>;
        }

        void RefreshRoom(Handle handle)
        {
            var R = GC.DataSource as List<Room>;

            var A = R.Where(a => a.Handle.Equals(handle));
            if (!A.Any()) return;

            var r = A.First();

            // Children 제거
            CADUtil.Erases(Utils.Get.ObjectIDs(r.ChildrenH));

            r.Children.Clear();
            r.ChildrenH.Clear();

            var ObjId = new ObjectId();
            AC.DB.TryGetObjectId(handle, out ObjId);

            var acPoly = Utils.Get.GetObject<Polyline>(ObjId);
            if (acPoly == null) return;

            r.Area = Math.Round(acPoly.Area / 1000000, 4);

            var cen = Utils.PointUtil.GetCenterP(acPoly);
            var pos1 = new Point3d(cen.X, cen.Y + 150, cen.Z);
            var pos2 = new Point3d(cen.X, cen.Y - 150, cen.Z);

            // 이름 적기
            r.ChildrenH.Add(CADUtil.CreateText(pos1, 200, r.Name, ColorIndex.Red).Handle);

            // 길이 적기
            var objIds = Utils.DIMUtil.ReCreate(acPoly, DB.DimLayerId, DB.DimStyleId);
            r.ChildrenH.AddRange(Utils.Get.ObjHandles(objIds));

            // 면적 적기
            r.ChildrenH.Add(CADUtil.CreateText(pos2, 200, Math.Round(r.Area, 4).ToString(), ColorIndex.Red).Handle);

            GV.RefreshData();

            DB.ROOM = GC.DataSource as List<Room>;
        }

        void RefreshRoom(ObjectId id)
        {
            var R = GC.DataSource as List<Room>;

            var A = R.Where(x => x.Id.Equals(id));
            if (!A.Any()) return;

            var r = A.First();

            //MessageBox.Show("테스트1");
            r.Children.ForEach(cid => CADUtil.Erase(cid));

            //MessageBox.Show("테스트2");
            r.Children.Clear();

            var acPoly = Utils.Get.GetObject<Polyline>(id);
            if (acPoly == null) return;

            r.Area = Math.Round(acPoly.Area / 1000000, 4);

            var cen = Utils.PointUtil.GetCenterP(acPoly);
            var pos1 = new Point3d(cen.X, cen.Y + 150, cen.Z);
            var pos2 = new Point3d(cen.X, cen.Y - 150, cen.Z);

            //var acDimLayerId = Utils.Layer.Create("HDC_Dimension", ColorIndex.Green);    // 레이어 생성
            //var acDimStyleId = Utils.DIMUtil.Create_HDC_DimensionStyle("HDC_Dim_Style");             // 치수 스타일 생성

            // 이름 적기
            r.Children.Add(CADUtil.CreateText(pos1, 200, r.Name).Id);

            // 길이 적기
            r.Children.AddRange(Utils.DIMUtil.ReCreate(acPoly, DB.DimLayerId, DB.DimStyleId));

            // 면적 적기
            //r.Children.Add(AC.Drawing.CreateText(pos2, Math.Round(r.Area, 4).ToString()));

            GV.RefreshData();

            DB.ROOM = GC.DataSource as List<Room>;
        }

        #endregion
    }
}
