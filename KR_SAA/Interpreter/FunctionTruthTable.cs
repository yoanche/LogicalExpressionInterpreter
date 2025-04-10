using KR_SAA.Models;
using System;
using KR_SAA.DataStructures;

namespace KR_SAA.Interpreter
{
    public class FunctionTruthTable
    {
        private readonly LogicalInterpreter _interpreter;

        public FunctionTruthTable(LogicalInterpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public void PrintTruthTable(string functionName)
        {
            if (!_interpreter.Functions.ContainsKey(functionName))
            {
                throw new InvalidOperationException($"Function '{functionName}' not defined.");
            }

            DefinedFunction function = _interpreter.Functions.GetValue(functionName);
            string[] operands = function.Operands;
            int numOperands = operands.Length;
            int numRows = 1 << numOperands;

            Console.WriteLine($"{string.Join(", ", operands)} : {functionName}");

            for (int row = 0; row < numRows; row++)
            {
                int[] arguments = new int[numOperands];
                for (int col = 0; col < numOperands; col++)
                {
                    arguments[col] = (row & (1 << col)) != 0 ? 1 : 0;
                }

                int result = _interpreter.Solve($"SOLVE {functionName}({string.Join(", ", arguments)})");
                Console.WriteLine($"{string.Join(", ", arguments)} : {result}");
            }
        }


    }
}
