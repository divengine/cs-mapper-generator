/*
 * Div CSharp Mapper Generator
 * 
 * This program is a code generator designed to create mapping classes in C# based on user-defined class pairs.
 * The class pairs and mapping configuration are provided via a JSON file. For each pair of source and destination
 * classes specified, the program generates a mapper class with two mapping methods: one for mapping from the 
 * source class to the destination class, and another for the reverse mapping.
 *
 * The generated mapper class will include necessary using directives for the namespaces of the source and 
 * destination classes, ensuring that all references are correctly resolved. The mapper class itself, along with 
 * the mapping methods, is written to a specified output folder.
 *
 * Key Features:
 * - Automatically generates mapping methods for specified class pairs.
 * - Ensures the mapper class includes the correct using directives for dependencies.
 * - Outputs the generated mapper classes in a user-defined directory.
 *
 * Usage:
 * - Provide a JSON configuration file with the mapper name, output directory, namespace, and pairs of source and 
 *   destination classes.
 * - Run the program with the path to the JSON file as an argument.
 * - The program will generate the corresponding mapper classes and save them in the specified output folder.
 */

using Divengine.CSharpMapper.Services;

namespace Divengine.CSharpMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: <program> <json file>");
                return;
            }

            // Receive the JSON file path from the arguments
            string jsonFilePath = args[0];

            MapperGenerator.Generate(jsonFilePath);
        }
    }
}
