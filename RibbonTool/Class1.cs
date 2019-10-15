#region .NET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;
#endregion

#region AUTOCAD
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Colors;
using Autodesk.Windows;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
#endregion

namespace RibbonTool
{
    public class Class1
    {
        [CommandMethod("YTBIM2")]
        public void YTBIM()
        {
            // 상하관계 : 탭 > 패널 > 패널 소스 > 갤러리 > 버튼 > 툴팁
            // 생성순서 : 탭 -> 갤러리 -> 패널 소스 -> 버튼 -> 툴팁 -> 패널

            // 리본 컨트롤
            RibbonControl RC = ComponentManager.Ribbon;

            #region 탭 추가
            // 메뉴탭에 탭 추가
            RibbonTab RT = new RibbonTab();
            RT.Name = "YT.BIM";
            RT.Title = "YT.BIM";
            RT.Id = "YTBIM ID";
            RC.Tabs.Add(RT);
            #endregion

            #region 갤러리 추가
            // 버튼이 담기는 갤러리 추가

            // #1 콤보박스 갤러리
            RibbonGallery RG1 = new RibbonGallery();
            RG1.Name = "MyGallery1";
            RG1.Id = "MyGalleryId1";
            RG1.DisplayMode = GalleryDisplayMode.ComboBox;

            // #2 라지박스 갤러리
            RibbonGallery RG2 = new RibbonGallery();
            RG2.Name = "MyGallery2";
            RG2.Id = "MyGalleryId2";
            RG2.DisplayMode = GalleryDisplayMode.LargeButton;

            // #3 표준박스 갤러리
            RibbonGallery RG3 = new RibbonGallery();
            RG3.Name = "MyGallery3";
            RG3.Id = "MyGalleryId3";
            RG3.DisplayMode = GalleryDisplayMode.StandardButton;

            // #4 윈도우 갤러리
            RibbonGallery RG4 = new RibbonGallery();
            RG4.Name = "MyGallery4";
            RG4.Id = "MyGalleryId4";
            RG4.DisplayMode = GalleryDisplayMode.Window;

            #endregion

            #region 패널 소스에 갤러리 추가
            // 패널 소스 추가
            RibbonPanelSource RPS = new RibbonPanelSource();
            RPS.Name = "YT AUTO BIM";
            RPS.Title = "YT AUTO BIM";
            RPS.Id = "MyPanelId";
            RPS.Items.Add(RG1);
            RPS.Items.Add(RG2);
            RPS.Items.Add(RG3);
            RPS.Items.Add(RG4);
            #endregion

            #region 버튼 생성
            // 버튼 1
            RibbonButton B1 = new RibbonButton();
            B1.Name = "기둥일람표 작성";
            B1.Text = "기둥 일람표";
            B1.Id = "MyButtonId1";
            B1.ShowText = true;
            B1.ShowImage = true;
            B1.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B1.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B1.Orientation = System.Windows.Controls.Orientation.Vertical;
            B1.Size = RibbonItemSize.Large;
            B1.CommandHandler = new RibbonCommandHandler();

            // 버튼 1의 툴팁
            RibbonToolTip rbnT1 = new RibbonToolTip();
            rbnT1.Command = "Create Column Schedule";
            rbnT1.Title = "YT AUTO COLUMN SCHEDULE";
            rbnT1.Content = "Create column schedule automatically";
            rbnT1.ExpandedContent = "In the opened window, input column data and click Just one button.";
            B1.ToolTip = rbnT1;


            // 버튼2
            RibbonButton B2 = new RibbonButton();
            B2.Name = "MyButton2";
            B2.Text = "My Button2";
            B2.Id = "MyButtonId2";
            B2.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B2.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B2.Size = RibbonItemSize.Large;
            B2.Orientation = System.Windows.Controls.Orientation.Vertical;
            B2.ShowText = true;


            // 버튼3
            RibbonButton B3 = new RibbonButton();
            B3.Name = "MyButton3";
            B3.Text = "My Button3";
            B3.Id = "MyButtonId3";
            B3.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B3.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B3.Size = RibbonItemSize.Large;
            B3.Orientation = System.Windows.Controls.Orientation.Vertical;
            B3.ShowText = true;


            // 버튼4
            RibbonButton B4 = new RibbonButton();
            B4.Name = "MyButton4";
            B4.Text = "My Button4";
            B4.Id = "MyButtonId4";
            B4.Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B4.LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_);
            B4.Size = RibbonItemSize.Large;
            B4.Orientation = System.Windows.Controls.Orientation.Vertical;
            B4.ShowText = true;
            #endregion

            #region 갤러리에 버튼 추가
            RG1.Items.Add(B1);
            RG2.Items.Add(B1);
            RG3.Items.Add(B1);
            RG4.Items.Add(B1);

            RG1.Items.Add(B2);
            RG2.Items.Add(B2);
            RG3.Items.Add(B2);
            RG4.Items.Add(B2);

            RG1.Items.Add(B3);
            RG2.Items.Add(B3);
            RG3.Items.Add(B3);
            RG4.Items.Add(B3);

            RG1.Items.Add(B4);
            RG2.Items.Add(B4);
            RG3.Items.Add(B4);
            RG4.Items.Add(B4);
            #endregion

            #region 패널에 일반 버튼 추가
            // 일반 패널
            RibbonRowPanel RRP = new RibbonRowPanel();
            RRP.Items.Add(B1);
            RRP.Items.Add(new RibbonSeparator());
            RRP.Items.Add(B2);
            RRP.Items.Add(B3);
            RRP.Items.Add(B4);

            RPS.Items.Add(RRP);

            // 탭에 패널 추가
            RibbonPanel RP = new RibbonPanel();
            RP.Source = RPS;

            RT.Panels.Add(RP);
            //RT.Panels.Add(RPS);
            #endregion

        }

        [CommandMethod("HDCLOGOUT")]
        public void RibbonTab()
        {
            // 리본 컨트롤
            RibbonControl RC = ComponentManager.Ribbon;

            #region 탭 추가
            RibbonTab RT = new RibbonTab();
            RT.Name = "HDC";
            RT.Title = "HDC";
            RT.Id = "HDC ID";
            RC.Tabs.Add(RT);
            #endregion

            #region 패널 소스에 갤러리 추가
            // 패널 소스 추가
            RibbonPanelSource RPS1 = new RibbonPanelSource();
            RPS1.Name = "설정";
            RPS1.Title = "설정";
            RPS1.Id = "MyPanelId_1";

            RibbonPanelSource RPS2 = new RibbonPanelSource();
            RPS2.Name = "HDC 모듈";
            RPS2.Title = "HDC 모듈";
            RPS2.Id = "MyPanelId_2";
            #endregion


            #region 버튼 생성

            // 버튼 > 로그인
            RibbonButton B0 = new RibbonButton
            {
                Name = "로그아웃",
                Text = "로그아웃",
                Id = "MyButtonId_0",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.LogOut_03_png_32x32),
                LargeImage = Images.getBitmap(Properties.Resources.LogOut_03_png_32x32),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
            };

            bool B = true;

            // 버튼 > POLYLINE 추출 모듈
            RibbonButton B1 = new RibbonButton
            {
                Name = "POLYLINE\r추출 모듈",
                Text = "POLYLINE\r추출 모듈",
                Id = "MyButtonId_1",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 치수 및 면적 검토 모듈
            RibbonButton B2 = new RibbonButton
            {
                Name = "치수 및 면적\r검토 모듈",
                Text = "치수 및 면적\r검토 모듈",
                Id = "MyButtonId_2",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 협의용 도면 자동화 모듈
            RibbonButton B3 = new RibbonButton
            {
                Name = "협의용 도면\r자동화 모듈",
                Text = "협의용 도면\r자동화 모듈",
                Id = "MyButtonId_3",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 인허가용 도면 자동화 모듈
            RibbonButton B4 = new RibbonButton
            {
                Name = "인허가용 도면\r자동화 모듈",
                Text = "인허가용 도면\r자동화 모듈",
                Id = "MyButtonId_4",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 설계기준 검토 모듈
            RibbonButton B5 = new RibbonButton
            {
                Name = "설계 기준\r검토 모듈",
                Text = "설계 기준\r검토 모듈",
                Id = "MyButtonId_5",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 마감 물량 산출 모듈
            RibbonButton B6 = new RibbonButton
            {
                Name = "마감 물량\r산출 모듈2",
                Text = "마감 물량\r산출 모듈2",
                Id = "MyButtonId_6",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            #endregion

            RPS1.Items.Add(B0);

            RPS2.Items.Add(B1);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B2);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B3);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B4);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B5);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B6);

            // 탭에 패널 추가
            RibbonPanel RP1 = new RibbonPanel();
            RP1.Source = RPS1;

            RibbonPanel RP2 = new RibbonPanel();
            RP2.Source = RPS2;

            RT.Panels.Add(RP1);
            RT.Panels.Add(RP2);

        }

        [CommandMethod("HDCLOGIN")]
        public void RibbonTab_LogIn()
        {
            // 리본 컨트롤
            RibbonControl RC = ComponentManager.Ribbon;

            #region 탭 추가
            RibbonTab RT = new RibbonTab();
            RT.Name = "HDC";
            RT.Title = "HDC";
            RT.Id = "HDC ID";
            RC.Tabs.Add(RT);
            #endregion

            #region 패널 소스에 갤러리 추가
            // 패널 소스 추가
            RibbonPanelSource RPS1 = new RibbonPanelSource();
            RPS1.Name = "설정";
            RPS1.Title = "설정";
            RPS1.Id = "MyPanelId_1";

            RibbonPanelSource RPS2 = new RibbonPanelSource();
            RPS2.Name = "HDC 모듈";
            RPS2.Title = "HDC 모듈";
            RPS2.Id = "MyPanelId_2";
            #endregion


            #region 버튼 생성

            // 버튼 > 로그인
            RibbonButton B0 = new RibbonButton
            {
                Name = "로그인",
                Text = "로그인",
                Id = "MyButtonId_0",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.LogIn_03_png_32x32),
                LargeImage = Images.getBitmap(Properties.Resources.LogIn_03_png_32x32),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
            };

            bool B = false;

            // 버튼 > POLYLINE 추출 모듈
            RibbonButton B1 = new RibbonButton
            {
                Name = "POLYLINE\r추출 모듈",
                Text = "POLYLINE\r추출 모듈",
                Id = "MyButtonId_1",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 치수 및 면적 검토 모듈
            RibbonButton B2 = new RibbonButton
            {
                Name = "치수 및 면적\r검토 모듈",
                Text = "치수 및 면적\r검토 모듈",
                Id = "MyButtonId_2",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 협의용 도면 자동화 모듈
            RibbonButton B3 = new RibbonButton
            {
                Name = "협의용 도면\r자동화 모듈",
                Text = "협의용 도면\r자동화 모듈",
                Id = "MyButtonId_3",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 인허가용 도면 자동화 모듈
            RibbonButton B4 = new RibbonButton
            {
                Name = "인허가용 도면\r자동화 모듈",
                Text = "인허가용 도면\r자동화 모듈",
                Id = "MyButtonId_4",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 설계기준 검토 모듈
            RibbonButton B5 = new RibbonButton
            {
                Name = "설계 기준\r검토 모듈",
                Text = "설계 기준\r검토 모듈",
                Id = "MyButtonId_5",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            // 버튼 > 마감 물량 산출 모듈
            RibbonButton B6 = new RibbonButton
            {
                Name = "마감 물량\r산출 모듈2",
                Text = "마감 물량\r산출 모듈2",
                Id = "MyButtonId_6",
                ShowText = true,
                ShowImage = true,
                Image = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                LargeImage = Images.getBitmap(Properties.Resources.기둥일람표2_32x32_),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Size = RibbonItemSize.Large,
                CommandHandler = new RibbonCommandHandler(),
                IsEnabled = B,
            };

            #endregion

            RPS1.Items.Add(B0);

            RPS2.Items.Add(B1);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B2);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B3);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B4);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B5);
            RPS2.Items.Add(new RibbonSeparator());

            RPS2.Items.Add(B6);

            // 탭에 패널 추가
            RibbonPanel RP1 = new RibbonPanel();
            RP1.Source = RPS1;

            RibbonPanel RP2 = new RibbonPanel();
            RP2.Source = RPS2;

            RT.Panels.Add(RP1);
            RT.Panels.Add(RP2);

        }
    }

    public class RibbonCommandHandler : System.Windows.Input.ICommand
    {
        public Action<String> CallBack;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.ShowAlertDialog("실행");

            //if (parameter is RibbonButton)
            //{
            //    RibbonButton button = parameter as RibbonButton;
            //    CallBack(button.Id);
            //}
        }
    }

    public class Images
    {
        public static BitmapImage getBitmap(Bitmap image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();

            return bmp;
        }
    }
}
