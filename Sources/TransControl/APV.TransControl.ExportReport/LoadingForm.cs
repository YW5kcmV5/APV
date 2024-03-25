using System;
using System.Windows.Forms;
using System.Threading;

namespace APV.TransControl.ExportReport
{
    public partial class fmLoading : Form
    {
        private bool _fisrtCall;

        public fmLoading()
        {
            InitializeComponent();
            _fisrtCall = true;
           timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = Program.Manager.InitState;
            lbState.Text = Program.Manager.InitStateDescription;

            if (_fisrtCall)
            {
                _fisrtCall = false;
                try
                {
                    string err;
                    if (Program.Manager.CheckConnection(out err))
                    {
                        Program.Manager.Init();
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно установить соединение с БД.", MessageBoxButtons.OK);
                        var fmSettings = new fmConnectionSettings();
                        Visible = false;
                        if (fmSettings.ShowDialog() == DialogResult.OK)
                        {
                            Visible = true;
                            Program.Manager.Init();
                        }
                        else
                        {
                            Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ReportManager.ExceptionToStr(ex), "Исключение. Невозможно установить соединение с БД.", MessageBoxButtons.OK);
                    Close();
                }
            }

            if (Program.Manager.IsInitFinished)
            {
                Thread.Sleep(300);
                timer1.Enabled = false;
                Close();
            }
        }
    }
}