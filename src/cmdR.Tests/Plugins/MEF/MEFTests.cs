using cmdR;
using System.Linq;
using NUnit.Framework;
using cmdR.Plugins.MEF;

namespace cmdR.Tests.Plugins.MEF
{
    [TestFixture]
    public class MEFTests
    {
        [Test]
        public void MEFFactory_CanLoadAsseblies()
        {
            var mefFactory = new MefFactory();

            var assembaliesLoaded = mefFactory.LoadPlugins();
            Assert.AreEqual(2, assembaliesLoaded);
        }

        [Test]
        public void MEFFactory_CanRegisterPluginCommands()
        {
            var cmd = new cmdR.CmdR("", new string[0]);
            cmd.AutoRegisterCommands();

            Assert.AreEqual(1, cmd.State.Routes.Count(x => x.Name == "hello-world-command"));
        }

        [Test]
        public void MEFFactory_CanRegisterPluginModules()
        {
            var cmdR = new CmdR("", new string[0]);
            cmdR.AutoRegisterCommands();

            Assert.AreEqual(1, cmdR.State.Routes.Count(x => x.Name == "hello-world-module"));
        }
    }
}
