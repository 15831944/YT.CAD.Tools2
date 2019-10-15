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

namespace CADAUTO
{
    public class CAD
    {
        #region 필드
        static Document Doc = Application.DocumentManager.MdiActiveDocument;
        static Database DB = Doc.Database;
        static DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
        static Editor ED = Doc.Editor;
        #endregion

        #region POINT
        public static Point3d PickPoint()
        {
            PromptPointResult PPR = ED.GetPoint("Pick Point");

            return (PPR.Status == PromptStatus.OK) ? PPR.Value : new Point3d();
        }

        public static Point2d ToPoint2D(Point3d P)
        {
            return new Point2d(P.X, P.Y);
        }
        public static Point3d ToPoint3D(Point2d P)
        {
            return new Point3d(P.X, P.Y, 0);
        }

        public static Point2d GetCenterPoint2d(Point2d P1, Point2d P2)
        {
            return new Point2d((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
        }
        public static Point3d GetCenterPoint3d(Point3d P1, Point3d P2)
        {
            return new Point3d((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2, 0);
        }
        public static Point2d GetCenterPoint2d(Curve2d C)
        {
            return GetCenterPoint2d(C.StartPoint, C.EndPoint);
        }
        public static Point3d GetCenterPoint3d(Curve3d C)
        {
            return GetCenterPoint3d(C.StartPoint, C.EndPoint);
        }
        #endregion

        #region LINE
        public static Line CreateLine(Point3d SP, Point3d EP)
        {
            var Return = new Line();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return = new Line(SP, EP);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Line CreateLine(Point3d SP, Point3d EP, Color C)
        {
            var Return = new Line();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return = new Line(SP, EP);

                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Line CreateLine(Point2d SP, Point2d EP)
        {
            var Return = new Line();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return = new Line(ToPoint3D( SP),ToPoint3D( EP));

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }

        public static Line CreateLine(Point3d SP, double W, double H)
        {
            var Return = new Line();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point3d EP = new Point3d(SP.X + W, SP.Y + H, SP.Z);

                Return = new Line(SP, EP);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Line CreateLine(Point3d SP, double W, double H, Color C)
        {
            var Return = new Line();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Point3d EP = new Point3d(SP.X + W, SP.Y + H, SP.Z);

                Return = new Line(SP, EP);

                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        #endregion

        #region CURVE
        public static Curve CreateCurve(Point3d P1, Point3d P2)
        {
            return CreateLine(P1, P2) as Curve;
        }

        public static Curve ConnectCurve(Curve2d C1, Curve2d C2)
        {
            var sp1 = C1.StartPoint;
            var ep1 = C1.EndPoint;
            var sp2 = C2.StartPoint;
            var ep2 = C2.EndPoint;

            var SP = new Point2d();
            var EP = new Point2d();

            var distance = C1.GetDistanceTo(C2);

            if (sp1.GetDistanceTo(sp2) > ep1.GetDistanceTo(sp2))
            {
                SP = sp1;
            }
            else
            {
                SP = ep1;
            }

            if (sp2.GetDistanceTo(SP) > ep2.GetDistanceTo(SP))
            {
                EP = sp2;
            }
            else
            {
                EP = sp1;
            }

            var curve = CreateLine(ToPoint3D(SP), ToPoint3D(EP)) as Curve;

            return curve;
        }
        #endregion

        #region RECTANGLE
        public static Polyline CreateRectangle(Point3d SP, double W, double H)
        {
            var Return = new Polyline();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var P = new Point2d(SP.X, SP.Y);

                Return.SetDatabaseDefaults();

                Return.AddVertexAt(0, P, 0, 0, 0);
                Return.AddVertexAt(1, MoveP(P, W, 0), 0, 0, 0);
                Return.AddVertexAt(2, MoveP(P, W, H), 0, 0, 0);
                Return.AddVertexAt(3, MoveP(P, 0, H), 0, 0, 0);
                Return.AddVertexAt(4, MoveP(P, 0, 0), 0, 0, 0);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Polyline CreateRectangle(Point3d SP, double W, double H, Color C)
        {
            var Return = new Polyline();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var P = new Point2d(SP.X, SP.Y);

                Return.SetDatabaseDefaults();

                Return.AddVertexAt(0, P, 0, 0, 0);
                Return.AddVertexAt(1, MoveP(P, W, 0), 0, 0, 0);
                Return.AddVertexAt(2, MoveP(P, W, H), 0, 0, 0);
                Return.AddVertexAt(3, MoveP(P, 0, H), 0, 0, 0);
                Return.AddVertexAt(4, MoveP(P, 0, 0), 0, 0, 0);

                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Polyline CreateCenterRectangle(Point3d SP, double W, double H)
        {
            var Return = new Polyline();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var P = MoveP(SP, -W / 2, -H / 2);

                CreateCenterRectangle(P, W, H);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Polyline CreateCenterRectangle(Point3d SP, double W, double H, Color C)
        {
            var Return = new Polyline();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var P = MoveP(SP, -W / 2, -H / 2);

                CreateCenterRectangle(P, W, H, C);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Polyline CreateRectangle(Point3d SP, Point3d EP, Vector2d Vec1, double Ang)
        {
            var Return = new Polyline();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var dis = SP.DistanceTo(EP);

                double W = dis * Math.Cos(Ang);
                double H = dis * Math.Sin(Ang);

                var P1 = new Point2d(SP.X, SP.Y);
                var P2 = P1 + Vec1.GetNormal() * W;
                var P3 = new Point2d(EP.X, EP.Y);
                var P4 = P3 - Vec1.GetNormal() * W;

                Return.SetDatabaseDefaults();

                Return.AddVertexAt(0, P1, 0, 0, 0);
                Return.AddVertexAt(1, P2, 0, 0, 0);
                Return.AddVertexAt(2, P3, 0, 0, 0);
                Return.AddVertexAt(3, P4, 0, 0, 0);

                Return.Closed = true;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }

        #endregion

        #region CIRCLE
        public static Circle CreateCircle(Point3d CP, double r)
        {
            var Return = new Circle();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Center = CP;
                Return.Radius = r;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Circle CreateCircle(Point3d CP, double r, Color C)
        {
            var Return = new Circle();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Center = CP;
                Return.Radius = r;
                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        #endregion

        #region ARC
        public static Arc CreateArc(Point3d CP, double r, double SA, double EA)
        {
            var Return = new Arc();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.Center = CP;
                Return.Radius = r;
                Return.StartAngle = Util.ToDegree(SA);
                Return.EndAngle = Util.ToDegree(EA);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Arc CreateArc(Point3d CP, double r, double SA, double EA, Color C)
        {
            var Return = new Arc();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.Center = CP;
                Return.Radius = r;
                Return.StartAngle = Util.ToDegree(SA);
                Return.EndAngle = Util.ToDegree(EA);
                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        #endregion

        #region TEXT
        public static DBText CreateText(Point3d P, double H, string S)
        {
            var Return = new DBText();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AttachmentPoint.MiddleMid;
                Return.AlignmentPoint = P;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, string S, Color C)
        {
            var Return = new DBText();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AttachmentPoint.MiddleMid;
                Return.AlignmentPoint = P;
                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, string S, AttachmentPoint AP)
        {
            var Return = new DBText();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AP;
                Return.AlignmentPoint = P;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, string S, AttachmentPoint AP, Color C)
        {
            var Return = new DBText();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AP;
                Return.AlignmentPoint = P;
                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        #endregion

        #region HATCH
        public static Hatch CreateHatch(ObjectId ID, string Type)
        {
            var Return = new Hatch();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                var OC = new ObjectIdCollection();
                OC.Add(ID);

                Return.SetDatabaseDefaults();
                Return.SetHatchPattern(HatchPatternType.PreDefined, Type);
                Return.Associative = true;
                Return.AppendLoop(HatchLoopTypes.Default, OC);
                Return.EvaluateHatch(true);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Hatch CreateHatch(ObjectId ID, string Type, Color C)
        {
            var Return = new Hatch();

            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                var OC = new ObjectIdCollection();
                OC.Add(ID);

                Return.SetDatabaseDefaults();
                Return.SetHatchPattern(HatchPatternType.PreDefined, Type);
                Return.Associative = true;
                Return.AppendLoop(HatchLoopTypes.Default, OC);
                Return.EvaluateHatch(true);
                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        #endregion



        #region MOVE
        public static Point3d MoveP(Point3d P, double W, double H)
        {
            return new Point3d(P.X + W, P.Y + H, P.Z);
        }
        public static Point3d MoveP(Point3d P, double X, double Y, double Z)
        {
            return new Point3d(P.X + X, P.Y + Y, P.Z + Z);
        }
        public static Point2d MoveP(Point2d P, double W, double H)
        {
            return new Point2d(P.X + W, P.Y + H);
        }
        #endregion

        #region ERASE
        public static void Erase(ObjectId ID)
        {
            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord BTR = T.GetObject(DB.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                Object Obj = T.GetObject(ID, OpenMode.ForWrite) as Object;

                switch (ID.ObjectClass.DxfName.ToUpper())
                {
                    case "POLYLINE": var PL = Obj as Polyline; PL.Erase(true); break;
                    case "CIRCLE": var C = Obj as Circle; C.Erase(true); break;
                    case "LINE": var L = Obj as Line; L.Erase(true); break;
                    case "ARC": var A = Obj as Arc; A.Erase(true); break;
                    case "HATCH": var H = Obj as Hatch; H.Erase(true); break;
                }

                T.Commit();
            }
        }
        #endregion

        #region GROUP
        public void CreateGroup(ObjectIdCollection OIC, string GName)
        {
            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                var DBD = T.GetObject(DB.GroupDictionaryId, OpenMode.ForWrite) as DBDictionary;

                using (Group G = new Group(GName, true))
                {
                    DBD.SetAt(GName, G);

                    T.AddNewlyCreatedDBObject(G, true);

                    OIC.Cast<ObjectId>().ToList().ForEach(id => G.Append(id));

                    T.Commit();
                }
            }
        }
        #endregion

        #region OFFSET
        public static Polyline Offset(Polyline PL, double D)
        {
            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                PL.GetOffsetCurves(D);

                BTR.AppendEntity(PL);
                T.AddNewlyCreatedDBObject(PL, true);
                T.Commit();
            }

            return PL;
        }
        public static Circle Offset(Circle C, double D)
        {
            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                C.GetOffsetCurves(D);

                BTR.AppendEntity(C);
                T.AddNewlyCreatedDBObject(C, true);
                T.Commit();
            }

            return C;
        }
        public static Line Offset(Line L, double D)
        {
            using (Transaction T = Doc.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                L.GetOffsetCurves(D);

                BTR.AppendEntity(L);
                T.AddNewlyCreatedDBObject(L, true);
                T.Commit();
            }

            return L;
        }
        #endregion

        #region GET VALUE
        public static Vector2d GetVector(Curve2d curve)
        {
            return GetVector(curve.StartPoint, curve.EndPoint);
        }
        public static Vector2d GetVector(Point2d P1, Point2d P2)
        {
            return new Vector2d(P2.X - P1.X, P2.Y - P1.Y);
        }
        #endregion

        #region VIEW
        public static void ZoomExtents(Point3d P1, Point3d P2, double dFactor)
        {
            #region 포인트 지정 [P1, P2, P3]
            //var P1 = new Point3d();
            //var P2 = new Point3d();
            var P3 = GetCenterPoint3d(P1, P2);

            int nCurVport = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"));

            /// 포인트가 없을 경우 따로 포인트 범위를 지정해줘야한다.
            if (DB.TileMode == true)
            {
                if (P1.IsEqualTo(new Point3d()) && P2.IsEqualTo(new Point3d()))
                {
                    P1 = DB.Extmin; // Extents Space
                    P2 = DB.Extmax;
                }
            }
            else
            {
                if (nCurVport == 1)
                {
                    if (P1.IsEqualTo(new Point3d()) && P2.IsEqualTo(new Point3d()))
                    {
                        P1 = DB.Pextmin;    // Paper Space
                        P2 = DB.Pextmax;
                    }
                }
                else
                {
                    if (P1.IsEqualTo(new Point3d()) && P2.IsEqualTo(new Point3d()))
                    {
                        P1 = DB.Extmin;
                        P2 = DB.Extmax;
                    }
                }
            }
            #endregion

            using (Transaction T = DB.TransactionManager.StartTransaction())
            {
                using (ViewTableRecord VTR = Doc.Editor.GetCurrentView())
                {
                    Extents3d extents;

                    // Translate WCS coordinates to DCS
                    Matrix3d matrix;
                    matrix = Matrix3d.PlaneToWorld(VTR.ViewDirection);
                    matrix = Matrix3d.Displacement(VTR.Target - Point3d.Origin) * matrix;
                    matrix = Matrix3d.Rotation(-VTR.ViewTwist, VTR.ViewDirection, VTR.Target) * matrix;

                    if (P3.IsEqualTo(Point3d.Origin))
                    {
                        P1 = new Point3d(P3.X - (VTR.Width / 2), P3.Y - (VTR.Height / 2), 0);
                        P2 = new Point3d((VTR.Width) / 2 * P3.X, (VTR.Height / 2) + P3.Y, 0);
                    }

                    using (var L = new Line(P1, P2))
                    {
                        extents = new Extents3d(L.Bounds.Value.MinPoint, L.Bounds.Value.MaxPoint);
                    }

                    double V_Ratio = VTR.Width / VTR.Height;

                    matrix = matrix.Inverse();
                    extents.TransformBy(matrix);

                    double W;
                    double H;
                    Point2d NewCenP;

                    if (!P3.IsEqualTo(Point3d.Origin))
                    {
                        W = VTR.Width;
                        H = VTR.Height;

                        if (dFactor == 0)
                        {
                            P3 = P3.TransformBy(matrix);
                        }

                        NewCenP = new Point2d(P3.X, P3.Y);
                    }
                    else
                    {
                        W = extents.MaxPoint.X - extents.MinPoint.X;
                        H = extents.MaxPoint.Y - extents.MinPoint.Y;

                        NewCenP = new Point2d((extents.MaxPoint.X + extents.MinPoint.X) * 0.5, (extents.MaxPoint.Y + extents.MinPoint.Y) * 0.5);
                    }

                    if (W > (H * V_Ratio)) H = H / V_Ratio;
                    if (dFactor != 0)
                    {
                        VTR.Height = H * dFactor;
                        VTR.Width = W * dFactor;
                    }

                    VTR.CenterPoint = NewCenP;

                    ED.SetCurrentView(VTR);
                }

                T.Commit();
            }
        }
        #endregion
    }


    public class ColorIndex
    {
        public static Color White { get { return Color.FromColorIndex(ColorMethod.ByAci, 7); } }
        public static Color Red { get { return Color.FromColorIndex(ColorMethod.ByAci, 1); } }
        public static Color Yellow { get { return Color.FromColorIndex(ColorMethod.ByAci, 2); } }
        public static Color Green { get { return Color.FromColorIndex(ColorMethod.ByAci, 3); } }
        public static Color SkyBlue { get { return Color.FromColorIndex(ColorMethod.ByAci, 4); } }
        public static Color Blue { get { return Color.FromColorIndex(ColorMethod.ByAci, 5); } }
        public static Color Magenta { get { return Color.FromColorIndex(ColorMethod.ByAci, 6); } }
        public static Color DarkGray { get { return Color.FromColorIndex(ColorMethod.ByAci, 253); } }
        public static Color Gray { get { return Color.FromColorIndex(ColorMethod.ByAci, 254); } }

        /* ColorIndex
         * 0 = 블록별(흰색)
         * 1 = 빨간색
         * 2 = 노란색
         * 3 = 초록색
         * 4 = 하늘색
         * 5 = 파란색
         * 6 = 선홍색
         */
    }

    public class Util
    {
        #region 변환
        public static string ToString(object Cell)
        {
            return (Cell is DBNull || Convert.ToString(Cell) == "") ? "" : Convert.ToString(Cell);
        }
        public static int ToInt(object Cell)
        {
            return (Cell is DBNull || Cell.ToString() == "") ? 0 : Convert.ToInt32(Cell);
        }
        public static double ToDouble(object Cell)
        {
            return (Cell is DBNull || Cell.ToString() == "") ? 0 : Convert.ToDouble(Cell);
        }

        public static double ToRadian(double Degree)
        {
            return Degree * Math.PI / 180;
        }
        public static double ToDegree(double Radian)
        {
            return Radian * 180 / Math.PI;
        }

        public static double ToDouble(DevExpress.XtraEditors.TextEdit T)
        {
            return T.Text == "" || T.Text == string.Empty ? 0 : double.Parse(T.Text);
        }
        #endregion

    }
}
