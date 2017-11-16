using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
