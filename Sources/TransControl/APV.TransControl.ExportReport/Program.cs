using System;
using System.Windows.Forms;

namespace APV.TransControl.ExportReport
{
    static class Program
    {
        public static ReportManager Manager = new ReportManager();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fmLoading());
            if (Manager.IsValidConnection && Manager.Inited)
            {
                Application.Run(new MainForm());
            }
        }
    }
}