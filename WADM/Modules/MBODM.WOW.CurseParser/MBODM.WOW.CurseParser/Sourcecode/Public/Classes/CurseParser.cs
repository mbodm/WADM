using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WOW
{
    public sealed class CurseParser : ICurseParser
    {
        private const string CurseAddonUrl = "mods.curse.com/addons/wow/";
        private const string CurseDownloadUrl = "addons.curse.cursecdn.com/files/";
        private const string ArgumentNullOrEmptyMessage = "Argument is null or empty.";
        private const string InvalidCurseAddonUrlMessage = "The url is not a valid curse addon url.";
        private const string InvalidCurseDownloadUrlMessage = "The url is not a valid curse download url.";

        public string ParseAddonNameFromString(string addonSiteUrl)
        {
            if (string.IsNullOrEmpty(addonSiteUrl))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, "addonSiteUrl");
            }

            addonSiteUrl = FormAndCheckAddonSiteUrl(addonSiteUrl);

            try
            {
                var name = addonSiteUrl.
                    Replace("http://", string.Empty).
                    Replace(CurseAddonUrl, string.Empty);

                if (string.IsNullOrEmpty(name))
                {
                    throw new CurseParserException();
                }

                return name;
            }
            catch
            {
                throw new CurseParserException(InvalidCurseAddonUrlMessage);
            }
        }

        public string ParseAddonFileFromString(string downloadFileUrl)
        {
            if (string.IsNullOrEmpty(downloadFileUrl))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, "downloadFileUrl");
            }

            downloadFileUrl = FormAndCheckDownloadFileUrl(downloadFileUrl);

            try
            {
                var file = downloadFileUrl.
                    Replace("http://", string.Empty).
                    Replace(CurseDownloadUrl, string.Empty).
                    Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).
                    LastOrDefault();

                if (string.IsNullOrEmpty(file) || !file.EndsWith(".zip"))
                {
                    throw new CurseParserException();
                }

                return file;
            }
            catch
            {
                throw new CurseParserException(InvalidCurseDownloadUrlMessage);
            }
        }

        public string ParseAddonNumberFromString(string downloadFileUrl)
        {
            if (string.IsNullOrEmpty(downloadFileUrl))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, "downloadFileUrl");
            }

            downloadFileUrl = FormAndCheckDownloadFileUrl(downloadFileUrl);

            try
            {
                var parts = downloadFileUrl.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                int tmp;
                var numbers = parts.Where(s => int.TryParse(s.Trim(), out tmp));

                return numbers.First().Trim().PadLeft(3, '0') + numbers.Last().Trim().PadLeft(3, '0');
            }
            catch
            {
                throw new CurseParserException(InvalidCurseDownloadUrlMessage);
            }
        }

        public async Task<string> ParseAddonNumberFromSiteAsync(string addonSiteUrl)
        {
            if (string.IsNullOrEmpty(addonSiteUrl))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, "addonSiteUrl");
            }

            addonSiteUrl = FormAndCheckAddonSiteUrl(addonSiteUrl);

            try
            {
                Predicate<string> predicate = (line) =>
                {
                    return
                    string.IsNullOrEmpty(line) ||
                    !line.Contains("cta-button download-cc-large") ||
                    !line.Contains("-client");
                };

                var parseResult = await ParseCurseAsync(addonSiteUrl, predicate).ConfigureAwait(false);

                var lineParts = parseResult.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                var clientPart = (from s in lineParts where s.Contains("-client") select s).FirstOrDefault();

                var subParts = clientPart.Split(new string[] { "/", "-client" }, StringSplitOptions.RemoveEmptyEntries);

                var number = subParts.Last();

                return number.Trim();
            }
            catch
            {
                throw new CurseParserException("Parse error.");
            }
        }

        public async Task<string> ParseDownloadFileUrlFromSiteAsync(string addonSiteUrl, string number)
        {
            // After many tests, this is the fastest and best method, no matter if we use the forward or update version.
            // 1) The site www.curse.com use "chunked Transfer-Encoding" and so we safe time if we do not load all HTML.
            // 2) The HTML Agility Pack is really fast enough, but we have only a half HTML site, so we can not use him.
            // 3) We use HttpWebRequest instead of WebClient, cause we only need parts of the content instead all of it.

            if (string.IsNullOrEmpty(addonSiteUrl))
            {
                throw new ArgumentException(ArgumentNullOrEmptyMessage, "addonSiteUrl");
            }

            addonSiteUrl = FormAndCheckAddonSiteUrl(addonSiteUrl);

            try
            {
                var downloadSiteUrl = string.Empty;

                if (!string.IsNullOrEmpty(number))
                {
                    // Faster direct version
                    downloadSiteUrl = addonSiteUrl + "/" + number;
                }
                else
                {
                    // Slower forwarding version
                    downloadSiteUrl = addonSiteUrl + "/" + "download";
                }

                Predicate<string> predicate = (line) =>
                {
                    return
                    string.IsNullOrEmpty(line) ||
                    !line.Contains("data-href") ||
                    !line.Contains(CurseDownloadUrl) ||
                    !line.Contains(".zip");
                };

                var parseResult = await ParseCurseAsync(downloadSiteUrl, predicate).ConfigureAwait(false);

                var parts = parseResult.Split(new string[] { "\"" }, StringSplitOptions.None);

                var url = (from s in parts where s.Contains(".zip") select s).FirstOrDefault();

                return url.ToLower().Trim();
            }
            catch
            {
                throw new CurseParserException("Parse error.");
            }
        }

        public Task<string> ParseDownloadFileUrlFromSiteAsync(string addonSiteUrl)
        {
            return ParseDownloadFileUrlFromSiteAsync(addonSiteUrl, null);
        }

        private string FormAndCheckAddonSiteUrl(string addonSiteUrl)
        {
            addonSiteUrl = addonSiteUrl.ToLower().Trim();

            if (!addonSiteUrl.Contains(CurseAddonUrl))
            {
                throw new CurseParserException(InvalidCurseAddonUrlMessage);
            }

            if (addonSiteUrl.Last() == '/')
            {
                addonSiteUrl = addonSiteUrl.TrimEnd('/');
            }

            return addonSiteUrl;
        }

        private string FormAndCheckDownloadFileUrl(string downloadFileUrl)
        {
            downloadFileUrl = downloadFileUrl.ToLower().Trim();

            if (!downloadFileUrl.Contains(CurseDownloadUrl))
            {
                throw new CurseParserException(InvalidCurseDownloadUrlMessage);
            }

            return downloadFileUrl;
        }

        private async Task<string> ParseCurseAsync(string url, Predicate<string> predicate)
        {
            url = url.ToLower().Trim();

            var request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Proxy = null;
            request.Timeout = 5000;

            using (var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var line = string.Empty;
                        var nothingFound = false;

                        while (predicate(line))
                        {
                            if (streamReader.EndOfStream)
                            {
                                nothingFound = true;
                                break;
                            }

                            line = await streamReader.ReadLineAsync().ConfigureAwait(false);
                        }

                        streamReader.Close();
                        responseStream.Close();
                        response.Close();

                        if (nothingFound)
                        {
                            throw new CurseParserException();
                        }

                        return line;
                    }
                }
            }
        }
    }
}
