﻿using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class XmlManagement : BaseManagement<DataXmlEntity, DataXmlCollection, XmlDataLayerManager>
    {

        public static readonly XmlManagement Instance = (XmlManagement)EntityFrameworkManager.GetManagement<DataXmlEntity>();
    }
}