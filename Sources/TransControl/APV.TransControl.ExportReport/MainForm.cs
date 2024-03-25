using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using APV.TransControl.Common;
using APV.TransControl.Core.Entities;
using APV.TransControl.ExportReport.ReportData;

namespace APV.TransControl.ExportReport
{
    public partial class MainForm : Form
    {
        private static string _searchString;
        private static DateTime _searchTimestamp = DateTime.Now;
        private static readonly HashSet<char> SearchCharsSet = new HashSet<char>("qwertyuiopasdfghjklzxcvbnmйцукенгшщзхъфывапролджэячсмитьбю0123456789");

        private static bool _maskWasChanged;
        private static bool _internalMaskChanging;
        //private static bool _templateWasChanged;

        public MainForm()
        {
            InitializeComponent();

            cbModels.Items.Add("Все модели");
            for (int i = 0; i < Program.Manager.Models.Count; i++)
            {
                cbModels.Items.Add(Program.Manager.Models[i]);
            }

            FillLbAllMasks(null);
            FillLbTemplates();
            cbModels.SelectedIndex = 0;
            FillLbObj();
        }

        private void FillLbTemplates()
        {
            string templateName = (lbTemplates.SelectedIndex != -1) ? (string)lbTemplates.SelectedItem : string.Empty;
            lbTemplates.BeginUpdate();
            lbTemplates.Items.Clear();
            for (int i = 0; i < Program.Manager.Templates.Length; i++)
            {
                string name = Program.Manager.Templates[i].TemplateName;
                lbTemplates.Items.Add(name);
                if (name == templateName)
                {
                    lbTemplates.SelectedIndex = i;
                }
            }
            lbTemplates.EndUpdate();
        }

        private void FillLbObj()
        {
            MonObj currentObj = (lbObj.SelectedIndex != -1)
                                    ? Program.Manager.GetObjByAvtoNo((string) lbObj.SelectedItem)
                                    : null;
            int currentObjId = (currentObj != null) ? currentObj.Objid : -1;

            lbObj.BeginUpdate();
            lbObj.SelectedIndex = -1;
            lbObj.Items.Clear();
            int k = 0;
            string model = cbModels.Text.Trim();
            for (int i = 0; i < Program.Manager.Obj.Length; i++)
            {
                if ((cbModels.SelectedIndex == 0) || (model == Program.Manager.Obj[i].Avto_model.Trim()))
                {
                    bool hasEquipment = Program.Manager.Obj[i].AddEquipment.Count > 0;
                    bool hasTemplate = (hasEquipment) && (!string.IsNullOrEmpty(Program.Manager.GetTemplateNameByObjId(Program.Manager.Obj[i].Objid)));
                    string symbol = (hasEquipment) ? ((hasTemplate) ? "*" : "!") : string.Empty;
                    string objViewName = string.Format("{0} {1}", Program.Manager.Obj[i].Avto_no, symbol);
                    lbObj.Items.Add(objViewName);
                    if (currentObjId == Program.Manager.Obj[i].Objid)
                    {
                        lbObj.SelectedIndex = k;
                    }
                    k++;
                }
            }
            if ((lbObj.SelectedIndex == -1) && (Program.Manager.Obj.Length > 0))
            {
                lbObj.SelectedIndex = 0;
            }
            lbObj.EndUpdate();
        }

        private void FillLbAllMasks(string[] withoutMaskNames)
        {
            if (withoutMaskNames == null)
            {
                withoutMaskNames = new string[0];
            }
            var withoutNames = new List<string>(withoutMaskNames);
            
            var selectedMaskName = (lbAllMasks.SelectedItem as string);
            int selectedIndex = (lbAllMasks.SelectedIndex);

            lbAllMasks.BeginUpdate();
            lbAllMasks.Items.Clear();
            int index = 0;
            int newSelectedIndex = 0;
            for (int i = 0; i < Program.Manager.Masks.Length; i++)
            {
                string name = Program.Manager.Masks[i].Name;
                if (!withoutNames.Contains(name))
                {
                    lbAllMasks.Items.Add(name);
                    if ((selectedMaskName != null) && (name == selectedMaskName))
                    {
                        newSelectedIndex = index;
                    }
                    index++;
                }
            }
            int newCount = lbAllMasks.Items.Count;
            if (newCount > 0)
            {
                if ((newSelectedIndex == 0) && (selectedIndex != -1))
                {
                    if (selectedIndex < newCount)
                    {
                        newSelectedIndex = selectedIndex;
                    }
                    else
                    {
                        newSelectedIndex = newCount - 1;
                    }
                }
                lbAllMasks.SelectedIndex = newSelectedIndex;
            }
            lbAllMasks.EndUpdate();
        }

        private void lbObj_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbObj.SelectedIndex;
            if (index != -1)
            {
                var avtoNo = (string) lbObj.SelectedItem;
                MonObj currentObj = Program.Manager.GetObjByAvtoNo(avtoNo);
                
                string[] equipmentNames = ((currentObj != null) && (currentObj.AddEquipmentName != null))
                                              ? currentObj.AddEquipmentName.ToArray()
                                              : new string[0];

                string templateName = Program.Manager.GetTemplateNameByMask(equipmentNames);
                tbTemplateName2.Text = templateName;
                tbTemplateName3.Text = templateName;

                if (lbTemplates.Items.Contains(templateName))
                {
                    lbTemplates.SelectedIndex = lbTemplates.Items.IndexOf(templateName);
                    MasksToScreen(templateName, equipmentNames);
                }
                else
                {
                    if (equipmentNames.Length > 0)
                    {
                        lbTemplates.SelectedIndex = -1;
                        MasksToScreen(string.Empty, equipmentNames);
                    }
                    else
                    {
                        lbTemplates.SelectedIndex = -1;
                        ClearMask();
                    }
                }

                if (currentObj != null)
                {
                    tbAvtoNo.Text = currentObj.Avto_no;
                    tbAvtoNo2.Text = currentObj.Avto_no;
                    tbObjId.Text = currentObj.Objid.ToString(CultureInfo.InvariantCulture);
                    tbObjId2.Text = currentObj.Objid.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    tbAvtoNo.Text = avtoNo;
                    tbAvtoNo2.Text = avtoNo;
                    tbObjId.Text = "[Машина не найдена]";
                    tbObjId2.Text = "[Машина не найдена]";
                }

                btDelAllMaskFromObj.Enabled = (equipmentNames.Length > 0);
            }

            btAddTemplateToObj.Enabled = false;

            lbAllMasks.BackColor = SystemColors.Control;
            lbTemplates.BackColor = SystemColors.Control;
            lbObj.BackColor = Color.White;
        }

        private void lbAllMasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbMask.Enabled = true;
            btAddTemplateToObj.Enabled = false;

            lbAllMasks.BackColor = Color.White;
            lbObj.BackColor = SystemColors.Control;
            lbTemplates.BackColor = SystemColors.Control;

            lbMasks.SelectedIndex = -1;

            AddEquipment equipment = Program.Manager.Masks[lbAllMasks.SelectedIndex];

            MaskButtonEnable();

            EquipmentToScreen(equipment);
        }

        private void lbTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbMask.Enabled = true;

            if (lbTemplates.SelectedIndex != -1)
            {
                btAddTemplateToObj.Enabled = ((lbTemplates.SelectedItem != null) && ((string)lbTemplates.SelectedItem != tbTemplateName2.Text));

                lbTemplates.BackColor = Color.White;
                lbObj.BackColor = SystemColors.Control;
                lbAllMasks.BackColor = SystemColors.Control;

                string templateName = Program.Manager.Templates[lbTemplates.SelectedIndex].TemplateName;
                string[] maskNames = Program.Manager.Templates[lbTemplates.SelectedIndex].AddEquipmentName;
                MasksToScreen(templateName, maskNames);
            }
            else
            {
                ClearTemplate();
                ClearMask();
            }
        }

        private void lbMasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            string maskName = (string)lbMasks.SelectedItem;

            int index = lbObj.SelectedIndex;
            if (index >= 0)
            {
                MaskButtonEnable();

                MonObj _currentObj = Program.Manager.GetObjByAvtoNo((string)lbObj.SelectedItem);

                AddEquipment equipment = null;
                for (int i = 0; i < Program.Manager.Masks.Length; i++)
                {
                    if (Program.Manager.Masks[i].Name == maskName)
                    {
                        equipment = Program.Manager.Masks[i];
                        break;
                    }
                }
                if (equipment != null)
                {
                    EquipmentToScreen(equipment);
                }
                else
                {
                    ClearMask();
                }
            }
        }

        private void cbModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLbObj();
        }

        private void ClearMask()
        {
            _internalMaskChanging = true;
            tbMaskName.Text = string.Empty;
            tbDescription.Text = string.Empty;
            tbFuelConsumption.Text = string.Empty;
            tbSpeedControl.Text = string.Empty;
            cbSK1.Checked = false;
            cbSK2.Checked = false;
            cbSK3.Checked = false;
            cbSK4.Checked = false;
            cbSK5.Checked = false;
            cbSK6.Checked = false;
            cbSK7.Checked = false;
            cbSK8.Checked = false;
            cbStateMask.SelectedIndex = 0;
            cbSpeedFlag.SelectedIndex = 6;
            cbAddressEnabled.Checked = false;
            cbCountMode.Text = string.Empty;
            if (_maskWasChanged)
            {
                _maskWasChanged = false;
                btSaveMask.Enabled = false;
            }
            _internalMaskChanging = false;
        }

        private void ClearTemplate()
        {
            tbTemplateName.Text = string.Empty;
            lbMasks.Items.Clear();
        }

        private void MaskButtonEnable()
        {
            AddEquipment equipment = (lbAllMasks.SelectedIndex != -1) ? Program.Manager.Masks[lbAllMasks.SelectedIndex] : null;

            btDelMaskFromTemplate.Enabled = !string.IsNullOrEmpty(tbTemplateName.Text) && (lbMasks.SelectedItem != null);
            btAddMaskToTemplate.Enabled = !string.IsNullOrEmpty(tbTemplateName.Text) && (equipment != null) && !lbMasks.Items.Contains(equipment.Name);

            btDeleteMask.Enabled = (tbMaskName.ReadOnly) && (equipment != null) && lbAllMasks.Items.Contains(equipment.Name);
            btNewMask.Enabled = !_maskWasChanged;

            btDelTemplate.Enabled = (tbTemplateName.ReadOnly) && (!string.IsNullOrEmpty(tbTemplateName.Text)) && (lbTemplates.Items.Contains(tbTemplateName.Text));
        }

        private void MasksToScreen(string templateName, string[] maskNames)
        {
            tbTemplateName.Text = templateName;

            MaskButtonEnable();

            lbMasks.BeginUpdate();
            lbMasks.Items.Clear();
            for (int i = 0; i < maskNames.Length; i++)
            {
                lbMasks.Items.Add(maskNames[i]);
            }
            if (maskNames.Length > 0)
            {
                gbMask.Enabled = true;
                lbMasks.SelectedIndex = 0;
            }
            else
            {
                ClearMask();
                gbMask.Enabled = false;
            }
            lbMasks.EndUpdate();
        }

        private void EquipmentToScreen(AddEquipment equipment)
        {
            _internalMaskChanging = true;
            tbMaskName.Text = equipment.Name;
            tbDescription.Text = equipment.Description;
            tbFuelConsumption.Text = (equipment.FuelConsumption / 100).ToString("0000.00");
            cbSK1.Checked = (equipment.InputMask[0] == 1) || (equipment.InputMask[0] == 2);
            cbSK2.Checked = (equipment.InputMask[1] == 1) || (equipment.InputMask[1] == 2);
            cbSK3.Checked = (equipment.InputMask[2] == 1) || (equipment.InputMask[2] == 2);
            cbSK4.Checked = (equipment.InputMask[3] == 1) || (equipment.InputMask[3] == 2);
            cbSK5.Checked = (equipment.InputMask[4] == 1) || (equipment.InputMask[4] == 2);
            cbSK6.Checked = (equipment.InputMask[5] == 1) || (equipment.InputMask[5] == 2);
            cbSK7.Checked = (equipment.InputMask[6] == 1) || (equipment.InputMask[6] == 2);
            cbSK8.Checked = (equipment.InputMask[7] == 1) || (equipment.InputMask[7] == 2);
            cbnSK1.Checked = (equipment.InputMask[0] == 2);
            cbnSK2.Checked = (equipment.InputMask[1] == 2);
            cbnSK3.Checked = (equipment.InputMask[2] == 2);
            cbnSK4.Checked = (equipment.InputMask[3] == 2);
            cbnSK5.Checked = (equipment.InputMask[4] == 2);
            cbnSK6.Checked = (equipment.InputMask[5] == 2);
            cbnSK7.Checked = (equipment.InputMask[6] == 2);
            cbnSK8.Checked = (equipment.InputMask[7] == 2);

            int state = BitConverter.ToInt32(equipment.StateMask, 0);
            switch (state)
            {
                case 0:
                    cbStateMask.SelectedIndex = 0;
                    break;
                case 256:
                    cbStateMask.SelectedIndex = 1;
                    break;
                case 1:
                    cbStateMask.SelectedIndex = 2;
                    break;
                case 2:
                    cbStateMask.SelectedIndex = 3;
                    break;
                default:
                    cbStateMask.Text = state.ToString("X");
                    break;
            }

            switch (equipment.SpeedConditionFlag)
            {
                case ">":
                    cbSpeedFlag.SelectedIndex = 0;
                    break;
                case ">=":
                    cbSpeedFlag.SelectedIndex = 1;
                    break;
                case "<":
                    cbSpeedFlag.SelectedIndex = 2;
                    break;
                case "<=":
                    cbSpeedFlag.SelectedIndex = 3;
                    break;
                case "=":
                    cbSpeedFlag.SelectedIndex = 4;
                    break;
                case "<>":
                    cbSpeedFlag.SelectedIndex = 5;
                    break;
                default:
                    cbSpeedFlag.SelectedIndex = 6;
                    break;
            }

            cbaSK1.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[0] >= 0) && (equipment.AlgorithmMask[0] <= 2))
                ? (int)equipment.AlgorithmMask[0] : 0;
            cbaSK2.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[1] >= 0) && (equipment.AlgorithmMask[1] <= 2))
                ? (int)equipment.AlgorithmMask[1] : 0;
            cbaSK3.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[2] >= 0) && (equipment.AlgorithmMask[2] <= 2))
                ? (int)equipment.AlgorithmMask[2] : 0;
            cbaSK4.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[3] >= 0) && (equipment.AlgorithmMask[3] <= 2))
                ? (int)equipment.AlgorithmMask[3] : 0;
            cbaSK5.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[4] >= 0) && (equipment.AlgorithmMask[4] <= 2))
                ? (int)equipment.AlgorithmMask[4] : 0;
            cbaSK6.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[5] >= 0) && (equipment.AlgorithmMask[5] <= 2))
                ? (int)equipment.AlgorithmMask[5] : 0;
            cbaSK7.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[6] >= 0) && (equipment.AlgorithmMask[6] <= 2))
                ? (int)equipment.AlgorithmMask[6] : 0;
            cbaSK8.SelectedIndex = ((equipment.AlgorithmMask != null) && (equipment.AlgorithmMask[7] >= 0) && (equipment.AlgorithmMask[7] <= 2))
                ? (int)equipment.AlgorithmMask[7] : 0;

            tbSpeedControl.Text = equipment.SpeedCondition.ToString("0000.00");
            cbAddressEnabled.Checked = equipment.AddressEnabled;
            cbCountMode.SelectedIndex = (equipment.CountMode) ? 1 : 0;

            if (_maskWasChanged)
            {
                _maskWasChanged = false;
                btSaveMask.Enabled = false;
            }
            _internalMaskChanging = false;
            tbMaskName.ReadOnly = true;
        }

        private void btAddTemplateToObj_Click(object sender, EventArgs e)
        {
            string templateName = (lbTemplates.SelectedIndex != -1) ? (string)lbTemplates.Items[lbTemplates.SelectedIndex] : null;
            MonObj currentObj = Program.Manager.GetObjByAvtoNo((string)lbObj.SelectedItem);
            AddEquipmentTemplate template = Program.Manager.GetTemplate(templateName);
            if ((!string.IsNullOrEmpty(templateName)) && (templateName != tbTemplateName2.Text) && (template != null))
            {
                if (template.Equipment.Length == 0)
                {
                    MessageBox.Show("Шабнон пуст. Отсутсвуют маски спец. оборудования. Добавьте маски спец. оборудования в шаблон.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    return;
                }
                if (MessageBox.Show(string.Format("Для машины №'{0}' будет установлен шаблон '{1}' Вы уверены?", currentObj.Avto_no, templateName), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string err;
                    if (Program.Manager.ReplaceTemplateToObjByObjId(currentObj.Objid, templateName, out err))
                    {
                        FillLbObj();
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Шаблон не найден.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        private void btDelAllMaskFromObj_Click(object sender, EventArgs e)
        {
            MonObj currentObj = Program.Manager.GetObjByAvtoNo((string)lbObj.SelectedItem);
            if (currentObj.AddEquipment.Count > 0)
            {
                if (MessageBox.Show(string.Format("Для машины №'{0}' будут удалены все маски спец. оборудования. Вы уверены?", currentObj.Avto_no), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string err;
                    if (Program.Manager.DelAllMasksFromObjByObjId(currentObj.Objid, out err))
                    {
                        FillLbObj();
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format(@"Машина '{0}' не имеет спец. оборудование.", currentObj.Avto_no), "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        private void btAddMaskToTemplate_Click(object sender, EventArgs e)
        {
            AddEquipment equipment = (lbAllMasks.SelectedIndex != -1) ? Program.Manager.Masks[lbAllMasks.SelectedIndex] : null;

            if (!string.IsNullOrEmpty(tbTemplateName.Text) && (equipment != null) && !lbMasks.Items.Contains(equipment.Name))
            {
                if (MessageBox.Show(string.Format("Маска спец. оборудования '{0}' будет добавлена к шаблону '{1}'. Вы уверены?", equipment.Name, tbTemplateName.Text), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string err;
                    if (Program.Manager.AddMaskToTemplate(tbTemplateName.Text, equipment.Name, out err))
                    {
                        lbMasks.Items.Add(equipment.Name);
                        lbMasks.SelectedIndex = lbMasks.Items.Count - 1;
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Маска или шаблон не определены", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        private void btDelMaskFromTemplate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbTemplateName.Text) && lbMasks.SelectedItem != null)
            {
                var maskName = (string)lbMasks.SelectedItem;
                if (lbMasks.Items.Count == 1)
                {
                    MessageBox.Show("Маска '{0}' последняя маска в шаблоне '{1}'. Удаление невозможно", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    return;
                }
                if(MessageBox.Show(string.Format("Маска спец. оборудования '{0}' будет удалена из шаблока '{1}'. Вы уверены?", maskName, tbTemplateName.Text), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string err;
                    if (Program.Manager.DelMaskFromTemplate(tbTemplateName.Text, maskName, out err))
                    {
                        lbMasks.Items.Remove(maskName);
                        if (lbMasks.Items.Count > 0)
                        {
                            lbMasks.SelectedIndex = 0;
                        }
                        else
                        {
                            FillLbTemplates();
                        }
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Маска или шаблон не определены", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }           
        }

        private void btSaveMask_Click(object sender, EventArgs e)
        {
            AddEquipment equipment;
            bool createMode = false;
            if (tbMaskName.ReadOnly == false)
            {
                createMode = true;
                //create mode
                if (string.IsNullOrEmpty(tbMaskName.Text))
                {
                    MessageBox.Show("Введите название маски спец. оборудования", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    tbMaskName.Focus();
                    return;
                }
                equipment = Program.Manager.GetMask(tbMaskName.Text);
                if (equipment != null)
                {
                    MessageBox.Show(string.Format(@"Маска с именем '{0}' уже существует. Измените имя.", tbMaskName.Text), "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    tbMaskName.Focus();
                    return;
                }
                equipment = new AddEquipment {Name = tbMaskName.Text};
            }
            else
            {
                equipment = Program.Manager.GetMask(tbMaskName.Text);
            }

            double r;
            if (!double.TryParse(tbFuelConsumption.Text, out r))
            {
                MessageBox.Show("Некоретное значение в поле 'Расход топлива'.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                tbFuelConsumption.Focus();
                return;
            }
            if (!double.TryParse(tbSpeedControl.Text, out r))
            {
                MessageBox.Show("Некоретное значение в поле 'Скорость'.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                tbSpeedControl.Focus();
                return;
            }

            btSaveMask.Enabled = false;
            _maskWasChanged = false;
            tbMaskName.ReadOnly = true;

            if (equipment != null)
            {
                if (MessageBox.Show(string.Format("Маска спец. оборудования '{0}' будет изменена. Вы уверены?", equipment.Name), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    equipment.Name = tbMaskName.Text;
                    equipment.Description = tbDescription.Text;
                    equipment.FuelConsumption = 100.0 * double.Parse(tbFuelConsumption.Text);
                    equipment.InputMask[0] = cbSK1.Checked ? ((!cbnSK1.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[1] = cbSK2.Checked ? ((!cbnSK2.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[2] = cbSK3.Checked ? ((!cbnSK3.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[3] = cbSK4.Checked ? ((!cbnSK4.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[4] = cbSK5.Checked ? ((!cbnSK5.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[5] = cbSK6.Checked ? ((!cbnSK6.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[6] = cbSK7.Checked ? ((!cbnSK7.Checked) ? (byte)1 : (byte)2) : (byte)0;
                    equipment.InputMask[7] = cbSK8.Checked ? ((!cbnSK8.Checked) ? (byte)1 : (byte)2) : (byte)0;

                    switch (cbStateMask.SelectedIndex)
                    {
                        case 0:
                            equipment.StateMask = BitConverter.GetBytes(0);
                            break;
                        case 1:
                            equipment.StateMask = BitConverter.GetBytes(256);
                            break;
                        case 2:
                            equipment.StateMask = BitConverter.GetBytes(1);
                            break;
                        case 3:
                            equipment.StateMask = BitConverter.GetBytes(2);
                            break;
                    }
                    equipment.SpeedConditionFlag = cbSpeedFlag.Text;

                    equipment.SpeedCondition = double.Parse(tbSpeedControl.Text);
                    equipment.AddressEnabled = cbAddressEnabled.Checked;
                    equipment.CountMode = cbCountMode.SelectedIndex == 1;

                    equipment.AlgorithmMask = new byte[]
                    {
                        (byte)cbaSK1.SelectedIndex,
                        (byte)cbaSK2.SelectedIndex,
                        (byte)cbaSK3.SelectedIndex,
                        (byte)cbaSK4.SelectedIndex,
                        (byte)cbaSK5.SelectedIndex,
                        (byte)cbaSK6.SelectedIndex,
                        (byte)cbaSK7.SelectedIndex,
                        (byte)cbaSK8.SelectedIndex,
                        0, 0, 0, 0, 0, 0, 0, 0
                    };

                    string err;
                    if (Program.Manager.InsertUpdateDSTMask(equipment, out err))
                    {
                        if (createMode)
                        {
                            Program.Manager.AddEquipment(equipment);
                            lbAllMasks.Items.Add(equipment.Name);
                            lbAllMasks.SelectedIndex = lbAllMasks.Items.Count - 1;
                        }
                        EquipmentToScreen(equipment);
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    EquipmentToScreen(equipment);
                }
            }
            else
            {
                MessageBox.Show("Маска не найдена.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        private void btDeleteMask_Click(object sender, EventArgs e)
        {
            AddEquipment equipment = Program.Manager.GetMask(tbMaskName.Text);
            if (equipment != null)
            {
                if (MessageBox.Show(string.Format("Маска спец. оборудования '{0}' будет удалена. Вы уверены?", equipment.Name), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    equipment.Name = tbMaskName.Text;
                    string err;
                    if (Program.Manager.DeleteDSTMask(equipment.Name, out err))
                    {
                        FillLbAllMasks(null);
                        FillLbObj();
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Маска не найдена.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        private void btDelTemplate_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(tbTemplateName.Text) && (lbTemplates.Items.Contains(tbTemplateName.Text)))
            {
                if (MessageBox.Show(string.Format("Шаблон '{0}' будет удален. Маски соответсвующих машин также будут удалены. Вы уверены?", tbTemplateName.Text), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string err;
                    if (Program.Manager.DeleteTemplate(tbTemplateName.Text, out err))
                    {
                        FillLbTemplates();
                        FillLbAllMasks(null);
                        FillLbObj();
                    }
                    else
                    {
                        MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("Шаблон не найден.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        private void btSaveTemplateChanges_Click(object sender, EventArgs e)
        {
            if (!tbTemplateName.ReadOnly)
            {
                if (string.IsNullOrEmpty(tbTemplateName.Text))
                {
                    MessageBox.Show("Введите название шаблона.", "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    tbTemplateName.Focus();
                    return;
                }
                if (lbTemplates.Items.Contains(tbTemplateName.Text))
                {
                    MessageBox.Show(string.Format(@"Шаблон с именем {0} уже существует.", tbTemplateName.Text), "Невозможно выполнить запрос.", MessageBoxButtons.OK);
                    tbTemplateName.Focus();
                    return;
                }
                if (MessageBox.Show(string.Format("Будет создан новый шаблон '{0}'. Вы уверены?", tbTemplateName.Text), "Подтверждение операции.", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    AddEquipmentTemplate template = new AddEquipmentTemplate();
                    template.TemplateName = tbTemplateName.Text;
                    Program.Manager.AddTemplate(template);
                    lbTemplates.Items.Add(tbTemplateName.Text);
                    lbTemplates.SelectedIndex = lbTemplates.Items.Count - 1;
                    tbTemplateName.ReadOnly = true;
                }
            }
            //_templateWasChanged = false;
            btSaveTemplateChanges.Enabled = false;
        }

        private void btNewTemplate_Click(object sender, EventArgs e)
        {
            tbTemplateName.ReadOnly = false;
            tbTemplateName.Text = string.Empty;
            tbTemplateName.Focus();
            lbMasks.Items.Clear();
            ClearMask();
            //_templateWasChanged = true;
            btSaveTemplateChanges.Enabled = true;
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
            if ((sender == cbSK1) && (!cbSK1.Checked))
            {
                cbnSK1.Checked = false;
            }
            if ((sender == cbSK2) && (!cbSK2.Checked))
            {
                cbnSK2.Checked = false;
            }
            if ((sender == cbSK3) && (!cbSK3.Checked))
            {
                cbnSK3.Checked = false;
            }
            if ((sender == cbSK4) && (!cbSK4.Checked))
            {
                cbnSK4.Checked = false;
            }
            if ((sender == cbSK5) && (!cbSK5.Checked))
            {
                cbnSK5.Checked = false;
            }
            if ((sender == cbSK6) && (!cbSK6.Checked))
            {
                cbnSK6.Checked = false;
            }
            if ((sender == cbSK7) && (!cbSK7.Checked))
            {
                cbnSK7.Checked = false;
            }
            if ((sender == cbSK8) && (!cbSK8.Checked))
            {
                cbnSK8.Checked = false;
            }

            cbnSK1.Enabled = cbSK1.Checked;
            cbnSK2.Enabled = cbSK2.Checked;
            cbnSK3.Enabled = cbSK3.Checked;
            cbnSK4.Enabled = cbSK4.Checked;
            cbnSK5.Enabled = cbSK5.Checked;
            cbnSK6.Enabled = cbSK6.Checked;
            cbnSK7.Enabled = cbSK7.Checked;
            cbnSK8.Enabled = cbSK8.Checked;

            if (!_internalMaskChanging)
            {
                btSaveMask.Enabled = true;
                _maskWasChanged = true;
            }
        }

        private void btNewMask_Click(object sender, EventArgs e)
        {
            ClearMask();
            tbFuelConsumption.Text = "0,0";
            tbSpeedControl.Text = "0,0";
            _maskWasChanged = true;
            tbMaskName.ReadOnly = false;
            tbMaskName.Text = string.Empty;
            tbMaskName.Focus();
            btSaveMask.Enabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void btReport_Click(object sender, EventArgs e)
        {
            string err;
            MonObj currentObj = Program.Manager.GetObjByAvtoNo((string)lbObj.SelectedItem);
            DateTime begin = dtFrom.Value;
            DateTime end = dtTo.Value;
            if (Program.Manager.StartReport1(currentObj.Objid, begin, end, cbExtended.Checked, out err))
            {
                var fmProcessing = new ProcessingForm();
                fmProcessing.ShowDialog();

                var report = new StringBuilder();
                report.AppendFormat("Отчет от {0} {1} (с {2} {3} по {4}:{5})",
                                    DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(),
                                    begin.ToLongDateString(), begin.ToLongTimeString(),
                                    end.ToLongDateString(), end.ToLongTimeString());
                
                report.Append(Environment.NewLine);
                report.Append(Environment.NewLine);

                CommonReportData common = Program.Manager.ReportResult.CommonReportData;
                MonPos[] pos = Program.Manager.ReportResult.MonPos;

                if (!string.IsNullOrEmpty(Program.Manager.ReportError))
                {
                    MessageBox.Show(Program.Manager.ReportError, "Ошибка при выполнении запроса.", MessageBoxButtons.OK);
                    return;
                }
                
                if (common != null)
                {
                    const string noData = "[нет данных]";

                    tbMoto.Text = (common.Moto >= 0) ? common.Moto.ToString("00.0000") : noData;
                    tbDist.Text = (common.Dist >= 0) ? common.Dist.ToString("00.0000") : noData;
                    tbX.Text = (common.X >= 0) ? common.X.ToString("00.00000000") : noData;
                    tbY.Text = (common.Y >= 0) ? common.Y.ToString("00.00000000") : noData;
                    tbAddress.Text = !string.IsNullOrEmpty(common.Address) ? common.Address : "Адрес не определен.";

                    report.AppendFormat("Данные с Эвереста:{0}", Environment.NewLine);
                    if(common.EverestMainData.OperationResult == OperationResult.Success)
                    {
                        report.AppendFormat("Имя (AdminName):                         \t{0}{1}", common.EverestMainData.AdminName, Environment.NewLine);
                        report.AppendFormat("Время обеда (DinnerTime):                \t{0}{1}", common.EverestMainData.DinnerTime, Environment.NewLine);
                        report.AppendFormat("Расход топлива (FuelConsumption):        \t{0}{1}", common.EverestMainData.FuelConsumption, Environment.NewLine);
                        report.AppendFormat("Норма расхода топлива (FuelConsumption): \t{0}{1}", common.EverestMainData.FuelConsumptionNorm, Environment.NewLine);
                        report.AppendFormat("Модель (Model):                          \t{0}{1}", common.EverestMainData.Model, Environment.NewLine);
                        report.AppendFormat("WorkGraphTypeName:                       \t{0}{1}", common.EverestMainData.WorkGraphTypeName, Environment.NewLine);
                        report.AppendFormat("Тип машины (VehicleType):                \t{0}{1}", common.EverestMainData.VehicleType == VehicleType.Avto ? "Авто" : "ДСТ", Environment.NewLine);
                    }
                    else
                    {
                        report.AppendFormat("Невозможно выгрузить данные с Эвереста. Ошибка={0}{1}", common.EverestMainData.OperationResult, Environment.NewLine);
                    }

                    report.Append(Environment.NewLine);
                    report.AppendFormat("Дистанция за период:           \t{0} км.{1}", (common.Dist >= 0) ? common.Dist.ToString("00.00") : noData, Environment.NewLine);
                    report.AppendFormat("Моточасы за период:            \t{0} час.{1}", (common.Moto >= 0) ? common.Moto.ToString("00.0000") : noData, Environment.NewLine);
                    string commonCoordinates = ((common.X >= 0) && (common.Y >= 0)) ? string.Format("{0:00.00000000}/{1:00.00000000}", common.X, common.Y) : noData;
                    report.AppendFormat("Кординаты первой точки работы: \t{0}{1}", commonCoordinates, Environment.NewLine);
                    report.AppendFormat("Адрес первой точки работы:     \t{0}{1}", ((!string.IsNullOrWhiteSpace(tbAddress.Text)) && (tbAddress.Text != "-1")) ? tbAddress.Text : noData, Environment.NewLine);
                    report.AppendFormat("Расход топлива на пробег:      \t{0} л. {1}", (common.FuelDepletion >= 0) ? common.FuelDepletion.ToString("000.00") : noData, Environment.NewLine);
                    report.AppendFormat("Расход топлива на моточасы:    \t{0} л. {1}", (common.FuelDepletionAdd >= 0) ? common.FuelDepletionAdd.ToString("000.00") : noData, Environment.NewLine);
                    string totalConsumption = ((common.FuelDepletion >= 0) && (common.FuelDepletionAdd >= 0)) ? (common.FuelDepletion + common.FuelDepletionAdd).ToString("000.00") : noData;
                    report.AppendFormat("Суммарный расход:              \t{0} л. {1}", totalConsumption, Environment.NewLine);
                    report.Append(Environment.NewLine);

                    report.Append("Детализация по спец. оборудованию:");
                    report.Append(Environment.NewLine);
                    report.Append("Название\tМоточасы\tРасход топлива");
                    report.Append(Environment.NewLine);
                    string[] equipments = common.Equipments.Split(new[] { ';' });
                    string[] fuelDetail = common.FuelDetails.Split(new[] { ';' });
                    string[] motoDetail = common.MotoDetails.Split(new[] { ';' });
                    for (int i = 0; i < equipments.Length; i++)
                    {
                        report.AppendFormat("{0}\t{1}\t{2}{3}", equipments[i], motoDetail[i], fuelDetail[i], Environment.NewLine);
                    }

                    if (cbExtended.Checked)
                    {
                        report.Append("Исходные значения для сравнения:");
                        report.Append(Environment.NewLine);
                        report.AppendFormat("Всего записей: {0}{1}", pos.Length, Environment.NewLine);
                        report.AppendFormat("Рассчетное значение дистанции: {0} км.{1}", (MonPos.CalcDist(pos, begin, end)).ToString("0000.00"), Environment.NewLine);

                        if (pos.Length > 0)
                        {
                            report.Append(Environment.NewLine);
                            report.Append("Дата\tВремя\tДата GMT\tВремя GMT\tState\tFormat\tDIST\tSPEED\tСК1\tСК2\tСК3\tСК4\tСК5\tСК6\tСК7\tСК8\tLAT\tLON\tРазница по полю DIST");
                            report.Append(Environment.NewLine);
                            double pDist = 0.0;

                            for (int i = 0; i < pos.Length; i++)
                            {
                                DateTime localGmt = pos[i].Gmt.ToLocalTime();

                                string SK1 = string.Empty;
                                string SK2 = string.Empty;
                                string SK3 = string.Empty;
                                string SK4 = string.Empty;
                                string SK5 = string.Empty;
                                string SK6 = string.Empty;
                                string SK7 = string.Empty;
                                string SK8 = string.Empty;
                                if (pos[i].Input != null)
                                {
                                    SK1 = (pos[i].Input[0] == 1) ? "1" : "0";
                                    SK2 = (pos[i].Input[1] == 1) ? "1" : "0";
                                    SK3 = (pos[i].Input[2] == 1) ? "1" : "0";
                                    SK4 = (pos[i].Input[3] == 1) ? "1" : "0";
                                    SK5 = (pos[i].Input[4] == 1) ? "1" : "0";
                                    SK6 = (pos[i].Input[5] == 1) ? "1" : "0";
                                    SK7 = (pos[i].Input[6] == 1) ? "1" : "0";
                                    SK8 = (pos[i].Input[7] == 1) ? "1" : "0";
                                }
                                string format = string.Empty;
                                if (pos[i].Format != null)
                                {
                                    format = BitConverter.ToInt32(pos[i].Format, 0).ToString("X");
                                }
                                string state = string.Empty;
                                if (pos[i].State != null)
                                {
                                    int st = BitConverter.ToInt32(pos[i].State, 0);
                                    switch (st)
                                    {
                                        case 0:
                                            state = (string)cbStateMask.Items[0];
                                            break;
                                        case 256:
                                            state = (string)cbStateMask.Items[1];
                                            break;
                                        case 1:
                                            state = (string)cbStateMask.Items[2];
                                            break;
                                        case 2:
                                            state = (string)cbStateMask.Items[3];
                                            break;
                                        default:
                                            state = st.ToString("X");
                                            break;
                                    }
                                }
                                string pDistsStr = (i != 0) ? pDist.ToString("00.00000") : "-";

                                pDist = pos[i].Dist;

                                report.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}{19}",
                                    localGmt.ToShortDateString(), localGmt.ToShortTimeString(),
                                    pos[i].Gmt.ToShortDateString(), pos[i].Gmt.ToShortTimeString(),
                                    state, format,
                                    pos[i].Dist.ToString("00.00000"), pos[i].Speed.ToString("000.0"),
                                    SK1, SK2, SK3, SK4, SK5, SK6, SK7, SK8,
                                    pos[i].Lat, pos[i].Lon, pDist,
                                    Environment.NewLine);
                            }
                        }
                    }
                }

                //rt.Text = report.ToString();
                rt.Rtf = TextToRtfText(report.ToString());
            }
            else
            {
                MessageBox.Show(err, "Невозможно выполнить запрос.", MessageBoxButtons.OK);
            }
        }

        public string RtfRow(string[] cell)
        {
            return string.Empty;
        }

        public string TextToRtfText(string text)
        {
            string color = "{\\colortbl ;\\red255\\green0\\blue0;\\red0\\green0\\blue255;\\red153\\green0\\blue0;\\red0\\green0\\blue0;\\red136\\green136\\blue136;}";
            string strBegin = "{\\rtf1\\ansi\\ansicpg1251\\deff0\\deflang1049{\\fonttbl{\\f1\\fnil\\fcharset204 Microsoft Sans Serif;}}" + color + "\\viewkind4\\uc1\\pard\\nowidctlpar\\cf1\\lang1033\\f1\\fs20";
            string strEnd = "}";

            string cr = "\\line ";
            string tab = "\\tab ";
            string blue = "\\cf2 ";
            //string red = "\\cf3 ";
            //string lightRed = "\\cf1 ";
            //string black = "\\cf4 ";
            //string grey = "\\cf5 ";
            //string boldBegin = "{\\b ";
            //string boldEnd = "}";

            text = text.Replace("\t", tab);
            text = text.Replace(Environment.NewLine, cr);

            //return strBegin + blue + "\\clbrdrt\\brdrth \\clbrdrl\\brdrth \\clbrdrb\\brdrdb" +
            //    "\\clbrdrr\\brdrdb \\cellx3636\\clbrdrt\\brdrth" +
            //    "\\clbrdrl\\brdrdb \\clbrdrb\\brdrdb \\clbrdrr\\brdrdb" +
            //    "\\cellx7236\\clbrdrt\\brdrth \\clbrdrl\\brdrdb" +
            //    "\\clbrdrb\\brdrdb \\clbrdrr\\brdrdb \\cellx10836\\pard \\intbl" +
            //    "\\cell \\pard \\intbl \\cell \\pard \\intbl \\cell \\pard \\intbl \\row" +
            //    "\\trowd \\trqc\\trgaph108\\trrh280\\trleft36 \\clbrdrt\\brdrdb" +
            //    "\\clbrdrl\\brdrth \\clbrdrb \\brdrsh\\brdrs \\clbrdrr\\brdrdb" +
            //    "\\cellx3636\\clbrdrt\\brdrdb \\clbrdr \\brdrdb" +
            //    "\\clbrdrb\\brdrsh\\brdrs \\clbrdrr\\brdrdb" +
            //    "\\cellx7236\\clbrdrt\\brdrdb \\clbrdr \\brdrdb" +
            //    "\\clbrdrb\\brdrsh\\brdrs \\clbrdrr\\brdrdb \\cellx10836\\pard" +
            //    "\\intbl \\cellS0\\pard \\intbl \\cellS1\\pard \\intbl \\cellS2\\pard" +
            //    "\\intbl \\row \\pard" + strEnd;
            return strBegin + blue + text + strEnd;
        }

        public int Search(ListBox.ObjectCollection items, char newSymbol)
        {
            var delay = (int)(DateTime.Now - _searchTimestamp).TotalMilliseconds;
            if (delay > 1000)
            {
                _searchString = "" + newSymbol;
            }
            else
            {
                _searchString += newSymbol;
            }
            _searchTimestamp = DateTime.Now;

            _searchString = _searchString.Trim().Replace(" ", string.Empty).Replace("-", string.Empty);

            if (_searchString.Length > 0)
            {
                int index = 0;
                foreach (var itemValue in items)
                {
                    var item = itemValue as string;
                    item = (item ?? string.Empty).Trim().Replace(" ", string.Empty).Replace("-", string.Empty);
                    if ((!string.IsNullOrWhiteSpace(item)) &&
                        (item.StartsWith(_searchString, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return index;
                    }
                    index++;
                }
            }
            return -1;
        }

        private void lbObj_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = char.ToLowerInvariant(e.KeyChar);
            if (SearchCharsSet.Contains(symbol))
            {
                e.Handled = true;
                int index = Search(lbObj.Items, symbol);
                if (index != -1)
                {
                    lbObj.SelectedIndex = index;
                }
            }
        }

        private void lbTemplates_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = char.ToLowerInvariant(e.KeyChar);
            if (SearchCharsSet.Contains(symbol))
            {
                e.Handled = true;
                int index = Search(lbTemplates.Items, symbol);
                if (index != -1)
                {
                    lbTemplates.SelectedIndex = index;
                }
            }
        }

        private void lbAllMasks_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = char.ToLowerInvariant(e.KeyChar);
            if (SearchCharsSet.Contains(symbol))
            {
                e.Handled = true;
                int index = Search(lbAllMasks.Items, symbol);
                if (index != -1)
                {
                    lbAllMasks.SelectedIndex = index;
                }
            }
        }
    }
}