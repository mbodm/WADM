using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace MBODM
{
    public sealed class ZipExtractor : IZipExtractor
    {
        private const bool Debug = false;
        private const int ReliableMethodsDelay = 25;
        private const int ReliableMethodsTimeout = 3000;

        public void ExtractZipFile(string zipFile, string destFolder, bool deleteZipFile)
        {
            try
            {
                zipFile = zipFile.Trim();

                if (string.IsNullOrEmpty(zipFile))
                {
                    throw new ArgumentException("Parameter is null or empty.", "zipFile");
                }

                destFolder = destFolder.Trim();

                if (string.IsNullOrEmpty(destFolder))
                {
                    throw new ArgumentException("Parameter is null or empty.", "destFolder");
                }

                zipFile = Path.GetFullPath(zipFile);

                if (!File.Exists(zipFile))
                {
                    throw new ArgumentException("Zip file not exists.", "zipFile");
                }

                destFolder = Path.GetFullPath(destFolder);

                if (destFolder.Last() == Path.DirectorySeparatorChar)
                {
                    destFolder = destFolder.TrimEnd(Path.DirectorySeparatorChar);
                }

                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }

                var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName().Replace(".", string.Empty));

                while (Directory.Exists(tempFolder))
                {
                    tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName().Replace(".", string.Empty));
                }

                ZipFile.ExtractToDirectory(zipFile, tempFolder);

                if (deleteZipFile)
                {
                    ReliableFileDelete(zipFile);
                }

                var newFileNames = from f in Directory.GetFiles(tempFolder) select Path.GetFileName(f);

                foreach (var fileName in newFileNames)
                {
                    var oldFile = Path.Combine(destFolder, fileName);
                    var newFile = Path.Combine(tempFolder, fileName);

                    if (File.Exists(oldFile))
                    {
                        ReliableFileDelete(oldFile);
                    }

                    ReliableFileCopy(newFile, oldFile);
                }

                var newDirNames = from d in Directory.GetDirectories(tempFolder) select (new DirectoryInfo(d)).Name;

                foreach (var dirName in newDirNames)
                {
                    var oldDir = Path.Combine(destFolder, dirName);
                    var newDir = Path.Combine(tempFolder, dirName);

                    if (Directory.Exists(oldDir))
                    {
                        ReliableDirectoryDelete(oldDir);
                    }

                    ReliableDirectoryCopy(newDir, oldDir);
                }

                ReliableDirectoryDelete(tempFolder);
            }
            catch (Exception e)
            {
                DebugLog("ExtractZipFile() [Main Method]", @"C:\ZipExtractor.fake", null, e, null);

                throw;
            }
        }

        // We need the following methods, because the zip class, or a windows behavior,
        // or an antivirus scanner, need some time to unlock the files and directories.

        private void ReliableFileCopy(string sourceFile, string destFile)
        {
            var flag = false;
            var repeatCounter = 0;
            var repeatMaximum = ReliableMethodsTimeout / ReliableMethodsDelay;

            while (!flag)
            {
                try
                {
                    File.Copy(sourceFile, destFile, true);
                    flag = true;
                }
                catch (Exception e)
                {
                    if (repeatCounter < repeatMaximum) repeatCounter++; else throw;
                    Thread.Sleep(ReliableMethodsDelay);

                    DebugLog("File.Copy()", sourceFile, destFile, e, null);
                }
            }
        }

        private void ReliableFileDelete(string file)
        {
            var flag = false;
            var repeatCounter = 0;
            var repeatMaximum = ReliableMethodsTimeout / ReliableMethodsDelay;

            while (!flag)
            {
                try
                {
                    File.Delete(file);
                    flag = true;
                }
                catch (Exception e)
                {
                    if (repeatCounter < repeatMaximum) repeatCounter++; else throw;
                    Thread.Sleep(ReliableMethodsDelay);

                    DebugLog("File.Delete()", file, null, e, null);
                }
            }
        }

        private void ReliableDirectoryCopy(string sourceDirectory, string destDirectory)
        {
            var flag = false;
            var repeatCounter = 0;
            var repeatMaximum = ReliableMethodsTimeout / ReliableMethodsDelay;

            while (!flag)
            {
                try
                {
                    // We use this copy directory approach, because some self-made
                    // directory deep-copy solution could result in dangerous code.

                    var computer = new Microsoft.VisualBasic.Devices.Computer();
                    computer.FileSystem.CopyDirectory(sourceDirectory, destDirectory, true);
                    flag = true;
                }
                catch (Exception e)
                {
                    if (repeatCounter < repeatMaximum) repeatCounter++; else throw;
                    Thread.Sleep(ReliableMethodsDelay);

                    var text = (from string key in e.Data.Keys select "Key: " + key.ToString() + " Value: " + e.Data[key] + Environment.NewLine).ToString();
                    DebugLog("FileSystem.CopyDirectory()", sourceDirectory, destDirectory, e, text);
                }
            }
        }

        private void ReliableDirectoryDelete(string directory)
        {
            var flag = false;
            var repeatCounter = 0;
            var repeatMaximum = ReliableMethodsTimeout / ReliableMethodsDelay;

            while (!flag)
            {
                try
                {
                    Directory.Delete(directory, true);
                    flag = true;
                }
                catch (Exception e)
                {
                    if (repeatCounter < repeatMaximum) repeatCounter++; else throw;
                    Thread.Sleep(ReliableMethodsDelay);

                    DebugLog("Directory.Delete()", directory, null, e, null);
                }
            }
        }

        private void DebugLog(string methodName, string methodParam1, string methodParam2, Exception catchedException, string additionalText)
        {
            if (Debug)
            {
                var text = string.Empty;

                text += methodName + " " + methodParam1;
                if (!string.IsNullOrEmpty(methodParam2)) text += " " + methodParam2;
                text += Environment.NewLine;
                var now = DateTime.Now;
                text += string.Format("Date/Time {0} {1:00}:{2:00}:{3:00}:{4:0000}", now.ToShortDateString(), now.Hour, now.Minute, now.Second, now.Millisecond);
                text += Environment.NewLine;
                text += catchedException.GetType().ToString();
                text += Environment.NewLine;
                text += catchedException.Message;
                text += Environment.NewLine;
                text += catchedException.StackTrace;
                text += Environment.NewLine;
                if (!string.IsNullOrEmpty(additionalText)) text += additionalText;
                text += Environment.NewLine;
                text += Environment.NewLine;

                var logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ZipExtractorDebugLogs", Path.GetFileNameWithoutExtension(methodParam1) + ".txt");

                if (!Directory.Exists(Path.GetDirectoryName(logFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logFile));
                }

                File.AppendAllText(logFile, text);
            }
        }
    }
}
