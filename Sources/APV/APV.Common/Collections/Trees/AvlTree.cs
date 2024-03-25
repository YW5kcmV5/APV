using System;
using System.Collections.Generic;

namespace RealtyStorage.Common.RealtySystem.Collections.Trees
{
    /// <summary>
    /// http://www.niisi.ru/iont/projects/rfbr/90308/90308_miphi6.php
    /// http://www.jurnal.org/articles/2008/inf26.html
    /// http://ru.wikipedia.org/wiki/%D0%90%D0%92%D0%9B-%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE
    /// http://eternallyconfuzzled.com/tuts/datastructures/jsw_tut_avl.aspx
    /// </summary>
    public class AvlTree<TKey, TValue>
    {
        private class AvlNode
        {
            public TKey Key;
            public TValue Value;

            public AvlNode Left;
            public AvlNode Right;
            public AvlNode Parent;
            public int Balance;
            public int BalanceD;

            private int CalcedBalance
            {
                get
                {
                    return TreeDepth(Right) - TreeDepth(Left);
                }
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}({2})", Key, Balance, CalcedBalance);
            }
        }

        private AvlNode _root;
        private readonly HashSet<TKey> _keys = new HashSet<TKey>();
        private int? _depth;

        /// <summary>
        /// Малое левое вращение
        /// </summary>
        private static AvlNode SmallLeftRotate(AvlNode a)
        {
            AvlNode parent = a.Parent;
            AvlNode b = a.Right;
            AvlNode c = b.Left;

            b.Parent = parent;
            a.Parent = b;
            a.Right = c;
            if (c != null) c.Parent = a;
            b.Left = a;

            if (b.Balance == 0)
            {
                a.Balance = +1;
                b.Balance = -1;
            }
            else
            {
                a.Balance = 0;
                b.Balance = 0;
            }

            return b;
        }

        /// <summary>
        /// Большое левое вращение
        /// </summary>
        private static AvlNode BigLeftRotate(AvlNode a)
        {
            AvlNode parent = a.Parent;
            AvlNode b = a.Right;
            AvlNode c = b.Left;
            AvlNode m = c.Left;
            AvlNode n = c.Right;

            if (m != null) m.Parent = a;
            if (n != null) n.Parent = b;
            a.Parent = c;
            b.Parent = c;
            c.Parent = parent;

            a.Right = m;
            b.Left = n;
            c.Left = a;
            c.Right = b;

            if (c.Balance == 0)
            {
                a.Balance = 0;
                b.Balance = 0;
            }
            else if (c.Balance == +1)
            {
                a.Balance = -1;
                b.Balance = 0;
            }
            else
            {
                a.Balance = 0;
                b.Balance = +1;
            }
            c.Balance = 0;

            return c;
        }

        /// <summary>
        /// Малое правое вращение
        /// </summary>
        private static AvlNode SmallRightRotate(AvlNode a)
        {
            AvlNode parent = a.Parent;
            AvlNode b = a.Left;
            AvlNode c = b.Right;

            b.Parent = parent;
            a.Parent = b;
            a.Left = c;
            if (c != null) c.Parent = a;
            b.Right = a;

            if (b.Balance == 0)
            {
                a.Balance = -1;
                b.Balance = +1;
            }
            else
            {
                a.Balance = 0;
                b.Balance = 0;
            }

            return b;
        }

        /// <summary>
        /// Большое правое вращение
        /// </summary>
        private static AvlNode BigRightRotate(AvlNode a)
        {
            AvlNode parent = a.Parent;
            AvlNode b = a.Left;
            AvlNode c = b.Right;
            AvlNode m = c.Left;
            AvlNode n = c.Right;

            if (m != null) m.Parent = b;
            if (n != null) n.Parent = a;
            a.Parent = c;
            b.Parent = c;
            c.Parent = parent;

            b.Right = m;
            a.Left = n;
            c.Right = a;
            c.Left = b;

            if (c.Balance == 0)
            {
                a.Balance = 0;
                b.Balance = 0;
            }
            else if (c.Balance == +1)
            {
                a.Balance = 0;
                b.Balance = -1;
            }
            else
            {
                a.Balance = +1;
                b.Balance = 0;
            }
            c.Balance = 0;

            return c;
        }

        private void UpdateNode(AvlNode parent, AvlNode oldValue, AvlNode newValue)
        {
            if (parent != null)
            {
                if (parent.Left == oldValue)
                {
                    parent.Left = newValue;
                }
                else
                {
                    parent.Right = newValue;
                }
            }
            else
            {
                _root = newValue;
            }
        }

        private AvlNode RestoreBalance(AvlNode node)
        {
            AvlNode a;
            AvlNode parent = node.Parent;
            //Restore balance
            if (node.Balance == -2)
            {
                int balance = node.Left.Balance;
                a = (balance == +1) ? BigRightRotate(node) : SmallRightRotate(node);
            }
            else //(node.Balance == +2)
            {
                int balance = node.Right.Balance;
                a = (balance == -1) ? BigLeftRotate(node) : SmallLeftRotate(node);
            }
            UpdateNode(parent, node, a);
            return a;
        }

        private bool UpdateBalance(AvlNode node)
        {
            //Calc balance
            if (node.BalanceD > 0)
            {
                switch (node.Balance)
                {
                    case -1:
                        node.Balance = 0;
                        return true;
                    case 0:
                        node.Balance = +1;
                        return false;
                    case 1:
                        node.Balance = +2;
                        RestoreBalance(node);
                        return true;
                }
            }
            else //node.BalanceD < 0
            {
                switch (node.Balance)
                {
                    case -1:
                        node.Balance = -2;
                        RestoreBalance(node);
                        return true;
                    case 0:
                        node.Balance = -1;
                        return false;
                    case 1:
                        node.Balance = 0;
                        return true;
                }
            }
            return false;
        }

        private AvlNode UpdateBalanceAfterDelete(AvlNode node, out bool balanced)
        {
            balanced = false;
            //Calc balance
            if (node.BalanceD > 0)
            {
                switch (node.Balance)
                {
                    case -1:
                        node.Balance = 0;
                        break;
                    case 0:
                        node.Balance = +1;
                        balanced = true;
                        break;
                    case 1:
                        node.Balance = +2;
                        node = RestoreBalance(node);
                        balanced = (node.Balance == -1) || (node.Balance == +1);
                        break;
                }
            }
            else if (node.BalanceD < 0)
            {
                switch (node.Balance)
                {
                    case -1:
                        node.Balance = -2;
                        node = RestoreBalance(node);
                        balanced = (node.Balance == -1) || (node.Balance == +1);
                        break;
                    case 0:
                        node.Balance = -1;
                        balanced = true;
                        break;
                    case 1:
                        node.Balance = 0;
                        break;
                }
            }
            return node.Parent;
        }

        private static AvlNode FindMax(AvlNode x)
        {
            x.BalanceD = -1;
            while (x.Right != null)
            {
                x = x.Right;
                x.BalanceD = -1;
            }
            return x;
        }

        private static AvlNode FindMin(AvlNode x)
        {
            x.BalanceD = +1;
            while (x.Left != null)
            {
                x = x.Left;
                x.BalanceD = +1;
            }
            return x;
        }

        private AvlNode RemoveElement(AvlNode x)
        {
            AvlNode parent;
            AvlNode v;
            if (x.Balance >= 0) //Правое поддерево больше или равно левому
            {
                v = FindMin(x.Right);
                parent = (v.Parent != x) ? v.Parent : x;

                x.Key = v.Key;
                x.Value = v.Value;
                x.BalanceD = -1;

                UpdateNode(v.Parent, v, v.Right);
                if (v.Right != null) v.Right.Parent = v.Parent;
                v.Parent = null;
            }
            else //Левое поддерево больше
            {
                v = FindMax(x.Left);
                parent = (v.Parent != x) ? v.Parent : x;

                x.Key = v.Key;
                x.Value = v.Value;
                x.BalanceD = +1;

                UpdateNode(v.Parent, v, v.Left);
                if (v.Left != null) v.Left.Parent = v.Parent;
                v.Parent = null;
            }
            return parent;
        }

        private static TValue BinarySearch(AvlNode node, TKey key, IComparer<TKey> comparer)
        {
            int c = comparer.Compare(key, node.Key);
            if (c == 0)
            {
                return node.Value;
            }
            AvlNode vsp = node;
            while (true)
            {
                vsp = (c < 0) ? vsp.Left : vsp.Right;
                c = comparer.Compare(key, vsp.Key);
                if (c == 0)
                {
                    return vsp.Value;
                }
            }
        }

        private static int TreeDepth(AvlNode node)
        {
            return (node != null) ? 1 + Math.Max(TreeDepth(node.Left), TreeDepth(node.Right)) : 0;
        }

        private static void InfixTraverse(AvlNode node, Action<AvlNode> action)
        {
            if (node.Left != null)
            {
                InfixTraverse(node.Left, action);
            }
            
            action(node);

            if (node.Right != null)
            {
                InfixTraverse(node.Right, action);
            }
        }

        private static void PostfixTraverse(AvlNode node, Action<TKey, TValue> action)
        {
            if (node.Left != null)
            {
                PostfixTraverse(node.Left, action);
            }
            
            if (node.Right != null)
            {
                PostfixTraverse(node.Right, action);
            }

            action(node.Key, node.Value);
        }

        private static void PrefixTraverse(AvlNode node, Action<TKey, TValue> action)
        {
            action(node.Key, node.Value);
            
            if (node.Left != null)
            {
                PrefixTraverse(node.Left, action);
            }

            if (node.Right != null)
            {
                PrefixTraverse(node.Right, action);
            }
        }

        private static bool CheckBalance(AvlNode node)
        {
            if (node == null)
            {
                return true;
            }
            bool balanced = true;
            InfixTraverse(node, x =>
                                    {
                                        int leftDepth = TreeDepth(x.Left);
                                        int rightDepth = TreeDepth(x.Right);
                                        int balance = x.Balance;
                                        int calcedBalance = rightDepth - leftDepth;
                                        bool correctLeftParent = (x.Left == null) || ((x.Left.Parent == x));
                                        bool correctRightParent = (x.Right == null) || ((x.Right.Parent == x));
                                        if ((!correctLeftParent) || (!correctRightParent))
                                        {
                                            balanced = false;
                                        }
                                        if (Math.Abs(calcedBalance) > 1)
                                        {
                                            balanced = false;
                                        }
                                        if (balance != calcedBalance)
                                        {
                                            balanced = false;
                                        }
                                    });
            return balanced;
        }

        public TValue Remove(TKey key, IComparer<TKey> comparer = null)
        {
            if (_keys.Contains(key))
            {
                comparer = comparer ?? Comparer<TKey>.Default;
                AvlNode node = _root;
                do
                {
                    int c = comparer.Compare(key, node.Key);
                    if (c < 0)
                    {
                        node.BalanceD = +1;
                        node = node.Left;
                    }
                    else if (c > 0)
                    {
                        node.BalanceD = -1;
                        node = node.Right;
                    }
                    else
                    {
                        TValue value = node.Value;
                        bool isLeftNull = node.Left == null;
                        bool isRightNull = node.Right == null;

                        AvlNode parent = node.Parent;
                        if ((isLeftNull) || (isRightNull))
                        {
                            if ((isLeftNull) && (isRightNull))
                            {
                                UpdateNode(parent, node, null);
                            }
                            else if (isLeftNull)
                            {
                                node.Right.Parent = parent;
                                UpdateNode(parent, node, node.Right);
                            }
                            else
                            {
                                node.Left.Parent = parent;
                                UpdateNode(parent, node, node.Left);
                            }
                        }
                        else
                        {
                            parent = RemoveElement(node);
                        }

                        while (parent != null)
                        {
                            bool balanced;
                            parent = UpdateBalanceAfterDelete(parent, out balanced);
                            if (balanced) break;
                        }

                        _keys.Remove(key);

                        return value;
                    }
                } while (true);
            }
            return default(TValue);
        }

        public TValue BinarySearch(TKey key, IComparer<TKey> comparer = null)
        {
            if (_keys.Contains(key))
            {
                comparer = comparer ?? Comparer<TKey>.Default;
                return BinarySearch(_root, key, comparer);
            }
            return default(TValue);
        }

        public bool Add(TKey key, TValue value, IComparer<TKey> comparer = null)
        {
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                comparer = comparer ?? Comparer<TKey>.Default;
                _depth = null;

                var newNode = new AvlNode { Key = key, Value = value };
                AvlNode node = _root;
                if (node == null)
                {
                    _root = newNode;
                    return true;
                }
                do
                {
                    int c = comparer.Compare(key, node.Key);
                    if (c < 0)
                    {
                        node.BalanceD = -1;
                        if (node.Left == null)
                        {
                            node.Left = newNode;
                            newNode.Parent = node;

                            do
                            {
                                if (UpdateBalance(node)) break;
                                node = node.Parent;
                            } while (node != null);

                            return true;
                        }
                        node = node.Left;
                    }
                    else
                    {
                        node.BalanceD = +1;
                        if (node.Right == null)
                        {
                            node.Right = newNode;
                            newNode.Parent = node;

                            do
                            {
                                if (UpdateBalance(node)) break;
                                node = node.Parent;
                            } while (node != null);

                            return true;
                        }
                        node = node.Right;
                    }
                } while (true);
            }
            return false;
        }

        public bool Contains(TKey key)
        {
            return _keys.Contains(key);
        }

        public void Clear()
        {
            if (_root != null)
            {
                InfixTraverse(_root, node =>
                {
                    node.Key = default(TKey);
                    node.Value = default(TValue);
                });
                _root = null;
                _keys.Clear();
            }
        }

        /// <summary>
        /// INFIX_TRAVERSE ( f ) — обойти всё дерево, следуя порядку (левое поддерево, вершина, правое поддерево).
        /// </summary>
        public void InfixTraverse(Action<TKey, TValue> action)
        {
            if ((_root != null) && (action != null))
            {
                InfixTraverse(_root, node => action(node.Key, node.Value));
            }
        }

        /// <summary>
        /// PREFIX_TRAVERSE ( f ) — обойти всё дерево, следуя порядку (вершина, левое поддерево, правое поддерево).
        /// </summary>
        public void PrefixTraverse(Action<TKey, TValue> action)
        {
            if (_root != null)
            {
                PrefixTraverse(_root, action);
            }
        }

        /// <summary>
        /// POSTFIX_TRAVERSE ( f ) — обойти всё дерево, следуя порядку (левое поддерево, правое поддерево, вершина).
        /// </summary>
        public void PostfixTraverse(Action<TKey, TValue> action)
        {
            if (_root != null)
            {
                PostfixTraverse(_root, action);
            }
        }

        public bool IsBalanced
        {
            get { return CheckBalance(_root); }
        }

        public int Count
        {
            get { return _keys.Count; }
        }

        public int Depth
        {
            get
            {
                if (_depth == null)
                {
                    _depth = TreeDepth(_root);
                }
                return _depth.Value;
            }
        }
    }
}