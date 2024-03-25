using System;
using System.Diagnostics;

namespace APV.Math.MathObjects.Expressions
{
    [DebuggerDisplay("{GetMnemonicName()}")]
    public abstract class BaseMathExpressionItem : IMnemonicName
    {
        private MathExpressionScope _parent;

        protected BaseMathExpressionItem()
        {
        }

        protected BaseMathExpressionItem(MathExpressionScope parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            SetParent(parent);
        }

        public void ClearParent()
        {
            SetParent(null);
        }

        public void SetParent(MathExpressionScope parent)
        {
            _parent = parent;
        }

        public MathExpressionScope GetParent()
        {
            return _parent;
        }

        public abstract string GetMnemonicName();
    }
}