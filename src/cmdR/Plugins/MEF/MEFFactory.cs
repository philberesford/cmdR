using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace cmdR.Plugins.MEF
{
    public class MefFactory
    {
        [ImportMany(typeof(ICmdRCommand))]
        private static IEnumerable<ICmdRCommand> _commands;

        [ImportMany(typeof(ICmdRModule))]
        private static IEnumerable<ICmdRModule> _modules;



        public int LoadPlugins()
        {
            var container = GetContainer(GetAbsolutePath());
            container.ComposeParts(new object[] { this });

            return container.Catalog.Parts.Count();
            
            // (_commands == null ? 0 : _commands.Count()) + (_modules == null ? 0 : _modules.Count());
        }

        private string GetAbsolutePath()
        {
            //return new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "\\plugins\\mef")).FullName;
            return Path.Combine(Directory.GetCurrentDirectory(), "plugins\\mef");
        }

        private CompositionContainer GetContainer(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var thisAssembly = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            var catalog = new AggregateCatalog();
            
            catalog.Catalogs.Add(thisAssembly);
            catalog.Catalogs.Add(new DirectoryCatalog(path));

            return new CompositionContainer(catalog);
        }
    }
}