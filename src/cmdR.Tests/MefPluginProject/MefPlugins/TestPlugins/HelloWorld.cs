using System.Collections.Generic;
using System.ComponentModel.Composition;
using cmdR;

namespace TestPlugins
{
    [Export(typeof(ICmdRCommand))]
    public class HelloWorldCommand : ICmdRCommand
    {
        public string Command { get { return "hello-world-command"; } }
        public string Description { get { return "Prints out Hello World!"; } }

        public void Execute(IDictionary<string, string> param, CmdR cmdR)
        {
            cmdR.Console.WriteLine("Mef Plugin COMMAND would like to say: Hello World!");
        }
    }

    [Export(typeof(ICmdRModule))]
    public class HelloWorldModule : ICmdRModule
    {
        public HelloWorldModule()
        {
        }

        public HelloWorldModule(CmdR cmdR)
        {
            Initalise(cmdR);
        }

        public void Initalise(CmdR cmdR)
        {
            cmdR.RegisterRoute("hello-world-module", SayHello, "Prints out Hello World!");
        }

        private void SayHello(IDictionary<string, string> param, CmdR cmdR)
        {
            cmdR.Console.WriteLine("Mef Plugin MODULE would like to say: Hello World!");
        }

        
    }
}
