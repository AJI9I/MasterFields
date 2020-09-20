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
    public partial class UCCalibrationPoint : UserControl
    {
        public UCCalibrationPoint()
        {
            InitializeComponent();
            ChekedButtonEvent();
            PolarisationCheked();
            parametrAddform();
        }

        #region Отслеживание нажатия кнопки калибровки и получение ее имени
        private void ChekedButtonEvent()
        {
            radioButton1.CheckedChanged += new EventHandler(rbchek);
            radioButton2.CheckedChanged += new EventHandler(rbchek);
            radioButton3.CheckedChanged += new EventHandler(rbchek);
            radioButton4.CheckedChanged += new EventHandler(rbchek);
            radioButton5.CheckedChanged += new EventHandler(rbchek);
            radioButton6.CheckedChanged += new EventHandler(rbchek);
            radioButton7.CheckedChanged += new EventHandler(rbchek);
            radioButton8.CheckedChanged += new EventHandler(rbchek);
            radioButton9.CheckedChanged += new EventHandler(rbchek);
            radioButton10.CheckedChanged += new EventHandler(rbchek);
            radioButton11.CheckedChanged += new EventHandler(rbchek);
            radioButton12.CheckedChanged += new EventHandler(rbchek);
            radioButton13.CheckedChanged += new EventHandler(rbchek);
            radioButton14.CheckedChanged += new EventHandler(rbchek);
            radioButton15.CheckedChanged += new EventHandler(rbchek);
            radioButton16.CheckedChanged += new EventHandler(rbchek);
        }

        void rbchek(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            StaticParametr.PointName = rb.Text;
            StaticParametr.PointEnable = true;
        }
        #endregion

        #region Отслеживание выполнения условия выбора поляризации и получение выбранной поляризации
        private void PolarisationCheked()
        {
            radioButton17.CheckedChanged += new EventHandler(pochek);
            radioButton18.CheckedChanged += new EventHandler(pochek);
        }

        void pochek(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            StaticParametr.PolarisationName = rb.Text;
            StaticParametr.PolarisationEnable = true;
        }
        #endregion

        #region Добавляем параметры калибровки на форму
        private void parametrAddform()
        {
            label17.Text = " " + StaticParametr.FileParametrName;
            label7.Text = " " + Convert.ToString(StaticParametr.FqMax / StaticParametr.DelitelKHz) + " КГц";
            label9.Text = " " + Convert.ToString(StaticParametr.FqMin / StaticParametr.DelitelKHz) + " КГц";
            foreach (double str  in StaticParametr.TensionParametr)
            {
                label11.Text = label11.Text + " "+Convert.ToString(str);
            }
        }
        #endregion

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        XMLNewCalibrationPoint xmlnewcalibrationpoint;

        public void XMLCalibrationPointAdd()
        {
            xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
            if (StaticParametr.PolarisationEnable == true && StaticParametr.PointEnable == true)
                xmlnewcalibrationpoint.CalibrationPointAddFile(StaticParametr.PointName, StaticParametr.PolarisationName);
        }

                private void button1_Click(object sender, EventArgs e)
        {
            xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
            if (StaticParametr.PolarisationEnable == true && StaticParametr.PointEnable == true)
                xmlnewcalibrationpoint.CalibrationPointAddFile(StaticParametr.PointName, StaticParametr.PolarisationName);
        }
    }
}
