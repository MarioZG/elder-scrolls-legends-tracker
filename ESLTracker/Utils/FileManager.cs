using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.Utils.IOWrappers;
using System.Drawing;
using System.Windows;
using ESLTracker.DataModel;
using System.Reflection;
using ESLTracker.Utils.FileUpdaters;
using System.Xml;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.Utils
{
    public class FileManager : IFileManager
    {
        string DataPath
        {
            get
            {
                string dp = settings.DataPath;
                if(String.IsNullOrWhiteSpace(dp))
                {
                    dp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    dp = Path.Combine(dp, Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName));
                    settings.DataPath = dp;
                    settings.Save();
                }
                return dp;
            }
        }

        public string FullDataFilePath
        {
            get
            {
                return Path.Combine(DataPath, DataFile);
            }
        }

        string DataFile = "data.xml";
        string ScreenShotFolder = "Screenshot";

        ITrackerFactory trackerfactory;
        ISettings settings;

        public FileManager() : this(new TrackerFactory())
        {
        }

        public FileManager(ITrackerFactory trackerfactory)
        {
            this.trackerfactory = trackerfactory;
            this.settings = trackerfactory.GetService<ISettings>();
        }


        public Tracker LoadDatabase(bool throwDataFileException = false)
        {
            Tracker tracker = null;
            try
            {
                if (File.Exists(FullDataFilePath))
                {
                    tracker = LoadDatabase<DataModel.Tracker>(FullDataFilePath);

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
                    tracker = new Tracker(this.trackerfactory);
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

            //fix up ref to decks in games
            foreach (Game g in tracker.Games)
            {
                g.Deck = tracker.Decks.Where(d => d.DeckId == g.DeckId).FirstOrDefault();
            }
            //fix up ref to decks in rewards
            foreach (Reward r in tracker.Rewards)
            {
                r.ArenaDeck = tracker.Decks.Where(d => d.DeckId == r.ArenaDeckId).FirstOrDefault();
            }

            return tracker;
        }

        public void SaveDatabase()
        {
            SaveDatabase<Tracker>(
                FullDataFilePath, 
                trackerfactory.GetTracker() as Tracker);
        }

        public void SaveDatabase<T>(string path, T tracker)
        {
            IWrapperProvider wrapperProvider = trackerfactory.GetWrapperProvider();
            //check if path exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            //make backup
            if (File.Exists(path))
            {
                BackupDatabase(wrapperProvider, path);
            }
            //standard serialization
            using (TextWriter writer = new StreamWriter(path))
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(writer, tracker);
            }
        }

        public void BackupDatabase(IWrapperProvider wrapperProvider, string path)
        {
            IPathWrapper pathWrapper = wrapperProvider.GetWrapper<IPathWrapper>();
            IDirectoryWrapper directoryWrapper = wrapperProvider.GetWrapper<IDirectoryWrapper>();
            IFileWrapper fileWrapper = wrapperProvider.GetWrapper<IFileWrapper>();
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
                ManageBackups(path, pathWrapper, directoryWrapper, fileWrapper);
            }
        }

        public void ManageBackups(
            string path, 
            IPathWrapper pathWrapper, 
            IDirectoryWrapper directoryWrapper,
            IFileWrapper fileWrapper)
        {
            string dataFileFilter = Path.ChangeExtension(
                string.Format("{0}*", Path.GetFileNameWithoutExtension(DataFile)),
                Path.GetExtension(DataFile));
            var backupFiles = directoryWrapper.EnumerateFiles(
                            pathWrapper.GetDirectoryName(path),
                            dataFileFilter).Where(f=> f != FullDataFilePath).OrderByDescending(f => f);
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

        public Task SaveScreenShot(string fileName)
        {
            IntPtr? eslHandle = trackerfactory.GetWinAPI().GetEslProcess()?.MainWindowHandle;
            if (eslHandle.HasValue)
            {
                var rect = new WinAPI.Rect();
                WinAPI.GetWindowRect(eslHandle.Value, ref rect);

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(bmp);

                List<Window> hiddenWindows = new List<Window>();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (Window w in Application.Current.Windows)
                    {
                        // System.Diagnostics.Debugger.Log(1, "", "w"+ w.Title);
                        // System.Diagnostics.Debugger.Log(1, "", "  w.IsActive" + w.IsActive);
                        // System.Diagnostics.Debugger.Log(1, "", "   w.Topmost" + w.Topmost);
                        // System.Diagnostics.Debugger.Log(1, "", Environment.NewLine) ;
                        if (w.IsActive) //if other if visible - cannot do anything; otherwise if it was in back, it would be show at the top:/...
                        {
                            w.Hide();
                            hiddenWindows.Add(w);
                        }
                    }
                });
                WinAPI.SetForegroundWindow(eslHandle.Value);
                gfxBmp.CopyFromScreen(
                    rect.left,
                    rect.top,
                    0,
                    0,
                    new System.Drawing.Size(width, height),
                    CopyPixelOperation.SourceCopy);

                foreach (Window w in hiddenWindows)
                {
                    w.Dispatcher.Invoke(() => w.Show());
                }

                string path = Path.Combine(
                    DataPath,
                    ScreenShotFolder,
                    Path.ChangeExtension(fileName,"png")
                    );
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                gfxBmp.Dispose();

            }

            return Task.FromResult<object>(null);
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
            IFileUpdater updater = (IFileUpdater)Activator.CreateInstance(updaterTypes.First());
            return updater.UpdateFile(this.FullDataFilePath, tracker);
        }


        private SerializableVersion ReadCurrentFileVersionFromXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FullDataFilePath);
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
            IFileWrapper fileWrapper = trackerfactory.GetService<IFileWrapper>();
            ICardsDatabase cardsDatabase = trackerfactory.GetService<ICardsDatabase>();

            string backupFileName = string.Format("{0}_{1}{2}", 
                Path.GetFileNameWithoutExtension(fileName),
                cardsDatabase.Version,
                Path.GetExtension(fileName)); //includes . 
            backupFileName = Path.Combine(Path.GetDirectoryName(fileName), backupFileName);

            if (fileWrapper.Exists(backupFileName))
            {
                fileWrapper.Delete(backupFileName);
            }
            fileWrapper.Move(fileName, backupFileName);

            fileWrapper.WriteAllText(fileName, newContent);
            cardsDatabase.RealoadDB();
            return cardsDatabase;
        }


    }
}
