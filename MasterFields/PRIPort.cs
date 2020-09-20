using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;


namespace MasterFields
{
    class PRIPort
    {
        SerialPort serialport = new SerialPort();

        #region Открытие порта
        public bool OpenPort()
        {
            try
            {
                serialport.BaudRate = StaticParametr.PRIBaudRate;
                serialport.DataBits = StaticParametr.PRIDataBits;
                serialport.StopBits = StopBits.One;
                serialport.PortName = StaticParametr.PRIPortName;
                serialport.DataReceived += new SerialDataReceivedEventHandler(serialportDataRecived);
                serialport.Open();

                return true;
            }
            catch
            {
                if (serialport.IsOpen)
                {
                    serialport.Close();
                }
                return false;
            }
        }
        #endregion

        #region Проверка открыт ли используемый порт
        public bool ISOpenPort()
        {
            return serialport.IsOpen;
        }
        #endregion

        #region Событие прихода сообщения в порт, устанавливает только флаг что что то пришло дальше нужно опрашивать этот флаг постоянно
        //Поток на приход сообщения вывести только на установку флага внешенму потоку на запрос
        // прихода сообщения в порт
        // внешним потоком производить чтение.
        void serialportDataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            PortReader();
            if (StaticParametr.LastRead)
            {
                byte[] RCommand = new byte[] { 0x52, 0x00, 0x00, 0x00, 0x52, 0x00 };
                PortWrite(RCommand);
                StaticParametr.LastRead = false;
                StaticParametr.CommandJob = false;
            }
        }
        #endregion

        #region Делегат и функция для записи лог файла
        LogFile logfile;
        public delegate void LOG(string msg);
        LOG log;
        private void LogDelegate(string msg)
        {
            log = new LOG(Log);
            Invoke(log, msg);
        }
        private void Invoke(LOG log, string msg)
        {
            log(msg);
        }

        private void Log(string msg)
        {
            logfile = new LogFile();
            logfile.LogWrite(msg);
        }
        #endregion

        #region Запись команды в порт 30.03.17
        public void PortWrite(byte[] command)
        {
            RET:
            if (serialport.IsOpen)
            {
                Log(">p> " + String.Join(" ", command));
                serialport.Write(command, 0, command.Length);
            }
            else
            {
                OpenPort();
                goto RET;
            }
            
        }
        #endregion

        #region Закрытие порта
        public void PortClose()
        {
            serialport.Close();
        }
        #endregion

        #region Получение сообщения из порта по изменению флага чтения
        /// <summary>
        /// Чтение сообщения из порта приемника по выставленному флагу прихода сообщения
        /// ответ содержит значение полученное с порта в данном случае значение представляет напряженность поля
        /// возможно и другие исходы структура пакета описанны в статическом классе 
        /// </summary>
        public double PortRead()
        {
            StaticParametr.PRIPortReadBool = true;
            byte[] portmessage = new byte[StaticParametr.PRICountButesRead];
            byte[] pm;
            while (StaticParametr.PRIPortReadBool)
            {
                try
                {
                    RET:
                    RETT:
                    int portMessageSize = serialport.BytesToRead;
                    int raznica = StaticParametr.PRICountButesRead - portMessageSize;
                    if (raznica != 0)
                    {
                        Thread.Sleep(raznica * StaticParametr.PRITimeWriteOneByte);
                        goto RET;
                    }

                    serialport.Read(portmessage, 0, StaticParametr.PRICountButesRead);
                    StaticParametr.PRIPortReadBool = false;
                    StaticParametr.PRIPortMessageEnable = false;
                }
                catch
                {
                    pm = new byte[serialport.BytesToRead];
                    serialport.Read(pm, 0, serialport.BytesToRead);
                }
            }

            return DelitelPakage(StaticParametr.PRICountOne, StaticParametr.PRICountTwo, StaticParametr.PRICountThree, portmessage);
        }


        #endregion

        #region Парсинг пакета с напряженностью
        private double DelitelPakage(int summByte, int summInformation, int summControlSumm, byte[] msg)
        {
            byte[] one = new byte[summByte];
            int Ione = 0;

            byte[] two = new byte[summInformation];
            int Itwo = 0;

            byte[] three = new byte[summControlSumm];
            int Ithree = 0;

            for (int i = 0; i < msg.Length; i++)
            {

                if (i < summByte)
                {
                    one[Ione] = msg[i];
                    Ione++;
                }

                if (i < summByte + summInformation && i > summByte - 1)
                {
                    two[Itwo] = msg[i];
                    Itwo++;
                }

                if (i < summByte + summInformation + summControlSumm && i > summByte + summInformation - 1)
                {
                    three[Ithree] = msg[i];
                    Ithree++;
                }
            }
            return Converter(two);
        }
        #endregion

        #region Конвертер переводит значение напряженности полученное из порта в действительное
        private double Converter(byte[] msg)
        {
            UInt32 res = 0;
            int j = 0;
            foreach (byte ms in msg)
            {
                res += Convert.ToUInt32(ms) << j;
                j += 8;
            }
            //Результат ответа необходимо разделить на 10000.00  что бы получить нужное число
            double answ = (double)res / 10000.00;
            return answ;
        }
        #endregion

        #region Не ИСПОЛЬЗУЕТСЯ
        #region Чтение порта 2.0 НЕ ИСПОЛЬЗУЕТСЯ

        private void PriPortRead()
        {
            byte[] BytePakageCount = new byte[2];

            #region Получение длинны пакета
            int count = 0;

            byte[] CountByte = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            
            bool countBoolReadWhile = true;
            while (countBoolReadWhile)
            {
                int countButePort = serialport.BytesToRead;
                if (countButePort == 2 || countButePort > 2)
                {
                    //Записываем байты содержащие информацию о количестве байтов данных в массив
                    serialport.Read(BytePakageCount, 0, 2);

                    //Добавление в массив для пересчета в количество приходящих байт
                    Array.ConstrainedCopy(BytePakageCount, 0, CountByte, 0, 2);

                    //Преобразованиенв числовое значение
                    count = BitConverter.ToInt32(CountByte, 0);
                    countBoolReadWhile = false;
                }
            }
            #endregion

            byte[] BytePakageMessage = new byte[count];

            #region Получение информационной части пакета
            bool portMessageReadWhile = true;

            while (portMessageReadWhile)
            {
                int countButePortMessage = serialport.BytesToRead;

                if (countButePortMessage == count || countButePortMessage > count)
                {
                    serialport.Read(BytePakageMessage, 0, count);
                    portMessageReadWhile = false;
                }
                else
                {
                    int raznica = count - countButePortMessage;
                    raznica = raznica * 7;
                    Thread.Sleep(raznica);
                }
            }
            #endregion

            byte[] BytePakageControlSumm = new  byte[2];

            #region Получение контрольной суммы

            bool portControllSummReadWhile = true;

            while (portControllSummReadWhile)
            {
                int countButePortControllSumm = serialport.BytesToRead;
                if (countButePortControllSumm == 2)
                {
                    serialport.Read(BytePakageControlSumm, 0, 2);
                    portControllSummReadWhile = false;
                }
            }
            #endregion


        }
        #endregion

        private int PRIPortReadCountBute()
        {
            int count = 0;
            byte[] CountByte = new byte[] { 0x00, 0x00, 0x00, 0x00 }; 
            bool countBool = true;
            while (countBool)
            {
                int countButePort = serialport.BytesToRead;
                if (countButePort == 2 || countButePort > 2)
                {
                    serialport.Read(CountByte,0,2);
                    count = BitConverter.ToInt32(CountByte,0);
                    countBool = false;
                }
            }
            return count;
        }

        #region Определение пакета данных
        private int CommandLenght(byte[] CommandIdentification)
        {
            char Idn = Convert.ToChar(CommandIdentification[0]);
            if (Idn == 'K')
            {
                return 30;
            }
            if (Idn == 'L')
            {
                return 5;
            }
            return 30;
        }
        #endregion
        #endregion


        #region Чтение установок прибора
        public void ReadSetupDevice()
        {
            BytesToReadCommand = 68;
            SleepTime = BytesToReadCommand * 12;

            #region Сервисные функции , работа команды и название рабочей команды
            StaticParametr.CommandJob = true;
            StaticParametr.CommandName = "L";
            #endregion

            byte[] I = new byte[] { 0x4C, 0x00, 0x00, 0x00, 0x4C, 0x00 };
            PortWrite(I);
        }

        //Симещение поля
        int[] PackageSetupDiviceShift = new int[] { 0, 2, 3, 6, 9, 12, 15, 18, 21, 24, 27, 45, 53, 66 };
        //Размер поля байтов
        int[] PackageSetupDiviceSize = new int[] { 2, 1, 3, 3, 3, 3, 3, 3, 3, 3, 18, 8, 13, 2 };
        //Массив байтов данных
        object[] PackageSetupDiviceParse = new object[14];
        //Парсинг пакета с учетом размера поля и смещения
        private void PackageSetupDevice(byte[] value)
        {
            StaticParametr.MessagaOtladchik = "Первая функция PackageSetupDevice(byte[] value)";
            string DataString = String.Join(", ", value);

            for (int i = 0; i < PackageSetupDiviceParse.Length; i++)
            {
                int size = PackageSetupDiviceSize[i];
                byte[] VALUE = new byte[size];
                for (int a = 0; a < VALUE.Length; a++)
                {
                    VALUE[a] = value[PackageSetupDiviceShift[i] + a];
                }
                PackageSetupDiviceParse[i] = VALUE;
            }
            TranslitePackageSetupDevice(PackageSetupDiviceParse);
        }

        //Массив множителей
        double[] MultiplerPackageSetupDevice = new double[] { 1, 1, 100.00, 100.00, 100.00, 10000.00, 10.00, 100.00, 100.00, 100.00 };
        //Массив переведенных значений
        object[] PackageSetupDiviceValue = new object[12];

        //Парсинг пакета на значения выдаваемые прибором
        private void TranslitePackageSetupDevice(object[] value)
        {
            StaticParametr.MessagaOtladchik = "Вторая функция TranslitePackageSetupDevice(object[] value)";
            for (int i = 0; i < MultiplerPackageSetupDevice.Length; i++)
            {
                byte[] mask = new byte[] { 0, 0, 0, 0 };
                byte[] objByte = (byte[])value[i];

                #region Вырываем отдельно из пакета авторизации значение поправочного коэфициента
                if (i == 4)
                {
                    StaticParametr.PoprKoeficient = objByte;
                }
                #endregion
                Array.Copy(objByte, mask, objByte.Length);
                int val = BitConverter.ToInt32(mask, 0);
                double vald = val / MultiplerPackageSetupDevice[i];
                PackageSetupDiviceValue[i] = vald;
            }
            string ProgrammVersion1 = System.Text.Encoding.ASCII.GetString((byte[])value[12]).Trim('\0');
            string ProgrammVersion = System.Text.Encoding.ASCII.GetString((byte[])value[11]);
            PackageSetupDiviceValue[10] = ProgrammVersion1;
            PackageSetupDiviceValue[11] = ProgrammVersion;

            StaticParametr.PackageSetupDiviceValue = PackageSetupDiviceValue;
            StaticParametr.PriborParametrSetups = PackageSetupDiviceValue;

            StaticParametr.MessagaOtladchik = "Установка флага авторизации";
            //Флаг авторизации измерителя
            StaticParametr.AutorisationPZ = true;
        }

        #region Название значений в массиве PackageSetupDiviceValue
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
        #endregion

        #endregion

        #region Чтение порта приемника непосредственно
        //Количество байтов для чтения
        public static int BytesToReadCommand;
        //Сообщение которое пришло в порт
        byte[] ValuePortRead;
        //Время ожидания для записи пакета в порт включая полное ожидание покета
        int SleepTime = 0;
        private void PortReader()
        {
            Thread.Sleep(200);
            bool fall = true;
            ValuePortRead = new byte[BytesToReadCommand];
            Thread.Sleep(SleepTime);
            serialport.Read(ValuePortRead, 0, BytesToReadCommand);
            //Проверяем на наличие пакета
            //После изменения установок прибора
            byte package = 88;
            if (ValuePortRead[0] == package)
            {
                fall = false;
                StaticParametr.WriteNewParametrOk = false;
            }
            //Если флаг пропуска пакета не стоит в отрицательном положении то читаем как положенно
            if (fall)
            {
                StaticParametr.MessagaOtladchik = "Переход к подсчету контрольной суммы";
                ControllSummControl();
            }
        }

        private void ControllSummControl()
        {
            byte[] package = new byte[ValuePortRead.Length - 2];
            byte[] Crc = new byte[2];

            Array.Copy(ValuePortRead, 0, package, 0, ValuePortRead.Length - 2);
            Array.Copy(ValuePortRead, ValuePortRead.Length - 2, Crc, 0, 2);

            byte[] CrcPackage = BitConverter.GetBytes(crc(package));

            StaticParametr.MessagaOtladchik = "Сравнение полученной суммы";

            if (CrcPackage[0] == Crc[0] && CrcPackage[1] == Crc[1])
            {
                //Пакет, получен/передан верно
                StaticParametr.MessagaOtladchik = "Пакет пересчитан и принят верно";
                PortReadCommandSwitch();
            }
            else
            {
                int i = 0;
                //Пакет, получен/передан с ошибкой
            }
        }

        private ushort crc(byte[] data, ushort sum = 0)
        {
            foreach (byte b in data) sum += b;
            return sum;
        }

        private void PortReadCommandSwitch()
        {
            StaticParametr.MessagaOtladchik = "Свич переключатель на определение рабочей команды";
            switch (StaticParametr.CommandName)
            {
                case "L":
                    StaticParametr.CommandJob = false;
                    PackageSetupDevice(ValuePortRead);
                    break;
                case "E":
                    PackageTensionDevice(ValuePortRead);
                    break;
                case "W":
                    //PackageEnergyFluxDensity(ValuePortRead);
                    break;
                case "P":
                    //PackageExposureElectricField(ValuePortRead);
                    break;
                case "Q":
                    //PackageExposureDensityFlowEnergy(ValuePortRead);
                    break;
            }


        }
        #endregion

        #region Парсинг пакета со значением напряженности
        //Симещение поля
        int[] PackageTensionDiviceShift = new int[] { 0, 2, 5 };

        //Размер поля байтов
        int[] PackageTensionDiviceSize = new int[] { 2, 3, 2 };

        //Массив байтов данных
        object[] PackageTensionDiviceParse = new object[3];

        //Парсинг пакета с учетом размера поля и смещения
        private void PackageTensionDevice(byte[] value)
        {
            for (int i = 0; i < PackageTensionDiviceParse.Length; i++)
            {
                int size = PackageTensionDiviceSize[i];
                byte[] VALUE = new byte[size];
                for (int a = 0; a < VALUE.Length; a++)
                {
                    VALUE[a] = value[PackageTensionDiviceShift[i] + a];
                }
                PackageTensionDiviceParse[i] = VALUE;
            }
            TranslitePackageTensionDevice(PackageTensionDiviceParse);
        }

        //Массив множителей
        int[] MultiplerPackageTensionDevice = new int[] { 1, 10000, 1 };
        //Массив переведенных значений
        object[] PackageTensionDiviceValue = new object[3];

        //Парсинг пакета на значения выдаваемые прибором
        private void TranslitePackageTensionDevice(object[] value)
        {
            for (int i = 0; i < MultiplerPackageTensionDevice.Length; i++)
            {
                double vald;
                if (i == 1)
                {
                    UInt32 res = 0;
                    int j = 0;
                    foreach (byte ms in (byte[])value[i])
                    {
                        res += Convert.ToUInt32(ms) << j;
                        j += 8;
                    }
                    vald = (double)res / 10000.00;
                }
                else
                {
                    byte[] mask = new byte[] { 0, 0, 0, 0 };
                    byte[] objByte = (byte[])value[i];
                    Array.Copy(objByte, mask, objByte.Length);
                    int val = BitConverter.ToInt32(mask, 0);
                    vald = val / MultiplerPackageTensionDevice[i];

                }
                PackageTensionDiviceValue[i] = vald;

            }

            UpdatePRIInformation((double)PackageTensionDiviceValue[1]);
        }

        private void UpdatePRIInformation(double tens)
        {
            Array.Resize(ref StaticParametr.PRITensMassive, StaticParametr.PRITensMassive.Length + 1);
            StaticParametr.LastPRITens = tens;
            StaticParametr.PriTensionNewMesurementer = true;
            StaticParametr.PRITensMassive[StaticParametr.PRITensMassive.Length - 1] = tens;

            Log("<p< " + tens);
        }
        #endregion

        #region Запись в измеритель новых значений
        public void PackageUpdateMesuremetnter(double frquency)
        {
            StaticParametr.PriborParametrSetups[2] = frquency;
            //double Fq = (double)StaticParametrClass.PriborParametrSetups[2];
            //double MaxE = (double)StaticParametrClass.PriborParametrSetups[4];
            //double MaxH = (double)StaticParametrClass.PriborParametrSetups[5];
            //double PPE = (double)StaticParametrClass.PriborParametrSetups[6];
            //double ExpE = (double)StaticParametrClass.PriborParametrSetups[7];
            //double ExpH = (double)StaticParametrClass.PriborParametrSetups[8];
            //double ExpPPE = (double)StaticParametrClass.PriborParametrSetups[9];
            int[] ParametrUpdateIndex = new int[] { 2, 4, 5, 6, 7, 8, 9 };
            int[] ParametrPositionIndex = new int[] { 5, 11, 14, 17, 20, 23, 26 };

            byte[] PackageParametrUpdate = new byte[] { 75, 0, 25, 0, 3,
                88, 2, 0,
                126,37, 0,
                188, 138, 169,
                53,66, 15,
                52, 115, 203,
                112, 103, 220,
                16, 92, 237, 76, 80, 254 };

            for (int i = 0; i < ParametrUpdateIndex.Length; i++)
            {
                int parametr = Convert.ToInt32((double)StaticParametr.PriborParametrSetups[ParametrUpdateIndex[i]] * MultiplerPackageSetupDevice[ParametrUpdateIndex[i]]);
                byte[] parametrByte = BitConverter.GetBytes(parametr);
                Array.Copy(parametrByte, 0, PackageParametrUpdate, ParametrPositionIndex[i], 3);

            }

            //Вставляем в отправочный пакет значение поправочного коэфициента
            Array.Copy(StaticParametr.PoprKoeficient, 0, PackageParametrUpdate, 8, 3);

            byte[] Antenna = BitConverter.GetBytes(Convert.ToInt32((double)StaticParametr.PriborParametrSetups[1]));
            PackageParametrUpdate[4] = Antenna[0];

            //
            string DataString = String.Join(", ", PackageParametrUpdate);

            var ControllSumm = BitConverter.GetBytes(crc(PackageParametrUpdate));

            Array.Resize(ref PackageParametrUpdate, PackageParametrUpdate.Length + 2);

            Array.Copy(ControllSumm, 0, PackageParametrUpdate, 29, 2);

            //Запись в прибор строки с обновленной информацией !!!!!СДЕЛАТЬ ПРОВЕРКУ ЗАНЯТ ЛИ ПОРТ!!!!!
            ControllCommands(PackageParametrUpdate);
        }

        bool doubleAutorisation;

        bool pakageDoubleAutorisation;

        private void ControllCommands(byte[] command)
        {
            if (StaticParametr.CommandJob)
            {
                StaticParametr.LastRead = true;

                while (StaticParametr.LastRead)
                {
                    Thread.Sleep(100);
                }
                StaticParametr.WriteNewParametrOk = true;
                PortWrite(command);
                while (StaticParametr.WriteNewParametrOk)
                {
                    Thread.Sleep(100);
                }
                StaticParametr.AutorisationPZ = false;
            }
            else
            {
                StaticParametr.WriteNewParametrOk = true;
                PortWrite(command);
                StaticParametr.AutorisationPZ = false;
                Thread.Sleep(100);
            }
            BytesToReadCommand = 92;
            SleepTime = BytesToReadCommand * 12;

            StaticParametr.WriteNewParametrOk = true;
            while (StaticParametr.WriteNewParametrOk)
            {
                Thread.Sleep(100);
            }
        }
        #endregion

        #region Запрос на получение напряженности в реальном времени
        public void ReadTension()
        {
            if (StaticParametr.AutorisationPZ)
            {
                if (!StaticParametr.CommandJob)
                {
                    BytesToReadCommand = 7;
                    SleepTime = BytesToReadCommand * 12;

                    #region Сервисные функции , работа команды и название рабочей команды
                    StaticParametr.CommandJob = true;
                    StaticParametr.CommandName = "E";
                    StaticParametr.ECommandEnable = true;
                    #endregion

                    byte[] I = new byte[] { 0x45, 0x00, 0x00, 0x00, 0x45, 0x00 };
                    PortWrite(I);
                }
                else
                {
                    //Идет незавершенное выполнение команды
                    //Добавить вывод шибки
                    // Имя команды на выполнении
                    string commandName = StaticParametr.CommandName;
                   //CommandStopTreadStart("E");
                }
            }
            else
            {
                //Вывод ошибки, авторизация не прошла
                //Прибор не подключен
            }
        }
        #endregion

        #region Принудительная остановка выполнения команды передачис приемника информации
        public void StopCommand()
        {
            byte[] RCommand = new byte[] { 0x52, 0x00, 0x00, 0x00, 0x52, 0x00 };
            PortWrite(RCommand);
            StaticParametr.CommandJob = false;
        }
        #endregion
    }
}
