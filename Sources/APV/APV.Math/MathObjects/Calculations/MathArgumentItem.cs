using System;

namespace APV.Math.MathObjects.Calculations
{
    public sealed class MathArgumentItem : BaseMathCalculationItem
    {
        private readonly string _mnemonicName;
        private bool? _isVariable;
        private IMathObject _constantValue;

        public MathArgumentItem(MathOperationContext context, string mnemonicName)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (mnemonicName == null)
                throw new ArgumentNullException("mnemonicName");
            if (string.IsNullOrWhiteSpace(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", "Mnemonic name is empty or white space.");

            _mnemonicName = mnemonicName;
        }

        public override string GetMnemonicName()
        {
            return string.Format("\"{0}\"", _mnemonicName);
        }

        public override IMathCalculationItem[] GetExecutionList()
        {
            return new[] { (IMathCalculationItem)this };
        }

        public override IMathObject Calculate()
        {
            if (IsVariable)
            {
                return Context.GetVariableValue(_mnemonicName);
            }
            if (_constantValue == null)
            {
                IMathObjectType defaultType = Context.GetDefaultType();
                _constantValue = defaultType.CreateInstance(_mnemonicName);
            }
            return _constantValue;
        }

        public override MathCalculationItemType GetItemType()
        {
            return (IsVariable) ? MathCalculationItemType.Variable : MathCalculationItemType.Constant;
        }

        public string MnemonicName
        {
            get { return _mnemonicName; }
        }

        public bool IsVariable
        {
            get { return (_isVariable ?? (_isVariable = Context.ContainsVariable(_mnemonicName))).Value; }
        }
    }
}