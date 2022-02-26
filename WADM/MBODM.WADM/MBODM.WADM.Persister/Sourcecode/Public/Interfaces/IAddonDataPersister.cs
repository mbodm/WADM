using System.Collections.Generic;

namespace MBODM.WADM
{
    interface IAddonDataPersister
    {
        string DownloadFolder
        {
            get;
            set;
        }

        IList<AddonDataEntry> AddonDataEntries
        {
            get;
            set;
        }

        bool Save(string file);
        bool Load(string file);
    }
}
