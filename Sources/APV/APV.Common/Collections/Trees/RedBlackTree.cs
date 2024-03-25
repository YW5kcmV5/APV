using System;
using System.Collections.Generic;

namespace RealtyStorage.Common.RealtySystem.Collections.Trees
{
    /// <summary>
    /// Красно-черное дерево
    /// </summary>
    /// <see cref="http://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B0%D1%81%D0%BD%D0%BE-%D1%87%D1%91%D1%80%D0%BD%D0%BE%D0%B5_%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE#.D0.A1.D1.80.D0.B0.D0.B2.D0.BD.D0.B5.D0.BD.D0.B8.D0.B5_.D1.81_.D0.B8.D0.B4.D0.B5.D0.B0.D0.BB.D1.8C.D0.BD.D0.BE_.D1.81.D0.B1.D0.B0.D0.BB.D0.B0.D0.BD.D1.81.D0.B8.D1.80.D0.BE.D0.B2.D0.B0.D0.BD.D0.BD.D1.8B.D0.BC_.D0.90.D0.92.D0.9B-.D0.B4.D0.B5.D1.80.D0.B5.D0.B2.D0.BE.D0.BC"/>
    public class RedBlackTree<TKey, TValue>
    {
        private enum TreeRotation
        {
            LeftRightRotation = 4,
            LeftRotation = 1,
            RightLeftRotation = 3,
            RightRotation = 2
        }

        private class Node
        {
            // Fields
            // Methods
            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                IsRed = true;
            }

            public Node(TKey key, TValue value, bool isRed)
            {
                Key = key;
                Value = value;
                IsRed = isRed;
            }

            // Properties
            public bool IsRed;
            public readonly TKey Key;
            public readonly TValue Value;
            public Node Left;
            public Node Right;
        }
 
        // Fields
        private readonly IComparer<TKey> _comparer;
        private int _count;
        private Node _root;
        private int _version;

        // Methods
        public RedBlackTree(IComparer<TKey> comparer )
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
        }

        public RedBlackTree()
            : this(null)
        {
        }

        public void Add(TKey key, TValue value)
        {
            if (_root == null)
            {
                _root = new Node(key, value, false);
                _count = 1;
            }
            else
            {
                Node root = _root;
                Node node = null;
                Node grandParent = null;
                Node greatGrandParent = null;
                int num = 0;
                while (root != null)
                {
                    num = _comparer.Compare(key, root.Key);
                    if (num == 0)
                    {
                        _root.IsRed = false;
                        throw new ArgumentException("Argument_AddingDuplicate");
                    }
                    if (Is4Node(root))
                    {
                        Split4Node(root);
                        if (IsRed(node))
                        {
                            InsertionBalance(root, ref node, grandParent, greatGrandParent);
                        }
                    }
                    greatGrandParent = grandParent;
                    grandParent = node;
                    node = root;
                    root = (num < 0) ? root.Left : root.Right;
                }
                var current = new Node(key, value);
                if (num > 0)
                {
                    node.Right = current;
                }
                else
                {
                    node.Left = current;
                }
                if (node.IsRed)
                {
                    InsertionBalance(current, ref node, grandParent, greatGrandParent);
                }
                _root.IsRed = false;
                _count++;
                _version++;
            }
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
            _version++;
        }

        public bool Contains(TKey key)
        {
            return (FindNode(key) != null);
        }

        private Node FindNode(TKey key)
        {
            int num;
            for (Node node = _root; node != null; node = (num < 0) ? node.Left : node.Right)
            {
                num = _comparer.Compare(key, node.Key);
                if (num == 0)
                {
                    return node;
                }
            }
            return null;
        }

        public TValue BinarySearch(TKey key)
        {
            return FindNode(key).Value;
        }
        
        private static Node GetSibling(Node node, Node parent)
        {
            if (parent.Left == node)
            {
                return parent.Right;
            }
            return parent.Left;
        }
        
        private void InsertionBalance(Node current, ref Node parent, Node grandParent, Node greatGrandParent)
        {
            Node node;
            bool flag = grandParent.Right == parent;
            bool flag2 = parent.Right == current;
            if (flag == flag2)
            {
                node = flag2 ? RotateLeft(grandParent) : RotateRight(grandParent);
            }
            else
            {
                node = flag2 ? RotateLeftRight(grandParent) : RotateRightLeft(grandParent);
                parent = greatGrandParent;
            }
            grandParent.IsRed = true;
            node.IsRed = false;
            ReplaceChildOfNodeOrRoot(greatGrandParent, grandParent, node);
        }

        private static bool Is2Node(Node node)
        {
            return ((IsBlack(node) && IsNullOrBlack(node.Left)) && IsNullOrBlack(node.Right));
        }

        private static bool Is4Node(Node node)
        {
            return (IsRed(node.Left) && IsRed(node.Right));
        }

        private static bool IsBlack(Node node)
        {
            return ((node != null) && !node.IsRed);
        }

        private static bool IsNullOrBlack(Node node)
        {
            if (node != null)
            {
                return !node.IsRed;
            }
            return true;
        }

        private static bool IsRed(Node node)
        {
            return ((node != null) && node.IsRed);
        }

        private static void Merge2Nodes(Node parent, Node child1, Node child2)
        {
            parent.IsRed = false;
            child1.IsRed = true;
            child2.IsRed = true;
        }
        
        public TValue Remove(TKey key)
        {
            if (_root == null)
            {
                return default(TValue);
            }
            Node root = _root;
            Node parent = null;
            Node node3 = null;
            Node match = null;
            Node parentOfMatch = null;
            bool flag = false;
            TValue v = default;
            while (root != null)
            {
                if (Is2Node(root))
                {
                    if (parent == null)
                    {
                        root.IsRed = true;
                    }
                    else
                    {
                        Node sibling = GetSibling(root, parent);
                        if (sibling.IsRed)
                        {
                            if (parent.Right == sibling)
                            {
                                RotateLeft(parent);
                            }
                            else
                            {
                                RotateRight(parent);
                            }
                            parent.IsRed = true;
                            sibling.IsRed = false;
                            ReplaceChildOfNodeOrRoot(node3, parent, sibling);
                            node3 = sibling;
                            if (parent == match)
                            {
                                parentOfMatch = sibling;
                            }
                            sibling = (parent.Left == root) ? parent.Right : parent.Left;
                        }
                        if (Is2Node(sibling))
                        {
                            Merge2Nodes(parent, root, sibling);
                        }
                        else
                        {
                            TreeRotation rotation = RotationNeeded(parent, root, sibling);
                            Node newChild = null;
                            switch (rotation)
                            {
                                case TreeRotation.LeftRotation:
                                    sibling.Right.IsRed = false;
                                    newChild = RotateLeft(parent);
                                    break;

                                case TreeRotation.RightRotation:
                                    sibling.Left.IsRed = false;
                                    newChild = RotateRight(parent);
                                    break;

                                case TreeRotation.RightLeftRotation:
                                    newChild = RotateRightLeft(parent);
                                    break;

                                case TreeRotation.LeftRightRotation:
                                    newChild = RotateLeftRight(parent);
                                    break;
                            }
                            newChild.IsRed = parent.IsRed;
                            parent.IsRed = false;
                            root.IsRed = true;
                            ReplaceChildOfNodeOrRoot(node3, parent, newChild);
                            if (parent == match)
                            {
                                parentOfMatch = newChild;
                            }
                            node3 = newChild;
                        }
                    }
                }
                int num = flag ? -1 : _comparer.Compare(key, root.Key);
                if (num == 0)
                {
                    flag = true;
                    match = root;
                    parentOfMatch = parent;
                    v = match.Value;
                }
                node3 = parent;
                parent = root;
                root = num < 0 ? root.Left : root.Right;
            }
            if (match != null)
            {
                ReplaceNode(match, parentOfMatch, parent, node3);
                _count--;
            }
            if (_root != null)
            {
                _root.IsRed = false;
            }
            _version++;
            return v;
        }

        private void ReplaceChildOfNodeOrRoot(Node parent, Node child, Node newChild)
        {
            if (parent != null)
            {
                if (parent.Left == child)
                {
                    parent.Left = newChild;
                }
                else
                {
                    parent.Right = newChild;
                }
            }
            else
            {
                _root = newChild;
            }
        }

        private void ReplaceNode(Node match, Node parentOfMatch, Node succesor, Node parentOfSuccesor)
        {
            if (succesor == match)
            {
                succesor = match.Left;
            }
            else
            {
                if (succesor.Right != null)
                {
                    succesor.Right.IsRed = false;
                }
                if (parentOfSuccesor != match)
                {
                    parentOfSuccesor.Left = succesor.Right;
                    succesor.Right = match.Right;
                }
                succesor.Left = match.Left;
            }

            if (succesor != null)
            {
                succesor.IsRed = match.IsRed;
            }

            ReplaceChildOfNodeOrRoot(parentOfMatch, match, succesor);
        }

        private static Node RotateLeft(Node node)
        {
            Node right = node.Right;
            node.Right = right.Left;
            right.Left = node;
            return right;
        }

        private static Node RotateLeftRight(Node node)
        {
            Node left = node.Left;
            Node right = left.Right;
            node.Left = right.Right;
            right.Right = node;
            left.Right = right.Left;
            right.Left = left;
            return right;
        }

        private static Node RotateRight(Node node)
        {
            Node left = node.Left;
            node.Left = left.Right;
            left.Right = node;
            return left;
        }

        private static Node RotateRightLeft(Node node)
        {
            Node right = node.Right;
            Node left = right.Left;
            node.Right = left.Left;
            left.Left = node;
            right.Left = left.Right;
            left.Right = right;
            return left;
        }

        private static TreeRotation RotationNeeded(Node parent, Node current, Node sibling)
        {
            if (IsRed(sibling.Left))
            {
                if (parent.Left == current)
                {
                    return TreeRotation.RightLeftRotation;
                }
                return TreeRotation.RightRotation;
            }
            if (parent.Left == current)
            {
                return TreeRotation.LeftRotation;
            }
            return TreeRotation.LeftRightRotation;
        }

        private static void Split4Node(Node node)
        {
            node.IsRed = true;
            node.Left.IsRed = false;
            node.Right.IsRed = false;
        }

        private static void InfixTraverse(Node node, Action<Node> action)
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

        // Properties
        public IComparer<TKey> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }
    }
}
