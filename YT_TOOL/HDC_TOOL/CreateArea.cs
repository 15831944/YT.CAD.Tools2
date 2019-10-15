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

namespace YT_CAD_TOOL.HDC
{
    public class CreateArea
    {
        #region 필드
        Document acDoc;
        Database acDb;
        Editor acDocEd;
        DocumentLock acDL;
        #endregion

        #region 속성

        #endregion

        #region 생성자
        public CreateArea()
        {
            #region PRE
            acDoc = Application.DocumentManager.MdiActiveDocument;
            acDb = acDoc.Database;
            acDL = acDoc.LockDocument(DocumentLockMode.ProtectedAutoWrite, null, null, true);
            acDocEd = acDoc.Editor;
            #endregion
        }
        #endregion

        #region PUBLIC 메서드
        /// <summary>
        /// 폴리라인 중심에 면적 입력하기(단일 선택)
        /// </summary>
        /// <param name="TextSize">문자 크기</param>
        /// <param name="n">반올림 자리수</param>
        public void CreateSingleObj(double TextSize = 100, int n = 2)
        {
            bool B = true;

            while (B)
            {
                #region 단일 객체 선택
                var acSSet = Utils.Select.SingleObj();

                if (acSSet == null)
                {
                    B = false;

                    return;
                }

                var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));
                #endregion

                #region 면적 입력
                acEnts.ForEach(acEnt =>
                {
                    var acPolyline = acEnt as Polyline;

                    var p = Utils.PointUtil.GetCenterP(acPolyline);

                    var PL_Area = acPolyline.Area;  // mm²

                    var Area = Math.Round(PL_Area / 100000, n); // m²

                    var acText = CADUtil.CreateText(p, TextSize, Area.ToString() + "m²", ColorIndex.Green);

                    acDocEd.WriteMessage(" 폴리라인에 면적 [" + acText.TextString + "]이 입력되었습니다. \n");
                });
                #endregion
            }

            acDocEd.PostCommandPrompt();
        }

        /// <summary>
        /// 폴리라인 중심에 면적 입력하기(복수 선택)
        /// </summary>
        /// <param name="TextSize">문자 크기</param>
        /// <param name="n">반올림 자리수</param>
        public void CreateMultiObj(double TextSize = 100, int n = 2)
        {
            #region 복수 객체 선택
            var acSSet = Utils.Select.MultiObjs();

            if (acSSet == null)
            {
                return;
            }

            var acEnts = Utils.Get.Entity(acSSet, typeof(Polyline));
            #endregion

            #region 면적 입력
            acEnts.ForEach(acEnt =>
            {
                var acPolyline = acEnt as Polyline;

                var p = Utils.PointUtil.GetCenterP(acPolyline);

                var PL_Area = acPolyline.Area;  // mm²

                var Area = Math.Round(PL_Area / 100000, n); // m²

                var acText = CADUtil.CreateText(p, TextSize, Area.ToString() + "m²", ColorIndex.Green);

                acDocEd.WriteMessage(" 폴리라인에 면적 [" + acText.TextString + "]이 입력되었습니다. \n");
            });
            #endregion

            acDocEd.PostCommandPrompt();
        }
        #endregion

        #region PRIVATE 메서드

        #endregion
    }
}
