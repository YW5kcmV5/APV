using System;
using System.Collections.Generic;
using System.Text;

namespace APV.Math.MathObjects.Functions
{
    public sealed class MathFunctionType : IMathFunctionType
    {
        private static readonly SortedList<string, MathFunctionType> Types = new SortedList<string, MathFunctionType>();
        private readonly SortedList<string, IMathCalculator> _calculators = new SortedList<string, IMathCalculator>();
        private readonly ulong _priority;
        private readonly string _mnemonicName;
        private readonly MathOperationType _operationType;
        private readonly ulong _argumentsCount;
        private Type _functionType;

        private static string GetHashCode(params IMathObjectType[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (arguments.Length == 0)
                throw new ArgumentOutOfRangeException("arguments", string.Format("As minimum one argument is expected for math function."));

            var sb = new StringBuilder();
            for (int i = 0; i < arguments.Length; i++)
            {
                IMathObjectType argument = arguments[i];

                if (argument == null)
                    throw new ArgumentOutOfRangeException("arguments", string.Format("Argument with index \"{0}\" is null.", i));

                sb.Append(argument.GetMnemonicName());
                sb.Append("/");
            }
            sb.Length -= 1;
            return sb.ToString();
        }

        private static string GetHashCode(params IMathObject[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (arguments.Length == 0)
                throw new ArgumentOutOfRangeException("arguments", string.Format("As minimum one argument is expected for math function."));

            var sb = new StringBuilder();
            for (int i = 0; i < arguments.Length; i++)
            {
                IMathObject argument = arguments[i];
                IMathObjectType argumentType = argument.GetMathType();

                if (argument == null)
                    throw new ArgumentOutOfRangeException("arguments", string.Format("Argument with index \"{0}\" is null.", i));

                sb.Append(argumentType.GetMnemonicName());
                sb.Append("/");
            }
            sb.Length -= 1;
            return sb.ToString();
        }

        private static string GetHashCode(string mnemonicName, MathOperationType operationType)
        {
            return string.Format("{0}/{1}", mnemonicName, operationType);
        }

        private static void Register(string mnemonicName, MathFunctionType functionType)
        {
            MathOperationType operationType = functionType.GetOperationType();
            string hashCode = GetHashCode(mnemonicName, operationType);
            lock (Types)
            {
                int index = Types.IndexOfKey(hashCode);
                if (index != -1)
                    throw new ArgumentOutOfRangeException("mnemonicName", string.Format("Function with name \"{0}\" and operation type \"{1}\", already exist.", mnemonicName, operationType));

                Types.Add(hashCode, functionType);
            }
        }

        private MathFunctionType(string mnemonicName, MathOperationType operationType, ulong priority)
        {
            if (mnemonicName == null)
                throw new ArgumentNullException("mnemonicName");
            if (string.IsNullOrWhiteSpace(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", string.Format("Function mnemonic name is empty or white space."));
            if (operationType == MathOperationType.Function)
                throw new InvalidOperationException(string.Format("For operation type \"{0}\" arguments count should be specified.", operationType));

            _mnemonicName = mnemonicName;
            _operationType = operationType;
            _argumentsCount = (operationType == MathOperationType.Unary) ? (ulong)1 : 2;
            _priority = priority;
        }

        private MathFunctionType(string mnemonicName, ulong argumentCount, ulong priority)
        {
            if (mnemonicName == null)
                throw new ArgumentNullException("mnemonicName");
            if (string.IsNullOrWhiteSpace(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", string.Format("Function mnemonic name is empty or white space."));
            if (argumentCount == 0)
                throw new ArgumentOutOfRangeException("argumentCount", string.Format("Math function needs as less one argument.."));

            _mnemonicName = mnemonicName;
            _operationType = MathOperationType.Function;
            _argumentsCount = argumentCount;
            _priority = priority;
        }

        public string GetMnemonicName()
        {
            return _mnemonicName;
        }

        public MathOperationType GetOperationType()
        {
            return _operationType;
        }

        public ulong GetArgumentsCount()
        {
            return _argumentsCount;
        }

        public ulong GetPriority()
        {
            return _priority;
        }

        public IMathCalculator GetCalculator(params IMathObject[] arguments)
        {
            string hashCode = GetHashCode(arguments);

            IMathCalculator calculator;
            lock (_calculators)
            {
                int index = _calculators.IndexOfKey(hashCode);
                calculator = (index != -1) ? _calculators.Values[index] : null;
            }

            if (calculator == null)
                throw new NotSupportedException(string.Format("Calculator not found for function \"{0}\" and argument list \"{1}\".", _mnemonicName, hashCode));

            return calculator;
        }

        public IMathCalculator GetCalculator(params IMathObjectType[] arguments)
        {
            string hashCode = GetHashCode(arguments);

            IMathCalculator calculator;
            lock (_calculators)
            {
                int index = _calculators.IndexOfKey(hashCode);
                calculator = (index != -1) ? _calculators.Values[index] : null;
            }

            if (calculator == null)
                throw new NotSupportedException(string.Format("Calculator not found for function \"{0}\" and argument list \"{1}\".", _mnemonicName, hashCode));

            return calculator;
        }

        public void AddCalculator(IMathCalculator calculator, params IMathObjectType[] arguments)
        {
            if (calculator == null)
                throw new ArgumentNullException("calculator");

            string hashCode = GetHashCode(arguments);

            lock (_calculators)
            {
                if (_calculators.ContainsKey(hashCode))
                {
                    _calculators[hashCode] = calculator;
                }
                else
                {
                    _calculators.Add(hashCode, calculator);
                }
            }
        }

        public bool CanCalculate(params IMathObjectType[] arguments)
        {
            string hashCode = GetHashCode(arguments);
            lock (_calculators)
            {
                return (_calculators.ContainsKey(hashCode));
            }
        }

        public IMathObject Calculate(params IMathObject[] arguments)
        {
            IMathCalculator calculator = GetCalculator(arguments);
            return calculator.Calculate(this, arguments);
        }

        public static MathFunctionType Register(string mnemonicName, MathOperationType operationType, ulong priority)
        {
            var type = new MathFunctionType(mnemonicName, operationType, priority);
            Register(mnemonicName, type);
            return type;
        }

        public static MathFunctionType Register(string mnemonicName, ulong argumentCount, ulong priority)
        {
            var type = new MathFunctionType(mnemonicName, argumentCount, priority);
            Register(mnemonicName, type);
            return type;
        }

        public static MathFunctionType Find(string functionType, MathOperationType operationType)
        {
            if (functionType == null)
                throw new ArgumentNullException("functionType");
            if (string.IsNullOrWhiteSpace(functionType))
                throw new ArgumentOutOfRangeException("functionType", "Function type is empty or whitespace.");

            string hashCode = GetHashCode(functionType, operationType);
            lock (Types)
            {
                int index = Types.IndexOfKey(hashCode);
                return (index != -1) ? Types.Values[index] : null;
            }
        }

        public static MathFunctionType Get(string functionType, MathOperationType operationType)
        {
            MathFunctionType type = Find(functionType, operationType);
            
            if (type == null)
                throw new ArgumentOutOfRangeException("functionType", string.Format("Function with mnemonic name \"{0}\" and operation type \"{1}\" is not registered.", functionType, operationType));

            return type;
        }

        /// <summary>
        /// Сложение ("+")
        /// </summary>
        public static readonly MathFunctionType Addition = Register("+", MathOperationType.Binary, 30);

        /// <summary>
        /// Вычитание ("-")
        /// </summary>
        public static readonly MathFunctionType Subtraction = Register("-", MathOperationType.Binary, 30);

        /// <summary>
        /// Деление ("/")
        /// </summary>
        public static readonly MathFunctionType Division = Register("/", MathOperationType.Binary, 40);

        /// <summary>
        /// Умножение ("*")
        /// </summary>
        public static readonly MathFunctionType Multiplication = Register("*", MathOperationType.Binary, 40);

        /// <summary>
        /// Возведение в степень ("^")
        /// </summary>
        public static readonly MathFunctionType Involution = Register("^", MathOperationType.Binary, 50);

        /// <summary>
        /// Инверсия ("-")
        /// </summary>
        public static readonly MathFunctionType Inversion = Register("-", MathOperationType.Unary, 60);
    }
}