using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using APV.Math.MathObjects.Functions;

namespace APV.Math.MathObjects.Expressions.Optimizers
{
    public class SimpleMathExpressionOptimizer : BaseMathExpressionOptimizer
    {
        #region Optimization

        private int GetLastOperationWithTheSamePriority(MathExpressionScope root, int index, ulong priority)
        {
            int? lastIndex = null;
            for (int i = index + 1; i < root.Count; i++)
            {
                var operation = root[i] as MathExpressionOperation;
                if (operation != null)
                {
                    if (operation.GetPriority() != priority)
                    {
                        break;
                    }
                    lastIndex = i;
                }
            }
            return lastIndex ?? index;
        }

        private void GroupOperationsWithDifferentPriority(MathExpressionScope root)
        {
            MathExpressionOperation[] operations = root.GetOperations();
            ulong[] priorities = operations.Select(operation => operation.GetPriority()).Distinct().ToArray();
            if (priorities.Length > 1)
            {
                ulong minOperation = priorities.Min();
                MathExpressionOperation[] operationsToScope = operations.Where(operation => operation.GetPriority() != minOperation).ToArray();
                foreach (MathExpressionOperation operation in operationsToScope)
                {
                    int index = root.IndexOf(operation);
                    if (index != -1)
                    {
                        IMathFunctionType function = operation.GetFunctionType();
                        MathOperationType operationType = function.GetOperationType();
                        if (operationType == MathOperationType.Binary)
                        {
                            var scope = new MathExpressionScope(root);
                            int lastIndex = GetLastOperationWithTheSamePriority(root, index, operation.GetPriority());
                            for (int i = index - 1; i <= lastIndex + 1; i++)
                            {
                                scope.Add(root[i]);
                            }
                            root.RemoveRange(index, lastIndex - index + 2);
                            root.Set(index - 1, scope);
                        }
                        else if (operationType == MathOperationType.Unary)
                        {
                            var scope = new MathExpressionScope(root);
                            scope.Add(operation);
                            scope.Add(root[index + 1]);
                            root.RemoveAt(index + 1);
                            root.Set(index, scope);
                        }
                    }
                }
                Optimize(root);
            }
        }

        private void SummarizeOperations(MathExpressionScope root)
        {
            MathExpressionOperation[] operations = root.GetOperations();
            if (operations.Length > 1)
            {
                MathExpressionOperation[] summarizeOperations = operations.Where(operation => (operation.IsBinary) && ((operation.IsAddition) || (operation.IsSubtraction))).ToArray();
                if (summarizeOperations.Length == operations.Length)
                {
                    BaseMathExpressionItem[] arguments = root.GetArgumentsAndScopes();
                    foreach (BaseMathExpressionItem item in arguments)
                    {
                        int index = root.IndexOf(item);
                        if (index != -1)
                        {
                            string name = item.GetMnemonicName();
                            BaseMathExpressionItem[] equals = arguments.Where(argument => argument.GetMnemonicName() == name).ToArray();
                            if (equals.Length > 1)
                            {
                                int summa = 0;
                                for (int i = 0; i < equals.Length; i++)
                                {
                                    BaseMathExpressionItem subItem = equals[i];
                                    int subIndex = root.IndexOf(subItem);

                                    bool plus;
                                    root.RemoveAt(subIndex);
                                    if (subIndex == 0)
                                    {
                                        plus = true;
                                    }
                                    else
                                    {
                                        var operation = (MathExpressionOperation)root[subIndex - 1];
                                        plus = operation.IsAddition;
                                        root.RemoveAt(subIndex - 1);
                                    }

                                    if (plus)
                                    {
                                        summa++;
                                    }
                                    else
                                    {
                                        summa--;
                                    }
                                }

                                var scope = new MathExpressionScope(root);
                                scope.Add(summa.ToString(CultureInfo.InvariantCulture));
                                scope.Add(MathFunctionType.Multiplication);
                                scope.Add(item);

                                root.Add(0, scope);
                            }
                        }
                    }
                }
            }
        }

        private void SortMultiplicationAndDivision(MathExpressionScope scope)
        {
            MathExpressionOperation[] operations = scope.GetOperations();
            if (operations.Length > 1)
            {
                MathExpressionOperation[] mulOperations = operations.Where(operation => (operation.IsMultiplication)).ToArray();
                MathExpressionOperation[] divOperations = operations.Where(operation => (operation.IsDivision)).ToArray();
                if ((mulOperations.Length + divOperations.Length == operations.Length) &&
                    /*(mulOperations.Length > 0) &&*/ (divOperations.Length > 0))
                {
                    var divArguments = new List<BaseMathExpressionItem>();
                    for (int i = 0; i < divOperations.Length; i++)
                    {
                        MathExpressionOperation divOperation = divOperations[i];
                        BaseMathExpressionItem rightArgument = divOperation.RightArgument;
                        scope.Remove(divOperation, rightArgument);
                        divArguments.Add(rightArgument);
                    }

                    MathExpressionArgument[] mulArguments = scope.GetArguments();
                    if (mulArguments.Length > 1)
                    {
                        var mulScope = new MathExpressionScope(scope);
                        for (int i = 0; i < mulArguments.Length; i++)
                        {
                            mulScope.Add(mulArguments[i]);
                            if (i < mulArguments.Length - 1)
                            {
                                mulScope.Add(MathFunctionType.Multiplication);
                            }
                        }
                        scope.Set(mulScope);
                    }

                    BaseMathExpressionItem divider = divArguments[0];
                    if (divArguments.Count > 1)
                    {
                        var divScope = new MathExpressionScope(scope);
                        for (int i = 0; i < divArguments.Count; i++)
                        {
                            divScope.Add(divArguments[i]);
                            if (i < divArguments.Count - 1)
                            {
                                divScope.Add(MathFunctionType.Multiplication);
                            }
                        }
                        divider = divScope;
                    }
                    scope.Add(MathFunctionType.Division);
                    scope.Add(divider);
                }
            }
        }

        private void GroupMultiplicationByArgument(MathExpressionScope scope)
        {
            MathExpressionOperation[] operations = scope.GetOperations();
            if (operations.Length > 1)
            {
                MathExpressionOperation[] mulOperations = operations.Where(operation => (operation.IsMultiplication)).ToArray();
                if ((mulOperations.Length == operations.Length) && (mulOperations.Length >= 2))
                {
                    BaseMathExpressionItem[] arguments = scope.GetArgumentsAndScopes();
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        BaseMathExpressionItem argument = arguments[i];
                        string argumentName = argument.GetMnemonicName();
                        BaseMathExpressionItem[] equals = arguments.Where(equalArgument => equalArgument.GetMnemonicName() == argumentName).ToArray();
                        if (equals.Length >= 2)
                        {
                            int groups = equals.Length/2;
                            for (int j = 0; j < groups; j++)
                            {
                                var newScope = new MathExpressionScope(scope);

                                BaseMathExpressionItem leftArgument = equals[2*j];
                                BaseMathExpressionItem rightArgument = equals[2*j + 1];

                                int leftIndex = scope.IndexOf(leftArgument);
                                int rightIndex = scope.IndexOf(rightArgument);
                                if ((leftIndex != -1) && (rightIndex != -1))
                                {
                                    scope.RemoveAt(rightIndex);
                                    if (rightIndex > 0)
                                    {
                                        scope.RemoveAt(rightIndex - 1);
                                    }

                                    newScope.Add(leftArgument);
                                    newScope.Add(MathFunctionType.Multiplication);
                                    newScope.Add(rightArgument);

                                    scope.Set(leftIndex, newScope);
                                }
                            }
                            GroupMultiplicationByArgument(scope);
                            return;
                        }
                    }
                }
            }
        }

        private void DeleteRedundantBraces(MathExpressionScope root)
        {
            while ((root.Count == 1) && (root[0] is MathExpressionScope))
            {
                var childToDelete = (MathExpressionScope)root[0];
                root.Set(childToDelete.GetItems());
                childToDelete.Clear();
            }
        }

        private void OpenWithSamePriority(MathExpressionScope root)
        {
            
        }

        #endregion

        public override void Optimize(MathExpressionScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            DeleteRedundantBraces(scope);

            GroupOperationsWithDifferentPriority(scope);

            SummarizeOperations(scope);

            SortMultiplicationAndDivision(scope);

            GroupMultiplicationByArgument(scope);
        }

        public static readonly SimpleMathExpressionOptimizer Instance = new SimpleMathExpressionOptimizer();
    }
}