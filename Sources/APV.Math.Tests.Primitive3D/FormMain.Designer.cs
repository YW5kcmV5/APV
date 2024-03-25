namespace APV.Math.Tests.Primitive3D
{
    partial class FormMain
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
            this.PanelContol = new System.Windows.Forms.Panel();
            this.buttonPauseResume = new System.Windows.Forms.Button();
            this.ButtonRotateZ = new System.Windows.Forms.Button();
            this.ButtonRotateY = new System.Windows.Forms.Button();
            this.ButtonRotateX = new System.Windows.Forms.Button();
            this.primitive3DPanel1 = new APV.Math.Primitive3D.Primitive3DPanel();
            this.PanelContol.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelContol
            // 
            this.PanelContol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelContol.Controls.Add(this.buttonPauseResume);
            this.PanelContol.Controls.Add(this.ButtonRotateZ);
            this.PanelContol.Controls.Add(this.ButtonRotateY);
            this.PanelContol.Controls.Add(this.ButtonRotateX);
            this.PanelContol.Dock = System.Windows.Forms.DockStyle.Left;
            this.PanelContol.Location = new System.Drawing.Point(0, 0);
            this.PanelContol.Name = "PanelContol";
            this.PanelContol.Size = new System.Drawing.Size(231, 849);
            this.PanelContol.TabIndex = 0;
            // 
            // buttonPauseResume
            // 
            this.buttonPauseResume.Location = new System.Drawing.Point(12, 99);
            this.buttonPauseResume.Name = "buttonPauseResume";
            this.buttonPauseResume.Size = new System.Drawing.Size(212, 23);
            this.buttonPauseResume.TabIndex = 3;
            this.buttonPauseResume.Text = "Pause - Resume";
            this.buttonPauseResume.UseVisualStyleBackColor = true;
            this.buttonPauseResume.Click += new System.EventHandler(this.buttonPauseResume_Click);
            // 
            // ButtonRotateZ
            // 
            this.ButtonRotateZ.Location = new System.Drawing.Point(12, 70);
            this.ButtonRotateZ.Name = "ButtonRotateZ";
            this.ButtonRotateZ.Size = new System.Drawing.Size(212, 23);
            this.ButtonRotateZ.TabIndex = 2;
            this.ButtonRotateZ.Text = "Rotate Z";
            this.ButtonRotateZ.UseVisualStyleBackColor = true;
            this.ButtonRotateZ.Click += new System.EventHandler(this.ButtonRotateZ_Click);
            // 
            // ButtonRotateY
            // 
            this.ButtonRotateY.Location = new System.Drawing.Point(12, 41);
            this.ButtonRotateY.Name = "ButtonRotateY";
            this.ButtonRotateY.Size = new System.Drawing.Size(212, 23);
            this.ButtonRotateY.TabIndex = 1;
            this.ButtonRotateY.Text = "Rotate Y";
            this.ButtonRotateY.UseVisualStyleBackColor = true;
            this.ButtonRotateY.Click += new System.EventHandler(this.ButtonRotateY_Click);
            // 
            // ButtonRotateX
            // 
            this.ButtonRotateX.Location = new System.Drawing.Point(12, 12);
            this.ButtonRotateX.Name = "ButtonRotateX";
            this.ButtonRotateX.Size = new System.Drawing.Size(212, 23);
            this.ButtonRotateX.TabIndex = 0;
            this.ButtonRotateX.Text = "Rotate X";
            this.ButtonRotateX.UseVisualStyleBackColor = true;
            this.ButtonRotateX.Click += new System.EventHandler(this.button1_Click);
            // 
            // primitive3DPanel1
            // 
            this.primitive3DPanel1.BackColor = System.Drawing.SystemColors.WindowText;
            this.primitive3DPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.primitive3DPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.primitive3DPanel1.Location = new System.Drawing.Point(231, 0);
            this.primitive3DPanel1.Name = "primitive3DPanel1";
            this.primitive3DPanel1.Size = new System.Drawing.Size(1040, 849);
            this.primitive3DPanel1.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1271, 849);
            this.Controls.Add(this.primitive3DPanel1);
            this.Controls.Add(this.PanelContol);
            this.DoubleBuffered = true;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Primitive 3D";
            this.PanelContol.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelContol;
        private Math.Primitive3D.Primitive3DPanel primitive3DPanel1;
        private System.Windows.Forms.Button ButtonRotateX;
        private System.Windows.Forms.Button ButtonRotateY;
        private System.Windows.Forms.Button ButtonRotateZ;
        private System.Windows.Forms.Button buttonPauseResume;
    }
}

