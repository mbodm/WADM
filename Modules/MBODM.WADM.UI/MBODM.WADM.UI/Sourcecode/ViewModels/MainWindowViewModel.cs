using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MBODM.WPF;

namespace MBODM.WADM.UI
{
    public sealed class MainFormViewModel : ObservableModel, IMainWindowViewModel
    {
        private BusinessLogic businessLogic;

        public MainFormViewModel(BusinessLogic businessLogic)
        {
            if (businessLogic == null)
            {
                throw new ArgumentNullException("businessLogic");
            }

            this.businessLogic = businessLogic;

            TextData = new ObservableLookupTable<string, string>((new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Title", businessLogic.ProgramName),
                new KeyValuePair<string, string>("Folder", "Download-Folder:"),
                new KeyValuePair<string, string>("Search", "Search..."),
                new KeyValuePair<string, string>("Config", "show config file"),
                new KeyValuePair<string, string>("Status", ""),
                new KeyValuePair<string, string>("Download", "Download"),
                new KeyValuePair<string, string>("Add", "Add"),
                new KeyValuePair<string, string>("Del", "Del"),
            }));
        }

        public ObservableLookupTable<string, string> TextData
        {
            get;
            private set;
        }

        public string DownloadFolder
        {
            get { return businessLogic.DownloadFolder; }
            set { if (businessLogic.DownloadFolder != value) { businessLogic.DownloadFolder = value; OnPropertyChanged(); } }
        }

        private bool isDownloading;
        public bool IsDownloading
        {
            get { return isDownloading; }
            set { SetProperty<bool>(ref isDownloading, value); }
        }

        private bool errorsOccurred;
        public bool ErrorsOccurred
        {
            get { return errorsOccurred; }
            set { SetProperty<bool>(ref errorsOccurred, value); }
        }

        public ObservableCollection<AddonData> Addons
        {
            get { return businessLogic.Addons; }
        }

        public AddonData SelectedAddon
        {
            get;
            set;
        }

        public ICommand Search
        {
            get
            {
                return new RelayCommand((p) =>
                {
                    if (SearchDialog != null)
                    {
                        var initialFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        if (!string.IsNullOrEmpty(DownloadFolder) && Directory.Exists(DownloadFolder)) initialFolder = DownloadFolder;

                        var dialogResult = SearchDialog(initialFolder);

                        if (!string.IsNullOrEmpty(dialogResult)) DownloadFolder = dialogResult;
                    }
                },
                null);
            }
        }

        public ICommand Config
        {
            get
            {
                return new RelayCommand((p) => businessLogic.ShowConfig(), null);
            }
        }

        public ICommand Add
        {
            get
            {
                return new RelayCommand((p) =>
                {
                    try
                    {
                        // That also could be done by a dialog service or a view manager

                        var addWindowViewModel = new AddWindowViewModel();
                        (new AddWindow(addWindowViewModel)).ShowDialog();
                        if (addWindowViewModel.Result) businessLogic.AddAddon(addWindowViewModel.URL);
                    }
                    catch (Exception exception)
                    {
                        if (ErrorDialog != null) ErrorDialog(exception.Message);
                    }
                },
                null);
            }
        }

        public ICommand Del
        {
            get
            {
                return new RelayCommand((p) =>
                {
                    if (SelectedAddon != null) businessLogic.DeleteAddon(SelectedAddon.AddonName);
                },
                null);
            }
        }

        public ICommand Download
        {
            get
            {
                return new RelayCommand((p) =>
                {
                    if (!businessLogic.Addons.Any())
                    {
                        return;
                    }

                    foreach (var a in businessLogic.Addons)
                    {
                        a.FileName = string.Empty;
                        a.DownloadPercentage = 0;
                        a.ReceivedData = 0;
                        a.TotalData = 0;
                        a.StatusText = string.Empty;
                        a.Error = false;
                    }

                    IsDownloading = true;
                    ErrorsOccurred = false;
                    TextData["Status"] = string.Empty;

                    var dsHandler = DownloadStarted;
                    if (dsHandler != null) dsHandler();

                    businessLogic.DownloadFinished += (wasCancelled) =>
                    {
                        IsDownloading = false;
                        TextData["Status"] = "All downloads finished";

                        var errorAddons = from a in Addons where a.Error select a;
                        if (errorAddons.Any())
                        {
                            ErrorsOccurred = true;
                            TextData["Status"] = "Errors occurred";

                            // We need this workaround to make also the error progressbars visible

                            foreach (var a in errorAddons) a.DownloadPercentage = 100;
                        }

                        if (wasCancelled) TextData["Status"] = "Cancelled";

                        var dfHandler = DownloadFinished;
                        if (dfHandler != null) dfHandler();
                    };

                    try
                    {
                        businessLogic.Download();
                    }
                    catch (Exception exception)
                    {
                        IsDownloading = false;
                        ErrorsOccurred = true;
                        TextData["Status"] = "Errors occurred";

                        if (ErrorDialog != null) ErrorDialog(exception.Message);

                        var dfHandler = DownloadFinished;
                        if (dfHandler != null) dfHandler();
                    }
                },
                null);
            }
        }

        public ICommand Copy
        {
            get
            {
                return new RelayCommand((p) =>
                {
                    if (SelectedAddon != null) Clipboard.SetData(DataFormats.Text, SelectedAddon.AddonUrl);
                },
                null);
            }
        }

        public Func<string, string> SearchDialog
        {
            get;
            set;
        }

        public Action<string> ErrorDialog
        {
            get;
            set;
        }

        public event Action DownloadStarted;
        public event Action DownloadFinished;
    }
}
