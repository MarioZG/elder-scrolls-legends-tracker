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
    public class BoolToVisibiltyCollapsedConverterTests
    {
        [TestMethod()]
        public void ConvertTest001_True()
        {
            bool value = true;
            object param = null;

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
        public void ConvertTest002_False()
        {
            bool value = false;
            object param = null;

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
        public void ConvertTest003_NOTTrue()
        {
            bool value = true;
            object param = "NOT";

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
        public void ConvertTest004_NOTFalse()
        {
            bool value = false;
            object param = "NOT";

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