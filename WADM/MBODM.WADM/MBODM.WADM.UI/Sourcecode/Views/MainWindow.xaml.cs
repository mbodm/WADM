using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
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
using MBODM.WPF;

namespace MBODM.WADM.UI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BusinessLogic businessLogic;
        private readonly int oldMinWorkerThreadsCount;
        private readonly int oldCompletionPortThreadsCount;

        private bool sortAscending = true;
        private string lastClickedColumn = "AddonName";

        public MainWindow()
        {
            InitializeComponent();

            ThreadPool.GetMinThreads(out oldMinWorkerThreadsCount, out oldCompletionPortThreadsCount);
            ThreadPool.SetMinThreads(24, 24);

            // That also could be injected with an IoC container

            businessLogic = new BusinessLogic();

            businessLogic.LoadConfig();

            var viewModel = new MainFormViewModel(businessLogic);

            DataContext = viewModel;

            // That also could be a dialog service

            var standardDialogs = new StandardDialogs();

            viewModel.ErrorDialog += (errorText) =>
            {
                standardDialogs.ShowErrorMessage(errorText);
            };

            viewModel.SearchDialog += (initialFolder) =>
            {
                return standardDialogs.ShowFolderBrowserDialog(initialFolder);
            };

            this.Closing += (s, e) =>
            {
                businessLogic.SaveConfig();

                ThreadPool.SetMinThreads(oldMinWorkerThreadsCount, oldCompletionPortThreadsCount);
            };

            listView.TargetUpdated += (s, e) =>
            {
                // Resize columns of ListView on update

                var gridView = listView.View as GridView;

                if (gridView != null)
                {
                    foreach (var column in gridView.Columns)
                    {
                        if (double.IsNaN(column.Width))
                        {
                            column.Width = column.ActualWidth;
                        }

                        column.Width = double.NaN;
                    }
                }
            };

            (listView.Items as INotifyCollectionChanged).CollectionChanged += (s, e) =>
            {
                // Select items of ListView after delete

                if (listView.Items.Count == 0) return;

                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    if (e.OldStartingIndex >= listView.Items.Count)
                    {
                        listView.SelectedIndex = e.OldStartingIndex - 1;
                    }
                    else
                    {
                        listView.SelectedIndex = e.OldStartingIndex;
                    }
                }
            };

            listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler((s, e) =>
            {
                var columnHeader = e.OriginalSource as GridViewColumnHeader;

                if (columnHeader == null) return;
                if (columnHeader.Tag == null) return;

                var clickedColumn = columnHeader.Tag.ToString();

                sortAscending = !sortAscending;

                if ((clickedColumn != lastClickedColumn) &&
                    ((clickedColumn == "AddonName") && (lastClickedColumn != "AddonUrl")) &&
                    ((clickedColumn == "AddonUrl") && (lastClickedColumn != "AddonName")))
                {
                    sortAscending = true;
                }

                lastClickedColumn = clickedColumn;

                switch (clickedColumn)
                {
                    case "Active":
                        businessLogic.SortAddons(a => a.Active, sortAscending);
                        break;
                    case "AddonName":
                        businessLogic.SortAddons(a => a.AddonName, sortAscending);
                        break;
                    case "AddonUrl":
                        businessLogic.SortAddons(a => a.AddonUrl, sortAscending);
                        break;
                    case "StatusText":
                        businessLogic.SortAddons(a => a.ReceivedData, sortAscending);
                        break;
                    case "FileName":
                        businessLogic.SortAddons(a => a.FileName, sortAscending);
                        break;
                    case "DownloadUrl":
                        businessLogic.SortAddons(a => a.DownloadUrl, sortAscending);
                        break;
                    default:
                        break;
                }
            }));
        }
    }
}
