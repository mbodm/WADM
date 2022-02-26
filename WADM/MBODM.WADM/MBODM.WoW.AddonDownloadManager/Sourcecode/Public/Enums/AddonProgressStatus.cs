namespace MBODM.WoW
{
    public enum AddonProgressStatus
    {
        Ready,
        Parsing,
        ParsingFinished,
        ParseErrorNoRetailRelease, // Patch 26 Feb 2022: We have to add this, since we wanna show a different status in the UI.
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
