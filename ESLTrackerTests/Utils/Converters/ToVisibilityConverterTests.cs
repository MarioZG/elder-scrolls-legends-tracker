using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;

namespace ESLTracker.Utils.Converters.Tests
{
    public class SampleToVisibiltyClass : ToVisibilityConverter<bool>
    {
        protected override bool Condition(object value)
        {
            return true;
        }

        internal void CheckForOtherParameters_Access(object parameter)
        {
            base.CheckForOtherParameters(parameter);
        }

        internal Visibility ReturnWhenFalse_Access
        {
            get
            {
                return base.ReturnWhenFalse;
            }
        }

        internal Visibility ReturnWhenTrue_Access
        {
            get
            {
                return base.ReturnWhenTrue;
            }
        }
    }

    [TestClass()]
    public class ToVisibilityConverterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckForOtherParametersTest001_ThrowWhenBoth()
        {
            string paramaterPassed = "collapsed-hidden";
            Visibility expetced = Visibility.Collapsed;

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }

        [TestMethod]
        public void CheckForOtherParametersTest002_SetCollapsed()
        {
            string paramaterPassed = "collapsed-test";
            Visibility expetced = Visibility.Collapsed;

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }

        [TestMethod]
        public void CheckForOtherParametersTest003_SetHidden()
        {
            string paramaterPassed = "other-hidden";
            Visibility expetced = Visibility.Hidden;

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }

        [TestMethod]
        public void CheckForOtherParametersTest004_SetNotCollapsed()
        {
            string paramaterPassed = "collapsed-not";
            Visibility expetced = Visibility.Collapsed;

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }

        [TestMethod]
        public void CheckForOtherParametersTest005_SetNotHidden()
        {
            string paramaterPassed = "not-hidden";
            Visibility expetced = Visibility.Hidden;//defualt for false 

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }

        [TestMethod]
        public void CheckForOtherParametersTest006_SomeRandoemParams()
        {
            string paramaterPassed = "some random string - and other collapsed string - test";
            Visibility expetced = Visibility.Collapsed;//defualt for false 

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }

        [TestMethod]
        public void CheckForOtherParametersTest007_ConvertTwice_FirstOverridesReturnFalse()
        {
            string paramaterPassed = "some -hidden";
            Visibility expetced = Visibility.Hidden;//defualt for false 

            SampleToVisibiltyClass converter = new SampleToVisibiltyClass();

            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);

            //another call - without converter params
            paramaterPassed = "";
            expetced = Visibility.Collapsed;
            converter.CheckForOtherParameters_Access(paramaterPassed);

            Assert.AreEqual(expetced, converter.ReturnWhenFalse_Access);
        }
    }
}