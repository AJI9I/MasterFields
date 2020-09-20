using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace MasterFields
{
    class GENPort
    {
        
        /// <summary>
        /// Порт отвечающий за соединение с генератором
        /// содержит несколько функцийй
        /// Открыть:
        /// Параметры задаваемые порту находятся в статическом классе с параметрами
        /// 
        ///Закрыть:
        /// без входных параметров
        /// 
        /// Чтение:
        /// Происходит чтение строки записанной в порт
        /// 
        /// Перед отправкой команды либо массива команд должно необходимо
        /// 1 - открыть порт
        /// 2 - выдержать время ожидания
        /// 3 - прочитать сообщение
        /// 4 - закрыть порт
        /// </summary>
        SerialPort serialport = new SerialPort();

        #region Открытие порта
        public bool OpenPort()
        {
            try
            {
                serialport.BaudRate = StaticParametr.GENBaudRate;
                serialport.DataBits = StaticParametr.GENDataBits;
                serialport.StopBits = StopBits.One;
                serialport.PortName = StaticParametr.GENPortName;
                serialport.DataReceived += new SerialDataReceivedEventHandler(serialportDataRecived);
                serialport.Open();
                return true;

            }
            catch
            {
                if (serialport.IsOpen)
                {
                    serialport.Close();
                }
                return false;
            }
        }
        #endregion

        #region Событие прихода сообщения в порт, устанавливает только флаг что что то пришло дальше нужно опрашивать этот флаг постоянно
        //Поток на приход сообщения вывести только на установку флага внешенму потоку на запрос
        // прихода сообщения в порт
        // внешним потоком производить чтение.
        void serialportDataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            StaticParametr.GENPortMessage = true;
        }
        #endregion


        #region Чтение порта
        public void PortReadWhile()
        {
            StaticParametr.GENPortMessageWhileRead = true;
            StaticParametr.GENPortReadMessage = new string[0];
            int i = 0;
            while (StaticParametr.GENPortMessageWhileRead)
            {
                Thread.Sleep(100);
                if (StaticParametr.GENPortMessage)
                {
                    
                    StaticParametr.GENPortReadMessage = ComandAnswerParser(PortRead());
                    StaticParametr.GENPortMessageWhileRead = false;
                    StaticParametr.GENPortMessage = false;
                }
                if (i == 10)
                {
                    StaticParametr.GENPortReadMessage = new string[] { "Нет ответа от генератора, проверьте подключение. Вышло вреемея ожидания 100 * 10 = 1000 мс" }; 
                        
                    StaticParametr.GENPortMessageWhileRead = false;
                }
                i++;
            }    
        }

        public string PortRead()
        {
            return serialport.ReadLine();
        }
        #endregion

        #region Чтение порта генератора на выход времени теста
        public string[] PortReadWhileVaitForProcess(int time)
        {
            StaticParametr.GENPortMessageWhileRead = true;
            string[] MessageGen = new string[0];
            int i = 0;
            int WeitTime = time * 100;
            while (StaticParametr.GENPortMessageWhileRead)
            {
                Thread.Sleep(100);
                if (StaticParametr.GENPortMessage)
                {

                    MessageGen = ComandAnswerParser(PortRead());
                    StaticParametr.GENPortMessageWhileRead = false;
                    StaticParametr.GENPortMessage = false;
                }
                if (i == WeitTime)
                {
                    MessageGen = new string[] { "Тишина" } ;
                    StaticParametr.GENPortMessageWhileRead = false;
                }
                i++;
            }
            return MessageGen;
        }
        #endregion
        #region Закрытие порта
        public bool PortClose()
        {
            try
            {
                serialport.Close();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion

        #region Запись в порт команды
        private void PortWrite(byte[] command)
        {
            Array.Resize(ref command, command.Length + 1);
            command[command.Length-1] = 0x0A;
            serialport.Write(command,0,command.Length);
        }
        #endregion

        public string Generator(string command)
        {
            RET:
            if (serialport.IsOpen)
            {
                Log(">g> "+command);
                PortWrite(Converter(command));
                return command;
            }
            else
            {
                OpenPort();
                goto RET;
            }
            //return "ошибка: public string Generator(string command)";

        }

        #region Конвертер команды в байты и добавлением контрольной суммы на выходе конечное сообщение
        public byte[] Converter(string msg)
        {
            ASCIIEncoding ask = new ASCIIEncoding();
            char[] msgChar = msg.ToCharArray();
            byte[] CommandByte = ask.GetBytes(msgChar);
            byte ControlSummASkIIByte = GetControlSumm(CommandByte);
            //byte[] LFByte = LfCommand(ask);
            return GetByteCommand(CommandByte, ControlSummASkIIByte);
        }

        private byte[] GetByteCommand(byte[] Command, byte ControlSymmByte)
        {
            byte[] CommandSendByte = new byte[Command.Length + 1];
            for (int i = 0; i < Command.Length; i++)
            {
                CommandSendByte[i] = Command[i];
            }

            CommandSendByte[Command.Length] = ControlSymmByte;
            return CommandSendByte;
        }

        private byte GetControlSumm(byte[] byteArr)
        {
            byte[] summ = { 0x00 };
            //byte[] ss = new byte[] { 0x4E, 0x55, 0x2C, 0x31, 0x38, 0x30, 0x3B };
            foreach (byte input in byteArr)
            {
                summ[0] += input;

            }
            byte[] summASCII = new byte[] { 0x00 };
            return (byte)(summASCII[0] - summ[0]);
        }
        #endregion

        #region Делегат и функция для записи лог файла
        LogFile logfile;
        public delegate void LOG(string msg);
        LOG log;
        private void LogDelegate(string msg)
        {
            log = new LOG(Log);
            Invoke(log, msg);
        }

        private void Invoke(LOG log, string msg)
        {
            log(msg);
        }

        private void Log(string msg)
        {
            logfile = new LogFile();
            logfile.LogWrite(msg);
        }
        #endregion

        #region Парсинг ответа генератора
        public string[] ComandAnswerParser(string msg)

        {
            msg = msg.Replace(" ", "");
            string[] commandParamert = msg.Split(new Char[] { ',', '<', '>', ';' });
            return commandParamert;
        }
        #endregion
    }
}
