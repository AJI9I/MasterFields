using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterFields
{
    static class StaticFormUserControl
    {
        #region информация  юзер контролах на форме что на ней присутствуют

        #region Стартовые позиции элементов формы для работы Location x:y
        public static int LocationX = 119;
        public static int LocationY = 85;
        #endregion


        #region Новый фаил с параметрами
        static public bool UCNewFileParametrVisible = false;
        static public int UCNewFileParametrWidth = 670;
        static public int UCNewFileParametrHeight = 420;
        #endregion

        #region UCCalibrationPoint Панель выбора точки калибровки
        static public bool UCCalibrationPointVisible = false;
        static public int UCCalibrationPointWidth = 670;
        static public int UCCalibrationPointHeight = 420;
        #endregion

        #region UCEditFqDiapazon Диапазон исследуемых частот
        static public bool UCEditFqDiapazonVisible = false;
        static public int UCEditFqDiapazonWidth = 670;
        static public int UCEditFqDiapazonHeight = 420;
        #endregion

        #region UCProcessCalibration Процесс калибровки
        static public bool UCProcessCalibrationVisible = false;
        static public int UCProcessCalibrationWidth = 670;
        static public int UCProcessCalibrationHeight = 420;
        #endregion

        #region UCProcessCalibration Стартовая страница
        static public bool UCStartPageVisible = false;
        static public int UCStartPageWidth = 670;
        static public int UCStartPageHeight = 420;
        #endregion

        #region UCCalibrationPoint Панель выбора точки калибровки
        static public bool UCStudyProcessVisible = false;
        static public int UCStudyProcessWidth = 797;
        static public int UCStudyProcessHeight = 699;
        #endregion

        #endregion

        #region Информация о кнопках

        public static int ButtonNoVisibleLocation = -400;

        #region Информация о кнопках видны ли они пользователю или нет
        public static bool Button6Visible = false;
        #endregion
        #endregion
    }
}
