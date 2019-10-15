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
    public class CreateText
    {
        #region 필드
        string Txt;
        double T_Size;
        TextPosition T_Position;
        #endregion

        #region 생성자
        public CreateText()
        {
            Txt = U002.Instance.Txt;
            T_Size = double.Parse(U002.Instance.T_Size);
            T_Position = U002.Instance.T_Position;
        }
        #endregion

        #region PUBLIC 메서드

        public void CreateText_Polyline_Center()
        {
            var acPolyLines = new List<Polyline>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    #region 폴리라인 선택
                    SelectionSet acSSet = SelectLines();

                    var Lines = from id in acSSet.GetObjectIds()
                                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                where acEnt is Polyline
                                let acLine = acEnt as Polyline
                                select acLine;

                    if (!Lines.Any())
                        return;

                    acPolyLines.AddRange(Lines.ToArray());
                    #endregion
                }
                catch (System.Exception ex)
                {
                    AC.Editor.WriteMessage(Convert.ToString(ex));
                }
            }

            #region 실행
            acPolyLines.ForEach(acPolyline =>
            {
                double X = 0;
                double Y = 0;
                int N = acPolyline.NumberOfVertices;
                double length = 0;
                double ang = 0;

                if (U002.Instance.rotate)
                {
                    for (int i = 0; i < N; i++)
                    {
                        try
                        {
                            var L = acPolyline.GetLineSegment2dAt(i);
                            var CP = CADUtil.GetCenterPoint2d(L);

                            X += CP.X;
                            Y += CP.Y;

                            if (length < L.Length)
                            {
                                length = L.Length;
                                ang = CADUtil.GetVector(L).Angle;
                            }
                        }
                        catch
                        {
                            N = N - 1;
                        }
                    }

                    if (ang > Math.PI / 2 && ang <= Math.PI / 2 * 3)
                    {
                        ang += Math.PI;
                    }
                }

                var P = acPolyline.NumberOfVertices == 4 ? GetCenterPoint(acPolyline) : GetVisualCenterPoint(acPolyline);

                var acText = CADUtil.CreateText(P, T_Size, ang, T_Position, Txt);
            });
            #endregion

            AC.Editor.WriteMessage("\nPolyline 내부에 Text생성이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }

        /// <summary>
        /// 폴리선과 텍스트를 각각 선택하여 폴리선에 맞게 텍스트를 회전한다.
        /// </summary>
        public void RotateText_by_Polyline()
        {
            bool B = true;

            while (B)
            {
                #region PRE
                SelectionSet acSSet1 = SelectAPolyline();

                if (acSSet1 == null)
                {
                    B = false;

                    return;
                }

                SelectionSet acSSet2 = SelectAText();

                if (acSSet2 == null)
                {
                    B = false;

                    return;
                }
                #endregion

                using (DocumentLock DL = AC.Doc.LockDocument())
                {
                    using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                    {
                        #region 폴리라인 선택
                        var Lines = from id in acSSet1.GetObjectIds()
                                    let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                    where acEnt is Polyline
                                    let acLine = acEnt as Polyline
                                    select acLine;

                        if (!Lines.Any())
                            return;
                        #endregion

                        #region 각도 구하기 [ang]
                        var acPolyline = Lines.ToList().First();

                        bool shape = U002.Instance.shape;   // 장축 & 단축 판별
                        double X = 0;
                        double Y = 0;
                        int N = acPolyline.NumberOfVertices;
                        double length = shape ? 0 : double.MaxValue;
                        double ang = 0;

                        if (U002.Instance.rotate2)
                        {
                            for (int i = 0; i < N; i++)
                            {
                                try
                                {
                                    var L = acPolyline.GetLineSegment2dAt(i);
                                    var CP = CADUtil.GetCenterPoint2d(L);

                                    X += CP.X;
                                    Y += CP.Y;

                                    #region 장축
                                    if (shape)
                                    {
                                        // 장축
                                        if (length < L.Length)
                                        {
                                            length = L.Length;
                                            ang = CADUtil.GetVector(L).Angle;
                                        }
                                    }
                                    #endregion

                                    #region 단축
                                    else
                                    {
                                        // 단축
                                        if (length > L.Length)
                                        {
                                            length = L.Length;
                                            ang = CADUtil.GetVector(L).Angle;
                                        }
                                    }
                                    #endregion
                                }
                                catch
                                {
                                    N = N - 1;
                                }
                            }
                            // 둔각인 경우
                            if (ang > Math.PI / 2 && ang <= Math.PI / 2 * 3)
                            {
                                ang += Math.PI;
                            }
                        }
                        else if (U002.Instance.rotate3)
                        {
                            ang = double.Parse(U002.Instance.T_angle) / 180 * Math.PI;
                        }
                        #endregion

                        #region 텍스트 선택
                        var Entities = from id in acSSet2.GetObjectIds()
                                       let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                       where acEnt is DBText
                                       select acEnt;

                        if (!Entities.Any())
                            return;
                        #endregion

                        #region 텍스트 이동 | 회전
                        var acText = T.GetObject(Entities.First().Id, OpenMode.ForWrite) as DBText;

                        if (U002.Instance.move)
                        {
                            var p = acPolyline.NumberOfVertices == 4 ? GetCenterPoint(acPolyline) : GetVisualCenterPoint(acPolyline);

                            MoveText(acText, p);
                        }

                        if (U002.Instance.rotate2 || U002.Instance.rotate3) acText.Rotation = ang;
                        #endregion

                        T.Commit();
                    }
                }
            }

            AC.Editor.WriteMessage("\nText 정렬이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }

        /// <summary>
        /// 폴리선과 텍스트를 한번에 선택하여 폴리선 내부의 텍스트를 회전한다.
        /// </summary>
        public void RotateTexts_by_Polyline()
        {
            var acPolyLines = new List<Polyline>();
            var acTexts = new List<DBText>();

            using (DocumentLock DL = AC.Doc.LockDocument())
            {
                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    #region 폴리라인 | 텍스트 선택
                    SelectionSet acSSet = SelectLinesAndTexts();

                    #region 폴리라인
                    var Polylines = from id in acSSet.GetObjectIds()
                                    let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                    where acEnt is Polyline
                                    let acLine = acEnt as Polyline
                                    select acLine;

                    if (!Polylines.Any())
                    {
                        MessageBox.Show("선택된 폴리선이 없습니다.");
                        return;
                    }

                    acPolyLines.AddRange(Polylines.ToArray());
                    #endregion

                    #region 텍스트 받기
                    var Texts = from id in acSSet.GetObjectIds()
                                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                where acEnt is DBText
                                let acText = acEnt as DBText
                                select acText;

                    if (!Texts.Any())
                    {
                        MessageBox.Show("선택된 텍스트가 없습니다.");
                        return;
                    }

                    acTexts.AddRange(Texts.ToArray());
                    #endregion

                    #endregion

                    acTexts.ForEach(t =>
                    {
                        #region 텍스트가 입력된 폴리라인 찾기
                        var matchedPL = from pl in acPolyLines
                                        where IsInsideInPolyline(pl, t.Position)
                                        select pl;
                        #endregion

                        if (matchedPL.Any())
                        {
                            var acPolyline = matchedPL.First();

                            #region 각도 구하기
                            bool shape = U002.Instance.shape;
                            double X = 0;
                            double Y = 0;
                            int N = acPolyline.NumberOfVertices;
                            double length = shape ? 0 : double.MaxValue;
                            double ang = t.Rotation;

                            if (U002.Instance.rotate2)
                            {
                                for (int i = 0; i < N; i++)
                                {
                                    try
                                    {
                                        var L = acPolyline.GetLineSegment2dAt(i);
                                        var CP = CADUtil.GetCenterPoint2d(L);

                                        X += CP.X;
                                        Y += CP.Y;

                                        #region 장축
                                        if (shape)
                                        {
                                            // 장축
                                            if (length < L.Length)
                                            {
                                                length = L.Length;
                                                ang = CADUtil.GetVector(L).Angle;
                                            }
                                        }
                                        #endregion

                                        #region 단축
                                        else
                                        {
                                            // 단축
                                            if (length > L.Length)
                                            {
                                                length = L.Length;
                                                ang = CADUtil.GetVector(L).Angle;
                                            }
                                        }
                                        #endregion
                                    }
                                    catch
                                    {
                                        N = N - 1;
                                    }
                                }
                                // 둔각일 경우
                                if (ang > Math.PI / 2 && ang <= Math.PI / 2 * 3)
                                {
                                    ang += Math.PI;
                                }
                            }
                            else if (U002.Instance.rotate3)
                            {
                                ang = double.Parse(U002.Instance.T_angle) / 180 * Math.PI;
                            }
                            #endregion

                            #region 텍스트 이동 | 회전
                            var acText = T.GetObject(t.Id, OpenMode.ForWrite) as DBText;

                            if (U002.Instance.move)
                            {
                                var p = acPolyline.NumberOfVertices == 4 ? GetCenterPoint(acPolyline) : GetVisualCenterPoint(acPolyline);

                                MoveText(acText, p);
                                //acText.Position = p;
                            }

                            if (U002.Instance.rotate2 || U002.Instance.rotate3) acText.Rotation = ang;
                            #endregion
                        }
                    });

                    T.Commit();
                }
            }

            AC.Editor.WriteMessage("\nText 정렬이 완료되었습니다.");
            AC.Editor.PostCommandPrompt();
        }

        public void IsInsideAPolyline()
        {
            var acPolyLines = new List<Polyline>();

            #region 폴리라인 선택
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                try
                {
                    SelectionSet acSSet = SelectLines();

                    var Lines = from id in acSSet.GetObjectIds()
                                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                where acEnt is Polyline
                                let acLine = acEnt as Polyline
                                select acLine;

                    if (!Lines.Any())
                        return;

                    acPolyLines.AddRange(Lines.ToArray());
                }
                catch (System.Exception ex)
                {
                    AC.Editor.WriteMessage(Convert.ToString(ex));
                }
            }

            //if (acPolyLines.Count != 1)
            //    return;

            //Polyline acPolyline = acPolyLines[0];

            acPolyLines.ForEach(acPolyline =>
            {
                GetVisualCenterPoint(acPolyline);

            });

            #endregion

            #region 점 입력
            //var P = CAD.PickPoint();

            //if (P == new Point3d())
            //    return;
            #endregion

            //var B = IsInsideInPolyline(acPolyline, P);

            //MessageBox.Show(B.ToString());
        }

        /// <summary>
        /// 폴리라인 중심에 면적 입력하기(단일 선택)
        /// </summary>
        public void Create_PolylineAreaText()
        {
            bool B = true;

            while (B)
            {
                var acSSet = SelectAPolyline();

                if (acSSet == null)
                {
                    B = false;

                    return;
                }

                using (Transaction T = AC.DB.TransactionManager.StartTransaction())
                {
                    #region 폴리라인 선택
                    var Lines = from id in acSSet.GetObjectIds()
                                let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                                where acEnt is Polyline
                                let acLine = acEnt as Polyline
                                select acLine;

                    if (!Lines.Any())
                        return;

                    var acPolyline = Lines.ToList().First();
                    #endregion

                    #region 면적 입력
                    var p = acPolyline.NumberOfVertices == 4 ? GetCenterPoint(acPolyline) : GetVisualCenterPoint(acPolyline);

                    var PL_Area = acPolyline.Area;

                    var Area = Math.Round(PL_Area / 100000, 2);

                    var acText = CADUtil.CreateText(p, 100, Area.ToString() + "m²");

                    acText.Color = ColorIndex.Green;

                    AC.Editor.WriteMessage(" 폴리라인에 면적 [" + acText.TextString + "]이 입력되었습니다. \n");
                    #endregion

                    T.Commit();
                }
            }

            AC.Editor.PostCommandPrompt();
        }
        /// <summary>
        /// 폴리라인 중심에 면적 입력하기(복수 선택)
        /// </summary>
        public void Create_PolylineAreaTexts()
        {
            var acSSet = SelectPolylines();

            if (acSSet == null)
            {
                return;
            }

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                #region 폴리라인 선택
                var Lines = from id in acSSet.GetObjectIds()
                            let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                            where acEnt is Polyline
                            let acLine = acEnt as Polyline
                            where acLine.Closed
                            select acLine;

                if (!Lines.Any())
                    return;
                #endregion

                int n = 0;

                Lines.ToList().ForEach(acPolyline =>
                {
                    #region 면적 입력
                    var p = acPolyline.NumberOfVertices == 4 ? GetCenterPoint(acPolyline) : GetVisualCenterPoint(acPolyline);

                    var PL_Area = acPolyline.Area;

                    var Area = Math.Round(PL_Area / 100000, 2);

                    var acText = CADUtil.CreateText(p, 100, Area.ToString() + "m²");

                    acText.Color = ColorIndex.Green;

                    n++;
                    #endregion
                });

                T.Commit();

                AC.Editor.WriteMessage("\n" + n.ToString() + "개의 폴리라인에 면적이 입력되었습니다.");
                AC.Editor.PostCommandPrompt();
            }
        }


        #endregion

        #region PRIVATE 메서드
        private void Test()
        {
            List<int> I = new List<int> { 1, 2, 3, 4, 5 };

            foreach (int i in I)
                MessageBox.Show(i.ToString());

            I.ForEach(el => MessageBox.Show(el.ToString()));

            Array.ForEach(I.ToArray(), el => MessageBox.Show(el.ToString()));

            Expression<Func<int, int, int>> exp = (i, j) => i + j;
            var f = exp.Compile();

            var s = f(10, 20);

            BinaryExpression be = (BinaryExpression)exp.Body;

            exp.Compile();

        }
        // 선택
        private SelectionSet SelectLines()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "TEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "MTEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "폴리선 선택";
            opt.SinglePickInSpace = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\nError in getting selections");
                return acPSR.Value;
            }

            AC.Doc.GetAcadDocument();
            #endregion

            return acPSR.Value;
        }
        private SelectionSet SelectAPolyline()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "폴리선 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                MessageBox.Show("Polyline이 선택되지 않았습니다.");
                return null;
            }
            #endregion

            return acPSR.Value;
        }
        private SelectionSet SelectPolylines()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "폴리선 선택: ";
            opt.SinglePickInSpace = false;
            opt.SingleOnly = false;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                MessageBox.Show("Polyline이 선택되지 않았습니다.");
                return null;
            }
            #endregion

            return acPSR.Value;
        }

        private SelectionSet SelectLines2()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "TEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "MTEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "폴리선 선택";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }

            AC.Doc.GetAcadDocument();
            #endregion

            return acPSR.Value;
        }

        private SelectionSet SelectLinesAndTexts()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "TEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "MTEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "폴리선 & 텍스트 선택";
            opt.SinglePickInSpace = true;
            //opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }

            AC.Doc.GetAcadDocument();
            #endregion

            return acPSR.Value;
        }

        private SelectionSet SelectTexts()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "TEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "MTEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "텍스트 선택";

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }

            AC.Doc.GetAcadDocument();
            #endregion

            return acPSR.Value;
        }
        private SelectionSet SelectAText()
        {
            #region 필터
            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "TEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Start), "MTEXT"),
                new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
            };

            SelectionFilter oSf = new SelectionFilter(tvs);
            #endregion

            #region 선택
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "텍스트 선택: ";
            opt.SinglePickInSpace = true;
            opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt, oSf);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\n선택이 취소되었습니다.");
                AC.Editor.PostCommandPrompt();
                return null;
            }

            AC.Doc.GetAcadDocument();
            #endregion

            return acPSR.Value;
        }



        public Point3d returnMidpointofPolygon(ArrayList points)
        {
            Point3d[] pts = (Point3d[])points.ToArray(typeof(Point3d));
            Point3d temp;
            int i = 2;

            temp = getMidpoint(pts[0], pts[1]); //첨엔 1/2

            while (i < pts.Length)

                temp = getNdivedPoint(temp, pts[i], ++i); //담엔 1/n

            return temp;
        }

        public Point3d getMidpoint(Point3d x1, Point3d x2) //두 점사이의 중점을 구하는 메서드
        {
            Point3d result = new Point3d((x1.X + x2.X) / 2, (x1.Y + x2.Y) / 2, 0);
            return result;
        }

        public Point3d getNdivedPoint(Point3d x1, Point3d x2, int i) //두 점의 벡터를 구해서 그 벡터의 
        {                                                                       // 1/n 지점을 구하는 메서드 
            Point3d vector = new Point3d(x2.X - x1.X, x2.Y - x1.Y, 0);
            Point3d result = new Point3d(x1.X + (int)(vector.X / i), x1.Y + (int)(vector.Y / i), 0);

            return result;
        }


        private Point3d GetCenterPoint2(Polyline acPolyline)
        {
            var A = acPolyline.Area;
            var X = 0.0;
            var Y = 0.0;

            var Ps = new List<Point3d>();

            for (int i = 0; i < acPolyline.NumberOfVertices; i++)
            {
                try
                {
                    Ps.Add(acPolyline.GetPoint3dAt(i));
                }
                catch
                {
                }
            }

            #region 다각형 넓이
            //for (int i = 0; i < Ps.Count - 1; i++)
            //{
            //    var X1 = Ps[i].X;
            //    var X2 = Ps[i + 1].X;
            //    var Y1 = Ps[i].Y;
            //    var Y2 = Ps[i + 1].Y;

            //    A += (X1 * Y2 - X2 * Y1) / 2;
            //}
            #endregion

            //MessageBox.Show(Ps.Count.ToString());
            //MessageBox.Show(A.ToString());

            #region 다각형 무게중심
            for (int i = 0; i < Ps.Count - 1; i++)
            {
                var X1 = Ps[i].X;
                var X2 = Ps[i + 1].X;
                var Y1 = Ps[i].Y;
                var Y2 = Ps[i + 1].Y;

                X += (X1 + X2) * (X1 * Y2 - X2 * Y1);
                Y += (Y1 + Y2) * (X1 * Y2 - X2 * Y1);
            }

            X = X / (6 * A);
            Y = Y / (6 * A);
            #endregion

            return new Point3d(X, Y, 0);
        }


        // 사각형인 경우 중심점 구하기
        private Point3d GetCenterPoint(Polyline acPolyline)
        {
            var X = 0.0;
            var Y = 0.0;

            var Ps = new List<Point3d>();

            for (int i = 0; i < acPolyline.NumberOfVertices; i++)
            {
                try
                {
                    X += acPolyline.GetPoint3dAt(i).X / acPolyline.NumberOfVertices;
                    Y += acPolyline.GetPoint3dAt(i).Y / acPolyline.NumberOfVertices;
                }
                catch
                {
                }
            }

            return new Point3d(X, Y, 0);
        }

        /// <summary>
        /// 폴리라인의 중심점 구하기
        /// </summary>
        /// <param name="acPolyline"></param>
        /// <returns></returns>
        private Point3d GetVisualCenterPoint(Polyline acPolyline)
        {
            #region 폴리라인 내부 점 받기
            var min_P = acPolyline.Bounds.Value.MinPoint;
            var max_P = acPolyline.Bounds.Value.MaxPoint;

            int n = 20;

            double dis_X = (max_P.X - min_P.X) / n;
            double dis_Y = (max_P.Y - min_P.Y) / n;

            var Points = new List<Point3d>();

            for (int i = 0; i < n + 1; i++)
            {
                for (int j = 0; j < n + 1; j++)
                {
                    var p = CADUtil.MoveP(min_P, dis_X * j, dis_Y * i);

                    if (IsInsideInPolyline(acPolyline, p))
                    {
                        Points.Add(p);
                    }
                }
            }
            #endregion

            #region 무게중심
            double value = double.MaxValue;
            Point3d cp = new Point3d();

            Points.ForEach(p1 =>
            {
                double val = 0;
                Points.ForEach(p2 =>
                {
                    if (!p1.IsEqualTo(p2))
                    {
                        val += p1.DistanceTo(p2) * p1.DistanceTo(p2);
                    }
                });

                if (value > val)
                {
                    value = val;
                    cp = p1;
                }
            });

            return cp;
            #endregion

            #region 폴리라인의 중심점
            var X = 0.0;
            var Y = 0.0;

            Points.ForEach(p =>
            {
                X += (p.X) / Points.Count;
                Y += (p.Y) / Points.Count;
            });

            //return new Point3d(X, Y, 0);
            #endregion

            #region 점, 문자
            //using (Transaction T = acDb.TransactionManager.StartTransaction())
            //{
            //    BlockTable BT = T.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;
            //    BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

            //    DBPoint p1 = new DBPoint(min_P);
            //    DBPoint p2 = new DBPoint(max_P);
            //    DBPoint p3 = new DBPoint(new Point3d(X, Y, 0));
            //    DBPoint p4 = new DBPoint(cp);

            //    //BTR.AppendEntity(p1);
            //    //BTR.AppendEntity(p2);
            //    //BTR.AppendEntity(p3);
            //    BTR.AppendEntity(p4);

            //    //T.AddNewlyCreatedDBObject(p1, true);
            //    //T.AddNewlyCreatedDBObject(p2, true);
            //    //T.AddNewlyCreatedDBObject(p3, true);
            //    T.AddNewlyCreatedDBObject(p4, true);

            //    for (int i = 0; i < Points.Count; i++)
            //    {
            //        //using (DBPoint acPoint = new DBPoint(Points[i]))
            //        //{
            //        //    BTR.AppendEntity(acPoint);
            //        //    T.AddNewlyCreatedDBObject(acPoint, true);
            //        //}
            //    }
            //    acDb.Pdmode = 34;
            //    acDb.Pdsize = 150;

            //    T.Commit();
            //}
            //return new Point3d();
            #endregion
        }

        private bool IsInsideInPolyline(Polyline acPolyline, Point3d p)
        {
            #region 경계선에 걸리는 경우
            //for (int i = 0; i < acPolyline.NumberOfVertices; i++)
            //{
            //    try
            //    {
            //        var l = acPolyline.GetLineSegmentAt(i);

            //        if (l.GetDistanceTo(p) < 0.00001) return true;
            //    }
            //    catch
            //    {
            //    }
            //}
            #endregion

            #region 점이 폴리라인 내부에 있는지 판별
            Point3d Min_P = acPolyline.GetPoint3dAt(0);
            Point3d Max_P = acPolyline.GetPoint3dAt(acPolyline.NumberOfVertices - 1);

            double total_Ang = GetAngle(Max_P, p, Min_P);

            for (int i = 0; i < acPolyline.NumberOfVertices - 1; i++)
            {
                var line = acPolyline.GetLineSegmentAt(i);
                var SP = line.StartPoint;
                var EP = line.EndPoint;

                total_Ang += GetAngle(SP, p, EP);
            }

            return Math.Abs(total_Ang) > 0.0000000001;
            #endregion
        }

        private double GetAngle(Point3d A, Point3d B, Point3d C)
        {
            double dot_product = GetDotProduct(A, B, C);
            double cross_product = GetCrossProductLength(A, B, C);

            return Math.Atan2(cross_product, dot_product);
        }

        private double GetDotProduct(Point3d A, Point3d B, Point3d C)
        {
            var Ax = A.X;
            var Ay = A.Y;
            var Bx = B.X;
            var By = B.Y;
            var Cx = C.X;
            var Cy = C.Y;

            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            return (BAx * BCx + BAy * BCy);
        }

        private double GetCrossProductLength(Point3d A, Point3d B, Point3d C)
        {
            var Ax = A.X;
            var Ay = A.Y;
            var Bx = B.X;
            var By = B.Y;
            var Cx = C.X;
            var Cy = C.Y;

            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            return (BAx * BCy - BAy * BCx);
        }

        private void MoveText(DBText T, Point3d P)
        {
            T.Position = P;

            if (!T.IsDefaultAlignment)
            {
                T.AlignmentPoint = P;
            }

            //switch (T.Justify)
            //{
            //    case AttachmentPoint.TopLeft:
            //    case AttachmentPoint.TopCenter:
            //    case AttachmentPoint.TopRight:
            //    case AttachmentPoint.TopMid:

            //    case AttachmentPoint.MiddleLeft:
            //    case AttachmentPoint.MiddleCenter:
            //    case AttachmentPoint.MiddleRight:
            //    case AttachmentPoint.MiddleMid:

            //    case AttachmentPoint.BottomLeft:
            //    case AttachmentPoint.BottomCenter:
            //    case AttachmentPoint.BottomRight:
            //    case AttachmentPoint.BottomMid:

            //    case AttachmentPoint.BaseCenter:
            //    case AttachmentPoint.BaseRight:
            //    case AttachmentPoint.BaseMid:
            //        T.AlignmentPoint = P;
            //        break;

            //    //case AttachmentPoint.BottomAlign:
            //    //case AttachmentPoint.MiddleAlign:
            //    //case AttachmentPoint.TopAlign:
            //    //case AttachmentPoint.BaseAlign:

            //    //case AttachmentPoint.BottomFit:
            //    //case AttachmentPoint.MiddleFit:
            //    //case AttachmentPoint.TopFit:
            //    //case AttachmentPoint.BaseFit:

            //    case AttachmentPoint.BaseLeft:

            //        T.Position = P;
            //        break;
            //}
        }

        #endregion
    }
}
