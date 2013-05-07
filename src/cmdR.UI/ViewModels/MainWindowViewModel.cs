using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using cmdR.CommandParsing;
using cmdR.IO;

namespace cmdR.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IWpfViewModel
    {
        private CmdR _cmdR;

        public string Command { get; set; }
        public string Output { get; set; }
        public string Prompt { get { return _cmdR.State.CmdPrompt; } set { } }

        public MainWindowViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            _cmdR = new CmdR(console: new WPFConsole(this));

            _cmdR.State.CmdPrompt = GetUserDirectory();
            _cmdR.State.Variables.Add("path", GetUserDirectory());
            _cmdR.Console.WriteLine("Welcome to CmdR :D");
            
            InvokeOnBackgroundThread(() => {
                    _cmdR.Console.WriteLine("Discovering commands, please wait...");
                    _cmdR.AutoRegisterCommands();
                    _cmdR.Console.WriteLine("\n{0} routes registered found\n", _cmdR.State.Routes.Count);
                    
                    NotifyPropertyChanged("Output");
                    NotifyPropertyChanged("Prompt");
                });
        }

        private string GetUserDirectory()
        {
            return Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).FullName;
        }

        public void HandleReturnKeyPressed()
        {
            if (string.IsNullOrEmpty(Command))
                return;

            InvokeOnBackgroundThread(() =>
                {
                    try
                    {
                        _cmdR.ExecuteCommand(Command);
                        
                        Command = string.Empty;
                    }
                    catch (Exception e)
                    {
                        _cmdR.Console.WriteLine("An exception was thrown while running your command");
                        _cmdR.Console.WriteLine("  {0}", e.Message);
                    }

                    NotifyPropertyChanged("Output");
                    NotifyPropertyChanged("Command");
                    NotifyPropertyChanged("Prompt");
                });
        }

        public void HandleUpKeyPress()
        {
            
        }

        public void HandleDownKeyPress()
        {
            
        }

        public void HandleTabKeyPress()
        {
            
        }
    }

    public class WPFConsole : ICmdRConsole
    {
        private readonly IWpfViewModel _viewmodel;

        public WPFConsole(IWpfViewModel viewModel)
        {
            _viewmodel = viewModel;
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void Write(string line, params object[] param)
        {
            _viewmodel.Output = string.Format("{0}{1}", _viewmodel.Output, string.Format(line, param));
        }

        public void Write(string line)
        {
            _viewmodel.Output = string.Format("{0}{1}", _viewmodel.Output, line);
        }

        public void WriteLine(string line, params object[] param)
        {
            _viewmodel.Output = string.Format("{0}\n{1}", _viewmodel.Output, string.Format(line, param));
        }

        public void WriteLine(string line)
        {
            _viewmodel.Output = string.Format("{0}\n{1}", _viewmodel.Output, line);
        }
    }
}
