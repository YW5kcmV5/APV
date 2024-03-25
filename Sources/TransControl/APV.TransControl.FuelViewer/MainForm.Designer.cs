using APV.Math.Diagrams;

namespace APV.TransControl.FuelViewer
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TextBoxConsumptionInfo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonLoad = new System.Windows.Forms.Button();
            this.ListBoxFuelLoadings = new System.Windows.Forms.ListBox();
            this.DateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.DateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.ComboBoxObj = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.DiagramPanelConsumption = new DiagramPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.LabelTo = new System.Windows.Forms.Label();
            this.LabelFrom = new System.Windows.Forms.Label();
            this.CheckBoxMF11_MF5_AF10 = new System.Windows.Forms.CheckBox();
            this.CheckBoxMF11_MF5 = new System.Windows.Forms.CheckBox();
            this.CheckBoxMF11_DF = new System.Windows.Forms.CheckBox();
            this.CheckBoxMF11_AF10 = new System.Windows.Forms.CheckBox();
            this.CheckBoxMF11 = new System.Windows.Forms.CheckBox();
            this.CheckBoxOriginal = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TextBoxTransformationConfig = new System.Windows.Forms.TextBox();
            this.LabelTransformationFreq = new System.Windows.Forms.Label();
            this.DataGridViewTransform = new System.Windows.Forms.DataGridView();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Freq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fuel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.k = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DiagramPanelTransform = new DiagramPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.СomboBoxLoadFuelLimit = new System.Windows.Forms.ComboBox();
            this.СomboBoxDrainFuelLimit = new System.Windows.Forms.ComboBox();
            this.СomboBoxLoadTimeLimit = new System.Windows.Forms.ComboBox();
            this.СomboBoxDrainTimeLimit = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.СheckBoxDrainEngineControl = new System.Windows.Forms.CheckBox();
            this.DiagramPanelEngine = new DiagramPanel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewTransform)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ButtonLoad);
            this.groupBox1.Controls.Add(this.ListBoxFuelLoadings);
            this.groupBox1.Controls.Add(this.DateTimePickerTo);
            this.groupBox1.Controls.Add(this.DateTimePickerFrom);
            this.groupBox1.Controls.Add(this.ComboBoxObj);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1209, 226);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Машина ";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.TextBoxConsumptionInfo);
            this.groupBox2.Location = new System.Drawing.Point(647, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(550, 193);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Информация о заправке ";
            // 
            // TextBoxConsumptionInfo
            // 
            this.TextBoxConsumptionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxConsumptionInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBoxConsumptionInfo.Location = new System.Drawing.Point(7, 20);
            this.TextBoxConsumptionInfo.Multiline = true;
            this.TextBoxConsumptionInfo.Name = "TextBoxConsumptionInfo";
            this.TextBoxConsumptionInfo.ReadOnly = true;
            this.TextBoxConsumptionInfo.Size = new System.Drawing.Size(537, 164);
            this.TextBoxConsumptionInfo.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(192, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Дата:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Номер:";
            // 
            // ButtonLoad
            // 
            this.ButtonLoad.Enabled = false;
            this.ButtonLoad.Location = new System.Drawing.Point(12, 73);
            this.ButtonLoad.Name = "ButtonLoad";
            this.ButtonLoad.Size = new System.Drawing.Size(324, 23);
            this.ButtonLoad.TabIndex = 0;
            this.ButtonLoad.Text = "Загрузить";
            this.ButtonLoad.UseVisualStyleBackColor = true;
            this.ButtonLoad.Click += new System.EventHandler(this.ButtonLoad_Click);
            // 
            // ListBoxFuelLoadings
            // 
            this.ListBoxFuelLoadings.FormattingEnabled = true;
            this.ListBoxFuelLoadings.Location = new System.Drawing.Point(342, 19);
            this.ListBoxFuelLoadings.Name = "ListBoxFuelLoadings";
            this.ListBoxFuelLoadings.ScrollAlwaysVisible = true;
            this.ListBoxFuelLoadings.Size = new System.Drawing.Size(298, 199);
            this.ListBoxFuelLoadings.TabIndex = 3;
            this.ListBoxFuelLoadings.SelectedIndexChanged += new System.EventHandler(this.ListBoxFuelLoadings_SelectedIndexChanged);
            // 
            // DateTimePickerTo
            // 
            this.DateTimePickerTo.CustomFormat = "dd-MM-yyyy HH:mm:ss";
            this.DateTimePickerTo.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.DateTimePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTimePickerTo.Location = new System.Drawing.Point(208, 47);
            this.DateTimePickerTo.Name = "DateTimePickerTo";
            this.DateTimePickerTo.Size = new System.Drawing.Size(128, 20);
            this.DateTimePickerTo.TabIndex = 2;
            this.DateTimePickerTo.Value = new System.DateTime(2012, 1, 14, 23, 59, 59, 0);
            this.DateTimePickerTo.ValueChanged += new System.EventHandler(this.DateTimePickerTo_ValueChanged);
            // 
            // DateTimePickerFrom
            // 
            this.DateTimePickerFrom.CustomFormat = "dd-MM-yyyy HH:mm:ss";
            this.DateTimePickerFrom.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.DateTimePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTimePickerFrom.Location = new System.Drawing.Point(58, 47);
            this.DateTimePickerFrom.Name = "DateTimePickerFrom";
            this.DateTimePickerFrom.Size = new System.Drawing.Size(128, 20);
            this.DateTimePickerFrom.TabIndex = 1;
            this.DateTimePickerFrom.Value = new System.DateTime(2012, 1, 14, 10, 0, 0, 0);
            this.DateTimePickerFrom.ValueChanged += new System.EventHandler(this.DateTimePickerFrom_ValueChanged);
            // 
            // ComboBoxObj
            // 
            this.ComboBoxObj.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxObj.Enabled = false;
            this.ComboBoxObj.FormattingEnabled = true;
            this.ComboBoxObj.Location = new System.Drawing.Point(58, 20);
            this.ComboBoxObj.MaxDropDownItems = 30;
            this.ComboBoxObj.Name = "ComboBoxObj";
            this.ComboBoxObj.Size = new System.Drawing.Size(278, 21);
            this.ComboBoxObj.TabIndex = 0;
            this.ComboBoxObj.SelectedIndexChanged += new System.EventHandler(this.ComboBoxObj_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 226);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1209, 652);
            this.panel2.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1209, 652);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.DiagramPanelEngine);
            this.tabPage1.Controls.Add(this.DiagramPanelConsumption);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1201, 626);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = " График ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // DiagramPanelConsumption
            // 
            this.DiagramPanelConsumption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DiagramPanelConsumption.BackColor = System.Drawing.SystemColors.Control;
            this.DiagramPanelConsumption.Location = new System.Drawing.Point(3, 3);
            this.DiagramPanelConsumption.Name = "DiagramPanelConsumption";
            this.DiagramPanelConsumption.Size = new System.Drawing.Size(1195, 400);
            this.DiagramPanelConsumption.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.LabelTo);
            this.panel1.Controls.Add(this.LabelFrom);
            this.panel1.Controls.Add(this.CheckBoxMF11_MF5_AF10);
            this.panel1.Controls.Add(this.CheckBoxMF11_MF5);
            this.panel1.Controls.Add(this.CheckBoxMF11_DF);
            this.panel1.Controls.Add(this.CheckBoxMF11_AF10);
            this.panel1.Controls.Add(this.CheckBoxMF11);
            this.panel1.Controls.Add(this.CheckBoxOriginal);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 490);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1195, 133);
            this.panel1.TabIndex = 2;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.DarkGreen;
            this.panel6.Location = new System.Drawing.Point(312, 76);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(17, 17);
            this.panel6.TabIndex = 13;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.DarkBlue;
            this.panel5.Location = new System.Drawing.Point(12, 76);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(17, 17);
            this.panel5.TabIndex = 10;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.DarkOrange;
            this.panel7.Location = new System.Drawing.Point(312, 53);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(17, 17);
            this.panel7.TabIndex = 12;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkRed;
            this.panel4.Location = new System.Drawing.Point(12, 53);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(17, 17);
            this.panel4.TabIndex = 9;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.DarkCyan;
            this.panel8.Location = new System.Drawing.Point(312, 30);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(17, 17);
            this.panel8.TabIndex = 11;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Location = new System.Drawing.Point(12, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(17, 17);
            this.panel3.TabIndex = 8;
            // 
            // LabelTo
            // 
            this.LabelTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelTo.ForeColor = System.Drawing.Color.DarkBlue;
            this.LabelTo.Location = new System.Drawing.Point(1018, 7);
            this.LabelTo.Name = "LabelTo";
            this.LabelTo.Size = new System.Drawing.Size(170, 13);
            this.LabelTo.TabIndex = 7;
            this.LabelTo.Text = "-";
            this.LabelTo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // LabelFrom
            // 
            this.LabelFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelFrom.ForeColor = System.Drawing.Color.DarkBlue;
            this.LabelFrom.Location = new System.Drawing.Point(6, 7);
            this.LabelFrom.Name = "LabelFrom";
            this.LabelFrom.Size = new System.Drawing.Size(170, 13);
            this.LabelFrom.TabIndex = 6;
            this.LabelFrom.Text = "-";
            // 
            // CheckBoxMF11_MF5_AF10
            // 
            this.CheckBoxMF11_MF5_AF10.AutoSize = true;
            this.CheckBoxMF11_MF5_AF10.Checked = true;
            this.CheckBoxMF11_MF5_AF10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxMF11_MF5_AF10.Location = new System.Drawing.Point(338, 77);
            this.CheckBoxMF11_MF5_AF10.Name = "CheckBoxMF11_MF5_AF10";
            this.CheckBoxMF11_MF5_AF10.Size = new System.Drawing.Size(353, 17);
            this.CheckBoxMF11_MF5_AF10.TabIndex = 5;
            this.CheckBoxMF11_MF5_AF10.Text = "Median Filter 11 + Median Filter 5 + Aperiodic Filter 0.10 (фильтр №5)";
            this.CheckBoxMF11_MF5_AF10.UseVisualStyleBackColor = true;
            this.CheckBoxMF11_MF5_AF10.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // CheckBoxMF11_MF5
            // 
            this.CheckBoxMF11_MF5.AutoSize = true;
            this.CheckBoxMF11_MF5.Location = new System.Drawing.Point(338, 54);
            this.CheckBoxMF11_MF5.Name = "CheckBoxMF11_MF5";
            this.CheckBoxMF11_MF5.Size = new System.Drawing.Size(248, 17);
            this.CheckBoxMF11_MF5.TabIndex = 4;
            this.CheckBoxMF11_MF5.Text = "Median Filter 11 + Median Filter 5 (фильтр №4)";
            this.CheckBoxMF11_MF5.UseVisualStyleBackColor = true;
            this.CheckBoxMF11_MF5.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // CheckBoxMF11_DF
            // 
            this.CheckBoxMF11_DF.AutoSize = true;
            this.CheckBoxMF11_DF.Location = new System.Drawing.Point(338, 31);
            this.CheckBoxMF11_DF.Name = "CheckBoxMF11_DF";
            this.CheckBoxMF11_DF.Size = new System.Drawing.Size(233, 17);
            this.CheckBoxMF11_DF.TabIndex = 3;
            this.CheckBoxMF11_DF.Text = "Median Filter 11 + Digital Filter (фильтр №3)";
            this.CheckBoxMF11_DF.UseVisualStyleBackColor = true;
            this.CheckBoxMF11_DF.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // CheckBoxMF11_AF10
            // 
            this.CheckBoxMF11_AF10.AutoSize = true;
            this.CheckBoxMF11_AF10.Location = new System.Drawing.Point(35, 77);
            this.CheckBoxMF11_AF10.Name = "CheckBoxMF11_AF10";
            this.CheckBoxMF11_AF10.Size = new System.Drawing.Size(272, 17);
            this.CheckBoxMF11_AF10.TabIndex = 2;
            this.CheckBoxMF11_AF10.Text = "Median Filter 11 + Aperiodic Filter 0.10 (фильтр №2)";
            this.CheckBoxMF11_AF10.UseVisualStyleBackColor = true;
            this.CheckBoxMF11_AF10.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // CheckBoxMF11
            // 
            this.CheckBoxMF11.AutoSize = true;
            this.CheckBoxMF11.Location = new System.Drawing.Point(35, 54);
            this.CheckBoxMF11.Name = "CheckBoxMF11";
            this.CheckBoxMF11.Size = new System.Drawing.Size(167, 17);
            this.CheckBoxMF11.TabIndex = 1;
            this.CheckBoxMF11.Text = "Median Filter 11 (фильтр №1)";
            this.CheckBoxMF11.UseVisualStyleBackColor = true;
            this.CheckBoxMF11.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // CheckBoxOriginal
            // 
            this.CheckBoxOriginal.AutoSize = true;
            this.CheckBoxOriginal.Checked = true;
            this.CheckBoxOriginal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxOriginal.Location = new System.Drawing.Point(35, 31);
            this.CheckBoxOriginal.Name = "CheckBoxOriginal";
            this.CheckBoxOriginal.Size = new System.Drawing.Size(192, 17);
            this.CheckBoxOriginal.TabIndex = 0;
            this.CheckBoxOriginal.Text = "Original (исходный набор данных)";
            this.CheckBoxOriginal.UseVisualStyleBackColor = true;
            this.CheckBoxOriginal.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.TextBoxTransformationConfig);
            this.tabPage2.Controls.Add(this.LabelTransformationFreq);
            this.tabPage2.Controls.Add(this.DataGridViewTransform);
            this.tabPage2.Controls.Add(this.DiagramPanelTransform);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1201, 686);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = " Таблица трансформации ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TextBoxTransformationConfig
            // 
            this.TextBoxTransformationConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxTransformationConfig.Location = new System.Drawing.Point(14, 7);
            this.TextBoxTransformationConfig.Name = "TextBoxTransformationConfig";
            this.TextBoxTransformationConfig.ReadOnly = true;
            this.TextBoxTransformationConfig.Size = new System.Drawing.Size(1179, 20);
            this.TextBoxTransformationConfig.TabIndex = 3;
            // 
            // LabelTransformationFreq
            // 
            this.LabelTransformationFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelTransformationFreq.AutoSize = true;
            this.LabelTransformationFreq.BackColor = System.Drawing.Color.Transparent;
            this.LabelTransformationFreq.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelTransformationFreq.ForeColor = System.Drawing.Color.DarkBlue;
            this.LabelTransformationFreq.Location = new System.Drawing.Point(11, 688);
            this.LabelTransformationFreq.Name = "LabelTransformationFreq";
            this.LabelTransformationFreq.Size = new System.Drawing.Size(25, 13);
            this.LabelTransformationFreq.TabIndex = 2;
            this.LabelTransformationFreq.Text = "0.0";
            // 
            // DataGridViewTransform
            // 
            this.DataGridViewTransform.AllowUserToAddRows = false;
            this.DataGridViewTransform.AllowUserToDeleteRows = false;
            this.DataGridViewTransform.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridViewTransform.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewTransform.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Number,
            this.Freq,
            this.Fuel,
            this.k});
            this.DataGridViewTransform.Location = new System.Drawing.Point(556, 33);
            this.DataGridViewTransform.MultiSelect = false;
            this.DataGridViewTransform.Name = "DataGridViewTransform";
            this.DataGridViewTransform.ReadOnly = true;
            this.DataGridViewTransform.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DataGridViewTransform.RowHeadersVisible = false;
            this.DataGridViewTransform.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DataGridViewTransform.ShowEditingIcon = false;
            this.DataGridViewTransform.Size = new System.Drawing.Size(637, 651);
            this.DataGridViewTransform.TabIndex = 0;
            // 
            // Number
            // 
            this.Number.HeaderText = "№";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            this.Number.Width = 30;
            // 
            // Freq
            // 
            this.Freq.HeaderText = "Частота, Гц";
            this.Freq.Name = "Freq";
            this.Freq.ReadOnly = true;
            // 
            // Fuel
            // 
            this.Fuel.HeaderText = "Объем, л";
            this.Fuel.Name = "Fuel";
            this.Fuel.ReadOnly = true;
            // 
            // k
            // 
            this.k.HeaderText = "K";
            this.k.Name = "k";
            this.k.ReadOnly = true;
            // 
            // DiagramPanelTransform
            // 
            this.DiagramPanelTransform.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.DiagramPanelTransform.BackColor = System.Drawing.SystemColors.Control;
            this.DiagramPanelTransform.Location = new System.Drawing.Point(14, 33);
            this.DiagramPanelTransform.Name = "DiagramPanelTransform";
            this.DiagramPanelTransform.Size = new System.Drawing.Size(536, 651);
            this.DiagramPanelTransform.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.СheckBoxDrainEngineControl);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.СomboBoxDrainTimeLimit);
            this.groupBox3.Controls.Add(this.СomboBoxLoadTimeLimit);
            this.groupBox3.Controls.Add(this.СomboBoxDrainFuelLimit);
            this.groupBox3.Controls.Add(this.СomboBoxLoadFuelLimit);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(12, 109);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(324, 109);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " Параметры фильтрации ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Предел, л.:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(133, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Заправки";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(261, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Сливы";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Время., мин.:";
            // 
            // СomboBoxLoadFuelLimit
            // 
            this.СomboBoxLoadFuelLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.СomboBoxLoadFuelLimit.FormattingEnabled = true;
            this.СomboBoxLoadFuelLimit.Items.AddRange(new object[] {
            "<Нет предела>",
            "1 литр",
            "2 литра",
            "3 литра",
            "4 литра",
            "5 литров",
            "6 литров",
            "7 литров",
            "8 литров",
            "9 литров",
            "10 литров",
            "11 литров",
            "12 литров",
            "13 литров",
            "14 литров",
            "15 литров",
            "16 литров",
            "17 литров",
            "18 литров",
            "19 литров",
            "20 литров",
            "21 литр",
            "22 литра",
            "23 литра",
            "24 литра",
            "25 литров"});
            this.СomboBoxLoadFuelLimit.Location = new System.Drawing.Point(123, 36);
            this.СomboBoxLoadFuelLimit.Name = "СomboBoxLoadFuelLimit";
            this.СomboBoxLoadFuelLimit.Size = new System.Drawing.Size(85, 21);
            this.СomboBoxLoadFuelLimit.TabIndex = 10;
            // 
            // СomboBoxDrainFuelLimit
            // 
            this.СomboBoxDrainFuelLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.СomboBoxDrainFuelLimit.FormattingEnabled = true;
            this.СomboBoxDrainFuelLimit.Items.AddRange(new object[] {
            "<Нет предела>",
            "1 литр",
            "2 литра",
            "3 литра",
            "4 литра",
            "5 литров",
            "6 литров",
            "7 литров",
            "8 литров",
            "9 литров",
            "10 литров",
            "11 литров",
            "12 литров",
            "13 литров",
            "14 литров",
            "15 литров",
            "16 литров",
            "17 литров",
            "18 литров",
            "19 литров",
            "20 литров",
            "21 литр",
            "22 литра",
            "23 литра",
            "24 литра",
            "25 литров"});
            this.СomboBoxDrainFuelLimit.Location = new System.Drawing.Point(228, 36);
            this.СomboBoxDrainFuelLimit.Name = "СomboBoxDrainFuelLimit";
            this.СomboBoxDrainFuelLimit.Size = new System.Drawing.Size(85, 21);
            this.СomboBoxDrainFuelLimit.TabIndex = 11;
            // 
            // СomboBoxLoadTimeLimit
            // 
            this.СomboBoxLoadTimeLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.СomboBoxLoadTimeLimit.FormattingEnabled = true;
            this.СomboBoxLoadTimeLimit.Items.AddRange(new object[] {
            "<Нет предела>",
            "1 мин.",
            "2 мин.",
            "3 мин.",
            "4 мин.",
            "5 мин.",
            "6 мин.",
            "7 мин.",
            "8 мин.",
            "9 мин.",
            "10 мин.",
            "11 мин.",
            "12 мин.",
            "13 мин.",
            "14 мин.",
            "15 мин.",
            "16 мин.",
            "17 мин.",
            "18 мин.",
            "19 мин.",
            "20 мин.",
            "21 мин.",
            "22 мин.",
            "23 мин.",
            "24 мин.",
            "25 мин."});
            this.СomboBoxLoadTimeLimit.Location = new System.Drawing.Point(123, 62);
            this.СomboBoxLoadTimeLimit.Name = "СomboBoxLoadTimeLimit";
            this.СomboBoxLoadTimeLimit.Size = new System.Drawing.Size(85, 21);
            this.СomboBoxLoadTimeLimit.TabIndex = 12;
            // 
            // СomboBoxDrainTimeLimit
            // 
            this.СomboBoxDrainTimeLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.СomboBoxDrainTimeLimit.FormattingEnabled = true;
            this.СomboBoxDrainTimeLimit.Items.AddRange(new object[] {
            "<Нет предела>",
            "1 мин.",
            "2 мин.",
            "3 мин.",
            "4 мин.",
            "5 мин.",
            "6 мин.",
            "7 мин.",
            "8 мин.",
            "9 мин.",
            "10 мин.",
            "11 мин.",
            "12 мин.",
            "13 мин.",
            "14 мин.",
            "15 мин.",
            "16 мин.",
            "17 мин.",
            "18 мин.",
            "19 мин.",
            "20 мин.",
            "21 мин.",
            "22 мин.",
            "23 мин.",
            "24 мин.",
            "25 мин."});
            this.СomboBoxDrainTimeLimit.Location = new System.Drawing.Point(228, 62);
            this.СomboBoxDrainTimeLimit.Name = "СomboBoxDrainTimeLimit";
            this.СomboBoxDrainTimeLimit.Size = new System.Drawing.Size(85, 21);
            this.СomboBoxDrainTimeLimit.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Контроль зажигания:";
            // 
            // СheckBoxDrainEngineControl
            // 
            this.СheckBoxDrainEngineControl.AutoSize = true;
            this.СheckBoxDrainEngineControl.Location = new System.Drawing.Point(228, 88);
            this.СheckBoxDrainEngineControl.Name = "СheckBoxDrainEngineControl";
            this.СheckBoxDrainEngineControl.Size = new System.Drawing.Size(15, 14);
            this.СheckBoxDrainEngineControl.TabIndex = 15;
            this.СheckBoxDrainEngineControl.UseVisualStyleBackColor = true;
            // 
            // DiagramPanelEngine
            // 
            this.DiagramPanelEngine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DiagramPanelEngine.Location = new System.Drawing.Point(3, 409);
            this.DiagramPanelEngine.Name = "DiagramPanelEngine";
            this.DiagramPanelEngine.Size = new System.Drawing.Size(1195, 81);
            this.DiagramPanelEngine.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 878);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "\"Трансконтроль - Эверест\". Выгрузка данных о заправках топлива.";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewTransform)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonLoad;
        private System.Windows.Forms.ListBox ListBoxFuelLoadings;
        private System.Windows.Forms.DateTimePicker DateTimePickerTo;
        private System.Windows.Forms.DateTimePicker DateTimePickerFrom;
        private System.Windows.Forms.ComboBox ComboBoxObj;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private DiagramPanel DiagramPanelTransform;
        private System.Windows.Forms.DataGridView DataGridViewTransform;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Freq;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fuel;
        private System.Windows.Forms.DataGridViewTextBoxColumn k;
        private DiagramPanel DiagramPanelConsumption;
        private System.Windows.Forms.Label LabelTransformationFreq;
        private System.Windows.Forms.TextBox TextBoxTransformationConfig;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox CheckBoxOriginal;
        private System.Windows.Forms.CheckBox CheckBoxMF11;
        private System.Windows.Forms.CheckBox CheckBoxMF11_AF10;
        private System.Windows.Forms.CheckBox CheckBoxMF11_DF;
        private System.Windows.Forms.CheckBox CheckBoxMF11_MF5;
        private System.Windows.Forms.CheckBox CheckBoxMF11_MF5_AF10;
        private System.Windows.Forms.Label LabelTo;
        private System.Windows.Forms.Label LabelFrom;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TextBoxConsumptionInfo;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox СomboBoxDrainTimeLimit;
        private System.Windows.Forms.ComboBox СomboBoxLoadTimeLimit;
        private System.Windows.Forms.ComboBox СomboBoxDrainFuelLimit;
        private System.Windows.Forms.ComboBox СomboBoxLoadFuelLimit;
        private System.Windows.Forms.CheckBox СheckBoxDrainEngineControl;
        private System.Windows.Forms.Label label8;
        private DiagramPanel DiagramPanelEngine;
    }
}