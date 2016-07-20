using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WADM.UI
{
    public sealed class AddonData : ObservableModel
    {
        public AddonData()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, false)
        {
        }

        public AddonData(string addonUrl, string addonName, string fileName, string downloadUrl, int downloadPercentage, int receivedData, int totalData, string statusText, bool active, bool error)
        {
            this.addonUrl = addonUrl;
            this.addonName = addonName;
            this.fileName = fileName;
            this.downloadUrl = downloadUrl;
            this.downloadPercentage = downloadPercentage;
            this.receivedData = receivedData;
            this.totalData = totalData;
            this.statusText = statusText;
            this.active = active;
            this.Error = error;
        }

        private string addonUrl;
        [DisplayName("  Addon-URL  ")]
        public string AddonUrl
        {
            get { return addonUrl; }
            set { SetProperty<string>(ref addonUrl, value); }
        }

        private string addonName;
        [DisplayName("  Addon-Name  ")]
        public string AddonName
        {
            get { return addonName; }
            set { SetProperty<string>(ref addonName, value); }
        }

        private string addonLocation;
        [DisplayName("  Addon-Location  ")]
        public string AddonLocation
        {
            get { return addonLocation; }
            set { SetProperty<string>(ref addonLocation, value); }
        }

        private string fileName;
        [DisplayName("  Filename  ")]
        public string FileName
        {
            get { return fileName; }
            set { SetProperty<string>(ref fileName, value); }
        }

        private string downloadUrl;
        [DisplayName("  Download-URL  ")]
        public string DownloadUrl
        {
            get { return downloadUrl; }
            set { SetProperty<string>(ref downloadUrl, value); }
        }

        private int downloadPercentage;
        public int DownloadPercentage
        {
            get { return downloadPercentage; }
            set { SetProperty<int>(ref downloadPercentage, value); }
        }

        private long receivedData;
        public long ReceivedData
        {
            get { return receivedData; }
            set { SetProperty<long>(ref receivedData, value); }
        }

        private long totalData;
        public long TotalData
        {
            get { return totalData; }
            set { SetProperty<long>(ref totalData, value); }
        }

        private string statusText;
        [DisplayName("                    Status                    ")]
        public string StatusText
        {
            get { return statusText; }
            set { SetProperty<string>(ref statusText, value); }
        }

        private bool active;
        [DisplayName("  Active  ")]
        public bool Active
        {
            get { return active; }
            set { SetProperty<bool>(ref active, value); }
        }

        private bool error;
        public bool Error
        {
            get { return error; }
            set { SetProperty<bool>(ref error, value); }
        }
    }
}
