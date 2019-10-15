using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
//using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

//[assembly: CommandClass(typeof(YT_CAD_TOOL.RibbonTool))]
//[assembly: ExtensionApplication(typeof(YT_CAD_TOOL.RibbonTool))]

namespace YT_CAD_TOOL
{
    public class RibbonTool : IExtensionApplication
    {
        public void Initialize()
        {
            RibbonTab_LogIn();
        }

        public void Terminate()
        {
        }

        [CommandMethod("LOADRIBBON")]
        public void RibbonTab_LogIn()
        {
            #region #1 리본 컨트롤 [RC]
            RibbonControl RC = ComponentManager.Ribbon;

            if (RC == null)
            {
                return;
            }
            #endregion

            #region #2 탭 추가 [RT]
            RibbonTab RT = new RibbonTab();

            var A = RC.Tabs.Where(a => a.Name == "HDC");

            if (A.Any())
            {
                RT = A.First();
            }
            else
            {
                RT = new RibbonTab();
                RT.Name = "HDC";
                RT.Title = "HDC";
                RT.Id = "HDC ID";
                RC.Tabs.Add(RT);
            }
            #endregion

            #region #3 패널 소스 생성 [RPS]
            RibbonPanelSource RPS1 = new RibbonPanelSource();

            var B = RT.Panels.Where(a => a.Source.Name == "설정");

            if (B.Any())
            {
                RPS1 = B.First().Source;
            }
            else
            {
                RPS1.Name = "설정";
                RPS1.Title = "설정";
                RPS1.Id = "MyPanelId_1";
            }
            #endregion

            #region #4 버튼 생성 [RB]

            #region 버튼1 > 로그인
            RibbonButton RB_LogIn = new RibbonButton();

            var C = RPS1.Items.Where(a => a.Name == "로그인");

            if (C.Any())
            {
                RB_LogIn = C.First() as RibbonButton;
            }
            else
            {
                RB_LogIn.Name = "로그인";
                RB_LogIn.Text = "로그인";
                RB_LogIn.Id = "Btn_LogIn";
                RB_LogIn.ShowText = true;
                RB_LogIn.ShowImage = true;
                RB_LogIn.IsEnabled = true;
                RB_LogIn.IsVisible = true;
                RB_LogIn.Image = Images.getBitmap(Properties.Resources.LogIn_03_png_32x32);
                RB_LogIn.LargeImage = Images.getBitmap(Properties.Resources.LogIn_03_png_32x32);
                RB_LogIn.Orientation = System.Windows.Controls.Orientation.Vertical;
                RB_LogIn.Size = RibbonItemSize.Large;
                RB_LogIn.CommandHandler = new RibbonCommandHandler();
            };
            #endregion

            #region 버튼2 > 로그아웃
            RibbonButton RB_LogOut = new RibbonButton();

            var D = RPS1.Items.Where(a => a.Name == "로그아웃");

            if (D.Any())
            {
                RB_LogOut = D.First() as RibbonButton;
            }
            else
            {
                RB_LogOut.Name = "로그아웃";
                RB_LogOut.Text = "로그아웃";
                RB_LogOut.Id = "Btn_LogOut";
                RB_LogOut.ShowText = true;
                RB_LogOut.ShowImage = true;
                RB_LogOut.IsEnabled = false;
                RB_LogOut.IsVisible = false;
                RB_LogOut.Image = Images.getBitmap(Properties.Resources.LogOut_03_png_32x32);
                RB_LogOut.LargeImage = Images.getBitmap(Properties.Resources.LogOut_03_png_32x32);
                RB_LogOut.Orientation = System.Windows.Controls.Orientation.Vertical;
                RB_LogOut.Size = RibbonItemSize.Large;
                RB_LogOut.CommandHandler = new RibbonCommandHandler();
            }
            #endregion

            #endregion

            #region #5 패널 소스에 버튼 추가
            if (!RPS1.Items.Contains(RB_LogIn)) RPS1.Items.Add(RB_LogIn);
            if (!RPS1.Items.Contains(RB_LogOut)) RPS1.Items.Add(RB_LogOut);
            #endregion

            #region #6 패널 생성 및 패널에 패널 소스 입력
            RibbonPanel RP1 = new RibbonPanel();

            if (RT.Panels.Any())
            {
                RP1 = RT.Panels.First();
                RP1.Source = RPS1;
            }
            else
            {
                RP1.Source = RPS1;

                RT.Panels.Add(RP1);
            }
            #endregion

            #region #7 리본 탭에 패널 입력

            #endregion
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
            if (parameter is RibbonButton)
            {
                RibbonButton button = parameter as RibbonButton;

                var LogIn = ComponentManager.Ribbon.FindItem("Btn_LogIn", false) as RibbonButton;
                var LogOut = ComponentManager.Ribbon.FindItem("Btn_LogOut", false) as RibbonButton;

                switch (button.Id)
                {
                    case "Btn_LogIn":
                        {
                            var U = new U_LogIn();

                            if (U.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                AC.IsLogIn = true;

                                ActivateButton(LogIn, false);
                                ActivateButton(LogOut, true);
                            }
                        }
                        break;

                    case "Btn_LogOut":
                        {
                            var R = MessageBox.Show("로그아웃 하시겠습니까?", "로그아웃", MessageBoxButton.OKCancel,
                                                                                     MessageBoxImage.None,
                                                                                     MessageBoxResult.OK);

                            if (R == MessageBoxResult.OK)
                            {
                                AC.IsLogIn = false;

                                ActivateButton(LogIn, true);
                                ActivateButton(LogOut, false);
                            }
                        }
                        break;
                }
            }
        }

        private void ActivateButton(RibbonButton RB, bool F)
        {
            RB.IsEnabled = F;
            RB.IsVisible = F;
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
