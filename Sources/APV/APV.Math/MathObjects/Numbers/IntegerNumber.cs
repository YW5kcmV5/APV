using System;

namespace APV.Math.MathObjects.Numbers
{
    /// <summary>
    /// Целые числа Z.
    /// Расширение множества натуральных чисел N, получаемое добавлением к N нуля и отрицательных чисел вида -N.
    /// </summary>
    public class IntegerNumber : INumber
    {
        private static readonly MathObjectType MathObjectType = new MathObjectType(MenmonicName, typeof(IntegerNumber));

        public IntegerNumber()
        {
        }

        public IntegerNumber(string mnemonic)
        {
            FromMnemonic(mnemonic);
        }

        public void FromMnemonic(string mnemonic)
        {
            throw new NotImplementedException();
        }

        public string ToMnemonic()
        {
            throw new NotImplementedException();
        }

        IMathObjectType IMathObject.GetMathType()
        {
            return GetMathType();
        }

        MathObjectType GetMathType()
        {
            return MathObjectType;
        }

        public string Mnemonic
        {
            get { return ToMnemonic(); }
        }

        public const string MenmonicName = @"Integer";
    }
}