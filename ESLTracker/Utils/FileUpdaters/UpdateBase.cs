using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TESLTracker.DataModel;
using TESLTracker.Utils;

namespace ESLTracker.Utils.FileUpdaters
{
    public abstract class UpdateBase : IFileUpdater
    {
        protected readonly ILogger logger;

        public abstract SerializableVersion TargetVersion { get; }

        public UpdateBase(ILogger logger)
        {
            this.logger = logger;
        }

        public bool UpdateFile(string filePath, Tracker tracker)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            VersionSpecificUpdateFile(doc, tracker);

            XmlNode versionNode = doc.SelectSingleNode("/Tracker/Version");
            //set new file version
            versionNode.InnerXml = CreateNewVersionXML(TargetVersion);

            File.Copy(filePath, filePath + "_" + TargetVersion + ".upgrade", true);
            using (TextWriter sw = new StreamWriter(filePath, false, Encoding.UTF8)) //Set encoding
            {
                doc.Save(sw);
            }
            return true;
        }

        protected abstract void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker);

        protected string CreateNewVersionXML(SerializableVersion TargetVersion)
        {
            StringBuilder serialisedVersion = new StringBuilder();
            using (TextWriter writer = new StringWriter(serialisedVersion))
            {
                var xml = new XmlSerializer(typeof(SerializableVersion), String.Empty);
                xml.Serialize(writer, TargetVersion);
            }

            XmlDocument newVersionDoc = new XmlDocument();
            newVersionDoc.LoadXml(serialisedVersion.ToString());

            return newVersionDoc.DocumentElement.InnerXml;
        }
    }
}
