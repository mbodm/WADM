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
            var url = $"{CurseApiUrl}/addon/{addonData.Id}/file/{addonData.DefaultFileId}/download-url";

            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var downloadUrl = content.Trim();

                return downloadUrl;
            }
        }

        private async Task<AddonDataFromApi> GetAddonDataFromApi(string addonName, CancellationToken cancellationToken)
        {
            // I could get the gameId for World of Warcraft first, also by using the api. But
            // i assume that gameId will not change for a good while (at least longer than it
            // takes, before Curse is sold again). So i decided it is safe to stick with that
            // fixed gameId here. Small benefit: App is a bit faster, without another request.

            var url = $"{CurseApiUrl}/addon/search?gameId=1";

            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url, cancellationToken))
            {
                // It is totally valid here to throw an exception. I also let JSON.NET fail
                // gracefully with an exception, if something goes wrong. For this reason i
                // do no JSON format checks here. Hide exceptions is bad practice anyway. I
                // also have to rely on some parser error handling in a higher layer anyway.

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                content = $"{{\"addons\": {content.Trim()}}}"; // JSON.NET can not parse an unnamed array.

                var root = JObject.Parse(content);
                var addons = (JArray)root["addons"];

                // Since the "name" property is not always the dedicated name and can contain
                // some other text (see Coordinates addon as example), we have to compare the
                // website url. This can not go wrong and guarantees a secure addon detection.

                var siteUrl = $"https://www.curseforge.com/wow/addons/{addonName}";

                var addonData = addons.
                    Where(addon => addon["websiteUrl"].ToString().Trim().ToLower() == siteUrl.Trim().ToLower()).
                    Select(addon => new AddonDataFromApi()
                    {
                        Id = addon["id"].ToString(),
                        Name = addon["name"].ToString(),
                        WebsiteUrl = addon["websiteUrl"].ToString(),
                        DefaultFileId = addon["defaultFileId"].ToString(),
                    }).
                    First();

                return addonData;
            }
        }
    }
}
