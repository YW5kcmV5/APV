using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using APV.Math.MathObjects.Expressions.Optimizers;
using APV.Math.MathObjects.Functions;

namespace APV.Math.MathObjects.Expressions
{
    [DebuggerDisplay("{GetMnemonicName()}")]
    public class MathExpressionScope : BaseMathExpressionItem
    {
        private readonly List<BaseMathExpressionItem> _items = new List<BaseMathExpressionItem>();

        public MathExpressionScope()
        {
        }

        public MathExpressionScope(MathExpressionScope parent)
            : base(parent)
        {
        }

        public void Clear()
        {
            ClearParent();
            _items.Clear();
        }

        public void Add(BaseMathExpressionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!_items.Contains(item))
            {
                item.SetParent(this);
                _items.Add(item);
            }
        }

        public void Add(int index, BaseMathExpressionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int existingIndex = _items.IndexOf(item);
            if (existingIndex != -1)
            {
                if (index != existingIndex)
                {
                    _items[existingIndex] = _items[index];
                    _items[index] = item;
                }
            }
            else
            {
                item.SetParent(this);
                _items.Insert(index, item);
            }
        }

        public void Set(int index, BaseMathExpressionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            int existingIndex = _items.IndexOf(item);
            if (existingIndex != -1)
            {
                if (existingIndex == index)
                {
                    return;
                }
                _items[existingIndex].ClearParent();
                _items.RemoveAt(existingIndex);
            }

            _items[index].ClearParent();
            item.SetParent(this);
            _items[index] = item;
        }

        public MathExpressionScope AddScope()
        {
            var scope = new MathExpressionScope(this);
            Add(scope);
            return scope;
        }

        public MathExpressionArgument Add(string argumentName)
        {
            var argument = new MathExpressionArgument(this, argumentName);
            Add(argument);
            return argument;
        }

        public MathExpressionOperation Add(MathFunctionType function)
        {
            var argument = new MathExpressionOperation(this, function);
            Add(argument);
            return argument;
        }

        public void Add(IEnumerable<BaseMathExpressionItem> children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            List<BaseMathExpressionItem> newChildren = children.Where(child => child != null).ToList();
            newChildren.ForEach(child => child.SetParent(this));
            _items.AddRange(newChildren);
        }

        public void Set(IEnumerable<BaseMathExpressionItem> children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            _items.Clear();
            Add(children);
        }

        public void Set(BaseMathExpressionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _items.Clear();
            Add(item);
        }

        public void RemoveRange(int index, int count)
        {
            for (int i = index; i < index + count; i++)
            {
                BaseMathExpressionItem item = _items[i];
                if (item.GetParent() == this)
                {
                    item.ClearParent();
                }
            }
            _items.RemoveRange(index, count);
        }

        public void RemoveAt(int index)
        {
            _items[index].ClearParent();
            _items.RemoveAt(index);
        }

        public void Remove(params BaseMathExpressionItem[] items)
        {
            if (items != null)
            {
                foreach (BaseMathExpressionItem item in items)
                {
                    if (item != null)
                    {
                        int index = _items.IndexOf(item);
                        if (index != -1)
                        {
                            RemoveAt(index);
                        }
                    }
                }
            }
        }

        public int IndexOf(BaseMathExpressionItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return _items.IndexOf(item);
        }

        public override string GetMnemonicName()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            foreach (BaseMathExpressionItem item in _items)
            {
                sb.Append(item.GetMnemonicName());
            }
            sb.Append(")");
            return sb.ToString();
        }

        public MathExpressionOperation[] GetOperations()
        {
            return _items
                .OfType<MathExpressionOperation>()
                .OrderByDescending(operation => operation.GetPriority())
                .ToArray();
        }

        public MathExpressionArgument[] GetArguments()
        {
            return _items
                .OfType<MathExpressionArgument>()
                .ToArray();
        }

        public MathExpressionScope[] GetScopes()
        {
            return _items
                .OfType<MathExpressionScope>()
                .ToArray();
        }

        public BaseMathExpressionItem[] GetArgumentsAndScopes()
        {
            var result = new List<BaseMathExpressionItem>();
            result.AddRange(GetArguments());
            result.AddRange(GetScopes());
            return result.ToArray();
        }

        public ulong[] GetPriorities()
        {
            return _items
                .OfType<MathExpressionOperation>()
                .Select(operation => operation.GetPriority())
                .Distinct()
                .ToArray();
        }

        public string[] GetOperationsNames()
        {
            return _items
                .OfType<MathExpressionOperation>()
                .Select(operation => operation.GetMnemonicName())
                .Distinct()
                .ToArray();
        }

        public MathExpressionScope GetRoot()
        {
            MathExpressionScope parent = GetParent();
            return (parent != null) ? parent.GetRoot() : this;
        }

        public BaseMathExpressionItem[] GetItems()
        {
            return _items.ToArray();
        }

        public BaseMathExpressionItem this[int index]
        {
            get { return _items[index]; }
            set { Set(index, value); }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public void Optimize(params BaseMathExpressionOptimizer[] optimizers)
        {
            if (optimizers != null)
            {
                BaseMathExpressionOptimizer[] availableOptimizers = optimizers.Where(optimizer => optimizer != null).Distinct().ToArray();

                foreach (BaseMathExpressionOptimizer optimizer in availableOptimizers)
                {
                    optimizer.Optimize(this);
                }

                var scopes = _items.OfType<MathExpressionScope>().ToArray();
                foreach (MathExpressionScope item in scopes)
                {
                    item.Optimize(availableOptimizers);
                }
            }
        }
    }
}