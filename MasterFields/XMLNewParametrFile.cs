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
    class XMLNewParametrFile
    {
        FindAndCreateFolder findandcreatefolder;
        /// <summary>
        /// Входная функция для создания нового файла
        /// Присылает положительный ответ если не чего плохого не произошло
        /// </summary>
        /// <returns></returns>
        public bool WHNewParametrFile(string fileName, double FqMax, double FqMin, int Time, double Step, bool StepParametr)
        {
            if (CreateFilder())
            {

                XMLFileParametrSetParametr(fileName, FqMax, FqMin, Time, Step, StepParametr);
                return true;
            }
            else
            {
                //Произошла проблема в создании директории джля 
                return false;
            }
        }

        #region Создание папки для храние калибровофчных параметров и сразу ее проверка в слуае удачного созджания приходит положительный ответ
        private bool CreateFilder()
        {
            string patch = StaticParametr.CalibrationFolderName + "//" + StaticParametr.FileParametrName;
            findandcreatefolder = new FindAndCreateFolder();
            findandcreatefolder.opendirectiryCalibration(patch);
            return findandcreatefolder.findDirectory(patch);

        }
        #endregion

        #region Запись парметров в фаил калибровки
        private void XMLFileParametrSetParametr(string fileName, double FqMax, double FqMin, int Time, double Step, bool StepParametr)
        {
            XDocument xdoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Этот фаил содержит головные настройки испытания и калибровки"),
                    new XElement("testParametrs",
                        new XElement("ValueTestParametrs",
                        //Редактировать сохранение параметров напряженности
                        new XElement("tensionParametr",
                        new XElement ("ne","sdf"),
                        new XElement ("fds","sdf")),
                        new XElement("fqMax", FqMax),
                        new XElement("fqMin", FqMin),
                        new XElement("fqStep",
                            new XElement("value", Step),
                            new XElement("param", StepParametr)),
                        new XElement("curingTime", Time)),
                        new XElement("studiespoint",
                            new XElement("count", "0")),
                    new XElement("fqStepValueStack",
                        new XElement("count", "0"),
                        new XElement("fqStep",
                        new XAttribute("step", "0"),
                        new XElement("Value", 1),
                        new XElement("bool", 1)
                                    )
                    )));

            XElement TensParametrXElement = xdoc.Element("testParametrs");
            XElement TensTens = TensParametrXElement.Element("ValueTestParametrs");
            XElement TensStackXElement = TensTens.Element("tensionParametr");
            TensStackXElement.RemoveAll();

            for (int x = 0; x < StaticParametr.TensionParametr.Length; x++)
            {
                TensStackXElement.Add(new XElement("tension",
                    new XAttribute("step", x),
                    StaticParametr.TensionParametr[x]));
            }

            XElement testParametrsXelem = xdoc.Element("testParametrs");
            XElement fqStackXelem = testParametrsXelem.Element("fqStepValueStack");
            fqStackXelem.RemoveNodes();

            for (int a = 0; a < StaticParametr.FqStepArray.Length; a++)
            {
                fqStackXelem.Add(new XElement("fqStep",
                                    new XAttribute("step", a),
                                    new XElement("Value", StaticParametr.FqStepArray[a]),
                                    new XElement("bool", StaticParametr.FqStepEnable[a])
                    ));


            }
            xdoc.Save(@StaticParametr.CalibrationFolderName + "//" + fileName + "//"+StaticParametr.CalibrationParametrFile);
        }
        #endregion

        #region Чтение файла параметров
        public bool XMLFileParametrGetParametr(string foldername, string currentDirectory)
        {
            //try
            //{
            #region сепаратор для разделения строки по значению double
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            #endregion

            XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(@StaticParametr.CalibrationFolderName + "\\"+ foldername + "\\" + StaticParametr.CalibrationParametrFile);
                XmlNode t = XmlDoc.SelectSingleNode("testParametrs/ValueTestParametrs");
                StaticParametr.FqMax = Convert.ToDouble(t.SelectSingleNode("fqMax").InnerText, provider); // Частота максимальная
                StaticParametr.FqMin = Convert.ToDouble(t.SelectSingleNode("fqMin").InnerText, provider); // Частота минимальная
                StaticParametr.Time = Convert.ToInt32(t.SelectSingleNode("curingTime").InnerText); // Время выдержки на частоте
                                                                                                   //Parametr._Tension = (string)t.SelectSingleNode("tension").InnerText; // Напряженность поля
                StaticParametr.Step = Convert.ToDouble(t.SelectSingleNode("fqStep").SelectSingleNode("value").InnerText); // Значение шага для перебора частоты
                StaticParametr.StepParametr = Convert.ToBoolean(t.SelectSingleNode("fqStep").SelectSingleNode("param").InnerText); // Значение параметра для частоты в процентах = 1, в Мг = 0

                

                XmlNode ttens = XmlDoc.SelectSingleNode("testParametrs/ValueTestParametrs/tensionParametr");

                StaticParametr.TensionParametr = new double[0];
                foreach (XmlNode node in ttens.ChildNodes)
                {
                    Array.Resize(ref StaticParametr.TensionParametr, StaticParametr.TensionParametr.Length + 1);
                    StaticParametr.TensionParametr[Convert.ToInt32(node.Attributes.GetNamedItem("step").Value)] = Convert.ToDouble(node.InnerText, provider);

                }


                XmlNode sstep = XmlDoc.SelectSingleNode("testParametrs/fqStepValueStack");

                
                StaticParametr.FqStepArray = new double[0];
                StaticParametr.FqStepEnable = new bool[0];
                foreach (XmlNode node in sstep.ChildNodes)
                {
                    Array.Resize(ref StaticParametr.FqStepArray, StaticParametr.FqStepArray.Length + 1);
                    Array.Resize(ref StaticParametr.FqStepEnable, StaticParametr.FqStepEnable.Length + 1);
                    StaticParametr.FqStepArray[Convert.ToInt32(node.Attributes.GetNamedItem("step").Value)] = Convert.ToDouble(node.SelectSingleNode("Value").InnerText, provider);
                    StaticParametr.FqStepEnable[Convert.ToInt32(node.Attributes.GetNamedItem("step").Value)] = Convert.ToBoolean(node.SelectSingleNode("bool").InnerText);
                }
                StaticParametr.FileLoad = true;
                return true;
            //}
            //catch { return false; }
        }
        #endregion
    }
}
