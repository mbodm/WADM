using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    public sealed class CurseParser : ICurseParser
    {
        public string ParseAddonNameFromString(string addonOverviewUrl)
        {
            addonOverviewUrl = addonOverviewUrl.ToLower().Trim();

            if (string.IsNullOrEmpty(addonOverviewUrl))
            {
                throw new ArgumentException("Parameter is null or empty.", "addonOverviewUrl");
            }

            if (!addonOverviewUrl.Contains("curse.com/addons/wow/"))
            {
                throw new CurseParserException("The url is not a valid curse addon url.");
            }

            var s = string.Empty;

            return addonOverviewUrl.Replace("http://", s).Replace("www.", s).Replace("curse.com/addons/wow", s).Replace("/", s).ToLower().Trim();
        }

        public string ParseAddonNumberFromString(string realDownloadUrl)
        {
            realDownloadUrl = realDownloadUrl.ToLower().Trim();

            if (string.IsNullOrEmpty(realDownloadUrl))
            {
                throw new ArgumentException("Parameter is null or empty.", "realDownloadUrl");
            }

            var parts = realDownloadUrl.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            int tmp;
            var numbers = parts.Where(s => int.TryParse(s.Trim(), out tmp));

            return numbers.First().Trim().PadLeft(3, '0') + numbers.Last().Trim().PadLeft(3, '0');
        }

        public string ParseAddonNumberFromSite(string addonOverviewUrl)
        {
            addonOverviewUrl = addonOverviewUrl.ToLower().Trim();

            if (string.IsNullOrEmpty(addonOverviewUrl))
            {
                throw new ArgumentException("Parameter is null or empty.", "addonOverviewUrl");
            }

            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(addonOverviewUrl);

                request.Proxy = null;
                request.Timeout = 5000;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var line = string.Empty;
                            var nothingFound = false;

                            while (string.IsNullOrEmpty(line) || !line.Contains("cta-button download-cc-large") || !line.Contains("-client"))
                            {
                                if (streamReader.EndOfStream)
                                {
                                    nothingFound = true;
                                    break;
                                }

                                line = streamReader.ReadLine();
                            }

                            streamReader.Close();
                            responseStream.Close();
                            response.Close();

                            if (nothingFound)
                            {
                                throw new CurseParserException();
                            }

                            var lineParts = line.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                            var clientPart = (from s in lineParts where s.Contains("-client") select s).FirstOrDefault();

                            var subParts = clientPart.Split(new string[] { "/", "-client" }, StringSplitOptions.RemoveEmptyEntries);

                            var number = subParts.Last();

                            return number.Trim();
                        }
                    }
                }
            }
            catch
            {
                throw new CurseParserException("Parse error.");
            }
        }

        public string ParseRealDownloadUrlFromSite(string addonDownloadUrl)
        {
            // After many tests, this is the fastest and best method, no matter if we use the forward or update version.
            // 1) The site www.curse.com use "chunked Transfer-Encoding" and so we safe time if we do not load all HTML.
            // 2) The HTML Agility Pack is really fast enough, but we have only a half HTML site, so we can not use him.
            // 3) We use HttpWebRequest instead of WebClient, cause we only need parts of the content instead all of it.

            addonDownloadUrl = addonDownloadUrl.ToLower().Trim();

            if (string.IsNullOrEmpty(addonDownloadUrl))
            {
                throw new ArgumentException("Parameter is null or empty.", "addonDownloadUrl");
            }

            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(addonDownloadUrl);

                request.Proxy = null;
                request.Timeout = 5000;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var line = string.Empty;
                            var nothingFound = false;

                            while (string.IsNullOrEmpty(line) || !line.Contains("data-href") || !line.Contains("http://addons.curse.cursecdn.com") || !line.Contains(".zip"))
                            {
                                if (streamReader.EndOfStream)
                                {
                                    nothingFound = true;
                                    break;
                                }

                                line = streamReader.ReadLine();
                            }

                            streamReader.Close();
                            responseStream.Close();
                            response.Close();

                            if (nothingFound)
                            {
                                throw new CurseParserException();
                            }

                            var lineParts = line.Split(new string[] { "\"" }, StringSplitOptions.None);

                            var result = (from s in lineParts where s.Contains(".zip") select s).FirstOrDefault();

                            return result.ToLower().Trim();
                        }
                    }
                }
            }
            catch
            {
                throw new CurseParserException("Parse error.");
            }
        }

        /*
        private string GetRealDownloadUrl(string addonName)
        {
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();

            var site = string.Empty;

            using (var webClient = new WebClient())
            {
                site = webClient.DownloadString("http://www.curse.com/addons/wow/" + addonName + "/download");
                
                if (string.IsNullOrEmpty(site))
                {
                    return null;
                }

                htmlDocument.LoadHtml(site);

                var q1 = from d in htmlDocument.DocumentNode.Descendants("div")
                         from a in d.Attributes
                         where a.Name == "class" && a.Value == "countdown"
                         select d;

                if (q1.Count() != 1) return string.Empty;

                var q2 = from d in q1.First().Descendants()
                         from a in d.Attributes
                         where a.Name == "data-href"
                         select a.Value;

                return q2.FirstOrDefault();
            }
        }
        */
    }
}
