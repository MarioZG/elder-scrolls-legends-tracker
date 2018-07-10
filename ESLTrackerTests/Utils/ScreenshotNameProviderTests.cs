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
using ESLTrackerTests;

namespace ESLTracker.Utils.Tests
{
    [TestClass]
    public class ScreenshotNameProviderTests : BaseTest
    {
        Mock<ITracker> tracker = new Mock<ITracker>();


        [TestMethod]
        public void GetScreenShotNameTest001_PackName()
        {
            mockSettings.Setup(s => s.Packs_ScreenshotNameTemplate)
                .Returns("Pack_{n:000}_{d:yyyy_MM_dd}-{1:000}_{0:yyyy_MM_dd}");

            tracker.Setup(t => t.Packs).Returns(
                new ObservableCollection<Pack>(
                    Enumerable.Range(0, 4).Select(i => new Pack())
                ));

            mockDatetimeProvider.Setup(tf => tf.DateTimeNow).Returns(new DateTime(2016, 12, 23));

            string expected = "Pack_004_2016_12_23-004_2016_12_23";

            ScreenshotNameProvider provider = CreateScreenshotProviderObject();

            string actual = provider.GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void GetScreenShotNameTest002_RegularName()
        {

            mockSettings.Setup(s => s.Packs_ScreenshotNameTemplate)
                .Returns("Pack_{d:yyyy_MM_dd}-{0:yyyy_MM_dd}");

            mockDatetimeProvider.Setup(tf => tf.DateTimeNow).Returns(new DateTime(2016, 12, 23));

            tracker.Setup(t => t.Packs).Returns(
                new ObservableCollection<Pack>(
                    Enumerable.Range(0, 4).Select(i => new Pack())
                ));

            string expected = "Pack_2016_12_23-2016_12_23";

            ScreenshotNameProvider provider = CreateScreenshotProviderObject();

            string actual = provider.GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);

            Assert.AreEqual(expected, actual);
        }

        private ScreenshotNameProvider CreateScreenshotProviderObject()
        {
            return new ScreenshotNameProvider(tracker.Object, mockSettings.Object, mockDatetimeProvider.Object);
        }

    }
}