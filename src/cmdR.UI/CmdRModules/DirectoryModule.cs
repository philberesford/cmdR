using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cmdR.UI.CmdRModules
{
    public class Module : ModuleBase, ICmdRModule
    {
        public Module()
        {
        }

        public Module(CmdR cmdR)
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
            var path = GetPath(param["path"], cmd.State.Variables);
            if (path == null)
            {
                cmd.Console.WriteLine("{0} does not exist", param["path"]);
                return;
            }

            cmd.State.Variables["path"] = path;
            cmd.State.CmdPrompt = path;

            cmd.Console.WriteLine("<Run>working directory: </Run><Run Foreground=\"Yellow\">{0}</Run>", cmd.State.Variables["path"].ToString().XmlEscape());
        }

        private void List(IDictionary<string, string> param, CmdR cmd)
        {
            var path = (string)cmd.State.Variables["path"];

            if (!Directory.Exists(path))
            {
                cmd.Console.WriteLine("{0} does not exist", path);
                return;
            }

            _cmdR.Console.WriteLine("");

            if (param.ContainsKey("search"))
            {
                var matches = 0;
                var regex = new Regex(param["search"]);

                foreach (var dir in Directory.GetDirectories(path).Where(dir => regex.IsMatch(dir)))
                {
                    WriteOrange(GetEndOfPath(dir + "\t"));
                    matches++;
                }

                foreach (var file in Directory.GetFiles(path).Where(file => regex.IsMatch(file)))
                {
                    WritePink(GetEndOfPath(file + "\t"));
                    matches++;
                }

                cmd.Console.WriteLine("{0} matches found for {1}", matches, param["search"]);
            }
            else
            {
                foreach (var dir in Directory.GetDirectories(path))
                    WriteYellow(GetEndOfPath(dir + "\t"));

                foreach (var file in Directory.GetFiles(path))
                    WriteMagenta(GetEndOfPath(file + "\t"));
            }
        }

        protected string GetEndOfPath(string path)
        {
            var getEndOfPath = new Regex(@"(?<=\\)[^\\]{1,}$");
            return getEndOfPath.IsMatch(path) ? getEndOfPath.Matches(path)[0].ToString() : path;
        }
    }
}