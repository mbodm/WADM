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
        private const string CurseAddonUrl = "https://www.curseforge.com/wow/addons/";
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
                    var url1 = CurseAddonUrl + addonName + "/download";

                    using (var request1 = new HttpRequestMessage(HttpMethod.Get, url1))
                    {
                        var response1 = await httpClient.SendAsync(request1, cancellationToken).ConfigureAwait(false);

                        var content = await response1.Content.ReadAsStringAsync().ConfigureAwait(false);

                        var url2 = content.
                            Split(new string[] { "<p class=\"text-sm\">" }, StringSplitOptions.RemoveEmptyEntries).
                            Last().
                            Split(new string[] { ">here</a>" }, StringSplitOptions.RemoveEmptyEntries).
                            First().
                            Split(new string[] { "<a href=" }, StringSplitOptions.RemoveEmptyEntries).
                            Last().
                            Trim().
                            Trim('"').
                            Replace("/wow/addons/", CurseAddonUrl);

                        using (var request2 = new HttpRequestMessage(HttpMethod.Get, url2))
                        {
                            var response2 = await httpClient.SendAsync(request2, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                            var downloadUrl = response2.Headers.Location.ToString();

                            return downloadUrl;
                        }
                    }
                }
            }
        }
    }
}
