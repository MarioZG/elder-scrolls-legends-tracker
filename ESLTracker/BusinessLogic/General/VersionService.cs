using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.Properties;
using ESLTracker.Utils;
using Newtonsoft.Json.Linq;
using NLog;
using Polly;
using Polly.Fallback;

namespace ESLTracker.BusinessLogic.General
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

        private ISettings settings;
        ICardsDatabase cardsDatabase;
        ICardsDatabaseFactory cardsDatabaseFactory;
        IHTTPService httpService;
        IApplicationInfo applicationService;

        public VersionService(
            ISettings settings,
            ICardsDatabase cardsDatabase,
            IHTTPService httpService,
            IApplicationInfo applicationService,
            ICardsDatabaseFactory cardsDatabaseFactory)
        {
            this.cardsDatabase = cardsDatabase;
            this.settings = settings;
            this.httpService = httpService;
            this.applicationService = applicationService;
            this.cardsDatabaseFactory = cardsDatabaseFactory;
        }

        public NewVersioInfo CheckNewAppVersionAvailable()
        {
            string url = settings.VersionCheck_VersionsUrl;

            var requestErrorPolicy = Policy<NewVersioInfo>
                                    .Handle<System.Net.WebException>()
                                    .Fallback(
                                        new NewVersioInfo() { IsAvailable = false }
                                    );

            return requestErrorPolicy.Execute(() =>
            {
                string versionJSON = httpService.SendGetRequest(url);
                JObject versions = JObject.Parse(versionJSON);
                SerializableVersion latest = new SerializableVersion(new Version(versions["Application"].ToString()));

                return new NewVersioInfo()
                {
                    IsAvailable = latest > applicationService.GetAssemblyVersion(),
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
                string versionJSON = httpService.SendGetRequest(url);
                JObject versions = JObject.Parse(versionJSON);
                Version latest = new Version(versions["CardsDB"].ToString());
                return latest > cardsDatabase.Version;
            });
        }

        public ICardsDatabase GetLatestCardsDB()
        {
            string url = settings.VersionCheck_CardsDBUrl;
            ICardsDatabase returnValue = null;
            var requestErrorPolicy = Policy
                        .Handle<System.Net.WebException>()
                        .Fallback(
                            () => { returnValue = cardsDatabase; },
                            (ex, context) => {
                                Logger.Trace(ex, "Exception when retreiving cards DB from {0}", url);
                                Logger log = LogManager.GetLogger(App.UserInfoLogger);
                                log.Info(ex.Message);
                            }
                        );
            requestErrorPolicy.Execute(() =>
            {
                Logger.Trace("Start retreiving cards DB from {0}", url);
                string cardsDBContent = httpService.SendGetRequest(url);
                ICardsDatabase cardsDB = cardsDatabaseFactory.UpdateCardsDB(cardsDBContent, cardsDatabase.Version);
                Logger.Trace("Finished retreiving cards DB from {0}", url);
                returnValue = cardsDB;
            });

            return returnValue;
        }

    }
}
