using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.General;
using ESLTracker.DataModel;
using ESLTracker.Properties;
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

namespace ESLTracker.BusinessLogic.DataFile
{
    public class FileSaver : IFileSaver
    {
        PathManager pathManager;
        IPathWrapper pathWrapper;
        IDirectoryWrapper directoryWrapper;
        IFileWrapper fileWrapper;
        
        public FileSaver(
            PathManager pathManager,
            IPathWrapper pathWrapper,
            IDirectoryWrapper directoryWrapper,
            IFileWrapper fileWrapper)
        {
            this.pathManager = pathManager;
            this.pathWrapper = pathWrapper;
            this.directoryWrapper= directoryWrapper;
            this.fileWrapper = fileWrapper;
        }

        public void SaveDatabase(ITracker tracker)
        {
            SaveDatabase<Tracker>(
                pathManager.FullDataFilePath,
                tracker as Tracker);
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
    }
}
