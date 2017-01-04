using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ESLTracker.Utils.FileUpdaters
{
    public abstract class UpdateBase : IFileUpdater
    {
        public abstract SerializableVersion TargetVersion { get; }

        public bool UpdateFile(FileManager fileManager)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileManager.FullDataFilePath);
            XmlNode versionNode = doc.SelectSingleNode("/Tracker/Version");
            //set new file version
            versionNode.InnerXml = fileManager.CreateNewVersionXML(TargetVersion);

            VersionSpecificUpdateFile(doc, fileManager);

            File.Copy(fileManager.FullDataFilePath, fileManager.FullDataFilePath + "_" + TargetVersion + ".upgrade", true);
            doc.Save(fileManager.FullDataFilePath);

            return true;
        }

        protected abstract void VersionSpecificUpdateFile(XmlDocument doc, FileManager fileManager);
    }
}
