using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.Properties;
using ESLTracker.DataModel;
using System.Collections.ObjectModel;

namespace ESLTracker.Utils.Tests
{
    [TestClass]
    public class ScreenshotNameProviderTests
    {
        [TestMethod]
        public void GetScreenShotNameTest001_PackName()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();

            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.Packs_ScreenshotNameTemplate)
                .Returns("Pack_{n:000}_{d:yyyy_MM_dd}-{1:000}_{0:yyyy_MM_dd}");

            trackerFactory.Setup(tf => tf.GetSettings()).Returns(settings.Object);

            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 12, 23));

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Packs).Returns(
                new ObservableCollection<Pack>(
                    Enumerable.Range(0,4).Select(i=> new Pack())
                ));
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            string expected = "Pack_004_2016_12_23-004_2016_12_23";

            ScreenshotNameProvider provider = new ScreenshotNameProvider(trackerFactory.Object);

            string actual = provider.GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetScreenShotNameTest002_RegularName()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();

            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.Packs_ScreenshotNameTemplate)
                .Returns("Pack_{d:yyyy_MM_dd}-{0:yyyy_MM_dd}");

            trackerFactory.Setup(tf => tf.GetSettings()).Returns(settings.Object);

            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 12, 23));

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Packs).Returns(
                new ObservableCollection<Pack>(
                    Enumerable.Range(0, 4).Select(i => new Pack())
                ));
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            string expected = "Pack_2016_12_23-2016_12_23";

            ScreenshotNameProvider provider = new ScreenshotNameProvider(trackerFactory.Object);

            string actual = provider.GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);

            Assert.AreEqual(expected, actual);
        }
    }
}