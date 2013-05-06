using System.Collections.Generic;
using System.IO;
using System.Linq;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace cmdR.Plugins.Scripts
{
    public static class RoslynScriptFactory
    {
        public const string SCRIPT_DDL_NAME = "cmdR.compiled.scripts.dll";

        public static EmitResult CompileScriptFiles(string scriptPath)
        {
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);

            if (File.Exists(SCRIPT_DDL_NAME))
            {
                File.Delete(SCRIPT_DDL_NAME);
            }

            var files = FindCmdRScriptFiles(scriptPath);
            
            var syntaxTrees = files.Select(csx => SyntaxTree.ParseText(File.ReadAllText(csx)));
            var compiledScripts = CompileScipts(syntaxTrees);

            EmitResult result;
            using (var file = new FileStream(SCRIPT_DDL_NAME, FileMode.Create))
            {
                result = compiledScripts.Emit(file);
            }

            return result;
        }

        private static Compilation CompileScipts(IEnumerable<SyntaxTree> syntaxTrees)
        {
            return Compilation.Create(SCRIPT_DDL_NAME, 
                options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                syntaxTrees: syntaxTrees,
                references: new[]
                    {
                        new MetadataFileReference(typeof(object).Assembly.Location),
                        new MetadataFileReference(typeof(System.Collections.Generic.HashSet<>).Assembly.Location),
                        new MetadataFileReference(typeof(System.Linq.Enumerable).Assembly.Location),
                        new MetadataFileReference(typeof(System.Text.ASCIIEncoding).Assembly.Location),
                        new MetadataFileReference(typeof(System.Threading.Tasks.Task).Assembly.Location),
                        new MetadataFileReference(typeof(CmdR).Assembly.Location)
                    });
        }

        private static IEnumerable<string> FindCmdRScriptFiles(string scriptPath)
        {
            var scripts = Directory.GetFiles(scriptPath, "*.CmdR.csx", SearchOption.AllDirectories);
            return scripts;
        }
    }
}
