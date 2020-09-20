using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    class PRICommandReader
    {

        #region Формирование пакета для изменения частоты
        public byte[] PRIFqCommandGeneration(double Fq)
        {
            //Заменяются набор байт  5,6,7
            byte[] StandartCommandMassive = new byte[] { 75,0,25,0,3,88,2,0,126,37,0,188,138,169,53,66,15,52,115,203,112,103,220,16,92,237,76,80,254 }; 

            int UpdateFq = Convert.ToInt32(Fq * 100.00);

            byte[] GetByteFq = BitConverter.GetBytes(UpdateFq);

            Array.ConstrainedCopy(GetByteFq, 0, StandartCommandMassive, 5, 3);

            byte[] ChekSumm = ChekSummGet(StandartCommandMassive);

            Array.Resize(ref StandartCommandMassive, StandartCommandMassive.Length + 2);

            Array.ConstrainedCopy(ChekSumm, 0, StandartCommandMassive, StandartCommandMassive.Length - 3, 2);

            return StandartCommandMassive;
        }
        #endregion

        #region Расчет контрольной суммы
        private byte[] ChekSummGet(byte[] command)
        {
            var sum = BitConverter.GetBytes(crc(command));
            return sum;
        }

        static ushort crc(byte[] data, ushort sum = 0)
        {
            foreach (byte b in data) sum += b;
            return sum;
        }
        #endregion
    }
}
