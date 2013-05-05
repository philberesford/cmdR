using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System.Reflection;


namespace cmdR.Roslyn
{
    public static class RoslynScriptFactory
    {
        public const string SCRIPT_DDL_NAME = "cmdR.compiled.scripts.dll";

        public static void CompileScriptFiles(string scriptPath)
        {
            var files = FindCmdRScriptFiles(scriptPath);
            
            var syntaxTrees = files.Select(csx => SyntaxTree.ParseFile(csx, ParseOptions.Default));
            var compiledScripts = CompileScipts(syntaxTrees);

            EmitResult result;
            using (var file = new FileStream(SCRIPT_DDL_NAME, FileMode.Create))
            {
                result = compiledScripts.Emit(file);
            }
        }

        private static Compilation CompileScipts(IEnumerable<SyntaxTree> syntaxTrees)
        {
            return Compilation.Create(SCRIPT_DDL_NAME, syntaxTrees: syntaxTrees,
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
