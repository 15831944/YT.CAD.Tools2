using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;

namespace YT_CAD_TOOL
{
    public class cad
    {
        #region 필드
        //Document Doc = Application.DocumentManager.MdiActiveDocument;
        //Database DB = Doc.Database;
        //DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
        //Editor ED = Doc.Editor;

        //static Document Doc = Application.DocumentManager.MdiActiveDocument;
        //static Database DB = Doc.Database;
        //static DocumentLock DL = Doc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
        //static Editor ED = Doc.Editor;
        #endregion

        #region SELECT
        public static SelectionSet SelectObjs()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();

            //SelectionFilter oSf = new SelectionFilter(tvs);

            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = "객체 선택: ";
            //opt.SinglePickInSpace = true;
            //opt.SingleOnly = true;

            PromptSelectionResult acPSR = AC.Editor.GetSelection(opt);

            if (acPSR.Status != PromptStatus.OK)
            {
                AC.Editor.WriteMessage("\nError in getting selections \n");

                return acPSR.Value;
            }

            AC.Doc.GetAcadDocument();

            return acPSR.Value;
        }

        public static void SetSelected(PromptSelectionResult acPSR)
        {
            #region T
            using (Transaction T = AC.Doc.Database.TransactionManager.StartTransaction())
            {
                BlockTable BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                if (acPSR.Status == PromptStatus.OK)
                {
                    AC.Editor.SetImpliedSelection(acPSR.Value.GetObjectIds());
                }

                T.Commit();
            }
            #endregion
        }
        #endregion

        #region POINT
        public static Point3d PickPoint()
        {
            PromptPointResult PPR = AC.Editor.GetPoint("Pick Point");

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
        public static Point3d GetCenterPoint3d(Line L)
        {
            return GetCenterPoint3d(L.StartPoint, L.EndPoint);
        }
        #endregion

        #region LINE
        public static Line CreateLine(Point3d SP, Point3d EP)
        {
            var Return = new Line();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return = new Line(SP, EP);
                {
                    Return.Color = C;
                }

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Line CreateLine(Point2d SP, Point2d EP)
        {
            var Return = new Line();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return = new Line(ToPoint3D(SP), ToPoint3D(EP));

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }

        public static Line CreateLine(Point3d SP, double W, double H)
        {
            var Return = new Line();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

        #region POLYLINE
        public static Polyline CreatePolyline(Point2d SP, Point2d EP)
        {
            var Return = new Polyline();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var P = new Point2d(SP.X, SP.Y);

                Return.SetDatabaseDefaults();

                Return.AddVertexAt(0, SP, 0, 0, 0);
                Return.AddVertexAt(1, EP, 0, 0, 0);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Polyline CreatePolyline(Point3d SP, Point3d EP)
        {
            var Return = new Polyline();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var P = new Point2d(SP.X, SP.Y);

                Return.SetDatabaseDefaults();

                Return.AddVertexAt(0, ToPoint2D(SP), 0, 0, 0);
                Return.AddVertexAt(1, ToPoint2D(EP), 0, 0, 0);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Polyline CreatePolyline(Point3dCollection A)
        {
            var Return = new Polyline();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                for (int i = 0; i < A.Count; i++)
                {
                    Return.AddVertexAt(i, ToPoint2D(A[i]), 0, 0, 0);
                }

                Return.Closed = true;

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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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
        public static Polyline CreateRectangle(Point3d SP, Point3d EP)
        {
            var Return = new Polyline();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                double W = EP.X - SP.X;
                double H = EP.Y - SP.Y;

                var P = new Point2d(SP.X, SP.Y);

                Return.SetDatabaseDefaults();

                Return.AddVertexAt(0, P, 0, 0, 0);
                Return.AddVertexAt(1, MoveP(P, W, 0), 0, 0, 0);
                Return.AddVertexAt(2, MoveP(P, W, H), 0, 0, 0);
                Return.AddVertexAt(3, MoveP(P, 0, H), 0, 0, 0);
                Return.AddVertexAt(4, MoveP(P, 0, 0), 0, 0, 0);

                //Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }

        public static Polyline CreateCenterRectangle(Point3d SP, double W, double H)
        {
            var Return = new Polyline();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

        public static Polyline CreateRectangle(List<Point3d> Points)
        {
            var Return = new Polyline();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                int i = 0;

                Points.ForEach(p =>
                {
                    Return.AddVertexAt(i++, ToPoint2D(p), 0, 0, 0);
                });

                if (Points[0].IsEqualTo(Points[Points.Count - 1]))
                {
                    Return.RemoveVertexAt(0);
                    Return.Closed = true;
                }

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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.Center = CP;
                Return.Radius = r;
                Return.StartAngle = to.Degree(SA);
                Return.EndAngle = to.Degree(EA);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static Arc CreateArc(Point3d CP, double r, double SA, double EA, Color C)
        {
            var Return = new Arc();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.Center = CP;
                Return.Radius = r;
                Return.StartAngle = to.Degree(SA);
                Return.EndAngle = to.Degree(EA);
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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
        public static DBText CreateText(Point3d P, double H, string S, Color C, ObjectId L)
        {
            var Return = new DBText();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AttachmentPoint.MiddleMid;
                Return.AlignmentPoint = P;
                Return.Color = C;
                Return.LayerId = L;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, string S, ObjectId L)
        {
            var Return = new DBText();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AttachmentPoint.MiddleMid;
                Return.AlignmentPoint = P;
                Return.LayerId = L;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, string S, ObjectId L, AttachmentPoint AP)
        {
            var Return = new DBText();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AP;
                Return.AlignmentPoint = P;
                Return.LayerId = L;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, string S, ObjectId StyleID, Color C)
        {
            var Return = new DBText();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AttachmentPoint.MiddleMid;
                Return.AlignmentPoint = P;
                Return.TextStyleId = StyleID;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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
        public static DBText CreateText(Point3d P, double H, double A, string S)
        {
            var Return = new DBText();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = AttachmentPoint.MiddleMid;
                Return.AlignmentPoint = P;
                Return.Position = P;
                Return.Rotation = A;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }
        public static DBText CreateText(Point3d P, double H, double A, TextPosition J, string S)
        {
            var Return = new DBText();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                Return.Height = H;
                Return.TextString = S;
                Return.Justify = (AttachmentPoint)J;
                Return.Position = P;

                if (!Return.IsDefaultAlignment || J == TextPosition.중간
                                               || J == TextPosition.맨아래중심
                                               || J == TextPosition.중간중심
                                               || J == TextPosition.맨위중심)
                {
                    Return.AlignmentPoint = P;
                }

                Return.Rotation = A;

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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

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
        public static Hatch CreateHatch(ObjectId ID, string Type, string Layer, double Scale)
        {
            var Return = new Hatch();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();
                Return.AppendLoop(HatchLoopTypes.Default, new ObjectIdCollection { ID });
                Return.SetHatchPattern(HatchPatternType.PreDefined, Type);
                //Return.Associative = true;
                Return.EvaluateHatch(true);
                Return.Layer = Layer;
                Return.PatternScale = Scale;
                Return.PatternAngle = to.Radian(315);

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);

                T.Commit();
            }

            return Return;
        }
        public static Hatch CreateHatch(ObjectId ID, string Type, string Layer, Color C, double Scale)
        {
            var Return = new Hatch();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                var OC = new ObjectIdCollection();
                OC.Add(ID);

                Return.SetDatabaseDefaults();
                Return.AppendLoop(HatchLoopTypes.Default, OC);
                Return.SetHatchPattern(HatchPatternType.PreDefined, Type);
                //Return.Associative = true;
                Return.EvaluateHatch(true);
                Return.Layer = Layer;
                Return.PatternScale = Scale;
                Return.PatternAngle = to.Radian(315);
                Return.Color = C;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);
                T.Commit();
            }

            return Return;
        }

        public static Hatch CreateHatch(ObjectId ID, string Type, string Layer)
        {
            var Return = new Hatch();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Return.SetDatabaseDefaults();

                var OC = new ObjectIdCollection();
                OC.Add(ID);

                Return.SetDatabaseDefaults();
                Return.AppendLoop(HatchLoopTypes.Default, OC);
                Return.SetHatchPattern(HatchPatternType.PreDefined, Type);
                //Return.Associative = true;
                Return.EvaluateHatch(true);
                Return.Layer = Layer;
                Return.PatternScale = 50;
                Return.PatternAngle = 0;

                Random rnd = new Random();

                Color randomColor = ColorIndex.RGB(byte.Parse(rnd.Next(256).ToString())
                                                 , byte.Parse(rnd.Next(256).ToString())
                                                 , byte.Parse(rnd.Next(256).ToString()));

                Return.Color = randomColor;

                BTR.AppendEntity(Return);
                T.AddNewlyCreatedDBObject(Return, true);

                T.Commit();
            }

            return Return;
        }
        public static Hatch CreateHatch(ObjectId OutlineID, List<ObjectId> HoleIDs, string Type, string Layer, double Scale)
        {
            var Return = new Hatch();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var DOT = t.GetObject(BTR.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;

                var externalColl = new ObjectIdCollection();
                externalColl.Add(OutlineID);

                BTR.AppendEntity(Return);
                t.AddNewlyCreatedDBObject(Return, true);

                Return.SetDatabaseDefaults();

                Return.Elevation = 0.0;
                Return.PatternScale = Scale;
                Return.SetHatchPattern(HatchPatternType.PreDefined, Type);
                Return.PatternAngle = to.Radian(315);
                Return.Layer = Layer;

                Return.Associative = true;

                Return.AppendLoop(HatchLoopTypes.External, externalColl);

                HoleIDs.ForEach(ID =>
                {
                    var innerColl = new ObjectIdCollection();
                    innerColl.Add(ID);
                    Return.AppendLoop(HatchLoopTypes.Default, innerColl);
                });

                Return.EvaluateHatch(true);

                DOT.MoveToBottom(new ObjectIdCollection { Return.Id });

                t.Commit();
            }

            return Return;
        }
        #endregion

        #region LAYER
        public static List<string> GetAllLayerNames()
        {
            List<string> Return = new List<string>();

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                var temp = (from id in acLT.Cast<ObjectId>().ToList()
                            let acLTR = T.GetObject(id, OpenMode.ForRead) as LayerTableRecord
                            orderby acLTR.Name
                            select acLTR.Name);

                if (temp.Any()) Return.AddRange(temp);
            }

            return Return;
        }

        public static void TurnOffLayer(string layerName)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(layerName) == true)
                {
                    LayerTableRecord acLTR = T.GetObject(acLT[layerName], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = true;

                        AC.Editor.WriteMessage(layerName + " was truned off");

                        T.Commit();
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(layerName + " could not be turned off");
                    }
                    AC.Editor.PostCommandPrompt();
                }
            }
            #endregion
        }
        public static void TurnOffAllLayers()
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId id in acLT)
                {
                    LayerTableRecord acLTR = T.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = true;
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(acLTR.Name + " could not be turned off");
                    }
                }

                AC.Editor.WriteMessage("all layers were truned off.");

                T.Commit();
            }
            AC.Editor.PostCommandPrompt();
            #endregion
        }
        public static void TurnOnLayer(string layerName)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(layerName) == true)
                {
                    LayerTableRecord acLTR = T.GetObject(acLT[layerName], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = false;

                        AC.Editor.WriteMessage(layerName + " was truned on");

                        T.Commit();
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(layerName + " could not be turned on");
                    }
                    AC.Editor.PostCommandPrompt();
                }
            }
            #endregion
        }
        public static void TurnOnAllLayers()
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                foreach (ObjectId id in acLT)
                {
                    LayerTableRecord acLTR = T.GetObject(id, OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsOff = false;
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(acLTR.Name + " could not be turned on");
                    }
                }

                AC.Editor.WriteMessage("all layers were truned on.");

                T.Commit();
            }
            AC.Editor.PostCommandPrompt();
            #endregion
        }
        public static void LockLayer(string layerName)
        {
            #region T
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                LayerTable acLT = T.GetObject(AC.DB.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLT.Has(layerName) == true)
                {
                    LayerTableRecord acLTR = T.GetObject(acLT[layerName], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        acLTR.IsLocked = true;
                        AC.Editor.WriteMessage(layerName + " was locked");

                        T.Commit();
                    }
                    catch
                    {
                        AC.Editor.WriteMessage(layerName + " could not be locked");
                    }
                    AC.Editor.PostCommandPrompt();
                }
            }
            #endregion
        }

        public static List<string> SelectObjectLayers()
        {
            var Return = new List<string>();

            #region 선택
            var acSSet = SelectObjs();

            if (acSSet == null) return Return;
            #endregion

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var acEnts = from id in acSSet.GetObjectIds()
                             let acEnt = T.GetObject(id, OpenMode.ForWrite) as Entity
                             select acEnt;

                if (!acEnts.Any()) return Return;

                Return = (from a in acEnts
                          orderby a.Layer
                          select a.Layer).Distinct().ToList();



            }

            AC.Editor.PostCommandPrompt();

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
        public static Point2d MoveP(Point2d P, Vector3d V, double d = 1)
        {
            return new Point2d(P.X + V.X * d, P.Y + V.Y * d);
        }
        public static Point3d MoveP(Point3d P, Vector3d V, double d = 1)
        {
            return new Point3d(P.X + V.X * d, P.Y + V.Y * d, P.Z + V.Z * d);
        }
        public static LineSegment3d MoveL(LineSegment3d L, Vector3d V)
        {
            var x1 = L.StartPoint.X + V.X;
            var y1 = L.StartPoint.Y + V.Y;
            var z1 = 0;

            var x2 = L.EndPoint.X + V.X;
            var y2 = L.EndPoint.Y + V.Y;
            var z2 = 0;

            var p1 = new Point3d(x1, y1, z1);
            var p2 = new Point3d(x2, y2, z2);

            return new LineSegment3d(p1, p2);
        }
        #endregion

        #region COPY
        public static List<ObjectId> Copy(List<ObjectId> ObjIds, Vector3d Vector)
        {
            var R = new List<ObjectId>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var A = from a in ObjIds
                        let b = t.GetObject(a, OpenMode.ForWrite) as Entity
                        where b != null
                        select b;

                if (A.Any())
                {
                    A.ToList().ForEach(a =>
                    {
                        var B = a.Clone() as Entity;

                        B.TransformBy(Matrix3d.Displacement(Vector));

                        BTR.AppendEntity(B);
                        t.AddNewlyCreatedDBObject(B, true);

                        R.Add(t.GetObject(B.Id, OpenMode.ForWrite).ObjectId);
                    });
                }

                t.Commit();
            }

            return R;
        }
        public static List<ObjectId> Copy(SelectionSet acSSet, Vector3d Vector)
        {
            var R = new List<ObjectId>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var A = from a in acSSet.GetObjectIds()
                        let b = t.GetObject(a, OpenMode.ForWrite) as Entity
                        where b != null
                        select b;

                if (A.Any())
                {
                    A.ToList().ForEach(a =>
                    {
                        var B = a.Clone() as Entity;

                        B.TransformBy(Matrix3d.Displacement(Vector));

                        BTR.AppendEntity(B);
                        t.AddNewlyCreatedDBObject(B, true);

                        R.Add(B.ObjectId);
                    });
                }

                t.Commit();
            }

            return R;
        }
        public static List<ObjectId> Copy(List<Polyline> acPolys, Vector3d Vector)
        {
            var R = new List<ObjectId>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var A = from a in acPolys
                        let b = t.GetObject(a.Id, OpenMode.ForWrite) as Entity
                        where b != null
                        select b;

                if (A.Any())
                {
                    A.ToList().ForEach(a =>
                    {
                        var B = a.Clone() as Entity;

                        B.TransformBy(Matrix3d.Displacement(Vector));

                        BTR.AppendEntity(B);
                        t.AddNewlyCreatedDBObject(B, true);

                        R.Add(B.ObjectId);
                    });
                }

                t.Commit();
            }

            return R;
        }
        public static List<ObjectId> Copy(List<Entity> acEntitys, Vector3d Vector)
        {
            var R = new List<ObjectId>();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var A = from a in acEntitys
                        let b = t.GetObject(a.Id, OpenMode.ForWrite) as Entity
                        where b != null
                        select b;

                if (A.Any())
                {
                    A.ToList().ForEach(a =>
                    {
                        var B = a.Clone() as Entity;

                        B.TransformBy(Matrix3d.Displacement(Vector));

                        BTR.AppendEntity(B);
                        t.AddNewlyCreatedDBObject(B, true);

                        R.Add(B.ObjectId);
                    });
                }

                t.Commit();
            }

            return R;
        }
        public static ObjectId Copy(Polyline acPoly, Vector3d Vector)
        {
            var R = new ObjectId();

            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                var B = acPoly.Clone() as Entity;

                B.TransformBy(Matrix3d.Displacement(Vector));

                BTR.AppendEntity(B);
                t.AddNewlyCreatedDBObject(B, true);

                R = B.ObjectId;

                t.Commit();
            }

            return R;
        }

        #endregion

        #region ERASE
        public static void Erase(ObjectId id)
        {
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                T.GetObject(id, OpenMode.ForWrite).Erase(true);

                T.Commit();
            }

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
        }
        public static void Erases(List<long?> handles)
        {
            Erases(handles.Select(handle => to.ObjectId(handle)).ToList());
        }
        public static void Erases(List<ObjectId> IDs)
        {
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                IDs.Where(id => !id.IsErased).ToList().ForEach(id =>
                {
                    try
                    {
                        T.GetObject(id, OpenMode.ForWrite).Erase(true);
                    }
                    catch (System.Exception)
                    {
                    }
                });

                T.Commit();
            }

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
        }
        //public static void Erases(ObjectId layerid)
        //{
        //    var A = DBDataContext.Select<RoomItem>(current.W.P);
        //    if (!A.Any()) return;

        //    var B = Utils.Get.Entities(A.Select(a => Utils.to.ObjectId(a.Handle)).ToList())
        //                     .Where(x => !x.Id.IsErased)
        //                     .Where(x => x.LayerId == layerid)
        //                     .Select(x => x.ObjectId);

        //    if (B.Any()) cad.Erases(B.ToList());
        //}

        #endregion

        #region GROUP
        public void CreateGroup(ObjectIdCollection OIC, string GName)
        {
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var DBD = T.GetObject(AC.DB.GroupDictionaryId, OpenMode.ForWrite) as DBDictionary;

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
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
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
            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = T.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = T.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                L.GetOffsetCurves(D);

                BTR.AppendEntity(L);
                T.AddNewlyCreatedDBObject(L, true);
                T.Commit();
            }

            return L;
        }
        #endregion

        #region VECTOR
        public static Vector2d GetVector(Curve2d curve)
        {
            return GetVector(curve.StartPoint, curve.EndPoint);
        }
        public static Vector2d GetVector(Line line)
        {
            return GetVector(line.StartPoint, line.EndPoint);
        }
        public static Vector2d GetVector(Point2d P1, Point2d P2)
        {
            return new Vector2d(P2.X - P1.X, P2.Y - P1.Y).GetNormal();
        }
        public static Vector2d GetVector(Point3d P1, Point3d P2)
        {
            return new Vector2d(P2.X - P1.X, P2.Y - P1.Y).GetNormal();
        }

        public static Vector3d GetVector3d(Point2d P1, Point2d P2)
        {
            return new Vector3d(P2.X - P1.X, P2.Y - P1.Y, 0).GetNormal();
        }
        public static Vector3d GetVector3d(Point3d P1, Point3d P2)
        {
            return new Vector3d(P2.X - P1.X, P2.Y - P1.Y, 0).GetNormal();
        }

        public static Vector3d GetReflectedVector(Vector3d FV, Vector3d IV)
        {
            var Return = new Vector3d();

            var axisVector = FV.GetAngleTo(IV) > Math.PI ? Vector3d.ZAxis : -Vector3d.ZAxis;

            var VV = FV.CrossProduct(axisVector);   // Vertical Vector

            var V_Ang = Math.Atan2(VV.Y, VV.X);     // Vector Angle

            var N_Ang = V_Ang + new Random().Next(0, 90) / Math.PI;   // New Angle

            Return = new Vector3d(Math.Cos(N_Ang), Math.Sin(N_Ang), 0);

            return Return;
        }
        #endregion

        #region GET VALUE
        public static List<Point3d> getFurPoints(Line L1, Line L2)
        {
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;
            var Ps1 = new List<Point3d>();
            Ps1.Add(sp1);
            Ps1.Add(ep1);

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;
            var Ps2 = new List<Point3d>();
            Ps2.Add(sp2);
            Ps2.Add(ep2);

            var Points = from p1 in Ps1
                         from p2 in Ps2
                         let d = p1.DistanceTo(p2)
                         orderby d descending
                         select new List<Point3d> { p1, p2 };

            return Points.First();
        }
        public static List<Point3d> getFurPoints(LineSegment3d L1, LineSegment3d L2)
        {
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;
            var Ps1 = new List<Point3d>();
            Ps1.Add(sp1);
            Ps1.Add(ep1);

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;
            var Ps2 = new List<Point3d>();
            Ps2.Add(sp2);
            Ps2.Add(ep2);

            var Points = from p1 in Ps1
                         from p2 in Ps2
                         let d = p1.DistanceTo(p2)
                         orderby d descending
                         select new List<Point3d> { p1, p2 };

            return Points.First();
        }
        public static List<Point3d> getFurPoints(Polyline polyline1, List<Polyline> Ps)
        {
            var Return = new List<Point3d>();

            var L1 = polyline1.GetLineSegmentAt(0);
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;
            var Ps1 = new List<Point3d>();
            Ps1.Add(sp1);
            Ps1.Add(ep1);

            double D = 0;

            Ps.ForEach(polyline2 =>
            {
                var L2 = polyline2.GetLineSegmentAt(0);
                var sp2 = L2.StartPoint;
                var ep2 = L2.EndPoint;
                var Ps2 = new List<Point3d>();
                Ps2.Add(sp2);
                Ps2.Add(ep2);

                double Dis = getFurDistance(L1, L2);

                if (D < Dis)
                {
                    D = Dis;
                    Return = getFurPoints(L1, L2);
                }
            });

            return Return;
        }
        public static List<Point3d> getFurPoints(List<Polyline> Ps)
        {
            var Return = new List<Point3d>();

            Return = (from p1 in Ps
                      from p2 in Ps
                      let L1 = p1.GetLineSegmentAt(0)
                      let L2 = p2.GetLineSegmentAt(0)
                      let d = getFurDistance(L1, L2)
                      orderby d descending
                      select getFurPoints(L1, L2)).First();

            return Return;
        }

        public static Point3d GetIntersectedPoint(Point3d P, Vector3d V, LineSegment3d L)
        {
            var EP = new Point3d(P.X + V.X, P.Y + V.Y, P.Z + V.Z);

            LineSegment3d InputLine = new LineSegment3d(P, EP);

            return GetIntersectPoint(InputLine, L);
        }

        public static Point3d GetIntersectedPoint(List<Entity> Ents, Point3d P, Vector3d V)
        {
            Point3d Return = new Point3d();

            var InputLine = new LineSegment3d(P, MoveP(P, V, 10));

            Return = IntersectedLine(Ents, InputLine, P);

            return Return;
        }

        public static List<Entity> IntersectedEntities(List<Entity> Ents, LineSegment3d L)
        {
            List<Entity> Return = new List<Entity>();

            Ents.ForEach(ent =>
            {
                if (ent is Line)
                {
                    var line = ToLineSegment3d(ent as Line);

                    if (line.IntersectWith(L) != null)
                    {
                        Return.Add(ent);
                    }
                }
                else if (ent is Polyline)
                {
                    var acPoly = ent as Polyline;

                    for (int i = 0; i < acPoly.NumberOfVertices - 1; i++)
                    {
                        var line = acPoly.GetLineSegmentAt(i);

                        if (line.IntersectWith(L) != null)
                        {
                            Return.Add(ent);
                        }
                    }
                }
            });

            return Return;
        }
        public static List<LineSegment3d> IntersectedLines(List<Entity> Ents, LineSegment3d L)
        {
            var Return = new List<LineSegment3d>();

            Ents.ForEach(ent =>
            {
                if (ent is Line)
                {
                    var line = ToLineSegment3d(ent as Line);

                    var IP = GetIntersectPoint(L, line);

                    if (line.GetDistanceTo(IP) < 1)
                    {
                        var D1 = new LineSegment3d(L.StartPoint, IP).Direction;
                        var D2 = L.Direction;

                        if (Math.Abs(D1.X - D2.X) < 0.1 && Math.Abs(D1.Y - D2.Y) < 0.1)
                        {
                            Return.Add(line);
                        }
                    }
                }
                else if (ent is Polyline)
                {
                    var acPoly = ent as Polyline;

                    for (int i = 0; i < acPoly.NumberOfVertices; i++)
                    {
                        try
                        {
                            var line = acPoly.GetLineSegmentAt(i);

                            var IP = GetIntersectPoint(L, line);

                            if (line.GetDistanceTo(IP) < 1)
                            {
                                var D1 = new LineSegment3d(L.StartPoint, IP).Direction;
                                var D2 = L.Direction;

                                if (Math.Abs(D1.X - D2.X) < 0.1 && Math.Abs(D1.Y - D2.Y) < 0.1)
                                {
                                    Return.Add(line);
                                }
                            }
                        }
                        catch { }
                    }
                }
            });

            return Return;
        }
        public static LineSegment3d IntersectedLine(List<Entity> Ents, LineSegment3d L)
        {
            var Return = new LineSegment3d();

            var ILs = new List<LineSegment3d>();
            var IPs = new List<Point3d>();

            #region Get Intersected Lines
            Ents.ForEach(ent =>
            {
                if (ent is Line)
                {
                    var line = ToLineSegment3d(ent as Line);

                    var IP = GetIntersectPointwithVector(L, line);

                    if (line.GetDistanceTo(IP) < 1)
                    {
                        cad.CreateCircle(IP, 5);
                        ILs.Add(line);
                        IPs.Add(IP);
                    }
                }
                else if (ent is Polyline)
                {
                    var acPoly = ent as Polyline;

                    for (int i = 0; i < acPoly.NumberOfVertices; i++)
                    {
                        try
                        {
                            var line = acPoly.GetLineSegmentAt(i);

                            var IP = GetIntersectPointwithVector(L, line);

                            if (line.GetDistanceTo(IP) < 1)
                            {
                                cad.CreateCircle(IP, 5);
                                ILs.Add(line);
                                IPs.Add(IP);
                            }
                        }
                        catch { }
                    }
                }
            });
            #endregion

            var orderedILs = from a in ILs
                             orderby a.GetDistanceTo(L.StartPoint)
                             select a;

            if (orderedILs.Any()) Return = orderedILs.First();

            return Return;
        }

        public static Point3d IntersectedLine(List<Entity> Ents, LineSegment3d L, Point3d P)
        {
            var Return = new Point3d();

            var IPs = new List<Point3d>();

            #region Get Intersected Lines
            Ents.ForEach(ent =>
            {
                if (ent is Line)
                {
                    var line = ToLineSegment3d(ent as Line);

                    var IP = GetIntersectPointwithVector(L, line);

                    if (line.GetDistanceTo(IP) < 1)
                    {
                        cad.CreateCircle(IP, 5);
                        IPs.Add(IP);
                    }
                }
                else if (ent is Polyline)
                {
                    var acPoly = ent as Polyline;

                    for (int i = 0; i < acPoly.NumberOfVertices; i++)
                    {
                        try
                        {
                            var line = acPoly.GetLineSegmentAt(i);

                            var IP = GetIntersectPointwithVector(L, line);

                            if (line.GetDistanceTo(IP) < 1)
                            {
                                cad.CreateCircle(IP, 5);
                                IPs.Add(IP);
                            }
                        }
                        catch { }
                    }
                }
            });
            #endregion

            var NearPoints = from a in IPs
                             let d = a.DistanceTo(P)
                             where d != 0
                             orderby d
                             select a;

            if (NearPoints.Any())
            {
                Return = NearPoints.First();
            }

            return Return;
        }
        /// <summary>
        /// L을 L의 방향으로 D 만큼 연장시켰을 때 Ents 객체와 교차되는 가장 가까운 점을 받아온다.
        /// </summary>
        /// <param name="Ents">교차를 비교할 객체들</param>
        /// <param name="L">간섭시킬 Line 객체</param>
        /// <param name="P">기준점</param>
        /// <param name="D">기준점으로 부터의 거리</param>
        /// <returns></returns>
        public static Point3d IntersectedLine(List<Entity> Ents, LineSegment3d L, Point3d P, double D = 500)
        {
            var Return = new Point3d();

            var IPs = new List<Point3d>();

            #region Get Intersected Lines
            Ents.ForEach(ent =>
            {
                if (ent is Line)
                {
                    var line = ToLineSegment3d(ent as Line);

                    var IP = GetIntersectPointwithVector(L, line);

                    if (line.GetDistanceTo(IP) < 1)
                    {
                        IPs.Add(IP);
                    }
                }
                else if (ent is Polyline)
                {
                    var acPoly = ent as Polyline;

                    for (int i = 0; i < acPoly.NumberOfVertices; i++)
                    {
                        try
                        {
                            var line = acPoly.GetLineSegmentAt(i);

                            var IP = GetIntersectPointwithVector(L, line);

                            if (line.GetDistanceTo(IP) < 1)
                            {
                                IPs.Add(IP);
                            }
                        }
                        catch { }
                    }
                }
            });
            #endregion

            var NearPoints = from a in IPs
                             let d = a.DistanceTo(P)
                             where 0 < d && d < D
                             orderby d
                             select a;

            if (NearPoints.Any())
            {
                Return = NearPoints.First();
            }

            return Return;
        }
        public static Point3d IntersectedLine(List<Polyline> Polylines, LineSegment3d L, Point3d P, double D = 500)
        {
            var Return = new Point3d();

            var IPs = new List<Point3d>();

            #region Get Intersected Lines
            Polylines.ForEach(polyline =>
            {
                for (int i = 0; i < polyline.NumberOfVertices; i++)
                {
                    try
                    {
                        var line = polyline.GetLineSegmentAt(i);

                        var IP = GetIntersectPointwithVector(L, line);

                        if (line.GetDistanceTo(IP) < 1)
                        {
                            IPs.Add(IP);
                        }
                    }
                    catch { }
                }
            });
            #endregion

            var NearPoints = from a in IPs
                             let d = a.DistanceTo(P)
                             where 0 < d && d < D
                             orderby d
                             select a;

            if (NearPoints.Any())
            {
                Return = NearPoints.First();
            }

            return Return;
        }


        public static Point3d GetIntersectPoint(LineSegment3d L1, LineSegment3d L2)
        {
            var Return = new Point3d();

            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;

            if (!IsParallel(L1, L2))
            {
                var x1 = sp1.X;
                var y1 = sp1.Y;
                var x2 = ep1.X;
                var y2 = ep1.Y;

                var x3 = sp2.X;
                var y3 = sp2.Y;
                var x4 = ep2.X;
                var y4 = ep2.Y;

                var X = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
                        ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                var Y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
                        ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                var Z = sp1.Z;

                Return = new Point3d(X, Y, Z);
            }

            return Return;
        }
        public static Point3d GetIntersectPointwithVector(LineSegment3d L1, LineSegment3d L2)
        {
            var Return = new Point3d();

            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;

            if (!IsParallel(L1, L2))
            {
                var x1 = sp1.X;
                var y1 = sp1.Y;
                var x2 = ep1.X;
                var y2 = ep1.Y;

                var x3 = sp2.X;
                var y3 = sp2.Y;
                var x4 = ep2.X;
                var y4 = ep2.Y;

                var X = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
                        ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                var Y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
                        ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

                var Z = sp1.Z;

                Return = new Point3d(X, Y, Z);
            }

            var v1 = GetVector3d(sp1, ep1);
            var v2 = GetVector3d(sp1, Return);

            // 같은 방향 판별
            if (Math.Abs(v1.X - v2.X) > 0.1 && Math.Abs(v1.Y - v2.Y) > 0.1)
            {
                Return = new Point3d();
            }

            return Return;
        }


        public static bool IsParallel(LineSegment3d L1, LineSegment3d L2)
        {
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;

            var x1 = sp1.X;
            var y1 = sp1.Y;
            var x2 = ep1.X;
            var y2 = ep1.Y;

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;

            var x3 = sp2.X;
            var y3 = sp2.Y;
            var x4 = ep2.X;
            var y4 = ep2.Y;
            var A = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            return Math.Round((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)) == 0 ? true : false;
        }

        public static bool IsIntersect(LineSegment3d L1, LineSegment3d L2)
        {
            return L1.IntersectWith(L2).Any();
        }

        public static List<Point3d> GetIntersectedPoints(LineSegment3d L1, LineSegment3d L2)
        {
            return L1.IntersectWith(L2).ToList();
        }





        public static double getFurDistance(LineSegment3d L1, LineSegment3d L2)
        {
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;
            var Ps1 = new List<Point3d>();
            Ps1.Add(sp1);
            Ps1.Add(ep1);

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;
            var Ps2 = new List<Point3d>();
            Ps2.Add(sp2);
            Ps2.Add(ep2);

            var Distances = from p1 in Ps1
                            from p2 in Ps2
                            let d = p1.DistanceTo(p2)
                            orderby d descending
                            select d;

            return Distances.First();
        }
        public static double getNearDistance(LineSegment3d L1, LineSegment3d L2)
        {
            var sp1 = L1.StartPoint;
            var ep1 = L1.EndPoint;
            var Ps1 = new List<Point3d>();
            Ps1.Add(sp1);
            Ps1.Add(ep1);

            var sp2 = L2.StartPoint;
            var ep2 = L2.EndPoint;
            var Ps2 = new List<Point3d>();
            Ps2.Add(sp2);
            Ps2.Add(ep2);

            var Distances = from p1 in Ps1
                            from p2 in Ps2
                            let d = p1.DistanceTo(p2)
                            orderby d ascending
                            select d;

            return Distances.First();
        }

        public static double GetAngle(LineSegment3d L)
        {
            var SP = L.StartPoint;
            var EP = L.EndPoint;

            var Ang = (EP.Y - SP.Y) / (EP.X - SP.X);

            if (Math.Abs(EP.X - SP.X) < 0.00000001)
            {
                Ang = 0;
            }
            if (Math.Abs(EP.Y - SP.Y) < 0.00000001)
            {
                Ang = 0;
            }

            return Ang;
        }
        #endregion

        #region TO
        public static LineSegment3d ToLineSegment3d(Line L)
        {
            return new LineSegment3d(L.StartPoint, L.EndPoint);
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
            if (AC.DB.TileMode == true)
            {
                if (P1.IsEqualTo(new Point3d()) && P2.IsEqualTo(new Point3d()))
                {
                    P1 = AC.DB.Extmin; // Extents Space
                    P2 = AC.DB.Extmax;
                }
            }
            else
            {
                if (nCurVport == 1)
                {
                    if (P1.IsEqualTo(new Point3d()) && P2.IsEqualTo(new Point3d()))
                    {
                        P1 = AC.DB.Pextmin;    // Paper Space
                        P2 = AC.DB.Pextmax;
                    }
                }
                else
                {
                    if (P1.IsEqualTo(new Point3d()) && P2.IsEqualTo(new Point3d()))
                    {
                        P1 = AC.DB.Extmin;
                        P2 = AC.DB.Extmax;
                    }
                }
            }
            #endregion

            using (Transaction T = AC.DB.TransactionManager.StartTransaction())
            {
                using (ViewTableRecord VTR = AC.Editor.GetCurrentView())
                {
                    Extents3d extents;

                    // Translate WCS coordinates to DCS
                    Matrix3d matrix;
                    matrix = Matrix3d.PlaneToWorld(VTR.ViewDirection);
                    matrix = Matrix3d.Displacement(VTR.Target - Point3d.Origin) * matrix;
                    matrix = Matrix3d.Rotation(-VTR.ViewTwist, VTR.ViewDirection, VTR.Target) * matrix;

                    //MessageBox.Show(P1.ToString() + "\n" + P2.ToString() + "\n" + P3.ToString());

                    //if (!P3.IsEqualTo(Point3d.Origin))
                    //{
                    //    P1 = new Point3d(P3.X - (VTR.Width / 2), P3.Y - (VTR.Height / 2), 0);

                    //    P2 = new Point3d((VTR.Width / 2) + P3.X, (VTR.Height / 2) + P3.Y, 0);

                    //    //MessageBox.Show(P1.ToString() + "\n" + P2.ToString());
                    //}

                    using (var L = new Line(P1, P2))
                    {
                        extents = new Extents3d(L.Bounds.Value.MinPoint, L.Bounds.Value.MaxPoint);

                        //MessageBox.Show(L.Bounds.Value.MinPoint.ToString() + "\n" + L.Bounds.Value.MaxPoint.ToString());
                    }

                    double V_Ratio = VTR.Width / VTR.Height;

                    Matrix3d n_matrix;
                    n_matrix = matrix.Inverse();
                    extents.TransformBy(n_matrix);

                    double W;
                    double H;
                    Point2d NewCenP;

                    if (!P3.IsEqualTo(Point3d.Origin))
                    {
                        W = VTR.Width;
                        H = VTR.Height;

                        if (dFactor == 0)
                        {
                            P3 = P3.TransformBy(n_matrix);
                        }

                        NewCenP = new Point2d(P3.X, P3.Y);
                    }
                    else
                    {
                        W = extents.MaxPoint.X - extents.MinPoint.X;
                        H = extents.MaxPoint.Y - extents.MinPoint.Y;

                        NewCenP = new Point2d((extents.MaxPoint.X + extents.MinPoint.X) * 0.5,
                                              (extents.MaxPoint.Y + extents.MinPoint.Y) * 0.5);
                    }

                    if (W > (H * V_Ratio)) H = W / V_Ratio;

                    if (dFactor != 0)
                    {
                        VTR.Height = H * dFactor;
                        VTR.Width = W * dFactor;
                    }

                    VTR.CenterPoint = NewCenP;

                    //string s = "";
                    //s += VTR.CenterPoint.ToString() + "\n";
                    //s += VTR.Height.ToString() + "\n";
                    //s += VTR.Width.ToString() + "\n";

                    AC.Editor.SetCurrentView(VTR);
                }

                T.Commit();
            }
        }

        public static void ZoomIn(Point3d pMin, Point3d pMax, Point3d pCenter, double dFactor)
        {
            int nCurVport = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"));

            if (AC.DB.TileMode == true)
            {
                if (pMin.Equals(new Point3d()) == true &&
                    pMax.Equals(new Point3d()) == true)
                {
                    pMin = AC.DB.Extmin;
                    pMax = AC.DB.Extmax;
                }
            }
            else
            {
                if (nCurVport == 1)
                {
                    if (pMin.Equals(new Point3d()) == true &&
                        pMax.Equals(new Point3d()) == true)
                    {
                        pMin = AC.DB.Pextmin;
                        pMax = AC.DB.Pextmax;
                    }
                }
                else
                {
                    if (pMin.Equals(new Point3d()) == true &&
                        pMax.Equals(new Point3d()) == true)
                    {
                        pMin = AC.DB.Extmin;
                        pMax = AC.DB.Extmax;
                    }
                }
            }

            using (Transaction acTrans = AC.DB.TransactionManager.StartTransaction())
            {
                using (ViewTableRecord acView = AC.Doc.Editor.GetCurrentView())
                {
                    Extents3d eExtents;

                    Matrix3d matWCS2DCS;
                    matWCS2DCS = Matrix3d.PlaneToWorld(acView.ViewDirection);
                    matWCS2DCS = Matrix3d.Displacement(acView.Target - Point3d.Origin) * matWCS2DCS;
                    matWCS2DCS = Matrix3d.Rotation(-acView.ViewTwist,
                                                    acView.ViewDirection,
                                                    acView.Target) * matWCS2DCS;

                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        pMin = new Point3d(pCenter.X - (acView.Width / 2),
                                            pCenter.Y - (acView.Height / 2), 0);

                        pMax = new Point3d((acView.Width / 2) + pCenter.X,
                                            (acView.Height / 2) + pCenter.Y, 0);
                    }

                    using (Line acLine = new Line(pMin, pMax))
                    {
                        eExtents = new Extents3d(acLine.Bounds.Value.MinPoint,
                                                    acLine.Bounds.Value.MaxPoint);
                    }

                    double dViewRatio;
                    dViewRatio = (acView.Width / acView.Height);

                    matWCS2DCS = matWCS2DCS.Inverse();
                    eExtents.TransformBy(matWCS2DCS);

                    double dWidth;
                    double dHeight;
                    Point2d pNewCentPt;

                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        dWidth = acView.Width;
                        dHeight = acView.Height;

                        if (dFactor == 0)
                        {
                            pCenter = pCenter.TransformBy(matWCS2DCS);
                        }

                        pNewCentPt = new Point2d(pCenter.X, pCenter.Y);
                    }
                    else // Working in Window, Extents and Limits mode
                    {
                        dWidth = eExtents.MaxPoint.X - eExtents.MinPoint.X;
                        dHeight = eExtents.MaxPoint.Y - eExtents.MinPoint.Y;

                        pNewCentPt = new Point2d(((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5),
                                                    ((eExtents.MaxPoint.Y + eExtents.MinPoint.Y) * 0.5));
                    }

                    if (dWidth > (dHeight * dViewRatio)) dHeight = dWidth / dViewRatio;

                    if (dFactor != 0)
                    {
                        acView.Height = dHeight * dFactor;
                        acView.Width = dWidth * dFactor;
                    }

                    acView.CenterPoint = pNewCentPt;

                    AC.Doc.Editor.SetCurrentView(acView);
                }

                acTrans.Commit();
            }
        }
        public static void ZoomIn(Point3d pMin, Point3d pMax)
        {
            
            var app = Application.AcadApplication as Autodesk.AutoCAD.Interop.AcadApplication;

            double[] lower = new double[3] { pMin.X, pMin.Y, 0 };
            double[] upper = new double[3] { pMax.X, pMax.Y, 0 };

            app.ZoomWindow(lower, upper);

            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
        }
        public static void ZoomIn(List<Polyline> Polylines, double d = 0.08)
        {
            if (Polylines.Count == 0) return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            Polylines.ForEach(poly =>
            {
                try
                {
                    for (int i = 0; i < poly.NumberOfVertices; i++)
                    {
                        var l = poly.GetLineSegmentAt(i);
                        var p = l.StartPoint;
                        var x = p.X;
                        var y = p.Y;

                        if (minX > x) minX = x;
                        if (minY > y) minY = y;
                        if (maxX < x) maxX = x;
                        if (maxY < y) maxY = y;
                    }
                }
                catch (System.Exception)
                {
                }
            });

            var lx = maxX - minX;
            var ly = maxY - minY;

            var Min = new Point3d(minX - lx * d, minY - ly * d, 0);
            var Max = new Point3d(maxX + lx * d, maxY + ly * d, 0);

            cad.ZoomIn(Min, Max);
        }
        public static void ZoomIn(List<ObjectId> objIDs, double d = 0.08)
        {
            if (objIDs.Count == 0) return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            var Polylines = Get.Objects<Polyline>(objIDs);

            Polylines.ForEach(poly =>
            {
                for (int i = 0; i < poly.NumberOfVertices; i++)
                {
                    try
                    {
                        var l = poly.GetLineSegmentAt(i);
                        if (l == null) continue;
                        var p = l.StartPoint;
                        var x = p.X;
                        var y = p.Y;

                        if (minX > x) minX = x;
                        if (minY > y) minY = y;
                        if (maxX < x) maxX = x;
                        if (maxY < y) maxY = y;
                    }
                    catch { }
                }
            });

            var lx = maxX - minX;
            var ly = maxY - minY;

            var Min = new Point3d(minX - lx * d, minY - ly * d, 0);
            var Max = new Point3d(maxX + lx * d, maxY + ly * d, 0);

            cad.ZoomIn(Min, Max);
        }
        public static void ZoomIn(List<long> handles, double d = 0.08)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            var plines = Get.Objects<Polyline>(handles);
            if (plines == null || plines.Count == 0) return;

            plines.ForEach(poly =>
            {
                for (int i = 0; i < poly.NumberOfVertices; i++)
                {
                    var l = poly.GetLineSegmentAt(i);
                    var p = l.StartPoint;
                    var x = p.X;
                    var y = p.Y;

                    if (minX > x) minX = x;
                    if (minY > y) minY = y;
                    if (maxX < x) maxX = x;
                    if (maxY < y) maxY = y;
                }
            });

            var lx = maxX - minX;
            var ly = maxY - minY;

            var Min = new Point3d(minX - lx * d, minY - ly * d, 0);
            var Max = new Point3d(maxX + lx * d, maxY + ly * d, 0);

            cad.ZoomIn(Min, Max);
        }
        public static void ZoomIn(long handle, double d = 0.08)
        {
            var line = Get.Object<Polyline>(handle);

        }
        #endregion

        #region 드로잉 순서 Drawing Order
        public static void DrawingOrders(List<ObjectId> ObjIDs, string TB)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var DOT = t.GetObject(BTR.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;

                var objToMove = new ObjectIdCollection();
                ObjIDs.ForEach(id => objToMove.Add(id));

                if (objToMove.Count != 0)
                {
                    switch (TB)
                    {
                        case "T": DOT.MoveToTop(objToMove); break;
                        case "B": DOT.MoveToBottom(objToMove); break;
                    }
                }

                t.Commit();
            }
        }
        public static void DrawingOrder(ObjectId ObjID, string TB)
        {
            using (Transaction t = AC.DB.TransactionManager.StartTransaction())
            {
                var BT = t.GetObject(AC.DB.BlockTableId, OpenMode.ForRead) as BlockTable;
                var BTR = t.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                var DOT = t.GetObject(BTR.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;

                var objToMove = new ObjectIdCollection();
                objToMove.Add(ObjID);

                if (objToMove.Count != 0)
                {
                    switch (TB)
                    {
                        case "T": DOT.MoveToTop(objToMove); break;
                        case "B": DOT.MoveToBottom(objToMove); break;
                    }
                }

                t.Commit();
            }
        }
        #endregion
    }
}
