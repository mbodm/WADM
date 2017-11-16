using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    public sealed class AddonData
    {
        public AddonData()
            : this(string.Empty, string.Empty, string.Empty, null)
        {
        }

        public AddonData(string addonUrl, string downloadFolder)
            : this(addonUrl, string.Empty, downloadFolder, null)
        {
        }

        public AddonData(string addonUrl, string lastDownloadUrl, string downloadFolder, object customTag)
        {
            AddonUrl = addonUrl;
            LastDownloadUrl = lastDownloadUrl;
            DownloadFolder = downloadFolder;
            CustomTag = customTag;
        }

        public string AddonUrl
        {
            get;
            set;
        }

        public string LastDownloadUrl
        {
            get;
            set;
        }

        public string DownloadFolder
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
