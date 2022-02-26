using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    public sealed class AddonDownloadManager : IAddonDownloadManager
    {
        // Curse.com provide 2 possible ways to access the site, that we need to get the download url from.
        // Either we can go to http://mods.curse.com/addons/wow/ADDONNAME/download and curse forward us to
        // the real site with a number at the end (like http://mods.curse.com/addons/wow/ADDONNAME/123456).
        // Or we can go directly to the site with the number. But to know that number, we need to open and
        // parse the addon project site (http://mods.curse.com/addons/wow/ADDONNAME). This is the same url
        // we saved in our database. Both ways have downsides. The forwarding takes time, and an access of
        // 2 sites takes time. The forwarding is around 200 ms faster than the 2-sites-variant. That leads
        // to the conclusion, we should use the forwarding. But it would be faster overall, if we save the
        // real download link after we got him once, to compare the number in site 1 and only go to site 2
        // if the number of the saved real download link is different. Indeed we have then the downside of
        // site 2 (remember that site 1 & 2 together is slower than forwarding, but site-1-only is faster).
        // But most of the time only some few addons need an update and the rest of them could be directly
        // downloaded with the saved download link, or even not downloaded at all, cause we own the latest
        // version. So if we want always download every addon, the forward option is faster and the way to
        // go. We now call the website parser with http://mods.curse.com/addons/wow/ADDONNAME/download for
        // every addon and we are done. But if we detect which addons need an update and there are 75 % or
        // less of it (which is true in most cases), the update is overall way faster, even if every addon
        // by itself takes 200 ms more. Our number parser given http://mods.curse.com/addons/wow/ADDONNAME
        // will serve us the number, which we compare with the one in our saved real download link, and if
        // they are not the same, website parser given a http://mods.curse.com/addons/wow/ADDONNAME/123456
        // will lead us to the real download link and therefore we download the zip. But if the new number
        // and the one in our saved real download link are the same, we take that saved real download link
        // and download that zip, if we want to always overwrite all addons, or, if not, simply do nothing.

        private readonly TaskScheduler taskScheduler;

        private bool unzipAddons;
        private bool cancelDownloads;

        public AddonDownloadManager()
            : this(TaskScheduler.FromCurrentSynchronizationContext())
        {
        }

        public AddonDownloadManager(TaskScheduler taskScheduler)
        {
            if (taskScheduler == null)
            {
                throw new ArgumentNullException("taskScheduler");
            }

            this.taskScheduler = taskScheduler;
        }

        public bool DownloadsRunning
        {
            get;
            private set;
        }

        public event Action<AddonProgressData> DownloadProgress;
        public event Action<bool> DownloadsFinished;

        public void DownloadAddonsAsync(IEnumerable<AddonData> addonData, bool unzipAddons, bool updateAlways, bool parallelDownload)
        {
            if (addonData == null)
            {
                throw new ArgumentNullException("addonData");
            }

            if (!addonData.Any())
            {
                throw new ArgumentException("List is empty.", "addonData");
            }

            if (!TestInternetConnection())
            {
                throw new AddonDownloadManagerException("Internet connection error.");
            }

            if (DownloadsRunning)
            {
                throw new AddonDownloadManagerException("Downloads already running.");
            }
            else
            {
                DownloadsRunning = true;
            }

            var addons = addonData.Select(ad => new AddonProgressData(ad.AddonUrl, ad.LastDownloadUrl, ad.DownloadFolder, updateAlways, ad.CustomTag));

            this.unzipAddons = unzipAddons;

            cancelDownloads = false;

            // Do not change this task implementations or they produce inexplicable side effects and do not work as intended.

            if (parallelDownload)
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 25;

                Task.Factory.StartNew(() =>
                {
                    Task.WaitAll(addons.Select(a => Task.Factory.StartNew(() => GetAddon(a))).ToArray());

                    OnFinished();
                });
            }
            else
            {
                Task.Factory.StartNew(() =>
                {
                    addons.ToList().ForEach(a => GetAddon(a));

                    OnFinished();
                });
            }
        }

        public void CancelDownloads()
        {
            cancelDownloads = true;
        }

        private bool TestInternetConnection()
        {
            using (var ping = new Ping())
            {
                var pr = ping.Send("www.google.com", 1500);

                return pr.Status == IPStatus.Success;
            }
        }

        private void GetAddon(AddonProgressData progress)
        {
            if (cancelDownloads)
            {
                return;
            }

            try
            {
                ParseAddon(progress);
            }
            catch (Exception e)
            {
                // Patch 26 Feb 2022: We have to add this, since we wanna show a different status in the UI.

                if (e?.InnerException?.Data != null && e.InnerException.Data.Contains("NoRetailRelease"))
                {
                    progress.Status = AddonProgressStatus.ParseErrorNoRetailRelease;
                }
                else
                {
                    progress.Status = AddonProgressStatus.ParseError;
                }

                OnProgress(progress);

                return;
            }

            if (!progress.DownloadAddon)
            {
                progress.Status = AddonProgressStatus.Finished;
                OnProgress(progress);

                return;
            }

            if (cancelDownloads)
            {
                return;
            }

            try
            {
                DownloadAddon(progress);
            }
            catch
            {
                progress.Status = AddonProgressStatus.DownloadError;
                OnProgress(progress);

                return;
            }

            try
            {
                if (unzipAddons)
                {
                    UnzipAddon(progress);
                }
            }
            catch
            {
                progress.Status = AddonProgressStatus.UnzipError;
                OnProgress(progress);

                return;
            }

            progress.Status = AddonProgressStatus.Finished;
            OnProgress(progress);
        }

        private void ParseAddon(AddonProgressData progress)
        {
            var parser = new CurseParserUsingApi();

            progress.AddonName = parser.GetAddonName(progress.AddonUrl);

            progress.Status = AddonProgressStatus.Parsing;
            OnProgress(progress);

            var task = parser.GetDownloadUrlAsync(progress.AddonName);
            task.Wait();
            var newDownloadUrl = task.Result;

            // If above "updateAlways" is true, we simply download the addon with new download url.
            // If above "updateAlways" is false, we check if old and new download url are the same.
            // If they are, we do nothing. Else we download the new addon via the new download url.

            if (progress.DownloadAddon)
            {
                progress.DownloadUrl = newDownloadUrl;
            }
            else
            {
                if (progress.DownloadUrl != newDownloadUrl)
                {
                    progress.DownloadUrl = newDownloadUrl;
                    progress.DownloadAddon = true;
                }
            }

            var fileName = progress.DownloadUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();

            if (!fileName.Contains(".zip"))
            {
                throw new AddonDownloadManagerException("The url does not contain a zip file.");
            }

            if (progress.DownloadFolder.Last() != Path.DirectorySeparatorChar)
            {
                fileName = Path.DirectorySeparatorChar + fileName;
            }

            progress.DownloadFile = progress.DownloadFolder + fileName;

            progress.Status = AddonProgressStatus.ParsingFinished;
            OnProgress(progress);
        }

        private void DownloadAddon(AddonProgressData progress)
        {
            if (!Directory.Exists(progress.DownloadFolder))
            {
                Directory.CreateDirectory(progress.DownloadFolder);
            }

            if (File.Exists(progress.DownloadFile))
            {
                File.Delete(progress.DownloadFile);
            }

            using (var are = new AutoResetEvent(false))
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadProgressChanged += (s, e) =>
                    {
                        progress.Status = AddonProgressStatus.Downloading;
                        progress.DownloadPercentage = (byte)e.ProgressPercentage;
                        progress.TotalBytes = e.TotalBytesToReceive;
                        progress.ReceivedBytes = e.BytesReceived;
                        OnProgress(progress);
                    };

                    wc.DownloadFileCompleted += (s, e) =>
                    {
                        progress.Status = e.Error == null ? AddonProgressStatus.DownloadingFinished : AddonProgressStatus.DownloadError;
                        OnProgress(progress);

                        (e.UserState as AutoResetEvent).Set();
                    };

                    wc.DownloadFileAsync(new Uri(progress.DownloadUrl), progress.DownloadFile, are);

                    are.WaitOne(1000 * 60 * 3);
                }
            }
        }

        private void UnzipAddon(AddonProgressData progress)
        {
            progress.Status = AddonProgressStatus.Unzipping;
            OnProgress(progress);

            var zipExtractor = new ZipExtractor();
            zipExtractor.ExtractZipFile(progress.DownloadFile, progress.DownloadFolder, true);

            progress.Status = AddonProgressStatus.UnzippingFinished;
            OnProgress(progress);
        }

        private AddonProgressData CreateProgressCopy(AddonProgressData progress)
        {
            var result = new AddonProgressData();

            result.Status = progress.Status;
            result.AddonName = progress.AddonName;
            result.AddonUrl = progress.AddonUrl;
            result.DownloadUrl = progress.DownloadUrl;
            result.DownloadFolder = progress.DownloadFolder;
            result.DownloadFile = progress.DownloadFile;
            result.DownloadPercentage = progress.DownloadPercentage;
            result.ReceivedBytes = progress.ReceivedBytes;
            result.TotalBytes = progress.TotalBytes;
            result.CustomTag = progress.CustomTag;

            return result;
        }

        private void OnProgress(AddonProgressData progress)
        {
            // We use tasks here to execute the event handler in a taskscheduler context, defined by the user. But
            // the task execution order is purely random and a downloading event can occure before a parsing event,
            // even if we start the tasks synchronous in the right order, as we do it in the GetAddon() method. So
            // we use TaskCreationOptions.PreferFairness, to ensure all Progress-Events are fired in correct order.

            var progressCopy = CreateProgressCopy(progress);

            Task.Factory.StartNew(state =>
            {
                var handler = DownloadProgress;

                if (handler != null)
                {
                    handler(state as AddonProgressData);
                }
            },
            progressCopy, CancellationToken.None, TaskCreationOptions.PreferFairness, taskScheduler);
        }

        private void OnFinished()
        {
            // We use tasks here to execute the event handler in a taskscheduler context, defined by the user. But
            // the task execution order is purely random and a downloading event can occure before a parsing event,
            // even if we start the tasks synchronous in the right order, as we do it in the GetAddon() method. So
            // we use TaskCreationOptions.PreferFairness, to ensure all Progress-Events are fired in correct order.

            DownloadsRunning = false;

            Task.Factory.StartNew(state =>
            {
                var handler = DownloadsFinished;

                if (handler != null)
                {
                    handler((bool)state);
                }
            },
            cancelDownloads, CancellationToken.None, TaskCreationOptions.PreferFairness, taskScheduler);
        }
    }
}
