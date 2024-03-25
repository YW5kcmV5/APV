using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using APV.EntityFramework;
using APV.Common.Reflection;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class SettingsManagement : BaseManagement<SettingEntity, SettingCollection, SettingDataLayerManager>
    {
        private struct SettingInfo
        {
            public Action<object> Set;
            public SettingEntity Entity;
        }

        private static bool _inited;
        private static readonly object Lock = new object();
        private static readonly List<SettingInfo> StaticSettings = new List<SettingInfo>();

        private static void ReadValueFromEntity(SettingInfo settingInfo)
        {
            string stringValue = settingInfo.Entity.Value;
            object value;
            switch (settingInfo.Entity.SettingType)
            {
                case SettingType.Boolean:
                    value = bool.Parse(stringValue);
                    break;

                case SettingType.Decimal:
                    value = decimal.Parse(stringValue);
                    break;

                case SettingType.Enum:
                    value = int.Parse(stringValue);
                    break;

                case SettingType.Long:
                    value = long.Parse(stringValue);
                    break;

                case SettingType.String:
                    value = stringValue;
                    break;

                case SettingType.Xml:
                    value = new XmlDocument();
                    ((XmlDocument)value).LoadXml(stringValue);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unknown setting type \"{0}\".", settingInfo.Entity.SettingType));
            }

            settingInfo.Set(value);
        }

        private static void UpdateStaticSettings()
        {
            try
            {
                lock (Lock)
                {
                    int length = StaticSettings.Count;
                    for (int i = 0; i < length; i++)
                    {
                        StaticSettings[i].Entity.Reload();
                        ReadValueFromEntity(StaticSettings[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: log
            }
        }

        private static void Init()
        {
            lock (Lock)
            {
                if (!_inited)
                {
                    _inited = true;
                    var manager = (SettingDataLayerManager)EntityFrameworkManager.GetManager<SettingEntity>();
                    var allSettings = (SettingCollection)manager.GetAll();
                    Type instanceType = typeof(SettingsManagement);
                    foreach (SettingEntity setting in allSettings)
                    {
                        PropertyInfo property = instanceType.GetProperty(setting.Name);
                        if (property != null)
                        {
                            Action<object> set = property.BuildStaticSetAccessor();
                            var settingInfo = new SettingInfo
                                {
                                    Entity = setting,
                                    Set = set,
                                };
                            ReadValueFromEntity(settingInfo);
                            StaticSettings.Add(settingInfo);
                        }
                    }
                }
            }
        }

        SettingsManagement()
        {
            Init();
        }

        [AdminAccess]
        public static void Reload()
        {
            lock (Lock)
            {
                UpdateStaticSettings();
            }
        }

        public static readonly SettingsManagement Instance = (SettingsManagement)EntityFrameworkManager.GetManagement<SettingEntity>();
    }
}