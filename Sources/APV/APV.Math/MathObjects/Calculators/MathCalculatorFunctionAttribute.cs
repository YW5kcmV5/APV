using System;
using APV.Math.MathObjects.Functions;

namespace APV.Math.MathObjects.Calculators
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MathCalculatorFunctionAttribute : Attribute
    {
        private readonly MathFunctionType _mathFunctionType;

        public MathCalculatorFunctionAttribute(string functionType, MathOperationType operationType = MathOperationType.Function)
        {
            _mathFunctionType = MathFunctionType.Get(functionType, operationType);
        }

        public MathFunctionType MathFunctionType
        {
            get { return _mathFunctionType; }
        }
    }
}