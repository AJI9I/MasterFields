using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MasterFields
{
    class LogFile
    {
        public void LogWrite(string msg)
        {
            if (!StaticParametr.FileDirectory)
                DorectoryLogCreate();
            addLogParametr(msg);
        }

        private void DorectoryLogCreate()
        {
            try
            {
                if (!Directory.Exists(StaticParametr.LogFileDirectory))
                {
                    Directory.CreateDirectory(StaticParametr.LogFileDirectory);
                }

                StaticParametr.FileDirectory = true;
            }
            catch { }
        }

        private void addLogParametr(string msg)
        {
            try
            {

                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + StaticParametr.LogFileDirectory + "/" + StaticParametr.LogFileName + ".txt", Convert.ToString(msg) + Environment.NewLine);
            }
            catch
            { }
       }
    }
}
