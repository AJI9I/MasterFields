using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterFields
{
    public partial class CalibrationCancelationForm : Form
    {
        public CalibrationCancelationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StaticParametr.CalibrationCancellWhile = false;
            Dispose();
        }
    }
}
