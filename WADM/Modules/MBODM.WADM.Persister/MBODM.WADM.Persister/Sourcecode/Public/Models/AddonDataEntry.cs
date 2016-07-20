using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WADM
{
    public sealed class AddonDataEntry
    {
        public AddonDataEntry()
            : this(string.Empty, string.Empty, true)
        {
        }

        public AddonDataEntry(string addonUrl, string lastDownloadUrl, bool isActive)
        {
            AddonUrl = addonUrl;
            LastDownloadUrl = lastDownloadUrl;
            IsActive = isActive;
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

        public bool IsActive
        {
            get;
            set;
        }
    }
}
