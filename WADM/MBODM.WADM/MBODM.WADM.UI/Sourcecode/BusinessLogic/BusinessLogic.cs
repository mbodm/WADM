using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MBODM.WoW;
using MBODM.WOW;

namespace MBODM.WADM.UI
{
    public sealed class BusinessLogic
    {
        private readonly string configFile;

        public BusinessLogic()
        {
            var configFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "MBODM";

            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            configFile = configFolder + Path.DirectorySeparatorChar + "WADM.xml";

            Addons = new ObservableCollection<AddonData>();

            DownloadFolder += Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + Path.DirectorySeparatorChar;
            DownloadFolder += "World of Warcraft" + Path.DirectorySeparatorChar;
            DownloadFolder += "Interface" + Path.DirectorySeparatorChar + "AddOns";
        }

        public string ProgramName
        {
            get { return "WADM " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "  -  World of Warcraft Addon Download Manager"; }
        }

        public string DownloadFolder
        {
            get;
            set;
        }

        public ObservableCollection<AddonData> Addons
        {
            get;
            private set;
        }

        public event Action<AddonProgressData> DownloadProgress;
        public event Action<bool> DownloadFinished;

        public void SaveConfig()
        {
            SortAddons(a => a.AddonName, true);

            var persister = new AddonDataPersister();

            persister.DownloadFolder = DownloadFolder;

            persister.AddonDataEntries.Clear();

            foreach (var a in Addons)
            {
                persister.AddonDataEntries.Add(new AddonDataEntry(a.AddonUrl, a.DownloadUrl, a.Active));
            }

            persister.Save(configFile);
        }

        public void LoadConfig()
        {
            var persister = new AddonDataPersister();

            if (persister.Load(configFile))
            {
                DownloadFolder = persister.DownloadFolder;

                foreach (var a in persister.AddonDataEntries)
                {
                    AddAddon(a.AddonUrl, a.LastDownloadUrl, a.IsActive);
                }
            }
        }

        public void ShowConfig()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "explorer.exe";
            psi.Arguments = Path.GetDirectoryName(configFile);

            var p = new Process();
            p.StartInfo = psi;
            p.Start();

            Application.Current.Shutdown();
        }

        public void AddAddon(string addonUrl)
        {
            AddAddon(addonUrl, string.Empty, true);
        }

        public void AddAddon(string addonUrl, string downloadUrl, bool isActive)
        {
            var parser = new CurseParserUsingApi();
            var addonName = parser.GetAddonName(addonUrl);

            var q = from a in Addons where a.AddonName == addonName select a;
            if (q.Any())
            {
                throw new WadmException("An addon with this name already exists.");
            }

            var addon = new AddonData();
            addon.AddonName = addonName;
            addon.AddonUrl = addonUrl;

            if (!string.IsNullOrEmpty(downloadUrl))
            {
                addon.DownloadUrl = downloadUrl;
            }

            addon.Active = isActive;

            Addons.Add(addon);
        }

        public void DeleteAddon(string name)
        {
            var q = from a in Addons where a.AddonName == name select a;
            if (!q.Any())
            {
                return;
            }

            Addons.Remove(q.First());
        }

        public void SortAddons<T>(Func<AddonData, T> orderBy, bool ascending)
        {
            List<AddonData> sortedAddons;

            if (ascending)
            {
                sortedAddons = Addons.OrderBy(orderBy).ToList();
            }
            else
            {
                sortedAddons = Addons.OrderByDescending(orderBy).ToList();
            }

            Addons.Clear();

            foreach (var a in sortedAddons)
            {
                Addons.Add(a);
            }
        }

        public void Download()
        {
            if (!Addons.Any())
            {
                return;
            }

            if (string.IsNullOrEmpty(DownloadFolder))
            {
                throw new ArgumentException("There must be a download folder.");
            }

            if (!Directory.Exists(DownloadFolder))
            {
                throw new ArgumentException("The given download folder not exists.");
            }

            var addonDownloadManager = new AddonDownloadManager(TaskScheduler.FromCurrentSynchronizationContext());

            addonDownloadManager.DownloadProgress += (progress) =>
            {
                switch (progress.Status)
                {
                    case AddonProgressStatus.Parsing:
                        (progress.CustomTag as AddonData).StatusText = "Parsing";
                        break;
                    case AddonProgressStatus.ParsingFinished:
                        var addon = (from a in Addons where a.AddonName == progress.AddonName select a).FirstOrDefault();
                        if (addon != null) addon.DownloadUrl = progress.DownloadUrl;
                        break;
                    case AddonProgressStatus.ParseError:
                        (progress.CustomTag as AddonData).StatusText = "Parse error";
                        (progress.CustomTag as AddonData).Error = true;
                        break;
                    case AddonProgressStatus.Downloading:
                        // Old version including all in one string:
                        // (progress.CustomTag as AddonData).StatusText = "Load... " + string.Format("{0} % ({1} KB / {2} KB) ", progress.DownloadPercentage, progress.ReceivedBytes / 1000, progress.TotalBytes / 1000);
                        (progress.CustomTag as AddonData).StatusText = "Loading " + progress.DownloadPercentage.ToString() + " %";
                        (progress.CustomTag as AddonData).DownloadPercentage = progress.DownloadPercentage;
                        (progress.CustomTag as AddonData).ReceivedData = progress.ReceivedBytes / 1024;
                        break;
                    case AddonProgressStatus.DownloadingFinished:
                        (progress.CustomTag as AddonData).ReceivedData = progress.TotalBytes / 1024;
                        (progress.CustomTag as AddonData).FileName = Path.GetFileName(progress.DownloadFile);
                        break;
                    case AddonProgressStatus.DownloadError:
                        (progress.CustomTag as AddonData).StatusText = "Download error";
                        (progress.CustomTag as AddonData).Error = true;
                        break;
                    case AddonProgressStatus.UnzippingFinished:
                        // Old version including all in one string:
                        // (progress.CustomTag as AddonData).StatusText = "Finished " + (progress.CustomTag as AddonData).StatusText.Replace("Load... ", string.Empty);
                        (progress.CustomTag as AddonData).StatusText = "Finished";
                        (progress.CustomTag as AddonData).DownloadPercentage = 100;
                        break;
                    case AddonProgressStatus.UnzipError:
                        (progress.CustomTag as AddonData).StatusText = "Unzip error";
                        (progress.CustomTag as AddonData).Error = true;
                        break;
                }

                var handler = DownloadProgress;
                if (handler != null) handler(progress);
            };

            addonDownloadManager.DownloadsFinished += (wasCancelled) =>
            {
                var handler = DownloadFinished;
                if (handler != null) handler(wasCancelled);
            };

            var addons = from addon in Addons where addon.Active select new WoW.AddonData(addon.AddonUrl, addon.DownloadUrl, DownloadFolder, addon);

            addonDownloadManager.DownloadAddonsAsync(addons, true, true, true);
        }
    }
}
