﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.FileUpdaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TESLTracker.DataModel;
using System.Collections.ObjectModel;
using ESLTrackerTests.Builders;
using ESLTrackerTests;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_2_0_To_2_1Tests : BaseTest
    {
        [TestMethod()]
        public void SetDeckLastUsedTest001()
        {
            Mock<ITracker> tracker = new Mock<ITracker>();

            Deck deckNoGames = new DeckBuilder().Build();
            Deck deckWithGames = new DeckBuilder().Build();

            DateTime lastGame = new DateTime(2017, 1, 30);

            ObservableCollection<Game> games = new ObservableCollection<Game>()
            {
                new GameBuilder().WithDeck(deckWithGames).WithDate(lastGame).Build()
            };

            tracker.Setup(t => t.Games).Returns(games);
            tracker.Setup(t => t.Decks).Returns(new ObservableCollection<Deck>() { deckWithGames, deckNoGames });

            Update_2_0_To_2_1 updater = new Update_2_0_To_2_1(mockLogger.Object);
            updater.SetDeckLastUsed(tracker.Object);

            Assert.AreEqual(deckNoGames.CreatedDate, deckNoGames.LastUsed);
            Assert.AreEqual(lastGame, deckWithGames.LastUsed);
        }
    }
}