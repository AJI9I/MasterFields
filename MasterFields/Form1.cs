using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;

namespace MasterFields
{
    delegate void ClearUcForm(UserControl usercontrol);
    delegate void AddUcForm(UserControl usercontrol);

    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
            clearusercontrolform = new ClearUcForm(ClearUCForm);
            adducform = new AddUcForm(AddUCForm);
            StartPageAdd();
            StartProgrammButton();
            UpdatePOrtName();
        }
        

        #region Вайл цикл длля опроса работы кнопок
        Thread WhieleThreadStart;
        private void ThreadWhieleButton()
        {
            WhieleThreadStart = new Thread(WhieleButton);
            WhieleThreadStart.Start();
        }

        private void WhieleButton()
        {
            wbd = new WBD(WhieleButtonDelegate);
            while (StaticParametr.FileLoadWhiele)
            {
                Thread.Sleep(500);
                if (StaticParametr.FileLoad)
                {
                    Invoke(wbd);
                    StaticParametr.FileLoadWhiele = false;
                }
                if (button5.Enabled == true && button4.Enabled == true)
                {
                    StaticParametr.FileLoadWhiele = false;
                }
            }
        }

        public delegate void WBD();
        WBD wbd;

        private void WhieleButtonDelegate()
        {
            button5.Enabled = true;
            button4.Enabled = true;
        }


        #endregion

        UCStartPage ucstartpage;
        private void StartPageAdd()
        {
            if(ucstartpage == null)
                ucstartpage = new UCStartPage();

            ucstartpage.Location = new Point(StaticFormUserControl.LocationX, StaticFormUserControl.LocationY);
            ucstartpage.Size = new Size(10, StaticFormUserControl.UCCalibrationPointHeight);
            Controls.Add(ucstartpage);
            AddUCFormPanel(ucstartpage);
        }
        
        
        #region
        private void button3_Click(object sender, EventArgs e)
        {
            ClearButtonVisible();

            AddUCNewFileParametr();
            StaticFormUserControl.UCNewFileParametrVisible = true;
        }

        #region Процесс калибровки
        UCProcessCalibration ucprocesscalibration;
        private void button6_Click(object sender, EventArgs e)
        {
            uccalibrationpoint.XMLCalibrationPointAdd();
            ClearButtonVisible();
            //  сделал столько статик визибл в и х надо дописать в слайдер
            if (ucprocesscalibration == null)
                ucprocesscalibration = new UCProcessCalibration();
            //if (!ucprocesscalibration.Created)
            //    ucprocesscalibration = new UCProcessCalibration();
            ucprocesscalibration.Location = new Point(StaticFormUserControl.LocationX, StaticFormUserControl.LocationY);
            ucprocesscalibration.Size = new Size(10,StaticFormUserControl.UCCalibrationPointHeight);
            Controls.Add(ucprocesscalibration);
            AddUCFormPanel(ucprocesscalibration);
        }
        #endregion

        #region Редактирование частот на исследование
        UCEditFqDiapazon uceditfqdiapazon;
        private void button5_Click(object sender, EventArgs e)
        {
            ClearButtonVisible();

            if (uceditfqdiapazon == null)
                uceditfqdiapazon = new UCEditFqDiapazon();
            if (!uceditfqdiapazon.Created)
                uceditfqdiapazon = new UCEditFqDiapazon();
            uceditfqdiapazon.Location = new Point(StaticFormUserControl.LocationX, StaticFormUserControl.LocationY);
            uceditfqdiapazon.Size = new Size(10, StaticFormUserControl.UCEditFqDiapazonHeight);
            Controls.Add(uceditfqdiapazon);
            AddUCFormPanel(uceditfqdiapazon);

        }
        #endregion

        #region Новый фаил параметров
        UCNewFileParametr ucnewfileparametr;
        private void AddUCNewFileParametr()
        {
            if (ucnewfileparametr == null)
                ucnewfileparametr = new UCNewFileParametr();
            if (!ucnewfileparametr.Created)
                ucnewfileparametr = new UCNewFileParametr();
            ucnewfileparametr.Location = new Point(StaticFormUserControl.LocationX, StaticFormUserControl.LocationY);
            ucnewfileparametr.Size = new Size(10,StaticFormUserControl.UCNewFileParametrHeight);
            Controls.Add(ucnewfileparametr);
            AddUCFormPanel(ucnewfileparametr);
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            AddUCCalibrationPoint();
            //Добавление кнопки начала калибровки на панель
            AddCalibrationButton();

        }

        #region Добавление кнопки калибровка в меню выбора точки расположения антенны
        private void AddCalibrationButton()
        {
            button6.Location = new Point(125, 400);
            button6.Size = new Size(241, 47);
            button6.Enabled = true;
            button6.BringToFront();

            //Информация о том видна ли кнопка или нет
            StaticFormUserControl.Button6Visible = true;

        }
        #endregion

        #region Удаление лишних кнопок с поля видимости
        private void ClearButtonVisible()
        {
            if (StaticFormUserControl.Button6Visible == true)
                button6.Location = new Point(StaticFormUserControl.ButtonNoVisibleLocation, button6.Location.Y);

            #region Добавление формы испытания на рабочую панель
            #endregion
        }
        #endregion

        #region Добавление точек калибровки
        UCCalibrationPoint uccalibrationpoint;
        private void AddUCCalibrationPoint()
        {
            if(uccalibrationpoint == null)
                uccalibrationpoint = new UCCalibrationPoint();
           if (!uccalibrationpoint.Created)
                uccalibrationpoint = new UCCalibrationPoint();
            uccalibrationpoint.Location = new Point(StaticFormUserControl.LocationX, StaticFormUserControl.LocationY);
            uccalibrationpoint.Size = new Size(10, StaticFormUserControl.UCCalibrationPointHeight);
            Controls.Add(uccalibrationpoint);
            AddUCFormPanel(uccalibrationpoint);
        }
        #endregion
        #endregion

        #region добавление рабочего контрола на форму
        Thread ThreadAddUCForm;

        private void AddUCFormPanel(UserControl ucontrol)
        {
            ClearUCFormPanel();
            MowementAddThreadUCForm(ucontrol);
        }

        private void MowementAddThreadUCForm(UserControl ucontrol)
        {
            ThreadAddUCForm = new Thread(MowementAddUCForm);
            ThreadAddUCForm.IsBackground = true;
            ThreadAddUCForm.Start(ucontrol);

        }
        //Делегат передачи управления
        AddUcForm adducform;
        private void MowementAddUCForm(object ucontrol)
        {
            UserControl control = (UserControl)ucontrol;
            while (UCVisibleAddForm(control))
            {
                Thread.Sleep(25);

                Invoke(adducform, ucontrol);
            }
        }

        private void AddUCForm(UserControl ucontrol)
        {
           //ucontrol.Location = new Point(ucontrol.Location.X + 10, ucontrol.Location.Y);
           ucontrol.Size = new Size(ucontrol.Size.Width + 30, ucontrol.Size.Height);
        }

        private bool UCVisibleAddForm(UserControl ucontrol)
        {
            #region  UCNewFileParametr Новые параметры файла
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCNewFileParametr")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width >= StaticFormUserControl.UCNewFileParametrWidth)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCNewFileParametrVisible = true;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
             
            #endregion

            #region UCCalibrationPoint Выбор точки калибровки
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCCalibrationPoint")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width >= StaticFormUserControl.UCCalibrationPointWidth)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCCalibrationPointVisible = true;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
            #endregion

            #region  UCEditFqdiapazon  Редактор диапазона частот
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCEditFqDiapazon")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width >= StaticFormUserControl.UCEditFqDiapazonWidth)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCEditFqDiapazonVisible = true;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
            #endregion

            #region  UCProcessCalibration  режим калибровки
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCProcessCalibration")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width >= StaticFormUserControl.UCProcessCalibrationWidth)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCProcessCalibrationVisible = true;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
            #endregion

            #region  UCStartPage  стартовая страница
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCStartPage")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width >= StaticFormUserControl.UCStartPageWidth)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCStartPageVisible = true;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
            #endregion

            #region  UCStartPage  стартовая страница
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCStudyProcess")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width >= StaticFormUserControl.UCStudyProcessWidth)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCStudyProcessVisible = true;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
            #endregion

            return true;
        }

        #endregion

        #region регион отчиска формы от лишнего мусора отработанной формы либо скрытие формы

        #region Потоки для отчистки формы
        Thread ThreadClearUCForm;
        #endregion

        //ГЛАВНАЯ ФУНКЦИЯ РЕГИОНА  запускается перед вызовом новой формы для удаления из виду старой
        private void ClearUCFormPanel()
        {
            if (StaticFormUserControl.UCNewFileParametrVisible)
                MowementClearThreadUCForm(ucnewfileparametr);
            if (StaticFormUserControl.UCCalibrationPointVisible)
                MowementClearThreadUCForm(uccalibrationpoint);
            if (StaticFormUserControl.UCEditFqDiapazonVisible)
                MowementClearThreadUCForm(uceditfqdiapazon);
            if (StaticFormUserControl.UCProcessCalibrationVisible)
                MowementClearThreadUCForm(ucprocesscalibration);
            if(StaticFormUserControl.UCStartPageVisible)
                MowementClearThreadUCForm(ucstartpage);
            if (StaticFormUserControl.UCStudyProcessVisible)
                MowementClearThreadUCForm(ucstudyprocess);
        }

        // создание потока для удаление нужного контрола 
        // передается нужный контрол
        private void MowementClearThreadUCForm(UserControl ucontrol)
        {
            ThreadClearUCForm = new Thread(MowementUCForm);
            ThreadClearUCForm.IsBackground = true;
            ThreadClearUCForm.Start(ucontrol);

        }

        // делегат передачи управления на уменьшение размера рабочей области
        ClearUcForm clearusercontrolform;

        // Функция с потоком удаляемой рабочей области
        private void MowementUCForm(object ucontrol)
        {
            UserControl control = (UserControl)ucontrol;
            while (UCVisibleBool(control))
            {
                Thread.Sleep(25);
                Invoke(clearusercontrolform, ucontrol);
            }
        }

        // Идет проверка достигла ли удаляемая форма нуля
        // если форма достигла нуля то выставляются флаги остановки бесконечного цикла вайл
        // и переменная что форма уже не существует.
        // в функцию передается обьекс UserControl  который в данный момент удаляется
        private bool UCVisibleBool(UserControl ucontrol)
        {
            // проверка имени формы которая удаляется если пролетает, то
            if(ucontrol.Name == "UCNewFileParametr")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width <= 0)
                { 
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCNewFileParametrVisible = false;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }

            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCCalibrationPoint")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width <= 0)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCCalibrationPointVisible = false;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }

            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCEditFqDiapazon")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width <= 0)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCEditFqDiapazonVisible = false;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }

            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCProcessCalibration")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width <= 0)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCProcessCalibrationVisible = false;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }
            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCStartPage")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width <= 0)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCStartPageVisible = false;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }

            // проверка имени формы которая удаляется если пролетает, то
            if (ucontrol.Name == "UCStudyProcess")
            {
                // Проверяется ее ширина на данный  момент , если ширина достигается нуля или стает меньше, то
                if (ucontrol.Width <= 0)
                {
                    // Выставляется флаг отсутствия формы на рабочей области
                    StaticFormUserControl.UCStudyProcessVisible = false;
                    // Происходит возврат булева сообщения о том что пора прекратить выполнения цикла while на очистку формы
                    return false;
                }
            }

            return true;
        }
        // функция уменьшения размеров формы        
        private void ClearUCForm(UserControl ucontrol)
        {
            ucontrol.Location = new Point(ucontrol.Location.X + 30, ucontrol.Location.Y);
            ucontrol.Size = new Size(ucontrol.Size.Width - 30, ucontrol.Size.Height);
        }


        #endregion

        

        private void button2_Click(object sender, EventArgs e)
        {
            AddUCFormPanel(uccalibrationpoint);
        }

        #region Работа панели навигации

        ////Фаил параметров
        //button3
        ////Исследуемые частоты
        //button5
        ////Точка испытания
        //button4
        ////Процесс калибровки
        //button6

        private void StartProgrammButton()
        {
            button7.Location = new Point(12,85);
            button8.Location = new Point(12, 130);
            button3.Location = new Point(-300, 85);
            button5.Location = new Point(-300, 130);
            button4.Location = new Point(-300, 175);
            button6.Location = new Point(-300, 220);
        }


        private void button7_Click(object sender, EventArgs e)
        {
            ClearButtonVisible();

            button7.Location = new Point(-300, 85);
            button8.Location = new Point(-300, 125);
            button3.Location = new Point(12,85);
            button5.Location = new Point(12,130);
            button4.Location = new Point(12, 175);
            //button6.Location = new Point(12, 220);
            VerificateButtonCalobration();
        }

        private void VerificateButtonCalobration()
        {
            if (StaticParametr.FileLoad)
            {
                button5.Enabled = true;
                button4.Enabled = true;
                button6.Enabled = false;
            }
            else
            {
                button5.Enabled = false;
                button4.Enabled = false;
                button6.Enabled = false;
                //если не чего не было загруженно запускается ожидающий поток
                ThreadWhieleButton();
            }
        }

        #endregion

        #region Режим испытания
        private void button8_Click(object sender, EventArgs e)
        {
            AddUCStudyProcess();
            //ClearButtonVisible();
        }

        UCStudyProcess ucstudyprocess;
        private void AddUCStudyProcess()
        {
            if (ucstudyprocess == null)
                ucstudyprocess = new UCStudyProcess();
            if (!ucstudyprocess.Created)
                ucstudyprocess = new UCStudyProcess();
            ucstudyprocess.Location = new Point(StaticFormUserControl.LocationX, StaticFormUserControl.LocationY);
            ucstudyprocess.Size = new Size(10, StaticFormUserControl.UCStudyProcessHeight);
            this.Size = new Size(960, 825);
            Controls.Add(ucstudyprocess);
            AddUCFormPanel(ucstudyprocess);
        }

        #endregion

        #region Определение портов 
        private void UpdatePOrtName()
        {
            string[] portName = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox1.Items.AddRange(portName);
            comboBox2.Items.AddRange(portName);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            StaticParametr.GENPortName = comboBox1.SelectedItem.ToString();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            StaticParametr.PRIPortName = comboBox2.SelectedItem.ToString();
        }
        #endregion

        #region Слепой поиск оборудования
        private void button1_Click(object sender, EventArgs e)
        {
            //ClearUCFormPanel();
            PortSkanerStart();
        }

        GENPort genport;
        GENCommand gencommand;

        private void PortSkanerStart()
        {
            genport = new GENPort();
            gencommand = new GENCommand();

            priport = new PRIPort();
            string[] portName = SerialPort.GetPortNames();

            bool generator = false;
            bool Izmeritel = false;

            for (int i = 0; i < portName.Length;i++)
            {
                if (!generator)
                {
                    if (GeneratorFind(portName[i]))
                    {
                        generator = true;
                        StaticParametr.GENPortName = portName[i];
                        textBox1.Text = portName[i];
                        textBox1.ReadOnly = true;
                    }
                }
                if (!Izmeritel)
                {
                    if (IzmeritelFind(portName[i]))
                    {
                        Izmeritel = true;
                        StaticParametr.PRIPortName = portName[i];
                        textBox2.Text = portName[i];
                        textBox2.ReadOnly = true;
                    }
                }
            }
        }

        #region Определение генератора
        private bool GeneratorFind(string portName)
        {
            StaticParametr.GENPortName = portName;
            //Отправка запроса на авторизацию
            string command = genport.Generator(gencommand.GeneratorIdentification());
            //LogerPanel(command, "<g<");
            //Чтение порта
            genport.PortReadWhile();
            //Полученное сообщение после отправки команды авторизации
            //LogerPanel(String.Join("", StaticParametr.GENPortReadMessage), ">g>");
            //Проверка условия на приход ответа
            if (StaticParametr.GENPortReadMessage[0] == StaticParametr.GENIdentification)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Определение измерителя

        PRIPort priport;
        private bool IzmeritelFind(string portName)
        {
            StaticParametr.PRIPortName = portName;
            priport.ReadSetupDevice();
            //После прочтения пакета с параметрами измерителя 
            //выставляется флаг авторизации измерителя
            //StaticParametr.AutorisationPZ в положение true
            int i = 0;
            //Флаг выхода из замкнутого цикла, больше не где не используется
            bool IzmeritelOff = true;

            while (IzmeritelOff)
            {

                Thread.Sleep(100);

                if (StaticParametr.AutorisationPZ)
                {
                    IzmeritelOff = false;
                    return true;
                }
                i++;
            }
            return false;
        }
        #endregion

        #endregion

        #region Сервисная кнопка генератор последовательности XML
        ServiceGeneratorXML servicegeneratorxml;
        private void button11_Click(object sender, EventArgs e)
        {
            servicegeneratorxml = new ServiceGeneratorXML();
            servicegeneratorxml.GenerateXMLFile();
        }
        #endregion
    }
}
