using System;
using System.Collections.Generic;
using APV.Math.MathObjects.Numbers;

namespace APV.Math.MathObjects.Calculations
{
    public class MathOperationContext : IMathCalculationContext
    {
        private readonly SortedList<string, IMathObject> _variables = new SortedList<string, IMathObject>();
        private readonly object _lock = new object();
        private readonly ulong _id;
        private ulong _lastId;
        private BaseMathCalculationItem _entryPoint;
        private IMathObjectType _defaultType = Int64Number.MathObjectType;

        internal ulong GetNextId()
        {
            lock (_lock)
            {
                _lastId += 100;
            }
            return _lastId;
        }

        public MathOperationContext()
        {
            _id = GetNextId();
        }

        public MathOperationContext(BaseMathCalculationItem entryPoint)
        {
            if (entryPoint == null)
                throw new ArgumentNullException("entryPoint");

            _entryPoint = entryPoint;
            _id = GetNextId();
        }

        public bool ContainsVariable(string variableName)
        {
            return (FindVariableValue(variableName) != null);
        }

        public IMathObject FindVariableValue(string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentOutOfRangeException("variableName", "Variable name is empty or whitespace.");

            lock (_variables)
            {
                int index = _variables.IndexOfKey(variableName);
                return (index != -1) ? _variables.Values[index] : null;
            }
        }

        public IMathObject GetVariableValue(string variableName)
        {
            IMathObject value = FindVariableValue(variableName);

            if (value == null)
                throw new ArgumentOutOfRangeException("variableName", string.Format("Variable with name \"{0}\" is not initialized.", variableName));

            return value;
        }

        public void SetVariableValue(string variableName, IMathObject value)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentOutOfRangeException("variableName", "Variable name is empty or whitespace.");

            lock (_variables)
            {
                if (_variables.ContainsKey(variableName))
                {
                    _variables[variableName] = value;
                }
                else
                {
                    _variables.Add(variableName, value);
                }
            }
        }

        public IMathObject ParseConstant(string constantValue)
        {
            return _defaultType.CreateInstance(constantValue);
        }

        public IMathObjectType GetDefaultType()
        {
            return _defaultType;
        }

        public void SetDefaultType(IMathObjectType defaultType)
        {
            if (defaultType == null)
                throw new ArgumentNullException("defaultType");

            _defaultType = defaultType;
        }

        public void SetEntryPoint(BaseMathCalculationItem entryPoint)
        {
            if (entryPoint == null)
                throw new ArgumentNullException("entryPoint");

            _entryPoint = entryPoint;
        }

        public ulong GetId()
        {
            return _id;
        }

        public IMathCalculationContext GetContext()
        {
            return this;
        }

        public IMathObject Calculate()
        {
            if (_entryPoint == null)
                throw new InvalidOperationException("Entry point is node specified.");

            return _entryPoint.Calculate();
        }

        public MathCalculationItemType GetItemType()
        {
            return MathCalculationItemType.Context;
        }

        public IMathCalculationItem[] GetExecutionList()
        {
            if (_entryPoint == null)
                throw new InvalidOperationException("Entry point is node specified.");

            return _entryPoint.GetExecutionList();
        }

        public string GetMnemonicName()
        {
            return (_entryPoint != null) ? _entryPoint.GetMnemonicName() : string.Format("#{0}:[]", _id);
        }
    }
}