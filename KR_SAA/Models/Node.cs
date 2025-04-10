using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_SAA.Models
{
    public class Node
    {
        public string NodeValue;
        public Node LeftChild;
        public Node RightChild;

        public Node(string value, Node left = null, Node right = null)
        {
            NodeValue = value;
            LeftChild = left;
            RightChild = right;
        }
    }
}