using ESLTracker.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.DataFile
{
    public class PathManager
    {
        public string DataFile = "data.xml";

        ISettings settings;

        public PathManager(ISettings settings)
        {
            this.settings = settings;
        }

        public string DataPath
        {
            get
            {
                string dp = settings.DataPath;
                if (String.IsNullOrWhiteSpace(dp))
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

        public string CardsDatabasePath = "./Resources/cards.json";
    }
}
