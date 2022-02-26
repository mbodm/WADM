using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MBODM.WADM.UI
{
    public interface IMainWindowViewModel
    {
        ObservableLookupTable<string, string> TextData
        {
            get;
        }

        string DownloadFolder
        {
            get;
            set;
        }

        bool IsDownloading
        {
            get;
            set;
        }

        bool ErrorsOccurred
        {
            get;
            set;
        }

        ObservableCollection<AddonData> Addons
        {
            get;
        }

        AddonData SelectedAddon
        {
            get;
            set;
        }

        ICommand Search
        {
            get;
        }

        ICommand Config
        {
            get;
        }

        ICommand Add
        {
            get;
        }

        ICommand Del
        {
            get;
        }

        ICommand Download
        {
            get;
        }

        ICommand Copy
        {
            get;
        }

        Func<string, string> SearchDialog
        {
            get;
            set;
        }

        Action<string> ErrorDialog
        {
            get;
            set;
        }

        event Action DownloadStarted;
        event Action DownloadFinished;
    }
}
