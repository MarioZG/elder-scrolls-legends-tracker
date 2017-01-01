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
    }
}