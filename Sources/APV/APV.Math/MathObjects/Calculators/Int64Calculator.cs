using System;
using APV.Math.MathObjects.Functions;
using APV.Math.MathObjects.Numbers;

namespace APV.Math.MathObjects.Calculators
{
    public sealed class Int64Calculator : MathCalculator
    {
        static Int64Calculator()
        {
            MathFunctionType.Inversion.AddCalculator(Instance, Int64Number.MathObjectType);
            MathFunctionType.Involution.AddCalculator(Instance, Int64Number.MathObjectType, Int64Number.MathObjectType);
            MathFunctionType.Multiplication.AddCalculator(Instance, Int64Number.MathObjectType, Int64Number.MathObjectType);
            MathFunctionType.Division.AddCalculator(Instance, Int64Number.MathObjectType, Int64Number.MathObjectType);
            MathFunctionType.Addition.AddCalculator(Instance, Int64Number.MathObjectType, Int64Number.MathObjectType);
            MathFunctionType.Subtraction.AddCalculator(Instance, Int64Number.MathObjectType, Int64Number.MathObjectType);
        }

        public static void Init()
        {
        }

        [MathCalculatorFunction("*")]
        public static Int64Number Multiplication(Int64Number x, Int64Number y)
        {
            return x * y;
        }

        [MathCalculatorFunction("/")]
        public static Int64Number Division(Int64Number x, Int64Number y)
        {
            return x / y;
        }

        [MathCalculatorFunction("+")]
        public static Int64Number Addition(Int64Number x, Int64Number y)
        {
            return x + y;
        }

        [MathCalculatorFunction("-")]
        public static Int64Number Subtraction(Int64Number x, Int64Number y)
        {
            return x - y;
        }

        [MathCalculatorFunction("-", MathOperationType.Unary)]
        public static Int64Number Inversion(Int64Number x)
        {
            return -x;
        }

        [MathCalculatorFunction("^")]
        public static Int64Number Involution(Int64Number x, Int64Number y)
        {
            return x ^ y;
        }

        public override IMathObject Calculate(IMathFunctionType function, params IMathObject[] arguments)
        {
            if (function == null)
                throw new ArgumentNullException("function");
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (function.GetArgumentsCount() != (uint)arguments.Length)
                throw new ArgumentOutOfRangeException("arguments", string.Format("For function \"{0}\" expected \"{1}\" parameters, but \"{2}\" presented.", function.GetMnemonicName(), function.GetArgumentsCount(), arguments.Length));

            if (function == MathFunctionType.Multiplication)
            {
                return Multiplication(arguments[0] as Int64Number, arguments[1] as Int64Number);
            }
            if (function == MathFunctionType.Division)
            {
                return Division(arguments[0] as Int64Number, arguments[1] as Int64Number);
            }
            if (function == MathFunctionType.Addition)
            {
                return Addition(arguments[0] as Int64Number, arguments[1] as Int64Number);
            }
            if (function == MathFunctionType.Subtraction)
            {
                return Subtraction(arguments[0] as Int64Number, arguments[1] as Int64Number);
            }
            if (function == MathFunctionType.Involution)
            {
                return Involution(arguments[0] as Int64Number, arguments[1] as Int64Number);
            }
            if (function == MathFunctionType.Inversion)
            {
                return Inversion(arguments[0] as Int64Number);
            }

            throw new NotSupportedException(string.Format("Function \"{0}\" is not supported.", function.GetMnemonicName()));
        }

        public static readonly Int64Calculator Instance = new Int64Calculator();
    }
}