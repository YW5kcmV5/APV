namespace APV.TransControl.ExportReport
{
    partial class fmConnectionSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmConnectionSettings));
            this.label5 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.numPortNumber = new System.Windows.Forms.NumericUpDown();
            this.gbConnectionSettings = new System.Windows.Forms.GroupBox();
            this.cbUseTnsOra = new System.Windows.Forms.CheckBox();
            this.cbDatabase = new System.Windows.Forms.ComboBox();
            this.tbHostname = new System.Windows.Forms.TextBox();
            this.panelTargeConnection = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.btConnect = new System.Windows.Forms.Button();
            this.rbCLConnection = new System.Windows.Forms.RadioButton();
            this.rbOLConnection = new System.Windows.Forms.RadioButton();
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.gbStringType = new System.Windows.Forms.GroupBox();
            this.rbSLConnection = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPortNumber)).BeginInit();
            this.gbConnectionSettings.SuspendLayout();
            this.panelTargeConnection.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.panelMiddle.SuspendLayout();
            this.gbStringType.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 44;
            this.label5.Text = "Пароль";
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 50;
            this.label1.Text = "Номер порта";
            // 
            // numPortNumber
            // 
            this.numPortNumber.Location = new System.Drawing.Point(137, 50);
            this.numPortNumber.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numPortNumber.Name = "numPortNumber";
            this.numPortNumber.Size = new System.Drawing.Size(82, 20);
            this.numPortNumber.TabIndex = 49;
            this.numPortNumber.Value = new decimal(new int[] {
            1521,
            0,
            0,
            0});
            // 
            // gbConnectionSettings
            // 
            this.gbConnectionSettings.Controls.Add(this.cbUseTnsOra);
            this.gbConnectionSettings.Controls.Add(this.label1);
            this.gbConnectionSettings.Controls.Add(this.numPortNumber);
            this.gbConnectionSettings.Controls.Add(this.cbDatabase);
            this.gbConnectionSettings.Controls.Add(this.tbHostname);
            this.gbConnectionSettings.Controls.Add(this.panelTargeConnection);
            this.gbConnectionSettings.Controls.Add(this.label2);
            this.gbConnectionSettings.Controls.Add(this.label6);
            this.gbConnectionSettings.Location = new System.Drawing.Point(73, 146);
            this.gbConnectionSettings.Name = "gbConnectionSettings";
            this.gbConnectionSettings.Size = new System.Drawing.Size(337, 204);
            this.gbConnectionSettings.TabIndex = 48;
            this.gbConnectionSettings.TabStop = false;
            this.gbConnectionSettings.Text = "Настройка соединения";
            // 
            // cbUseTnsOra
            // 
            this.cbUseTnsOra.AutoSize = true;
            this.cbUseTnsOra.Location = new System.Drawing.Point(14, 173);
            this.cbUseTnsOra.Name = "cbUseTnsOra";
            this.cbUseTnsOra.Size = new System.Drawing.Size(305, 17);
            this.cbUseTnsOra.TabIndex = 51;
            this.cbUseTnsOra.Text = "Использовать дескриптор соединения из tnsnames.ora";
            this.cbUseTnsOra.UseVisualStyleBackColor = true;
            this.cbUseTnsOra.CheckedChanged += new System.EventHandler(this.cbUseTnsOra_CheckedChanged);
            // 
            // cbDatabase
            // 
            this.cbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDatabase.Items.AddRange(new object[] {
            "TRAN",
            "TRANSCON"});
            this.cbDatabase.Location = new System.Drawing.Point(137, 76);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new System.Drawing.Size(178, 21);
            this.cbDatabase.TabIndex = 48;
            // 
            // tbHostname
            // 
            this.tbHostname.Location = new System.Drawing.Point(137, 24);
            this.tbHostname.Name = "tbHostname";
            this.tbHostname.Size = new System.Drawing.Size(178, 20);
            this.tbHostname.TabIndex = 0;
            this.tbHostname.Text = "192.168.58.24";
            // 
            // panelTargeConnection
            // 
            this.panelTargeConnection.Controls.Add(this.label5);
            this.panelTargeConnection.Controls.Add(this.label4);
            this.panelTargeConnection.Controls.Add(this.tbPassword);
            this.panelTargeConnection.Controls.Add(this.tbUsername);
            this.panelTargeConnection.Location = new System.Drawing.Point(14, 103);
            this.panelTargeConnection.Name = "panelTargeConnection";
            this.panelTargeConnection.Size = new System.Drawing.Size(313, 55);
            this.panelTargeConnection.TabIndex = 45;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Имя пользователя";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(123, 29);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(178, 20);
            this.tbPassword.TabIndex = 4;
            this.tbPassword.Text = "123";
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(123, 3);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(178, 20);
            this.tbUsername.TabIndex = 3;
            this.tbUsername.Text = "SYNCH_WL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Адрес сервера";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "Название БД";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.pictureBox);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(67, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.panelTop.Size = new System.Drawing.Size(349, 34);
            this.panelTop.TabIndex = 10;
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(349, 32);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.LemonChiffon;
            this.panelLeft.Controls.Add(this.pictureBox1);
            this.panelLeft.Controls.Add(this.pictureBox2);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(67, 356);
            this.panelLeft.TabIndex = 13;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(196)))), ((int)(((byte)(236)))));
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 32);
            this.pictureBox1.TabIndex = 51;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 32);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(67, 256);
            this.pictureBox2.TabIndex = 24;
            this.pictureBox2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(416, 2);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.groupBox1);
            this.panelBottom.Controls.Add(this.buttonCancel);
            this.panelBottom.Controls.Add(this.btConnect);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 356);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(416, 39);
            this.panelBottom.TabIndex = 12;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(335, 9);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "&Выход";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // btConnect
            // 
            this.btConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btConnect.Location = new System.Drawing.Point(233, 9);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(96, 23);
            this.btConnect.TabIndex = 41;
            this.btConnect.Text = "&Подключиться";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // rbCLConnection
            // 
            this.rbCLConnection.AutoSize = true;
            this.rbCLConnection.Checked = true;
            this.rbCLConnection.Enabled = false;
            this.rbCLConnection.Location = new System.Drawing.Point(14, 68);
            this.rbCLConnection.Name = "rbCLConnection";
            this.rbCLConnection.Size = new System.Drawing.Size(92, 17);
            this.rbCLConnection.TabIndex = 2;
            this.rbCLConnection.TabStop = true;
            this.rbCLConnection.Text = "DevArt Library";
            this.rbCLConnection.UseVisualStyleBackColor = true;
            // 
            // rbOLConnection
            // 
            this.rbOLConnection.AutoSize = true;
            this.rbOLConnection.Enabled = false;
            this.rbOLConnection.Location = new System.Drawing.Point(14, 45);
            this.rbOLConnection.Name = "rbOLConnection";
            this.rbOLConnection.Size = new System.Drawing.Size(221, 17);
            this.rbOLConnection.TabIndex = 1;
            this.rbOLConnection.Text = "Oracle Data Access Components (ODAC)";
            this.rbOLConnection.UseVisualStyleBackColor = true;
            // 
            // panelMiddle
            // 
            this.panelMiddle.Controls.Add(this.gbStringType);
            this.panelMiddle.Controls.Add(this.gbConnectionSettings);
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMiddle.Location = new System.Drawing.Point(0, 0);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(416, 395);
            this.panelMiddle.TabIndex = 11;
            // 
            // gbStringType
            // 
            this.gbStringType.Controls.Add(this.rbCLConnection);
            this.gbStringType.Controls.Add(this.rbOLConnection);
            this.gbStringType.Controls.Add(this.rbSLConnection);
            this.gbStringType.Location = new System.Drawing.Point(73, 40);
            this.gbStringType.Name = "gbStringType";
            this.gbStringType.Size = new System.Drawing.Size(337, 100);
            this.gbStringType.TabIndex = 49;
            this.gbStringType.TabStop = false;
            this.gbStringType.Text = "Выберите тип соединения";
            // 
            // rbSLConnection
            // 
            this.rbSLConnection.AutoSize = true;
            this.rbSLConnection.Enabled = false;
            this.rbSLConnection.Location = new System.Drawing.Point(14, 22);
            this.rbSLConnection.Name = "rbSLConnection";
            this.rbSLConnection.Size = new System.Drawing.Size(196, 17);
            this.rbSLConnection.TabIndex = 0;
            this.rbSLConnection.Text = "Microsoft System Library (ADO.NET)";
            this.rbSLConnection.UseVisualStyleBackColor = true;
            // 
            // fmConnectionSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 395);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelMiddle);
            this.Name = "fmConnectionSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка соединения";
            this.Load += new System.EventHandler(this.fmConnectionSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPortNumber)).EndInit();
            this.gbConnectionSettings.ResumeLayout(false);
            this.gbConnectionSettings.PerformLayout();
            this.panelTargeConnection.ResumeLayout(false);
            this.panelTargeConnection.PerformLayout();
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelMiddle.ResumeLayout(false);
            this.gbStringType.ResumeLayout(false);
            this.gbStringType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.Panel panelMiddle;
        private System.Windows.Forms.GroupBox gbStringType;
        private System.Windows.Forms.RadioButton rbCLConnection;
        private System.Windows.Forms.RadioButton rbOLConnection;
        private System.Windows.Forms.RadioButton rbSLConnection;
        private System.Windows.Forms.GroupBox gbConnectionSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numPortNumber;
        private System.Windows.Forms.ComboBox cbDatabase;
        private System.Windows.Forms.TextBox tbHostname;
        private System.Windows.Forms.Panel panelTargeConnection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cbUseTnsOra;
    }
}