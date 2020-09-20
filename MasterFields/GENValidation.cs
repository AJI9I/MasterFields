using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MasterFields
{
    class GENValidation
    {
        #region функция перевода строки в числовое значение double
        private double DoubleProvider(string str)
        {
            #region сепаратор для разделения строки по значению double
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = ".";
            #endregion
            return Convert.ToDouble(str, provider);
        }
        #endregion

        #region Частотная валидация
        public int Fq(string str)
        {
            double fq = DoubleProvider(str);
            fq = fq * 1000D;
            int toInt = (int)fq;
            double ToZerro = fq - toInt;
            if (ToZerro == 0)
            {
                if (toInt >= 9 && toInt <= 1000000)
                {
                    return toInt;
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

        #region Валидация аттенюатора
        public int Att(string str)
        {
            double att = DoubleProvider(str);
            att = att * 10D;
            int toInt = (int)att;
            double toZerro = att - toInt;
            if (toZerro == 0)
            {
                if (0 <= toInt && toInt <= 635)
                {
                    int To5 = toInt / 5;
                    To5 = To5 * 5;
                    if (To5 == toInt)
                    {
                        return toInt;
                    }
                    else
                    {
                        return toInt * (-1);
                    }
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

        #region Валидация модуляции ModDeptAM
        public int ModDeptAM(string str)
        {
            double moddeptam = DoubleProvider(str);
            int toInt = (int)moddeptam;
            double Zerro = moddeptam - toInt;
            if (Zerro == 0)
            {
                if (0 <= toInt && toInt <= 99)
                {
                    return toInt;
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

        #region Валидация ModDutyCyc
        public int ModDutyCyc(string str)
        {
            double moddutycyc = DoubleProvider(str);
            int toInt = (int)moddutycyc;
            double Zerro = moddutycyc - toInt;
            if (Zerro == 0)
            {
                if (toInt == 0 || 10 <= toInt && toInt <= 80)
                {
                    return toInt;
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

        #region Вуалидация ModFAm
        public int ModFAm(string str)
        {
            double modfam = DoubleProvider(str);
            int toInt = (int)modfam;
            double Zerro = modfam - toInt;
            if (Zerro == 0)
            {
                if (1 <= toInt && toInt <= 3000)
                {
                    return toInt;
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

        #region Валидация ModFPm
        public int ModFPm(string str)
        {
            double modfpm = DoubleProvider(str);
            int toInt = (int)modfpm;
            double Zerro = modfpm - toInt;

            if (Zerro == 0)
            {
                if (1 <= toInt && toInt <= 1000)
                {
                    return toInt;
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

        #region Валидация Времени выдержки
        public int Time(string str)
        {
            double time = DoubleProvider(str);
            time = time * 10;
            int toInt = (int)time;

            double Zerro = time - toInt;
            if (Zerro == 0)
            {
                if (3 <= toInt && toInt <= 99999)
                {
                    return toInt;
                }
                else
                {
                    return toInt * (-1);
                }
            }
            else
            {
                return toInt * (-1);
            }
        }
        #endregion

    }
}
