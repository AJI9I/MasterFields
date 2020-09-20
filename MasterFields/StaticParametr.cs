using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    static class StaticParametr
    {
        
        public static double[] FqStepArray;

        public static bool[] FqStepEnable;

        #region Параметры для редактирования точек чакстоты
        #region позиции массива для редактирования
        public static int[] PositionMassive;
        #endregion

        #region Параметры для редактирования полосы точек частоты
        public static bool DoubleClicLabbelNoLeave = false;
        public static string LabelFqName;
        #endregion

        #region Позиции аттенюатора на откалиброванных значениях напряженности поля
        //Тут хранятся найденные параметры аттенюатора для значений напряженности поля (приближенно)
        //последовательность значений совпадает со значениями в массиве необходимых напряженностей поля
        public static int[] TensionAttenuationFindedPoint;
        #endregion
        #endregion

        #region Делитель частоты ГЦ до МГЦ
        public static int DelitelKHz = 1000;
        #endregion

        #region Параметры задаваемые программе для выполнения задания либо теста

        public static double[] TensionParametr;

        public static int Time;

        public static double FqMax;

        public static double FqMin;

        public static double Step;

        public static bool StepParametr;

        public static string FileParametrName;
        #endregion

        #region Параметры для сохранения результатов в файл
        //Имя диретории для хранения параметров калибровки
        public static string CalibrationFolderName = "CalibrationFile";
        //Имя хмл файла параметров калибровки
        public static string CalibrationParametrFile = "CalibrationTestFile.xml";
        #endregion

        #region Параметры калибровки выбор точки и поляризации
        public static bool PointEnable = false;
        public static bool PolarisationEnable = false;

        public static void Refresh()
        {
            PointEnable = PolarisationEnable = false;
        }

        public static string PointName;
        public static string PolarisationName;
        #endregion

        #region Параметры для Записи информации об откалиброванных точках
        //Значение напряженности находящееся непосредственно на исследовании
        public static double PointTensionDefolt;
        public static int PointTensionDefoltNumber;

        public static int PointAttLast = 0;
        public static int PointAttMax = 0;
        public static int PointAttMin = 0;
        
        #region последние значения
        public static int PointAttLasMax;
        public static int PointAttLastMin;
        #endregion

        public static double PointTensMax;
        public static double PointTensMin;

        public static int PointTimeDelay = 3;

        public static double PointFqDefolt;
        public static int PointFqDefoltNumber;

        #region Информация для работы с антеннами
        public static string AntennName = "Тестовая антенна";
        public static int[] AntennNameArray = new int[] {1, 3};
        public static string[] AntennNameArrayString = new string[] { "АП - 1", "АП - 3" };
        public static double[,] AntennDiapasonFq = new double[2,2] { {300, 40000 }, {0.03, 300 } };

        //Переменная ожидания ответа оператора, блокирует поток в цикле while строка 591
        public static bool AntennaWhileThread = true;

        //Переменная содержащая значение ответа оператора
        public static bool AntennaSetup;
        #endregion

        public static int PointTensMaxMinResult()
        {
            return PointAttMax - PointAttMin;
        }

        public static int AttMinResult = 5;

        #region Для процесса калибровки
        #region Генератор процесс калибровки
        public static double GeneratorFqSet;
        public static int GeneratorAttSet = 635;
        #endregion

        #region приемник процесс калибровки
        public static bool PRIReadProcessLoad = false;
        #endregion
        #endregion
        #endregion

        #region Параметры для работы с портом генератора
        public static int GENBaudRate = 9600;
        public static int GENDataBits = 8;
        public static int GENReadTimeOut = 10000;
        public static string GENPortName = "COM3";

        public static int GENSleepTimeRead = 200;

        public static bool GENPortMessage = false;
        public static bool GENPortMessageWhileRead = true;

        public static string[] GENPortReadMessage;

        
        public static string GeneratorJobBlock = "3";
        
        #endregion

        #region Параметры для работы с портом приемника
        public static int PRIBaudRate = 9600;
        public static int PRIDataBits = 8;
        public static int PRIReadTimeOut = 10000;
        public static string PRIPortName = "COM5";

        //Флаг на то что чтение уже идет
        public static bool PRIPortReadBool = false;
        //Ожидаемоме число байт приема
        public static int PRICountButesRead = 7;
        //Время на запись одного байта, используется при чтении с порта приемника вычисления ожидания оставшихся
        // для записи байтов
        public static int PRITimeWriteOneByte = 11;
        //Переменная прихода данных в порт
        public static bool PRIPortMessageEnable = false;
        //Переменная бесконечного цикла выхода
        public static bool PRIPortReadWhile = false;
        #endregion

        #region Идентификационные строки генератор измеритель
        //Идентификация генератора
        public static string GENIdentification = "CWS500N1";


        public static string PRIIdentification = "PRI";
        #endregion

        #region Парметры конвеера команд
        public static bool GenIdentificationTru = false;

        public static bool GeneratorBlockOn = false;

        public static bool GeneratorRTSCommand = false;

        public static double GeneratorLastFq = 0.0;
        public static int GeneratorLastAtt = -1;
        public static int GeneratorTimeDelayLast = -1;

        public static int GeneratorSleepCommand = 30;
        #endregion

        #region Промежуточные флага калибровки
        //Благ передающий необходимость изменения напряженности на следующую если таковая имеется используется для калибровки
        public static bool NextTens = false;
        #endregion

        #region Промежуточные параметры приемника последнее значение и все принятые значения за промежутов времени
        //Последнее полученное значение напряженности
        public static double LastPRITens;
        //Все полученные значения  напряженности за сеанс работы либо для одной точки
        public static double[] PRITensMassive = new double[0];
        //Установленная частота на стороне приемника
        public static double PriFqSet = 0.0;
        #endregion

        #region Информация об ожидаемом пакете с приемника, количество байт данных, тело сообщения и контрольная сумма в количественном отшении
        //Количество байт содержащих информацию о количестве байт пакета
        public static int PRICountOne = 2;
        //Количество байт в информационно пакете
        public static int PRICountTwo = 3;
        //Количество байт в контрольной сумме
        public static int PRICountThree = 2;
        #endregion

        #region Команды посылаемые в приемник для получения результатов
        public static byte[] E = new byte[] { 0x45, 0x00, 0x00, 0x00, 0x45, 0x00 };
        public static bool ECommandEnable = false;

        public static byte[] R = new byte[] { 0x52, 0x00, 0x00, 0x00, 0x52,0x00};

        public static byte[] IdendPRICommand = new byte[] { 0x45, 0x00, 0x00, 0x00, 0x45, 0x00 };

        public static int AttParametrMax = 635;
        #endregion

        #region Установка калибровочных точек реализация продолжения калибровки заданной точки
        //Переменная номера частоты из цикла калибровки массива частот
        public static int FqStepArrayI;

        //Переменная номера напряженности
        public static int TensionParametrI;
        #endregion

        #region Промежуточные переменные работы программы
        //Переменна загрузки файла, обслуживается при открытии файла
        public static bool FileLoad = false;

        public static bool FileLoadWhiele = true;
        #endregion

        #region Логирование
        public static string LogFileDirectory = "Logger";
        public static string LogFileName = "LogFile";

        public static bool FileDirectory = false;
        public static bool LogFileCreate = false;
        #endregion

        #region Административные переменные ИЗМЕРИТЕЛЬ НАПРЯЖЕННОСТИ
        //Прошла ли авторизация на приемнике
        public static bool AutorisationPZ = false;

        //Идет ли выполнение какой либо команды
        public static bool CommandJob = false;

        //Имя команды которая выполняется
        public static string CommandName;

        //Флаг ожидания последнего чтения с порта приемника
        public static bool LastRead;

        //Флаг прихода ответа об успешной записи новыхзначений, можно опрашивать
        public static bool WriteNewParametrOk;

        //Флаг прочтения/прихода нового значения измеренной напряженности
        public static bool PriTensionNewMesurementer = false;
        #endregion

        #region Параметры измерителя напряженности 23.01.17
        //Поправочный коэффициент
        public static byte[] PoprKoeficient;

        //Параметры измерителя в переменной типа ОБЬЕКТ
        public static object PackageSetupDiviceValue;

        //Параметры измерителя для записи в него же
        public static object[] PriborParametrSetups = new object[12];
        #endregion

        #region Отладчик
        public static string MessagaOtladchik;
        #endregion

        #region Переменные для загрузки исследованных параметров
        //Исследованные параметры загружаются из XMLNewCalibrationPoint.cs , строка 239

        //Массив откалиброванных частот
        public static double[] PriviousLoaderFq = new double[0];
        //Массив значений аттенюаторов со вложенными значениями
        public static object[] PriviousLoaderAttObj = new object[0];
        //Массив значений напряженностей со вложенными значениями
        public static object[] PriviousLoaderTensObj = new object[0];
        //Массив значений точек напряженности
        public static object[] PriviousLoaderStepTens = new object[0];

        #endregion

        #region Окончание калибровки
        public static bool CalibrationCancellWhile = true;
        #endregion

        #region Статические параметры проведения анализа полученных паораметров
        public static double[] FqParametrArray;

        public static bool[] FqParamrtrArrayEnable;

        //32 позиции значений параметров файлов, связанные с параметрами полученными из папки где храняться файлы калибровки
        //Параметры хранятся для каждой частоты в обьекте, привязанном по индексу к массиву частот
        //Обьект состоят из 4 х позиций в которых записанны параметры ParamPamPam = new object[] { amax, amin, imax, imin}
        //Вложенность object[]{object[FileInformationName.lenght]{Object[FqParametrArray.lenght]{object[4]{}}},object[]{},object[]{},...,object[n]{}}
        public static object[] AttMinMax_FieldMinMax_FileCompilation;

        //Имена полученных файлов калибровки
        public static string[] FileInformationName;

        //Имена полученных файлов калибровки в разобранном виде для анализа
        public static string[,] FileInformationNameSplit;

        #endregion
    }
}
