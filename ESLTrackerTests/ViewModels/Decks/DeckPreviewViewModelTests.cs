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
    [TestClass]
    public class DeckPreviewViewModelTests : BaseTest
    {
        [TestMethod]
        public void CancelEditTest001()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetNewGuid()).Returns(() => Guid.NewGuid());

            DeckPreviewViewModel model = new DeckPreviewViewModel();
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
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            model.EditDeckStart(new Utils.Messages.EditDeck() { Deck = deck });

            Assert.AreEqual(deck, model.Deck);
            Assert.AreEqual(deck, model.savedState);
            Assert.AreEqual(true, model.IsInEditMode);

        }

        [TestMethod()]
        public void GetMaxSingleCardForDeckTest001_Constructed()
        {
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            int? actual = model.GetMaxSingleCardForDeck(new Deck() { Type = DataModel.Enums.DeckType.Constructed });

            Assert.AreEqual(3, actual);

        }

        [TestMethod()]
        public void GetMaxSingleCardForDeckTest001_SoloArena()
        {
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            int? actual = model.GetMaxSingleCardForDeck(new Deck() { Type = DataModel.Enums.DeckType.SoloArena });

            Assert.AreEqual(null, actual);
        }


        [TestMethod()]
        public void GetMaxSingleCardForDeckTest001_VersusArena()
        {
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            int? actual = model.GetMaxSingleCardForDeck(new Deck() { Type = DataModel.Enums.DeckType.VersusArena });

            Assert.AreEqual(null, actual);
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

            DeckPreviewViewModel model = new DeckPreviewViewModel();
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

            DeckPreviewViewModel model = new DeckPreviewViewModel();
            model.Deck = deck;
            
            model.BeginEdit();

            deck.SelectedVersion.Cards.Add(new CardInstance(Card.Unknown));
            deck.SelectedVersion.Cards.Add(new CardInstance(new Card(trackerFactory.Object)));

            model.SaveDeck(tracker.Object, new SerializableVersion(1, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(new SerializableVersion(2,0), model.Deck.SelectedVersion.Version);
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

            DeckPreviewViewModel model = new DeckPreviewViewModel();
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
            Guid selectedVersion =  deck.CreateVersion(1, 1, trackerFactory.Object.GetDateTimeNow()).VersionId;
            deck.CreateVersion(1, 2, trackerFactory.Object.GetDateTimeNow());

            deck.SelectedVersionId = selectedVersion; //select 1.1

            DeckPreviewViewModel model = new DeckPreviewViewModel();
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

            DeckPreviewViewModel model = new DeckPreviewViewModel();
            model.Deck = deck;

            model.BeginEdit();

            //tray save as 1.2
            model.SaveDeck(tracker.Object, new SerializableVersion(1, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(new SerializableVersion(2, 0), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(5, model.Deck.History.Count);
        }
    }
}