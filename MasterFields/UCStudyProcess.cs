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
    public partial class UCStudyProcess : UserControl
    {
        Label[] LabelBoxInForm;

        public UCStudyProcess()
        {
            InitializeComponent();

            LabelBoxInForm = new Label[] { label28, label29, label30, label31 };
            UpdateParametrForm();
        }

        #region Шлак который образовался при создании формы

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void imgVideo_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void imgVideo_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void imgVideo_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void bntSave_Click(object sender, EventArgs e)
        {

        }

        private void bntCapture_Click(object sender, EventArgs e)
        {

        }

        private void Stop_Thread_Click(object sender, EventArgs e)
        {

        }

        private void StartThread_Click(object sender, EventArgs e)
        {

        }

        private void bntContinue_Click(object sender, EventArgs e)
        {

        }

        private void bntStop_Click(object sender, EventArgs e)
        {

        }

        private void bntStart_Click_1(object sender, EventArgs e)
        {

        }

        private void UP_X_ValueChanged(object sender, EventArgs e)
        {

        }

        private void bntStop_Click_1(object sender, EventArgs e)
        {

        }
        #endregion

        #region Подкраска выбранных блоков на поле испытания
        private void label_Click(object sender, EventArgs e)
        {
            Label clickedlabel = sender as Label;
            if (clickedlabel.BackColor == Color.Green)
            {
                int index = Array.FindIndex(LabelOblastArray, p => p == Convert.ToInt32(clickedlabel.Text));
                LabelOblastArray[index] = 0;
                clickedlabel.BackColor = Color.CornflowerBlue;
                return;
            }
            if (Array.Exists(LabelOblastArray, p => p == 0))
            {
                int index = Array.FindIndex(LabelOblastArray, p => p == 0);
                LabelOblastArray[index] = Convert.ToInt32(clickedlabel.Text);
                clickedlabel.BackColor = Color.Green;
            }
            else
            {
                Array.Resize(ref LabelOblastArray, LabelOblastArray.Length + 1);
                LabelOblastArray[LabelOblastArray.Length-1] = Convert.ToInt32(clickedlabel.Text);
                clickedlabel.BackColor = Color.Green;
            }
            
            

        }
        #endregion

        #region обновление параметров на форме
        private void UpdateParametrForm()
        {
            LabelBoxInForm[0].Text = StaticParametr.FileParametrName;
            LabelBoxInForm[1].Text = StaticParametr.FqMin + " - " + StaticParametr.FqMax + " МГц";
            LabelBoxInForm[2].Text = "";
            foreach (int tens in StaticParametr.TensionParametr)
            {
                LabelBoxInForm[2].Text = LabelBoxInForm[2].Text + " " + tens;
            }
            LabelBoxInForm[2].Text = LabelBoxInForm[2].Text + " В/м^2";
            comboBox1.Items.Clear();
            foreach (int parametrtens in StaticParametr.TensionParametr)
            {
                comboBox1.Items.Add(parametrtens);
            }

        }

        #endregion

        #region Гроуп боксы удаление и восстановление в процессе анализа
        int[] positionGroupBoxUCFormSettingsStudy = new int[4];
        bool FormAnaliseClear = false;
        #region Удаление с формы
        private void UCStudyProcessClearFormAndAddAnalizeInformation()
        {
            positionGroupBoxUCFormSettingsStudy = new int[] { groupBox2.Location.X, groupBox3.Location.X, groupBox4.Location.X, groupBox7.Location.Y };
            bool WhileFalse = true;
            while (WhileFalse)
            {
                WhileFalse = false;
                if (groupBox2.Location.X < 1000)
                {
                    groupBox2.Location = new Point(groupBox2.Location.X + 10, groupBox2.Location.Y);
                    WhileFalse = true;
                }

                if (groupBox3.Location.X < 1000)
                {
                    groupBox3.Location = new Point(groupBox3.Location.X + 10, groupBox3.Location.Y);
                    WhileFalse = true;
                }

                if (groupBox4.Location.X > -400)
                {
                    groupBox4.Location = new Point(groupBox4.Location.X - 10, groupBox4.Location.Y);
                    WhileFalse = true;
                }

                if (groupBox7.Location.Y < 1000)
                {
                    groupBox7.Location = new Point(groupBox7.Location.X, groupBox7.Location.Y + 10);
                    WhileFalse = true;
                }
            }
            FormAnaliseClear = true;
        }
        #endregion
        #region Восставновление на форме
        private void UCStudyProcessAddFormAndAddAnalizeInformation()
        {
            bool WhileFalse = true;
            while (WhileFalse)
            {
                WhileFalse = false;
                if (groupBox2.Location.X > positionGroupBoxUCFormSettingsStudy[0])
                {
                    groupBox2.Location = new Point(groupBox2.Location.X - 10, groupBox2.Location.Y);
                    WhileFalse = true;
                }

                if (groupBox3.Location.X > positionGroupBoxUCFormSettingsStudy[1])
                {
                    groupBox3.Location = new Point(groupBox3.Location.X - 10, groupBox3.Location.Y);
                    WhileFalse = true;
                }

                if (groupBox4.Location.X < positionGroupBoxUCFormSettingsStudy[2])
                {
                    groupBox4.Location = new Point(groupBox4.Location.X + 10, groupBox4.Location.Y);
                    WhileFalse = true;
                }

                if (groupBox7.Location.Y > positionGroupBoxUCFormSettingsStudy[3])
                {
                    groupBox7.Location = new Point(groupBox7.Location.X, groupBox7.Location.Y - 10);
                    WhileFalse = true;
                }

            }
            FormAnaliseClear = false;
        }
        #endregion
        #endregion

        #region ListViev добавление
        ListView listview1;
        private void AddListViev()
        {
            if (listview1 == null)
                listview1 = new ListView();

            if (!listview1.Created)
                listview1 = new ListView();

            listview1.Location = new Point(10,10);
            listview1.Size = new Size(780, 660);
            //listview1.Items.Add("частота",1);
            //listview1.Columns.Add("Частота", "100", Left);
            //listview1.Columns.Add("sdfsdf", "100", Left);
            //listview1.Columns.Add("Частота", "100", Left);
            //listview1.Columns.Add("Частота", "100", Left);
            tabPage1.Controls.Add(listview1);
            
        }
        #endregion
        #region Кнопка проверки анализа
        AnalisticsField analisticsfield;
        int[] LabelOblastArray = new int[0];

        private void button1_Click(object sender, EventArgs e)
        {
            if (FormAnaliseClear == false)
            {
                UCStudyProcessClearFormAndAddAnalizeInformation();
            }
            else
            {
                UCStudyProcessAddFormAndAddAnalizeInformation();
            }
            AddListViev();
            int[] CalibrationPointArray = AnalisticsOblastFindPoint();
            analisticsfield = new AnalisticsField();
            int index = Array.FindIndex(StaticParametr.TensionParametr, p => p == Convert.ToDouble(comboBox1.SelectedItem));
            if (index != -1)
            {
                analisticsfield.WHAlistics(index, CalibrationPointArray);
                Analiz();
            }

        }

        #region Выявление необходимых точек для исследования,
        //Ответ содержит массив точек необходимых для анализа и исследования
        private int[] AnalisticsOblastFindPoint()
        {
            #region Точки калибровки привязанные к области на поле
            int[,] OblastPoint = new int[,] {
                {13, 14, 9,  10 },
                {14, 15, 10, 11 },
                {15, 16, 11, 12 },
                {9,  10, 5,  6 },
                {10, 11, 6,  7 },
                {11, 12, 7,  8 },
                {5,  6,  1,  2 },
                {6,  7,  2,  3 },
                {7,  8,  3,  4 }};
            #endregion

            #region Определение точек выбранных областей
            int[] CalibrationPointArray = new int[0];
            for (int i = 0; i<LabelOblastArray.Length;i++)
            {
                if (LabelOblastArray[i] != 0)
                {
                    int[] point = new int[4];
                    for (int b = 0; b < 4;b++)
                    {
                        point[b] = OblastPoint[LabelOblastArray[i]-1, b];
                    }
                    Array.Resize(ref CalibrationPointArray, CalibrationPointArray.Length+4);
                    Array.Copy(point, 0, CalibrationPointArray, CalibrationPointArray.Length - 4, 4);
                }
            }
            #endregion

            #region сортировка отобранных точек и удаление повторяющихся
            Array.Sort(CalibrationPointArray);
            int[] pointSort = new int[0];
            for (int t = 0; t < CalibrationPointArray.Length;t++)
            {
                if (!Array.Exists(pointSort, p => p == CalibrationPointArray[t]))
                {
                    Array.Resize(ref pointSort, pointSort.Length + 1);
                    pointSort[pointSort.Length - 1] = CalibrationPointArray[t];
                }
            }
            #endregion
            return pointSort;
        }
        #endregion
        #endregion

        #region Анализ
        private void Analiz()
        {
            //Переменная для записи на какой частоте идут отклонения либо все ок
            bool[] AnalizeFqComplited = new bool[StaticParametr.FqParametrArray.Length];

            double[] AnalizeAttParametr = new double[0];
            //Перебираем все частоты
            for (int i = 0; i< StaticParametr.FqParametrArray.Length;i++)
            {
                double[] AttParametrArray = GetFileParametrFq(i);

                //передаем количество точек напряженности поля для определения процента
                int SeventyFivePercent = Convert.ToInt32(SeventyFivePercentArray(AttParametrArray.Count()));
                SeventyFivePercent = AttParametrArray.Count() - SeventyFivePercent;

                Array.Sort(AttParametrArray);
                //Приходит ответ по максимальным значениям параметра напряженности, необходимо сделать выборку
                //и подгон результатов с разных параметров для того что бы поле считалось более однородным
                //приближенные значения
                AnalizeFqComplited[i] = FqTruFolse(SeventyFivePercent, AttParametrArray);

            }
        }

        #region Определение последовательности, удовлетворяет или нет уловию не менее 6 ДБ
        private bool FqTruFolse(int count, double[] AttParametrArray)
        {
            double[] AttArray = AttParametrArray;

            double summ = AttArray[AttArray.Length - 1] - AttArray[0];

            if (summ <= 6.0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Array.Resize(ref AttArray, AttArray.Length - 1);
                    summ = AttArray[AttArray.Length - 1] - AttArray[0];
                    if (summ <=6.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        //Расчет 75% точек от общего числа, точек расположененых на частоте
        private double SeventyFivePercentArray(double countPointArray)
        {
            double percent = countPointArray / 100.00;
            double SeventyFive = percent * 75.00;
            double cikle = Math.Ceiling(SeventyFive);
            return cikle;
        }

        //private bool AnalizePercent(double[] AttParametrArray)
        //{
        //    double ArrayLenght = AttParametrArray.Length;

        //    double PercentConstant = 75.00;


        //    double percentCount = 
        //}

        private double[] GetFileParametrFq(int FqIndex)
        {

            double[] AttParametrArray = new double[0];
            for (int i = 0; i< StaticParametr.AttMinMax_FieldMinMax_FileCompilation.Length;i++)
            {
                object[] PointParametrObject = (object[])StaticParametr.AttMinMax_FieldMinMax_FileCompilation[i];
                object[] FqParametrObject = (object[])PointParametrObject[FqIndex];
                int AttMaxParametr = (int)FqParametrObject[1];

                Array.Resize(ref AttParametrArray, AttParametrArray.Length + 1);
                AttParametrArray[AttParametrArray.Length - 1] = Convert.ToDouble(AttMaxParametr) / 10;
            }
            return AttParametrArray;
        }
        #endregion

    }
}
