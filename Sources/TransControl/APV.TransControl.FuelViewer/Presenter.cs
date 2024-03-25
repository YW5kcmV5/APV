using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using APV.Math.Diagrams;
using APV.TransControl.Core.Application;
using APV.TransControl.Core.DataLayer;
using APV.TransControl.Core.Entities;
using APV.TransControl.Core.Entities.Consumption;

namespace APV.TransControl.FuelViewer
{
    public class Presenter
    {
        private readonly IMainFromView _mainForm;
        private readonly IWaitingFormView _waitingForm;
        private ObjRecord _selectedObject;
        private Diagram _transformationDiagram;
        private ConsumptionInfo _selectedConsumption;
        private Diagram _selectedConsumptionDiagram;
        private Diagram _selectedEngineDiagram;
        private ConsumptionSettings _consumptionSettings;

        private void Init()
        {
            UserSettings.Default.Reload();

            ConnectionSettings connectionSettings = ContextManager.ConnectionSettings;

            string hostValue = UserSettings.Default.Host;
            int portValue = UserSettings.Default.Port;
            string dbNameValue = UserSettings.Default.DBName;
            string usernameValue = UserSettings.Default.Username;
            string passwordValue = UserSettings.Default.Password;
            bool useTnsDBNameValue = UserSettings.Default.UseTnsDBName;

            if (!string.IsNullOrWhiteSpace(hostValue))
            {
                connectionSettings.Host = hostValue;
            }
            connectionSettings.Port = portValue;
            if (!string.IsNullOrWhiteSpace(dbNameValue))
            {
                connectionSettings.DBName = dbNameValue;
            }
            if (!string.IsNullOrWhiteSpace(usernameValue))
            {
                connectionSettings.Username = usernameValue;
            }
            if (!string.IsNullOrWhiteSpace(passwordValue))
            {
                connectionSettings.Password = passwordValue;
            }
            connectionSettings.UseTnsDBName = useTnsDBNameValue;

            ContextManager.ConnectionSettings = connectionSettings;
             
            string consumptionSettings = UserSettings.Default.ConsumptionSettings;
            _consumptionSettings = ((!string.IsNullOrEmpty(consumptionSettings)) && (consumptionSettings != "System.Xml.XmlDocument"))
                ? new ConsumptionSettings(consumptionSettings)
                : new ConsumptionSettings(ConsumptionSettings.Default);
        }

        public Presenter(IMainFromView form, IWaitingFormView waitingForm)
        {
            _mainForm = form;
            _waitingForm = waitingForm;
            
            Init();
            UpdateSettings(false);
        }

        public void LoadObjects()
        {
            bool success = false;
            _waitingForm.Show("Загрузка списка машин...", () =>
            {
                try
                {
                    StorageManager.Init();
                    success = true;
                }
                catch (Exception ex)
                {
                    //Logger.Exception(GetType(), LogSection.Core, ex);
                    MessageBox.Show("Ошибка при загрузки списка машины. Приложение будет закрыто.\r\n\r\n" + Utility.ExceptionToString(ex), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //_waitingForm.Close();
                }
            });

            if (success)
            {
                Objects = StorageManager.Objects;
            }
            else
            {
                Application.Exit();
            }
        }

        public void LoadData()
        {
            bool success = false;
            _waitingForm.Show("Загрузка данных...", () =>
            {
                try
                {
                    DateTime from = DateTime.SpecifyKind(_mainForm.DateTimePickerFrom.Value, DateTimeKind.Local);
                    DateTime to = DateTime.SpecifyKind(_mainForm.DateTimePickerTo.Value, DateTimeKind.Local);
                    FreqRecord[] values = StorageManager.Load(_selectedObject.ObjId, from.AddDays(-1), to.AddDays(+1));

                    ConsumptionInfo[] loadingConsumptions = ConsumptionManager.FindFuelLoading1(_consumptionSettings, _selectedObject, values, from, to);
                    ConsumptionInfo[] drainingConsumptions = ConsumptionManager.FindFuelDrain1(_consumptionSettings, _selectedObject, values, from, to);

                    ConsumptionInfo[] consumptions = loadingConsumptions.Concat(drainingConsumptions).OrderBy(item => item.Gmt).ToArray();
                    _selectedObject.Consumptions = consumptions;
                    ConsumptionManager.Verify(_selectedObject, consumptions, DbManager.GetStkDataFuelXml(_selectedObject.AvtoNo, from, to));

                    success = true;
                }
                catch (Exception ex)
                {
                    _waitingForm.Close();
                    //Logger.Exception(GetType(), LogSection.Core, ex);
                    MessageBox.Show("Ошибка при загрузки данных.\r\n\r\n" + Utility.ExceptionToString(ex), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            _mainForm.ListBoxFuelLoadings.BeginUpdate();

            _mainForm.ListBoxFuelLoadings.Items.Clear();

            if (success)
            {
                if (_selectedObject.Consumptions.Length > 0)
                {
                    _mainForm.ListBoxFuelLoadings.Items.AddRange(_selectedObject.Consumptions);
                    SelectedConsumption = _selectedObject.Consumptions[0];
                }
                else
                {
                    SelectedConsumption = null;
                }
            }
            else
            {
                _selectedObject.Consumptions = null;
                SelectedConsumption = null;
            }

            _mainForm.ListBoxFuelLoadings.EndUpdate();
        }

        public void UpdateDiagrams()
        {
            if (_selectedConsumptionDiagram != null)
            {
                _selectedConsumptionDiagram.Diagrams["Original"].Visible = _mainForm.CheckBoxOriginal.Checked;
                _selectedConsumptionDiagram.Diagrams["MF11"].Visible = _mainForm.CheckBoxMF11.Checked;
                _selectedConsumptionDiagram.Diagrams["MF11_AF10"].Visible = _mainForm.CheckBoxMF11_AF10.Checked;
                _selectedConsumptionDiagram.Diagrams["MF11_DF"].Visible = _mainForm.CheckBoxMF11_DF.Checked;
                _selectedConsumptionDiagram.Diagrams["MF11_MF5"].Visible = _mainForm.CheckBoxMF11_MF5.Checked;
                _selectedConsumptionDiagram.Diagrams["MF11_MF5_AF10"].Visible = _mainForm.CheckBoxMF11_MF5_AF10.Checked;
            }
            if (_selectedEngineDiagram != null)
            {
                _selectedEngineDiagram.Visible = true;
            }
        }

        public void UpdateSettings(bool fromControlToEntity = true)
        {
            if (fromControlToEntity)
            {
                _consumptionSettings.Assign(new ConsumptionSettings
                                                {
                                                    LoadFuelLimit = _mainForm.СomboBoxLoadFuelLimit.SelectedIndex,
                                                    LoadTimeLimit = _mainForm.СomboBoxLoadTimeLimit.SelectedIndex,
                                                    DrainFuelLimit = _mainForm.СomboBoxDrainFuelLimit.SelectedIndex,
                                                    DrainTimeLimit = _mainForm.СomboBoxDrainTimeLimit.SelectedIndex,
                                                    DrainEngineControl = _mainForm.СheckBoxDrainEngineControl.Checked,
                                                });
            }
            else
            {
                _mainForm.СomboBoxLoadFuelLimit.SelectedIndex = _consumptionSettings.LoadFuelLimit;
                _mainForm.СomboBoxLoadTimeLimit.SelectedIndex = _consumptionSettings.LoadTimeLimit;
                _mainForm.СomboBoxDrainFuelLimit.SelectedIndex = _consumptionSettings.DrainFuelLimit;
                _mainForm.СomboBoxDrainTimeLimit.SelectedIndex = _consumptionSettings.DrainTimeLimit;
                _mainForm.СheckBoxDrainEngineControl.Checked = _consumptionSettings.DrainEngineControl;
            }

            _mainForm.СomboBoxLoadFuelLimit.BackColor = (_consumptionSettings.LoadFuelLimit == ConsumptionSettings.Default.LoadFuelLimit) ? Color.White : Color.Yellow;
            _mainForm.СomboBoxLoadTimeLimit.BackColor = (_consumptionSettings.LoadTimeLimit == ConsumptionSettings.Default.LoadTimeLimit) ? Color.White : Color.Yellow;
            _mainForm.СomboBoxDrainFuelLimit.BackColor = (_consumptionSettings.DrainFuelLimit == ConsumptionSettings.Default.DrainFuelLimit) ? Color.White : Color.Yellow;
            _mainForm.СomboBoxDrainTimeLimit.BackColor = (_consumptionSettings.DrainTimeLimit == ConsumptionSettings.Default.DrainTimeLimit) ? Color.White : Color.Yellow;
            _mainForm.СheckBoxDrainEngineControl.BackColor = (_consumptionSettings.DrainEngineControl == ConsumptionSettings.Default.DrainEngineControl) ? SystemColors.Control : Color.Yellow;
        }

        public ObjRecord[] Objects { get; private set; }

        public ObjRecord SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    SelectedConsumption = null;

                    float[][] transformationTable = value.TransformationTable;

                    //Show config
                    _mainForm.TextBoxTransformationConfig.Text = value.ParsedConfigData;

                    //Fill transformation table
                    _mainForm.DataGridViewTransform.Rows.Clear();
                    for (int i = 0; i < transformationTable.Length; i++)
                    {
                        float[] rowData = transformationTable[i];
                        var row = new DataGridViewRow();
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = (i + 1).ToString() });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = string.Format("{0}", rowData[0]) });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = string.Format("{0}", rowData[1]) });
                        row.Cells.Add(new DataGridViewTextBoxCell { Value = string.Format("{0:0.0000}", rowData[2]) });
                        _mainForm.DataGridViewTransform.Rows.Add(row);
                    }

                    //Fill Diagram
                    SelectedConsumption = null;
                    if (_transformationDiagram != null)
                    {
                        _transformationDiagram.Enabled = false;
                    }
                    
                    bool isCreated;
                    _transformationDiagram = value.GetDiagram(_mainForm.DiagramPanelTransform, out isCreated);
                    if (isCreated)
                    {
                        _transformationDiagram.HintSelectedPointEvent += (diagram, args) =>
                                                                             {
                                                                                 var eventArgs = (HintSelectedPointEventArgs) args;
                                                                                 eventArgs.Hint = string.Format("{0} Гц\n{1:0.00} л.", eventArgs.Point.ValueX, eventArgs.Point.ValueY);

                                                                                 eventArgs.Handled = true;
                                                                             };
                        _transformationDiagram.MouseMoveEvent += (diagram, args) =>
                                                                     {
                                                                         var eventArgs = (MouseMoveEventArgs) args;
                                                                         int mouseX = eventArgs.CurrentPoint.X;
                                                                         //int mouseY = eventArgs.CurrentPoint.Y;
                                                                         float freq = eventArgs.CurrentPoint.ValueX;
                                                                         float fuel = value.FreqToV(eventArgs.CurrentPoint.ValueX);
                                                                         string text = string.Format("{0} Гц ({1:0.00} л.)", freq, fuel);
                                                                         _mainForm.LabelTransformationFreq.Text = text;
                                                                         _mainForm.LabelTransformationFreq.Left = _mainForm.DiagramPanelTransform.Left + mouseX;
                                                                         //using (Graphics gr = _mainForm.LabelTransformationFreq.CreateGraphics())
                                                                         //{
                                                                         //    var width = (int)gr.MeasureString(text, _mainForm.LabelTransformationFreq.Font).Width;
                                                                         //    _mainForm.LabelTransformationFreq.Left = _mainForm.DiagramPanelTransform.Left + mouseX - width/2;
                                                                         //}
                                                                     };
                    }
                    _transformationDiagram.Draw();
                }
            }
        }

        public ConsumptionInfo SelectedConsumption
        {
            get { return _selectedConsumption; }
            set
            {
                if (_selectedConsumption != value)
                {
                    if (value == null)
                    {
                        _selectedConsumptionDiagram.Enabled = false;
                        _mainForm.ListBoxFuelLoadings.Items.Clear();
                        _selectedConsumption = null;

                        _mainForm.LabelFrom.Text = "-";
                        _mainForm.LabelTo.Text = "-";
                        _mainForm.TextBoxConsumptionInfo.Text = string.Empty;

                        _mainForm.DiagramPanelConsumption.Invalidate(true);

                        //Engine
                        _selectedEngineDiagram.Enabled = false;
                        _mainForm.DiagramPanelEngine.Invalidate(true);
                    }
                    else
                    {
                        _selectedConsumption = value;

                        //Select
                        int selectedItemIndex = _mainForm.ListBoxFuelLoadings.Items.IndexOf(value);
                        _mainForm.ListBoxFuelLoadings.SelectedIndex = selectedItemIndex;

                        //Fill diagrams for selected consumptions
                        if (_selectedConsumptionDiagram != null)
                        {
                            _selectedConsumptionDiagram.Enabled = false;
                        }
                        if (_selectedEngineDiagram != null)
                        {
                            _selectedEngineDiagram.Enabled = false;
                        }

                        bool isCreated;
                        _selectedConsumptionDiagram = value.GetDiagram(_mainForm.DiagramPanelConsumption, out isCreated);

                        if (isCreated)
                        {
                            _selectedConsumptionDiagram.HintSelectedPointEvent += (diagram, args) =>
                            {
                                var eventArgs = (HintSelectedPointEventArgs)args;

                                var index = (int) eventArgs.Point.ValueX;
                                if (_selectedConsumption != null)
                                {
                                    DateTime gmt = _selectedConsumption[diagram.Name][index].GMT;
                                    bool engine = (_selectedConsumption[diagram.Name][index].IsEngine);
                                    bool isStartPoint = (gmt == _selectedConsumption.Start.GMT);
                                    bool isEndPoint = (gmt == _selectedConsumption.End.GMT);
                                    bool isLoading = (_selectedConsumption.Value > 0);
                                    //float dt = eventArgs.Point.ValueX;
                                    //DateTime first = _selectedConsumption[diagram.Name][0].GMT;
                                    //DateTime gmt = first.AddSeconds(dt);

                                    float freq = eventArgs.Point.ValueY;
                                    float fuel = _selectedObject.FreqToV(freq);
                                    float maxFreq = _selectedObject.TransformationTable.Last()[0];
                                    bool outOfRange = (freq > maxFreq);
                                    
                                    string hint = string.Empty;
                                    if (isStartPoint)
                                    {
                                        hint += string.Format("Начало {0}.\n", isLoading ? "заправки" : "слива");
                                    }
                                    else if (isEndPoint)
                                    {
                                        hint += string.Format("Конец {0}.\n", isLoading ? "заправки" : "слива");
                                    }
                                    hint += (outOfRange)
                                        ? string.Format("{0}\n\n{1}\n{2} Гц ({3:0.00} л.)\n{4}\n\nВне диапазона! Значение частоты ({5}) больше максимально допустимого значения ({6})", diagram.Description, gmt, freq, fuel, engine ? "Зажигание включено" : "Зажигание выключено", freq, maxFreq)
                                        : string.Format("{0}\n\n{1}\n{2} Гц ({3:0.00} л.)\n{4}", diagram.Description, gmt, freq, fuel, engine ? "Зажигание включено" : "Зажигание выключено");
                                    
                                    eventArgs.Hint = hint;
                                }
                                eventArgs.Handled = true;
                            };
                        }

                        bool isEngineCreated;
                        _selectedEngineDiagram = value.GetEngineDiagram(_mainForm.DiagramPanelEngine, out isEngineCreated);
                        if (isEngineCreated)
                        {
                            _selectedEngineDiagram.HintSelectedPointEvent += (diagram, args) =>
                            {
                                var eventArgs = (HintSelectedPointEventArgs)args;

                                var index = (int)eventArgs.Point.ValueX;
                                if (_selectedConsumption != null)
                                {
                                    DateTime gmt = _selectedConsumption["Original"][index].GMT;
                                    bool engine = eventArgs.Point.ValueY > 0;
                                    eventArgs.Hint = string.Format("{0}\n\n{1}\n{2}", diagram.Description, gmt, engine ? "Зажигание включено" : "Зажигание выключено");
                                }
                                eventArgs.Handled = true;
                            };

                            _selectedEngineDiagram.SynchronizeTo(_selectedConsumptionDiagram);
                        }
                        

                        _selectedConsumptionDiagram.Draw();
                        _selectedEngineDiagram.Draw();

                        _mainForm.LabelFrom.Text = value.Freq.First().GMT.ToString();
                        _mainForm.LabelTo.Text = value.Freq.Last().GMT.ToString();
                        _mainForm.TextBoxConsumptionInfo.Text = value.GetInfo();
                    }

                    UpdateDiagrams();
                }
            }
        }

        public ConsumptionSettings ConsumptionSettings
        {
            get { return _consumptionSettings; }
        }
    }
}
