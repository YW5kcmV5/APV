using System;

namespace APV.Math.MathObjects.Expressions
{
    public sealed class MathExpressionArgument : BaseMathExpressionItem
    {
        private readonly string _name;

        public MathExpressionArgument(MathExpressionScope parent, string name)
            : base(parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("name", "name is empty or whitespace.");

            _name = name;
        }

        public override string GetMnemonicName()
        {
            return _name;
        }
    }
}