using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.Tests.Roslyn.Scripts
{
    public class HelloWorld : ICmdRCommand
    {
        public string Command { get { return "hello-world"; } }
        public string Description { get { return "Prints out Hello World!"; } }
        
        public void Execute(IDictionary<string, string> param, CmdR cmdR)
        {
            cmdR.Console.WriteLine("Hello World!");
        }
    }
}
