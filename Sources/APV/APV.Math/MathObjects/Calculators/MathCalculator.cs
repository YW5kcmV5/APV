namespace APV.Math.MathObjects.Calculators
{
    public abstract class MathCalculator : IMathCalculator
    {
        public abstract IMathObject Calculate(IMathFunctionType function, params IMathObject[] arguments);
    }
}