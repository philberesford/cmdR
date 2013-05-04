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
    public class MainWindowViewModel : ViewModelBase, IWPFViewModel
    {
        private CmdR _cmdR;

        public string Command { get; set; }
        public string Output { get; set; }

        public MainWindowViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            _cmdR = new CmdR(console: new WPFConsole(this));
            _cmdR.State.Variables.Add("path", GetUserDirectory());
            
            InvokeOnBackgroundThread(() => _cmdR.AutoRegisterCommands());
        }

        private string GetUserDirectory()
        {
            return Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).FullName;
        }

        public void HandleReturnKeyPressed()
        {
            InvokeOnBackgroundThread(() =>
                {
                    _cmdR.ExecuteCommand(Command);

                    Command = string.Empty;

                    NotifyPropertyChanged("Output");
                    NotifyPropertyChanged("Command");
                });
        }
    }

    public interface IWPFViewModel
    {
        string Command { get; set; }
        string Output { get; set; }
    }

    public class WPFConsole : ICmdRConsole
    {
        private readonly IWPFViewModel _viewmodel;

        public WPFConsole(IWPFViewModel viewModel)
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
