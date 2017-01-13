using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESLTracker.Utils.Converters.Tests
{
    [TestClass()]
    public class VersionToVisibiltyTests
    {
        [TestMethod]
        public void ConvertTest001_noParams()
        {
            VersionToVisibilty converter = new VersionToVisibilty();

            Visibility expected = Visibility.Visible;

            object result = converter.Convert(
                new SerializableVersion(new Version("0.0.0.0")),
                null,
                null,
                null);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertTest002_versionequals()
        {
            VersionToVisibilty converter = new VersionToVisibilty();

            Visibility expected = Visibility.Visible;

            object result = converter.Convert(
                new SerializableVersion(new Version("0.0.0.0")),
                null,
                "0.0.0.0",
                null);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertTest003_versionEqualsDifferent()
        {
            VersionToVisibilty converter = new VersionToVisibilty();

            Visibility expected = Visibility.Collapsed;

            object result = converter.Convert(
                new SerializableVersion(new Version("0.0.0.0")),
                null,
                "0.0.2.0",
                null);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertTest004_NotEqualsOperatort()
        {
            VersionToVisibilty converter = new VersionToVisibilty();

            Visibility expected = Visibility.Visible;

            object result = converter.Convert(
                new SerializableVersion(new Version("0.0.0.0")),
                null,
                "not-0.0.2.0",
                null);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertTest005_NotEqualsOperatorDifferent()
        {
            VersionToVisibilty converter = new VersionToVisibilty();

            Visibility expected = Visibility.Collapsed;

            object result = converter.Convert(
                new SerializableVersion(new Version("0.0.0.0")),
                null,
                "not-0.0.0.0",
                null);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertTest006_NotFullversion()
        {
            VersionToVisibilty converter = new VersionToVisibilty();

            Visibility expected = Visibility.Visible;

            object result = converter.Convert(
                new SerializableVersion(new Version("1.2.0.0")),
                null,
                "1.2",
                null);

            Assert.AreEqual(expected, result);
        }
    }
}