using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

namespace ESLTracker.Utils.Converters.Tests
{
    [TestClass()]
    public class BoolToVisibiltyConverterTests
    {
        [TestMethod()]
        public void ConvertTest001_True()
        {
            bool value = true;
            object param = null;

            Visibility expected = Visibility.Visible;


            BoolToVisibiltyConverter converter = new BoolToVisibiltyConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest002_False()
        {
            bool value = false;
            object param = null;

            Visibility expected = Visibility.Hidden;


            BoolToVisibiltyConverter converter = new BoolToVisibiltyConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest004_TrueToCollapsed()
        {
            bool value = true;
            object param = "ss-collapsed-ss";

            Visibility expected = Visibility.Visible;


            BoolToVisibiltyCollapsedConverter converter = new BoolToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest004_FalseToCollapsed()
        {
            bool value = false;
            object param = "collapsed";

            Visibility expected = Visibility.Collapsed;


            BoolToVisibiltyCollapsedConverter converter = new BoolToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest005_NOTTrueToCollapsed()
        {
            bool value = true;
            object param = "NOT-collapsed";

            Visibility expected = Visibility.Collapsed;


            BoolToVisibiltyCollapsedConverter converter = new BoolToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest006_NOTFalseToCollapsed()
        {
            bool value = false;
            object param = "NOT-rr-ss-collapsed-ss-";

            Visibility expected = Visibility.Visible;


            BoolToVisibiltyCollapsedConverter converter = new BoolToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }
    }
}