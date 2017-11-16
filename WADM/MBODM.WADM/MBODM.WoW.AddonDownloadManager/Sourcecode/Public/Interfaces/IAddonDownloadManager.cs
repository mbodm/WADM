using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    public interface IAddonDownloadManager
    {
        bool DownloadsRunning { get; }

        event Action<AddonProgressData> DownloadProgress;
        event Action<bool> DownloadsFinished;

        void DownloadAddonsAsync(IEnumerable<AddonData> addonData, bool unzipAddons, bool updateAlways, bool parallelDownload);
        void CancelDownloads();
    }
}
