using System;
using ESLTracker.BusinessLogic.General;
using ESLTracker.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESLTrackerTests.BusinessLogic.General
{
    [TestClass]
    public class ScreenShotTests
    {
        [TestMethod]
        public void AdjustForMultimonitorDPI_DPI_2_25()
        {
            WinAPI.Rect rect = new WinAPI.Rect();
            rect.left = 7200;
            rect.right = 11520;
            rect.top = 0;
            rect.bottom = 2430;

            WinAPI.Rect expected = new WinAPI.Rect();
            expected.left = 3200;
            expected.right = 5120;
            expected.top = 0;
            expected.bottom = 1080;

            ScreenShot screenShot = CreateScreenShotObject();

            var actual = screenShot.AdjustForMultimonitorDPI(rect, 216);

            Assert.AreEqual(expected.left, actual.left);
            Assert.AreEqual(expected.right, actual.right);
            Assert.AreEqual(expected.top, actual.top);
            Assert.AreEqual(expected.bottom, actual.bottom);


        }

        private ScreenShot CreateScreenShotObject()
        {
            return new ScreenShot(null, null, null, null);
        }
    }
}
