using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using APV.Math.Diagrams;
using APV.TransControl.Core.Entities;
using APV.TransControl.Core.Entities.Consumption;

namespace APV.TransControl.FuelViewer
{
    public interface IMainFromView
    {
        ComboBox ComboBoxObj { get; }
        DateTimePicker DateTimePickerFrom { get; }
        DateTimePicker DateTimePickerTo { get; }
        DataGridView DataGridViewTransform { get; }
        DiagramPanel DiagramPanelTransform { get; }
        Label LabelTransformationFreq { get; }
        TextBox TextBoxTransformationConfig { get; }
        ListBox ListBoxFuelLoadings { get; }
        DiagramPanel DiagramPanelConsumption { get; }
        CheckBox CheckBoxOriginal { get; }
        CheckBox CheckBoxMF11 { get; }
        CheckBox CheckBoxMF11_AF10 { get; }
        CheckBox CheckBoxMF11_DF { get; }
        CheckBox CheckBoxMF11_MF5 { get; }
        CheckBox CheckBoxMF11_MF5_AF10 { get; }
        Label LabelFrom { get; }
        Label LabelTo { get; }
        TextBox TextBoxConsumptionInfo { get; }
        ComboBox СomboBoxLoadFuelLimit { get; }
        ComboBox СomboBoxDrainFuelLimit { get; }
        ComboBox СomboBoxLoadTimeLimit { get; }
        ComboBox СomboBoxDrainTimeLimit { get; }
        CheckBox СheckBoxDrainEngineControl { get; }
        DiagramPanel DiagramPanelEngine { get; }
    }

    public partial class MainForm : Form, IMainFromView
    {
        private readonly Presenter _manager;
        private readonly WaitingForm _waitingFrom = new WaitingForm();

        #region IMainFromView

        ComboBox IMainFromView.ComboBoxObj { get { return ComboBoxObj; } }
        DataGridView IMainFromView.DataGridViewTransform { get { return DataGridViewTransform; } }
        DiagramPanel IMainFromView.DiagramPanelTransform { get { return DiagramPanelTransform; } }
        Label IMainFromView.LabelTransformationFreq { get { return LabelTransformationFreq; } }
        TextBox IMainFromView.TextBoxTransformationConfig { get { return TextBoxTransformationConfig; } }
        DateTimePicker IMainFromView.DateTimePickerFrom { get { return DateTimePickerFrom; } }
        DateTimePicker IMainFromView.DateTimePickerTo { get { return DateTimePickerTo; } }
        ListBox IMainFromView.ListBoxFuelLoadings { get { return ListBoxFuelLoadings; } }
        DiagramPanel IMainFromView.DiagramPanelConsumption { get { return DiagramPanelConsumption; } }
        CheckBox IMainFromView.CheckBoxOriginal { get { return CheckBoxOriginal; } }
        CheckBox IMainFromView.CheckBoxMF11 { get { return CheckBoxMF11; } }
        CheckBox IMainFromView.CheckBoxMF11_AF10 { get { return CheckBoxMF11_AF10; } }
        CheckBox IMainFromView.CheckBoxMF11_DF { get { return CheckBoxMF11_DF; } }
        CheckBox IMainFromView.CheckBoxMF11_MF5 { get { return CheckBoxMF11_MF5; } }
        CheckBox IMainFromView.CheckBoxMF11_MF5_AF10 { get { return CheckBoxMF11_MF5_AF10; } }
        Label IMainFromView.LabelFrom { get { return LabelFrom; } }
        Label IMainFromView.LabelTo { get { return LabelTo; } }
        TextBox IMainFromView.TextBoxConsumptionInfo { get { return TextBoxConsumptionInfo; } }
        ComboBox IMainFromView.СomboBoxLoadFuelLimit { get { return СomboBoxLoadFuelLimit; } }
        ComboBox IMainFromView.СomboBoxDrainFuelLimit { get { return СomboBoxDrainFuelLimit; } }
        ComboBox IMainFromView.СomboBoxLoadTimeLimit { get { return СomboBoxLoadTimeLimit; } }
        ComboBox IMainFromView.СomboBoxDrainTimeLimit { get { return СomboBoxDrainTimeLimit; } }
        CheckBox IMainFromView.СheckBoxDrainEngineControl { get { return СheckBoxDrainEngineControl; } }
        DiagramPanel IMainFromView.DiagramPanelEngine { get { return DiagramPanelEngine; } }

        #endregion

        public MainForm()
        {
            InitializeComponent();

            _manager = new Presenter(this, _waitingFrom);

            СomboBoxLoadFuelLimit.SelectedIndexChanged += (sender, args) => _manager.UpdateSettings();
            СomboBoxDrainFuelLimit.SelectedIndexChanged += (sender, args) => _manager.UpdateSettings();
            СomboBoxLoadTimeLimit.SelectedIndexChanged += (sender, args) => _manager.UpdateSettings();
            СomboBoxDrainTimeLimit.SelectedIndexChanged += (sender, args) => _manager.UpdateSettings();
            СheckBoxDrainEngineControl.CheckedChanged += (sender, args) => _manager.UpdateSettings();
        }

        private void VerifyDateTime(bool toChanged)
        {
            DateTime from = DateTimePickerFrom.Value;
            DateTime to = DateTimePickerTo.Value;

            DateTime now = DateTime.Now;
            if (toChanged)
            {
                if (to > now)
                {
                    to = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0);
                    DateTimePickerTo.Value = to;
                }
            }
            else
            {
                if (from > now)
                {
                    from = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0);
                    DateTimePickerFrom.Value = from;
                }

                if (to <= from)
                {
                    DateTimePickerTo.Value = from;
                }
            }

            ButtonLoad.Enabled = (DateTimePickerTo.Value > DateTimePickerFrom.Value);
        }
        
        private void MainForm_Shown(object sender, EventArgs e)
        {
            UserSettings.Default.Reload();
            DateTimePickerFrom.Value = UserSettings.Default.From;
            DateTimePickerTo.Value = UserSettings.Default.To;

            _manager.LoadObjects();

            if (_manager.Objects != null)
            {
                ComboBoxObj.Items.AddRange(_manager.Objects);
                if (_manager.Objects.Length > 0)
                {
                    ComboBoxObj.Enabled = true;
                    ButtonLoad.Enabled = true;

                    string avtoNo = UserSettings.Default.AvtoNo;
                    ObjRecord obj = _manager.Objects.SingleOrDefault(item => !string.IsNullOrEmpty(avtoNo) && string.Compare(item.AvtoNo, avtoNo, true, CultureInfo.InvariantCulture) == 0);
                    ComboBoxObj.SelectedIndex = (obj != null) ? ComboBoxObj.Items.IndexOf(obj) : 0;
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ComboBoxObj.SelectedItem != null)
            {
                UserSettings.Default.AvtoNo = ((ObjRecord) ComboBoxObj.SelectedItem).AvtoNo;
                UserSettings.Default.From = DateTimePickerFrom.Value;
                UserSettings.Default.To = DateTimePickerTo.Value;
                UserSettings.Default.ConsumptionSettings = _manager.ConsumptionSettings.OuterXml;

                UserSettings.Default.Save();
            }
        }

        private void ComboBoxObj_SelectedIndexChanged(object sender, EventArgs e)
        {
            var obj = (ObjRecord)ComboBoxObj.SelectedItem;
            _manager.SelectedObject = obj;
            ListBoxFuelLoadings.Items.Clear();
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            _manager.LoadData();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _manager.UpdateDiagrams();
        }

        private void ListBoxFuelLoadings_SelectedIndexChanged(object sender, EventArgs e)
        {
            _manager.SelectedConsumption = (ConsumptionInfo)ListBoxFuelLoadings.SelectedItem;
        }

        private void DateTimePickerTo_ValueChanged(object sender, EventArgs e)
        {
            VerifyDateTime(true);
        }

        private void DateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            VerifyDateTime(false);
        }
    }
}
