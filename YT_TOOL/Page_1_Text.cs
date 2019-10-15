#region .NET
using System;
using System.Collections;
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
using System.Linq.Expressions;
#endregion

namespace YT_CAD_TOOL
{
    public class InsertText
    {
        #region 필드

        #endregion

        #region 속성

        #endregion

        #region 생성자
        public InsertText()
        {

        }
        #endregion

        #region PUBLIC 메서드
        public static List<DBText> Command(string Text, double Size, TextPosition Position)
        {
            var Return = new List<DBText>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                var acSSet = Utils.Select.MultiObjs();
                var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);

                acPolys.ForEach(acPoly =>
                {
                    var CP = Utils.PointUtil.GetCenterP(acPoly);

                    var Vec = Utils.Vector.GetByLong(acPoly);

                    var Ang = Utils.Vector.To2D(Vec).Angle;

                    if (Ang > Math.PI / 2 && Ang <= Math.PI / 2 * 3)
                    {
                        Ang += Math.PI;
                    }

                    Return.Add(CADUtil.CreateText(CP, Size, Ang, Position, Text));
                });

                AC.Editor.PostCommandPrompt();
            }

            return Return;
        }
        #endregion

        #region PRIVATE 메서드

        #endregion
    }

    public class RotateText
    {
        #region 필드

        #endregion

        #region 속성

        #endregion

        #region 생성자
        public RotateText()
        {

        }
        #endregion

        #region PUBLIC 메서드
        public static List<DBText> Command1(string Text, double Size, TextPosition Position)
        {
            var Return = new List<DBText>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                var acSSet = Utils.Select.MultiObjs();
                var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);

                acPolys.ForEach(acPoly =>
                {
                    var CP = Utils.PointUtil.GetCenterP(acPoly);

                    var Vec = Utils.Vector.GetByLong(acPoly);

                    var Ang = Utils.Vector.To2D(Vec).Angle;

                    if (Ang > Math.PI / 2 && Ang <= Math.PI / 2 * 3)
                    {
                        Ang += Math.PI;
                    }

                    Return.Add(CADUtil.CreateText(CP, Size, Ang, Position, Text));
                });

                AC.Editor.PostCommandPrompt();
            }

            return Return;
        }

        public static List<DBText> Command_Single(bool moveTocenter, bool byshape, string shape, bool byangle, string angle)
        {
            var Return = new List<DBText>();

            bool B = true;

            while (B)
            {
                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    #region 폴리라인 선택
                    var acSSet_Poly = Utils.Select.SingleObj("폴리라인 선택");
                    if (acSSet_Poly == null)
                    {
                        B = false;
                        return new List<DBText>();
                    }

                    var acPolys = Utils.Get.GetEntity<Polyline>(acSSet_Poly);
                    if (!acPolys.Any())
                    {
                        B = false;
                        return new List<DBText>();
                    }

                    var acPoly = acPolys.First();
                    #endregion

                    #region 각도 구하기(모양에 따라 | 사용자 지정)
                    double Ang = 0;

                    if (byshape)
                    {
                        var Vec = Vector3d.XAxis;

                        switch (shape)
                        {
                            case "장축":

                                Vec = Utils.Vector.GetByLong(acPoly);

                                Ang = Utils.Vector.To2D(Vec).Angle;

                                break;


                            case "단축":

                                Vec = Utils.Vector.GetByShort(acPoly);

                                Ang = Utils.Vector.To2D(Vec).Angle;

                                break;
                        }
                        // 둔각인 경우
                        if (Ang > Math.PI / 2 && Ang <= Math.PI / 2 * 3)
                        {
                            Ang += Math.PI;
                        }
                    }
                    else if (byangle)
                    {
                        Ang = To.ToRadian(double.Parse(angle));
                    }
                    #endregion

                    #region 텍스트 선택
                    var acSSet_Text = Utils.Select.SingleObj("텍스트 선택");
                    if (acSSet_Text == null)
                    {
                        B = false;
                        return new List<DBText>();
                    }

                    var acTexts = Utils.Get.GetEntity<DBText>(acSSet_Text);
                    if (!acTexts.Any())
                    {
                        B = false;
                        return new List<DBText>();
                    }
                    #endregion

                    #region 텍스트 정렬
                    using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                    {
                        var acText = t.GetObject(acTexts.First().Id, OpenMode.ForWrite) as DBText;

                        if (moveTocenter)
                        {
                            var p = Utils.PointUtil.GetCenterP(acPoly);

                            acText.Position = p;

                            var J = (TextPosition)acText.Justify;

                            if (!acText.IsDefaultAlignment || J == TextPosition.중간
                                                           || J == TextPosition.맨아래중심
                                                           || J == TextPosition.중간중심
                                                           || J == TextPosition.맨위중심)
                            {
                                acText.AlignmentPoint = p;
                            }
                        }

                        if (byshape || byangle)
                        {
                            acText.Rotation = Ang;
                        }

                        Return.Add(acText);

                        t.Commit();
                    }
                    #endregion
                }
            }

            AC.Editor.PostCommandPrompt();

            return Return;
        }

        public static List<DBText> Command_Multi(bool moveTocenter, bool byshape, string shape, bool byangle, string angle)
        {
            var Return = new List<DBText>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                #region 폴리라인 | 텍스트 일괄 선택
                var acSSet = Utils.Select.SingleObj("폴리라인 | 텍스트 일괄 선택");
                if (acSSet == null)
                {
                    return new List<DBText>();
                }

                var acPolys = Utils.Get.GetEntity<Polyline>(acSSet);
                var acTexts = Utils.Get.GetEntity<DBText>(acSSet);

                if (!acPolys.Any() || !acTexts.Any())
                {
                    return new List<DBText>();
                }
                #endregion

                using (Transaction t = AC.DB.TransactionManager.StartTransaction())
                {
                    acTexts.ForEach(actext =>
                    {
                        var position = actext.Position;

                        var A = acPolys.Where(x => Utils.PointUtil.IsInsidePolyline(x, position, true))
                                       .Select(x => x);

                        if (A.Any())
                        {
                            var acPoly = A.First();

                            #region 각도 구하기(모양에 따라 | 사용자 지정)
                            double Ang = 0;

                            if (byshape)
                            {
                                var Vec = Vector3d.XAxis;

                                switch (shape)
                                {
                                    case "장축":

                                        Vec = Utils.Vector.GetByLong(acPoly);

                                        Ang = Utils.Vector.To2D(Vec).Angle;

                                        break;


                                    case "단축":

                                        Vec = Utils.Vector.GetByShort(acPoly);

                                        Ang = Utils.Vector.To2D(Vec).Angle;

                                        break;
                                }
                                // 둔각인 경우
                                if (Ang > Math.PI / 2 && Ang <= Math.PI / 2 * 3)
                                {
                                    Ang += Math.PI;
                                }
                            }
                            else if (byangle)
                            {
                                Ang = To.ToRadian(double.Parse(angle));
                            }
                            #endregion

                            #region 텍스트 정렬
                            var acText = t.GetObject(actext.Id, OpenMode.ForWrite) as DBText;

                            if (moveTocenter)
                            {
                                var p = Utils.PointUtil.GetCenterP(acPoly);

                                acText.Position = p;

                                var J = (TextPosition)acText.Justify;

                                if (!acText.IsDefaultAlignment || J == TextPosition.중간
                                                               || J == TextPosition.맨아래중심
                                                               || J == TextPosition.중간중심
                                                               || J == TextPosition.맨위중심)
                                {
                                    acText.AlignmentPoint = p;
                                }
                            }

                            if (byshape || byangle)
                            {
                                acText.Rotation = Ang;
                            }

                            Return.Add(acText);
                            #endregion
                        }
                    });

                    t.Commit();
                }

                AC.Editor.PostCommandPrompt();
            }

            return Return;
        }
        #endregion

        #region PRIVATE 메서드

        #endregion
    }
}
