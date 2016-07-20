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
using System.Windows.Shapes;
using MBODM.WPF;

namespace MBODM.WADM.UI
{
    /// <summary>
    /// Interaktionslogik für AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow(IAddWindowViewModel viewModel)
        {
            InitializeComponent();

            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            DataContext = viewModel;

            // That also could be a dialog service

            var standardDialogs = new StandardDialogs();

            viewModel.ErrorDialog += (errorMessage) =>
            {
                standardDialogs.ShowErrorMessage(errorMessage);
            };

            viewModel.CloseRequested += (sender, viewModelResult) =>
            {
                Close();
            };

            this.Loaded += (sender, eventArgs) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };

            this.KeyDown += (sender, eventArgs) =>
            {
                if (eventArgs.Key == Key.Enter) viewModel.OK.Execute(null);
                if (eventArgs.Key == Key.Escape) viewModel.Cancel.Execute(null);
            };
        }
    }
}
