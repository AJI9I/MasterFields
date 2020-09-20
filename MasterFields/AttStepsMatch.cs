using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    class AttStepsMatch
    {
        // Возвращает округленное значение аттенюатора
        // Которое нужно установить
        public int NewAttSteps(double FieldTens, int AttStep)
        {
            int att = MatchAtt(FieldTens, AttStep);
            
            if (att > StaticParametr.AttParametrMax)
            {
                return 0;
            }
            if (StaticParametr.PointAttLasMax == att)
            {
                att = att - 5;
            }
            if (StaticParametr.PointAttLastMin == att)
            {
                att = att + 5;
            }
            return att;
        }

        private int MatchAtt(double FieldTens, int AttStep)
        {
            if (AttStep == 0)
            {
                return AttStep + 5;
            }
            else
            { 
                //Приближенное значение аттенюатора для максимальной напряженности
                //Если напряженность полученная в ходе испытания меньше нужной
                //Ищем значение максимальной точки значения аттенюатора
                if (FieldTens < StaticParametr.PointTensionDefolt)
                {
                    double step = Math.Ceiling(AttStepp(FieldTens, AttStep));
                    int steps = (int)step;
                    return steps * 5;
                }

                //Приближенное значение аттенюатора для минимальной напряженности
                //Если напряженность полученная в ходе испытания больше нужной
                //Ищем значение минимальной точки значения аттенюатора
                if (FieldTens > StaticParametr.PointTensionDefolt)
                {
                    int step = (int)AttStepp(FieldTens, AttStep);
                    return step * 5;
                }
                return -1;
            }
        }

        private double AttStepp(double FieldTens, int AttStep)
        {
            var d = StaticParametr.PointTensionDefolt / (FieldTens / 100.00);
            double s = AttStep / 100.00;
            var a = d * s;
            //var c = d * (AttStep / 100);
            return a/5;
        }
    }
}
