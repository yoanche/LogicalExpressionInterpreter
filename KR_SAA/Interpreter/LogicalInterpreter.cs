using KR_SAA.DataStructures;
using KR_SAA.Models;
using System;
using System.Text;


namespace KR_SAA.Interpreter
{
    public class LogicalInterpreter
    {
        private readonly MyHashTable<string, DefinedFunction> _functions;
        private readonly FunctionTruthTable _truthTable;

        public LogicalInterpreter()
        {
            _functions = new MyHashTable<string, DefinedFunction>(16);
            _truthTable = new FunctionTruthTable(this);
        }

        public MyHashTable<string, DefinedFunction> Functions => _functions;
        public void Define(string definition)
        {
            string functionName = ParseFunctionName(definition);
            string[] operands = ParseFunctionArguments(definition);
            string body = ParseFunctionBody(definition);

            string postfixBody = ConvertToPostfix(body);

          
            Node expressionTree = BuildTree(postfixBody);

            if (_functions.ContainsKey(functionName))
            {
                throw new InvalidOperationException($"Function '{functionName}' is already defined.");
            }
            _functions.Add(functionName, new DefinedFunction(functionName, operands, expressionTree));
    

        }

        public int Solve(string command)
        {
            if (!command.StartsWith("SOLVE "))
                throw new InvalidOperationException("Invalid command: Must start with 'SOLVE'.");

   
            int nameEndIndex = FindIndex(command, '(');
            if (nameEndIndex == -1)
                throw new InvalidOperationException("Invalid command: Missing '(' after function name.");

            string functionName = ExtractSubstring(command, 6, nameEndIndex - 6).Trim();      
            int argsStartIndex = nameEndIndex + 1;
            int argsEndIndex = FindIndex(command, ')');
            if (argsEndIndex == -1)
                throw new InvalidOperationException("Invalid command: Missing ')' at the end of arguments.");

            string argsString = ExtractSubstring(command, argsStartIndex, argsEndIndex - argsStartIndex);
            int[] arguments = ParseAndValidateArguments(argsString);

            if (!_functions.ContainsKey(functionName))
                throw new InvalidOperationException($"Function '{functionName}' not defined.");

            DefinedFunction function = _functions.GetValue(functionName);
           

            if (function.Operands.Length != arguments.Length)
                throw new InvalidOperationException($"Argument count mismatch for function '{functionName}'. Expected {function.Operands.Length}, got {arguments.Length}.");

          
            var operandValues = new MyDictionary<string, int>();
            for (int i = 0; i < function.Operands.Length; i++)
            {
                operandValues.Add(function.Operands[i], arguments[i]);
            }
            return EvaluateTree(function.ExpressionTree, operandValues);
        }

        public void All(string command)
        {
            string functionName = ParseFunctionNameFromAll(command);

            if (!_functions.ContainsKey(functionName))
            {
                throw new InvalidOperationException($"Function '{functionName}' not defined.");
            }

            _truthTable.PrintTruthTable(functionName);
        }
        private string ParseFunctionNameFromAll(string command)
        {
            int startIndex = 4;

            int length = 0;
            while (startIndex + length < command.Length && command[startIndex + length] != ' ')
            {
                length++;
            }

            if (length == 0)
            {
                throw new InvalidOperationException("Invalid ALL command: Missing function name.");
            }
            char[] nameChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                nameChars[i] = command[startIndex + i];
            }

            return new string(nameChars);
        }



        private int EvaluateTree(Node node, MyDictionary<string, int> operandValues)
        {
            if (node == null) throw new InvalidOperationException("EvaluateTree: Node is null.");

            if (operandValues.ContainsKey(node.NodeValue))
            {
                Console.WriteLine($"Operand '{node.NodeValue}' found with value {operandValues.Get(node.NodeValue)}");
                return operandValues.Get(node.NodeValue);
            }

            Console.WriteLine($"Evaluating Node: {node.NodeValue}");
            if (node.NodeValue == "&")
            {
                return EvaluateTree(node.LeftChild, operandValues) & EvaluateTree(node.RightChild, operandValues);
            }
            else if (node.NodeValue == "|")
            {
                return EvaluateTree(node.LeftChild, operandValues) | EvaluateTree(node.RightChild, operandValues);
            }
            else if (node.NodeValue == "!")
            {
                return EvaluateTree(node.LeftChild, operandValues) == 0 ? 1 : 0;
            }
            else
            {
                throw new InvalidOperationException($"Unknown node value: {node.NodeValue}");
            }

        }

        private string ParseFunctionName(string definition)
        {
            int index = FindIndex(definition, '(');
            if (index == -1)
                throw new InvalidOperationException("Invalid function definition: Missing '('.");

            return ExtractSubstring(definition, 7, index - 7).Trim();
        }
       

        private int[] ParseAndValidateArguments(string argsString)
        {
            var argsList = new MyList<int>(argsString.Length);
            int currentIndex = 0;

            while (currentIndex < argsString.Length)
            {
                if (char.IsWhiteSpace(argsString[currentIndex]))
                {
                    currentIndex++;
                    continue;
                }

                int argStart = currentIndex;
                while (currentIndex < argsString.Length && argsString[currentIndex] != ',')
                {
                    currentIndex++;
                }

                string argument = ExtractSubstring(argsString, argStart, currentIndex - argStart).Trim();

                if (!int.TryParse(argument, out int value))
                {
                    throw new InvalidOperationException($"Invalid argument: '{argument}' is not a valid number.");
                }

                argsList.Add(value);

                if (currentIndex < argsString.Length && argsString[currentIndex] == ',')
                {
                    currentIndex++;
                }
            }
            int[] result = new int[argsList.Count];
            for (int i = 0; i < argsList.Count; i++)
            {
                result[i] = argsList[i];
            }

            return result;
        
           
        }
      

        private void CheckNestedFunctions(Node node)
        {
            if (node == null) return;

            if (node.NodeValue.StartsWith("func") && !_functions.ContainsKey(node.NodeValue))
                throw new InvalidOperationException($"Undefined function: {node.NodeValue}.");

            CheckNestedFunctions(node.LeftChild);
            CheckNestedFunctions(node.RightChild);
        }

        private bool ContainsNode(Node node, string value)
        {
            if (node == null) return false;
            if (node.NodeValue == value) return true;

            return ContainsNode(node.LeftChild, value) || ContainsNode(node.RightChild, value);
        }

        private string[] ParseFunctionArguments(string definition)
        {
            int start = FindIndex(definition, '(') + 1;
            int end = FindIndex(definition, ')');
            if (end == -1)
                throw new InvalidOperationException("Invalid function definition: Missing ')'.");

            string args = ExtractSubstring(definition, start, end - start);
            var argsList = new MyList<string>(args.Length);
            int currentIndex = 0;

            while (currentIndex < args.Length)
            {
                if (char.IsWhiteSpace(args[currentIndex]))
                {
                    currentIndex++;
                    continue;
                }

                int argStart = currentIndex;
                while (currentIndex < args.Length && args[currentIndex] != ',')
                {
                    currentIndex++;
                }

                string argument = ExtractSubstring(args, argStart, currentIndex - argStart).Trim();
                argsList.Add(argument);

                if (currentIndex < args.Length && args[currentIndex] == ',')
                {
                    currentIndex++;
                }
            }

            string[] result = new string[argsList.Count];
            for (int i = 0; i < argsList.Count; i++)
            {
                result[i] = argsList[i];
            }

            return result;
        }


        private string ParseFunctionBody(string definition)
        {
            int colonIndex = FindIndex(definition, ':');
            if (colonIndex == -1)
                throw new InvalidOperationException("Invalid function definition: Missing ':'.");

            string body = ExtractSubstring(definition, colonIndex + 1).Trim();
            Console.WriteLine($"Function body extracted: {body}");

            int parenthesesBalance = 0;

            for (int i = 0; i < body.Length; i++)
            {
                char c = body[i];
                Console.WriteLine($"Processing character: {c}");

                if (c == '(')
                {
                    parenthesesBalance++;
                }
                else if (c == ')')
                {
                    parenthesesBalance--;
                    if (parenthesesBalance < 0)
                        throw new InvalidOperationException("Mismatched parentheses in the function body.");
                }
                else if (c == ',')
                {
                    if (parenthesesBalance <= 0)
                        throw new InvalidOperationException("Invalid character ',' found outside of parentheses.");
                }
                else if (!char.IsLetterOrDigit(c) && c != '&' && c != '|' && c != '!' &&
                         c != '(' && c != ')' && c != ' ' && c != '_')
                {
                    throw new InvalidOperationException($"Invalid character '{c}' found in the function body.");
                }
            }

            if (parenthesesBalance != 0)
                throw new InvalidOperationException("Mismatched parentheses in the function body.");

            Console.WriteLine("Function body parsed successfully.");
            return body;

        }

        private string ConvertToPostfix(string infix)
        {
            var precedence = new MyDictionary<char, int>();
            precedence.Add('!', 3);
            precedence.Add('&', 2);
            precedence.Add('|', 1);

            char[] output = new char[infix.Length];
            int outputIndex = 0;

            var stack = new MyStack<char>(infix.Length);

            int operandCount = 0, operatorCount = 0;

            for (int i = 0; i < infix.Length; i++)
            {
                char c = infix[i];

                if (char.IsWhiteSpace(c)) continue;

                if (char.IsLetterOrDigit(c))
                {
                    output[outputIndex++] = c;
                    operandCount++;
                }
                else if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        output[outputIndex++] = stack.Pop();
                    }
                    if (stack.Count == 0 || stack.Pop() != '(')
                        throw new InvalidOperationException("Mismatched parentheses in the expression.");
                }
                else if (precedence.ContainsKey(c))
                {
                    operatorCount++;
                    while (stack.Count > 0 && precedence.ContainsKey(stack.Peek()) &&
                           precedence.Get(stack.Peek()) >= precedence.Get(c))
                    {
                        output[outputIndex++] = stack.Pop();
                    }
                    stack.Push(c);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid character: {c}");
                }
            }

            while (stack.Count > 0)
            {
                char top = stack.Pop();
                if (top == '(')
                    throw new InvalidOperationException("Mismatched parentheses in the expression.");
                output[outputIndex++] = top;
            }

            if (operandCount - operatorCount != 1)
            {
                throw new InvalidOperationException("Invalid expression: Operand and operator count mismatch.");
            }

            return new string(output, 0, outputIndex);
        }

        private Node BuildTree(string expression)
        {
            MyStack<Node> stack = new MyStack<Node>(expression.Length);
           

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];
              

                if (char.IsLetterOrDigit(c))
                {
                    stack.Push(new Node(c.ToString()));
                  
                }
                else if (c == '&' || c == '|')
                {
                    Console.WriteLine($"Operator encountered: {c}");
                    if (stack.Count < 2)
                    {
                        throw new InvalidOperationException($"Not enough operands for operator '{c}' at position {i}. Stack size: {stack.Count}");
                    }
                    Node right = stack.Pop();
                    Node left = stack.Pop();
                    stack.Push(new Node(c.ToString(), left, right));
                }
                else if (c == '!')
                {
                  
                    if (stack.Count < 1)
                    {
                        throw new InvalidOperationException($"Not enough operands for operator '{c}' at position {i}. Stack size: {stack.Count}");
                    }
                    Node child = stack.Pop();
                    stack.Push(new Node(c.ToString(), child));
                }
                else
                {
                    throw new InvalidOperationException($"Invalid character: {c} at position {i}.");
                }
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException($"Tree building failed. Final stack size: {stack.Count}");
            }

          
            return stack.Pop();
        }

        private int FindIndex(string input, char target)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == target)
                {
                    return i;
                }
            }
            return -1;
        }

        private string ExtractSubstring(string input, int start, int length = -1)
        {
            if (start < 0 || start >= input.Length)
                throw new ArgumentOutOfRangeException(nameof(start), "Start index is out of range.");

            if (length == -1)
                length = input.Length - start;

            if (length < 0 || start + length > input.Length)
                throw new ArgumentOutOfRangeException(nameof(length), "Length is out of range.");

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = input[start + i];
            }

            return new string(result);
        }

    }
}
    
