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
using ESLTracker.BusinessLogic.Decks;
using ESLTrackerTests;
using ESLTrackerTests.Builders;

namespace ESLTracker.Services.Tests
{
    [TestClass]
    public class DeckServiceTests : BaseTest
    {
        Mock<ITracker> tracker = new Mock<ITracker>();
        //Mock<IDeckVersionFactory> mockDeckVersionFactory = new Mock<IDeckVersionFactory>();

        [TestMethod]
        public void CanDeleteTest001_Forbidden()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.Forbidden);

            bool expected = false;

            DeckService deckService = CreateDeckService();

            bool acctual = deckService.CanDelete(deckService.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }



        [TestMethod]
        public void CanDeleteTest002_Empy_NoGamesInDeck()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.OnlyEmpty);
            tracker.SetupGet(t => t.Games).Returns(new ObservableCollection<Game>());

            bool expected = true;

            var deckService = CreateDeckService();

            bool acctual = deckService.CanDelete(deckService.CreateNewDeck());

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest003_Empy_DeckHasGames()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.OnlyEmpty);

            Deck deck = new DeckBuilder().Build();

            ObservableCollection<Game> games = new GameListBuilder()
                                                .UsingDeck(deck)
                                                .WithOutcome(1, DataModel.Enums.GameOutcome.Victory)
                                                .Build();
            tracker.Setup(t => t.Games).Returns(games);


            bool expected = false;

            DeckService deckService = CreateDeckService();

            bool acctual = deckService.CanDelete(deck);

            Assert.AreEqual(expected, acctual);
        }

        [TestMethod]
        public void CanDeleteTest004_Any()
        {
            mockSettings.Setup(s => s.DeckDeleteMode).Returns(ViewModels.Decks.DeckDeleteMode.Any);


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
                .WithOutcome(1, DataModel.Enums.GameOutcome.Victory)
                .UsingDeck(new DeckBuilder().Build())
                .WithOutcome(2, DataModel.Enums.GameOutcome.Victory)
                .UsingDeck(new DeckBuilder().Build())
                .WithOutcome(2, DataModel.Enums.GameOutcome.Victory)
                .Build();

            List<Reward> rewards = new List<Reward>();

            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.Games).Returns(games);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(4, games.Count);
            Assert.IsFalse(games.Any(g=> g.DeckId == null|| g.DeckId == deckToDelete.DeckId));

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

            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.Games).Returns(games);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(3, rewards.Count);
            Assert.IsFalse(rewards.Any(r=> r.ArenaDeckId != null));
        }

        [TestMethod]
        public void DeleteDeckTest003_RemoveActiveDeckRef()
        {
            DeckService deckService = CreateDeckService();

            Deck deckToDelete = deckService.CreateNewDeck();

            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            List<Reward> rewards = new List<Reward>();
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.Games).Returns(games);
            tracker.SetupGet(t => t.ActiveDeck).Returns(deckToDelete);

            deckService.DeleteDeck(deckToDelete);

            tracker.VerifySet(t => t.ActiveDeck = null);

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

            tracker.Setup(t => t.Decks).Returns(decks);
            tracker.Setup(t => t.Rewards).Returns(rewards);
            tracker.Setup(t => t.ActiveDeck).Returns(activeDeck); //some other deck
            tracker.Setup(t => t.Games).Returns(games);

            deckService.DeleteDeck(deckToDelete);

            Assert.AreEqual(activeDeck, tracker.Object.ActiveDeck);
        }

        private DeckService CreateDeckService()
        {
            return new DeckService(tracker.Object, mockSettings.Object, mockDatetimeProvider.Object, mockGuidProvider.Object);
        }
    }
}