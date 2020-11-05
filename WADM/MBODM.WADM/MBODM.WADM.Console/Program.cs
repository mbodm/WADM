using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MBODM.WoW;

namespace MBODM.WADM
{
    class Program
    {
        private static string error;
        private static bool finished;
        private static Stopwatch stopwatch;

        // SpeedTest
        //private static object syncRoot = new object();
        //private static Stopwatch speedTestStopWatch = new Stopwatch();

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("WADM Console " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (by MBODM 2015)");
            Console.WriteLine();

            var configFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "MBODM";

            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            var configFile = configFolder + Path.DirectorySeparatorChar + "WADM.xml";

            if (!File.Exists(configFile))
            {
                Console.WriteLine("Error: Could not find WADM config file.");
                Console.WriteLine();
                return;
            }

            var dataPersister = new AddonDataPersister();

            if (!dataPersister.Load(configFile))
            {
                Console.WriteLine("Error: Could not load addon data from WADM config file.");
                Console.WriteLine();
                return;
            }

            // SpeedTest
            //speedTestStopWatch.Start();
            //var addons = dataPersister.AddonDataEntries.Select((a, i) => new AddonData(a.AddonUrl, a.LastDownloadUrl, dataPersister.DownloadFolder, GetSpeedTestTime()));

            var addons = dataPersister.AddonDataEntries.Select((a, i) => new AddonData(a.AddonUrl, a.LastDownloadUrl, dataPersister.DownloadFolder, null));

            var downloadManager = new AddonDownloadManager(TaskScheduler.Default);

            downloadManager.DownloadProgress += (progressData) =>
            {
                switch (progressData.Status)
                {
                    case AddonProgressStatus.Parsing:
                        break;
                    case AddonProgressStatus.ParsingFinished:
                        Console.Write(".");
                        // SpeedTest
                        //Console.WriteLine("start - parsing finished " + (GetSpeedTestTime() - (long)progressData.CustomTag).ToString() + " ms");
                        //progressData.CustomTag = GetSpeedTestTime();
                        break;
                    case AddonProgressStatus.ParseError:
                        error = "parse";
                        downloadManager.CancelDownloads();
                        break;
                    case AddonProgressStatus.Downloading:
                        break;
                    case AddonProgressStatus.DownloadingFinished:
                        dataPersister.AddonDataEntries.Where(a => a.AddonUrl == progressData.AddonUrl).FirstOrDefault().LastDownloadUrl = progressData.DownloadUrl;
                        Console.Write(".");
                        // SpeedTest    
                        //Console.WriteLine("parsing finished - download finished " + (GetSpeedTestTime() - (long)progressData.CustomTag).ToString() + " ms");
                        break;
                    case AddonProgressStatus.DownloadError:
                        error = "download";
                        downloadManager.CancelDownloads();
                        break;
                    case AddonProgressStatus.Unzipping:
                        break;
                    case AddonProgressStatus.UnzippingFinished:
                        break;
                    case AddonProgressStatus.UnzipError:
                        error = "unzip";
                        downloadManager.CancelDownloads();
                        break;
                }
            };

            downloadManager.DownloadsFinished += (cancelled) =>
            {
                stopwatch.Stop();

                if (cancelled)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Error: " + error + " error occurred after " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds.");
                    Console.WriteLine();
                    Console.WriteLine("Cancelled ! Use the WADM UI to get more information.");
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(" !");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Successfully finished after " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds.");
                    Console.WriteLine();
                    Console.WriteLine("Have a nice day.");
                    Console.WriteLine();
                }

                finished = true;
            };

            int oldWorkerThreadsCount;
            int oldCompletionPortThreadsCount;
            ThreadPool.GetMinThreads(out oldWorkerThreadsCount, out oldCompletionPortThreadsCount);
            ThreadPool.SetMinThreads(24, 24);

            try
            {
                Console.Write("Processing " + addons.Count() + " addons ");

                // SpeedTest
                //Console.WriteLine();

                stopwatch = new Stopwatch();
                stopwatch.Start();

                downloadManager.DownloadAddonsAsync(addons, true, true, true);

                while (!finished)
                {
                }

                dataPersister.Save(configFile);
            }
            finally
            {
                ThreadPool.SetMinThreads(oldWorkerThreadsCount, oldCompletionPortThreadsCount);
            }

            // Debug
            //Console.WriteLine("Press ENTER key to exit...");
            //Console.WriteLine();
            //Console.ReadLine();
        }

        // SpeedTest
        //private static long GetSpeedTestTime()
        //{
        //    lock (syncRoot)
        //    {
        //        return speedTestStopWatch.ElapsedMilliseconds;
        //    }
        //}
    }
}
