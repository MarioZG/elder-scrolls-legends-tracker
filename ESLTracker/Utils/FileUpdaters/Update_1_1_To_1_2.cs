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
    [SerializableVersion(1,1)]
    public class Update_1_1_To_1_2 : IFileUpdater
    {
        public SerializableVersion TargetVersion { get; } = new SerializableVersion(1, 2);

        public bool UpdateFile(FileManager fileManager)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileManager.FullDataFilePath);
            XmlNode versionNode = doc.SelectSingleNode("/Tracker/Version");
            //set new file version
            versionNode.InnerXml = fileManager.CreateNewVersionXML(TargetVersion);

            //do nothing - just need to mark new version

            File.Copy(fileManager.FullDataFilePath, fileManager.FullDataFilePath + ".upgrade", true);
            doc.Save(fileManager.FullDataFilePath);

            return true;
        }


    }
}
