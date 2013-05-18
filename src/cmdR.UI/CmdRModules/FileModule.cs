using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace cmdR.UI.CmdRModules
{
    public class FileModule : DirectoryModuleBase, ICmdRModule
    {
        public void Initalise(CmdR cmdR, bool overwriteRoutes)
        {
            cmdR.RegisterRoute("head file rows?", Head, "Previews the first n lines of a file, n defaults to 10, but you can specify the number of rows to read", overwriteRoutes);
            cmdR.RegisterRoute("rn match replace", Rename, "Renames a file using a regex match and replace, if you want to run a test first use the switch /test", overwriteRoutes);
            cmdR.RegisterRoute("handles path?", Handles, "Lists the Open Handles on a directory", overwriteRoutes);
            cmdR.RegisterRoute("touch regex? date?", Touch, "Updates the Last Modified Date of all files in the current directory, you can use /modifed /created /accessed to choose which attributes to modify", overwriteRoutes);
            cmdR.RegisterRoute("re-multi regex-file file-regex-match", RegexMultiMatchAndReplace, "Opens all the files matching the file-regex-match and runs all the regex-file against it", overwriteRoutes);
        }

        public void RegexMultiMatchAndReplace(IDictionary<string, string> param, CmdR cmdR)
        {
            var filematch = new Regex(param["file-regex-match"]);

            var files = Directory.GetFiles((string)cmdR.State.Variables["Path"]).Where(x => filematch.IsMatch(x)).ToList();
            var regexMatches = GetRegexMatchAndReplaces(File.ReadAllText(param["regex-file"]));

            if (files.Count == 0)
            {
                cmdR.Console.WriteLine("No files matched the regex pattern {0}", param["file-regex-match"]);
                return;
            }

            if (regexMatches.Count == 0)
            {
                cmdR.Console.WriteLine("No regex match and replaces where found in the regex file {0}", param["regex-file"]);
                return;
            }

            var count = 0;
            foreach (var file in files)
            {
                cmdR.Console.WriteLine("Processing {0}", file);
                var content = File.ReadAllText(file);
                foreach (var match in regexMatches)
                {
                    content = match.GetRegex().Replace(content, match.Replace);
                }

                File.WriteAllText(file, content);
                count++;
            }

            cmdR.Console.WriteLine("{0} files processed", count);
        }

        private List<RegexMatchAndReplace> GetRegexMatchAndReplaces(string text)
        {
            var results = new List<RegexMatchAndReplace>();

            var individualRegex = text.Split(new [] { "=== END ====" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in individualRegex)
            {
                var sections = item.Split(new [] { "=== REPLACE ===" }, StringSplitOptions.RemoveEmptyEntries);
                results.Add(new RegexMatchAndReplace { Match = sections[0], Replace = sections[1] });
            }

            return results;
        }


        private void Touch(IDictionary<string, string> param, CmdR cmdR)
        {
            var path = cmdR.State.Variables["path"].ToString();
            if (Directory.Exists(path))
            {
                var modified = param.ContainsKey("/modified");
                var created = param.ContainsKey("/created");
                var accessed = param.ContainsKey("/accessed");

                var date = DateTime.Now;

                if (param.ContainsKey("date"))
                    date = DateTime.Parse(param["date"]);

                if (!modified && !created && !accessed)
                    modified = true;

                Regex match = null;
                if (param.ContainsKey("regex"))
                    match = new Regex(param["regex"]);

                foreach (var file in Directory.GetFiles(path).Where(x => match != null && match.IsMatch(x)))
                {
                    if (modified)
                        File.SetLastWriteTime(file, date);

                    if (created)
                        File.SetLastAccessTime(file, date);

                    if (accessed)
                        File.SetCreationTime(file, date);

                    cmdR.Console.WriteLine("touched {0}", file);
                }
            }
            else cmdR.Console.WriteLine("{0} does not exist", path);
        }

        private void Handles(IDictionary<string, string> param, CmdR cmdR)
        {
            var path = param.ContainsKey("path") ? GetPath(param["path"], cmdR.State.Variables) : (string)cmdR.State.Variables["path"];

            if (!Directory.Exists(path))
            {
                cmdR.Console.WriteLine("{0} doesnt exist", path ?? param["path"]);
                return;
            }

            cmdR.Console.WriteLine("looking for process with files handles open within {0}", path);

            var count = 0;
            foreach (var process in Win32Processes.GetProcessesLockingFile(path))
            {
                cmdR.Console.WriteLine(" {0}", process.ProcessName);
                count++;
            }

            cmdR.Console.WriteLine("{0} process with files handles open within {1}\n", count, path);
        }


        private void Rename(IDictionary<string, string> param, CmdR cmdR)
        {
            var match = new Regex(param["match"]);
            var count = 0;
            var errors = 0;

            foreach (var file in Directory.GetFiles((string)cmdR.State.Variables["path"]).Where(file => match.IsMatch(file)))
            {
                if (param.ContainsKey("/test"))
                {
                    count++;
                    cmdR.Console.WriteLine(" {0} to {1}", file, match.Replace(file, param["replace"]));
                }
                else
                {
                    try
                    {
                        File.Move(file, match.Replace(file, param["replace"]));
                        Console.WriteLine("{0}", match.Replace(file, param["replace"]));

                        count++;
                    }
                    catch (Exception e)
                    {
                        cmdR.Console.WriteLine("An exception occurred while renaming {0}", file);
                        errors++;
                    }
                }
            }

            if (param.ContainsKey("/test"))
                cmdR.Console.WriteLine("{0} files matched", count);
            else
                cmdR.Console.WriteLine("{0} files renames with {1} errors", count, errors);
        }

        private void Head(IDictionary<string, string> param, CmdR cmdR)
        {
            var path = Path.Combine((string)cmdR.State.Variables["path"], param["file"]);
            var take = param.ContainsKey("rows") ? int.Parse(param["rows"]) : 10;

            if (File.Exists(path))
            {
                cmdR.Console.WriteLine("PREVIEWING THE FIRST {0} LINES OF {1}", take, param["file"]);

                var count = 0;
                foreach (var line in File.ReadLines(path).Take(take))
                {
                    cmdR.Console.WriteLine(" {1}.  {0}", line, ++count);
                }
            }
            else cmdR.Console.WriteLine("{0} doesn't exist", param["file"]);

            cmdR.Console.WriteLine("");
        }
    }
}
