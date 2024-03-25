using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using APV.Common;

namespace APV.Math.Tests.ConsoleTest
{
    public static class MathExpressionTest
    {
        public enum ViewItemType
        {
            Operand,

            Argument,

            OpenBracket,

            CloseBracket,
        }

        public class FunctionInfo 
        {
            public string Name { get; set; }

            public int Priority { get; set; }

            public BaseMathOperation Operation { get; set; }

            public int Arguments { get; set; }
        }

        public static FunctionInfo GetFunctionInfo(string mnemonicName, bool unary = false)
        {
            switch (mnemonicName)
            {
                case "^":
                    return new FunctionInfo
                        {
                            Name = "^",
                            Arguments = 2,
                            Operation = BaseMathOperation.Involution,
                            Priority = 40,
                        };
                case "*":
                    return new FunctionInfo
                        {
                            Name = "*",
                            Arguments = 2,
                            Operation = BaseMathOperation.Multiplication,
                            Priority = 30,
                        };
                case "/":
                    return new FunctionInfo
                        {
                            Name = "/",
                            Arguments = 2,
                            Operation = BaseMathOperation.Division,
                            Priority = 30,
                        };
                case "%":
                    return new FunctionInfo
                        {
                            Name = "%",
                            Arguments = 2,
                            Operation = BaseMathOperation.RemainderOfDivision,
                            Priority = 30,
                        };
                case "+":
                    return (unary)
                               ? new FunctionInfo
                                   {
                                       Name = "+",
                                       Arguments = 1,
                                       Operation = BaseMathOperation.UnaryPlus,
                                       Priority = 50,
                                   }
                               : new FunctionInfo
                                   {
                                       Name = "+",
                                       Arguments = 2,
                                       Operation = BaseMathOperation.Addition,
                                       Priority = 20,
                                   };
                case "-":
                    return (unary)
                               ? new FunctionInfo
                                   {
                                       Name = "-",
                                       Arguments = 1,
                                       Operation = BaseMathOperation.UnaryMinus,
                                       Priority = 50,
                                   }
                               : new FunctionInfo
                                   {
                                       Name = "-",
                                       Arguments = 2,
                                       Operation = BaseMathOperation.Subtraction,
                                       Priority = 20,
                                   };
            }
            return null;
        }

        [DebuggerDisplay("{Value} ({ItemType})")]
        public abstract class ItemView
        {
            public abstract string Value { get; }

            public abstract ViewItemType ItemType { get; }

            public override string ToString()
            {
                return Value;
            }
        }

        public class ArgumentView : ItemView
        {
            private readonly string _value;

            public ArgumentView(string value)
            {
                _value = value;
            }

            public override string Value
            {
                get { return _value; }
            }

            public override ViewItemType ItemType
            {
                get { return ViewItemType.Argument; }
            }
        }

        public class OpenBracket : ItemView
        {
            public override string Value { get { return "("; } }

            public override ViewItemType ItemType
            {
                get { return ViewItemType.OpenBracket; }
            }
        }

        public class CloseBracket : ItemView
        {
            public override string Value { get { return ")"; } }

            public override ViewItemType ItemType
            {
                get { return ViewItemType.CloseBracket; }
            }
        }

        public class OperandItem : ItemView
        {
            private readonly FunctionInfo _info;

            public OperandItem(FunctionInfo info)
            {
                _info = info;
            }

            public override string Value
            {
                get { return _info.Name; }
            }

            public int Priority
            {
                get { return _info.Priority; }
            }

            public int Count
            {
                get { return _info.Arguments; }
            }

            public FunctionInfo Info
            {
                get { return _info; }
            }

            public override ViewItemType ItemType
            {
                get { return ViewItemType.Operand; }
            }
        }

        public abstract class BaseCalculationItem
        {
            public abstract decimal GetValue();
        }

        public class ArgumentCalculationItem : BaseCalculationItem
        {
            private decimal? _value;
            private bool? _isVariable;
            private readonly CalculationContext _context;
            private readonly string _name;

            public ArgumentCalculationItem(CalculationContext context, string name)
            {
                _context = context;
                _name = name;
            }

            public override decimal GetValue()
            {
                if (IsVariable)
                {
                    return _context.GetValue(_name);
                }
                if (_value == null)
                {
                    _value = decimal.Parse(_name);
                }
                return _value.Value;
            }

            public bool IsVariable
            {
                get
                {
                    if (_isVariable == null)
                    {
                        _isVariable = _context.IsVariable(_name);
                    }
                    return _isVariable.Value;
                }
            }
        }

        public class CalculationItem : BaseCalculationItem
        {
            private readonly FunctionInfo _info;
            private readonly BaseCalculationItem[] _arguments;
            private Func<decimal[], decimal> _func;

            public BaseCalculationItem[] Arguments
            {
                get { return _arguments; }
            }

            public CalculationItem(FunctionInfo operand, BaseCalculationItem[] arguments)
            {
                if (operand == null)
                    throw new ArgumentNullException("operand");
                if (arguments == null)
                    throw new ArgumentNullException("arguments");
                if (arguments.Length != operand.Arguments)
                    throw new ArgumentOutOfRangeException("arguments", string.Format("Incorrect arguments count \"{0}\", expected \"{1}\" for function \"{2}\".", arguments.Length, operand.Arguments, operand.Name));
                if (arguments.Any(argument => argument == null))
                    throw new ArgumentOutOfRangeException("arguments", "One of argument is null.");

                _info = operand;
                _arguments = arguments;
            }

            private Func<decimal[], decimal> GetFunc()
            {
                BaseMathOperation operation = _info.Operation;

                if (_arguments.Length == 1)
                {
                    switch (operation)
                    {
                        case BaseMathOperation.UnaryMinus:
                            return arguments => -arguments[0];
                        case BaseMathOperation.UnaryPlus:
                            return arguments => +arguments[0];
                    }
                }
                else
                {
                    switch (operation)
                    {
                        case BaseMathOperation.Addition:
                            return arguments => arguments[0] + arguments[1];
                        case BaseMathOperation.Subtraction:
                            return arguments => arguments[0] - arguments[1];
                        case BaseMathOperation.Multiplication:
                            return arguments => arguments[0] * arguments[1];
                        case BaseMathOperation.Division:
                            return arguments => arguments[0] / arguments[1];
                        case BaseMathOperation.RemainderOfDivision:
                            return arguments => arguments[0] % arguments[1];
                        case BaseMathOperation.Involution:
                            return arguments => (decimal)System.Math.Pow((double)arguments[0], (double)arguments[1]);
                    }
                }

                if (_info.Operation == BaseMathOperation.Function)
                {
                    if (_info.Name == "SIN")
                    {
                        return arguments => (decimal)System.Math.Sin((double)arguments[0]);
                    }
                    if (_info.Name == "COS")
                    {
                        return arguments => (decimal)System.Math.Cos((double)arguments[0]);
                    }
                }

                throw new InvalidOperationException(string.Format("Unknown operation \"{0}\".", operation));
            }

            public override decimal GetValue()
            {
                Func<decimal[], decimal> function = (_func ?? (_func = GetFunc()));
                var arguments = new decimal[_info.Arguments];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = Arguments[i].GetValue();
                }
                return function(arguments);
            }
        }

        public class CalculationContext : BaseCalculationItem
        {
            private readonly SortedList<string, decimal> _variables = new SortedList<string, decimal>();
            private readonly BaseCalculationItem _calculation;

            internal CalculationContext(ItemView[] rpn)
            {
                _calculation = ToExecutionList(this, rpn);
            }

            public void SetVariable(string name, decimal value)
            {
                if (!_variables.ContainsKey(name))
                {
                    _variables.Add(name, value);
                }
                else
                {
                    _variables[name] = value;
                }
            }

            public bool IsVariable(string name)
            {
                return _variables.ContainsKey(name);
            }

            public decimal GetValue(string name)
            {
                return _variables[name];
            }

            public override decimal GetValue()
            {
                return _calculation.GetValue();
            }
        }

        /// <summary>
        /// Обратная Польская Запись (ОПЗ)
        /// </summary>
        private static ItemView[] ToRPN(ItemView[] items)
        {
            //1. Если этот символ - число (или переменная), то просто помещаем его в выходную строку.
            //2. Если символ - знак операции (+, -, *, / ), то проверяем приоритет данной операции.
            //Операции умножения и деления имеют наивысший приоритет (допустим он равен 3).
            //Операции сложения и вычитания имеют меньший приоритет (равен 2).
            //Наименьший приоритет (равен 1) имеет открывающая скобка.
            //Получив один из этих символов, мы должны проверить стек: 
            //а) Если стек все еще пуст, или находящиеся в нем символы
            //(а находится в нем могут только знаки операций и открывающая скобка) имеют меньший приоритет,
            //чем приоритет текущего символа, то помещаем текущий символ в стек.
            //б) Если символ, находящийся на вершине стека имеет приоритет, больший или равный приоритету текущего символа, то извлекаем символы из стека в выходную строку до тех пор, пока выполняется это условие; затем переходим к пункту а).
            //3. Если текущий символ - открывающая скобка, то помещаем ее в стек.
            //4. Если текущий символ - закрывающая скобка, то извлекаем символы из стека в выходную строку до тех пор,
            //пока не встретим в стеке открывающую скобку (т.е. символ с приоритетом, равным 1), которую следует просто уничтожить.
            //Закрывающая скобка также уничтожается.

            var result = new List<ItemView>();
            var stack = new Stack<ItemView>();

            for (int i = 0; i < items.Length; i++)
            {
                ItemView item = items[i];
                switch (item.ItemType)
                {
                    case ViewItemType.Argument:
                        result.Add(item);
                        break;

                    case ViewItemType.Operand:
                        var operand = (OperandItem) item;
                        int priority = operand.Priority;
                        while (true)
                        {
                            if (stack.Count == 0)
                            {
                                break;
                            }
                            ItemView stackItem = stack.Pop();
                            if ((stackItem.ItemType == ViewItemType.OpenBracket) || (((OperandItem)stackItem).Priority < priority))
                            {
                                stack.Push(stackItem);
                                break;
                            }
                            result.Add(stackItem);
                        }
                        stack.Push(item);
                        break;

                    case ViewItemType.OpenBracket:
                        stack.Push(item);
                        break;

                    case ViewItemType.CloseBracket:
                        while (true)
                        {
                            if (stack.Count == 0)
                                throw new InvalidOperationException("Invalid brackets count or position.");

                            ItemView stackItem = stack.Pop();
                            if (stackItem.ItemType == ViewItemType.OpenBracket)
                            {
                                break;
                            }
                            result.Add(stackItem);
                        }
                        break;
                }
            }

            while (stack.Count > 0)
            {
                result.Add(stack.Pop());
            }

            return result.ToArray();
        }

        /// <summary>
        /// Выполнение на основе обратной польской записи
        /// </summary>
        private static BaseCalculationItem ToExecutionList(CalculationContext context, ItemView[] rpn)
        {
            //1. Если очередной символ входной строки - число, то кладем его в стек. 
            //2. Если очередной символ - знак операции, то извлекаем из стека два верхних числа, используем их в качестве операндов для этой операции,
            //затем кладем результат обратно в стек. 
            //Когда вся входная строка будет разобрана в стеке должно остаться одно число, которое и будет результатом данного выражения.

            var stack = new Stack<BaseCalculationItem>();
            var argumens = new SortedList<string, ArgumentCalculationItem>();

            for (int i = 0; i < rpn.Length; i++)
            {
                ItemView item = rpn[i];
                switch (item.ItemType)
                {
                    case ViewItemType.Argument:
                        ArgumentCalculationItem argument;
                        int index = argumens.IndexOfKey(item.Value);
                        if (index == -1)
                        {
                            argument = new ArgumentCalculationItem(context, item.Value);
                            argumens.Add(item.Value, argument);
                        }
                        else
                        {
                            argument = argumens.Values[index];
                        }
                        stack.Push(argument);
                        break;

                    case ViewItemType.Operand:
                        var operand = (OperandItem) item;
                        int argumentsCount = operand.Count;
                        var arguments = new List<BaseCalculationItem>();
                        for (int j = 0; j < argumentsCount; j++)
                        {
                            arguments.Insert(0, stack.Pop());
                        }
                        var calculationItem = new CalculationItem(operand.Info, arguments.ToArray());
                        stack.Push(calculationItem);
                        break;
                }
            }

            return stack.Pop();
        }

        private static CalculationContext Parse(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentOutOfRangeException("expression", "Expression is white string.");

            expression = expression.Trim();
            expression = string.Format("({0})", expression.Trim());

            var items = new List<ItemView>();
            string currentItem = string.Empty;
            bool previousWasOpenBracketOrOperand = false;
            for (int i = 0; i < expression.Length; i++)
            {
                string symbol = char.ToString(expression[i]);
                if (symbol != " ")
                {
                    bool bracketOpen = (symbol == "(");
                    bool bracketClose = (!bracketOpen) && (symbol == ")");

                    FunctionInfo operation = (!bracketOpen) && (!bracketClose)
                                                 ? GetFunctionInfo(symbol, previousWasOpenBracketOrOperand)
                                                 : null;

                    bool isOperation = (operation != null);
                    if ((bracketOpen) || (bracketClose) || (isOperation))
                    {
                        if (!string.IsNullOrWhiteSpace(currentItem))
                        {
                            if (string.Compare(currentItem, "sin", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                operation = new FunctionInfo
                                    {
                                        Name = "SIN",
                                        Operation = BaseMathOperation.Function,
                                        Arguments = 1,
                                        Priority = 40,
                                    };
                                items.Add(new OperandItem(operation));
                            }
                            else if (string.Compare(currentItem, "cos", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                operation = new FunctionInfo
                                    {
                                        Name = "COS",
                                        Operation = BaseMathOperation.Function,
                                        Arguments = 1,
                                        Priority = 40,
                                    };
                                items.Add(new OperandItem(operation));
                            }
                            else
                            {
                                var argument = new ArgumentView(currentItem);
                                items.Add(argument);
                            }
                            currentItem = string.Empty;
                        }

                        if (bracketOpen)
                        {
                            items.Add(new OpenBracket());
                        }
                        else if (bracketClose)
                        {
                            items.Add(new CloseBracket());
                        }
                        else
                        {
                            items.Add(new OperandItem(operation));
                        }

                        previousWasOpenBracketOrOperand = (bracketOpen) || (isOperation);
                    }
                    else
                    {
                        currentItem += symbol;
                        previousWasOpenBracketOrOperand = false;
                    }
                }
            }

            ItemView[] rpn = ToRPN(items.ToArray());
            var context = new CalculationContext(rpn);
            return context;
        }

        private static void ParseTest1()
        {
            //const string expression = "((-10)-(2*(x+25))^2)";
            //const string expression = "2*(x+25)^2";
            //const string expression = "a + ( b - c ) * d";
            //const string expression = "7+(5-2)*4";
            const string expression = "COS(x+(1+sin(2-x)*2))";
            //const string expression = "5-2";

            CalculationContext context = Parse(expression);
            context.SetVariable("x", 1);
            decimal value = context.GetValue();
            decimal value0;
            decimal value1;
            value = context.GetValue();

            const int count = 100000;
            DateTime begin = DateTime.Now;
            for (int x = 0; x < count; x++)
            {
                context.SetVariable("x", x);
                value0 = context.GetValue();
            }
            int totalMiliseconds1 = (int)(DateTime.Now - begin).TotalMilliseconds;

            begin = DateTime.Now;
            for (int x = 0; x < count; x++)
            {
                //COS(x+(1+sin(2-x)*2))
                decimal a0 = (decimal)2 - x;
                decimal a1 = (decimal)System.Math.Sin((double) a0);
                decimal a2 = x + (decimal) 1 + 2*a1;
                value1 = (decimal)System.Math.Cos((double)a2);
            }
            int totalMiliseconds2 = (int)(DateTime.Now - begin).TotalMilliseconds;

            Console.WriteLine("K={0:0.00}", (decimal)totalMiliseconds1 / totalMiliseconds2);
        }

        public static void Execute()
        {
            try
            {
                ParseTest1();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}", ex.ToTraceString());
                throw;
            }
        }
    }
}