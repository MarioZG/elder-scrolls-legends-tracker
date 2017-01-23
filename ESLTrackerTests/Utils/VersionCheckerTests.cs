using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace ESLTracker.Utils.Tests
{
    [TestClass]
    public class VersionCheckerTests
    {
        [TestMethod]
        public void IsNewApplicationVersionAvailableTest001()
        {
            Mock<IHTTPService> httpService = new Mock<IHTTPService>();
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5\"}");

            Mock<IApplicationService> appService = new Mock<IApplicationService>();
            appService.Setup(a => a.GetAssemblyVersion()).Returns(
                new SerializableVersion(0, 5, 1, 0)
                );

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<IHTTPService>()).Returns(httpService.Object);
            trackerFactory.Setup(tf => tf.GetService<IApplicationService>()).Returns(appService.Object);

            VersionChecker vc = new VersionChecker(trackerFactory.Object);
            bool actual = vc.CheckNewAppVersionAvailable().IsAvailable;

            Assert.AreEqual(false, actual);

        }

        [TestMethod]
        public void IsNewApplicationVersionAvailableTest002()
        {
            Mock<IHTTPService> httpService = new Mock<IHTTPService>();
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5.1\"}");

            Mock<IApplicationService> appService = new Mock<IApplicationService>();
            appService.Setup(a => a.GetAssemblyVersion()).Returns(
                new SerializableVersion(0, 5, 1, 0)
                );

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<IHTTPService>()).Returns(httpService.Object);
            trackerFactory.Setup(tf => tf.GetService<IApplicationService>()).Returns(appService.Object);

            VersionChecker vc = new VersionChecker(trackerFactory.Object);
            bool actual = vc.CheckNewAppVersionAvailable().IsAvailable;

            Assert.AreEqual(false, actual);

        }

        [TestMethod]
        public void IsNewApplicationVersionAvailableTest003_Available()
        {
            Mock<IHTTPService> httpService = new Mock<IHTTPService>();
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5.2\"}");

            Mock<IApplicationService> appService = new Mock<IApplicationService>();
            appService.Setup(a => a.GetAssemblyVersion()).Returns(
                new SerializableVersion(0, 5, 1, 0)
                );

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<IHTTPService>()).Returns(httpService.Object);
            trackerFactory.Setup(tf => tf.GetService<IApplicationService>()).Returns(appService.Object);

            VersionChecker vc = new VersionChecker(trackerFactory.Object);
            bool actual = vc.CheckNewAppVersionAvailable().IsAvailable;

            Assert.AreEqual(true, actual);

        }

        [TestMethod()]
        public void IsNewCardsDBAvailableTest()
        {
            Mock<IHTTPService> httpService = new Mock<IHTTPService>();
            httpService.Setup(hs => hs.SendGetRequest(It.IsAny<string>())).Returns(
                "{ \"CardsDB\": \"0.3\", \"Application\" :  \"0.5.2\"}");

            Mock<ICardsDatabase> cardsDB = new Mock<ICardsDatabase>();
            cardsDB.Setup(a => a.Version).Returns(
                new Version(0, 4)
                );

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<IHTTPService>()).Returns(httpService.Object);
            trackerFactory.Setup(tf => tf.GetCardsDatabase()).Returns(cardsDB.Object);

            VersionChecker vc = new VersionChecker(trackerFactory.Object);
            bool actual = vc.IsNewCardsDBAvailable();

            Assert.AreEqual(false, actual);
        }

        [TestMethod()]
        public void GetLatestDownladUrlTest()
        {
            string expected = "https://expected_link.zip";
            Mock<IHTTPService> httpService = new Mock<IHTTPService>();
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

            Mock<ICardsDatabase> cardsDB = new Mock<ICardsDatabase>();
            cardsDB.Setup(a => a.Version).Returns(
                new Version(0, 4)
                );

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<IHTTPService>()).Returns(httpService.Object);
            trackerFactory.Setup(tf => tf.GetCardsDatabase()).Returns(cardsDB.Object);

            VersionChecker vc = new VersionChecker(trackerFactory.Object);
            string actual = vc.GetLatestDownladUrl();

            Assert.AreEqual(expected, actual);
        }
    }
}