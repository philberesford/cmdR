using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cmdR.IO;

namespace cmdR.StdModules
{
    public class CoreModule : ICmdRModule
    {
        public CoreModule()
        {
        }

        public CoreModule(CmdR cmdR)
        {
            Initalise(cmdR);
        }

        public void Initalise(CmdR cmdR)
        {
            cmdR.RegisterRoute("help", Help, "List all the currently available commands");
            cmdR.RegisterRoute("?", Help, "List all the currently available commands");
        }


        private void Help(IDictionary<string, string> parameters, CmdR cmdR)
        {
            if (parameters.ContainsKey("route"))
            {
                if (cmdR.State.Routes.Any(x => x.Name.StartsWith(parameters["route"])))
                {
                    var route = cmdR.State.Routes.Single(x => x.Name == parameters["route"]);

                    cmdR.Console.Write("  {0}", route.Name);

                    foreach (var p in route.GetParameters())
                        cmdR.Console.Write(p.Value == ParameterType.Required ? " {0}" : " {0}?", p.Key);

                    cmdR.Console.WriteLine("");
                    if (!string.IsNullOrEmpty(route.Description))
                        cmdR.Console.WriteLine("  " + route.Description);
                }
                else cmdR.Console.WriteLine("  unknown command name [{0}]", parameters["route"]);
            }
            else
            {
                cmdR.Console.WriteLine("");

                foreach (var route in cmdR.State.Routes)
                    cmdR.Console.Write("{0}", route.Name.PadRight(20));

                cmdR.Console.WriteLine("");
            }
        }
    }
}