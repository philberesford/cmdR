using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cmdR.Roslyn;

namespace cmdR.Tests.Roslyn
{
    [TestFixture]
    public class RoslynTests
    {
        [Test]
        public void RoslynScriptFactory_CanFindAndLoadScriptFiles()
        {
            RoslynScriptFactory.CompileScriptFiles(@".\Roslyn\Scripts\");

            Assert.IsTrue(File.Exists(RoslynScriptFactory.SCRIPT_DDL_NAME));
        }

        [Test]
        public void RoslynScriptFactory_CanLoadAndCompileScriptsIntoDll()
        {
            RoslynScriptFactory.CompileScriptFiles(@".\Roslyn\Scripts\");

            Assert.IsTrue(File.Exists(RoslynScriptFactory.SCRIPT_DDL_NAME));
            Assert.Greater(0, File.ReadAllBytes(RoslynScriptFactory.SCRIPT_DDL_NAME).Count(), "The compiled dll does not contain any code");
        }
    }
}
