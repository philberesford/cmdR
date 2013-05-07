using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cmdR.UI.CmdRModules
{
    public class DirectoryModule : ICmdRModule
    {
        private CmdR _cmdR;


        public DirectoryModule()
        {
        }

        public DirectoryModule(CmdR cmdR)
        {
            Initalise(cmdR, false);
        }


        public void Initalise(CmdR cmdR, bool overwriteRoutes)
        {
            _cmdR = cmdR;

            cmdR.RegisterRoute("ls search?", List, "list all files and directories in the current path with an optional RegEx search pattern", overwriteRoutes);
            cmdR.RegisterRoute("cd path", ChangeDirectory, "sets the currently active path, all subsequent commands will be executed within this path", overwriteRoutes);
        }


        private void ChangeDirectory(IDictionary<string, string> param, CmdR cmd)
        {
            if (Directory.Exists(Path.Combine((string)cmd.State.Variables["path"], param["path"])))
            {
                var path = new DirectoryInfo(Path.Combine((string)cmd.State.Variables["path"], param["path"]));

                cmd.State.Variables["path"] = path.FullName;
                cmd.State.CmdPrompt = path.FullName;
            }
            else if (Directory.Exists(param["path"]))
            {
                cmd.State.Variables["path"] = param["path"];
                cmd.State.CmdPrompt = param["path"];
            }
            else cmd.Console.WriteLine("{0} does not exists", param["path"]);

            cmd.Console.WriteLine("working directory: {0}\n", cmd.State.Variables["path"]);
        }

        private void List(IDictionary<string, string> param, CmdR cmd)
        {
            var path = (string)cmd.State.Variables["path"];

            if (!Directory.Exists(path))
            {
                cmd.Console.WriteLine("{0} does not exist", path);
                return;
            }

            if (param.ContainsKey("search"))
            {
                var matches = 0;
                var regex = new Regex(param["search"]);

                foreach (var dir in Directory.GetDirectories(path).Where(dir => regex.IsMatch(dir)))
                {
                    cmd.Console.WriteLine("{0}\n", dir);
                    matches++;
                }

                foreach (var file in Directory.GetFiles(path).Where(file => regex.IsMatch(file)))
                {
                    cmd.Console.WriteLine("{0}\n", file);
                    matches++;
                }

                cmd.Console.WriteLine("{0} matches found for {1}", matches, param["search"]);
            }
            else
            {
                foreach (var dir in Directory.GetDirectories(path))
                    cmd.Console.WriteLine("{0}", dir);

                foreach (var file in Directory.GetFiles(path))
                    cmd.Console.WriteLine("{0}", file);
            }

            cmd.Console.WriteLine("");
        }
    }
}