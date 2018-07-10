using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTrackerTests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.BusinessLogic.Decks
{
    [TestClass]
    public class DeckCalculationsTests
    {
        [TestMethod()]
        public void IsArenaRunFinishedTest001_ConstructedDeck()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();

            Deck deck = new DeckBuilder().WithType(DeckType.Constructed).Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                .UsingDeck(deck)
                .WithOutcome(7, GameOutcome.Victory)
                .WithOutcome(3, GameOutcome.Defeat)
                .WithOutcome(2, GameOutcome.Disconnect)
                .WithOutcome(2, GameOutcome.Draw)
                .Build());

            bool expected = false;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest002_VersusArenaLoss()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();

            Deck deck = new DeckBuilder()
                        .WithType(DeckType.VersusArena)
                        .Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                        .UsingDeck(deck)
                        .WithOutcome(5, GameOutcome.Victory)
                        .WithOutcome(3, GameOutcome.Defeat)
                        .Build()
                );

            bool expected = true;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest003_VersusArenaWin()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();

            Deck deck = new DeckBuilder()
                        .WithType(DeckType.VersusArena)
                        .Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                        .UsingDeck(deck)
                        .WithOutcome(7, GameOutcome.Victory)
                        .WithOutcome(2, GameOutcome.Defeat)
                        .Build()
                );

            bool expected = true;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest004_SoloArenaLoss()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();

            Deck deck = new DeckBuilder()
                        .WithType(DeckType.SoloArena)
                        .Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                        .UsingDeck(deck)
                        .WithOutcome(7, GameOutcome.Victory)
                        .WithOutcome(3, GameOutcome.Defeat)
                        .Build()
                );

            bool expected = true;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest005_SoloArenaWin()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();


            Deck deck = new DeckBuilder()
                        .WithType(DeckType.SoloArena)
                        .Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                        .UsingDeck(deck)
                        .WithOutcome(9, GameOutcome.Victory)
                        .WithOutcome(2, GameOutcome.Defeat)
                        .Build()
                );

            bool expected = true;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest006_SoloArenaInProgress()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();

            Deck deck = new DeckBuilder()
                        .WithType(DeckType.SoloArena)
                        .Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                        .UsingDeck(deck)
                        .WithOutcome(7, GameOutcome.Victory)
                        .WithOutcome(2, GameOutcome.Defeat)
                        .Build()
                );

            bool expected = false;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest007_VersusArenaInPorgress()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();


            Deck deck = new DeckBuilder()
                        .WithType(DeckType.VersusArena)
                        .Build();

            trackerMock.Setup(t => t.Games)
                .Returns(new GameListBuilder()
                        .UsingDeck(deck)
                        .WithOutcome(5, GameOutcome.Victory)
                        .WithOutcome(2, GameOutcome.Defeat)
                        .Build()
                );

            bool expected = false;

            DeckCalculations deckCalculations = CreateDeckCalculations(trackerMock);

            bool result = deckCalculations.IsArenaRunFinished(deck);

            Assert.AreEqual(expected, result);
        }

        private static DeckCalculations CreateDeckCalculations(Mock<ITracker> trackerMock)
        {
            return new DeckCalculations(trackerMock.Object);
        }

    }
}
