namespace MBODM
{
    public interface IZipExtractor
    {
        void ExtractZipFile(string zipFile, string destFolder, bool deleteZipFile);
    }
}
