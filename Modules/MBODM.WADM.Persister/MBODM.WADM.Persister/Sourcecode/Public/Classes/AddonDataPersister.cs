using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MBODM.WADM
{
    public sealed class AddonDataPersister : IAddonDataPersister
    {
        public AddonDataPersister()
        {
            DownloadFolder = string.Empty;
            AddonDataEntries = new List<AddonDataEntry>();
        }

        public string DownloadFolder
        {
            get;
            set;
        }

        public IList<AddonDataEntry> AddonDataEntries
        {
            get;
            set;
        }

        public bool Save(string file)
        {
            try
            {
                var document = new XDocument(
                    new XElement("root",
                        new XElement("folder", new XAttribute("path", DownloadFolder)),
                        new XElement("addons", from addonDataEntry in AddonDataEntries
                                               select new XElement("addon",
                                                   new XAttribute("url", addonDataEntry.AddonUrl),
                                                   new XAttribute("last", addonDataEntry.LastDownloadUrl),
                                                   new XAttribute("active", addonDataEntry.IsActive)))));

                document.Save(file);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Load(string file)
        {
            try
            {
                var document = XDocument.Load(file);

                if (document.Element("root") == null)
                {
                    throw new Exception();
                }

                var downloadFolder = (from element in document.Element("root").Elements()
                                      where (element.Name == "folder") && (element.Attribute("path") != null)
                                      select element.Attribute("path").Value).FirstOrDefault();

                if (downloadFolder == null)
                {
                    throw new Exception();
                }

                DownloadFolder = downloadFolder;

                if (document.Element("root").Element("addons") == null)
                {
                    throw new Exception();
                }

                AddonDataEntries = (from e in document.Element("root").Element("addons").Elements()
                                    where (e.Name == "addon") && (e.Attribute("url") != null) && (e.Attribute("last") != null) && (e.Attribute("active") != null)
                                    select new AddonDataEntry(e.Attribute("url").Value, e.Attribute("last").Value, bool.Parse(e.Attribute("active").Value))).ToList();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
