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
    public class StringNonEmptyToVisibiltyCollapsedConverterTests
    {
        [TestMethod()]
        public void ConvertTest001_NonEmpty()
        {
            string value = "test str";
            object param = null;

            Visibility expected = Visibility.Visible;


            StringNonEmptyToVisibiltyCollapsedConverter converter = new StringNonEmptyToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest002_Empty()
        {
            string value = "    ";
            object param = null;

            Visibility expected = Visibility.Collapsed;


            StringNonEmptyToVisibiltyCollapsedConverter converter = new StringNonEmptyToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest003_NOTNonEmpty()
        {
            string value = "test str";
            object param = "NOT";

            Visibility expected = Visibility.Collapsed;


            StringNonEmptyToVisibiltyCollapsedConverter converter = new StringNonEmptyToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest004_NOTEmpty()
        {
            string value = "    ";
            object param = "NOT";

            Visibility expected = Visibility.Visible;


            StringNonEmptyToVisibiltyCollapsedConverter converter = new StringNonEmptyToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void ConvertTest004_null()
        {
            string value = null;
            object param = null;

            Visibility expected = Visibility.Collapsed;


            StringNonEmptyToVisibiltyCollapsedConverter converter = new StringNonEmptyToVisibiltyCollapsedConverter();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }
    }
}