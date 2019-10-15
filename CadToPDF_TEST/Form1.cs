using Aspose.CAD;
using Aspose.CAD.ImageOptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadToPDF_TEST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Test02();
        }

        void Test01()
        {
            var OFD = new OpenFileDialog();

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var path = OFD.FileName;

                using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(path))
                {
                    CadRasterizationOptions opt = new CadRasterizationOptions();
                    opt.PageWidth = 1200;
                    opt.PageHeight = 1200;

                    ImageOptionsBase opts = new PdfOptions();

                    opts.VectorRasterizationOptions = opt;

                    image.Save(path + "_TEST.pdf");
                }
            }
        }
        void Test02()
        {
            var OFD = new OpenFileDialog();

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var path = OFD.FileName;

                using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(path))
                {
                    var opt = new CadRasterizationOptions();
                    //opt.PageWidth = 1600;
                    //opt.PageHeight = 1600;

                    opt.AutomaticLayoutsScaling = true;
                    opt.NoScaling = true;

                    ImageOptionsBase pdfOpt = new PdfOptions();

                    pdfOpt.VectorRasterizationOptions = opt;

                    image.Save(path + "_TEST.pdf", pdfOpt);
                }
            }
        }
    }
}
