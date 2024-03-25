
namespace APV.Math
{
    public interface IMnemonicName
    {
        string GetMnemonicName();
    }

    public interface IMnemonicObject
    {
        void FromMnemonic(string mnemonic);

        string ToMnemonic();
    }

    public interface IMathObjectType : IMnemonicName
    {
        IMathObject CreateInstance();

        IMathObject CreateInstance(string mnemonic);
    }

    public interface IPrimitiveNumber
    {
        object GetPrimitive();
    }

    /// <summary>
    /// Математичекий объект (например число, матрица), над которым возможно выполнение математической операции
    /// </summary>
    public interface IMathObject : IMnemonicObject
    {
        IMathObjectType GetMathType();
    }

    /// <summary>
    /// Число (натуральное, целое, рациональное, действительное (вещественное), комплексное)
    /// </summary>
    public interface INumber : IMathObject
    {
    }

    public interface IInteger : INumber
    {
    }

    public interface IFloat : INumber
    {
    }

    public interface IMathFunctionType : IMnemonicName
    {
        MathOperationType GetOperationType();

        ulong GetArgumentsCount();

        ulong GetPriority();

        IMathCalculator GetCalculator(params IMathObjectType[] arguments);

        void AddCalculator(IMathCalculator calculator, params IMathObjectType[] arguments);

        bool CanCalculate(params IMathObjectType[] arguments);

        IMathObject Calculate(params IMathObject[] arguments);
    }

    //public interface IMathFunction : IMnemonicObject
    //{
    //    IMathFunctionType GetMathType();

    //    IMathObjectType[] GetParameters();

    //    IMathObject GetRetParameter();

    //    IMathObject Calculate(params IMathObject[] arguments);
    //}

    public interface IMathCalculator
    {
        IMathObject Calculate(IMathFunctionType function, params IMathObject[] arguments);

        //IMathFunction GetFunction(IMathFunctionType function, IMathObjectType[] arguments);
    }

    public interface IMathCalculationContext : IMathCalculationItem
    {
        bool ContainsVariable(string variableName);

        IMathObject GetVariableValue(string variableName);

        void SetVariableValue(string variableName, IMathObject value);

        IMathObject ParseConstant(string constantValue);

        IMathObjectType GetDefaultType();

        void SetDefaultType(IMathObjectType defaultType);
    }

    public interface IMathHardwareCalculationItem
    {
    }

    public interface IMathCalculationItem : IMnemonicName
    {
        ulong GetId();

        IMathCalculationContext GetContext();

        MathCalculationItemType GetItemType();

        IMathObject Calculate();

        IMathCalculationItem[] GetExecutionList();
    }
}