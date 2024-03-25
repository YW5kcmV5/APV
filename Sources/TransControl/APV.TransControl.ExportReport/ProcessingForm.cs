using System;
using System.Windows.Forms;

namespace APV.TransControl.ExportReport
{
    public partial class ProcessingForm : Form
    {
        public ProcessingForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value < progressBar.Maximum)
            {
                progressBar.Value++;
            }
            if(Program.Manager.ReportFinished)
            {
                Close();
            }
        }
    }
}