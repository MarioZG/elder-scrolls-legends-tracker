using ESLTracker.Utils.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Utils.Converters
{
    [TestClass()]
    public class TabIndexIncrementorTests
    {
        [TestMethod()]
        public void ConvertTest001_NoParam()
        {
            object value = 10;
            object param = null;

            int expected = 10;


            TabIndexIncrementor converter = new TabIndexIncrementor();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest001_WithParam()
        {
            object value = 10;
            object param = 3;

            int expected = 13;


            TabIndexIncrementor converter = new TabIndexIncrementor();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest001_WithParamAsString()
        {
            object value = 10;
            object param = "3";

            int expected = 13;


            TabIndexIncrementor converter = new TabIndexIncrementor();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest001_WithParamAsInvalidString()
        {
            object value = 10;
            object param = "olaboga!";

            int expected = 10;


            TabIndexIncrementor converter = new TabIndexIncrementor();

            object result = converter.Convert(
                value,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }
    }
}
