using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterFields
{
    public partial class UCStartPage : UserControl
    {
        FindAndCreateFolder findandcreatefolder;

        public UCStartPage()
        {
            InitializeComponent();
            comboboxFileParametrAdd();
            AddParametrForm();
        }

        private void comboboxFileParametrAdd()
        {
            ComboboxFileClear();
            string[] ii = CatalogGet();
            comboBox1.Items.AddRange(ii);
        }

        private void ComboboxFileClear()
        {
            comboBox1.Items.Clear();
        }

        private string[] CatalogGet()
        {
            findandcreatefolder = new FindAndCreateFolder();
            return findandcreatefolder.FindCalibrationFileParametr();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboboxFileParametrAdd();
            
        }

        XMLNewParametrFile xmlnewparametrfile;
        private void button2_Click(object sender, EventArgs e)
        {
            xmlnewparametrfile = new XMLNewParametrFile();
            findandcreatefolder = new FindAndCreateFolder();
            StaticParametr.FileParametrName = comboBox1.SelectedItem.ToString();
            xmlnewparametrfile.XMLFileParametrGetParametr(comboBox1.SelectedItem.ToString(), findandcreatefolder.GetCurrentDirectory());
            AddParametrFileInLabel();
        }

        #region Лэйблы содержащие информацию о загруженном файле
        Label TensLabel, FqMaxLabel, FqMinLabel, FqStepLabel, FqStepParamLabel, curingTimeLabel, FqCountLabel;
        
        private void AddParametrForm()
        {
            if (TensLabel == null)
                TensLabel = new Label();
            if (FqMaxLabel == null)
                FqMaxLabel = new Label();
            if (FqMinLabel == null)
                FqMinLabel = new Label();
            if (FqStepLabel == null)
                FqStepLabel = new Label();
            if (FqStepParamLabel == null)
                FqStepParamLabel = new Label();
            if (curingTimeLabel == null)
                curingTimeLabel = new Label();
            if (FqCountLabel == null)
                FqCountLabel = new Label();
        }

        private void AddParametrFileInLabel()
        {
            TensLabel.Location = new Point(3, 50);
            TensLabel.Size = new Size(200, 20);
            TensLabel.Text = "Напряженность: "+ String.Join(", ", StaticParametr.TensionParametr)+ " В";
            Controls.Add(TensLabel);
            TensLabel.BringToFront();

            FqMaxLabel.Location = new Point(3, 80);
            FqMaxLabel.Size = new Size(200, 20);
            FqMaxLabel.Text = "Максимальная частота: " + Convert.ToString(StaticParametr.FqMax) + " МГц";
            Controls.Add(FqMaxLabel);
            FqMaxLabel.BringToFront();

            FqMinLabel.Location = new Point(3, 110);
            FqMinLabel.Size = new Size(200, 20);
            FqMinLabel.Text = "Минимальная частота: " + Convert.ToString(StaticParametr.FqMin) + " МГц";
            Controls.Add(FqMinLabel);
            FqMinLabel.BringToFront();

            curingTimeLabel.Location = new Point(3, 140);
            curingTimeLabel.Size = new Size(200, 20);
            curingTimeLabel.Text = "Время выдержки: " + Convert.ToString(StaticParametr.Time) + " с";
            Controls.Add(curingTimeLabel);
            curingTimeLabel.BringToFront();

            FqCountLabel.Location = new Point(3, 170);
            FqCountLabel.Size = new Size(200, 20);
            FqCountLabel.Text = "Количество точек частоты: " + Convert.ToString(StaticParametr.FqStepArray.Count());
            Controls.Add(FqCountLabel);
            FqCountLabel.BringToFront();
        }

        
        #endregion
    }
}
