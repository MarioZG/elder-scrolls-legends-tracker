using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(1,0)]
    public class Update_1_0_To_1_1 : IFileUpdater
    {
        public SerializableVersion TargetVersion { get; } = new SerializableVersion(1, 1);

        public bool UpdateFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileManager.FullDataFilePath);
            XmlNode versionNode = doc.SelectSingleNode("/Tracker/Version");

            versionNode.InnerXml = FileManager.CreateNewVersionXML(TargetVersion);

            foreach (XmlNode dataNode in doc.SelectNodes("/Tracker/Decks/Deck/ArenaRank"))
            {
                if (!String.IsNullOrWhiteSpace(dataNode.InnerText))
                {
                    ArenaRank value = (ArenaRank)Enum.Parse(typeof(ArenaRank), dataNode.InnerText);
                    dataNode.InnerText = ((int)value).ToString();
                }
            }

            File.Copy(FileManager.FullDataFilePath, FileManager.FullDataFilePath + ".upgrade", true);
            doc.Save(FileManager.FullDataFilePath);

            return true;
        }


    }
}
