using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils;
using Moq;
using ESLTracker.DataModel;
using ESLTrackerTests;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class DeckEditViewModelTests : BaseTest
    {
        [TestMethod]
        public void CancelEditTest001()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetNewGuid()).Returns(() => Guid.NewGuid());

            DeckEditViewModel model = new DeckEditViewModel();
            Deck deck = Deck.CreateNewDeck(trackerFactory.Object);

            model.Deck = deck;

            PopulateObject(deck, StartProp);
            //fix up selected version id - otherwise it would be some random guid
            deck.History.Last().VersionId = deck.SelectedVersionId;

            TestContext.WriteLine("Begin Edit");
            model.BeginEdit();

            PopulateObject(deck, EditProp);

            TestContext.WriteLine("Cancel Edit");
            model.CancelEdit();

            foreach (PropertyInfo p in deck.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    if (p.PropertyType.GetInterface(nameof(ICollection)) != null)
                    {
                        CollectionAssert.AreEqual(StartProp[p.PropertyType] as ICollection, p.GetValue(deck) as ICollection, "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                    else
                    {
                        Assert.AreEqual(StartProp[p.PropertyType], p.GetValue(deck), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                }
            }
        }

        [TestMethod()]
        public void EditDeckStartTest001()
        {
            Deck deck = Deck.CreateNewDeck();
            DeckEditViewModel model = new DeckEditViewModel();

            model.EditDeckStart(new Utils.Messages.EditDeck() { Deck = deck });

            Assert.AreEqual(deck, model.Deck);
            Assert.AreEqual(deck, model.savedState);
        }

        [TestMethod()]
        public void LimitCardCountForDeckTest001_Constructed()
        {
            DeckEditViewModel model = new DeckEditViewModel();

            bool actual = model.LimitCardCountForDeck(new Deck() { Type = DataModel.Enums.DeckType.Constructed });

            Assert.AreEqual(true, actual);

        }

        [TestMethod()]
        public void LimitCardCountForDeckTest001_SoloArena()
        {
            DeckEditViewModel model = new DeckEditViewModel();

            bool actual = model.LimitCardCountForDeck(new Deck() { Type = DataModel.Enums.DeckType.SoloArena });

            Assert.AreEqual(false, actual);
        }


        [TestMethod()]
        public void LimitCardCountForDeckTest001_VersusArena()
        {
            DeckEditViewModel model = new DeckEditViewModel();

            bool actual = model.LimitCardCountForDeck(new Deck() { Type = DataModel.Enums.DeckType.VersusArena });

            Assert.AreEqual(false, actual);
        }

        [TestMethod()]
        public void SaveDeckTest001_OverwriteCurrent()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            Mock<ITracker> tracker = new Mock<ITracker>();
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            tracker.Setup(t => t.Decks).Returns(deckList);

            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            CardInstance card = new CardInstance(new Card(trackerFactory.Object));

            Deck deck = Deck.CreateNewDeck(trackerFactory.Object, "test deck");
            deck.SelectedVersion.Cards.Add(card);

            //  List<CardInstance> modifiedCollection = new List<CardInstance>()
            {
                //    card, //include this and ensure it has been cloned
                deck.SelectedVersion.Cards.Add(new CardInstance(Card.Unknown));
                deck.SelectedVersion.Cards.Add(new CardInstance(new Card(trackerFactory.Object)));
            };

            DeckEditViewModel model = new DeckEditViewModel();
            model.Deck = deck;

            model.SaveDeck(tracker.Object, new SerializableVersion(0, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(1, model.Deck.History.Count);
        }

        [TestMethod()]
        public void SaveDeckTest002_SaveMajor()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            Mock<ITracker> tracker = new Mock<ITracker>();
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            tracker.Setup(t => t.Decks).Returns(deckList);

            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            CardInstance card = new CardInstance(new Card(trackerFactory.Object));

            Deck deck = Deck.CreateNewDeck(trackerFactory.Object, "test deck");
            deck.SelectedVersion.Cards.Add(card);

            DeckEditViewModel model = new DeckEditViewModel();
            model.Deck = deck;

            model.BeginEdit();

            deck.SelectedVersion.Cards.Add(new CardInstance(Card.Unknown));
            deck.SelectedVersion.Cards.Add(new CardInstance(new Card(trackerFactory.Object)));

            model.SaveDeck(tracker.Object, new SerializableVersion(1, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(new SerializableVersion(2, 0), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(1, model.Deck.History[0].Cards.Count);
            Assert.AreEqual(2, model.Deck.History.Count);
            //endsure inial card has ebeen cloned
            Assert.IsFalse(model.Deck.SelectedVersion.Cards.Contains(card));
        }

        [TestMethod()]
        public void SaveDeckTest003_SaveMinor()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            Mock<ITracker> tracker = new Mock<ITracker>();
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            tracker.Setup(t => t.Decks).Returns(deckList);

            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            CardInstance card = new CardInstance(new Card(trackerFactory.Object));

            Deck deck = Deck.CreateNewDeck(trackerFactory.Object, "test deck");
            deck.SelectedVersion.Cards.Add(card);

            DeckEditViewModel model = new DeckEditViewModel();
            model.Deck = deck;

            model.BeginEdit();

            deck.SelectedVersion.Cards.Add(new CardInstance(Card.Unknown));
            deck.SelectedVersion.Cards.Add(new CardInstance(new Card(trackerFactory.Object)));

            model.SaveDeck(tracker.Object, new SerializableVersion(0, 1), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(new SerializableVersion(1, 1), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(1, model.Deck.History[0].Cards.Count);
            Assert.AreEqual(2, model.Deck.History.Count);
            //endsure inial card has ebeen cloned
            Assert.IsFalse(model.Deck.SelectedVersion.Cards.Contains(card));
        }

        [TestMethod()]
        public void SaveDeckTest004_SaveNotLatestVersion_CrashThatVersionExists()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(DateTime.Now);
            Mock<ITracker> tracker = new Mock<ITracker>();
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            tracker.Setup(t => t.Decks).Returns(deckList);

            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            CardInstance card = new CardInstance(new Card(trackerFactory.Object));

            Deck deck = Deck.CreateNewDeck(trackerFactory.Object, "test deck");
            deck.CreateVersion(1, 3, trackerFactory.Object.GetDateTimeNow()); //ensure its not ordered :)
            Guid selectedVersion = deck.CreateVersion(1, 1, trackerFactory.Object.GetDateTimeNow()).VersionId;
            deck.CreateVersion(1, 2, trackerFactory.Object.GetDateTimeNow());

            deck.SelectedVersionId = selectedVersion; //select 1.1

            DeckEditViewModel model = new DeckEditViewModel();
            model.Deck = deck;

            model.BeginEdit();

            //tray save as 1.2
            model.SaveDeck(tracker.Object, new SerializableVersion(0, 1), deck.SelectedVersion.Cards);

            Assert.AreEqual(new SerializableVersion(1, 4), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(5, model.Deck.History.Count);
        }

        [TestMethod()]
        public void SaveDeckTest005_SaveNotLatestVersion_EnsureNewMajorHaveMinor_0()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(DateTime.Now);
            Mock<ITracker> tracker = new Mock<ITracker>();
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            tracker.Setup(t => t.Decks).Returns(deckList);

            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            CardInstance card = new CardInstance(new Card(trackerFactory.Object));

            Deck deck = Deck.CreateNewDeck(trackerFactory.Object, "test deck");
            deck.CreateVersion(1, 3, trackerFactory.Object.GetDateTimeNow()); //ensure its not ordered :)
            Guid selectedVersion = deck.CreateVersion(1, 1, trackerFactory.Object.GetDateTimeNow()).VersionId;
            deck.CreateVersion(1, 2, trackerFactory.Object.GetDateTimeNow());

            deck.SelectedVersionId = selectedVersion; //select 1.1

            DeckEditViewModel model = new DeckEditViewModel();
            model.Deck = deck;

            model.BeginEdit();

            //tray save as 1.2
            model.SaveDeck(tracker.Object, new SerializableVersion(1, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(new SerializableVersion(2, 0), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(5, model.Deck.History.Count);
        }

        [TestMethod()]
        public void CalculateDeckChangesTest001()
        {
            Card c1 = new Card() { Name = "c1", Id = Guid.NewGuid() };
            Card c2 = new Card() { Name = "c2", Id = Guid.NewGuid() };
            Card c3 = new Card() { Name = "c3", Id = Guid.NewGuid() };
            Card c4 = new Card() { Name = "c4", Id = Guid.NewGuid() };
            Card c5 = new Card() { Name = "c5", Id = Guid.NewGuid() };

            ObservableCollection<CardInstance> coll1 = new ObservableCollection<CardInstance>();
            coll1.Add(new CardInstance(c1) { Quantity = 2 });
            coll1.Add(new CardInstance(c2) { Quantity = 2 });
            coll1.Add(new CardInstance(c3) { Quantity = 3 });
            coll1.Add(new CardInstance(c4) { Quantity = 3 });

            ObservableCollection<CardInstance> coll2 = new ObservableCollection<CardInstance>();
            coll2.Add(new CardInstance(c2) { Quantity = 2 });
            coll2.Add(new CardInstance(c3) { Quantity = 1 });
            coll2.Add(new CardInstance(c4) { Quantity = 3 });
            coll2.Add(new CardInstance(c5) { Quantity = 2 });

            ObservableCollection<CardInstance> expected = new ObservableCollection<CardInstance>();
            expected.Add(new CardInstance(c1) { Quantity = 2 });
            //c2 - no changes not in a list
            expected.Add(new CardInstance(c3) { Quantity = 2 });
            //c4 - no changes not in a list
            expected.Add(new CardInstance(c5) { Quantity = -2 });

            var actual = new DeckEditViewModel().CalculateDeckChanges(coll1, coll2);
            CollectionAssert.AreEqual(expected, actual,
                Comparer<CardInstance>.Create((x, y) => x.CardId == y.CardId && x.Quantity == y.Quantity ? 0 : 1),
                "{0}Actual:{0}{1}; {0}Expected:{0}{2}",
                Environment.NewLine,
                String.Join(Environment.NewLine, actual.Select(c => c.DebuggerInfo)),
                String.Join(Environment.NewLine, expected.Select(c => c.DebuggerInfo)));

        }
    }
}