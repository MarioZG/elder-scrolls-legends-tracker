using ESLTracker.BusinessLogic.Games;
using ESLTracker.DataModel.Enums;
using ESLTrackerTests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.BusinessLogic.Games
{
    [TestClass]
    public class RankCalculationsTests : BaseTest
    {
        [TestMethod]
        public void CalculateCurrentRankProgress001_NoGamesAtAll()
        {
            DateTime today = new DateTime(2018, 8, 5);

            var games = new GameListBuilder().Build(); //no games at all

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            rankCalculations.CalculateCurrentRankProgress(games, out actualRank, out actualProgress, out actualMaxStars);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(0, actualProgress);
            Assert.AreEqual(4, actualMaxStars);

        }

        [TestMethod]
        public void CalculateCurrentRankProgress002_OneGame_Victory()
        {
            DateTime today = new DateTime(2018, 8, 5);

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheRitual)
                .UsingDate(today)
                .WithOutcome(1, GameOutcome.Victory)
                .Build(); 

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            rankCalculations.CalculateCurrentRankProgress(games, out actualRank, out actualProgress, out actualMaxStars);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(1, actualProgress);
            Assert.AreEqual(4, actualMaxStars);

        }

        [TestMethod]
        public void CalculateCurrentRankProgress003_Rank12UpAndDown()
        {
            DateTime today = new DateTime(2018, 8, 5);

            //1 win, 2 looses, 1 win, 4 looses
            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheRitual)
                .UsingDate(today)
                .WithOutcome(1, GameOutcome.Victory)
                .WithOutcome(2, GameOutcome.Defeat)
                .WithOutcome(1, GameOutcome.Victory)
                .WithOutcome(4, GameOutcome.Defeat)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            rankCalculations.CalculateCurrentRankProgress(games, out actualRank, out actualProgress, out actualMaxStars);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(-1, actualProgress);
            Assert.AreEqual(4, actualMaxStars);

        }

        [TestMethod]
        public void CalculateCurrentRankProgress004_EdgeOfnextRank()
        {
            DateTime today = new DateTime(2018, 8, 5);

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheRitual)
                .UsingDate(today)
                .WithOutcome(4, GameOutcome.Victory)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            rankCalculations.CalculateCurrentRankProgress(games, out actualRank, out actualProgress, out actualMaxStars);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(4, actualProgress);
            Assert.AreEqual(4, actualMaxStars);

        }


        [TestMethod]
        public void CalculateCurrentRankProgress005_RankuUpNoGamesInNextRank()
        {
            DateTime today = new DateTime(2018, 8, 5);

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheRitual)
                .UsingDate(today)
                .WithOutcome(5, GameOutcome.Victory)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            rankCalculations.CalculateCurrentRankProgress(games, out actualRank, out actualProgress, out actualMaxStars);

            Assert.AreEqual(PlayerRank.TheLover, actualRank);
            Assert.AreEqual(0, actualProgress);
            Assert.AreEqual(4, actualMaxStars);
        }

        [TestMethod]
        public void CalculateCurrentRankProgress006_SeasonReset()
        {
            DateTime lastSeasonGames = new DateTime(2018, 7, 25);
            DateTime today = new DateTime(2018, 8, 5);

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheTower)
                .UsingDate(lastSeasonGames)
                .WithOutcome(1, GameOutcome.Victory)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            rankCalculations.CalculateCurrentRankProgress(games, out actualRank, out actualProgress, out actualMaxStars);

            Assert.AreEqual(PlayerRank.TheWarrior, actualRank);
            Assert.AreEqual(0, actualProgress);
            Assert.AreEqual(7, actualMaxStars);
        }

        private RankCalculations CreateRankCalulations()
        {
            return new RankCalculations(mockDatetimeProvider.Object);
        }
    }
}
