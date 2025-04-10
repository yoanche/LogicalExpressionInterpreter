using KR_SAA.Interpreter;

namespace KR_SAA;
class Program
{
    static void Main(string[] args)
    {
       
        var interpreter = new LogicalInterpreter();
        bool debugMode = false;

       
        void DisplayMenu()
        {
            Console.WriteLine("DEFINE; SOLVE; ALL; EXIT");
        }

        DisplayMenu();

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();

            try
            {
                if (input.StartsWith("DEFINE "))
                {
                    interpreter.Define(input);
                    Console.WriteLine("Function defined successfully.");
                }
                else if (input.StartsWith("SOLVE "))
                {
                    int result = interpreter.Solve(input);
                    Console.WriteLine($"Result: {result}");
                }
                else if (input.StartsWith("ALL "))
                {
                    interpreter.All(input);
                }
                else if (input == "EXIT")
                {
                    Console.WriteLine("Exiting...");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid command.");
                }

                DisplayMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine(debugMode ? $"Error (debug mode): {ex}" : $"Error: {ex.Message}");
                DisplayMenu();
            }
        }
    }
}
            


    
