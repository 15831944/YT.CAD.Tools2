#region .NET
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
#endregion

#region CAD
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;
#endregion

namespace YT_CAD_TOOL
{
    class CAD_enum
    {
    }

    public enum TextPosition
    {
        왼쪽 = AttachmentPoint.BaseLeft,
        중심 = AttachmentPoint.BaseCenter,
        오른쪽 = AttachmentPoint.BaseRight,
        정렬 = AttachmentPoint.BaseAlign,
        중간 = AttachmentPoint.BaseMid,
        맞춤 = AttachmentPoint.BaseFit,
        맨위왼쪽 = AttachmentPoint.TopLeft,
        맨위중심 = AttachmentPoint.TopMid,
        맨위오른쪽 = AttachmentPoint.TopRight,
        중간왼쪽 = AttachmentPoint.MiddleLeft,
        중간중심 = AttachmentPoint.MiddleMid,
        중간오른쪽 = AttachmentPoint.MiddleRight,
        맨아래왼쪽 = AttachmentPoint.BottomLeft,
        맨아래중심 = AttachmentPoint.BottomMid,
        맨아래오른쪽 = AttachmentPoint.BottomRight,
    }

    //public class BlockType
    //{
    //    public const string Window = "창";
    //    public const string Door = "문";
    //    public const string Furniture = "가구";
    //    public const string Bath = "욕실";

    //}

    public enum BlockType
    {
        창,
        문,
        가구,
        욕실,
    }
}
