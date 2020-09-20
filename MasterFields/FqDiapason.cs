using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    class FqDiapason
    {
        /// <summary>
        /// Расчет параметров шага для исследования и калибровки напряженности
        /// 
        /// </summary>
        /// <param name="fqMax">Максимальное значение частоты</param>
        /// <param name="fqMin">Минимальное значение частоты</param>
        /// <param name="Step">Шаг с которым необходимо производить действия</param>
        /// <param name="StepBool">Параметр для опредеоения как расчитывать шаг вычислений
        /// если параметр равен False - Вычисления проводятся в процентах
        /// если параметр равен true  - Вычисления проводятся в КГц(Данный параметр определяется ниже в переменной Mnogitel) </param>
        /// <returns>если все произшло без ошибок приходит true, с ошибкой False</returns>
        /// После расчета расчитанные параметры записываются в два массива 
        /// в сстатичном классе
        /// StaticParametr.FqStepArray - числовое знаечение хранит число в формате double.
        /// StaticParametr.FqStepEnable - булево значение хранит информацию о том используется ли эта частота или нет.
        public bool WHStepsCalculation(double fqMax, double fqMin, double Step, bool StepBool)
        {
            return StepsCalculation(fqMax, fqMin, Step, StepBool);
        }

        int Mnogitel = 1000;
        private bool StepsCalculation(double fqMax, double fqMin, double Step, bool StepBool)
        {
            double[] massiveFq = new double[0];
            bool[] massiveGqEnable = new bool[0];
            // идет расчет шага про процентах
            if (StepBool == false)
            {
                bool WhiileStoped = true;
                while (WhiileStoped)
                {
                    double last = 0;
                    if(massiveFq.Length != 0)
                    last = massiveFq.Last();
                    Array.Resize(ref massiveFq, massiveFq.Length + 1);
                    Array.Resize(ref massiveGqEnable, massiveGqEnable.Length + 1);
                    double result = PercentRaschet(last, Step, massiveFq.Length);
                    massiveFq[massiveFq.Length - 1] = Math.Round(result, 2);
                    massiveGqEnable[massiveGqEnable.Length - 1] = true;
                    if (massiveFq.Last() >= fqMax)
                    {
                        WhiileStoped = false;
                        Array.Resize(ref massiveFq, massiveFq.Length + 1);
                        Array.Resize(ref massiveGqEnable, massiveGqEnable.Length + 1);
                        massiveFq[massiveFq.Length - 1] = StaticParametr.FqMax;
                        massiveGqEnable[massiveGqEnable.Length - 1] = true;

                    }
                }
            }

            //идет расчет шага в шагах ГЦ
            if (StepBool == true)
            {
                bool WhiileStoped = true;
                while (WhiileStoped)
                {
                    double last = 0;
                    if (massiveFq.Length != 0)
                        last = massiveFq.Last();
                    Array.Resize(ref massiveFq, massiveFq.Length + 1);
                    Array.Resize(ref massiveGqEnable, massiveGqEnable.Length + 1);
                    double result = GercRaschet(last, Step, massiveFq.Length);
                    massiveFq[massiveFq.Length - 1] = Math.Round(result, 2);
                    massiveGqEnable[massiveGqEnable.Length - 1] = true;
                    if (massiveFq.Last() >= fqMax)
                    {
                        WhiileStoped = false;
                        Array.Resize(ref massiveFq, massiveFq.Length + 1);
                        Array.Resize(ref massiveGqEnable, massiveGqEnable.Length + 1);
                        massiveFq[massiveFq.Length - 1] = StaticParametr.FqMax;
                        massiveGqEnable[massiveGqEnable.Length - 1] = true;
                    }
                }

            }
            StaticParametr.FqStepArray = massiveFq;
            StaticParametr.FqStepEnable = massiveGqEnable;
            return true;
        }

        private double PercentRaschet(double lastMassiveElement, double step,int lenght)
        {
            if (lenght != 1)
            {
                return lastMassiveElement + (lastMassiveElement * (step / 100));
            }
            else
            {
                return StaticParametr.FqMin + (StaticParametr.FqMin * (step / 100));
            }
        }

        private double GercRaschet(double lastMassiveElement, double step, int lenght)
        {
            if (lenght != 1)
            {
                return lastMassiveElement + (step);
            }
            else
            {
                return StaticParametr.FqMin;
            }
        }










































        //private bool StepsCalculation(double fqMax, double fqMin, double Step, bool StepBool)
        //{
        //    try
        //    {




        //        double RoundfqMassStep;
        //        object[] Array = MassSizeFqStep(fqMax, fqMin, Step, StepBool);

        //        double[] fqSteps = new double[Convert.ToInt64(Array[1])];
        //        bool[] fqStepsEnable = new bool[Convert.ToInt64(Array[1])];

        //        object[] Massive = (object[])GetCountFqArr((Int64)Array[1]);

        //        for (int i = 0; i < (Int64)Array[1]; i++)
        //        {
        //            RoundfqMassStep = fqMin + i * (Double)Array[0];
        //            fqSteps[i] = Math.Round(RoundfqMassStep, 2);
        //            fqStepsEnable[i] = true;
        //        }
        //        StaticParametr.FqStepArray = fqSteps;
        //        StaticParametr.FqStepEnable = fqStepsEnable;
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private void FqMassiveAdd(double fqMax, double fqMin, object)
        //{
        //    return;
        //}

        private object[] MassSizeFqStep(double fqMax, double fqMin, double Step, bool StepBool)
        {
            //Распределение
            //0 - Шаг с которым необходимо расчитывать диапазон
            //1 - Размер массива частот
            object[] array = new object[3];
            if (StepBool)
            {
                array[1] = Convert.ToInt64((fqMax - fqMin) / Step);
                array[0] = Step;
                array[3] = GetCountFqArr((Int64)array[1]);
                return array;
            }
            else
            {
                array[0] = Convert.ToDouble((fqMax - fqMin) / 100 * Step);
                array[1] = Convert.ToInt64((fqMax - fqMin) / (double)array[0]);
                
                return array;
            }

        }

        private object GetCountFqArr(Int64 ArraySize)
        {
            // 0 - Количество массивов по 1000
            // 1 - Количество элементов в последнем массиве

            object ArrayInformation;
            Int64 CountArray = 0;

            CountArray = ArraySize / 1000;
            Int64 arr = CountArray * 1000;
            Int64 arrr = ArraySize - arr;
            if (arrr != 0)
            {
                CountArray += 1;
               return ArrayInformation = new object[] { CountArray, arrr };
            }
            else
            {
                return ArrayInformation = new object[] { CountArray, arr };
            }
        }

    }
}
