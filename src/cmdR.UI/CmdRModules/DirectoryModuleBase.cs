using System.Collections.Generic;
using System.IO;

namespace cmdR.UI.CmdRModules
{
    public class DirectoryModuleBase
    {
        protected string GetPath(string path, IDictionary<string, object> variables)
        {
            var result = "";

            if (Directory.Exists(Path.Combine((string)variables["path"], path)))
                return new DirectoryInfo(Path.Combine((string)variables["path"], path)).FullName;
            
            return Directory.Exists(path) ? path : null;
        }
    }
}