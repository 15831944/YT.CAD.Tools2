#region .NET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

#region CAD
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Windows;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;
#endregion

namespace YT_CAD_TOOL
{
    public class Command_Palette
    {
        private static PaletteSet m_paletteset = null;
        private static MenuPalette m_menupalatte = null;

        [CommandMethod("YTPALATTE")]
        public void MyCommand()
        {
            m_paletteset = new PaletteSet("YT CAD");
            m_paletteset.Visible = true;

            m_paletteset.Size = new System.Drawing.Size(320, 800);

            m_paletteset.Style = PaletteSetStyles.ShowCloseButton |
                                 PaletteSetStyles.Snappable |
                                 PaletteSetStyles.ShowAutoHideButton;

            m_paletteset.DockEnabled = DockSides.Left | DockSides.Right;
            m_paletteset.Dock = DockSides.Right;
            m_menupalatte = new MenuPalette();

            m_paletteset.Add("YT TEST", m_menupalatte);
            m_paletteset.KeepFocus = true;
        }
    }
}
