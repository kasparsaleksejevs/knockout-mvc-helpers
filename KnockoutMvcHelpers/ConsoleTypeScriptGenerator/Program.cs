using System;
using System.Reflection;
using System.Windows.Forms;

namespace ConsoleTypeScriptGenerator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("TypeScript generation console");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("Usage: supply required arguments in the command line, ");
            Console.WriteLine("    e.g., 'ConsoleTypeScriptGenerator.exe <Class_Full_Name> <Full_Path_to_Assembly.dll>'");
            Console.WriteLine("or use interactive mode and write required info manually");

            var mainColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            var typeToGen = string.Empty;
            var pathToAssembly = string.Empty;

            if (args.Length == 2)
            {
                typeToGen = args[0];
                pathToAssembly = args[1];
            }
            else
            {
                Console.WriteLine("===============");

                Console.WriteLine("Enter the full name of the class to generate TS from (e.g., MyNamespace.MyModel): ");
                typeToGen = Console.ReadLine();

                Console.WriteLine("Enter the full path to the containing assembly (e.g. \"C:\\stuff\\mydata.dll\"): ");
                pathToAssembly = Console.ReadLine();
            }

            Console.WriteLine("===============");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;

            var assembly = Assembly.LoadFrom(pathToAssembly);
            var targetType = assembly.GetType(typeToGen);

            var outputTs = new KnockMvc.TypeScriptGenerator.TypeScriptClassGenerator().Generate(targetType);

            Console.WriteLine(outputTs);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("===============");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Code generation complete!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press 'c' to copy TS to the clipboard, or any othe key to exit");
            Console.ForegroundColor = mainColor;
            var key = Console.ReadKey();
            if (key.KeyChar == 'c' || key.KeyChar == 'C')
                Clipboard.SetText(outputTs);
        }
    }
}
