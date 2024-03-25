using System;
using System.Collections.Generic;
using System.Text;
using APV.Math.MathObjects.Functions;

namespace APV.Math.MathObjects.Calculations
{
    public sealed class MathOperationItem : BaseMathCalculationItem
    {
        private readonly MathFunctionType _functionType;
        private readonly BaseMathCalculationItem[] _arguments;

        private static string InvokeGetMnemonicName(MathFunctionType functionType, params BaseMathCalculationItem[] arguments)
        {
            var sb = new StringBuilder();
            sb.Append(functionType.GetMnemonicName());
            sb.Append("(");
            int length = arguments.Length;
            for (int i = 0; i < length; i++)
            {
                BaseMathCalculationItem argument = arguments[i];

                if (argument is MathArgumentItem)
                {
                    sb.AppendFormat("\"{0}\"", ((MathArgumentItem)argument).MnemonicName);
                }
                else
                {
                    sb.Append("#");
                    sb.Append(argument.GetId());
                }
                sb.Append(", ");
            }
            sb.Length -= 2;
            sb.Append(")");
            return sb.ToString();
        }

        public MathOperationItem(MathOperationContext context, MathFunctionType functionType, params BaseMathCalculationItem[] arguments)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (functionType == null)
                throw new ArgumentNullException("functionType");
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if ((ulong)arguments.Length != functionType.GetArgumentsCount())
                throw new ArgumentOutOfRangeException("arguments", string.Format("Incorrect arguments count \"{0}\", expected \"{1}\" for function \"{2}\".", arguments.Length, functionType.GetArgumentsCount(), functionType.GetMnemonicName()));
            
            _functionType = functionType;
            _arguments = arguments;

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                    throw new ArgumentOutOfRangeException("arguments", string.Format("Argument with index \"{0}\" is null.", i));
            }
        }

        public override string GetMnemonicName()
        {
            return GetMnemonicName(_functionType, _arguments);
        }

        public override IMathCalculationItem[] GetExecutionList()
        {
            var executionList = new List<IMathCalculationItem>();
            int length = _arguments.Length;
            for (int i = 0; i < length; i++)
            {
                BaseMathCalculationItem argument = _arguments[i];
                if (argument is MathOperationItem)
                {
                    IMathCalculationItem[] argumentsExecutionList = argument.GetExecutionList();
                    foreach (IMathCalculationItem argumentExecutionList in argumentsExecutionList)
                    {
                        if (!executionList.Contains(argumentExecutionList))
                        {
                            executionList.Add(argumentExecutionList);
                        }
                    }
                }
            }
            executionList.Add(this);
            return executionList.ToArray();
        }

        public override IMathObject Calculate()
        {
            var arguments = new IMathObject[_arguments.Length];
            int length = _arguments.Length;
            for (int i = 0; i < length; i++)
            {
                arguments[i] = _arguments[i].Calculate();
            }
            IMathObject ret = _functionType.Calculate(arguments);
            return ret;
        }

        public override MathCalculationItemType GetItemType()
        {
            return MathCalculationItemType.Operation;
        }

        public BaseMathCalculationItem[] GetArguments()
        {
            return _arguments;
        }

        public MathFunctionType FunctionType
        {
            get { return _functionType; }
        }

        public static string GetMnemonicName(MathFunctionType functionType, params BaseMathCalculationItem[] arguments)
        {
            if (functionType == null)
                throw new ArgumentNullException("functionType");
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if ((ulong)arguments.Length != functionType.GetArgumentsCount())
                throw new ArgumentOutOfRangeException("arguments", string.Format("Incorrect arguments count \"{0}\", expected \"{1}\" for function \"{2}\".", arguments.Length, functionType.GetArgumentsCount(), functionType.GetMnemonicName()));

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                    throw new ArgumentOutOfRangeException("arguments", string.Format("Argument with index \"{0}\" is null.", i));
            }

            return InvokeGetMnemonicName(functionType, arguments);
        }
    }
}