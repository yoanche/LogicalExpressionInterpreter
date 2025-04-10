using System;

namespace KR_SAA.Models
{
    public class DefinedFunction
    {
        public string Name { get; private set; }
        public string[] Operands { get; private set; }
        public Node ExpressionTree { get; private set; }

        public DefinedFunction(string name, string[] operands, Node expressionTree)
        {
            Name = name;
            Operands = operands;
            ExpressionTree = expressionTree;
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", Operands)})";
        }
    }
}

