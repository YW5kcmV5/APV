using System;
using APV.Math.MathObjects.Functions;

namespace APV.Math.MathObjects.Expressions
{
    public sealed class MathExpressionOperation : BaseMathExpressionItem
    {
        private readonly MathFunctionType _function;

        public MathExpressionOperation(MathExpressionScope parent, MathFunctionType function)
            : base(parent)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            _function = function;
        }

        public override string GetMnemonicName()
        {
            return _function.GetMnemonicName();
        }

        public ulong GetPriority()
        {
            return _function.GetPriority();
        }

        public MathFunctionType GetFunctionType()
        {
            return _function;
        }

        public MathOperationType GetOperationType()
        {
            return _function.GetOperationType();
        }

        public BaseMathExpressionItem LeftArgument
        {
            get
            {
                MathExpressionScope parent = GetParent();
                MathOperationType operationType = _function.GetOperationType();
                if ((parent != null) && (operationType == MathOperationType.Binary))
                {
                    int index = parent.IndexOf(this) - 1;
                    return parent[index];
                }
                return null;
            }
        }

        public BaseMathExpressionItem RightArgument
        {
            get
            {
                MathExpressionScope parent = GetParent();
                MathOperationType operationType = _function.GetOperationType();
                if ((parent != null) && ((operationType == MathOperationType.Binary) || (operationType == MathOperationType.Unary)))
                {
                    int index = parent.IndexOf(this) + 1;
                    return parent[index];
                }
                return null;
            }
        }

        public bool IsBinary
        {
            get { return (_function.GetOperationType() == MathOperationType.Binary); }
        }

        public bool IsAddition
        {
            get { return (_function == MathFunctionType.Addition); }
        }

        public bool IsSubtraction
        {
            get { return (_function == MathFunctionType.Subtraction); }
        }

        public bool IsMultiplication
        {
            get { return (_function == MathFunctionType.Multiplication); }
        }

        public bool IsDivision
        {
            get { return (_function == MathFunctionType.Division); }
        }
    }
}