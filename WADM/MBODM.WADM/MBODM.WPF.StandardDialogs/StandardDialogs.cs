using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MBODM.WPF
{
    public sealed class StandardDialogs : IStandardDialogs
    {
        public IEnumerable<string> ShowOpenFileDialog(string folder, string filter, bool multiselect)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();

            ofd.Multiselect = multiselect;
            ofd.InitialDirectory = folder;
            ofd.Filter = filter + "|Alle Dateien (*.*)|*.*";

            if (ofd.ShowDialog().Value == true) return ofd.FileNames;

            return Enumerable.Empty<string>();
        }

        public string ShowSaveFileDialog(string folder, string filter)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog();

            sfd.InitialDirectory = folder;
            sfd.Filter = filter + "|Alle Dateien (*.*)|*.*";

            if (sfd.ShowDialog().Value == true) return sfd.FileName;

            return string.Empty;
        }

        public string ShowFolderBrowserDialog(string folder)
        {
            var cofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();

            cofd.Multiselect = false;
            cofd.IsFolderPicker = true;

            cofd.InitialDirectory = folder;
            cofd.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            cofd.EnsureFileExists = true;
            cofd.EnsurePathExists = true;
            cofd.AllowNonFileSystemItems = false;

            if (cofd.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                return cofd.FileName;
            }

            return string.Empty;
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
