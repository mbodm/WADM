using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
