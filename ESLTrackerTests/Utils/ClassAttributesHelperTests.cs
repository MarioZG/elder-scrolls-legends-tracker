using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.Tests
{
    [TestClass()]
    public class ClassAttributesHelperTests
    {

        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        [TestMethod()]
        public void FindClassByAttributeTest001()
        {
            List<DeckAttribute> filter = new List<DeckAttribute>()
            {
                DeckAttribute.Strength,
                DeckAttribute.Intelligence,
                DeckAttribute.Agility
            };
            DeckClass expected = DeckClass.Dagoth;

            IEnumerable<DeckClass> result = ClassAttributesHelper.FindClassByAttribute(filter);

            TestContext.WriteLine("Result:");
            foreach(DeckClass dc in result)
            {
                TestContext.WriteLine(dc+",");
            }

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(expected, result.ElementAt(0));
        }

        [TestMethod()]
        public void FindClassByAttributeTest002()
        {
            List<DeckAttribute> filter = new List<DeckAttribute>()
            {
                DeckAttribute.Strength,
            };
            List<DeckClass> expected = new List<DeckClass>() {
                DeckClass.Battlemage,
                DeckClass.Archer,
                DeckClass.Crusader,
                DeckClass.Warrior,
                DeckClass.Strength,
                DeckClass.Dagoth,
                DeckClass.Hlaalu,
                DeckClass.Redoran
            };

            IEnumerable<DeckClass> result = ClassAttributesHelper.FindClassByAttribute(filter);

            TestContext.WriteLine("Result:");
            foreach (DeckClass dc in result)
            {
                TestContext.WriteLine(dc + ",");
            }

            Assert.AreEqual(expected.Count, result.Count());
            int i = 0;
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));
            Assert.IsTrue(expected.Contains(result.ElementAt(i++)));

        }
    }
}