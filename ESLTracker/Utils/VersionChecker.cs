using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Fallback;

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
            var requestErrorPolicy = Policy<NewVersioInfo>
                                    .Handle<System.Net.WebException>()
                                    .Fallback(new NewVersioInfo()
                                    {
                                        IsAvailable = false
                                    });

            string url = "https://raw.githubusercontent.com/MarioZG/elder-scrolls-legends-tracker/master/Build/versions.json";
            IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();

            return requestErrorPolicy.Execute(() =>
            {
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
            });
        }

        internal string GetLatestDownladUrl()
        {
            var requestErrorPolicy = Policy<string>
                        .Handle<System.Net.WebException>()
                        .Fallback("");

            string url = "https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/latest";
            return requestErrorPolicy.Execute(() =>
            {
                IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
                string versionJSON = httpService.SendGetRequest(url);
                JObject lastetRelease = JObject.Parse(versionJSON);
                return lastetRelease["assets"]?[0]?["browser_download_url"]?.ToString();
            });
        }

        public bool IsNewCardsDBAvailable()
        {
            var requestErrorPolicy = Policy<bool>
                                .Handle<System.Net.WebException>()
                                .Fallback(false);

            string url = "https://raw.githubusercontent.com/MarioZG/elder-scrolls-legends-tracker/master/Build/versions.json";
            return requestErrorPolicy.Execute(() =>
            {
                IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
                string versionJSON = httpService.SendGetRequest(url);
                JObject versions = JObject.Parse(versionJSON);
                Version latest = new Version(versions["CardsDB"].ToString());
                return latest > trackerFactory.GetCardsDatabase().Version;
            });
        }

        public void GetLatestCardsDB()
        {
            var requestErrorPolicy = Policy
                        .Handle<System.Net.WebException>()
                        .Fallback(() => { /* do nothing*/});

            string url = "https://raw.githubusercontent.com/MarioZG/elder-scrolls-legends-tracker/v0.6.0/ESLTracker/Resources/cards.json";
            requestErrorPolicy.Execute(() =>
            {
                IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
                string cardsDBContent = httpService.SendGetRequest(url);
                trackerFactory.GetFileManager().UpdateCardsDB(cardsDBContent);
            });
        }

    }
}
