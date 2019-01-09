using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTrackerTests;
using TESLTracker.Utils;

namespace ESLTracker.Utils.Tests
{
    [TestClass]
    public class SerializableVersionTests : BaseTest
    {
        [TestMethod]
        public void ToStringTest001()
        {
            SerializableVersion sv = new SerializableVersion(1, 2, 3, 4);
            Assert.AreEqual("1.2.3.4", sv.ToString());
        }

        [TestMethod]
        public void ToStringTest002()
        {
            SerializableVersion sv = new SerializableVersion(1, 2, 3, 4);

            Dictionary<string, string> testCases = new Dictionary<string, string>()
            {
                { "G", "1.2.3.4" },
                { "{M}.{m}.{b}.{r}", "1.2.3.4" },
                { "{M}.{m}", "1.2" },
                { "hello {M}.{m}", "hello 1.2" },
                { "mm", "1.2" },
                { "hello mm" , "hello mm"}
            };

            foreach (KeyValuePair<string, string> textCase in testCases)
            {
                Assert.AreEqual(textCase.Value, sv.ToString(textCase.Key), "Failed for case {0}", textCase.Key);
            }
        }
    }
}