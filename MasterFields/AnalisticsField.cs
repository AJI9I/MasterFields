using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace MasterFields
{
    class AnalisticsField
    {

        #region Управляющий поток
        int tensStep;
        int[] calibrationPointArray;
        public void WHAlistics(int TensStep, int[] CalibrationPointArray)
        {
            //Номер нопряженности
            tensStep = TensStep;

            //Точки калибровки выбранной области необходимые для исследования
            calibrationPointArray = CalibrationPointArray;

            //Получаем все файлы из каталога и предварительно обрабатывам их для получения
            //информации о точке и поляризации файла
            FileInformation(GetFilePointCatalog());

            //Загрузка параметров частоты
            UploadFqParametr();

            //Загрузка параметров из файлов калибровки
            FileCalibrationParametrUpdate();

        }
        #endregion
        #region Получить путь к фалу калибровки
        private string GetPatch()
        {
            return Environment.CurrentDirectory + "//" + StaticParametr.CalibrationFolderName + "//" + StaticParametr.FileParametrName;
        }
        #endregion

        #region Получение списка файлов из каталого калибровки
        FindAndCreateFolder findandcreatefolder;
        private FileInfo[] GetFilePointCatalog()
        {
            findandcreatefolder = new FindAndCreateFolder();
            string patch = GetPatch();
            FileInfo[] fileinfo = findandcreatefolder.GetFilesFromDirectory(patch);
            return fileinfo;
        }
        #endregion

        #region Разбор названий файлов
        private void FileInformation(FileInfo[] fileinfo)
        {
            string[,] fileInformationSplit;
            string[] fileinformation;

            if (Array.Exists(fileinfo, p => p.ToString() == "CalibrationTestFile.xml"))
            {
                fileInformationSplit = new string[fileinfo.Count() - 1, 2];
                fileinformation = new string[0];
            }
            else
            {
                fileInformationSplit = new string[fileinfo.Count(), 2];
                fileinformation = new string[0];
            }

            for (int i = 0; i < fileinfo.Length; i++)
            {
                //Тут нужно уточнить в каком формате приходит сообщение
                if (Convert.ToString(fileinfo[i]) != "CalibrationTestFile.xml")
                {
                    
                    Array.Resize(ref fileinformation, fileinformation.Length + 1);
                    int fileinformationI = fileinformation.Length - 1;

                    string[] splitter = Convert.ToString(fileinfo[i]).Split(new Char[] { '_', '.' });

                    fileinformation[fileinformationI] = Convert.ToString(fileinfo[i]);
                    fileInformationSplit[fileinformationI, 0] = splitter[0];
                    fileInformationSplit[fileinformationI, 1] = splitter[1];
                    
                }
            }

            PointInsertArray(fileinformation, fileInformationSplit);
        }

        private void PointInsertArray(string[] fileinformation, string[,] fileInformationSplit)
        {
            string[] fileinformationInsert = new string[0];
            string[] fileInformationNumberPointSplitInsert = new string[0];
            string[] fileInformationPolarisationPointSplitInsert = new string[0];

            for (int i =0; i< fileinformation.Length;i++)
            {
                if (Array.Exists(calibrationPointArray, p => p == Convert.ToInt32(fileInformationSplit[i,0])))
                {
                    //Массив названий файлов калибровки
                    Array.Resize(ref fileinformationInsert, fileinformationInsert.Length + 1);
                    fileinformationInsert[fileinformationInsert.Length - 1] = fileinformation[i];

                    //Массив номеров точек
                    Array.Resize(ref fileInformationNumberPointSplitInsert, fileInformationNumberPointSplitInsert.Length + 1);
                    fileInformationNumberPointSplitInsert[fileInformationNumberPointSplitInsert.Length - 1] = fileInformationSplit[i,0];

                    //Массив поляризаций точек
                    Array.Resize(ref fileInformationPolarisationPointSplitInsert, fileInformationPolarisationPointSplitInsert.Length + 1);
                    fileInformationPolarisationPointSplitInsert[fileInformationPolarisationPointSplitInsert.Length - 1] = fileInformationSplit[i, 1];
                }
            }

            //Массив с разобранными 
            string[,] fileInformationSplitInsert = new string[fileinformationInsert.Length, 2];

            //Обратная сборка массива сплиттера для названий файлов калибровки
            for (int t = 0; t < fileinformationInsert.Length; t++)
            {
                fileInformationSplitInsert[t, 0] = fileInformationNumberPointSplitInsert[t];
                fileInformationSplitInsert[t, 1] = fileInformationPolarisationPointSplitInsert[t];
            }

            StaticParametr.AttMinMax_FieldMinMax_FileCompilation = new object[fileinformationInsert.Length];
            StaticParametr.FileInformationName = fileinformationInsert;
            StaticParametr.FileInformationNameSplit = fileInformationSplitInsert;
        }
        #endregion

        #region Анализ файлов на наличие наименьшего и наибольшего значения аттенюатора

        #region Получение информации о исследуемых частотах
        private void UploadFqParametr()
        {
            double[] fqStepArray = new double[0];
            bool[] fqStepEnable = new bool[0];

            object[,] Fq = new object[1, 2];

            string patch = GetPatch();
            XDocument xdoc = XDocument.Load(@patch + "//" + "CalibrationTestFile.xml");
            XElement xelem = xdoc.Element("testParametrs");
            XElement xxelem = xelem.Element("fqStepValueStack");

            #region сепаратор для разделения строки по значению double
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            #endregion

            StaticParametr.FqStepArray = new double[0];
            StaticParametr.FqStepEnable = new bool[0];
            foreach (var node in xxelem.Elements())
            {
                Array.Resize(ref fqStepArray, fqStepArray.Length + 1);
                Array.Resize(ref fqStepEnable, fqStepEnable.Length + 1);
                fqStepArray[Convert.ToInt32(node.Attribute("step").Value)] = Convert.ToDouble(node.Element("Value").Value, provider);
                fqStepEnable[Convert.ToInt32(node.Attribute("step").Value)] = Convert.ToBoolean(node.Element("bool").Value);
            }

            StaticParametr.FqParametrArray = fqStepArray;
            StaticParametr.FqParamrtrArrayEnable = fqStepEnable;
        }
        #endregion

        #region Загрузка параметров в массивы параметров по определенным файлам
        private void FileCalibrationParametrUpdate()
        {
            for (int i =0; i< StaticParametr.FileInformationName.Length;i++)
            {
                FileParametrAnalistics(StaticParametr.FileInformationName[i]);
            }
        }
        #endregion

        #region Получение параметров файла калибровки, для каждой частоты
        private void FileParametrAnalistics(string fileName)
        {
            string patch = GetPatch();
            XDocument xdoc = XDocument.Load(@patch + "//" + fileName);
            XElement xelem = xdoc.Element("Parametr");
            XElement xxelem = xelem.Element("CalibrationParametr");

            #region сепаратор для разделения строки по значению double
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            #endregion


            #region Создаем массив с частотами для записи отдельных параметров
            object[] ParametrInFq = new object[StaticParametr.FqParametrArray.Length];
            #endregion

            foreach (var xx in xxelem.Elements())
            {
                //Перебираем параметры частоты на определенной поляризации
                if (xx.Attribute("PointStep").Value != "pointstep")
                {
                    int PointStep = Convert.ToInt32(xx.Attribute("PointStep").Value);

                    object[] ParamPamPam = new object[0];

                    //Перебираем параметры напряженности полученные на одной частоте
                    foreach (var cc in xx.Elements())
                    {
                        if (cc.Elements().Count() == 5)
                        {
                            // Если параметр напряженности удовлетворяет необходимому, получаем откалиброванные параметры аттенюатора и напряженности на этих значениях
                            if (Convert.ToInt32(cc.Attribute("step").Value) == tensStep)
                            {
                                int amax = Convert.ToInt32(cc.Element("AttMax").Value);
                                int amin = Convert.ToInt32(cc.Element("AttMin").Value);
                                double imax = Convert.ToDouble(cc.Element("TensMax").Value,provider);
                                double imin = Convert.ToDouble(cc.Element("TensMin").Value, provider);

                                ParamPamPam = new object[] { amax, amin, imax, imin};
                            }
                        }
                        else
                        {
                            //Таких пока случаев не предусмотренно
                        }
                    }
                    ParametrInFq[PointStep] = ParamPamPam;
                }
            }

            StaticParametr.AttMinMax_FieldMinMax_FileCompilation[Array.FindIndex(
                        StaticParametr.FileInformationName, p => p == fileName)] = ParametrInFq;
        }
        #endregion

        private void ParametrSerialise(object[] parametrFileCompare)
        {

        }
        #endregion
    }
}
