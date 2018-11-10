using ESLTracker.BusinessLogic.Games;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils.Extensions;
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
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                today,
                out actualRank, 
                out actualProgress, 
                out actualMaxStars, 
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(0, actualProgress);
            Assert.AreEqual(3, actualMaxStars);

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
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                today,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(1, actualProgress);
            Assert.AreEqual(3, actualMaxStars);

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
                .UsingDate(today.AddMinutes(10))
                .WithOutcome(2, GameOutcome.Defeat)
                .UsingDate(today.AddMinutes(20))
                .WithOutcome(1, GameOutcome.Victory)
                .UsingDate(today.AddMinutes(30))
                .WithOutcome(4, GameOutcome.Defeat)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                today,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(-2, actualProgress);
            Assert.AreEqual(3, actualMaxStars);

        }

        [TestMethod]
        public void CalculateCurrentRankProgress004_EdgeOfnextRank()
        {
            DateTime today = new DateTime(2018, 8, 5);

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheRitual)
                .UsingDate(today)
                .WithOutcome(3, GameOutcome.Victory)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                today,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(PlayerRank.TheRitual, actualRank);
            Assert.AreEqual(3, actualProgress);
            Assert.AreEqual(3, actualMaxStars);

        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "./BusinessLogic/Games/RankCalculationsTests_Data/CalculateCurrentRankProgress005_RankuUpNoGamesInNextRank.csv", "CalculateCurrentRankProgress005_RankuUpNoGamesInNextRank#csv", DataAccessMethod.Sequential)]
        [DeploymentItem("./BusinessLogic/Games/RankCalculationsTests_Data/CalculateCurrentRankProgress005_RankuUpNoGamesInNextRank.csv"
            , "./BusinessLogic/Games/RankCalculationsTests_Data/")]
        [TestMethod]
        public void CalculateCurrentRankProgress005_RankuUpNoGamesInNextRank()
        {
            DateTime today = new DateTime(2018, 8, 5);

            PlayerRank current = TestContext.DataRow["CurrentRank"].ConvertToEnum<PlayerRank>();
            int  noOfWins = TestContext.DataRow["noOfWins"].ConvertToInt();
            PlayerRank expectedRank = TestContext.DataRow["expectedRank"].ConvertToEnum<PlayerRank>();
            int expectedMaxStars = (int)TestContext.DataRow["expectedMaxStars"].ConvertToInt();

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(current)
                .UsingDate(today)
                .WithOutcome(noOfWins, GameOutcome.Victory)
                .Build();

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(today);

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                today,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(expectedRank, actualRank);
            Assert.AreEqual(0, actualProgress);
            Assert.AreEqual(expectedMaxStars, actualMaxStars);
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
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                today,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(PlayerRank.TheWarrior, actualRank);
            Assert.AreEqual(0, actualProgress);
            Assert.AreEqual(6, actualMaxStars);
        }

        [TestMethod]
        public void CalculateCurrentRankProgress006_LegendRankChange_NoRankedGamesInSession()
        {
            DateTime lastGame = new DateTime(2018, 8, 5, 10, 0,0);
            DateTime sessionStart = new DateTime(2018, 8, 5, 12, 0, 0);

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(sessionStart.AddHours(1));


            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheLegend)
                .UsingDate(lastGame)
                .WithOutcome(1, GameOutcome.Victory)
                .Build();

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                sessionStart,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.IsNull(legendStart);
            Assert.IsNull(legendMin);
            Assert.IsNull(legedmax);
            Assert.IsNull(legendCurrent);

        }

        [TestMethod]
        public void CalculateCurrentRankProgress006_LegendRankChange_OneGameInSession()
        {
            DateTime lastGame = new DateTime(2018, 8, 5, 10, 0, 0);
            DateTime sessionStart = new DateTime(2018, 8, 5, 12, 0, 0);

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(sessionStart.AddHours(1));

            int? startRank = 40;

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheLegend)
                .UsingDate(lastGame)
                .WithOutcome(1, GameOutcome.Victory)
                .UsingPlayerLegendRank(startRank)
                .UsingDate(sessionStart.AddMinutes(10))
                .WithOutcome(1, GameOutcome.Victory)
                .Build();

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                sessionStart,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(startRank, legendStart);
            Assert.AreEqual(startRank, legendMin);
            Assert.AreEqual(startRank, legedmax);
            Assert.AreEqual(startRank, legendCurrent);

        }

        [TestMethod]
        public void CalculateCurrentRankProgress006_LegendRankChange_MoreGamesInSession()
        {
            DateTime lastGame = new DateTime(2018, 8, 5, 10, 0, 0);
            DateTime sessionStart = new DateTime(2018, 8, 5, 12, 0, 0);

            mockDatetimeProvider.SetupGet(mdp => mdp.DateTimeNow).Returns(sessionStart.AddHours(1));

            int? startRank = 40;
            int? secondGameRank = 45;
            int? lastGameRank = 25;

            var games = new GameListBuilder()
                .UsingType(GameType.PlayRanked)
                .UsingPlayerRank(PlayerRank.TheLegend)
                .UsingDate(lastGame)
                .WithOutcome(1, GameOutcome.Victory)
                .UsingPlayerLegendRank(startRank)
                .UsingDate(sessionStart.AddMinutes(10))
                .WithOutcome(1, GameOutcome.Victory)
                .UsingPlayerLegendRank(secondGameRank)
                .UsingDate(sessionStart.AddMinutes(20))
                .WithOutcome(1, GameOutcome.Victory)
                .UsingPlayerLegendRank(lastGameRank)
                .UsingDate(sessionStart.AddMinutes(30))
                .WithOutcome(1, GameOutcome.Victory).Build();

            RankCalculations rankCalculations = CreateRankCalulations();

            PlayerRank actualRank;
            int actualProgress;
            int actualMaxStars;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                games,
                sessionStart,
                out actualRank,
                out actualProgress,
                out actualMaxStars,
                out legendStart,
                out legendMin,
                out legedmax,
                out legendCurrent);

            Assert.AreEqual(startRank, legendStart);
            Assert.AreEqual(lastGameRank, legendMin);
            Assert.AreEqual(secondGameRank, legedmax);
            Assert.AreEqual(lastGameRank, legendCurrent);

        }

        private RankCalculations CreateRankCalulations()
        {
            return new RankCalculations(mockDatetimeProvider.Object);
        }
    }
}
