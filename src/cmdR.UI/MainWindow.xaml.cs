using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using cmdR.UI.ViewModels;

namespace cmdR.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _model;

        public MainWindow()
        {
            InitializeComponent();

            _model = new MainWindowViewModel(Dispatcher);
            DataContext = _model;

            var timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 2)};
            timer.Tick += ((sender, e) =>
                {
                    if (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight)
                        _scrollViewer.ScrollToEnd();
                });
            timer.Start();
        }

        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // the binding to the ViewModel only updates when focus is lost. this bit of code forces the binding to be updated
                object focusObj = FocusManager.GetFocusedElement(this);
                if (focusObj != null && focusObj is TextBox)
                {
                    var binding = (focusObj as TextBox).GetBindingExpression(TextBox.TextProperty);
                    binding.UpdateSource();
                }

                _model.HandleReturnKeyPressed();
            }
        }
    }
}
