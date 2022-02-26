namespace MBODM.WoW
{
    public enum AddonProgressStatus
    {
        Ready,
        Parsing,
        ParsingFinished,
        ParseError,
        Downloading,
        DownloadingFinished,
        DownloadError,
        Unzipping,
        UnzippingFinished,
        UnzipError,
        Finished,
    }
}
