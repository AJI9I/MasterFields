 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace MasterFields
{
    public partial class UCNewFileParametr : UserControl
    {
        #region Ошибка воода текс бокса
        private void TextBoxNoError(TextBox textbox)
        {
            textbox.ForeColor = Color.Black;
        }

        private void TextBoxEror(TextBox textbox)
        {
            textbox.ForeColor = Color.Red;
        }
        #endregion
  
        #region подключаемые классы
        FindAndCreateFolder findandcreatefolder;
        FqDiapason fqdiapason;
        #endregion

        #region Расчет диапазона частот
        private void button1_Click(object sender, EventArgs e)
        {
            //TestLabelBox();
            if (ChekedParametr())
            {
                if (FqDiapasonGet())
                {
                    //for (int i = 0; i < StaticParametr.FqStepArray.Length; i++)
                    //{
                    //    checkedListBox1.Items.Add(StaticParametr.FqStepArray[i]);
                    //}
                    if(StaticParametr.DoubleClicLabbelNoLeave)
                    {
                    NextFqParametrLabel();
                    }
                    ParametrFrequencuDisplay();
                }

            }
        }
        private bool FqDiapasonGet()
        {
            fqdiapason = new FqDiapason();
            return fqdiapason.WHStepsCalculation(StaticParametr.FqMax, StaticParametr.FqMin, StaticParametr.Step, StaticParametr.StepParametr);
        }

        #region Средства представления диапазона частот

        #region
        private void TestLabelBox()
        {
            Label LabelBox = new Label();
            LabelBox.Location = new Point(6, 19);
            LabelBox.Text = "Количество точек";
            
            Controls.Add(LabelBox);
            LabelBox.BringToFront();
        }
        #endregion

        #region лэйблы для надписей
        Label FqCountDisplay;
        Label FqCountDisplayText;
        Label[] fqStepsDisplay;
        #endregion

        private void ParametrFrequencuDisplay()
        {
            FqCountParametrDisplay();
            SrezMassive();

        }

        #region Вывод параметров частот для редактирования
        

        Label[] LabelFq = new Label[10];
        int posX = 360, posY = 65;
        private void SrezMassive()
        {
            if(LabelFq[0] != null)
            NextFqParametrLabel();
            int count = FqStepsParametrDisplay();
            for (int i=0; i< 10;i++)
            {
                int[] position = new int[2];

                position = MassivePosition(count, i);

                if(LabelFq[i] == null)
                { 
                LabelFq[i] = new Label();
                LabelFq[i].Location = new Point(posX, posY + i*20);
                LabelFq[i].Size = new Size(190,23);
                LabelFq[i].MouseEnter += LabelFqi_MouseEnter;
                LabelFq[i].MouseLeave += LabelGqi_MouseLeave;
                LabelFq[i].MouseDoubleClick += LabelFqi_MouseDoubleClick;
                Controls.Add(LabelFq[i]);
                LabelFq[i].BringToFront();
                }
                LabelFq[i].Name = position[0] + ":" + position[1];
                //LabelFq[i].Text = Convert.ToString(StaticParametr.FqStepArray[position[0]] / StaticParametr.DelitelKHz) + " ... " + Convert.ToString(StaticParametr.FqStepArray[position[1]] / StaticParametr.DelitelKHz) + " КГц";
                LabelFq[i].Text = Convert.ToString(StaticParametr.FqStepArray[position[0]]) + " ... " + Convert.ToString(StaticParametr.FqStepArray[position[1]]) + " МГц";
            }
        }

        private void LabelFqi_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Label lb = (Label)sender;
            if (StaticParametr.LabelFqName != lb.Name )
            { 
            StaticParametr.PositionMassive = null;
            StaticParametr.PositionMassive = new int[2];
            
            string[] positionS = lb.Name.Split(new Char[] { ':'}); 
            StaticParametr.PositionMassive[0] = Convert.ToInt32(positionS[0]);
            StaticParametr.PositionMassive[1] = Convert.ToInt32(positionS[1]);
            LABELRET:
            if (StaticParametr.DoubleClicLabbelNoLeave == false)
            {
                StaticParametr.DoubleClicLabbelNoLeave = true;
                StaticParametr.LabelFqName = lb.Name;
            }
            else
            {
                NextFqParametrLabel();
                goto LABELRET;
            }
            if(ChekListBoxFq != null)
            ChekListBoxFq.Items.Clear();
            OpenChecedListBox();
            ButtonChekedList();
            AddDeletedFqButton();
            }
        }

        Button ButtonChekedListPogasit;

        private void ButtonChekedList()
        {
            if (ButtonChekedListPogasit == null)
            {
                //↕
                ButtonChekedListPogasit = new Button();
                ButtonChekedListPogasit.Location = new Point(540,100);
                ButtonChekedListPogasit.Size = new Size(20, 100);
                ButtonChekedListPogasit.Text = "↕";
                Controls.Add(ButtonChekedListPogasit);
                ButtonChekedListPogasit.BringToFront();
            }
            ButtonChekedListPogasit.Click += ButtonChekedListPogasit_Click;
        }

        private void ButtonChekedListPogasit_Click(object sender, EventArgs e)
        {
            int[] elementsId = ReturnMinMaxId();
            for (int i = elementsId[0]; i < elementsId[1]; i++)
            {
                ChekListBoxFq.SetItemChecked(i, true);
            }
        }

        private int[] ReturnMinMaxId()
        {
            var cheked = ChekListBoxFq.CheckedIndices.Cast<int>();
            int[] elementsId = new int[] { cheked.Min(), cheked.Max() };
            return elementsId;
        }

        Button ButtonDeletedFq;

        private void AddDeletedFqButton()
        {
            if (ButtonDeletedFq == null)
            {
                ButtonDeletedFq = new Button();

                ButtonDeletedFq.Location = new Point(550,290);
                ButtonDeletedFq.Size = new Size(70,30);
                ButtonDeletedFq.Text = "Исключить";
                ButtonDeletedFq.Click += ButtonDeletedFq_Click;
                Controls.Add(ButtonDeletedFq);
                ButtonDeletedFq.BringToFront();
            }
        }

        private void ButtonDeletedFq_Click(object sender, EventArgs e)
        {
            var nummeratorFq = ChekListBoxFq.CheckedItems.GetEnumerator();
            for (int i = 0; i < ChekListBoxFq.CheckedItems.Count; i++)
            {
                nummeratorFq.MoveNext();
                double dd = Convert.ToDouble(nummeratorFq.Current);
                dd = dd * 1000;
                dd = Math.Round(dd, 2);
                StaticParametr.FqStepEnable[Array.IndexOf(StaticParametr.FqStepArray, dd)] = false;
            }
            
            ParametrFrequencuDisplay();
        }

        private void FqDeletedChekBox(double dd)
        {
            
        }

        CheckedListBox ChekListBoxFq;

        private void OpenChecedListBox()
        {
            if (ChekListBoxFq == null)
            {
                ChekListBoxFq = new CheckedListBox();

                ChekListBoxFq.Location = new Point(560,60);
                ChekListBoxFq.Size = new Size(90,200);
                Controls.Add(ChekListBoxFq);
                ChekListBoxFq.BringToFront();
            }
            for (int i = StaticParametr.PositionMassive[0]; i <= StaticParametr.PositionMassive[1]; i++)
            {
                ChekListBoxFq.Items.Add(StaticParametr.FqStepArray[i]);
            }

        }

        int PosXSmeshenie = 20;
        int posXSmeshenieTest = 10;
        private void NextFqParametrLabel()
        {
            foreach (Label ll in LabelFq)
            {
                if (ll.Name == StaticParametr.LabelFqName)
                {
                    ll.Location = new Point(ll.Location.X - PosXSmeshenie, ll.Location.Y);
                    StaticParametr.DoubleClicLabbelNoLeave = false;
                    StaticParametr.LabelFqName = null;
                    return;
                }
            }
        }

        private void LabelGqi_MouseLeave(object sender, EventArgs e)
        {
            Label lb = (Label)sender;
            if (StaticParametr.LabelFqName != lb.Name)
            {
            lb.Location = new Point(lb.Location.X + posXSmeshenieTest, lb.Location.Y);
            lb.Font = new Font(lb.Font.FontFamily, 8);
            }
        }

        private void LabelFqi_MouseEnter(object sender, EventArgs e)
        {
            Label lb = (Label)sender;
            if(StaticParametr.LabelFqName != lb.Name)
            lb.Location = new Point(lb.Location.X - posXSmeshenieTest, lb.Location.Y);
            lb.Font = new Font(lb.Font.FontFamily, 11); 
        }

        private int FqStepsParametrDisplay()
        {
            double count = Array.FindAll(StaticParametr.FqStepEnable, FindFlagsMassive).Count() / 10.0;
            int countInt = (int)count;
            double countD = count - countInt;
            if (countD != 0)
            {
                countInt = countInt + 1 ;
            }
            return countInt;
        }

        private bool FindFlagsMassive(bool bb)
        {
            if (bb == true)
            {
                return true;
            }
            else { return false; }
        }

        private int[] MassivePosition(int count, int i)
        {
            int[] positionMassive = new int[2];
            if (i == 0)
            {
                positionMassive[0] = 0;
                positionMassive[1] = count - 1;
                return positionMassive;
            }
            else 
            {
                    positionMassive[0] = (count) * i;
                if (positionMassive[0] + count - 1 < StaticParametr.FqStepArray.Count() - 1)
                {
                    positionMassive[1] = positionMassive[0] + count - 1;
                }
                else
                {
                    positionMassive[1] = StaticParametr.FqStepArray.Count() - 1;
                }
                return positionMassive;
            }
        }
        #endregion

        #region Отображение информации о количестве расчитанных частот
        private void FqCountParametrDisplay()
        {

            if (FqCountDisplay == null)
            {
                FqCountDisplay = new Label();
                FqCountDisplayText = new Label();
            }
            //if (!FqCountDisplay.Created)
            //{
            //    FqCountDisplay = new Label();
            //    FqCountDisplayText = new Label();
            //}
            FqCountDisplayText.Location = new Point(posX, 40);
            FqCountDisplay.Location = new Point(435, 40);
            var QueryTrue =
                from num in StaticParametr.FqStepEnable
                where num == true
                select num;
            FqCountDisplay.Text = Convert.ToString(QueryTrue.Count());
            FqCountDisplayText.Size = new Size(85,FqCountDisplayText.Size.Height);
            FqCountDisplayText.Text = "Точек частоты: ";
            
            Controls.Add(FqCountDisplay);
            Controls.Add(FqCountDisplayText);

            FqCountDisplay.BringToFront();
            FqCountDisplayText.BringToFront();
        }
        #endregion
        #endregion

        #endregion

        #region переменные срды
        // Булевы значения параметров на коррктность ввода
        bool BOOLtension, BOOLtime, BOOLfqmax, BOOLfqmin, BOOLstep, BOOLFileParametrName = false;

        //Значения введенных параметров
        //int[] tension, fq;
        //int time;
        //double step;
        //string filename;

        //Обьект содержащий введенные параметры
        //0- напряженность 
        //1 - Время
        //2 - Максимальная и Минимальная частота
        //3 - Значение шага
        object[,] parametr = new object[4,2];

        #endregion

        public UCNewFileParametr()
        {
            InitializeComponent();
            CalibrationFolder();
        }

        #region Создание папки
        //Создание папки для фалов калибровки
        private void CalibrationFolder()
        {
            CreateCalibrationFolder(StaticParametr.CalibrationFolderName);
        }

        //Проверка есть ли папка для сохранения параметров калибровки
        private void CreateCalibrationFolder(string directory)
        {
            findandcreatefolder = new FindAndCreateFolder();
            if (findandcreatefolder.findDirectory(findandcreatefolder.GetCurrentDirectory() + "//" + directory) == false)
                findandcreatefolder.opendirectiryCalibration(directory);

        }
        #endregion

        #region обработка параметров напряженности

        //Проверка валидности введенных данных ПРОВЕРИТЬ НЕ РАЗУ НЕ ПРОБЫВАЛ
        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
             ValidationTension();
        }

        

        private void ValidationTension()
        {
            if (TensionParametrArr() == true)
            {
                BOOLtension = true;
                TextBoxNoError(textBox1);
            }
            else
            {
                // вывод проверьте корректность ввода
                BOOLtension = false;
                TextBoxEror(textBox1);

            }
        }
        //Обработка введенных параметров напряженности
        private bool TensionParametrArr()
        {
            string tens = this.textBox1.Text;
            if (tens.Length != 0)
            {
                try
                {
                    double[] tension = Array.ConvertAll(tensionSplitter(tens), new Converter<string,double> (convdouble));
                    parametr[0,0] = (object)tension;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private double convdouble(string str)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            return Convert.ToDouble(str,provider);
        }

        private string[] tensionSplitter(string tension)
        {
            string[] f = tension.Replace(" ", string.Empty).Split(new Char[] { ',', ';' });
            return f;
        }
        #endregion

        #region обработка параметров времени
        private void textBox2_Validated(object sender, EventArgs e)
        {
            ValidationTime();
        }

        private void ValidationTime()
        {
            string time = textBox2.Text;
            if(time.Length !=0)
            { 
            if (TimeParametr(time))
            {
                //все ок параметр в обьекте
                BOOLtime = true;
                TextBoxNoError(textBox2);
                }
            else
            {
                //ошибка ввода проверить корректность
                BOOLtime = false;
                TextBoxEror(textBox2);
                }
            }
        }

        

        private bool TimeParametr(string time)
        {
            int timeInt = Convert.ToInt32(time.Replace(" ", string.Empty));
            if (timeInt > 0)
            {
                parametr[1,0] = timeInt;
                return true;
            }
            else
            { return false; }
        }

        
        #endregion

        #region обработка параметров максимальной и минимальной частоты
        private void textBox3_Validated(object sender, EventArgs e)
        {
            fqMax();
        }
        //Максимальная частота
        private void fqMax()
        {
            //Максимальная частота
            string fqMax = textBox3.Text;
            if (fqMax.Length != 0)
            {
                if (fqParametrEmptyValid(fqMax, 0))
                {
                    if (parametr[2, 1] != null)
                    {
                        FqMAxMinValidate();
                    }
                    BOOLfqmax = true;
                    TextBoxNoError(textBox3);
                    return;
                }
                else
                {
                    //ошибка максимальной частоты
                    BOOLfqmax = false;
                    TextBoxEror(textBox3);
                }
            }
            //Возврат ошибки
        }

        private void textBox4_Validated(object sender, EventArgs e)
        {
            fqMin();
        }

        

        //Минимальная частота
        private void fqMin()
        {
            //Минтмальная частота
            string fqMin = textBox4.Text;
            if(fqMin.Length != 0)
            {
                if (fqParametrEmptyValid(fqMin, 1))
                {
                    if (parametr[2, 0] != null)
                    {
                        FqMAxMinValidate();
                    }
                    BOOLfqmin = true;
                    TextBoxNoError(textBox4);
                    return;
                }
                else
                {
                    //Ошибка корретности введенной частоты
                    BOOLfqmin = false;
                    TextBoxEror(textBox4);
                }
            }
            //Возврат ошибки
        }

        //проверка параметров частот для обоих вариантов
        private bool fqParametrEmptyValid(string fq, int index)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            double fqDouble;
            fq = fq.Replace(" ", string.Empty);
            fqDouble = Convert.ToDouble(fq, provider);
            //fqDouble = fqDouble * 1000; 24/01/17 Расчет производился в Гц теперь все будет в МГц
            //fqDouble = fqDouble * 1000;
            if (fqDouble >= 0)
            {
                if (fqDouble >= 0)
                {
                    parametr[2, index] = fqDouble;
                    return true;
                }
                else
                {
                    parametr[2, index] = null;
                    return false;
                }
            }
            parametr[2, index] = null;
            return false;

        }

        private void FqMAxMinValidate()
        {
            if ((double)parametr[2, 0] - (double)parametr[2, 1] > 0)
            {
                //все ништяк
            }
            else
            {
                //ошибка
                BOOLfqmax = false;
                BOOLfqmin = false;
                TextBoxEror(textBox4);
                TextBoxEror(textBox3);
            }
        }

       

        #endregion

        #region обработка параметров шага
        private void textBox5_Validated(object sender, EventArgs e)
        {
            validationStep();
        }

        private void validationStep()
        {
            string step = textBox5.Text;
            if (step.Length != 0)
            {
                string steps = step.Replace(" ", string.Empty);
                if (StepDouble(steps))
                {
                    BOOLstep = true;
                    TextBoxNoError(textBox5);
                    //все прошло отлично
                }
                else
                {
                    BOOLstep = false;
                    TextBoxEror(textBox5);
                }
            }
        }

        private bool StepDouble(string step)
        {
            try
            {
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberGroupSeparator = ".";
                double stepdouble = Convert.ToDouble(step, provider);
                parametr[3, 0] = stepdouble;
                return true;
            }
            catch { return false; }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        #endregion

        #region Проверка имени файла


        private void textBox6_Validated(object sender, EventArgs e)
        {
            fileNameVerifed();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void fileNameVerifed()
        {
            string filename = textBox6.Text;
            if (filename.Length != 0)
            {
                findandcreatefolder = new FindAndCreateFolder();
                if (findandcreatefolder.findDirectory(StaticParametr.CalibrationFolderName + "//" + filename))
                {
                    //ошибка
                    BOOLFileParametrName = false;
                    TextBoxEror(textBox6);
                }
                else
                {
                    BOOLFileParametrName = true;
                    StaticParametr.FileParametrName = filename;
                    TextBoxNoError(textBox6);
                }

            }
            else
            {
                BOOLFileParametrName = false;
            }
        }


        private void findFileNameInCatalog()
        {

        }
        #endregion

        #region Сохрание параметров калибровки
        XMLNewParametrFile xmlnewparametrfile;
        private void button3_Click(object sender, EventArgs e)
        {
            if (ChekedParametr())
            { 
                //Все вроде должно быть проверенно 
                //Передаем параметры для создания нового файла с параметрами калибровки
                xmlnewparametrfile = new XMLNewParametrFile();
                xmlnewparametrfile.WHNewParametrFile(StaticParametr.FileParametrName, StaticParametr.FqMax, StaticParametr.FqMin, StaticParametr.Time, StaticParametr.Step, StaticParametr.StepParametr);
            }
        }

        private bool ChekedParametr()
        {
            //Проверка условия все ли параметры введены корректно
            if (BOOLfqmax && BOOLfqmin && BOOLstep && BOOLtension && BOOLtime && BOOLFileParametrName == true)
            {
                //сбор параметров с формы
                StaticParametr.TensionParametr = (double[])parametr[0, 0];
                StaticParametr.Time = (int)parametr[1, 0];
                StaticParametr.FqMax = (double)parametr[2, 0];
                StaticParametr.FqMin = (double)parametr[2, 1];
                StaticParametr.Step = (double)parametr[3, 0];
                StaticParametr.StepParametr = radioButton2.Checked;
                StaticParametr.FileParametrName = textBox6.Text;
                return true;
            }
            else
            {
                return false;
                //Вывод сообщения для ввода корректных параметров
            }
        }
        #endregion

        #region выставляем флаги о проверки значений
        private void button2_Click(object sender, EventArgs e)
        {
            BOOLfqmax = BOOLfqmin = BOOLstep = BOOLtension = BOOLtime == true;
        }
        #endregion
    }
}
