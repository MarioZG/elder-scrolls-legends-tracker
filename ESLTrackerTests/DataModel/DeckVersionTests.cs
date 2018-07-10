using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTrackerTests;
using ESLTracker.Utils;
using Moq;
using System.Reflection;
using System.Collections;

namespace ESLTracker.DataModel.Tests
{
    [TestClass()]
    public class DeckVersionTests : BaseTest
    {
        /*
               *     x.Equals(x) returns true. This is called the reflexive property.

                      x.Equals(y) returns the same value as y.Equals(x). This is called the symmetric property.

                      if (x.Equals(y) && y.Equals(z)) returns true, then x.Equals(z) returns true. This is called the transitive property.

                      Successive invocations of x.Equals(y) return the same value as long as the objects referenced by x and y are not modified.

                      x.Equals(null) returns false. However, null.Equals(null) throws an exception; it does not obey rule number two above.
                      */

        [TestMethod]
        public void EqualsTest001_GuideRule_1()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            Assert.IsTrue(deckVersion.Equals(deckVersion));
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [TestMethod]
        public void EqualsTest002_GuideRule_2_diffvalues()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, EditProp);

            Assert.AreEqual(deckVersion.Equals(deckVersion2), deckVersion2.Equals(deckVersion));
        }

        [TestMethod]
        public void EqualsTest003_GuideRule_2_sameValues()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, StartProp);

            Assert.AreEqual(deckVersion.Equals(deckVersion2), deckVersion2.Equals(deckVersion));
        }


        [TestMethod]
        public void EqualsTest004_GuideRule_3()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, StartProp);

            DeckVersion deckVersion3 = new DeckVersion();
            PopulateObject(deckVersion3, StartProp);

            Assert.IsTrue(deckVersion.Equals(deckVersion2));
            Assert.IsTrue(deckVersion2.Equals(deckVersion3));
        }

        [TestMethod]
        public void EqualsTest005_GuideRule_4()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, StartProp);

            Assert.IsTrue(deckVersion.Equals(deckVersion2));
            Assert.IsTrue(deckVersion.Equals(deckVersion2));
            Assert.IsTrue(deckVersion.Equals(deckVersion2));
            Assert.IsTrue(deckVersion.Equals(deckVersion2));
            Assert.IsTrue(deckVersion.Equals(deckVersion2));
            Assert.IsTrue(deckVersion.Equals(deckVersion2));
        }

        [TestMethod]
        public void EqualsTest006_GuideRule_5()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            Assert.IsFalse(deckVersion.Equals(null));

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void EqualsTest007_GuideRule_5()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            object nullObject = null;

            Assert.IsFalse(nullObject.Equals(deckVersion));

        }


        [TestMethod]
        public void EqualsTest008_operator_equals()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, EditProp);

            Assert.IsFalse(deckVersion == deckVersion2);
        }

        [TestMethod]
        public void EqualsTest009_operator_equals()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, StartProp);

            Assert.IsTrue(deckVersion == deckVersion2);
        }


        [TestMethod]
        public void EqualsTest009_operator_not_equals()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, StartProp);

            Assert.IsFalse(deckVersion != deckVersion2);
        }

        [TestMethod]
        public void EqualsTest010_operator_not_equals()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion deckVersion2 = new DeckVersion();
            PopulateObject(deckVersion2, EditProp);

            Assert.IsTrue(deckVersion != deckVersion2);
        }

        [TestMethod]
        public void EqualsTest011_comparesAllFields()
        {
            Guid guid = Guid.NewGuid(); //ensure have same guids
            mockDatetimeProvider.SetupGet(f => f.DateTimeNow).Returns(new DateTime(2017, 1, 3, 23, 44, 0));

            foreach (PropertyInfo p in typeof(DeckVersion).GetProperties())
            {
                if (p.CanWrite)
                {
                    DeckVersion d1 = new DeckVersion();
                    DeckVersion d2 = new DeckVersion();

                    TestContext.WriteLine("Checking prop:" + p.Name);

                    p.SetValue(d2, EditProp[p.PropertyType]);

                    Assert.IsFalse(d1 == d2); //should return false
                }
            }
        }

        [TestMethod]
        public void CloneTest()
        {
            DeckVersion deckVersion = new DeckVersion();
            PopulateObject(deckVersion, StartProp);

            DeckVersion clone = deckVersion.Clone() as DeckVersion;

            foreach (PropertyInfo p in typeof(DeckVersion).GetProperties())
            {
                if (p.CanWrite)
                {
                    TestContext.WriteLine("Checking prop:{0}.{1};{2}", p.Name, p.GetValue(deckVersion), p.GetValue(clone));
                    if (p.PropertyType == typeof(string))
                    {
                        //http://stackoverflow.com/questions/506648/how-do-strings-work-when-shallow-copying-something-in-c
                        continue;
                    }
                    if (p.PropertyType.GetInterface(nameof(ICollection)) != null)
                    {
                        CollectionAssert.AreNotEqual(p.GetValue(deckVersion) as ICollection, p.GetValue(clone) as ICollection, new ReferenceComparer());
                    }
                    else
                    {
                        Assert.IsFalse(Object.ReferenceEquals(p.GetValue(deckVersion), p.GetValue(clone)));
                    }
                }
            }
        }
    }
}