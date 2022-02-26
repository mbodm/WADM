using System;
using System.Windows.Input;

namespace MBODM.WADM.UI
{
    public interface IAddWindowViewModel
    {
        ObservableLookupTable<string, string> TextData
        {
            get;
        }

        string URL
        {
            get;
            set;
        }

        bool Result
        {
            get;
        }

        ICommand OK
        {
            get;
        }

        ICommand Cancel
        {
            get;
        }

        Action<string> ErrorDialog
        {
            get;
            set;
        }

        event Action<object, bool> CloseRequested;
    }
}
