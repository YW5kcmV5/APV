using System;
using System.Linq;
using System.Collections.Generic;
using APV.EntityFramework;
using APV.Common;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class LanguageManagement : BaseManagement<LanguageEntity, LanguageCollection, LanguageDataLayerManager>
    {
        private static bool _inited;
        private static LanguageCollection _items;
        private static readonly SortedList<string, LanguageEntity> Codes = new SortedList<string, LanguageEntity>();
        private static readonly SortedList<string, LanguageEntity> Names = new SortedList<string, LanguageEntity>();

        private static void Init()
        {
            if (!_inited)
            {
                _inited = true;
                var manager = (LanguageDataLayerManager)EntityFrameworkManager.GetManager<LanguageEntity>();
                _items = (LanguageCollection)manager.GetAll();
                foreach (LanguageEntity language in _items)
                {
                    Codes.Add(language.Code.ToUpperInvariant(), language);
                    Names.Add(language.Name.ToUpperInvariant(), language);
                }
            }
        }

        LanguageManagement()
        {
            Init();
        }

        [AnonymousAccess]
        public override LanguageEntity FindByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            int index = Names.IndexOfKey(name.ToUpperInvariant());
            return (index != -1) ? Names.Values[index] : null;
        }

        [AnonymousAccess]
        public override LanguageEntity GetByName(string name)
        {
            LanguageEntity entity = FindByName(name);

            if (entity == null)
                throw new ArgumentOutOfRangeException("name", string.Format("Language with name \"{0}\" does not exist.", name));

            return entity;
        }

        [AnonymousAccess]
        public LanguageEntity FindByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("code");

            int index = Codes.IndexOfKey(code.ToUpperInvariant());
            return (index != -1) ? Codes.Values[index] : null;
        }

        [AnonymousAccess]
        public LanguageEntity GetByCode(string code)
        {
            LanguageEntity entity = FindByCode(code);

            if (entity == null)
                throw new ArgumentOutOfRangeException("code", string.Format("Language with code \"{0}\" does not exist.", code));

            return entity;
        }

        [AnonymousAccess]
        public LanguageEntity Define(string word)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");

            LanguageCollection languages = All;
            foreach (LanguageEntity language in languages)
            {
                if (language.WordChars != null)
                {
                    var hash = new HashSet<char>(language.WordChars);
                    if (word.All(hash.Contains))
                    {
                        return language;
                    }
                }
            }
            return Default;
        }

        [AnonymousAccess]
        public override LanguageCollection GetAll()
        {
            return _items;
        }

        public static readonly LanguageManagement Instance = (LanguageManagement)EntityFrameworkManager.GetManagement<LanguageEntity>();

        public static readonly LanguageEntity Russian = Instance.GetByName(SystemConstants.LanguageNameRussian);

        public static readonly LanguageEntity Default = Instance.GetByCode(SystemConstants.DefaultLanguageCode);

        public static readonly LanguageCollection All = Instance.GetAll();

        public static readonly char[] WordChars = All.GetWordChars();

        public static readonly HashSet<char> WordCharsHashset = new HashSet<char>(WordChars);
    }
}