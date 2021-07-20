using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MBODM.WoW
{
    public sealed class CurseParserUsingApi : ICurseParser
    {
        private const string CurseApiUrl = "https://addons-ecs.forgesvc.net/api/v2";
        private const string ArgumentNullOrEmptyMessage = "Argument is null or empty.";

        public string GetAddonName(string addonUrl)
        {
            var orgCurseParser = new CurseParser();

            return orgCurseParser.GetAddonName(addonUrl);
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
            var addonData = await GetAddonDataFromApi(addonName, cancellationToken);

            return addonData.DownloadUrl;
        }

        private async Task<AddonDataFromApi> GetAddonDataFromApi(string addonName, CancellationToken cancellationToken)
        {
            // I could get the gameId for World of Warcraft first, also by using the api. But
            // i assume that gameId will not change for a good while (at least longer than it
            // takes, before Curse is sold again). So i decided it is safe to stick with that
            // fixed gameId here. Small benefit: App is a bit faster, without another request.

            var url = $"{CurseApiUrl}/addon/search?gameId=1&searchFilter={addonName}";

            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url, cancellationToken))
            {
                // It is totally valid here to throw an exception. I also let JSON.NET fail
                // gracefully with an exception, if something goes wrong. For this reason i
                // do no JSON format checks here. Hide exceptions is bad practice anyway. I
                // also have to rely on some parser error handling in a higher layer anyway.

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // BugFix 01 July 2021:
                // There are some addons containing an underscore in the addon name. In example
                // the "quest_completist" addon. It seems the Curse API is unable to find these
                // addons, when using such an addon name as "searchFilter" query in the url. In
                // that case we try to find the addon, by repeating the Curse API requests, for
                // every non-underscore part of the addon name. So, either we find the addon or
                // fail gracefully with an exception, as we did before implementing this bugfix.

                if (content == "[]" && addonName.Contains('_'))
                {
                    content = await UnderscoreInAddonNameBugFix(addonName, cancellationToken);
                }

                content = $"{{\"addons\": {content.Trim()}}}"; // JSON.NET can not parse an unnamed array.

                var root = JObject.Parse(content);
                var addons = (JArray)root["addons"];

                // Since the "name" property is not always the dedicated name and can contain
                // some other text (see Coordinates addon as example), we have to compare the
                // website url. This can not go wrong and guarantees a secure addon detection.

                var siteUrl = $"https://www.curseforge.com/wow/addons/{addonName}";

                var addon = addons.Where(addonToken => addonToken["websiteUrl"].ToString().Trim().ToLower() == siteUrl.Trim().ToLower()).FirstOrDefault();

                if (addon == null)
                {
                    throw new InvalidOperationException("Could not found addon, using Curse API.");
                }

                var downloadUrl = GetRetailDownloadUrl(addon);

                var addonData = new AddonDataFromApi()
                {
                    Id = addon["id"].ToString(),
                    Name = addon["name"].ToString(),
                    WebsiteUrl = addon["websiteUrl"].ToString(),
                    DownloadUrl = downloadUrl,
                };

                return addonData;
            }
        }

        private async Task<string> UnderscoreInAddonNameBugFix(string addonName, CancellationToken cancellationToken)
        {
            var addonNameParts = addonName.Split('_');

            foreach (var addonNamePart in addonNameParts)
            {
                if (!string.IsNullOrWhiteSpace(addonNamePart))
                {
                    var url = $"{CurseApiUrl}/addon/search?gameId=1&searchFilter={addonNamePart}";

                    using (var httpClient = new HttpClient())
                    using (var response = await httpClient.GetAsync(url, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();

                        var content = await response.Content.ReadAsStringAsync();

                        if (content != "[]")
                        {
                            return content;
                        }
                    }
                }
            }

            return "[]";
        }

        private string GetRetailDownloadUrl(JToken addon)
        {
            var latestFiles = addon["latestFiles"];

            if (latestFiles != null && latestFiles.Type == JTokenType.Array)
            {
                foreach (var latestFile in latestFiles)
                {
                    // gameVersionFlavor -> Make sure it is WoW Retail. Not WoW Classic. Not Burning Crusade Classic.
                    // releaseType -> Make sure it is a Release version. Not Alpha. Not Beta. Not something else.
                    // downloadUrl -> Make sure it is a non-empty/valid download url. Not something else.

                    var gameVersionFlavorValue = latestFile["gameVersionFlavor"]?.ToString()?.Trim() ?? string.Empty;
                    var releaseTypeValue = latestFile["releaseType"]?.ToString()?.Trim() ?? string.Empty;
                    var downloadUrlValue = latestFile["downloadUrl"]?.ToString()?.Trim() ?? string.Empty;

                    if (gameVersionFlavorValue == "wow_retail" && releaseTypeValue == "1" && Uri.TryCreate(downloadUrlValue, UriKind.Absolute, out _))
                    {
                        return downloadUrlValue;
                    }
                }
            }

            throw new InvalidOperationException("Could not found retail-download-url of addon, using Curse API.");
        }
    }
}
