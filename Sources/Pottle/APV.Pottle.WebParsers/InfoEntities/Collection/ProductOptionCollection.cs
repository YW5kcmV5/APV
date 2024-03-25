using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using APV.Pottle.Common;
using APV.Pottle.Common.Extensions;

namespace APV.Pottle.WebParsers.InfoEntities.Collection
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class ProductOptionCollection
    {
        public sealed class CharacteristicsCollection : Dictionary<string, ProductCharacteristicInfo>
        {
            private readonly ProductOptionCollection _source;

            internal CharacteristicsCollection(ProductOptionCollection source)
            {
                _source = source;
            }

            public string Color
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.ColorName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.ColorName, value); }
            }

            public bool HasColor
            {
                get { return (!string.IsNullOrWhiteSpace(Color)); }
            }

            public string Age
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.AgeName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.AgeName, value); }
            }

            public string Structure
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.StructureName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.StructureName, value); }
            }

            public string Size
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.SizeName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.SizeName, value); }
            }

            public bool HasSize
            {
                get { return (!string.IsNullOrWhiteSpace(Size)); }
            }

            public string PackingSize
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.PackingSizeName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.PackingSizeName, value); }
            }

            public string Weight
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.WeightName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.WeightName, value); }
            }

            public string Mode
            {
                get { return _source.GetCharacteristicValue(ProductCharacteristicInfo.ModeName); }
                set { _source.SetCharacteristicValue(ProductCharacteristicInfo.ModeName, value); }
            }

            public void Set()
            {
                Clear();
                var items = _source._options.Values.OfType<ProductCharacteristicInfo>();
                foreach (ProductCharacteristicInfo info in items)
                {
                    Add(info.Name, info);
                }
            }
        }

        private SortedList<string, BaseProductOptionInfo> _options;
        private ProductCharacteristicModifiers _modifiers;
        private CharacteristicsCollection _characteristics;
        private BaseProductOptionInfo[] _items;

        private string GetHash(ProductOptionType optionType, string name)
        {
            return string.Format("{0}:{1}", optionType, name);
        }

        private void ClearCache()
        {
            _characteristics.Clear();
            _items = null;
        }

        private bool CanAddModifier(ProductCharacteristicModifier modifier, out string error)
        {
            error = null;
            string name = ProductCharacteristicInfo.GetName(modifier);
            string value = GetCharacteristicValue(name);
            if (!string.IsNullOrEmpty(value))
            {
                error = string.Format("Characteristic modifier \"{0}\" can not be added because there is characteristic \"{0}\" with value \"{1}\".", name, value);
                return false;
            }
            return true;
        }

        private void ValidateModifiers(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                ProductCharacteristicModifier modifier = ProductCharacteristicInfo.GetModifier(name);
                if ((modifier != ProductCharacteristicModifier.None) && (_modifiers.Has(modifier)))
                    throw new InvalidOperationException(string.Format("Characteristic with name \"{0}\" and value \"{1}\" can not be added because modifier \"{2}\" is specified.", name, value, modifier));
            }
        }

        public ProductOptionCollection()
        {
            _options = new SortedList<string, BaseProductOptionInfo>();
            _characteristics = new CharacteristicsCollection(this);
        }

        public ProductOptionCollection(ProductOptionCollection collection)
            : this()
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _characteristics = new CharacteristicsCollection(this);
            _options = new SortedList<string, BaseProductOptionInfo>(collection._options);
            _modifiers = collection._modifiers;
        }

        public bool Contains(ProductOptionType optionType, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            string hash = GetHash(optionType, name);
            return _options.ContainsKey(hash);
        }

        public BaseProductOptionInfo Get(ProductOptionType optionType, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            string hash = GetHash(optionType, name);
            int index = _options.IndexOfKey(hash);
            return (index != -1) ? _options.Values[index] : null;
        }

        public void Set(BaseProductOptionInfo option)
        {
            if (option == null)
                throw new ArgumentNullException("option");

            if (option is ProductCharacteristicInfo)
            {
                ValidateModifiers(option.Name, option.Value);
            }
            string hash = GetHash(option.OptionType, option.Name);
            if (_options.ContainsKey(hash))
            {
                _options[hash] = option;
            }
            else
            {
                _options.Add(hash, option);
                ClearCache();
            }
        }

        public bool ContainsOption(ProductOptionType optionType)
        {
            return Contains(optionType, optionType.ToString());
        }

        public string GetOptionValue(ProductOptionType optionType)
        {
            BaseProductOptionInfo option = Get(optionType, optionType.ToString());
            return (option != null) ? option.Value : null;
        }

        public void SetOptionValue(ProductOptionType optionType, string value)
        {
            string hash = GetHash(optionType, optionType.ToString());
            if (_options.ContainsKey(hash))
            {
                _options[hash].Value = value;
            }
            else
            {
                _options.Add(hash, new ProductOptionInfo(optionType) { Value = value });
                ClearCache();
            }
        }

        public bool ContainsCharacteristic(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return Contains(ProductOptionType.Characteristic, name);
        }

        public string GetCharacteristicValue(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            BaseProductOptionInfo option = Get(ProductOptionType.Characteristic, name);
            return (option != null) ? option.Value : null;
        }

        public void SetCharacteristicValue(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            ValidateModifiers(name, value); 
            string hash = GetHash(ProductOptionType.Characteristic, name);
            if (_options.ContainsKey(hash))
            {
                _options[hash].Value = value;
            }
            else
            {
                _options.Add(hash, new ProductCharacteristicInfo(name) { Value = value });
                ClearCache();
            }
        }

        public void Clear()
        {
            //For serailization
            _options = _options ?? new SortedList<string, BaseProductOptionInfo>();
            _characteristics = _characteristics ?? new CharacteristicsCollection(this);

            _modifiers = ProductCharacteristicModifiers.None;
            _options.Clear();
            ClearCache();
        }

        public void AddModifier(ProductCharacteristicModifier modifier)
        {
            string error;
            if (!CanAddModifier(modifier, out error))
                throw new ArgumentOutOfRangeException("modifier", error);

            _modifiers |= (ProductCharacteristicModifiers) modifier;
        }

        public void DeleteModifier(ProductCharacteristicModifier modifier)
        {
            _modifiers = _modifiers.Not(modifier);
        }

        [IgnoreDataMember]
        public string Description
        {
            get { return GetOptionValue(ProductOptionType.Description); }
            set { SetOptionValue(ProductOptionType.Description, value); }
        }

        [IgnoreDataMember]
        public string AdditionalInfo
        {
            get { return GetOptionValue(ProductOptionType.AdditionalInfo); }
            set { SetOptionValue(ProductOptionType.AdditionalInfo, value); }
        }

        [IgnoreDataMember]
        public string Complectation
        {
            get { return GetOptionValue(ProductOptionType.Complectation); }
            set { SetOptionValue(ProductOptionType.Complectation, value); }
        }

        [IgnoreDataMember]
        public string Instruction
        {
            get { return GetOptionValue(ProductOptionType.Instruction); }
            set { SetOptionValue(ProductOptionType.Instruction, value); }
        }

        [IgnoreDataMember]
        public string Warning
        {
            get { return GetOptionValue(ProductOptionType.Warning); }
            set { SetOptionValue(ProductOptionType.Warning, value); }
        }

        [IgnoreDataMember]
        public int Count
        {
            get { return _options.Count; }
        }

        [DataMember]
        public BaseProductOptionInfo[] Items
        {
            get { return (_items ?? (_items = _options.Values.ToArray())); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                Clear();
                foreach (BaseProductOptionInfo option in value)
                {
                    Set(option);
                }
            }
        }
        
        [IgnoreDataMember]
        public CharacteristicsCollection Characteristics
        {
            get
            {
                if (_characteristics.Count != _options.Count)
                {
                    _characteristics.Set();
                }
                return _characteristics;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                Clear();
                foreach (ProductCharacteristicInfo option in value.Values)
                {
                    Set(option);
                }
            }
        }

        [DataMember]
        public ProductCharacteristicModifiers Modifiers
        {
            get { return _modifiers; }
            set
            {
                if (_modifiers != value)
                {
                    var newModifiers = ProductCharacteristicModifiers.None;
                    List<ProductCharacteristicModifier> modifiers = value.ToList();
                    foreach (ProductCharacteristicModifier modifier in modifiers)
                    {
                        string error;
                        if (!CanAddModifier(modifier, out error))
                            throw new ArgumentOutOfRangeException("value", error);

                        newModifiers |= (ProductCharacteristicModifiers) modifier;
                    }
                    _modifiers = newModifiers;
                }
            }
        }
    }
}