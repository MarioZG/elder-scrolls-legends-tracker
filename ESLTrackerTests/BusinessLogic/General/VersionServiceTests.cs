using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.Properties;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.General;

namespace ESLTrackerTests.BusinessLogic.General
{
    [TestClass]
    public class VersionCheckerTests : BaseTest
    {
        Mock<ISettings> settings;
        Mock<IHTTPService> httpService = new Mock<IHTTPService>();
        Mock<IApplicationInfo> appService = new Mock<IApplicationInfo>();
        Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();
        Mock<ICardsDatabaseFactory> cardsDatabaseFactory = new Mock<ICardsDatabaseFactory>();

        [TestInitialize]
        public void TestInit()
        {
            settings = new Mock<ISettings>();
            settings.Setup(s => s.VersionCheck_CardsDBUrl).Returns(String.Empty);
            settings.Setup(s => s.VersionCheck_LatestBuildUrl).Returns(String.Empty);
            settings.Setup(s => s.VersionCheck_LatestBuildUserUrl).Returns(String.Empty);
            settings.Setup(s => s.VersionCheck_VersionsUrl).Returns(String.Empty);
        }

        [TestMethod]
        public void IsNewApplicationVersionAvailableTest001()
        {
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5\"}");

            appService.Setup(a => a.GetAssemblyVersion()).Returns(
                new SerializableVersion(0, 5, 1, 0)
                );

            VersionService vc = CreateVersionServiceObject();
            bool actual = vc.CheckNewAppVersionAvailable().IsAvailable;

            Assert.AreEqual(false, actual);

        }



        [TestMethod]
        public void IsNewApplicationVersionAvailableTest002()
        {
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5.1\"}");

            appService.Setup(a => a.GetAssemblyVersion()).Returns(
                new SerializableVersion(0, 5, 1, 0)
                );

            VersionService vc = CreateVersionServiceObject();
            bool actual = vc.CheckNewAppVersionAvailable().IsAvailable;

            Assert.AreEqual(false, actual);

        }

        [TestMethod]
        public void IsNewApplicationVersionAvailableTest003_Available()
        {
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5.2\"}");

            appService.Setup(a => a.GetAssemblyVersion()).Returns(
                new SerializableVersion(0, 5, 1, 0)
                );

            VersionService vc = CreateVersionServiceObject();
            bool actual = vc.CheckNewAppVersionAvailable().IsAvailable;

            Assert.AreEqual(true, actual);

        }

        [TestMethod()]
        public void IsNewCardsDBAvailableTest()
        {
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5.2\"}");

            cardsDatabase.Setup(a => a.Version).Returns(
                new Version(0, 4)
                );


            VersionService vc = CreateVersionServiceObject();
            bool actual = vc.IsNewCardsDBAvailable();

            Assert.AreEqual(false, actual);
        }

        [TestMethod()]
        public void GetLatestDownladUrlTest()
        {
            string expected = "https://expected_link.zip";
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                @"{
  ""url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/5198358"",
  ""assets_url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/5198358/assets"",
  ""assets"": [
    {
                ""url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/assets/3023121"",
      ""id"": 3023121,
      ""name"": ""ESLTracker_0.6.zip"",
      ""label"": null,
      ""content_type"": ""application/zip"",
      ""state"": ""uploaded"",
      ""size"": 14158667,
      ""download_count"": 3,
      ""created_at"": ""2017-01-19T21:12:55Z"",
      ""updated_at"": ""2017-01-19T21:18:45Z"",
      ""browser_download_url"": """+ expected + @"""
    }
  ],
  ""tarball_url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/tarball/v0.6.0"",
  ""zipball_url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/zipball/v0.6.0"",
  ""body"": ""## New features\r\n- Track changes in deck and compare win ratio of different versions\r\n- Import decks from text\r\n- Filter Statistics by Deck #7 \r\n- Update cards database to reflect card changes from January 2017\r\n- About form with contact info\r\n\r\n## Fixes\r\n- Sometimes edit game window blocked closing application\r\n- Double click on system tray icon now opens/hides main form\r\n- Number of minor fixes and performance improvements\r\n""
}
");

            cardsDatabase.Setup(a => a.Version).Returns(
                new Version(0, 4)
                );

            VersionService vc = CreateVersionServiceObject();
            string actual = vc.GetLatestDownladUrl();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ReleaseHasNoAssets()
        {
            string expected = "";
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                @"{
  ""url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/5198358"",
  ""assets_url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/releases/5198358/assets"",
  ""assets"": [
 
  ],
  ""tarball_url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/tarball/v0.6.0"",
  ""zipball_url"": ""https://api.github.com/repos/MarioZG/elder-scrolls-legends-tracker/zipball/v0.6.0"",
  ""body"": ""## New features\r\n- Track changes in deck and compare win ratio of different versions\r\n- Import decks from text\r\n- Filter Statistics by Deck #7 \r\n- Update cards database to reflect card changes from January 2017\r\n- About form with contact info\r\n\r\n## Fixes\r\n- Sometimes edit game window blocked closing application\r\n- Double click on system tray icon now opens/hides main form\r\n- Number of minor fixes and performance improvements\r\n""
}
");

            cardsDatabase.Setup(a => a.Version).Returns(
                new Version(0, 4)
                );

            VersionService vc = CreateVersionServiceObject();
            string actual = vc.GetLatestDownladUrl();

            Assert.AreEqual(expected, actual);
        }

        private VersionService CreateVersionServiceObject()
        {
            return new VersionService(mockLogger.Object, settings.Object, cardsDatabase.Object, httpService.Object, appService.Object, cardsDatabaseFactory.Object);
        }
    }
}