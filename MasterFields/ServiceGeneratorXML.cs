using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    class ServiceGeneratorXML
    {
        string[] FileNamePolarosation = new string[] {"Вертикальная", "Горизонтальная" };
        int[] FileNameNumber = new int[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 };

        XMLNewParametrFile xmlnewparametrfile;
        XMLNewCalibrationPoint xmlnewcalibrationpoint;
        public void GenerateXMLFile()
        {
            StaticParametr.FileParametrName = "test";
            Random randomizer = new Random();

            xmlnewparametrfile = new XMLNewParametrFile();
            xmlnewparametrfile.WHNewParametrFile(StaticParametr.FileParametrName, StaticParametr.FqMax, StaticParametr.FqMin, StaticParametr.Time, StaticParametr.Step, StaticParametr.StepParametr);

            xmlnewcalibrationpoint = new XMLNewCalibrationPoint();

            for (int i = 0; i < FileNamePolarosation.Length; i++)
            {
                for (int t = 0; t < FileNameNumber.Length; t++)
                {
                    StaticParametr.PointName = Convert.ToString(FileNameNumber[t]);
                    StaticParametr.PolarisationName = FileNamePolarosation[i];

                    xmlnewcalibrationpoint.CalibrationPointAddFile(Convert.ToString(FileNameNumber[t]), FileNamePolarosation[i]);

                    for (int o = 0; o<StaticParametr.FqStepArray.Length;o++)
                    {
                        #region Статические переменные частоы и ее порядкового номера из мессива частот
                        StaticParametr.PointFqDefolt = StaticParametr.FqStepArray[o];
                        StaticParametr.PointFqDefoltNumber = o;
                        #endregion

                        #region Создание блока для окалиброванных точек напряженности на определенной частоте
                        xmlnewcalibrationpoint.XMLCalibrationFqPointWrite();
                        #endregion

                        for (int y=0; y< StaticParametr.TensionParametr.Length;y++)
                        {
                            int g = randomizer.Next(4);
                            if (g == 0)
                            {
                                StaticParametr.PointAttMax = 600;
                            }
                            if (g == 1)
                            {
                                StaticParametr.PointAttMax = 500;
                            }
                            if (g == 2)
                            {
                                StaticParametr.PointAttMax = 400;
                            }
                            if (g == 3)
                            {
                                StaticParametr.PointAttMax = 300;
                            }

                            g = randomizer.Next(4);
                            if (g == 0)
                            {
                                StaticParametr.PointAttMin = 600;
                            }
                            if (g == 1)
                            {
                                StaticParametr.PointAttMin = 500;
                            }
                            if (g == 2)
                            {
                                StaticParametr.PointAttMin = 400;
                            }
                            if (g == 3)
                            {
                                StaticParametr.PointAttMin = 300;
                            }

                            StaticParametr.PointTensionDefoltNumber = y;

                            StaticParametr.PointTensMax = 3.123;
                            StaticParametr.PointTensMin = 3.00;
                            StaticParametr.PointTimeDelay = StaticParametr.Time;

                            xmlnewcalibrationpoint.XMLCalibrationInformationPointWrite();
                        }
                    }
                }
            }
        }
    }
}
