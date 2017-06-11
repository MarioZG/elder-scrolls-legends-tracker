using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Properties;
using ESLTracker.Utils;
using Newtonsoft.Json.Linq;
using NLog;
using Polly;
using Polly.Fallback;

namespace ESLTracker.Services
{
    public class VersionService : IVersionService
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

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
        private ISettings settings;

        public VersionService(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.settings = trackerFactory.GetService<ISettings>();
        }

        public NewVersioInfo CheckNewAppVersionAvailable()
        {
            var requestErrorPolicy = Policy<NewVersioInfo>
                                    .Handle<System.Net.WebException>()
                                    .Fallback(new NewVersioInfo()
                                    {
                                        IsAvailable = false
                                    });

            string url = settings.VersionCheck_VersionsUrl;
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
                        .Or<ArgumentOutOfRangeException>() //no assets on release
                        .Fallback("");

            string url = settings.VersionCheck_LatestBuildUrl;
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

            string url = settings.VersionCheck_VersionsUrl;
            return requestErrorPolicy.Execute(() =>
            {
                IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
                string versionJSON = httpService.SendGetRequest(url);
                JObject versions = JObject.Parse(versionJSON);
                Version latest = new Version(versions["CardsDB"].ToString());
                return latest > trackerFactory.GetService<ICardsDatabase>().Version;
            });
        }

        public void GetLatestCardsDB()
        {
            string url = settings.VersionCheck_CardsDBUrl;
            var requestErrorPolicy = Policy
                        .Handle<System.Net.WebException>()
                        .Fallback(
                            () => { /* do nothing*/},
                            (ex) => {
                                Logger.Trace(ex, "Exception when retreiving cards DB from {0}", url);
                                Logger log = LogManager.GetLogger(App.UserInfoLogger);
                                log.Info(ex.Message);
                            }
                        );
            requestErrorPolicy.Execute(() =>
            {
                Logger.Trace("Start retreiving cards DB from {0}", url);
                IHTTPService httpService = (IHTTPService)trackerFactory.GetService<IHTTPService>();
                string cardsDBContent = httpService.SendGetRequest(url);
                trackerFactory.GetFileManager().UpdateCardsDB(cardsDBContent);
                Logger.Trace("Finished retreiving cards DB from {0}", url);
            });
        }

    }
}
