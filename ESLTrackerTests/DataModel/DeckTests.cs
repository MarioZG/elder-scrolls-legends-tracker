using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Moq;
using ESLTrackerTests;
using ESLTracker.Utils;
using System.Reflection;
using System.Collections;

namespace ESLTracker.DataModel.Tests
{
#pragma warning disable CS0618 // Type or member is obsolete - multiple new Deck() cases
    [TestClass()]
    public class DeckTests : BaseTest
    {
        [TestMethod()]
        public void DeckTest001_GuidPreservedDurinDeserialisation()
        {
            Deck d = new Deck();
            Guid original = d.DeckId;

            StringBuilder tempString = new StringBuilder();
            using (TextWriter writer = new StringWriter(tempString))
            {
                var xml = new XmlSerializer(typeof(Deck));
                xml.Serialize(writer, d);
            }

            Deck deckDeserialise;

            using (TextReader reader = new StringReader(tempString.ToString()))
            {
                var xml = new XmlSerializer(typeof(Deck));
                deckDeserialise = (Deck)xml.Deserialize(reader);
            }

            Assert.AreEqual(original, deckDeserialise.DeckId);
        }

        [TestMethod()]
        public void DeckTest002_CheckDateTimeAdded()
        {

            Deck d = new Deck();

            Assert.IsTrue(d.CreatedDate.Year > 1999);

        }

        [TestMethod()]
        public void DeckTest003_CheckHistoryCreated()
        {
            Deck deck = Deck.CreateNewDeck();

            Assert.IsNotNull(deck.History);
            Assert.AreEqual(1, deck.History.Count);
            Assert.AreEqual(deck.History.First().VersionId, deck.SelectedVersionId);
        }

        [TestMethod()]
        public void DeckTest003_HistoryDeserialised()
        {
            DateTime dateTime_1_1 = new DateTime(2016, 12, 22);
            Deck deck = Deck.CreateNewDeck();
            deck.CreateVersion(1, 1, dateTime_1_1);
            deck.CreateVersion(1, 2, dateTime_1_1);

            string xml = SerializationHelper.SerializeXML(deck);

            Deck deserialised = SerializationHelper.DeserializeXML<Deck>(xml);

            Assert.AreEqual(deck, deserialised, "Deck is different after deserialisation");

        }

        [TestMethod()]
        public void IsArenaRunFinishedTest001_ConstructedDeck()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.Constructed };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 3, 2, 2));

            bool expected = false;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest002_VersusArenaLoss()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);
            trackerFactory.Setup(tf => tf.GetService<IDeckService>()).Returns(new DeckService(trackerFactory.Object));

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.VersusArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 5, 3));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest003_VersusArenaWin()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);
            trackerFactory.Setup(tf => tf.GetService<IDeckService>()).Returns(new DeckService(trackerFactory.Object));

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.VersusArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 2));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest004_SoloArenaLoss()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);
            trackerFactory.Setup(tf => tf.GetService<IDeckService>()).Returns(new DeckService(trackerFactory.Object));

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.SoloArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 3));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest005_SoloArenaWin()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);
            trackerFactory.Setup(tf => tf.GetService<IDeckService>()).Returns(new DeckService(trackerFactory.Object));

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.SoloArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 9, 2));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest006_SoloArenaInProgress()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);
            trackerFactory.Setup(tf => tf.GetService<IDeckService>()).Returns(new DeckService(trackerFactory.Object));

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.SoloArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 2));

            bool expected = false;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest007_VersusArenaInPorgress()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);
            trackerFactory.Setup(tf => tf.GetService<IDeckService>()).Returns(new DeckService(trackerFactory.Object));

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.VersusArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 5, 2));

            bool expected = false;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void CreateVersionTest001_CreateVersion()
        {
            DateTime date = new DateTime(2017, 1, 4);
            int major = 1;
            int minor = 2;

            Deck deck = new Deck();

            int expectedCount = deck.History.Count + 1;

            DeckVersion dv = deck.CreateVersion(major, minor, date);

            Assert.AreEqual(expectedCount, deck.History.Count);
            Assert.AreEqual(date, dv.CreatedDate);
            Assert.AreEqual(major, dv.Version.Major);
            Assert.AreEqual(minor, dv.Version.Minor);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateVersionTest002_CreateVersionAlreadyExists()
        {
            DateTime date = new DateTime(2017, 1, 4);
            int major = 1;
            int minor = 1;

            Deck deck = new Deck();
            DeckVersion dv = deck.CreateVersion(major, minor, date);

            dv = deck.CreateVersion(major, minor, date);

        }

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
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
            Assert.IsTrue(deck.Equals(deck));
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
        }

        [TestMethod]
        public void EqualsTest002_GuideRule_2_diffvalues()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, EditProp);

            Assert.AreEqual(deck.Equals(deck2), deck2.Equals(deck));
        }

        [TestMethod]
        public void EqualsTest003_GuideRule_2_sameValues()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, StartProp);

            Assert.AreEqual(deck.Equals(deck2), deck2.Equals(deck));
        }


        [TestMethod]
        public void EqualsTest004_GuideRule_3()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, StartProp);

            Deck deck3 = new Deck();
            PopulateObject(deck3, StartProp);

            Assert.IsTrue(deck.Equals(deck2));
            Assert.IsTrue(deck2.Equals(deck3));
        }

        [TestMethod]
        public void EqualsTest005_GuideRule_4()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, StartProp);

            Assert.IsTrue(deck.Equals(deck2));
            Assert.IsTrue(deck.Equals(deck2));
            Assert.IsTrue(deck.Equals(deck2));
            Assert.IsTrue(deck.Equals(deck2));
            Assert.IsTrue(deck.Equals(deck2));
            Assert.IsTrue(deck.Equals(deck2));
        }

        [TestMethod]
        public void EqualsTest006_GuideRule_5()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Assert.IsFalse(deck.Equals(null));

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void EqualsTest007_GuideRule_5()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            object nullObject = null;

            Assert.IsFalse(nullObject.Equals(deck));

        }


        [TestMethod]
        public void EqualsTest008_operator_equals()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, EditProp);

            Assert.IsFalse(deck == deck2);
        }

        [TestMethod]
        public void EqualsTest009_operator_equals()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, StartProp);

            Assert.IsTrue(deck == deck2);
        }


        [TestMethod]
        public void EqualsTest009_operator_not_equals()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, StartProp);

            Assert.IsFalse(deck != deck2);
        }

        [TestMethod]
        public void EqualsTest010_operator_not_equals()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            Deck deck2 = new Deck();
            PopulateObject(deck2, EditProp);

            Assert.IsTrue(deck != deck2);
        }

        [TestMethod]
        public void EqualsTest011_comparesAllFields()
        {
            Guid guid = Guid.NewGuid(); //ensure have same guids
            Mock<ITrackerFactory> factory = new Mock<ITrackerFactory>();
            factory.Setup(f => f.GetDateTimeNow()).Returns(new DateTime(2017, 1, 3, 23, 44, 0));
            factory.Setup(f => f.GetNewGuid()).Returns(guid);

            foreach (PropertyInfo p in typeof(Deck).GetProperties())
            {
                if (p.CanWrite)
                {
                    Deck d1 = Deck.CreateNewDeck(factory.Object);
                    Deck d2 = Deck.CreateNewDeck(factory.Object);

                    d2.CopyHistory(d1.History); //ensure hist references same objects - will alwyas pass otherwise

                    TestContext.WriteLine("Checking prop:" + p.Name);

                    p.SetValue(d2, EditProp[p.PropertyType]);

                    Assert.IsFalse(d1 == d2); //should return false
                }
            }
        }

        [TestMethod()]
        public void CopyHistoryTest001_newObjects()
        {
            Deck deck = new Deck();
            deck.CreateVersion(1, 1, DateTime.Now);
            deck.SelectedVersionId = deck.CreateVersion(2, 1, DateTime.Now).VersionId;
            deck.CreateVersion(3, 1, DateTime.Now);

            Deck deck2 = new Deck();
            deck2.CopyHistory(deck.History);

            CollectionAssert.AreEqual(deck.History, deck2.History);
            //check if selected deck is in history!!
            Assert.IsTrue(deck2.History.Any(dh => dh.VersionId == deck2.SelectedVersionId));
            //selectd should be latest
            Assert.IsTrue(deck2.SelectedVersionId ==
                    deck2.History.OrderByDescending(dv => dv.Version).First().VersionId);

        }

        [TestMethod()]
        public void CopyHistoryTest002_sameHisotryDiffSelectedDeck()
        {
            Deck deck = new Deck();
            Guid d2selectd = deck.CreateVersion(1, 1, DateTime.Now).VersionId;
            deck.SelectedVersionId = deck.CreateVersion(2, 1, DateTime.Now).VersionId;
            deck.CreateVersion(3, 1, DateTime.Now);

            Deck deck2 = new Deck();
            deck2.SelectedVersionId = d2selectd;
            deck2.CopyHistory(deck.History);

            CollectionAssert.AreEqual(deck.History, deck2.History);
            Assert.AreEqual(d2selectd, deck2.SelectedVersionId);
        }

        [TestMethod()]
        public void CloneTest001()
        {
            Deck deck = new Deck();
            PopulateObject(deck, StartProp);

            deck.PropertyChanged += (s, e) => { };

            Deck clone = deck.Clone() as Deck;

            foreach (PropertyInfo p in typeof(Deck).GetProperties())
            {
                if (p.CanWrite)
                {
                    TestContext.WriteLine("Checking prop:{0}.{1};{2}", p.Name, p.GetValue(deck), p.GetValue(clone));
                    if (p.PropertyType == typeof(string))
                    {
                        //http://stackoverflow.com/questions/506648/how-do-strings-work-when-shallow-copying-something-in-c
                        continue;
                    }
                    if (p.PropertyType.GetInterface(nameof(ICollection)) != null)
                    {
                        CollectionAssert.AreNotEqual(p.GetValue(deck) as ICollection, p.GetValue(clone) as ICollection, new ReferenceComparer());
                    }
                    else
                    {
                        Assert.IsFalse(Object.ReferenceEquals(p.GetValue(deck), p.GetValue(clone)));
                    }
                }
            }

            foreach (EventInfo ev in typeof(CardInstance).GetEvents())
            {
                FieldInfo fieldTheEvent = GetAllFields(typeof(CardInstance)).Where(f => f.Name == ev.Name).First();
                Assert.IsNull(fieldTheEvent.GetValue(clone));
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete - new deck()
    }
}
