using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    class GENCommand
    {
        /// <summary>
        /// Комманды для отправки сообщний генератору
        /// Комманды имеют входные значения изменяемых параметров возвращают ответ полностью комманду
        /// </summary>
        #region Сервисные функции
        public string GeneratorBWCommand()
        {
            return "BW;";
        }

        public string GeneratorBSCommand(string block)
        {
            return "BS," + block + ";";
        }

        public string GeneratorOnCommand()
        {
            return "on;";
        }

        public string GeneratorStartCommand()
        {
            return "start;";
        }

        private string GeneratorStopCommand()
        {
            return "stop" + ";";
        }
        #endregion

        #region inicialization
        public string GeneratorRSTCommand()
        {
            return "*rst" + ";";
        }

        public string GeneratorGTLCommant()
        {
            return "gtl;";
        }

        public string GeneratorCcCommand()
        {
            return "CC;";
        }

        public string GeneratorDispStringCommand(string msg)
        {
            return "disp:str " + msg + ";";
        }
        #endregion

        #region параметры
        public string GeneratorIdentification()
        {
            return "*idn?";
        }

        public string GeneratorSetFq(double fq)
        {
            fq = fq * 1000;
            return "gen:frq " + fq + ";";
        }

        public string GeneratorSetAtt(int att)
        {
            return "gen:att " + att + ";";
        }

        public string GeneratorTimeDelay(int time)
        {
            return "gen:time " + time + ";";
        }

        private string GeneratorModFAm(int modfam)
        {
            return "gen:mod:frq:am " + modfam + ";";
        }

        private string GeneratorModFPm(int modfpm)
        {
            return "gen:mod:frq:pm1 " + modfpm + ";";
        }

        private string GeneratorModDeptAm(int moddeptam)
        {
            return "gen:mod:prm:am " + moddeptam + ";";
        }
        private string GeneratorModDutyCycPM(int moddutycycpm)
        {
            return "gen:mod:prm:pm1 " + moddutycycpm + ";";
        }

        private string GeneratorContCommand(string cont)
        {
            return "gen:cont " + cont + ";";
        }
        #endregion


    }
}
