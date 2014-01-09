﻿using System;
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
            Initalise(cmdR, false);
        }

        public void Initalise(CmdR cmdR, bool overwriteRoutes)
        {
            cmdR.RegisterRoute("help command?", Help, "List all the currently available commands", overwriteRoutes);
            cmdR.RegisterRoute("? command?", Help, "List all the currently available commands", overwriteRoutes);

            cmdR.RegisterRoute("reload-plugins", ReloadPlugins, "Forces a reload off all the plugins, all clashing routes will be replaced", overwriteRoutes);
        }


        private void Help(IDictionary<string, string> parameters, CmdR cmdR)
        {
            if (parameters.ContainsKey("command"))
            {
                if (cmdR.State.Routes.Any(x => x.Name.StartsWith(parameters["command"])))
                {
                    var route = cmdR.State.Routes.Single(x => x.Name == parameters["command"]);

                    cmdR.Console.Write("  {0}", route.Name);

                    foreach (var p in route.GetParameters())
                        cmdR.Console.Write(p.Value == ParameterType.Required ? " {0}" : " {0}?", p.Key);

                    if (!string.IsNullOrEmpty(route.Description))
                        cmdR.Console.WriteLine("  " + route.Description);
                }
                else cmdR.Console.WriteLine("  unknown command name [{0}]", parameters["command"]);
            }
            else
            {
                foreach (var route in cmdR.State.Routes)
                    cmdR.Console.Write("{0}", route.Name.PadRight(20));
            }
        }

        private void ReloadPlugins(IDictionary<string, string> param, CmdR cmdR)
        {
            cmdR.AutoRegisterCommands(true);
        }
    }
}