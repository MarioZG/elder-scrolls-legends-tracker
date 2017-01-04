using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ESLTracker.Properties.Tests
{
    [TestClass()]
    public class SettingsTests
    {
        [TestMethod()]
        public void SettingsTest001_EnsureAllDefaultsAreSet()
        {
            foreach (SettingsProperty property in ESLTracker.Properties.Settings.Default.Properties)
            {
                if (property.DefaultValue == null)
                {
                    Assert.Fail("Property {0} doesn't have default value!", property.Name);
                }
            }
        }
    }
}