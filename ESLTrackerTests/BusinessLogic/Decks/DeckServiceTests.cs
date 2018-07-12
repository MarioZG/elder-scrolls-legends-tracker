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
using ESLTracker.BusinessLogic.Decks;
using ESLTrackerTests.Builders;
using ESLTracker.DataModel.Enums;
using System.Collections.ObjectModel;
using ESLTracker.ViewModels.Decks;

namespace ESLTrackerTests.BusinessLogic.Decks
{
    [TestClass()]
    public class DeckServiceTests : BaseTest
    {
        Mock<ITracker> mockTracker = new Mock<ITracker>();
        //Mock<IDeckVersionFactory> mockDeckVersionFactory = new Mock<IDeckVersionFactory>();

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

            DeckService deckService = CreateDeckService();

            Deck deck = deckService.CreateNewDeck();

            Assert.IsTrue(deck.CreatedDate.Year > 1999);

        }

        [TestMethod()]
        public void DeckTest003_CheckHistoryCreated()
        {
            DeckService deckService = CreateDeckService();

            Deck deck = deckService.CreateNewDeck();

            Assert.IsNotNull(deck.History);
            Assert.AreEqual(1, deck.History.Count);
            Assert.AreEqual(deck.History.First().VersionId, deck.SelectedVersionId);
        }


        [TestMethod()]
        public void DeckTest003_HistoryDeserialised()
        {
            DateTime dateTime_1_1 = new DateTime(2016, 12, 22);

            DeckService deckService = CreateDeckService();

            Deck deck = new DeckBuilder()
                .WithVersion(new DeckVersionBuilder().WithVersion(1, 1).WithDate(dateTime_1_1).Build())
                .WithVersion(new DeckVersionBuilder().WithVersion(1, 2).WithDate(dateTime_1_1).Build())
                .Build();

            string xml = SerializationHelper.SerializeXML(deck);

            Deck deserialised = SerializationHelper.DeserializeXML<Deck>(xml);

            Assert.AreEqual(deck, deserialised, "Deck is different after deserialisation");

        }

        

        [TestMethod()]
        public void CreateVersionTest001_CreateVersion()
        {
            DateTime date = new DateTime(2017, 1, 4);
            int major = 1;
            int minor = 2;

            Deck deck = new DeckBuilder().Build();

            int expectedCount = deck.History.Count + 1;

            DeckVersion dv = CreateDeckService().CreateDeckVersion(deck, major, minor, date);

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

            Deck deck = new DeckBuilder()
                        .WithVersion(new DeckVersionBuilder().WithVersion(major, minor).WithDate(date).Build())
                        .Build();

            DeckService deckVersionFactory = CreateDeckService();

            deckVersionFactory.CreateDeckVersion(deck, major, minor, date);

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
            mockDatetimeProvider.Setup(f => f.DateTimeNow).Returns(new DateTime(2017, 1, 3, 23, 44, 0));
            mockGuidProvider.Setup(f => f.GetNewGuid()).Returns(guid);

            foreach (PropertyInfo p in typeof(Deck).GetProperties())
            {
                if (p.CanWrite)
                {
                    Deck d1 = new DeckBuilder().WithDefaultVersion().Build();
                    Deck d2 = new DeckBuilder().WithDefaultVersion().Build();

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
            Deck deck = new DeckBuilder()
                .WithVersion(new DeckVersionBuilder().WithVersion(1, 1).Build())
                .WithSelectedVersion(new DeckVersionBuilder().WithVersion(2, 1).Build())
                .WithVersion(new DeckVersionBuilder().WithVersion(3, 1).Build())
                .Build();

            Deck deck2 = new DeckBuilder().Build();

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

            DeckVersion d2selectd = new DeckVersionBuilder().WithVersion(1, 1).Build();

            Deck deck = new DeckBuilder()
                .WithVersion(d2selectd)
                .WithSelectedVersion(new DeckVersionBuilder().WithVersion(2, 1).Build())
                .WithVersion(new DeckVersionBuilder().WithVersion(3, 1).Build())
                .Build();

            Deck deck2 = new DeckBuilder()
                .WithSelectedVersion(d2selectd)
                .Build();

            deck2.CopyHistory(deck.History);

            CollectionAssert.AreEqual(deck.History, deck2.History);
            Assert.AreEqual(d2selectd.VersionId, deck2.SelectedVersionId);
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

        [TestMethod]
        public void CanDeleteTest001_Forbidden()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(DeckDeleteMode.Forbidden);

            bool expected = false;

            DeckService deckService = CreateDeckService();

            bool acctual = deckService.CanDelete(deckService.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }



        [TestMethod]
        public void CanDeleteTest002_Empy_NoGamesInDeck()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(DeckDeleteMode.OnlyEmpty);
            mockTracker.SetupGet(t => t.Games).Returns(new ObservableCollection<Game>());

            bool expected = true;

            var deckService = CreateDeckService();

            bool acctual = deckService.CanDelete(deckService.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest003_Empy_DeckHasGames()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(DeckDeleteMode.OnlyEmpty);

            Deck deck = new DeckBuilder().Build();

            ObservableCollection<Game> games = new GameListBuilder()
                                                .UsingDeck(deck)
                                                .WithOutcome(1, GameOutcome.Victory)
                                                .Build();
            mockTracker.Setup(t => t.Games).Returns(games);


            bool expected = false;

            DeckService deckService = CreateDeckService();

            bool acctual = deckService.CanDelete(deck);

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest004_Any()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(DeckDeleteMode.Any);


            bool expected = true;

            DeckService deckService = CreateDeckService();

            bool actual = deckService.CanDelete(deckService.CreateNewDeck());

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void DeleteDeckTest001_RemoveDeck()
        {
            DeckService deckService = CreateDeckService();

            Deck deckToDelete = new DeckBuilder().Build();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>()
            {
                new DeckBuilder().Build(),
                deckToDelete,
                new DeckBuilder().Build()
            };

            ObservableCollection<Game> games = new GameListBuilder()
                .UsingDeck(deckToDelete)
                .WithOutcome(1, GameOutcome.Victory)
                .UsingDeck(new DeckBuilder().Build())
                .WithOutcome(2, GameOutcome.Victory)
                .UsingDeck(new DeckBuilder().Build())
                .WithOutcome(2, GameOutcome.Victory)
                .Build();

            List<Reward> rewards = new List<Reward>();

            mockTracker.Setup(t => t.Decks).Returns(decks);
            mockTracker.Setup(t => t.Rewards).Returns(rewards);
            mockTracker.Setup(t => t.Games).Returns(games);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(4, games.Count);
            Assert.IsFalse(games.Any(g => g.DeckId == null || g.DeckId == deckToDelete.DeckId));

            Assert.AreEqual(2, decks.Count);
            Assert.IsFalse(decks.Contains(deckToDelete));
        }

        [TestMethod]
        public void DeleteDeckTest002_RemoveRewardsRef()
        {
            DeckService deckService = CreateDeckService();

            Deck deckToDelete = deckService.CreateNewDeck();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>()
            {
                deckService.CreateNewDeck(),
                deckToDelete,
                deckService.CreateNewDeck()
            };

            List<Reward> rewards = new List<Reward>()
            {
                new RewardBuilder().WithDeck(deckToDelete).Build(),
                new RewardBuilder().WithDeck(deckToDelete).Build(),
                new RewardBuilder().WithDeck(deckToDelete).Build(),
            };

            ObservableCollection<Game> games = new ObservableCollection<Game>();

            mockTracker.Setup(t => t.Decks).Returns(decks);
            mockTracker.Setup(t => t.Rewards).Returns(rewards);
            mockTracker.Setup(t => t.Games).Returns(games);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(3, rewards.Count);
            Assert.IsFalse(rewards.Any(r => r.ArenaDeckId != null));
        }

        [TestMethod]
        public void DeleteDeckTest003_RemoveActiveDeckRef()
        {
            DeckService deckService = CreateDeckService();

            Deck deckToDelete = deckService.CreateNewDeck();

            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            List<Reward> rewards = new List<Reward>();
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            mockTracker.Setup(t => t.Decks).Returns(decks);
            mockTracker.Setup(t => t.Rewards).Returns(rewards);
            mockTracker.Setup(t => t.Games).Returns(games);
            mockTracker.SetupGet(t => t.ActiveDeck).Returns(deckToDelete);

            deckService.DeleteDeck(deckToDelete);

            mockTracker.VerifySet(t => t.ActiveDeck = null);

            // Assert.AreEqual(null, tracker.Object.ActiveDeck);
        }

        [TestMethod]
        public void DeleteDeckTest004_ActiveDeckOther()
        {
            DeckService deckService = CreateDeckService();

            Deck deckToDelete = deckService.CreateNewDeck();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            List<Reward> rewards = new List<Reward>();
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            Deck activeDeck = deckService.CreateNewDeck();

            mockTracker.Setup(t => t.Decks).Returns(decks);
            mockTracker.Setup(t => t.Rewards).Returns(rewards);
            mockTracker.Setup(t => t.ActiveDeck).Returns(activeDeck); //some other deck
            mockTracker.Setup(t => t.Games).Returns(games);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(activeDeck, mockTracker.Object.ActiveDeck);
        }

        private DeckService CreateDeckService()
        {
            return new DeckService(mockTracker.Object, mockSettings.Object, mockDatetimeProvider.Object, mockGuidProvider.Object);
        }

    }
}
