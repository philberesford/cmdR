using System.IO;
using System.Reflection;
using cmdR.Abstract;
using cmdR.CommandParsing;
using cmdR.Exceptions;
using cmdR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cmdR.Plugins.MEF;
using cmdR.Plugins.Scripts;

namespace cmdR
{
    public class CmdR
    {
        private IParseCommands _commandParser;
        private IRouteCommands _commandRouter;
        private IParseRoutes _routeParser;

        private ICmdRState _state;
        private ICmdRConsole _console;


        public ICmdRState State { get { return _state; } }
        public ICmdRConsole Console { get { return _console; } }



        public CmdR(string cmdPrompt = "> ", string[] exitcodes = null)
        {
            this.Init(new OrderedCommandParser(), new Routing(), new RouteParser(), new CmdRConsole(), new CmdRState(), exitcodes, cmdPrompt);
        }

        public CmdR(IParseCommands parser = null, IRouteCommands routing = null, IParseRoutes routeParser = null, ICmdRConsole console = null, ICmdRState state = null, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            this.Init(parser ?? new OrderedCommandParser(), routing ?? new Routing(), routeParser ?? new RouteParser(), console ?? new CmdRConsole(), state ?? new CmdRState(), exitcodes, cmdPrompt);
        }


        private void Init(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, ICmdRConsole console, ICmdRState state, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            _state = state;
            _state.CmdPrompt = cmdPrompt;
            _state.Routes = routing.GetRoutes();
            _state.ExitCodes = exitcodes ?? new string[] { "exit" };

            _console = console;

            _commandParser = parser;
            _commandRouter = routing;
            _routeParser = routeParser;
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null, bool overwriteRoutes = false)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);
            
            _commandRouter.RegisterRoute(name, parameters, action, description, overwriteRoutes);
        }


        public void RegisterRoute(string route, Action<IDictionary<string, string>, CmdR> action, string description = null, bool overwriteRoutes = false)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);

            _commandRouter.RegisterRoute(name, parameters, (dictionary, console, state) => action.Invoke(dictionary, this), description, overwriteRoutes);
        }



        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            if (!_commandParser.HasRoutes())
                _commandParser.SetRoutes(_commandRouter.GetRoutes());

            var commandName = "";
            var parameters = _commandParser.Parse(command, out commandName);
            var route = _commandRouter.FindRoute(commandName, parameters);

            route.Execute(parameters, _console, _state);
        }



        public void Run(string[] args)
        {
            var command = string.Join(" ", args);

            do
            {
                try
                {
                    this.ExecuteCommand(command);
                }
                catch (Exception e)
                {
                    _console.WriteLine("An exception was thrown while running your command\n  Message: {0}\n  Trace: {1}", e.Message, e.StackTrace);
                }

                _console.Write(_state.CmdPrompt);
                command = _console.ReadLine();
            }
            while (!_state.ExitCodes.Contains(command));
        }


        public void AutoRegisterCommands(bool overwriteRoutes = false)
        {
            LoadMefPlugins();
 
            RegisterModules(overwriteRoutes);
            RegisterCommands(overwriteRoutes);

            ComplieAndLoadScripts(overwriteRoutes);
        }

        private void ComplieAndLoadScripts(bool overwriteRoutes)
        {
            var compileResult = RoslynScriptFactory.CompileScriptFiles(@".\Plugins\Scripts\");
            if (compileResult.Success)
            {
                var moduleType = typeof(ICmdRModule);
                var commandType = typeof(ICmdRCommand);

                var assembly = Assembly.LoadFrom(RoslynScriptFactory.SCRIPT_DDL_NAME);
                var modules = assembly.GetTypes()
                                      .Where(t => moduleType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                      .Select(m => (ICmdRModule)Activator.CreateInstance(m));

                var commands = assembly.GetTypes()
                                       .Where(t => commandType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                       .Select(m => (ICmdRCommand)Activator.CreateInstance(m));

                RegisterModules(modules, overwriteRoutes);
                RegisterCommands(commands, overwriteRoutes);
            }
            else
            {
                Console.WriteLine("Unable to compile the scripts into a dll\n");

                foreach(var error in compileResult.Diagnostics)
                    _console.WriteLine("  {0}: {1}", error.Location, error.Info.GetMessage());
            } 
        }

        private void LoadMefPlugins()
        {
            var mefFactory = new MefFactory();
            var pluginCount = mefFactory.LoadPlugins();
        }

        private void RegisterModules(bool overwriteRoutes)
        {
            var type = typeof(ICmdRModule);
            var modules = AppDomain.CurrentDomain.GetAssemblies()
                                   .ToList()
                                   .SelectMany(s => s.GetTypes())
                                   .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                                   .Select(c => (ICmdRModule) Activator.CreateInstance(c));

            RegisterModules(modules, overwriteRoutes);
        }

        private void RegisterModules(IEnumerable<ICmdRModule> modules, bool overwriteRoutes)
        {
            var count = 0;
            foreach (var module in modules)
            {
                try
                {
                    module.Initalise(this, overwriteRoutes);
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading MODULE: {0}\nException: {1}", module.GetType().FullName, e.Message);
                }
            }
        }


        private void RegisterCommands(bool overwriteRoutes)
        {
            RegisterCommands(FindAllTypesImplementingICmdRCommand(), overwriteRoutes);
        }

        private void RegisterCommands(IEnumerable<ICmdRCommand> commands, bool overwriteRoutes)
        {
            var count = 0;
            foreach (var cmd in commands)
            {
                try
                {
                    RegisterRoute(cmd.Command, cmd.Execute, cmd.Description, overwriteRoutes);
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading COMMAND: {0}\nException: {1}", cmd.GetType().FullName, e.Message);
                }
            }
        }

        private IEnumerable<ICmdRCommand> FindAllTypesImplementingICmdRCommand()
        {
            var type = typeof(ICmdRCommand);
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .ToList()
                                          .SelectMany(s => s.GetTypes())
                                          .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                                          .Select(c => (ICmdRCommand)Activator.CreateInstance(c));
        }
    }
}