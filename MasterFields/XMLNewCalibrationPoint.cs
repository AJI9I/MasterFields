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
    class XMLNewCalibrationPoint
    {
        FindAndCreateFolder findandcreatefilder;

        #region Получение информации и пути к файлу и имени файла
        private string GetFileName()
        {
            return StaticParametr.PointName+"_"+StaticParametr.PolarisationName+".xml";
        }

        private string GetPatch()
        {
            return StaticParametr.CalibrationFolderName + "//" + StaticParametr.FileParametrName + "//";
        }
        #endregion

        #region Создание общего файла хранения откалиброванной информации
        public void CalibrationPointAddFile(string pointname, string polarisation)
        {
            findFilePoint();
        }

        private void findFilePoint()
        {
            findandcreatefilder = new FindAndCreateFolder();
            string FileName = GetFileName();
            string patch = findandcreatefilder.GetCurrentDirectory() + "//" + StaticParametr.CalibrationFolderName+"//"+StaticParametr.FileParametrName;
            FileInfo[] fi = findandcreatefilder.FindFilesFromDirectory(patch, FileName);
            if (fi.Length != 0)
            {
                XMLCalibrationPointLoad(FileName);
                // ошибка в контейнере поиска файлов нашлось больше нуля файлов по полному имени файла
                return;
            }

            // Создание нового файла XML
            XMLFileParametrCalibrationPoint(FileName);
        }

        #region Создание файла информации откалиброванной частоты
        private void XMLFileParametrCalibrationPoint(string FileName)
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Этот фаил содержит информацию о откалиброванной точке частоты в выбранной точке поля в определенной поляризации антенны"),
                    new XElement("Parametr",
                    new XComment("Параметры файла с информацией"),
                    new XElement("FolderName", StaticParametr.CalibrationFolderName),
                    new XElement("FileName", StaticParametr.FileParametrName),
                    new XElement("PointName", StaticParametr.PointName),
                    new XElement("PolarisationName", StaticParametr.PolarisationName),
                    new XElement("MassiveFqSize", StaticParametr.FqStepArray.Length),
                    new XElement("MassiveFqEnableSize", StaticParametr.FqStepEnable.Length),

                        new XElement("CalibrationParametr",
                            new XComment("Информация об откалиброванных точках"),
                            new XElement("PointInformation",
                            //Номер частоты из массива частот
                            new XAttribute("PointStep","pointstep"),
                            //Частота из массива частот
                            new XAttribute("Fq", "fq"),
                                new XElement("TensionStep", 
                                    new XAttribute("step","tensionstep"),
                                    new XElement("AttMax","attmax"),
                                    new XElement("AttMin", "attmin"),
                                    new XElement("TensMax", "tensmax"),
                                    new XElement("TensMin", "tensmin"),
                                    new XElement("Time", "time"))
                    )
                    )
                    ));
            xdoc.Save(@GetPatch() + "//" + FileName);

            StaticParametr.FqStepArrayI = 0;
            StaticParametr.TensionParametrI = 0;
        }
        #endregion
        #endregion

        #region Обновление информации об откалиброванной точке

        public void XMLCalibrationInformationPointWrite()
        {
            string patch = GetPatch();
            string FileName = GetFileName();
            //XmlDocument XmlDoc = new XmlDocument();
            //XmlDoc.Load(@patch+"//"+FileName);
            //XmlNode node = XmlDoc.SelectSingleNode("Parametr/CalibrationParametr");

            XDocument xdoc = XDocument.Load(@patch + "//" + FileName);
            XElement xelem = xdoc.Element("Parametr");
            XElement xxelem = xelem.Element("CalibrationParametr");
            foreach (XElement xel in xxelem.Elements())
            {
                if (xel.Attribute("PointStep").Value == Convert.ToString(StaticParametr.PointFqDefoltNumber))
                 {
                    string ss = Convert.ToString(StaticParametr.PointFqDefolt).Replace(',','.');
                    string cc = Convert.ToString(xel.Attribute("Fq").Value);
                    if (cc == ss)
                    {
                        XElement xxel = xel;
                        xxel.Add(new XElement("TensionStep",
                                    new XAttribute("step", StaticParametr.PointTensionDefoltNumber),
                                    new XElement("AttMax", StaticParametr.PointAttMax),
                                    new XElement("AttMin", StaticParametr.PointAttMin),
                                    new XElement("TensMax", StaticParametr.PointTensMax),
                                    new XElement("TensMin", StaticParametr.PointTensMin),
                                    new XElement("Time", StaticParametr.PointTimeDelay))
                            );
                        xdoc.Save(@patch + "//" + FileName);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Создание блока для записи откалиброванных точек на определенной частоте
        public void XMLCalibrationFqPointWrite()
        {
            string patch = GetPatch();
            string FileName = GetFileName();

            XDocument xdoc = XDocument.Load(@patch + "//" + FileName);
            XElement xelem = xdoc.Element("Parametr");
            XElement xxelem = xelem.Element("CalibrationParametr");
            if (!FindNodePointInformation(xxelem))
            {
                xxelem.Add(new XElement("PointInformation",
                                //Номер частоты из массива частот
                                new XAttribute("PointStep", StaticParametr.PointFqDefoltNumber),
                                //Частота из массива частот
                                new XAttribute("Fq", StaticParametr.PointFqDefolt))
                    );
                xdoc.Save(@patch + "//" + FileName);
            }
        }

        private bool FindNodePointInformation(XElement xxelem)
        {
            bool param = false;
            foreach (XElement xel in xxelem.Elements())
            {
                if (xel.Attribute("PointStep").Value == Convert.ToString(StaticParametr.PointFqDefoltNumber))
                {
                    string ss = Convert.ToString(StaticParametr.PointFqDefolt).Replace(',', '.');
                    string cc = Convert.ToString(xel.Attribute("Fq").Value);
                    if (cc == ss)
                    {
                        param = true;
                    }
                    param = true;
                }
                else
                { 
                param = false;
                }
            }
            return param;
        }
        #endregion

        #region Продолжение калибровки выбранной точки
        ///Продолжение калибровки выбранной точки
        ///происходит загрузка данных с найденного файла 
        ///и продолжение калибровки с начатой позиции

        public void XMLCalibrationPointLoad(string FileName)
        {
            int tensStep = 60;
            string patch = GetPatch();
            XDocument xdoc = XDocument.Load(@patch + "//" + FileName);
            XElement xelem = xdoc.Element("Parametr");
            XElement xxelem = xelem.Element("CalibrationParametr");
            if ((XElement)xxelem.LastNode != null)
            {
                XElement xnode = (XElement)xxelem.LastNode;
                if(xnode.Attribute("PointStep").Value != "pointstep")
                    { 
                        StaticParametr.FqStepArrayI = Convert.ToInt32(xnode.Attribute("PointStep").Value);
                        if ((XElement)xnode.LastNode != null)
                        {
                            XElement xxnode = (XElement)xnode.LastNode;
                            tensStep = Convert.ToInt32(xxnode.Attribute("step").Value);
                            TensStepVerificate(tensStep);
                        }
                        else                   
                        {
                            StaticParametr.TensionParametrI = 0;
                        }
                    }
            }
            else
            {
                StaticParametr.FqStepArrayI = 0;
                StaticParametr.TensionParametrI = 0;
            }

            // Переход к считыванию полученных значений при предыдущих запусках процесса калибровки
            // Добвленно 02.05.17
            FindPointAttTension(xxelem);
        }

        private void TensStepVerificate(int TensStep)
        {
            if (TensStep >= StaticParametr.TensionParametr.Length-1)
            {
                
                StaticParametr.FqStepArrayI = StaticParametr.FqStepArrayI + 1;
                StaticParametr.TensionParametrI = 0;
                if (StaticParametr.FqStepArrayI > StaticParametr.FqStepArray.Length-1)
                {
                    StaticParametr.FqStepArrayI = StaticParametr.FqStepArray.Length - 1;
                    StaticParametr.TensionParametrI = StaticParametr.TensionParametr.Length - 1;
                }

            }
            else
            {
                StaticParametr.TensionParametrI = TensStep+1;
            }
        }
        #endregion

        #region Определение полученных значений для вывода в область калибровки
        //Отслеживание полученных значений, при продолжении калибровки после прерывания
        private void FindPointAttTension(XElement xxelem)
        {
            #region сепаратор для разделения строки по значению double
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            #endregion
            foreach (var xx in xxelem.Elements())
            {
                if (xx.Attribute("PointStep").Value != "pointstep")
                {
                    Array.Resize(ref StaticParametr.PriviousLoaderFq, StaticParametr.PriviousLoaderFq.Length + 1);
                    StaticParametr.PriviousLoaderFq[StaticParametr.PriviousLoaderFq.Length - 1] = Convert.ToDouble(xx.Attribute("Fq").Value, provider);

                    Array.Resize(ref StaticParametr.PriviousLoaderAttObj, StaticParametr.PriviousLoaderAttObj.Length + 1);
                    Array.Resize(ref StaticParametr.PriviousLoaderTensObj, StaticParametr.PriviousLoaderTensObj.Length + 1);
                    Array.Resize(ref StaticParametr.PriviousLoaderStepTens, StaticParametr.PriviousLoaderStepTens.Length + 1);


                    int[,] att = new int[xx.Elements().Count(), 2];
                    double[,] tens = new double[xx.Elements().Count(), 2];
                    int[] steps = new int[xx.Elements().Count()];

                    int i = 0;

                    foreach (var cc in xx.Elements())
                    {
                        if (cc.Elements().Count() == 5)
                        {
                            steps[i] = Convert.ToInt32(cc.Attribute("step").Value);
                            att[i, 0] = Convert.ToInt32(cc.Element("AttMax").Value);
                            att[i, 1] = Convert.ToInt32(cc.Element("AttMin").Value);
                            tens[i, 0] = Convert.ToDouble(cc.Element("TensMax").Value, provider);
                            tens[i, 1] = Convert.ToDouble(cc.Element("TensMin").Value, provider);
                            i++;
                        }
                        else
                        {
                            //Таких пока случаев не предусмотренно
                        }
                    }
                    StaticParametr.PriviousLoaderAttObj[StaticParametr.PriviousLoaderAttObj.Length - 1] = att;
                    StaticParametr.PriviousLoaderTensObj[StaticParametr.PriviousLoaderTensObj.Length - 1] = tens;
                    StaticParametr.PriviousLoaderStepTens[StaticParametr.PriviousLoaderStepTens.Length - 1] = steps;
                }
            }
        }
        #endregion
    }
}
