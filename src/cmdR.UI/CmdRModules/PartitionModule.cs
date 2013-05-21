using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cmdR.UI.CmdRModules
{
    public class PartitionModule : ModuleBase, ICmdRModule
    {
        public void Initalise(CmdR cmdR, bool overwriteRoutes)
        {
            _cmdR = cmdR;

            _cmdR.RegisterRoute("part match output parts", PartitionFile, "Partitions all files which match into a given number of sub files", overwriteRoutes);
            _cmdR.RegisterRoute("top match output take", Top, "Takes the top n rows from all files matching the regex", overwriteRoutes);
            _cmdR.RegisterRoute("tail match output take", Tail, "Takes the n bottom rows from all the files matching the regex", overwriteRoutes);
            _cmdR.RegisterRoute("top-tail match output take", TopAndTail, "Takes the n rows from the top and bottom of all the files which match the regex\n\t/middle take n items from the middle as well", overwriteRoutes);
        }



        private void PartitionFile(IDictionary<string, string> param, CmdR cmdR)
        {
            var pathRegex = new Regex(param["path-match"]);
            var replace = param["output"];


            throw new NotImplementedException();
        }


        private void TopAndTail(IDictionary<string, string> param, CmdR cmdR)
        {
            
        }

        private void Tail(IDictionary<string, string> param, CmdR cmdR)
        {
            throw new NotImplementedException();
        }

        private void Top(IDictionary<string, string> param, CmdR cmdR)
        {
            var pathRegex = new Regex(param["path-match"]);
            var output = param["output"];

            foreach (var file in Directory.GetFiles((string) _cmdR.State.Variables["path"]))
            {
                
            }
        }
    }
}
