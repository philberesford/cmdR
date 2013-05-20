using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using cmdR.IO;

namespace cmdR.UI.CmdRModules
{
    public class GrepModule : ModuleBase, ICmdRModule
    {
        public void Initalise(CmdR cmdR, bool overwriteRoutes)
        {
            _cmdR = cmdR;

            cmdR.RegisterRoute("grep file regex", Grep, "Reads the file and matches its contents against the regex, each matching line is printed out to the console", overwriteRoutes);
            cmdR.RegisterRoute("grep-rep file output-file regex replace", GrepReplace, "Reads the file and matches its contents against the regex all matches will be replaced and the result will be written to the output-file\nUse /test to run without saving the results back to the file", overwriteRoutes);
            cmdR.RegisterRoute("grep-rep-multi regex-file match", GrepMultiMatchAndReplace, "Opens all the files matching the file-regex-match and runs all the regex-file against it", overwriteRoutes);
        }


        private void Grep(IDictionary<string, string> param, CmdR cmdR)
        {
            var path = Path.Combine((string) cmdR.State.Variables["path"], param["file"]);
            if (File.Exists(path))
            {
                var count = 0;
                var lines = 0;
                var match = new Regex(param["regex"]);

                foreach (var line in File.ReadLines(path))
                {
                    lines++;
                    if (match.IsMatch(line))
                    {
                        count++;
                        
                        //todo: highlight the matched text
                        cmdR.Console.WriteLine(" {0} {1} ", lines.ToString().PadRight(3), line);
                    }
                }

                cmdR.Console.WriteLine("{0} matches found", count);
            }
            else cmdR.Console.WriteLine("{0} does not exist", path);
        }


        private void GrepReplace(IDictionary<string, string> param, CmdR cmdR)
        {
            var path = Path.Combine((string)cmdR.State.Variables["path"], param["file"]);
            if (File.Exists(path))
            {
                var count = 0;
                var lines = 0;
                var match = new Regex(param["regex"]);
                var content = "";

                foreach (var line in File.ReadLines(path))
                {
                    lines++;
                    if (match.IsMatch(line))
                    {
                        count++;

                        var newline = match.Replace(line, param["replace"]);

                        //todo: highlight the matched text
                        cmdR.Console.WriteLine(" {0} {1} ", lines.ToString().PadRight(3), line);
                        cmdR.Console.WriteLine("     {0} ", newline);

                        content = string.Format("{0}{2}{1}", content, newline, (lines == 0) ? "" : "\r\n");
                    }
                    else content = string.Format("{0}{2}{1}", content, line, (lines == 0) ? "" : "\r\n");
                }

                if (count > 0 && !param.ContainsKey("/test"))
                    File.WriteAllText(path, content);

                cmdR.Console.WriteLine("{0} matches found", count);
            }
            else cmdR.Console.WriteLine("{0} does not exist", path);
        }


        public void GrepMultiMatchAndReplace(IDictionary<string, string> param, CmdR cmdR)
        {
            var filematch = new Regex(param["match"]);

            var files = Directory.GetFiles((string)cmdR.State.Variables["path"]).Where(x => filematch.IsMatch(x)).ToList();
            var regexMatches = GetRegexMatchAndReplaces(File.ReadAllText(Path.Combine((string)cmdR.State.Variables["path"], param["regex-file"])));

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

            var individualRegex = text.Split(new[] { "=== END ===\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in individualRegex)
            {
                var sections = item.Split(new[] { "=== REPLACE ===\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                results.Add(new RegexMatchAndReplace { Match = sections[0], Replace = sections[1] });
            }

            return results;
        }
    }
}
