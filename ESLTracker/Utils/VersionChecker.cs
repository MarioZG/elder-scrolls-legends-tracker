using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ESLTracker.Utils
{
    public class VersionChecker
    {
        private NewVersioInfo appVersionInfo;
        public NewVersioInfo AppVersionInfo {
            get
            {
                if (appVersionInfo == null)
                {
                    appVersionInfo = CheckNewAppVersionAvailable();
                }
                return appVersionInfo;
            }
        }      

        private ITrackerFactory trackerFactory;

        public VersionChecker(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
        }

        public NewVersioInfo CheckNewAppVersionAvailable()
        {
            string url = "https://raw.githubusercontent.com/MarioZG/elder-scrolls-legends-tracker/master/Build/versions.json";
            IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
            string versionJSON = httpService.SendGetRequest(url);
            JObject versions = JObject.Parse(versionJSON);
            SerializableVersion latest = new SerializableVersion(new Version(versions["Application"].ToString()));
            IApplicationService appService = (IApplicationService)trackerFactory.GetService<IApplicationService>();

            return new NewVersioInfo()
            {
                IsAvailable = latest > appService.GetAssemblyVersion(),
                Number = latest.ToString(),
                DownloadUrl = GetLatestDownladUrl()
            };
        }

        internal string GetLatestDownladUrl()
        {
            string url = "https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/latest";
            IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
            string versionJSON = httpService.SendGetRequest(url);
            JObject lastetRelease = JObject.Parse(versionJSON);
            return lastetRelease["assets"]?[0]?["browser_download_url"]?.ToString();
        }

        public bool IsNewCardsDBAvailable()
        {
            string url = "https://raw.githubusercontent.com/MarioZG/elder-scrolls-legends-tracker/master/Build/versions.json";
            IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
            string versionJSON = httpService.SendGetRequest(url);
            JObject versions = JObject.Parse(versionJSON);
            Version latest = new Version(versions["CardsDB"].ToString());
            return latest > trackerFactory.GetCardsDatabase().Version;
        }

        public void GetLatestCardsDB()
        {
            string url = "https://raw.githubusercontent.com/MarioZG/elder-scrolls-legends-tracker/v0.6.0/ESLTracker/Resources/cards.json";
            IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
            string cardsDBContent = httpService.SendGetRequest(url);
            trackerFactory.GetFileManager().UpdateCardsDB(cardsDBContent);
        }

    }
}
