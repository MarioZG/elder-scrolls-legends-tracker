using System;
using ESLTracker.BusinessLogic.Games;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTrackerTests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESLTrackerTests.BusinessLogic.Games
{
    [TestClass]
    public class GameFactoryTests : BaseTest
    {
        [TestMethod]
        public void CreateGame001_EnsureDateIsSet()
        {
            DateTime expectedDate = new DateTime(2018, 7, 6, 12, 34, 56);

            mockDatetimeProvider.SetupGet(dtp => dtp.DateTimeNow).Returns(expectedDate);

            GameFactory gamefactory = CreateGameFactory();

            Game game = gamefactory.CreateGame();

            Assert.AreEqual(expectedDate, game.Date);
        }

        [TestMethod]
        public void CreateGame002_CopyRankedDataOnlyForRankedGames_RankedGame()
        {
            DateTime date = new DateTime(2018, 12, 6, 12, 34, 56);
            Deck deck = new DeckBuilder().Build();
            int playerLegRank = 123;
            PlayerRank playerRank = PlayerRank.TheLegend;

            Game prevgame = new GameBuilder()
                .WithDate(date)
                .WithDeck(deck)
                .WithOpponentClass(DeckClass.Battlemage)
                .WithOutcome(GameOutcome.Victory)
                .WithPlayerLegendRank(playerLegRank)
                .WithPlayerRank(playerRank)
                .WithType(GameType.PlayRanked)
                .WithBonusRound(true)
                .Build();

            mockDatetimeProvider.SetupGet(dtp => dtp.DateTimeNow).Returns(date);

            GameFactory gamefactory = CreateGameFactory();

            Game game = gamefactory.CreateGame(prevgame);

            Assert.AreEqual(date, game.Date);
            Assert.AreEqual(deck, game.Deck);
            Assert.AreEqual(null, game.OpponentClass);
            Assert.AreEqual(default(GameOutcome), game.Outcome);
            Assert.AreEqual(playerLegRank, game.PlayerLegendRank);
            Assert.AreEqual(playerRank, game.PlayerRank);
            Assert.AreEqual(GameType.PlayRanked, game.Type);
            Assert.AreEqual(false, game.BonusRound);

            Assert.AreEqual(null, game.Notes);
            Assert.AreEqual(null, game.OpponentClass);
            Assert.AreEqual(null, game.OpponentLegendRank);
            Assert.AreEqual(null, game.OpponentName);
            Assert.AreEqual(null, game.OpponentRank);
            Assert.AreEqual(null, game.OrderOfPlay);

        }

        [TestMethod]
        public void CreateGame003_CopyRankedDataOnlyForRankedGames_VsArenaGame()
        {
            DateTime date = new DateTime(2018, 12, 6, 12, 34, 56);
            Deck deck = new DeckBuilder().Build();
            int playerLegRank = 123;
            PlayerRank playerRank = PlayerRank.TheLegend;
            GameType gameType = GameType.VersusArena;

            Game prevgame = new GameBuilder()
                .WithDate(date)
                .WithDeck(deck)
                .WithOpponentClass(DeckClass.Battlemage)
                .WithOutcome(GameOutcome.Victory)
                .WithPlayerLegendRank(playerLegRank)
                .WithPlayerRank(playerRank)
                .WithType(gameType)
                .WithBonusRound(true)
                .Build();

            mockDatetimeProvider.SetupGet(dtp => dtp.DateTimeNow).Returns(date);

            GameFactory gamefactory = CreateGameFactory();

            Game game = gamefactory.CreateGame(prevgame);

            Assert.AreEqual(date, game.Date);
            Assert.AreEqual(deck, game.Deck);
            Assert.AreEqual(null, game.OpponentClass);
            Assert.AreEqual(default(GameOutcome), game.Outcome);
            Assert.AreEqual(null, game.PlayerLegendRank);
            Assert.AreEqual(null, game.PlayerRank);
            Assert.AreEqual(gameType, game.Type);
            Assert.AreEqual(default(bool?), game.BonusRound);

            Assert.AreEqual(null, game.Notes);
            Assert.AreEqual(null, game.OpponentClass);
            Assert.AreEqual(null, game.OpponentLegendRank);
            Assert.AreEqual(null, game.OpponentName);
            Assert.AreEqual(null, game.OpponentRank);
            Assert.AreEqual(null, game.OrderOfPlay);

        }

        private GameFactory CreateGameFactory()
        {
            return new GameFactory(mockDatetimeProvider.Object);
        }
    }
}
