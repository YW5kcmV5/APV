namespace APV.Math.MathObjects.Expressions.Optimizers
{
    public abstract class BaseMathExpressionOptimizer : IMnemonicName
    {
        public virtual string GetMnemonicName()
        {
            return GetType().Name;
        }

        public abstract void Optimize(MathExpressionScope scope);
    }
}