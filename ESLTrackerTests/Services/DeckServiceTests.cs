using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.Properties;
using ESLTracker.DataModel;
using System.Collections.ObjectModel;
using ESLTracker.Services;

namespace ESLTracker.Services.Tests
{
    [TestClass]
    public class DeckServiceTests
    {
        [TestMethod]
        public void CanDeleteTest001_Forbidden()
        {
            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.Forbidden);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<ISettings>()).Returns(settings.Object);

            bool expected = false;

            DeckService deckService = new DeckService(trackerFactory.Object);

            bool acctual = deckService.CanDelete(Deck.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest002_Empy_NoGamesInDeck()
        {
            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.OnlyEmpty);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<ISettings>()).Returns(settings.Object);

            bool expected = true;

            Mock<DeckService> deckService = new Mock<DeckService>(trackerFactory.Object);
            deckService.Setup(ds => ds.GetDeckGames(It.IsAny<Deck>())).Returns(new List<Game>());

            bool acctual = deckService.Object.CanDelete(Deck.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest003_Empy_DeckHasGames()
        {
            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.OnlyEmpty);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<ISettings>()).Returns(settings.Object);

            Mock<IList<Game>> games = new Mock<IList<Game>>();
            games.Setup(g => g.Count).Returns(1);

            bool expected = false;

            Mock<DeckService> deckService = new Mock<DeckService>(trackerFactory.Object);
            deckService.Setup(ds => ds.GetDeckGames(It.IsAny<Deck>())).Returns(games.Object);

            bool acctual = deckService.Object.CanDelete(Deck.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest004_Any()
        {
            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.Any);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetService<ISettings>()).Returns(settings.Object);

            bool expected = true;

            DeckService deckService = new DeckService(trackerFactory.Object);

            bool acctual = deckService.CanDelete(Deck.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }


        [TestMethod]
        public void DeleteDeckTest001_RemoveDeck()
        {
            Deck deckToDelete = Deck.CreateNewDeck();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>()
            {
                Deck.CreateNewDeck(),
                deckToDelete,
                Deck.CreateNewDeck()
            };

            ObservableCollection<Game> games = new ObservableCollection<Game>()
            {
                new Game() {Deck = deckToDelete },
                new Game() {Deck = Deck.CreateNewDeck() },
                new Game() {Deck = Deck.CreateNewDeck() },
                new Game() {Deck = Deck.CreateNewDeck() },
                new Game() {Deck = Deck.CreateNewDeck() },
            };

            List<Reward> rewards = new List<Reward>();

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.Games).Returns(games);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            DeckService deckService = new DeckService(trackerFactory.Object);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(4, games.Count);
            Assert.IsFalse(games.Any(g=> g.DeckId == null|| g.DeckId == deckToDelete.DeckId));

            Assert.AreEqual(2, decks.Count);
            Assert.IsFalse(decks.Contains(deckToDelete));
        }

        [TestMethod]
        public void DeleteDeckTest002_RemoveRewardsRef()
        {
            Deck deckToDelete = Deck.CreateNewDeck();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>()
            {
                Deck.CreateNewDeck(),
                deckToDelete,
                Deck.CreateNewDeck()
            };

            List<Reward> rewards = new List<Reward>()
            {
                new Reward() {ArenaDeck = deckToDelete },
                new Reward() {ArenaDeck = deckToDelete },
                new Reward() {ArenaDeck = deckToDelete },
            };

            ObservableCollection<Game> games = new ObservableCollection<Game>();

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.Games).Returns(games);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            DeckService deckService = new DeckService(trackerFactory.Object);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(3, rewards.Count);
            Assert.IsFalse(rewards.Any(r=> r.ArenaDeckId != null));
        }

        [TestMethod]
        public void DeleteDeckTest003_RemoveActiveDeckRef()
        {
            Deck deckToDelete = Deck.CreateNewDeck();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            List<Reward> rewards = new List<Reward>();
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.ActiveDeck).Returns(deckToDelete);
            tracker.Setup(t => t.Games).Returns(games);


            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            DeckService deckService = new DeckService(trackerFactory.Object);

            deckService.DeleteDeck(deckToDelete);

            tracker.VerifySet(t => t.ActiveDeck = null);

           // Assert.AreEqual(null, tracker.Object.ActiveDeck);
        }

        [TestMethod]
        public void DeleteDeckTest004_ActiveDeckOther()
        {
            Deck deckToDelete = Deck.CreateNewDeck();
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            List<Reward> rewards = new List<Reward>();
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            Deck activeDeck = Deck.CreateNewDeck();

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.ActiveDeck).Returns(activeDeck); //some other deck
            tracker.Setup(t => t.Games).Returns(games);

            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            DeckService deckService = new DeckService(trackerFactory.Object);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(activeDeck, tracker.Object.ActiveDeck);
        }
    }
}