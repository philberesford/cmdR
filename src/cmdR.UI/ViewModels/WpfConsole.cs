using System;
using cmdR.IO;

namespace cmdR.UI.ViewModels
{
    public class WpfConsole : ICmdRConsole
    {
        private readonly IWpfViewModel _viewmodel;

        public WpfConsole(IWpfViewModel viewModel)
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