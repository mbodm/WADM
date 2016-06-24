using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    public interface ICurseParser
    {
        string ParseAddonNameFromString(string addonOverviewUrl);
        string ParseAddonNumberFromString(string realDownloadUrl);
        string ParseAddonNumberFromSite(string addonOverviewUrl);
        string ParseRealDownloadUrlFromSite(string addonDownloadUrl);
    }
}
