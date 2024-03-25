using System;
using System.Windows.Forms;
using APV.TransControl.Core.Application;
using APV.TransControl.Core.DataLayer;

namespace APV.TransControl.ExportReport
{
    public partial class fmConnectionSettings : Form
    {
        public fmConnectionSettings()
        {
            InitializeComponent();
            cbDatabase.SelectedIndex = 0;
        }

        private void fmConnectionSettings_Load(object sender, EventArgs e)
        {
            ConnectionSettings connectionSettings = ContextManager.ConnectionSettings;
            tbUsername.Text = connectionSettings.Username;
            tbPassword.Text = connectionSettings.Password;
            tbHostname.Text = connectionSettings.Host;
            numPortNumber.Value = connectionSettings.Port;
            cbUseTnsOra.Checked = connectionSettings.UseTnsDBName;
            cbDatabase.Text = connectionSettings.DBName;
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            Cursor cur = Cursor;
            Cursor = Cursors.WaitCursor;
            string err;

            var connectionSettings = new ConnectionSettings
                {
                    Username = tbUsername.Text,
                    Password = tbPassword.Text,
                    Host = tbHostname.Text,
                    Port = decimal.ToInt32(numPortNumber.Value),
                    UseTnsDBName = cbUseTnsOra.Checked,
                    DBName = cbDatabase.Text,
                };

            ContextManager.ConnectionSettings = connectionSettings;

            if (DbManager.CheckConnection(out err))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(err, "Невозможно установить соединение с БД.", MessageBoxButtons.OK);
                //Close();
            }
            Cursor = cur;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cbUseTnsOra_CheckedChanged(object sender, EventArgs e)
        {
            tbHostname.Enabled = !cbUseTnsOra.Checked;
            numPortNumber.Enabled = !cbUseTnsOra.Checked;
        }
    }
}