using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.FileUpdaters;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TESLTracker.Utils;

namespace ESLTracker.BusinessLogic.DataFile
{
    public class FileUpdater
    {
        private readonly PathManager pathManager;

        public FileUpdater(PathManager pathManager)
        {
            this.pathManager = pathManager;
        }

        internal bool UpdateFile()
        {
            return UpdateFile(ReadCurrentFileVersionFromXML(), null);
        }

        internal bool UpdateFile(SerializableVersion fromVersion, Tracker tracker)
        {
            if (fromVersion == null)
            {
                return false;
            }
            IEnumerable<Type> updaterTypes = FindUpdateClass(fromVersion);
            if (updaterTypes.Count() != 1)
            {
                //no updater found, or too many
                return false;
            }
            IFileUpdater updater = (IFileUpdater)MasserContainer.Container.GetInstance(updaterTypes.First());
            return updater.UpdateFile(pathManager.FullDataFilePath, tracker);
        }

        private SerializableVersion ReadCurrentFileVersionFromXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(pathManager.FullDataFilePath);
            XmlNode versionNode = doc.SelectSingleNode("/Tracker/Version");

            return ParseCurrentFileVersion(versionNode);
        }

        public SerializableVersion ParseCurrentFileVersion(XmlNode versionNode)
        {
            if (versionNode == null)
            {
                return null;
            }

            bool allOK = true;
            SerializableVersion ret = new SerializableVersion();
            allOK &= Int32.TryParse(versionNode.SelectSingleNode("Build")?.InnerText, out ret.Build);
            allOK &= Int32.TryParse(versionNode.SelectSingleNode("Major")?.InnerText, out ret.Major);
            allOK &= Int32.TryParse(versionNode.SelectSingleNode("Minor")?.InnerText, out ret.Minor);
            allOK &= Int32.TryParse(versionNode.SelectSingleNode("Revision")?.InnerText, out ret.Revision);

            if (!allOK)
            {
                //parse failed
                return null;
            }
            return ret;
        }

        private IEnumerable<Type> FindUpdateClass(SerializableVersion currentFileVersion)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypesSafely())
                {
                    SerializableVersion upgradeFrom = type.GetCustomAttributes(typeof(SerializableVersion), true).FirstOrDefault() as SerializableVersion;
                    if (upgradeFrom == currentFileVersion)
                    {
                        yield return type;
                    }
                }
            }
        }
    }
}
