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
    public class RecursiveAvlTree<TKey, TValue>
    {
        private class AvlNode
        {
            public TKey Key;
            public TValue Value;

            public AvlNode Left;
            public AvlNode Right;
            public int Balance;

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
        /// Рекурсивный алгоритм с балансировкой (АВЛ-дерево)
        /// </summary>
        private static void Add(ref AvlNode node, TKey key, TValue value, ref bool flag, IComparer<TKey> comparer)
        {
            AvlNode node1, node2;
            if (node == null)
            {
                node = new AvlNode { Key = key, Value = value };
                flag = true;
            }
            else
            {
                int c = comparer.Compare(node.Key, key);
                if (c > 0)
                {
                    Add(ref node.Left, key, value, ref flag, comparer);
                    if (flag)
                    {
                        switch (node.Balance)
                        {
                            case 1:
                                node.Balance = 0;
                                flag = false;
                                break;
                            case 0:
                                node.Balance = -1;
                                break;
                            case -1:
                                node1 = node.Left;
                                if (node1.Balance == -1)
                                {
                                    node.Left = node1.Right;
                                    node1.Right = node;
                                    node.Balance = 0;
                                    node = node1;
                                }
                                else
                                {
                                    node2 = node1.Right;
                                    node1.Right = node2.Left;
                                    node2.Left = node1;
                                    node.Left = node2.Right;
                                    node2.Right = node;
                                    node.Balance = node2.Balance == -1 ? 1 : 0;
                                    if (node2.Balance == 1) node1.Balance = -1; else node1.Balance = 0;
                                    node = node2;
                                }
                                node.Balance = 0;
                                flag = false;
                                break;
                        }
                    }
                }
                else if (c < 0)
                {
                    Add(ref node.Right, key, value, ref flag, comparer);
                    if (flag)
                    {
                        switch (node.Balance)
                        {
                            case -1:
                                node.Balance = 0;
                                flag = false;
                                break;
                            case 0:
                                node.Balance = 1;
                                break;
                            case 1:
                                node1 = node.Right;
                                if (node1.Balance == 1)
                                {
                                    node.Right = node1.Left;
                                    node1.Left = node;
                                    node.Balance = 0;
                                    node = node1;
                                }
                                else
                                {
                                    node2 = node1.Left;
                                    node1.Left = node2.Right;
                                    node2.Right = node1;
                                    node.Right = node2.Left;
                                    node2.Left = node;
                                    if (node2.Balance == 1) node.Balance = -1; else node.Balance = 0;
                                    node1.Balance = node2.Balance == -1 ? 1 : 0;
                                    node = node2;
                                }
                                node.Balance = 0;
                                flag = false;
                                break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Удаление элемента
        /// </summary>
        private static TValue Remove(ref AvlNode node, TKey key, ref bool flag, IComparer<TKey> comparer)
        {
            AvlNode v;
            TValue r;
            int c = comparer.Compare(key, node.Key);
            if (c < 0)// (key < node.Key)
            {
                r = Remove(ref node.Left, key, ref flag, comparer);
                BalanceRight(ref node, ref flag);
            }
            else if (c > 0) //(key > node.Key)
            {
                r = Remove(ref node.Right, key, ref flag, comparer);
                BalanceLeft(ref node, ref flag);
            }
            else
            {
                v = node;
                r = node.Value;

                bool isLeftNull = node.Left == null;
                bool isRightNull = node.Right == null;

                if((isLeftNull) || (isRightNull))
                {
                    if((isLeftNull) && (isRightNull))
                    {
                        node = null;
                        flag = true;
                    }
                    else if (isLeftNull)
                    {
                        node = v.Right;
                        flag = true;
                    }
                    else
                    {
                        node = v.Left;
                        flag = true;
                    }
                }
                else
                {
                    if (v.Balance >= 0) //Правое поддерево больше или равно левому
                    {
                        RemoveRightElement(ref v.Right, ref v, ref flag);
                        BalanceLeft(ref node, ref flag);
                    }
                    else
                    {
                        RemoveLeftElement(ref v.Left, ref v, ref flag);
                        BalanceRight(ref node, ref flag);
                    }
                }
            }
            return r;
        }
        
        /// <summary>
        /// Обработка случая когда удаляемый элемент имеет двух потомков, удаление из левого поддерева
        /// </summary>
        private static void RemoveLeftElement(ref AvlNode tr, ref AvlNode v, ref bool flag)
        {
            if (tr.Right != null)
            {
                RemoveLeftElement(ref tr.Right, ref v, ref flag);
                BalanceLeft(ref tr, ref flag);
            }
            else
            {
                v.Key = tr.Key;
                v.Value = tr.Value;
                v = tr;

                tr = tr.Left;
                flag = true;
            }
        }
        
        /// <summary>
        /// Обработка случая когда удаляемый элемент имеет двух потомков, удаление из правого поддерева
        /// </summary>
        private static void RemoveRightElement(ref AvlNode tr, ref AvlNode v, ref bool flag)
        {
            if (tr.Left != null)
            {
                RemoveRightElement(ref tr.Left, ref v, ref flag);
                BalanceRight(ref tr, ref flag);
            }
            else
            {
                v.Key = tr.Key;
                v.Value = tr.Value;
                v = tr;

                tr = tr.Right;
                flag = true;
            }
        }
        
        /// <summary>
        /// Восстановление баланса при удалении из правого поддерева
        /// </summary>
        private static void BalanceLeft(ref AvlNode a, ref bool flag)
        {
            AvlNode b, c;
            if (flag)
            {
                switch (a.Balance)
                {
                    case 1:
                        a.Balance = 0;
                        break;
                    case 0:
                        a.Balance = -1;
                        break;
                    case -1:
                        b = a.Left;
                        if ((b.Balance == -1) || b.Balance == 0)
                        {
                            a.Left = b.Right;
                            b.Right = a;
                            if (b.Balance == 0)
                            {
                                a.Balance = -1;
                                b.Balance = 1;
                            }
                            else
                            {
                                a.Balance = 0;
                                b.Balance = 0;
                            }
                            a = b;
                        }
                        else
                        {
                            c = b.Right;
                            b.Right = c.Left;
                            c.Left = b;
                            a.Left = c.Right;
                            c.Right = a;
                            a.Balance = c.Balance == -1 ? 1 : 0;
                            if (c.Balance == 1) b.Balance = -1; else b.Balance = 0;
                            c.Balance = 0;
                            a = c;
                        }
                        break;
                }
                flag = (a.Balance != -1) && (a.Balance != +1);
            }
        }
        
        /// <summary>
        /// Восстановление баланса при удалении из левого поддерева
        /// </summary>
        private static void BalanceRight(ref AvlNode a, ref bool flag)
        {
            AvlNode b, c;
            if (flag)
            {
                switch (a.Balance)
                {
                    case -1:
                        a.Balance = 0;
                        break;
                    case 0:
                        a.Balance = 1;
                        break;
                    case 1:
                        b = a.Right;
                        if ((b.Balance == 1) || b.Balance == 0)
                        {
                            a.Right = b.Left;
                            b.Left = a;
                            if (b.Balance == 0)
                            {
                                a.Balance = 1;
                                b.Balance = -1;
                            }
                            else
                            {
                                a.Balance = 0;
                                b.Balance = 0;
                            }
                            a = b;
                        }
                        else
                        {
                            c = b.Left;
                            b.Left = c.Right;
                            c.Right = b;
                            a.Right = c.Left;
                            c.Left = a;
                            if (c.Balance == 1) a.Balance = -1; else a.Balance = 0;
                            b.Balance = c.Balance == -1 ? 1 : 0;
                            c.Balance = 0;
                            a = c;
                        }
                        break;
                }
                flag = (a.Balance != -1) && (a.Balance != +1);
            }
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

        private static void PostfixTraverse(AvlNode node, Action<AvlNode> action)
        {
            if (node.Left != null)
            {
                PostfixTraverse(node.Left, action);
            }
            if (node.Right != null)
            {
                PostfixTraverse(node.Right, action);
            }
            action(node);
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

        public bool Add(TKey key, TValue value, IComparer<TKey> comparer = null)
        {
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                comparer = comparer ?? Comparer<TKey>.Default;
                _depth = null;

                bool flag = false;
                Add(ref _root, key, value, ref flag, comparer);
                return true;
            }
            return false;
        }

        public TValue Remove(TKey key, IComparer<TKey> comparer = null)
        {
            if (_keys.Contains(key))
            {
                _keys.Remove(key);
                comparer = comparer ?? Comparer<TKey>.Default;
                _depth = null;

                bool flag = false;
                return Remove(ref _root, key, ref flag, comparer);
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

        public bool Contains(TKey key)
        {
            return _keys.Contains(key);
        }

        public void Clear()
        {
            if (_root != null)
            {
                PostfixTraverse(_root, node =>
                {
                    node.Key = default(TKey);
                    node.Value = default(TValue);
                    node.Left = null;
                    node.Right = null;
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
                if(_depth == null)
                {
                    _depth = TreeDepth(_root);
                }
                return _depth.Value;
            }
        }
    }
}