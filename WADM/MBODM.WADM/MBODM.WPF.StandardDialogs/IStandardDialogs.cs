using System.Collections.Generic;

namespace MBODM.WPF
{
    public interface IStandardDialogs
    {
        IEnumerable<string> ShowOpenFileDialog(string folder, string filter, bool multiselect);
        string ShowSaveFileDialog(string folder, string filter);
        string ShowFolderBrowserDialog(string folder);
        void ShowErrorMessage(string message);
        void ShowInfoMessage(string message);
    }
}
