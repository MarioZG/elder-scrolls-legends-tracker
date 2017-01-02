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
    public class ObjectToVisibiltyCollapsedConverterTests
    {
        [TestMethod()]
        public void ConvertTest001_IsNotNull()
        {
            Visibility expected = Visibility.Visible;
            object param = null;

            ObjectToVisibilty converter = new ObjectToVisibilty();

            object result = converter.Convert(
                new object(),
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest002_IsNull()
        {
            Visibility expected = Visibility.Collapsed;
            object param = null;

            object valueToCOnvert = null;

            ObjectToVisibilty converter = new ObjectToVisibilty();

            object result = converter.Convert(
                valueToCOnvert,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest003_IsNullReversed()
        {
            Visibility expected = Visibility.Visible;
            object param = "NOT";

            object valueToCOnvert = null;

            ObjectToVisibilty converter = new ObjectToVisibilty();

            object result = converter.Convert(
                valueToCOnvert,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest004_IsNOTNullReversed()
        {
            Visibility expected = Visibility.Collapsed;
            object param = "NOT";

            object valueToCOnvert = new object();

            ObjectToVisibilty converter = new ObjectToVisibilty();

            object result = converter.Convert(
                valueToCOnvert,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }
    }
}