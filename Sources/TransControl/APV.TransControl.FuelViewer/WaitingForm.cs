using System;
using System.Threading;
using System.Windows.Forms;

namespace APV.TransControl.FuelViewer
{
    public interface IWaitingFormView
    {
        GroupBox GroupBoxWaiting { get; }
        ProgressBar ProgressBar { get; }
        void Show(string message, Action action);
        void Close(DialogResult dialogResult = DialogResult.Cancel);
    }

    public partial class WaitingForm : Form, IWaitingFormView
    {
        private Action _action;

        public WaitingForm()
        {
            InitializeComponent();
        }

        #region IWaitingFormView

        GroupBox IWaitingFormView.GroupBoxWaiting { get { return GroupBoxWaiting; } }

        ProgressBar IWaitingFormView.ProgressBar { get { return ProgressBar; } }

        void IWaitingFormView.Show(string message, Action action)
        {
            ProgressBar.Value = 1;
            _action = action;
            GroupBoxWaiting.Text = message;
            ShowDialog();
        }

        void IWaitingFormView.Close(DialogResult dialogResult)
        {
            Invoke(new MethodInvoker(delegate { DialogResult = dialogResult; }));
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProgressBar.Invoke(new MethodInvoker(delegate { ProgressBar.Value = (ProgressBar.Value == 100) ? 0 : ProgressBar.Value + 1; }));
        }

        private void WaitingForm_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            if (_action != null)
            {
                ThreadPool.QueueUserWorkItem(instance =>
                    {
                        try
                        {
                            _action();
                        }
                        finally
                        {
                            if (Visible)
                            {
                                try
                                {
                                    Invoke(new MethodInvoker(Close));
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                );
            }
        }

        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
        }
    }
}
