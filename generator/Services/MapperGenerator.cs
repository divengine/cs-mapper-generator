using Divengine.CSMapperGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Text;

namespace Divengine.CSMapperGenerator.Services
{
    public class MapperGenerator
    {
        public static string RuntimeVersion = "1.0.0";

        public static void Generate(string configFile)
        {
            // Load the JSON configuration
            var jsonContent = File.ReadAllText(configFile);
            var jsonDirectory = Path.GetDirectoryName(configFile);
            var config = JsonConvert.DeserializeObject<MapperConfiguration>(jsonContent);

            // Process each mapper in the configuration
            foreach (var mapper in config?.Mappers ?? [])
            {
                var methods = new List<MethodDeclarationSyntax>();
                var usings = new HashSet<string>();

                foreach (var pair in mapper.ClassPairs)
                {
                    string sourceClassPath = Path.Combine(jsonDirectory ?? "", pair[0]);
                    string destinationClassPath = Path.Combine(jsonDirectory ?? "", pair[1]);

                    if (!File.Exists(sourceClassPath) || !File.Exists(destinationClassPath))
                    {
                        Console.WriteLine($"Warning: Could not find source or destination class file for {sourceClassPath} and {destinationClassPath}. Skipping...");
                        continue;
                    }

                    // Load and parse the class definitions
                    var sourceClassContent = File.ReadAllText(sourceClassPath);
                    var destinationClassContent = File.ReadAllText(destinationClassPath);

                    var sourceSyntaxTree = CSharpSyntaxTree.ParseText(sourceClassContent);
                    var destinationSyntaxTree = CSharpSyntaxTree.ParseText(destinationClassContent);

                    var sourceRoot = sourceSyntaxTree.GetCompilationUnitRoot();
                    var destinationRoot = destinationSyntaxTree.GetCompilationUnitRoot();

                    var sourceClassNode = sourceRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                    var destinationClassNode = destinationRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

                    // Extract the namespaces for the source and destination classes
                    var sourceNamespace = sourceRoot.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
                    var destinationNamespace = destinationRoot.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();

                    if (!string.IsNullOrEmpty(sourceNamespace))
                    {
                        usings.Add(sourceNamespace);
                    }

                    if (!string.IsNullOrEmpty(destinationNamespace))
                    {
                        usings.Add(destinationNamespace);
                    }

                    bool isClone = sourceClassNode.Identifier.Text == destinationClassNode.Identifier.Text;

                    // Generate the mapping methods (both directions)
                    var mappingMethodAToB = GenerateMappingMethod(sourceClassNode, destinationClassNode, isClone ? "Clone" : "Map");
                    methods.Add(mappingMethodAToB);

                    if (!isClone)
                    {
                        var mappingMethodBToA = GenerateMappingMethod(destinationClassNode, sourceClassNode);
                        methods.Add(mappingMethodBToA);
                    }
                }

                if (methods.Any())
                {
                    // Create the mapper class
                    var mapperClass = SyntaxFactory.ClassDeclaration(mapper.Mapper)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddMembers(methods.ToArray())
                        .NormalizeWhitespace();

                    // Create the namespace for the mapper class
                    var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(mapper.Namespace))
                        .AddMembers(mapperClass)
                        .NormalizeWhitespace();

                    // Create the Compilation Unit (file) with usings
                    var compilationUnit = SyntaxFactory.CompilationUnit()
                        .AddUsings(usings.Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u))).ToArray())
                        .AddMembers(namespaceDeclaration)
                        .NormalizeWhitespace();

                    // Generate the code
                    var code = compilationUnit.ToFullString();

                    mapper.OutputFolder = Path.Combine(jsonDirectory, mapper.OutputFolder);

                    // Ensure the output folder exists
                    Directory.CreateDirectory(mapper.OutputFolder);

                    // Save the generated class to the specified output file
                    var outputMapperFilePath = Path.Combine(mapper.OutputFolder, mapper.Mapper + ".cs");

                    // add comment on top of the code

                    code = $@"{AutoGeneratedFileHeader("Custom Mapper: " + mapper.Mapper)}{code}";

                    File.WriteAllText(outputMapperFilePath, code);

                    Console.WriteLine($"Mapping class {mapper.Mapper} generated successfully at {outputMapperFilePath}!");
                }
            }
        }

        static MethodDeclarationSyntax GenerateMappingMethod(ClassDeclarationSyntax fromClass, ClassDeclarationSyntax toClass, string methodName = "Map")
        {
            // Generate variable names based on class names
            string fromVarName = "fromEntity"; // Char.ToLowerInvariant(fromClass.Identifier.Text[0]) + fromClass.Identifier.Text.Substring(1);
            string toVarName = "toEntity"; // Char.ToLowerInvariant(toClass.Identifier.Text[0]) + toClass.Identifier.Text.Substring(1);

            // first return null if fromEntity is null

            var nullCheck = SyntaxFactory.IfStatement(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    SyntaxFactory.IdentifierName(fromVarName),
                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
                ),
                SyntaxFactory.Block(SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)))
            );
            
            // Create the destination variable declaration and assignment
            var toVarDeclaration = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName(toClass.Identifier.Text)
                )
                .AddVariables(
                    SyntaxFactory.VariableDeclarator(toVarName)
                        .WithInitializer(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.IdentifierName(toClass.Identifier.Text)
                            ).WithArgumentList(SyntaxFactory.ArgumentList())
                        ))
                )
            );

            // Create the list of assignment statements
            var statements = toClass.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(prop =>
                {
                    var sourceProperty = fromClass.Members
                        .OfType<PropertyDeclarationSyntax>()
                        .FirstOrDefault(p => IsMatchingProperty(p, prop));

                    if (sourceProperty != null)
                    {
                        // Direct assignment if property names match
                        return SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(toVarName),
                                    SyntaxFactory.IdentifierName(prop.Identifier.Text)
                                ),
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(fromVarName),
                                    SyntaxFactory.IdentifierName(sourceProperty.Identifier.Text)
                                )
                            )
                        );
                    }

                    return null;
                })
                .Where(statement => statement != null)
                .ToList();


            // Create the method block
            var methodBody = SyntaxFactory.Block(toVarDeclaration)
                .AddStatements(statements?.ToArray())
                .AddStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(toVarName)));

            // Add null check to the beginning of the method
            methodBody = methodBody.WithStatements(new SyntaxList<StatementSyntax>().Add(nullCheck).AddRange(methodBody.Statements));

            // Create the mapping method
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(toClass.Identifier.Text), methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(fromVarName))
                    .WithType(SyntaxFactory.ParseTypeName(fromClass.Identifier.Text)))
                .WithBody(methodBody);
        }

        /// <summary>
        /// Match properties
        /// </summary>
        /// <param name="sourceProp"></param>
        /// <param name="targetProp"></param>
        /// <returns></returns>
        static bool IsMatchingProperty(PropertyDeclarationSyntax sourceProp, PropertyDeclarationSyntax targetProp)
        {
            // Matching criteria: Check if the property names are equal or related in some common scenarios
            if (sourceProp.Identifier.Text.Equals(targetProp.Identifier.Text, StringComparison.OrdinalIgnoreCase))
            {
                // Matching criteria: Check if the property types are equal
                if (sourceProp.Type.ToString().Equals(targetProp.Type.ToString(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static string AutoGeneratedFileHeader(string title)
        {
            var header = new StringBuilder();
            header.AppendLine("//------------------------------------------------------------------------------");
            header.AppendLine("// <auto-generated>");
            header.AppendLine($"//    This code was generated by a Div CS Mapper Generator.");
            header.AppendLine($"//    Runtime Version: {RuntimeVersion}");
            header.AppendLine($"//    See: https://divengine.org");
            header.AppendLine($"//");
            header.AppendLine($"//    Changes to this file may cause incorrect behavior and will be lost if");
            header.AppendLine($"//    the code is regenerated.");
            header.AppendLine("// </auto-generated>");
            header.AppendLine("//------------------------------------------------------------------------------");
            header.AppendLine();
            header.AppendLine($"// {title}");
            header.AppendLine();

            return header.ToString();
        }
    }
}

