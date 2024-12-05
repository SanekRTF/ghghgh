using System;

namespace BinaryTrees
{
    public class BinaryTree<T>
        where T : IComparable
    {
        private TreeNode root { get; set; }

        public void Add(T value)
        {
            var newNode = new TreeNode(value);
            if (root == null)
            {
                root = newNode;
                return;
            }

            TreeNode parentNode = null;
            var currentNode = root;
            while (currentNode != null)
            {
                parentNode = currentNode;
                currentNode = (currentNode.Data.CompareTo(value) > 0) ? currentNode.Left : currentNode.Right;
            }

            if (parentNode.Data.CompareTo(value) > 0)
                parentNode.Left = newNode;
            else
                parentNode.Right = newNode;
        }

        public bool Contains(T value)
        {
            if (root == null)
                return false;
            var currentNode = root;
            while (!currentNode.Data.Equals(value))
            {
                var nextNode = (currentNode.Data.CompareTo(value) > 0) ? currentNode.Left : currentNode.Right;
                if (nextNode == null)
                    return false;
                currentNode = nextNode;
            }

            return true;
        }

        private class TreeNode
        {
            public T Data { get; }
            public TreeNode Left { get; set; }
            public TreeNode Right { get; set; }

            public TreeNode(T data)
            {
                Data = data;
            }
        }
    }
}