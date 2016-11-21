using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.Utils.IOWrappers;

namespace ESLTracker.Utils
{
    public class FileManager
    {

        public static T LoadDatabase<T>(string path)
        {
            T tracker;
            //standard serialization
            using (TextReader reader = new StreamReader(path))
            {
                var xml = new XmlSerializer(typeof(T));
                tracker = (T)xml.Deserialize(reader);
            }

            return tracker;
        }

        public static void SaveDatabase()
        {
            SaveDatabase<DataModel.Tracker>("./data.xml", DataModel.Tracker.Instance);
        }

        public static void SaveDatabase<T>(string path, T tracker)
        {
            IWrapperProvider wrapperProvider = new WrapperProvider();
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

        public static void BackupDatabase(IWrapperProvider wrapperProvider, string path)
        {
            IPathWrapper pathWrapper = wrapperProvider.GetWrapper(typeof(IPathWrapper)) as IPathWrapper;
            IDirectoryWrapper directoryWrapper = wrapperProvider.GetWrapper(typeof(IDirectoryWrapper)) as IDirectoryWrapper;
            IFileWrapper fileWrapper = wrapperProvider.GetWrapper(typeof(IFileWrapper)) as IFileWrapper;
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

        public static void ManageBackups(
            string path, 
            IPathWrapper pathWrapper, 
            IDirectoryWrapper directoryWrapper,
            IFileWrapper fileWrapper)
        {
            var backupFiles = directoryWrapper.EnumerateFiles(
                            pathWrapper.GetDirectoryName(path),
                            "data*.xml").OrderBy(f => f);
            //first save of day - delete old backups
            int backupcount = backupFiles.Count();
            int skipfiles = 8; //backups to preserve + data file
            if (backupcount > skipfiles) //7+1 for actual data.xml file
            {
                foreach (string s in backupFiles.Skip(skipfiles))
                {
                    fileWrapper.Delete(s);
                }
            }
        }
    }
}
