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
#endregion

#region PROJECT
#endregion

namespace CADAUTO
{
    public class DivideRectangle
    {
        #region 필드

        #endregion

        #region 속성

        #endregion

        #region 생성자
        public DivideRectangle()
        {

        }
        #endregion

        #region PUBLIC 메서드
        public void Play()
        {

        }
        #endregion

        #region PRIVATE 메서드
        private void SelectPolyline()
        {
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult acPSR;
            
            SelectionSet acSSet;
                    
            acPSR = acDocEd.GetSelection();
            // 선택한 객체를 받음
            if (acPSR.Status == PromptStatus.OK)
            {
                acSSet = acPSR.Value;
                


                Application.ShowAlertDialog("Number of objects selected: " + acSSet.Count.ToString());
            }
            else
            {
                Application.ShowAlertDialog("Number of objects selected: 0");
            }


        }
        #endregion
    }
}
