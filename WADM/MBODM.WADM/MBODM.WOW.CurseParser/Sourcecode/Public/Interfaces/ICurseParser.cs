using System.Threading;
using System.Threading.Tasks;

namespace MBODM.WOW
{
    public interface ICurseParser
    {
        string GetAddonName(string addonUrl);
        Task<string> GetDownloadUrlAsync(string addonName);
        Task<string> GetDownloadUrlAsync(string addonName, CancellationToken cancellationToken);
    }
}
