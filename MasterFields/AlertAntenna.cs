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
    public partial class AlertAntenna : Form
    {
        public AlertAntenna()
        {
            InitializeComponent();
        }

        public void AntennNameLabel(string name)
        {
             label2.Text = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StaticParametr.AntennaSetup = true;
            StaticParametr.AntennaWhileThread = false;
            Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StaticParametr.AntennaSetup = false;
            StaticParametr.AntennaWhileThread = false;
            Dispose();
        }
    }
}
