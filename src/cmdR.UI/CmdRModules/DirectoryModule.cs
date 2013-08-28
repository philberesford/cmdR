using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cmdR.UI.CmdRModules
{
    public class DirectoryModule : ModuleBase, ICmdRModule
    {
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

            cmdR.RegisterRoute("ls search?", List, "List all files and directories in the current path with an optional RegEx search pattern", overwriteRoutes);
            cmdR.RegisterRoute("cd path", ChangeDirectory, "Sets the currently active path, all subsequent commands will be executed within this path", overwriteRoutes);
            
            cmdR.RegisterRoute("mkd directory", MakeDirectory, "Creates a directory", overwriteRoutes);
            cmdR.RegisterRoute("rnd match replace", RenameDirectory, "Renames a directory\n/t to run a test without modifying the system", overwriteRoutes);
            cmdR.RegisterRoute("rmd match", RemoveDirectory, "Rmeoves all directories matching the regex\n/all delete all subdirectories and files\n/t to run a test without modifying the system", overwriteRoutes);

            cmdR.RegisterRoute("mark name path?", Mark, "Marks a directory for quick access, this can be used with other commands, for example [mark home] [cd {home}] or [mark me c:\\users\\andy] [cd {me}]", overwriteRoutes);
            cmdR.RegisterRoute("mark-delete name", MarkDelete, "Delets the Mark from the list of stored marks", overwriteRoutes);
        }

        private void MarkDelete(IDictionary<string, string> param, CmdR cmdR)
        {
            var marks = _cmdR.State.Variables["marks"] as IDictionary<string, string>
                      ?? new Dictionary<string, string>();

            if (marks.ContainsKey(param["name"]))
            {
                marks.Remove(param["name"]);
                WriteLineYellow(string.Format("Mark {0} deleted", param["name"]));
            }
            else WriteLineRed(string.Format("Mark {0} does not exists", param["name"]));
        }


        private void Mark(IDictionary<string, string> param, CmdR cmdR)
        {
            if (param.ContainsKey("path"))
                StoreNewMark(param["name"], param["path"]);
            else
                StoreNewMark(param["name"], (string)cmdR.State.Variables["path"]);
        }


        private void StoreNewMark(string name, string path)
        {
            var marks = _cmdR.State.Variables["marks"] as IDictionary<string, string> 
                      ?? new Dictionary<string, string>();

            if (marks.ContainsKey(name))
                marks[name] = path;
            else
                marks.Add(name, path);
        }



        private void RemoveDirectory(IDictionary<string, string> param, CmdR cmdR)
        {
            var match = new Regex(param["match"]);

            foreach (var directory in Directory.GetDirectories((string)cmdR.State.Variables["path"]))
            {
                if (match.IsMatch(directory))
                {
                    if (param.ContainsKey("/t"))
                        WriteLineOrange(string.Format("\\{0}", GetEndOfPath(directory)));
                    else
                        Directory.Delete(directory, param.ContainsKey("/all"));
                }
            }
        }

        private void RenameDirectory(IDictionary<string, string> param, CmdR cmdR)
        {
            var match = new Regex(param["match"]);
            var count = 0;

            foreach (var directory in Directory.GetDirectories((string)cmdR.State.Variables["path"]))
            {
                if (match.IsMatch(directory))
                {
                    count++;

                    if (param.ContainsKey("/t"))
                        cmdR.Console.WriteLine("{0} to {1}", directory, match.Replace(directory, param["replace"]));
                    else
                        Directory.Move(directory, match.Replace(directory, match.Replace(directory, param["replace"])));
                }

                if (param.ContainsKey("/t"))
                    cmdR.Console.WriteLine("{0} directories would be renamed", count);
                else
                    cmdR.Console.WriteLine("{0} directories renamed", count);
            }
        }

        private void MakeDirectory(IDictionary<string, string> param, CmdR cmdR)
        {
            Directory.CreateDirectory(Path.Combine((string)cmdR.State.Variables["path"], param["directory"]));
        }


        private void ChangeDirectory(IDictionary<string, string> param, CmdR cmd)
        {
            var path = GetPath(param["path"]);
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
                var regex = new Regex(param["search"]);

                foreach (var dir in Directory.GetDirectories(path).Where(dir => regex.IsMatch(dir)))
                    WriteOrange(string.Format("\\{0}   ", GetEndOfPath(dir.PadLeft(10))));

                foreach (var file in Directory.GetFiles(path).Where(file => regex.IsMatch(file)))
                    WritePink(string.Format("\\{0}   ", GetEndOfPath(file.PadLeft(10))));
            }
            else
            {
                foreach (var dir in Directory.GetDirectories(path))
                    WriteOrange(string.Format("\\{0}   ", GetEndOfPath(dir.PadLeft(10))));

                foreach (var file in Directory.GetFiles(path))
                    WritePink(string.Format("\\{0}   ", GetEndOfPath(file.PadLeft(10))));
            }
        }
    }
}