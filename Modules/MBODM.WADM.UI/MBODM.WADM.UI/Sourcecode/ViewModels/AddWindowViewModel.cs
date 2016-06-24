using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MBODM.WPF;

namespace MBODM.WADM.UI
{
    public sealed class AddWindowViewModel : ObservableModel, IAddWindowViewModel
    {
        public AddWindowViewModel()
        {
            TextData = new ObservableLookupTable<string, string>((new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Title", "Add"),
                new KeyValuePair<string, string>("Url", "URL:"),
                new KeyValuePair<string, string>("Ok", "OK"),
                new KeyValuePair<string, string>("Cancel", "Cancel"),
            }));
        }

        public ObservableLookupTable<string, string> TextData
        {
            get;
            private set;
        }

        private string url = "Enter addon url from curse.com";
        public string URL
        {
            get { return url; }
            set { SetProperty<string>(ref url, value); }
        }

        public bool Result
        {
            get;
            private set;
        }

        public ICommand OK
        {
            get { return new RelayCommand((p) => Close(true), null); }
        }

        public ICommand Cancel
        {
            get { return new RelayCommand((p) => Close(false), null); }
        }

        public Action<string> ErrorDialog
        {
            get;
            set;
        }

        public event Action<object, bool> CloseRequested;

        private void Close(bool isOkCommand)
        {
            if (isOkCommand)
            {
                if (string.IsNullOrEmpty(url))
                {
                    if (ErrorDialog != null) ErrorDialog("There must be a download url.");
                    return;
                }
            }

            Result = isOkCommand;

            var handler = CloseRequested;
            if (handler != null) handler(this, Result);
        }
    }
}
