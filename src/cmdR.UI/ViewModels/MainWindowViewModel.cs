using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using cmdR.CommandParsing;

namespace cmdR.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IWpfViewModel
    {
        private readonly CmdR _cmdR;

        public string Command { get; set; }
        public string Output { get; set; }
        public string Prompt { get { return _cmdR == null ? "c:\\users\\andy\\" : _cmdR.State.CmdPrompt; } set { } }


        public IList<string> CommandHistory { get; set; }
        public int? CommandHistoryPointer { get; set; }

        public MainWindowViewModel()
        {
            Command = "";
            Output = "";

            CommandHistory = new List<string>();
        }
        public MainWindowViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            _cmdR = new CmdR(console: new WpfConsole(this));

            _cmdR.State.CmdPrompt = GetUserDirectory();
            _cmdR.State.Variables.Add("path", GetUserDirectory());
            _cmdR.Console.WriteLine("Welcome to CmdR :D");

            Command = "";
            Output = "";

            CommandHistory = new List<string>();
            CommandHistoryPointer = null;
            
            InvokeOnBackgroundThread(() => {
                    _cmdR.Console.WriteLine("Discovering commands, please wait...");
                    _cmdR.AutoRegisterCommands();
                    _cmdR.Console.WriteLine("\n{0} routes registered\n", _cmdR.State.Routes.Count);
                    
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
                        CommandHistory.Add(Command);

                        _cmdR.ExecuteCommand(Command);
                        
                        Command = string.Empty;
                    }
                    catch (Exception e)
                    {
                        _cmdR.Console.WriteLine("An exception was thrown while running your command");
                        _cmdR.Console.WriteLine(" {0}\n", e.Message);
                    }

                    NotifyPropertyChanged("Output");
                    NotifyPropertyChanged("Command");
                    NotifyPropertyChanged("Prompt");
                });
        }

        public void HandleUpKeyPress()
        {
            // check to see if we have any history to cycle through before we do anything
            if (CommandHistory.Count() == 0)
                return;

            // if we haven't cycled yet and we are pressing Up we probably want to start at the begining
            if (CommandHistoryPointer == null)
                CommandHistoryPointer = 0;
            else
                CommandHistoryPointer += 1;

            if (CommandHistoryPointer >= CommandHistory.Count())
                CommandHistoryPointer = 0;

            if (CommandHistoryPointer < CommandHistory.Count())
            {
                Command = CommandHistory[CommandHistoryPointer.Value];
                NotifyPropertyChanged("Command");
            }
        }

        public void HandleDownKeyPress()
        {
            // check to see if we have any history to cycle through before we do anything
            if (CommandHistory.Count() == 0)
                return;

            // if we haven't cycled yet and we are pressing Down we probably want to see the last command
            if (CommandHistoryPointer == null)
                CommandHistoryPointer = CommandHistory.Count() -2;
            else
                CommandHistoryPointer -= 1;

            if (CommandHistoryPointer < 0)
                CommandHistoryPointer = CommandHistory.Count() - 1;

            if (CommandHistoryPointer < CommandHistory.Count())
            {
                Command = CommandHistory[CommandHistoryPointer.Value];
                NotifyPropertyChanged("Command");
            }
        }

        public void HandleTabKeyPress()
        {
            // todo: show a dropdown or autocomplete the current bit of text being types into the command prompt
        }
    }
}
