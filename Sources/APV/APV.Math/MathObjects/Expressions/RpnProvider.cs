using System;
using System.Collections.Generic;
using System.Diagnostics;
using APV.Math.MathObjects.Calculations;
using APV.Math.MathObjects.Functions;

namespace APV.Math.MathObjects.Expressions
{
    public static class RpnProvider
    {
        #region Types

        private enum ItemType
        {
            Operand,

            Argument,

            OpenBracket,

            CloseBracket,
        }

        [DebuggerDisplay("{Value} ({ItemType})")]
        private abstract class ItemView
        {
            public abstract string Value { get; }

            public abstract ItemType ItemType { get; }

            public override string ToString()
            {
                return Value;
            }
        }

        private class ArgumentView : ItemView
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

            public override ItemType ItemType
            {
                get { return ItemType.Argument; }
            }
        }

        private class OpenBracket : ItemView
        {
            public override string Value { get { return "("; } }

            public override ItemType ItemType
            {
                get { return ItemType.OpenBracket; }
            }
        }

        private class CloseBracket : ItemView
        {
            public override string Value { get { return ")"; } }

            public override ItemType ItemType
            {
                get { return ItemType.CloseBracket; }
            }
        }

        [DebuggerDisplay("{Value} ({ItemType}, ScopeId:{ScopeId}, Priority:{Priority})")]
        private class OperandItem : ItemView
        {
            private readonly MathFunctionType _info;

            public OperandItem(MathFunctionType info)
            {
                _info = info;
            }

            public override string Value
            {
                get { return _info.GetMnemonicName(); }
            }

            public ulong Priority
            {
                get { return _info.GetPriority(); }
            }

            public ulong Count
            {
                get { return _info.GetArgumentsCount(); }
            }

            public MathFunctionType Info
            {
                get { return _info; }
            }

            public override ItemType ItemType
            {
                get { return ItemType.Operand; }
            }
        }

        #endregion

        private static MathFunctionType FindFunctionInfo(string mnemonicName, MathOperationType operationType)
        {
            return MathFunctionType.Find(mnemonicName, operationType);
        }

        private static ItemView[] ParseToItems(string expression)
        {
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

                    MathFunctionType operation = null;
                    if ((!bracketOpen) && (!bracketClose))
                    {
                        MathOperationType operationType = previousWasOpenBracketOrOperand
                                                              ? MathOperationType.Unary
                                                              : MathOperationType.Binary;
                        operation = FindFunctionInfo(symbol, operationType);
                    }

                    bool isOperation = (operation != null);
                    if ((bracketOpen) || (bracketClose) || (isOperation))
                    {
                        if (!string.IsNullOrWhiteSpace(currentItem))
                        {
                            MathFunctionType function = FindFunctionInfo(currentItem, MathOperationType.Function);
                            if (function != null)
                            {
                                items.Add(new OperandItem(function));
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

            return items.ToArray();
        }

        private static MathExpressionScope ParseToExpression(ItemView[] items)
        {
            var scope = new MathExpressionScope();
            for (int i = 0; i < items.Length; i++)
            {
                ItemView item = items[i];
                switch (item.ItemType)
                {
                    case ItemType.OpenBracket:
                        scope = scope.AddScope();
                        break;

                    case ItemType.CloseBracket:
                        scope = scope.GetParent();
                        break;

                    case ItemType.Argument:
                        scope.Add(item.Value);
                        break;

                    case ItemType.Operand:
                        var operand = (OperandItem) item;
                        scope.Add(operand.Info);
                        break;
                }
            }

            scope = scope.GetRoot();
            return scope;
        }

        private static ItemView[] ToItems(MathExpressionScope scope)
        {
            var items = new List<ItemView>();
            BaseMathExpressionItem[] subItems = scope.GetItems();
            foreach (BaseMathExpressionItem itemView in subItems)
            {
                if (itemView is MathExpressionScope)
                {
                    items.Add(new OpenBracket());
                    items.AddRange(ToItems((MathExpressionScope) itemView));
                    items.Add(new CloseBracket());
                }
                else if (itemView is MathExpressionArgument)
                {
                    items.Add(new ArgumentView(itemView.GetMnemonicName()));
                }
                else if (itemView is MathExpressionOperation)
                {
                    items.Add(new OperandItem(((MathExpressionOperation) itemView).GetFunctionType()));
                }
            }
            return items.ToArray();
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
                    case ItemType.Argument:
                        result.Add(item);
                        break;

                    case ItemType.Operand:
                        var operand = (OperandItem)item;
                        ulong priority = operand.Priority;
                        while (true)
                        {
                            if (stack.Count == 0)
                            {
                                break;
                            }
                            ItemView stackItem = stack.Pop();
                            if ((stackItem.ItemType == ItemType.OpenBracket) || (((OperandItem)stackItem).Priority < priority))
                            {
                                stack.Push(stackItem);
                                break;
                            }
                            result.Add(stackItem);
                        }
                        stack.Push(item);
                        break;

                    case ItemType.OpenBracket:
                        stack.Push(item);
                        break;

                    case ItemType.CloseBracket:
                        while (true)
                        {
                            if (stack.Count == 0)
                                throw new InvalidOperationException("Invalid brackets count or position.");

                            ItemView stackItem = stack.Pop();
                            if (stackItem.ItemType == ItemType.OpenBracket)
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
        private static BaseMathCalculationItem ToExecutionList(MathOperationContext context, ItemView[] rpn)
        {
            //1. Если очередной символ входной строки - число, то кладем его в стек. 
            //2. Если очередной символ - знак операции, то извлекаем из стека два верхних числа, используем их в качестве операндов для этой операции,
            //затем кладем результат обратно в стек. 
            //Когда вся входная строка будет разобрана в стеке должно остаться одно число, которое и будет результатом данного выражения.

            var stack = new Stack<BaseMathCalculationItem>();
            var argumens = new SortedList<string, MathArgumentItem>();
            var operations = new SortedList<string, MathOperationItem>();

            for (int i = 0; i < rpn.Length; i++)
            {
                ItemView item = rpn[i];
                string mnemonicName;
                int index;
                switch (item.ItemType)
                {
                    case ItemType.Argument:
                        mnemonicName = item.Value;
                        index = argumens.IndexOfKey(mnemonicName);
                        MathArgumentItem mathAgrument;
                        if (index == -1)
                        {
                            mathAgrument = new MathArgumentItem(context, item.Value);
                            argumens.Add(item.Value, mathAgrument);
                        }
                        else
                        {
                            mathAgrument = argumens.Values[index];
                        }
                        stack.Push(mathAgrument);
                        break;

                    case ItemType.Operand:
                        var operand = (OperandItem)item;
                        ulong argumentsCount = operand.Count;
                        var arguments = new BaseMathCalculationItem[argumentsCount];
                        for (ulong j = 0; j < argumentsCount; j++)
                        {
                            BaseMathCalculationItem argument = stack.Pop();
                            arguments[argumentsCount - j - 1] = argument;
                        }

                        mnemonicName = MathOperationItem.GetMnemonicName(operand.Info, arguments);
                        MathOperationItem calculationItem;
                        index = operations.IndexOfKey(mnemonicName);
                        if (index == -1)
                        {
                            calculationItem = new MathOperationItem(context, operand.Info, arguments);
                            operations.Add(mnemonicName, calculationItem);
                        }
                        else
                        {
                            calculationItem = operations.Values[index];
                        }
                        stack.Push(calculationItem);
                        break;
                }
            }

            return stack.Pop();
        }

        public static MathExpressionScope ParseToExpression(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentOutOfRangeException("expression", "Expression is white string.");

            ItemView[] items = ParseToItems(expression);
            MathExpressionScope scope = ParseToExpression(items);
            return scope;
        }

        public static MathOperationContext Parse(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentOutOfRangeException("expression", "Expression is white string.");

            ItemView[] items = ParseToItems(expression);
            ItemView[] rpn = ToRPN(items);
            var context = new MathOperationContext();
            BaseMathCalculationItem entryPoint = ToExecutionList(context, rpn);
            context.SetEntryPoint(entryPoint);
            return context;
        }

        public static MathOperationContext Parse(MathExpressionScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            ItemView[] items = ToItems(scope);
            ItemView[] rpn = ToRPN(items);
            var context = new MathOperationContext();
            BaseMathCalculationItem entryPoint = ToExecutionList(context, rpn);
            context.SetEntryPoint(entryPoint);
            return context;
        }
    }
}