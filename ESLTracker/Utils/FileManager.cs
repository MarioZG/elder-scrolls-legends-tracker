using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        public static void SaveDatabase<T>(string path, T tracker)
        {
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

        private static void BackupDatabase(string path)
        {
            string backupFileName = Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("yyyyMMddHHmmss");
            string backupPath = Path.Combine(
                Path.GetDirectoryName(path),
                backupFileName);
            backupPath = Path.ChangeExtension(backupPath, Path.GetExtension(path));
            File.Copy(path, backupPath);
        }
    }
}
