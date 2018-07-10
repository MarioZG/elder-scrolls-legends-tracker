using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.General;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.FileUpdaters;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ESLTracker.Utils
{
    public class FileManager : IFileManager
    {
        ISettings settings;
        PathManager pathManager;
        IPathWrapper pathWrapper;
        IDirectoryWrapper directoryWrapper;
        IFileWrapper fileWrapper;
        ICardsDatabaseFactory cardsDatabaseFactory;
        ITrackerFactory trackerFactory;

        public static object _instanceLock = new object();
        public static Tracker _instance = null;

        public FileManager(
            ISettings settings, 
            PathManager pathManager,
            IPathWrapper pathWrapper,
            IDirectoryWrapper directoryWrapper,
            IFileWrapper fileWrapper,
            ICardsDatabaseFactory cardsDatabaseFactory,
            ITrackerFactory trackerFactory)
        {
            this.settings = settings;
            this.pathManager = pathManager;
            this.pathWrapper = pathWrapper;
            this.directoryWrapper= directoryWrapper;
            this.fileWrapper = fileWrapper;
            this.cardsDatabaseFactory = cardsDatabaseFactory;
            this.trackerFactory = trackerFactory;
        }

        public ITracker GetTrackerInstance()
        {
            lock(_instanceLock)
            {
                if(_instance == null)
                {
                    _instance = LoadDatabase();
                }
                return _instance;
            }
        } 

        public Tracker LoadDatabase(bool throwDataFileException = false)
        {
            Tracker tracker = null;
            try
            {
                if (File.Exists(pathManager.FullDataFilePath))
                {
                    tracker = LoadDatabase<DataModel.Tracker>(pathManager.FullDataFilePath);

                    //check for data update
                    if (tracker.Version < Tracker.CurrentFileVersion)
                    {
                        if (UpdateFile(tracker.Version, tracker))
                        {
                            //reload after update
                            tracker = LoadDatabase();
                        }
                        else
                        {
                            throw new DataFileException(string.Format("You are using old file format version and application cannot upgrade your file." + Environment.NewLine + Environment.NewLine + "File version={0}. Application works with {1}", tracker.Version, Tracker.CurrentFileVersion));
                        }
                    }
                    else if (tracker.Version > Tracker.CurrentFileVersion)
                    {
                        //using old application
                        throw new DataFileException(
                            string.Format("You are using old version of application. If you continue you might loose data!" + Environment.NewLine + Environment.NewLine + "Press Yes to start anyway (and potencailly loose data), No to cancel." + Environment.NewLine + Environment.NewLine + " File version={0}. Application works with {1}", tracker.Version, Tracker.CurrentFileVersion),
                            true);
                    }

                    //restore active deck
                    Guid? activeDeckFromSettings = settings.LastActiveDeckId;
                    if ((activeDeckFromSettings != null)
                        && (activeDeckFromSettings != Guid.Empty))
                    {
                        tracker.ActiveDeck = tracker.Decks.Where(d => d.DeckId == activeDeckFromSettings).FirstOrDefault();
                    }
                }
                else
                {
                    tracker = new Tracker();
                    tracker.Version = Tracker.CurrentFileVersion;
                }
            }
            catch (DataFileException)
            {
                //Datafile isses should have been resolved on app init
                if (throwDataFileException) //should be true only in app init code
                { 
                    throw;
                }
            }
            catch
            {
                if (tracker != null)
                {
                    if (tracker.Version != Tracker.CurrentFileVersion)
                    {
                        if (UpdateFile(tracker.Version, tracker))
                        {
                            //reload after update
                            tracker = LoadDatabase();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    if (UpdateFile())
                    {
                        //reload after update
                        tracker = LoadDatabase();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return tracker;
        }

        public T LoadDatabase<T>(string path) where T: ITracker
        {
            T tracker;
            //standard serialization
            using (TextReader reader = new StreamReader(path))
            {
                var xml = new XmlSerializer(typeof(T));
                tracker = (T)xml.Deserialize(reader);
            }

            trackerFactory.FixUpDeserializedTracker(tracker);

            return tracker;
        }

        public void SaveDatabase()
        {
            SaveDatabase<Tracker>(
                pathManager.FullDataFilePath,
                _instance);
        }

        public void SaveDatabase<T>(string path, T tracker)
        {
            //check if path exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            //make backup
            if (File.Exists(path))
            {
                BackupDatabase(path);
            }
            //standard serialization
            using (TextWriter writer = new StreamWriter(path))
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(writer, tracker);
            }
        }

        public void BackupDatabase(string path)
        {
            //copy data.xml to data.xml.bak
            //copy data.xml to data_date.bak - it will override last dayily backup. first run of day will create new one
            string backupFileName = pathWrapper.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("yyyyMMdd");
            string backupPath = pathWrapper.Combine(
                pathWrapper.GetDirectoryName(path),
                backupFileName);
            backupPath = pathWrapper.ChangeExtension(backupPath, pathWrapper.GetExtension(path));

            bool backupExists = File.Exists(backupPath);

            File.Copy(path, backupPath, true);

            if (! backupExists)
            {
                ManageBackups(path);
            }
        }

        public void ManageBackups(string path)
        {
            string dataFileFilter = Path.ChangeExtension(
                string.Format("{0}*", Path.GetFileNameWithoutExtension(pathManager.DataFile)),
                Path.GetExtension(pathManager.DataFile));
            var backupFiles = directoryWrapper.EnumerateFiles(
                            pathWrapper.GetDirectoryName(path),
                            dataFileFilter).Where(f=> f != pathManager.FullDataFilePath).OrderByDescending(f => f);
            //first save of day - delete old backups
            int backupcount = backupFiles.Count();
            int skipfiles = 7; //backups to keep
            if (backupcount > skipfiles) 
            {
                foreach (string s in backupFiles.Skip(skipfiles))
                {
                    fileWrapper.Delete(s);
                }
            }
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

        public static SerializableVersion ParseCurrentFileVersion(XmlNode versionNode)
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

        public ICardsDatabase UpdateCardsDB(string newContent)
        {
            string fileName = ".\\Resources\\cards.json";

            string backupFileName = string.Format("{0}_{1}{2}", 
                Path.GetFileNameWithoutExtension(fileName),
                cardsDatabaseFactory.GetCardsDatabase().Version,
                Path.GetExtension(fileName)); //includes . 
            backupFileName = Path.Combine(Path.GetDirectoryName(fileName), backupFileName);

            if (fileWrapper.Exists(backupFileName))
            {
                fileWrapper.Delete(backupFileName);
            }
            fileWrapper.Move(fileName, backupFileName);

            fileWrapper.WriteAllText(fileName, newContent);
            return cardsDatabaseFactory.RealoadDB();
        }


    }
}
