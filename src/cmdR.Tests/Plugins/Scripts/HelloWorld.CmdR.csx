using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cmdR;

namespace cmdR.Tests.Roslyn.Scripts
{
    public class ScriptHelloWorldCommand : ICmdRCommand
    {
        public string Command { get { return "hello-world-script-command"; } }
        public string Description { get { return "Prints out Hello World!"; } }
        
        public void Execute(IDictionary<string, string> param, CmdR cmdR)
        {
            cmdR.Console.WriteLine("Roslyn Script Plugin COMMAND would like to say: Hello World!");
        }
    }

    public class ScriptHelloWorldModule : ICmdRModule
    {
        public void Initalise(CmdR cmdR)
        {
            cmdR.RegisterRoute("hello-world-script-module", SayHello, "Prints out Hello World!");
        }

        private void SayHello(IDictionary<string, string> param, CmdR cmdR)
        {
            cmdR.Console.WriteLine("Roslyn Script Plugin MODULE would like to say: Hello World!");
        }
    }
}
