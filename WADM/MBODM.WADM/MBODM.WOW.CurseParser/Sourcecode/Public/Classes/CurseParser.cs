using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MBODM.WOW
{
    public sealed class CurseParser : ICurseParser
    {
        private const string CurseAddonUrl = "https://wow.curseforge.com/projects/";
        private const string ArgumentNullOrEmptyMessage = "Argument is null or empty.";
        private const string InvalidCurseAddonUrlMessage = "The url is not a valid curse addon url.";

        public string GetAddonName(string addonUrl)
        {
            if (string.IsNullOrEmpty(addonUrl))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, nameof(addonUrl));
            }

            try
            {
                addonUrl = addonUrl.ToLower().Trim();

                if (!addonUrl.StartsWith(CurseAddonUrl))
                {
                    throw new CurseParserException();
                }

                if (addonUrl.Last() == '/')
                {
                    addonUrl = addonUrl.TrimEnd('/');
                }

                var addonName = addonUrl.Replace(CurseAddonUrl, string.Empty);

                if (string.IsNullOrEmpty(addonName) || new Regex(@"^[a-z0-9_-]*$").IsMatch(addonName) == false)
                {
                    throw new CurseParserException();
                }

                return addonName;
            }
            catch
            {
                throw new CurseParserException(InvalidCurseAddonUrlMessage);
            }
        }

        public Task<string> GetDownloadUrlAsync(string addonName)
        {
            if (string.IsNullOrEmpty(addonName))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, nameof(addonName));
            }

            return GetDownloadUrlAsync(addonName, CancellationToken.None);
        }

        public async Task<string> GetDownloadUrlAsync(string addonName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(addonName))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, nameof(addonName));
            }

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.AllowAutoRedirect = false;
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.Credentials = null;
                httpClientHandler.PreAuthenticate = false;
                httpClientHandler.UseCookies = false;
                httpClientHandler.UseDefaultCredentials = false;
                httpClientHandler.UseProxy = false;

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var url = CurseAddonUrl + addonName + "/files/latest";

                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                        var downloadUrl = response.Headers.Location.ToString();

                        return downloadUrl;
                    }
                }
            }
        }
    }
}
