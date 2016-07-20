using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WOW
{
    public interface ICurseParser
    {
        string ParseAddonNameFromString(string addonSiteUrl);
        string ParseAddonFileFromString(string downloadFileUrl);
        string ParseAddonNumberFromString(string downloadFileUrl);
        Task<string> ParseAddonNumberFromSiteAsync(string addonSiteUrl);
        Task<string> ParseDownloadFileUrlFromSiteAsync(string addonSiteUrl);
        Task<string> ParseDownloadFileUrlFromSiteAsync(string addonSiteUrl, string number);
    }
}
