using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Windows.Forms;
using APV.TransControl.Core.Application;
using APV.TransControl.Core.DataLayer;
using APV.TransControl.Core.Entities;
using APV.TransControl.ExportReport.ReportData;

namespace APV.TransControl.ExportReport
{
    public class ReportManager
    {
        private bool _isValidConnection;

        private bool _inited;
        private string _initStateDescription;
        private int _initState;
        private bool _isInitFinished;

        private bool _reportFinished;
        private string _reportErr;
        private readonly ReportContainer _reportResult = new ReportContainer();
        
        private MonObj[] _obj;
        private AddEquipmentTemplateItem[] _templatesItem = new AddEquipmentTemplateItem[0];
        private AddEquipment[] _masks = new AddEquipment[0];
        private AddEquipmentTemplate[] _tepmlates = new AddEquipmentTemplate[0];

        private readonly List<string> _models = new List<string>();

        private static string GetAppKey(string key)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                return (element != null) ? element.Value : null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private static void SetAppKey(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                element.Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void ReadSettingFromAppConfig()
        {
            ConnectionSettings connectionSettings = ContextManager.ConnectionSettings;

            string hostValue = GetAppKey("Host");
            string portValue = GetAppKey("Port");
            string dbNameValue = GetAppKey("DBName");
            string usernameValue = GetAppKey("Username");
            string passwordValue = GetAppKey("Password");
            string useTnsDBNameValue = GetAppKey("UseTnsDBName");

            //string dataSource, connString;
            if (!string.IsNullOrWhiteSpace(hostValue))
            {
                connectionSettings.Host = hostValue;
            }
            int port;
            if ((!string.IsNullOrWhiteSpace(portValue)) && (int.TryParse(portValue, out port)))
            {
                connectionSettings.Port = port;
            }
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
            bool useTnsDBName;
            if ((!string.IsNullOrWhiteSpace(useTnsDBNameValue)) && (bool.TryParse(GetAppKey("UseTnsDBName"), out useTnsDBName)))
            {
                connectionSettings.UseTnsDBName = useTnsDBName;
            }

            ContextManager.ConnectionSettings = connectionSettings;
        }

        private void SaveSettingsToAppConfig()
        {
            SetAppKey("Host", ContextManager.ConnectionSettings.Host);
            SetAppKey("Port", ContextManager.ConnectionSettings.Port.ToString(CultureInfo.InvariantCulture));
            SetAppKey("DBName", ContextManager.ConnectionSettings.DBName);
            SetAppKey("Username", ContextManager.ConnectionSettings.Username);
            SetAppKey("Password", ContextManager.ConnectionSettings.Password);
            SetAppKey("UseTnsDBName", ContextManager.ConnectionSettings.UseTnsDBName.ToString());
        }

        public static string ExceptionToStr(Exception ex)
        {
            string str = string.Empty;
            if (ex != null)
            {
                str = string.Format("{0}{1}{2}{3}", ex, Environment.NewLine, ex.Message, Environment.NewLine);
                if (ex.InnerException != null)
                {
                    str += string.Format("{0}{1}{2}{3}", ex.InnerException, Environment.NewLine, ex.InnerException.Message, Environment.NewLine);
                }
            }
            return str;
        }

        public bool CheckConnection(out string errMsg)
        {
            _isValidConnection = DbManager.CheckConnection(out errMsg);
            if (_isValidConnection)
            {
                SaveSettingsToAppConfig();
            }
            return _isValidConnection;
        }

        private void ReadAllTemplates()
        {
            if (IsValidConnection)
            {
                _templatesItem = DbManager.ReadAllTemplates();
            }
        }

        private void ReadAllMask()
        {
            if (IsValidConnection)
            {
                _masks = DbManager.ReadAllMasks();
            }
        }

        private void ReadAllObj()
        {
            if (IsValidConnection)
            {
                _obj = DbManager.ReadAllObjs();

                string[] models = _obj
                    .Where(item => !string.IsNullOrWhiteSpace(item.Avto_model))
                    .Select(item => item.Avto_model)
                    .Distinct()
                    .OrderBy(item => item)
                    .ToArray();

                _models.Clear();
                _models.AddRange(models);
            }
        }

        private void ReadAllObjMask()
        {
            if (IsValidConnection)
            {
                DbManager.FillObjMasks(_obj, _masks);
            }
        }

        private void InitThread(object obj)
        {
            try
            {
                _inited = false;
                _isInitFinished = false;
                _initState = 1;
                _initStateDescription = "Подключение... 1/6";
                if (IsValidConnection)
                {
                    _initState = 2;
                    _initStateDescription = "Чтение шаблонов... 2/6";
                    ReadAllTemplates();
                    _initState = 3;
                    _initStateDescription = "Чтение масок... 3/6";
                    ReadAllMask();
                    _tepmlates = AddEquipmentTemplate.CollectTemplates(_masks, _templatesItem);
                    _initState = 4;
                    _initStateDescription = "Загрузка объектов... 4/6";
                    ReadAllObj();
                    _initState = 5;
                    _initStateDescription = "Загрузка объектов... 5/6";
                    ReadAllObjMask();
                    _inited = true;
                }
                _initState = 6;
                _initStateDescription = "Выполнено.";
                _isInitFinished = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ExceptionToStr(ex), "Исключение. Невозможно загрузить данные.", MessageBoxButtons.OK);
                _isInitFinished = true;
            }
        }

        public ReportManager()
        {
            ReadSettingFromAppConfig();
        }

        public void Init()
        {
            ThreadPool.QueueUserWorkItem(InitThread);
        }

        public bool ReplaceTemplateToObjByObjId(int objId, string templateName, out string errMsg)
        {
            errMsg = string.Empty;

            MonObj obj = GetObjByObjId(objId);
            AddEquipmentTemplate template = GetTemplate(templateName);
            if (obj == null)
            {
                errMsg = string.Format("Объект Id={0} не найден.", objId);
                return false;
            }
            if (template == null)
            {
                errMsg = string.Format("Шаблон '{0}' не найден.", templateName);
                return false;
            }

            try
            {
                int res = DbManager.ReplaceTemplateToObjByObjId(objId, templateName);
                switch (res)
                {
                    case 0:
                        obj.AddEquipment.Clear();
                        obj.AddEquipmentName.Clear();
                        obj.AddEquipmentName.AddRange(template.AddEquipmentName);
                        obj.AddEquipment.AddRange(template.Equipment);
                        return true;
                    case -1:
                        errMsg = string.Format(@"Объект Id={0} или шаблон с имененем '{1}' не найден.", objId, templateName);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool DelAllMasksFromObjByObjId(int objId, out string errMsg)
        {
            errMsg = string.Empty;

            MonObj obj = GetObjByObjId(objId);
            if (obj == null)
            {
                errMsg = string.Format("Объект Id={0} не найден.", objId);
                return false;
            }

            try
            {
                int res = DbManager.DelAllMasksFromObjByObjId(objId);
                switch (res)
                {
                    case 0:
                        obj.AddEquipment.Clear();
                        obj.AddEquipmentName.Clear();
                        return true;
                    case -1:
                        errMsg = string.Format(@"Объект Id={0} не найден.", objId);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool AddMaskToTemplate(string templateName, string maskName, out string errMsg)
        {
            errMsg = string.Empty;

            AddEquipmentTemplate template = GetTemplate(templateName);
            AddEquipment mask = GetMask(maskName);
            if (template == null)
            {
                errMsg = string.Format("Шаблон '{0}' не найден.", templateName);
                return false;
            }
            if (mask == null)
            {
                errMsg = string.Format("Маска спец. оборудования '{0}' не найден.", maskName);
                return false;
            }

            try
            {
                int res = DbManager.AddMaskToTemplate(templateName, maskName);
                switch (res)
                {
                    case 0:
                        for (int i = 0; i < _obj.Length; i++)
                        {
                            if (GetTemplateNameByObjId(_obj[i].Objid) == templateName)
                            {
                                string err;
                                if (ReplaceTemplateToObjByObjId(_obj[i].Objid, templateName, out err))
                                {
                                    _obj[i].AddEquipmentName.Add(maskName);
                                    _obj[i].AddEquipment.Add(mask);
                                }
                                else
                                {
                                    errMsg += string.Format(@"Ошибка при обновлении шаблона для машины №{0} ({1}).", _obj[i].Avto_no, err);
                                }
                            }
                        }
                        template.AddEquipment(mask);
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            return false;
                        }
                        return true;
                    case -1:
                        errMsg = string.Format(@"Шаблон '{0}' или маска '{1}' не найден.", templateName, maskName);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool DelMaskFromTemplate(string templateName, string maskName, out string errMsg)
        {
            errMsg = string.Empty;

            AddEquipmentTemplate template = GetTemplate(templateName);
            AddEquipment mask = GetMask(maskName);
            if (template == null)
            {
                errMsg = string.Format("Шаблон '{0}' не найден.", templateName);
                return false;
            }
            if (mask == null)
            {
                errMsg = string.Format("Маска спец. оборудования '{0}' не найден.", maskName);
                return false;
            }

            try
            {
                int res = DbManager.DelMaskFromTemplate(templateName, maskName);
                switch (res)
                {
                    case 0:
                        for (int i = 0; i < _obj.Length; i++)
                        {
                            if (GetTemplateNameByObjId(_obj[i].Objid) == templateName)
                            {
                                string err;
                                if (ReplaceTemplateToObjByObjId(_obj[i].Objid, templateName, out err))
                                {
                                    int k = -1;
                                    for (int j = 0; j < _obj[i].AddEquipment.Count; j++)
                                    {
                                        if (_obj[i].AddEquipment[j].Name == maskName)
                                        {
                                            k = j;
                                            break;
                                        }
                                    }
                                    if (k != -1)
                                    {
                                        _obj[i].AddEquipmentName.RemoveAt(k);
                                        _obj[i].AddEquipment.RemoveAt(k);
                                    }
                                }
                                else
                                {
                                    errMsg += string.Format(@"Ошибка при удалении шаблона для машины №{0} ({1}).", _obj[i].Avto_no, err);
                                }
                            }
                        }
                        template.DelEquipment(mask);
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            return false;
                        }
                        return true;
                    case -1:
                        errMsg = string.Format(@"Шаблон '{0}' или маска '{1}' не найден.", templateName, maskName);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool InsertUpdateDSTMask(AddEquipment equipment, out string errMsg)
        {
            errMsg = string.Empty;

            if (equipment == null)
            {
                errMsg = "Маска спец. оборудования не указана.";
                return false;
            }

            try
            {
                int res = DbManager.InsertUpdateDSTMask(equipment);
                switch (res)
                {
                    case 0:
                        for (int i = 0; i < _masks.Length; i++)
                        {
                            if (_masks[i].Name == equipment.Name)
                            {
                                _masks[i] = equipment;
                            }
                        }
                        for (int i = 0; i < _tepmlates.Length; i++)
                        {
                            for (int j = 0; j < _tepmlates[i].Equipment.Length; j++)
                            {
                                if (_tepmlates[i].Equipment[j].Name == equipment.Name)
                                {
                                    _tepmlates[i].Equipment[j] = equipment;
                                }
                            }
                        }
                        return true;
                    case -1:
                        errMsg = string.Format(@"Макса '{0}' не найдена.", equipment.Name);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool DeleteDSTMask(string maskName, out string errMsg)
        {
            errMsg = string.Empty;

            AddEquipment equipment = GetMask(maskName);
            if (equipment == null)
            {
                errMsg = string.Format("Маска спец. оборудования '{0}' не найден.", maskName);
                return false;
            }

            try
            {
                int res = DbManager.DeleteDSTMask(maskName);
                switch (res)
                {
                    case 0:
                        for (int i = 0; i < _tepmlates.Length; i++)
                        {
                            _tepmlates[i].DelEquipment(equipment);
                        }
                        int k;
                        for (int i = 0; i < _obj.Length; i++)
                        {
                            k = -1;
                            for (int j = 0; j < _obj[i].AddEquipment.Count; j++)
                            {
                                if (_obj[i].AddEquipment[j].Name == equipment.Name)
                                {
                                    k = j;
                                    break;
                                }
                            }
                            if (k != -1)
                            {
                                _obj[i].AddEquipment.RemoveAt(k);
                                _obj[i].AddEquipmentName.RemoveAt(k);
                            }
                        }
                        List<AddEquipment> masks = new List<AddEquipment>(_masks);
                        k = -1;
                        for (int i = 0; i < masks.Count; i++)
                        {
                            if (masks[i].Name == equipment.Name)
                            {
                                k = i;
                                break;
                            }
                        }
                        if (k != -1)
                        {
                            masks.RemoveAt(k);
                        }
                        _masks = masks.ToArray();
                        return true;
                    case -1:
                        errMsg = string.Format(@"Макса '{0}' не найдена.", maskName);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public bool DeleteTemplate(string templateName, out string errMsg)
        {
            errMsg = string.Empty;

            AddEquipmentTemplate template = GetTemplate(templateName);
            if (template == null)
            {
                errMsg = string.Format("Шаблон '{0}' не найден.", templateName);
                return false;
            }

            try
            {
                string err = string.Empty;
                for (int i = 0; i < _obj.Length; i++)
                {
                    if (GetTemplateNameByObjId(_obj[i].Objid) == templateName)
                    {
                        string str;
                        if (!DelAllMasksFromObjByObjId(_obj[i].Objid, out str))
                        {
                            err += err + " " + errMsg;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(err))
                {
                    errMsg = string.Format(@"Ошибка при удалении шаблонов с машин ({0}).", err);
                    return false;
                }

                int res = DbManager.DeleteTemplate(templateName);
                switch (res)
                {
                    case 0:
                        int k = -1;
                        for (int i = 0; i < _tepmlates.Length; i++)
                        {
                            if (_tepmlates[i].TemplateName == templateName)
                            {
                                k = i;
                                break;
                            }
                        }
                        if (k != -1)
                        {
                            var templates = new List<AddEquipmentTemplate>(_tepmlates);
                            templates.RemoveAt(k);
                            _tepmlates = templates.ToArray();
                        }
                        return true;
                    case -1:
                        errMsg = string.Format(@"Шаблон '{0}' не найден.", templateName);
                        return false;
                    case -2:
                        errMsg = "Хранимая процедура вернула исключение.";
                        return false;
                    default:
                        errMsg = "Хранимая процедура вернула неопределённое значение.";
                        return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        private void ReportThread(object obj)
        {
            _reportErr = string.Empty;
            var prms = obj as ReportParams;
            if (prms != null)
            {
                try
                {
                    double p_imp_run = 0.0;
                    double p_imp_fueldepletion = 0.0;
                    double p_imp_workeadd = 0.0;
                    double p_imp_fueldepletionadd = 0.0;
                    double p_coord_x = 0.0;
                    double p_coord_y = 0.0;
                    string p_imp_address = string.Empty;
                    DateTime p_imp_stardate = DateTime.Now;
                    DateTime p_imp_enddate = DateTime.Now;

                    double moto;
                    double fuel;
                    string equipments;
                    string motoDetails;
                    string fuelDetails;

                    DbManager.GetStkData(prms.AvtoNo, prms.Begin, prms.End, out p_imp_run, out p_imp_fueldepletion,
                        out p_imp_workeadd, out p_imp_fueldepletionadd, out p_coord_x, out p_coord_y,
                        out p_imp_address, out p_imp_stardate, out p_imp_enddate);

                    DbManager.CalcDetailDSTMotoByAvtoNo(prms.AvtoNo, prms.Begin, prms.End, out moto, out fuel,
                        out equipments, out motoDetails, out fuelDetails);

                    EverestMainData everestMainData = DbManager.GetMainDataFromEverestByAvtoNo(prms.AvtoNo, prms.Begin);

                    var pos = new MonPos[0];
                    if (prms.ExtendedData)
                    {
                        pos = DbManager.GetAllMonPos(prms.ObjId, prms.Begin, prms.End);
                    }

                    var data = new CommonReportData
                        {
                            Begin = prms.Begin,
                            End = prms.End,
                            Moto = p_imp_workeadd,
                            Dist = p_imp_run,
                            X = p_coord_x,
                            Y = p_coord_y,
                            Address = p_imp_address,
                            FuelDepletion = p_imp_fueldepletion,
                            FuelDepletionAdd = p_imp_fueldepletionadd,
                            Equipments = equipments,
                            MotoDetails = motoDetails,
                            FuelDetails = fuelDetails,
                            EverestMainData = everestMainData
                        };

                    _reportResult.Add(data);
                    _reportResult.Add(pos);
                }
                catch (Exception ex)
                {
                    _reportErr = ExceptionToStr(ex);
                }
            }
            _reportFinished = true;
        }

        public bool StartReport1(int objId, DateTime begin, DateTime end, bool extendedData, out string err)
        {
            err = string.Empty;
            _reportFinished = false;

            MonObj obj = GetObjByObjId(objId);
            if (obj == null)
            {
                err = string.Format("Машина (объект) не найдена.");
                return false;
            }

            var prms = new ReportParams
                {
                    Begin = begin,
                    End = end,
                    ObjId = obj.Objid,
                    AvtoNo = obj.Avto_no,
                    ExtendedData = extendedData
                };
            _reportResult.Clear();
            ThreadPool.QueueUserWorkItem(ReportThread, prms);
            return true;
        }

        public MonObj GetObjByObjId(int objId)
        {
            for (int i = 0; i < _obj.Length; i++)
            {
                if (_obj[i].Objid == objId)
                {
                    return _obj[i];
                }
            }
            return null;
        }

        public MonObj GetObjByAvtoNo(string avtoNo)
        {
            if (avtoNo != null)
            {
                avtoNo = avtoNo.Replace("*", string.Empty);
                avtoNo = avtoNo.Replace("!", string.Empty);
                avtoNo = avtoNo.Replace(" ", string.Empty);
                for (int i = 0; i < _obj.Length; i++)
                {
                    string no = _obj[i].Avto_no;
                    no = no.Replace("*", string.Empty);
                    no = no.Replace("!", string.Empty);
                    no = no.Replace(" ", string.Empty);
                    if (no == avtoNo)
                    {
                        return _obj[i];
                    }
                }
            }
            return null;
        }

        public AddEquipmentTemplate GetTemplate(string templateName)
        {
            for(int i = 0; i < _tepmlates.Length; i++)
            {
                if (templateName == _tepmlates[i].TemplateName)
                {
                    return _tepmlates[i];
                }
            }
            return null;
        }

        public AddEquipment GetMask(string maskName)
        {
            for (int i = 0; i < _masks.Length; i++)
            {
                if (maskName == _masks[i].Name)
                {
                    return _masks[i];
                }
            }
            return null;
        }

        public void AddEquipment(AddEquipment equipment)
        {
            var masks = new List<AddEquipment>(_masks) { equipment };
            _masks = masks.ToArray();
        }

        public void AddTemplate(AddEquipmentTemplate template)
        {
            var templates = new List<AddEquipmentTemplate>(_tepmlates) { template };
            _tepmlates = templates.ToArray();
        }

        public string GetTemplateNameByMask(string[] maskNames)
        {
            if ((maskNames != null) && (maskNames.Length > 0))
            {
                var names = new List<string>(maskNames);
                for (int i = 0; i < _tepmlates.Length; i++)
                {
                    bool b = true;
                    for (int j = 0; j < _tepmlates[i].Equipment.Length; j++)
                    {
                        if (!names.Contains(_tepmlates[i].Equipment[j].Name))
                        {
                            b = false;
                            break;
                        }
                    }
                    if ((b) && (_tepmlates[i].Equipment.Length == maskNames.Length))
                    {
                        return _tepmlates[i].TemplateName;
                    }
                }
            }
            return string.Empty;
        }

        public string GetTemplateNameByObjId(int objId)
        {
            for (int i = 0; i < _obj.Length; i++)
            {
                if (_obj[i].Objid == objId)
                {
                    return GetTemplateNameByMask(_obj[i].AddEquipmentName.ToArray());
                }
            }
            return string.Empty;
        }

        public bool IsValidConnection
        {
            get
            {
                return _isValidConnection;
            }
        }

        public bool Inited
        {
            get
            {
                return _inited;
            }
        }

        public bool IsInitFinished
        {
            get
            {
                return _isInitFinished;
            }
        }

        public string InitStateDescription
        {
            get
            {
                return _initStateDescription;
            }
        }

        public int InitState
        {
            get
            {
                return _initState;
            }
        }

        public MonObj[] Obj
        {
            get { return _obj; }
        }

        public List<string> Models
        {
            get { return _models; }
        }

        public AddEquipmentTemplate[] Templates
        {
            get { return _tepmlates; }
        }

        public AddEquipment[] Masks
        {
            get { return _masks; }
        }

        public bool ReportFinished
        {
            get { return _reportFinished; }
        }

        public string ReportError
        {
            get { return _reportErr; }
        }

        public ReportContainer ReportResult
        {
            get { return _reportResult; }
        }
    }
}