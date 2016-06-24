using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    public sealed class AddonProgressData
    {
        public AddonProgressData()
            : this(AddonProgressStatus.Ready, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0, false, null)
        {
        }

        public AddonProgressData(string addonUrl, string downloadUrl, string downloadFolder, bool downloadAddon, object customTag)
            : this(AddonProgressStatus.Ready, string.Empty, addonUrl, downloadUrl, downloadFolder, string.Empty, 0, 0, 0, downloadAddon, customTag)
        {
        }

        public AddonProgressData(AddonProgressStatus status, string addonName, string addonUrl, string downloadUrl, string downloadFolder, string downloadFile, byte downloadPercentage, long receivedBytes, long totalBytes, bool downloadAddon, object customTag)
        {
            Status = status;
            AddonName = addonName;
            AddonUrl = addonUrl;
            DownloadUrl = downloadUrl;
            DownloadFolder = downloadFolder;
            DownloadFile = downloadFile;
            DownloadPercentage = downloadPercentage;
            ReceivedBytes = receivedBytes;
            TotalBytes = totalBytes;
            DownloadAddon = downloadAddon;
            CustomTag = customTag;
        }

        public AddonProgressStatus Status
        {
            get;
            set;
        }

        public string AddonName
        {
            get;
            set;
        }

        public string AddonUrl
        {
            get;
            set;
        }

        public string DownloadUrl
        {
            get;
            set;
        }

        public string DownloadFolder
        {
            get;
            set;
        }

        public string DownloadFile
        {
            get;
            set;
        }

        public byte DownloadPercentage
        {
            get;
            set;
        }

        public long ReceivedBytes
        {
            get;
            set;
        }

        public long TotalBytes
        {
            get;
            set;
        }

        public bool DownloadAddon
        {
            get;
            set;
        }

        public object CustomTag
        {
            get;
            set;
        }
    }
}
