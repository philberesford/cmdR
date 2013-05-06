using System.IO;
using System.Linq;
using NUnit.Framework;
using cmdR.Plugins.Scripts;

namespace cmdR.Tests.Plugins.Scripts
{
    [TestFixture]
    public class RoslynTests
    {
        [Test]
        public void RoslynScriptFactory_CanLoadAndCompileScriptsIntoDll()
        {
            var compileResult = RoslynScriptFactory.CompileScriptFiles(@".\Plugins\Scripts\");

            Assert.IsTrue(compileResult.Success);
            Assert.IsTrue(File.Exists(RoslynScriptFactory.SCRIPT_DDL_NAME));
            Assert.Greater(File.ReadAllBytes(RoslynScriptFactory.SCRIPT_DDL_NAME).Count(), 0, "The compiled dll does not contain any code O_o");
        }


        [Test]
        public void RoslynScriptFactory_CanRegisterPluginCommands()
        {
            var cmd = new cmdR.CmdR("", new string[0]);
            cmd.AutoRegisterCommands();

            Assert.AreEqual(1, cmd.State.Routes.Count(x => x.Name == "hello-world-script-command"));
        }

        [Test]
        public void MEFFactory_CanRegisterPluginModules()
        {
            var cmdR = new CmdR("", new string[0]);
            cmdR.AutoRegisterCommands();

            Assert.AreEqual(1, cmdR.State.Routes.Count(x => x.Name == "hello-world-script-module"));
        }
    }
}
