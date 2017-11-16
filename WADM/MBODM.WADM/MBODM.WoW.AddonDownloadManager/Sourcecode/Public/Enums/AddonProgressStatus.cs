using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
