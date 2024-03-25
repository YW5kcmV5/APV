using System;

namespace APV.Math
{
    [Serializable]
    public enum MathOperationType
    {
        /// <summary>
        /// Унарная (например, -1)
        /// </summary>
        Unary,
        
        /// <summary>
        /// Бинарная (например 2*2)
        /// </summary>
        Binary,

        /// <summary>
        /// Функция (например, sin(x))
        /// </summary>
        Function,
    }

    [Serializable]
    public enum BaseMathOperation
    {
        /// <summary>
        /// "+"
        /// </summary>
        Addition,

        /// <summary>
        /// "-"
        /// </summary>
        Subtraction,

        /// <summary>
        /// "*"
        /// </summary>
        Multiplication,

        /// <summary>
        /// "/"
        /// </summary>
        Division,

        /// <summary>
        /// "%"
        /// </summary>
        RemainderOfDivision,

        /// <summary>
        /// "^"
        /// </summary>
        Involution,

        /// <summary>
        /// "Minus"
        /// </summary>
        UnaryMinus,

        /// <summary>
        /// "Plus"
        /// </summary>
        UnaryPlus,

        /// <summary>
        /// f([args])
        /// </summary>
        Function
    }

    [Serializable]
    public enum MathCalculationItemType
    {
        Context,

        Constant,

        Variable,

        Operation
    }
}