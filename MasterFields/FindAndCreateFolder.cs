using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MasterFields
{
    class FindAndCreateFolder
    {
        // Создаем директорию
        public void opendirectiryCalibration(string directory)
        {
            Directory.CreateDirectory(GetCurrentDirectory() + "//" +directory);
        }

        // Проверяем есть ли деректория 
        public bool findDirectory(string directory)
        {
            return Directory.Exists(directory);
        }
        
        //Получаем текущую деректорию где лежит программа
        public string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        // Массивв каталогов с параметрами для загрузки
        public string[] FindCalibrationFileParametr()
        {
            DirectoryInfo fintParametrFile = new DirectoryInfo(@StaticParametr.CalibrationFolderName);
            var catalog = fintParametrFile.GetDirectories();
            string[] catalogName = new string[catalog.Count()];

            for (int y = 0; y < catalog.Count(); y++)
            {
                catalogName[y] = catalog[y].Name;
            }

            return catalogName;
        }

        public FileInfo[] FindFilesFromDirectory(string patch, string filename)
        {
            DirectoryInfo directoryinfo = new DirectoryInfo(patch);
            return directoryinfo.GetFiles(filename);
        }

        public FileInfo[] GetFilesFromDirectory(string patch)
        {
            DirectoryInfo directoryinfo = new DirectoryInfo(patch);
            return directoryinfo.GetFiles();
        }

    }
} 
