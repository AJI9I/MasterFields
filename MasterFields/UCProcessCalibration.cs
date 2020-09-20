using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MasterFields
{
    public partial class UCProcessCalibration : UserControl
    {
        public UCProcessCalibration()
        {
            InitializeComponent();
            //Отладочная форма
           //ControlFormThreadFunction();
            //
            StaticParametrFormUpdate();
            ParametrFirmUpdate();
            pbsp1 = new PBPS1(ProgressBarConveer);
            pbps2 = new PBPS2(ProgressBarTensionConveer);
            BWCalibrationProcess();
        }

        

        #region Вспомогательные формы
        AlertAntenna alertantenna;
        #endregion

        #region Обработка команд монитора
        private void LogerPanel(string msg, string pribor)
        {
            string[] command = new string[] { msg, pribor };
            MsgLog(command);
        }
        public delegate void MSGLOG(string msg);
        MSGLOG msglog;
        private void MsgLog(string[] msg)
        {
            MessageWriteBox(msg);
        }
        private void MessageWriteBox(string[] msg)
        {
            msglog = new MSGLOG(Write);
            

            if (msg[1] == "<g<")
            {
                Invoke(msglog, "<---g---< -   " + msg[0]);
            }
            if (msg[1] == ">g>")
            {
                Invoke(msglog, ">---g---> -   " + msg[0]);
            }
            if (msg[1] == "<p<")
            {
                Invoke(msglog, "<---p---< -   " + msg[0]);
            }
            if (msg[1] == ">p>")
            {
                Invoke(msglog, ">---p---> -   " + msg[0]);
            }
            if (msg[1] == "compare")
            {
                Invoke(msglog, ">compare> -   " + msg[0]);
            }
        }
        private void Write(string  msg)
        {
            textBox1.AppendText(msg + Environment.NewLine);
        }
        #endregion

        #region Делегат и функция для записи лог файла
        LogFile logfile;
        public delegate void LOG( string msg);
        LOG log;
        private void LogDelegate(string msg)
        {
            log = new LOG(Log);
            Invoke(log, msg);
        }
        private void Log(string msg)
        {
            logfile = new LogFile();
            logfile.LogWrite(msg);
        }
        #endregion

        #region Заполнение параметров формы
        private void StaticParametrFormUpdate()
        {
            textBox2.Text = StaticParametr.FileParametrName;
            textBox3.Text = StaticParametr.PointName;
            textBox4.Text = StaticParametr.PolarisationName;

        }

        private void ParametrFirmUpdate()
        {
            //foreach (var tt in StaticParametr.TensionParametr)
            //{
            //    label12.Text = label12.Text + " "+ tt;
            //}
            //label13.Text = " " + Convert.ToString(StaticParametr.Time) + " c";
            //label14.Text = " " + Convert.ToString(StaticParametr.Step);
            //label9.Text = " " + Convert.ToString(StaticParametr.FqMax) + " MГц";
            //label10.Text = " " + Convert.ToString(StaticParametr.FqMin) + " МГц";
        }

        #region Заполнение параметров формы 03.05.17
        #region Обновление частоты на форме калибровки
        public delegate void FQFORMUPDATE(double fq);
        FQFORMUPDATE fqformupdate;
        private void FqFormUpdate(double fq)
        {
            fqformupdate = new FQFORMUPDATE(FqUpdate);
            Invoke(fqformupdate, fq);
        }

        private void FqUpdate(double fq)
        {
            textBox6.Text = fq + " МГц";
        }
        #endregion

        #region Обновление напряженности на форме калибровки
        public delegate void TNFORMUPDATE(double tn);
        TNFORMUPDATE tnformupdate;
        private void TnFormUpdate(double tn)
        {
            tnformupdate = new TNFORMUPDATE(TnUpdate);
            Invoke(tnformupdate, tn);
        }

        private void TnUpdate(double tn)
        {
            textBox7.Text = tn + " В/м^2";
        }
        #endregion

        #region Обновление времени выдержки на форме калибровки
        public delegate void TMFORMUPDATE();
        TMFORMUPDATE tmformupdate;
        private void TmFormUpdate()
        {
            tmformupdate = new TMFORMUPDATE(TmUpdate);
            Invoke(tmformupdate);
        }

        private void TmUpdate()
        {
            textBox8.Text = StaticParametr.Time + " c";
        }

        #region Событие редактирования текст бокса со временем
        #region Событие двойного щелчка мышью
        string lactTb8;
        private void textBox8_DoubleClick(object sender, EventArgs e)
        {
            if (textBox8.ReadOnly == false)
            {
                textBox8.Text = lactTb8;
                textBox8.ReadOnly = true;
            }
            else
            {
                lactTb8 = textBox8.Text;
                textBox8.ReadOnly = false;
            }
        }
        #endregion

        #region Событие нажатия клавиши энтер
        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == '\r')
            {

                try
                {
                    string t = textBox8.Text;
                    int time = Convert.ToInt32(t);

                    lactTb8 = textBox8.Text;
                    StaticParametr.Time = time;
                    textBox8.ReadOnly = true;
                }
                catch
                {
                    textBox8.Text = lactTb8;
                    textBox8.ReadOnly = true;
                }

            }

        }
        #endregion
        #endregion

        #endregion

        #region Обвновление антенны на форме
        public delegate void ANFORMUPDATE();
        ANFORMUPDATE anformupdate;
        private void AnFormUpdate()
        {
            anformupdate = new ANFORMUPDATE(AnUpdate);
            Invoke(anformupdate);
        }

        private void AnUpdate()
        {
            if (StaticParametr.PriborParametrSetups[1] != null)
            {
                int DiapasonAntennIndex = Array.FindIndex(StaticParametr.AntennNameArray, p => p == Convert.ToInt32(StaticParametr.PriborParametrSetups[1]));
                textBox5.Text = StaticParametr.AntennNameArrayString[DiapasonAntennIndex];
            }
            else
            {
                textBox5.Text = "???????";
            }
            
        }
        #endregion

        #endregion
        #endregion

        #region Поток для отслеживания калибровки
        BackgroundWorker BackgroudWorcerCalibrationProcess = new BackgroundWorker();
        private void BWCalibrationProcess()
        {
            BackgroudWorcerCalibrationProcess.WorkerReportsProgress = true;
            BackgroudWorcerCalibrationProcess.WorkerSupportsCancellation = true;
            BackgroudWorcerCalibrationProcess.DoWork += new DoWorkEventHandler(BWCalibrationProcess);
            BackgroudWorcerCalibrationProcess.ProgressChanged += new ProgressChangedEventHandler(BWCalibrationProgressProcess);
            BackgroudWorcerCalibrationProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BWCalibrationCompletedtProcess);
            BackgroudWorcerCalibrationProcess.RunWorkerAsync();
        }

        #region Сервисные функции подпроцесса калибровки такие как завершение процесса калибровки и отслеживания прогресса
        private void BWCalibrationCompletedtProcess(object sender, EventArgs e)
        {
            
        }
        private void BWCalibrationProgressProcess(object sender, EventArgs e)
        {

        }
        #endregion

        #region Идентификация измерителя и генератора
        private void PriborIdentification()
        {
            //Перед началом калибровки проверяется подключение устройств
            GeneratorIdentification();
            IzmeritelIdentification();

            #region Сервисные сообщения о генераторе и приемнике
            GeneratorInformationMessage();
            PriemnikInformationMessge();
            #endregion

            if (!StaticParametr.GenIdentificationTru || !StaticParametr.AutorisationPZ)
            {
                while (true)
                {
                    LogerPanel("Программа ушла в спячку, т.к. один из приборов не подключен, подключите приборы и перезапустите режим калибровки", ">g>");
                    Thread.Sleep(99999);
                }
            }
        }

        #region Идентификация генератора
        private void GeneratorIdentification()
        {

            //Отправка запроса на авторизацию
            string command = genport.Generator(gencommand.GeneratorIdentification());
            LogerPanel(command, "<g<");
            //Чтение порта
            genport.PortReadWhile();
            //Полученное сообщение после отправки команды авторизации
            LogerPanel(String.Join("", StaticParametr.GENPortReadMessage), ">g>");
            //Проверка условия на приход ответа
            if (StaticParametr.GENPortReadMessage[0] == StaticParametr.GENIdentification)
            {
                StaticParametr.GenIdentificationTru = true;
            }
            else
            {
                StaticParametr.GenIdentificationTru = false;
            }
        }
        #endregion

        #region Идентификация измерителя
        private void IzmeritelIdentification()
        {
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
                LogerOtladchk();
                if (StaticParametr.AutorisationPZ)
                {
                    IzmeritelOff = false;
                }
                i++;
            }
        }

        private void LogerOtladchk()
        {
            if (StaticParametr.MessagaOtladchik != "")
            {
                LogerPanel(StaticParametr.MessagaOtladchik, ">p>");
                StaticParametr.MessagaOtladchik = "";
            }
            
        }
        #endregion
        #endregion

        #region Вывод сообщений о автонризации приемника и генератора

        #region Вывод сообщения об авторизации генератора
        private void GeneratorInformationMessage()
        {
            string command = "";

            if (StaticParametr.GenIdentificationTru)
            {
                command = "Генератор подключен";
            }
            else
            {
                command = "Ошибка подключения генератора";
            }

            LogerPanel(command, ">g>");
        }
        #endregion

        #region Вывод сообщения об авторизации приемника с параметрами
        private void PriemnikInformationMessge()
        {
            string command = "";

            if (StaticParametr.AutorisationPZ)
            {
                command = "Измеритель ПЗ-41 подключен";
            }
            else
            {
                command = "Ошибка подключения измерителя ПЗ-41";
            }

            LogerPanel(command, ">p>");

            if (StaticParametr.AutorisationPZ)
            {
                PriemnikInformationParametrMessage();
            }
        }

        private void PriemnikInformationParametrMessage()
        {

            string[] LabelDesription = new string[] {"Количество байт в пакете",
                    "Индекс номера антенны",
                    "Рабочая частота",
                    "Поправочный коэфициент",
                    "Максимальнео-допустимое значение напряженности электрического поля",
                    "Максимально-допустимое значение напряженности магнитного поля",
                    "Максимольно-допустимое значение плотности потока электромагнитного поля",
                    "Максимально-допустимое значение экспозиции электрического поля",
                    "Максимально-допустимое значение экспозиции магнитного поля",
                    "Максимально допустимое значение экспозиции потока энергии электромагнитного поля",
                    "Параметр прибора",
                    "Параметр прибора"};

            for (int i = 0; i < StaticParametr.PriborParametrSetups.Length; i++)
            {
                LogerPanel(LabelDesription[i] + " : " + StaticParametr.PriborParametrSetups[i], ">p>");
            }
        }
        #endregion

        #endregion

        private void BWCalibrationProcess(object sender, EventArgs e)
        {
            #region Делегат для обновления прогресс бара
            //Делегат обновления прогресс бара
            Invoke(pbsp1, true);
            #endregion

            #region Вывод полученной ранее информации
            DelegateParametrListWiewLastParametr();
            #endregion

            #region Идентификация приборов
            PriborIdentification();
            #endregion

            #region Обновление информации на форме об антенне
            AnFormUpdate();
            #endregion

            #region Подготовка генератора к работе
            //Сброс всех настроек генератора и чтение ответа если таковой есть
            GeneratorRST(); genport.PortReadWhile();
            //Установка нужного блока и чтение ответа после проделанной операции
            GeneratorBlockSetup(); genport.PortReadWhile();
            LogerPanel("Предварительная настройка генератора произведенна, переход к режиму калибровки", ">g>");
            #endregion

            #region Создаем массив для записи точек аттенюатора, по умолчанию заполняется 9999 для проверки
            StaticParametr.TensionAttenuationFindedPoint = new int[StaticParametr.TensionParametr.Length];
            for (int c = 0; c < StaticParametr.TensionAttenuationFindedPoint.Length;c++)
            {
                StaticParametr.TensionAttenuationFindedPoint[c] = 9999;
            }
            #endregion

            //Начинаем процесс калибровки получением частоты для калибровки поля
            for (int i = StaticParametr.FqStepArrayI; i < StaticParametr.FqStepArray.Length; i++)
            {
                
                #region Статические переменные частоы и ее порядкового номера из мессива частот
                StaticParametr.PointFqDefolt = StaticParametr.FqStepArray[i];
                StaticParametr.PointFqDefoltNumber = i;
                #endregion

                #region Отправка сигнала на обновление формы частоты
                FqFormUpdate(StaticParametr.PointFqDefolt);
                #endregion

                //Производим проверку нужна ли нам эта частота
                if (StaticParametr.FqStepEnable[i] == true)
                {
                    #region Проверка на установленную антенну
                    ANTENNA:
                    int DiapasonAntennIndex = Array.FindIndex(StaticParametr.AntennNameArray, p => p == Convert.ToInt32(StaticParametr.PriborParametrSetups[1]));
                    if (StaticParametr.AntennDiapasonFq[DiapasonAntennIndex, 0] > StaticParametr.PointFqDefolt && StaticParametr.AntennDiapasonFq[DiapasonAntennIndex, 1] < StaticParametr.PointFqDefolt)
                    {
                        LogerPanel("Переход к диалогу смены антенны", ">p>");
                        AntennaFormDialog(DiapasonAntennIndex);
                        LogerPanel("Переход к проверке частоты и установленной антенне приемника", ">p>");
                        goto ANTENNA;
                    }
                    #endregion

                    #region Обновление информации на форме об антенне
                    AnFormUpdate();
                    #endregion

                    #region Создание блока для окалиброванных точек напряженности на определенной частоте
                    xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
                    xmlnewcalibrationpoint.XMLCalibrationFqPointWrite();
                    #endregion

                    for (int b = StaticParametr.TensionParametrI; b< StaticParametr.TensionParametr.Length; b++)
                    {

                        #region Параметры напряженности поля, которо необходимо найти
                        StaticParametr.PointTensionDefolt = StaticParametr.TensionParametr[b];
                        StaticParametr.PointTensionDefoltNumber = b;
                        #endregion

                        #region Обновление напряженности на форме
                        TnFormUpdate(StaticParametr.PointTensionDefolt);
                        #endregion

                        #region Обновление времени выдержки на форме
                        TmFormUpdate();
                        #endregion

                        #region Установка параметоров аттенюатора для найденных приближенных значений напряженности поля
                        if (StaticParametr.TensionAttenuationFindedPoint[StaticParametr.PointTensionDefoltNumber] != 9999)
                        StaticParametr.GeneratorAttSet = StaticParametr.TensionAttenuationFindedPoint[StaticParametr.PointTensionDefoltNumber];
                        #endregion

                        #region Параметр частоты, для установки в генератор
                        StaticParametr.GeneratorFqSet = StaticParametr.FqStepArray[i];
                        #endregion

                        #region Обнуление параметров, максимальной и минимальных значений напряженности и установок аттенюатора
                        StaticParametr.PointTensMax = 0;
                        StaticParametr.PointTensMin = 0;
                        StaticParametr.PointAttMax = 0;
                        StaticParametr.PointAttMin = 0;
                        StaticParametr.PointAttLasMax = 0;
                        StaticParametr.PointAttLastMin = 0;
                        #endregion

                        #region Предварительная установка частоты на устройствах
                        //Предварительная настройка генератора и измерителя на исследуемую частоту
                        PreviousCalibrationParametrSetup();
                        #endregion

                        #region Процесс калибровки непосредственно
                        Calibraton();
                        #endregion

                        #region Произвожу вывод пользователю на экран найденных значений, перед переходй к поиску следующего значения напряженности
                        //Вывод резуклитатов откалиброванной позиции
                        DelegateParametrListWiew();
                        #endregion      
                    }
                }
                //Обновление прогресс бара о смене частоты
                Invoke(pbsp1, false);
            }
            #region Завершение калибровки точки, вызов диалога остановки калибровки
            CalibrationCancellAlert();
            #endregion
        }

        #region Предварительная установка частоты на устройствах (Актуальная версия) 05.04.17
        private void PreviousCalibrationParametrSetup()
        {
            #region Установка частоты на генераторе, ожидание ответа, вывод информации в лог фаил
            string comandfq = genport.Generator(gencommand.GeneratorSetFq(StaticParametr.GeneratorFqSet));
            //Отладочная информация
            genport.PortReadWhile();
            //Отладочная информация
            LogerPanel("Установка частоты на генераторе - "+comandfq, "<g<");
            #endregion

            

            #region Установка параметров частоты на приемнике
            priport.PackageUpdateMesuremetnter(StaticParametr.GeneratorFqSet);
            LogerPanel("Установка исследуемой частоты на приемнике ", "<p<");
            #endregion

            #region Проверка установленных значений приемника
            IzmeritelIdentification();
            LogerPanel("Проверка установленной частоты на приемнике", "<p<");
            PriemnikInformationMessge();
            #endregion

        }
        #endregion

        #region Установка параметров Аттенюатора и времени выдержки перед запуском генератора и считыванием напряженности поля
        private void PreviousSetupAttenuationAndTimeDelay()
        {
            #region Установка параметров аттенюатора генератора
            string comandatt = genport.Generator(gencommand.GeneratorSetAtt(StaticParametr.GeneratorAttSet));
            //Отладочная информация
            genport.PortReadWhile();
            //Отладочная информация
            LogerPanel("Установка параметров аттенюатора - " + comandatt, "<g<");
            #endregion

            #region Установка параметров времени выдержки
            int GenTime = StaticParametr.Time * 10;
            string commandtime = genport.Generator(gencommand.GeneratorTimeDelay(GenTime));
            genport.PortReadWhile();
            LogerPanel(commandtime, "<g<");
            #endregion
        }
        #endregion

        #region Непосредственно сам процесс калибровки (Актуальная версия) 05.04.17
        bool CALIBRATION;
        private void Calibraton()
        {
            

            CALIBRATION = true;
            while (CALIBRATION)
            {
                #region Прогресс барр отслеживания работы
                Invoke(pbps2, true);
                #endregion

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

                #region Предварительная установка параметров 
                //Установка параметров аттенюатора и времени выдержки работы генератора
                PreviousSetupAttenuationAndTimeDelay();
                #endregion

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

                #region Установка команды ON
                string commandON = genport.Generator(gencommand.GeneratorOnCommand());
                LogerPanel("Установка актуальных параметров на выходы генератора : " + commandON, "<g<");
                #endregion

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

                #region Запрос на получение напряженности поля
                //Запрос на передачу считанных напряженностей электромагнитного поля
                priport.ReadTension();
                LogerPanel("Запрос на получение напряженности поля", "<p<");
                #endregion

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

                #region Запуск генератора на Start
                string commandStart = genport.Generator(gencommand.GeneratorStartCommand());
                LogerPanel("Запуск теста, на генераторе : " + commandStart, "<g<");
                //Чтение порта, после выхода времени теста генератор дает ответ о завершении
                string[] generatorCancelationMessage = genport.PortReadWhileVaitForProcess(StaticParametr.Time);
                #endregion

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

                #region Оставновка выполнения команды на приемнике
                priport.StopCommand();
                #endregion

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

                if (generatorCancelationMessage[1] == "00")
                {
                    LogerPanel("Генератор закончил свою работу командой RR, 00;", ">g>");
                    double[] dd = StaticParametr.PRITensMassive;
                    double tension = StaticParametr.LastPRITens;
                    CALIBRATION = tensionComparation(tension);
                }
                else
                {
                    LogerPanel("Не пришла ожидаемая команда от генератора RR, 00;", ">g>");
                    while (true) { Thread.Sleep(10000000); }
                }

                #region Прогресс барр отслеживания работы
                Invoke(pbps2, false);
                #endregion

            }

            #region Прогресс барр отслеживания работы
            Invoke(pbps2, false);
            #endregion
        }
        #endregion

        #region Сравнение значений напряженности поля
        private bool tensionComparation(double t)
        {
            double tension = t;
            if (tension == StaticParametr.PointTensionDefolt)
            {
                xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
                xmlnewcalibrationpoint.XMLCalibrationInformationPointWrite();
                //Сохраняем результаты
                return false;
            }
            else
            {
                Comparation(tension);

                if (StaticParametr.PointTensMaxMinResult() == StaticParametr.AttMinResult && StaticParametr.PointTensMax > StaticParametr.PointTensionDefolt && StaticParametr.PointTensMin < StaticParametr.PointTensionDefolt)
                {

                    xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
                    xmlnewcalibrationpoint.XMLCalibrationInformationPointWrite();
                    //Произвести запись собранных параметров и выбрать следующую напряженность либо частоту
                    //Параметр AttMinResul характеризует минимальную разницу равную 5 в значениях аттенюатора
                    StaticParametr.TensionAttenuationFindedPoint[StaticParametr.PointTensionDefoltNumber] = StaticParametr.PointAttMax;
                    return false;

                }
                else
                {
                    //Передать управление на поиск следующей точки значения аттенюатора
                    //NextPointAtt(tension);
                    return true;
                }
            }
        }
        #endregion

        #region Сравнение полученной напряженности с необходимым значением
        private void Comparation(double tension)
        {
            if (tension < StaticParametr.PointTensionDefolt)
            {

                StaticParametr.PointAttLasMax = StaticParametr.PointAttMax;
                StaticParametr.PointTensMin = tension;
                StaticParametr.PointAttMax = StaticParametr.GeneratorAttSet;
                LogerPanel("PointTensMin Фиксированная напряженность - " + tension + " В  Необходимая - " + StaticParametr.PointTensionDefolt + " B", "compare");

                StaticParametr.GeneratorAttSet = StaticParametr.GeneratorAttSet - 5;

            }
            if (tension > StaticParametr.PointTensionDefolt)
            {

                StaticParametr.PointAttLastMin = StaticParametr.PointAttMin;
                StaticParametr.PointTensMax = tension;
                StaticParametr.PointAttMin = StaticParametr.GeneratorAttSet;
                LogerPanel("PointTensMax Фиксированная напряженность - " + tension + " В  Необходимая - " + StaticParametr.PointTensionDefolt + " B", "compare");

                StaticParametr.GeneratorAttSet = StaticParametr.GeneratorAttSet + 5;
            }
        }
        #endregion

        #region Подсчет следующей точки калибровки
        private void NextPointAtt(double tension)
        {
                attstepsmatch = new AttStepsMatch();
                int attset = attstepsmatch.NewAttSteps(tension, StaticParametr.GeneratorAttSet);
                StaticParametr.GeneratorAttSet = attset;
        }
        #endregion

        #region Работа с формой оповещения о смене антенны
        private void AntennaFormDialog(int antennNumber)
        {
            alertantenna = new AlertAntenna();
            int AntennNumber = 9999;
            for (int y = 0; y < StaticParametr.AntennNameArray.Length; y++)
            {
                if (StaticParametr.AntennDiapasonFq[y,0] <= StaticParametr.PointFqDefolt && StaticParametr.PointFqDefolt <= StaticParametr.AntennDiapasonFq[y, 1])
                {
                    alertantenna.AntennNameLabel(StaticParametr.AntennNameArrayString[y]);
                    AntennNumber = y;
                    LogerPanel("Для работы определенна антенна № " + StaticParametr.AntennNameArray[y] +" , " + StaticParametr.AntennNameArrayString[y], ">p>");
                }
            }

            //Устанавливаем флаг ожидангия подключения антенны, в положение ожидания ответа формы
            StaticParametr.AntennaWhileThread = true;
            StaticParametr.AntennaSetup = false;

            LogerPanel("Вызов диалогового окна", ">p>");
            alertantenna.ShowDialog();

            //Переходим в режим ожидания ответа оператора по форме
            AntennaWhile();
        }

        private void AntennaWhile()
        {
            while (StaticParametr.AntennaWhileThread)
            {
                Thread.Sleep(500);
            }
        }
        #endregion

        #region Завершение калибровки вызов окна и остановки процесса
        CalibrationCancelationForm calibrationcancelationform;
        private void CalibrationCancellAlert()
        {
            calibrationcancelationform = new CalibrationCancelationForm();
            StaticParametr.CalibrationCancellWhile = true;
            calibrationcancelationform.ShowDialog();

            while (StaticParametr.CalibrationCancellWhile)
            {
                Thread.Sleep(500);
            }

        }
        #endregion


        #region Процесс калибровки
        private void CalibrationProcess(double fq, double tens)
        {
            GeneratorCommandSend(fq, StaticParametr.PointAttLast);
            if (!StaticParametr.ECommandEnable)
            {
                LogerPanel(String.Join(",", StaticParametr.E), "<p<");
                PRICommandSend(StaticParametr.E);
                StaticParametr.ECommandEnable = true;
            }
           // Thread.Sleep(StaticParametr.PointTimeDelay * 1000);
        }

        XMLNewCalibrationPoint xmlnewcalibrationpoint;
        private bool CalibrationTensIf()
        {
            double tension = StaticParametr.LastPRITens;
            if (tension == StaticParametr.PointTensionDefolt)
            {
                xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
                xmlnewcalibrationpoint.XMLCalibrationInformationPointWrite();
                //Сохраняем результаты
                return true;
            }
            else
            {
                MinMaxCompare(tension);
                //if (StaticParametr.PointAttMax != 0 && StaticParametr.PointAttMin != null)
                //{
                    if (StaticParametr.PointTensMaxMinResult() == StaticParametr.AttMinResult && StaticParametr.PointTensMax > StaticParametr.PointTensionDefolt && StaticParametr.PointTensMin < StaticParametr.PointTensionDefolt)
                    {
                    
                        xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
                        xmlnewcalibrationpoint.XMLCalibrationInformationPointWrite();
                        //Произвести запись собранных параметров и выбрать следующую напряженность либо частоту
                        //Параметр AttMinResul характеризует минимальную разницу равную 5 в значениях аттенюатора
                        return true;
                 
                }
                    else
                    {
                        //Передать управление на поиск следующей точки значения аттенюатора
                        TensField(tension);
                        return false;
                    }
                //}
                //else
                //{
                //    //Передать управление на поиск следующей точки значения аттенюатора
                //    TensField(tension);
                //    return false;
                //}
            }
        }

        #region Определение принадлижания полученной напряженности и значения аттенюатора
        /// <summary>
        /// К минимальной и максимальной переменной значений
        /// </summary>
        /// 
        private void MinMaxCompare(double tension)
        {
            if (tension < StaticParametr.PointTensionDefolt)
            {

                StaticParametr.PointAttLasMax = StaticParametr.PointAttMax;
                StaticParametr.PointTensMin = tension;
                StaticParametr.PointAttMax = StaticParametr.GeneratorAttSet;
                LogerPanel("PointTensMin Фиксированная напряженность - " + tension + " В  Необходимая - " + StaticParametr.PointTensionDefolt + " B", "compare");



                StaticParametr.PointAttLastMin = StaticParametr.PointAttMax; 
                StaticParametr.PointTensMin = tension;
                StaticParametr.PointAttMin = StaticParametr.GeneratorAttSet;
                LogerPanel("PointTensMin Фиксированная напряженность - " + tension+ " В  Необходимая - " + StaticParametr.PointTensionDefolt+" B", "compare");
            }
            if (tension > StaticParametr.PointTensionDefolt)
            {






                StaticParametr.PointAttLasMax = StaticParametr.PointAttMin;
                StaticParametr.PointTensMax = tension;
                StaticParametr.PointAttMax = StaticParametr.GeneratorAttSet;
                LogerPanel("PointTensMax Фиксированная напряженность - " + tension + " В  Необходимая - " + StaticParametr.PointTensionDefolt + " B", "compare");
            }
        }
        #endregion

        AttStepsMatch attstepsmatch;
        private void TensField(double tension)
        {
            if (tension == StaticParametr.PointTensionDefolt)
            {
                xmlnewcalibrationpoint = new XMLNewCalibrationPoint();
                xmlnewcalibrationpoint.XMLCalibrationInformationPointWrite();
                //Сохраняем результаты
            }
            else
            {
                attstepsmatch = new AttStepsMatch();
                int attset = attstepsmatch.NewAttSteps(tension, StaticParametr.GeneratorAttSet);
                StaticParametr.GeneratorAttSet = attset;
            }
        }
        #endregion

        #endregion

        #region Конвеер для установки значений в ГЕНЕРАТОРЕ
        GENPort genport = new GENPort();
        GENCommand gencommand = new GENCommand();
        private bool GeneratorCommandSend(double fq, int att)
        {
            return GeneratorConveer(fq, att, StaticParametr.PointTimeDelay);
        }

        #region Конвеер команд генератора
        private bool GeneratorConveer(double fq, int att, int time)
        {
            try
            {
                string command;
                //Идентификация

                #region Идентификация
                Thread.Sleep(StaticParametr.GeneratorSleepCommand);
                if (!StaticParametr.GenIdentificationTru)
                {
                    GeneratorIdentification();
                }
                Thread.Sleep(StaticParametr.GeneratorSleepCommand);
                #endregion

                #region Сброс всех настроек генератора
                //Сброс всех настроек
                if (!StaticParametr.GeneratorRTSCommand)
                {
                    GeneratorRST();
                }
                #endregion

                Thread.Sleep(StaticParametr.GeneratorSleepCommand);

                #region Установка нужного блока команд
                //Установка нужного блока команд
                if (!StaticParametr.GeneratorBlockOn)
                {
                    GeneratorBlockSetup();
                }
                #endregion

                Thread.Sleep(StaticParametr.GeneratorSleepCommand);
                
                #region Установка необходимой частоты частоты
                //По необходимости выставление частоты
                if (StaticParametr.GeneratorLastFq != fq)
                {
                    command = genport.Generator(gencommand.GeneratorSetFq(fq));
                    LogerPanel(command, "<g<");
                }
                #endregion

                //По необходимости выставление аттенюатора
                Thread.Sleep(StaticParametr.GeneratorSleepCommand);
                #region Установка аттенюатора
                if (StaticParametr.GeneratorLastAtt != att)
                {
                    command = genport.Generator(gencommand.GeneratorSetAtt(att));
                    LogerPanel(command, "<g<");
                }
                #endregion
                Thread.Sleep(StaticParametr.GeneratorSleepCommand);

                #region Установка времени работы в режиме теста, по истечении времени работа генератора прекращается, и генератор выдает команду RR, 00; <LF>
                if (StaticParametr.GeneratorTimeDelayLast != time)
                {
                    int ttime = time * 10;
                    command = genport.Generator(gencommand.GeneratorTimeDelay(ttime));
                    LogerPanel(command, "<g<");
                }
                #endregion

                Thread.Sleep(StaticParametr.GeneratorSleepCommand);
                command = genport.Generator(gencommand.GeneratorOnCommand());
                LogerPanel(command, "<g<");
                //запуск генератора
                Thread.Sleep(StaticParametr.GeneratorSleepCommand);
                command = genport.Generator(gencommand.GeneratorStartCommand());
                LogerPanel(command, "<g<");
                StaticParametr.GeneratorLastFq = fq;
                StaticParametr.GeneratorLastAtt = att;
                StaticParametr.GeneratorTimeDelayLast = time;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Сброс всех настроек генератора
        private void GeneratorRST()
        {
            string command = genport.Generator(gencommand.GeneratorRSTCommand());
            LogerPanel(command, "<g<");
            StaticParametr.GeneratorRTSCommand = true;
        }
        #endregion

        #region Установка нужного блока
        private void GeneratorBlockSetup()
        {
            BLOCK:
            string command = genport.Generator(gencommand.GeneratorBWCommand());
            LogerPanel(command, "<g<");
            genport.PortReadWhile();
            if (StaticParametr.GENPortReadMessage[0] == "BW")
            {
                if (StaticParametr.GENPortReadMessage[1] == StaticParametr.GeneratorJobBlock)
                {
                    LogerPanel(String.Join("", StaticParametr.GENPortReadMessage), ">g>");
                    LogerPanel("Установлен блок: "+ StaticParametr.GENPortReadMessage[1], ">g>");
                    StaticParametr.GeneratorBlockOn = true;

                }
                else
                {
                    LogerPanel("Установлен блок: " + StaticParametr.GENPortReadMessage[1], ">g>");
                    StaticParametr.GeneratorBlockOn = false;
                    LogerPanel("Установка блока: " + StaticParametr.GeneratorJobBlock, ">g>");
                    string ccommand = genport.Generator(gencommand.GeneratorBSCommand(StaticParametr.GeneratorJobBlock));
                    LogerPanel(ccommand, "<g<");
                    genport.PortReadWhile();
                    if (StaticParametr.GENPortReadMessage[1] == StaticParametr.GeneratorJobBlock)
                    {
                        LogerPanel(String.Join("", StaticParametr.GENPortReadMessage), ">g>");
                        LogerPanel("Установлен блок: " + StaticParametr.GENPortReadMessage[1], ">g>");
                        StaticParametr.GeneratorBlockOn = true;

                    }
                    else
                    {
                        LogerPanel("ОШИБКА - ПеРеЛеТ 443 строка - Установлен блок: " + StaticParametr.GENPortReadMessage[1], ">g>");
                        StaticParametr.GeneratorBlockOn = false;
                        goto BLOCK;
                    }
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region Конвер управления задачей калибровки

        // сделать конвер управления задачей калибровки
        //
        /// <summary>
        /// Сделать конвеер который будет устанавливать 
        /// и запускать генератор ждать определенное время и 
        /// собирать значения с последних значений напряженности
        /// запускать процессы записи сравнения и принятия решения
        /// для ппередачи и повторной установки параметров генератора 
        /// и запуска теста либо выхода из цикла гото
        /// 
        /// </summary>

        #region Отладочная форма отслеживания выполнения команд
        CommandControl commandcontrol;

        private void ControlFormThreadFunction()
        {

            if (commandcontrol == null)
            {
                commandcontrol = new CommandControl();
                commandcontrol.Show();
            }
            else
            {

            }
        }
        #endregion

        private void ConveerCommander()
        {
            RERUN:
            //Установка параметров на генератор
            GeneratorCommandSend(StaticParametr.GeneratorFqSet, StaticParametr.GeneratorAttSet);

            //Установка частоты на приемнике
            if (StaticParametr.GeneratorFqSet != StaticParametr.PriFqSet)
            {
                //Сбрасываем флаг авторизации измерителя, т.к. после установки новых значений авторизация будет проходить заново
                StaticParametr.AutorisationPZ = false;

                //Устанавлтваем значение частоты на приемнике
                //Если идет какое то выполнение команды то оно прикращается
                priport.PackageUpdateMesuremetnter(StaticParametr.GeneratorFqSet);

                //Ожидание установки новых значений и последующей авторизации
                while (!StaticParametr.AutorisationPZ)
                {
                    Thread.Sleep(100);
                }
                PRIFqSet(StaticParametr.GeneratorFqSet);
                while (StaticParametr.CommandJob)
                {
                    Thread.Sleep(100);
                }
                //Отправляем команду на измерение напряженности поля приемником
                if (StaticParametr.CommandJob != true )
                priport.ReadTension();

                //Цикл бесконечного чтения значений выдаваемых измерителем
                if (StaticParametr.PRIReadProcessLoad)
                {
                    PRIPortReadWhileThread.Abort();
                    StaticParametr.PRIReadProcessLoad = false;
                }

                //Установка значений частоты на измерителе напряженности
                //PRIFqSet(StaticParametr.GeneratorFqSet);
            }

            //Запуск потока на передачу считанных напряженностей
            if(!StaticParametr.PRIReadProcessLoad)
                ThreadWhileStart();

            //Время ожидания на сравнение полученной напрфженности
            Thread.Sleep(StaticParametr.PointTimeDelay * 1000);
            if(!CalibrationTensIf())
                goto RERUN;
        }
        #endregion

        PRICommandReader pricommandreader;

        #region Установка частоты на приемнике
        private void PRIFqSet(double fq)
        {
            ///Частота приходит в ГЦ одно из значений 127994.29
            /// 
            pricommandreader = new PRICommandReader();
            PRICommandSend(pricommandreader.PRIFqCommandGeneration(fq));
            StaticParametr.PriFqSet = fq;
            LogerPanel("Установка параметров частоты на приемнике: " + fq, ">p>");
        }
        #endregion

        PRIPort priport = new PRIPort();

        #region Отправка команды в порт ПРИЕМНИКА, проверяет открыт ли порт если не открыт открывает и записывает в него сообщение
        private void PRICommandSend(byte[] command)
        {
            if(!priport.ISOpenPort())
                priport.OpenPort();
            priport.PortWrite(command);
        }
        #endregion

        #region Чтение порта приемника в бесконечном цикле вайл
        /// <summary>
        /// Происходит чтение порта 
        /// </summary>

        Thread PRIPortReadWhileThread;
        private void ThreadWhileStart()
        {
            PRIPortReadWhileThread = new Thread(WhileReadPort);
            PRIPortReadWhileThread.Start();
            StaticParametr.PRIReadProcessLoad = true;
        }

        private void WhileReadPort()
        {
            StaticParametr.PRIReadProcessLoad = false;
        }
        #endregion

        #region отправка команды на измерение напряженности поля
        private bool SendCommandGenerator()
        {
            priport.PortWrite(StaticParametr.E);
            StaticParametr.ECommandEnable = true;
            return true;

        }
        #endregion

        #region Обновление значений о последней напряженности и запись в массив напряжденностей
        private void UpdatePRIInformation(double tens)
        {
            Array.Resize(ref StaticParametr.PRITensMassive, StaticParametr.PRITensMassive.Length + 1);
            StaticParametr.LastPRITens = tens;
            StaticParametr.PRITensMassive[StaticParametr.PRITensMassive.Length - 1] = tens;

            Log("<p< "+tens);
            LogerPanel("Напряженность поля: " + tens, ">p>");
        }
        #endregion

        #region  Прогресс бары
        #region Прогресс бар для отслеживания откалиброванных частот
        public delegate void PBPS1(bool fq);
        PBPS1 pbsp1;
        private void ProgressBarConveer(bool fq)
        {
            if (fq == true)
            {
                ProgressBarOption();
            }
            else
            {
                ProgressBarPerformStep();
            }
        }

        private void ProgressBarOption()
        {
            this.progressBar1.Visible = true;
            this.progressBar1.Minimum = 1;
            this.progressBar1.Maximum = StaticParametr.FqStepArray.Length;
            this.progressBar1.Value = StaticParametr.FqStepArrayI+1;
            this.progressBar1.Step = 1;
        }

        private void ProgressBarPerformStep()
        {
            this.progressBar1.PerformStep();
        }
        #endregion

        #region Прогресс бар для отслеживания откалиброванных напряженностей
        public delegate void PBPS2(bool tens);
        PBPS2 pbps2;

        private void ProgressBarTensionConveer(bool tens)
        {
            if (tens == true)
            {
                ProgressBarTensionSetup();
            }
            else
            {
                ProgressBarTensionPerformStep();
            }
        }

        private void ProgressBarTensionSetup()
        {
            this.progressBar2.Visible = true;
            this.progressBar2.Minimum = 1;
            this.progressBar2.Maximum = 6;
            this.progressBar2.Value = 1;
            this.progressBar2.Step = 1;
        }

        private void ProgressBarTensionPerformStep()
        {
            this.progressBar2.PerformStep();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion

        #region Обновление информации в ListView

        #region Обновление информации в лист вью, о предыдущих точках
        public delegate void LWDGq();
        LWDGq lwdgq;

        private void DelegateParametrListWiewLastParametr()
        {
            if (lwdgbool == false)
            {
                lwdgq = new LWDGq(ListVievLastParametr);
            }
            Invoke(lwdgq);
        }

        private void ListVievLastParametr()
        {
            for (int y = 0; y < StaticParametr.PriviousLoaderFq.Length; y++)
            {
                int[,] att = (int[,])(StaticParametr.PriviousLoaderAttObj[y]);
                double[,] tens = (double[,])(StaticParametr.PriviousLoaderTensObj[y]);
                int[] steps = (int[])(StaticParametr.PriviousLoaderStepTens[y]);
                for (int o = 0; o < att.Length/2; o++)
                { 
                    AddParametrListView(Convert.ToString(StaticParametr.PriviousLoaderFq[y]), 
                        Convert.ToString(Convert.ToDouble(att[o,1]) / 10), 
                        Convert.ToString(tens[o, 1]), 
                        Convert.ToString(StaticParametr.TensionParametr[steps[o]]), 
                        Convert.ToString(tens[o,0]), 
                        Convert.ToString(Convert.ToDouble(att[o, 0]) / 10));
                }
            }
        }
        #endregion


        public delegate void LWDG();
        LWDG lwdg;
        bool lwdgbool = false;
        private void DelegateParametrListWiew()
        {
            if (lwdgbool == false)
            {
                lwdg = new LWDG(ParametrListWiew);
            }
            Invoke(lwdg);
        }

        private void ParametrListWiew()
        {
            AddParametrListView(Convert.ToString(StaticParametr.PointFqDefolt), 
                Convert.ToString(Convert.ToDouble(StaticParametr.PointAttMin)/10), 
                Convert.ToString(StaticParametr.PointTensMin), 
                Convert.ToString(StaticParametr.PointTensionDefolt), 
                Convert.ToString(StaticParametr.PointTensMax), 
                Convert.ToString(Convert.ToDouble(StaticParametr.PointAttMax) / 10));
        }

        private void AddParametrListView(string fq, string attMin, string emin,string e, string emax, string attmax)
        {
            listView1.Items.Add(new ListViewItem(new string[] { fq+ " МГц", attMin + " Дб", emin + " В/м^2", e + " В/м^2", emax + " В/м^2", attmax + " Дб" }));
            int lastItems = listView1.Items.Count - 1;
            listView1.MultiSelect = false;
            listView1.GridLines = true;
            listView1.Items[lastItems].EnsureVisible();
            if (listView1.Items.Count == 8)
            {
                listView1.Columns[5].Width = listView1.Columns[5].Width - 18;
            }
        }
        #endregion

        #region 
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {

        }

        
    }
}
