namespace APV.TransControl.FuelViewer
{
    partial class WaitingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GroupBoxWaiting = new System.Windows.Forms.GroupBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.GroupBoxWaiting.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBoxWaiting
            // 
            this.GroupBoxWaiting.BackColor = System.Drawing.Color.Transparent;
            this.GroupBoxWaiting.Controls.Add(this.ProgressBar);
            this.GroupBoxWaiting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBoxWaiting.Location = new System.Drawing.Point(0, 0);
            this.GroupBoxWaiting.Name = "GroupBoxWaiting";
            this.GroupBoxWaiting.Size = new System.Drawing.Size(409, 64);
            this.GroupBoxWaiting.TabIndex = 0;
            this.GroupBoxWaiting.TabStop = false;
            this.GroupBoxWaiting.Text = "Подключение...";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(24, 26);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(361, 23);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 0;
            this.ProgressBar.UseWaitCursor = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // WaitingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(409, 64);
            this.Controls.Add(this.GroupBoxWaiting);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitingForm";
            this.Opacity = 0.99D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Подключение...";
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaitingForm_FormClosing);
            this.Shown += new System.EventHandler(this.WaitingForm_Shown);
            this.GroupBoxWaiting.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBoxWaiting;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Timer timer1;
    }
}