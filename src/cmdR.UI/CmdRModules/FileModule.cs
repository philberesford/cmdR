using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.UI.CmdRModules
{
    public class FileModule : ICmdRModule
    {
        public void Initalise(CmdR cmdR, bool overwriteRoutes)
        {
            cmdR.RegisterRoute("preview file rows?", Preview, "", overwriteRoutes);
            cmdR.RegisterRoute("rn match replace test?", Preview, "", overwriteRoutes);
        }

        private void Preview(IDictionary<string, string> param, CmdR cmdR)
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
