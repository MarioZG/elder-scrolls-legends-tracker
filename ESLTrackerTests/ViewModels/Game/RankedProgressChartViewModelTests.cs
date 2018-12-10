using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTrackerTests.Builders;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Game.Tests
{
    [TestClass()]
    public class RankedProgressChartViewModelTests
    {
        [TestMethod()]
        public void GetChartDataTest()
        {
            DateTime date = new DateTime(2016, 12, 1);
            int minutes = 1;
            int legendRankValue = RankedProgressChartViewModel.GetPlayerRankStartValue(DataModel.Enums.PlayerRank.TheLegend);
            List<DataModel.Game> games = new List<DataModel.Game>()
            {
                new GameBuilder().WithPlayerLegendRank(300).WithDate(date.AddMinutes(minutes++)).WithType(GameType.PlayRanked).WithPlayerRank(PlayerRank.TheLegend).Build(),
                new GameBuilder().WithPlayerLegendRank(245).WithDate(date.AddMinutes(minutes++)).WithType(GameType.PlayRanked).WithPlayerRank(PlayerRank.TheLegend).Build(),
                new GameBuilder().WithPlayerLegendRank(423).WithDate(date.AddMinutes(minutes++)).WithType(GameType.PlayRanked).WithPlayerRank(PlayerRank.TheLegend).Build(),
                new GameBuilder().WithPlayerLegendRank(12 ).WithDate(date.AddMinutes(minutes++)).WithType(GameType.PlayRanked).WithPlayerRank(PlayerRank.TheLegend).Build(),
                new GameBuilder().WithPlayerLegendRank(76 ).WithDate(date.AddMinutes(minutes++)).WithType(GameType.PlayRanked).WithPlayerRank(PlayerRank.TheLegend).Build(),
                new GameBuilder().WithPlayerLegendRank(150).WithDate(date.AddMinutes(minutes++)).WithType(GameType.PlayRanked).WithPlayerRank(PlayerRank.TheLegend).Build(),
            };
            int index = 0;
            Dictionary<string, Tuple<DateTime, int>> chartDatagameAfterGame;
            Dictionary<string, Tuple<DateTime, int>> expectedChartDatagameAfterGame = new Dictionary<string, Tuple<DateTime, int>>()
            {
                { index.ToString(), new Tuple<DateTime, int>(games[index++].Date, legendRankValue + 423 - 300) },
                { index.ToString(), new Tuple<DateTime, int>(games[index++].Date, legendRankValue + 423 - 245) },
                { index.ToString(), new Tuple<DateTime, int>(games[index++].Date, legendRankValue + 423 - 423) },
                { index.ToString(), new Tuple<DateTime, int>(games[index++].Date, legendRankValue + 423 - 12) },
                { index.ToString(), new Tuple<DateTime, int>(games[index++].Date, legendRankValue + 423 - 76) },
                { index.ToString(), new Tuple<DateTime, int>(games[index++].Date, legendRankValue + 423 - 150) },
            };

            Dictionary<DateTime, Tuple<int, int, int>> chartDataMaxMin;

            RankedProgressChartViewModel.GetChartData(
                games.OrderBy(g=> date), 
                out chartDatagameAfterGame, 
                out chartDataMaxMin);

            Assert.IsNotNull(chartDatagameAfterGame);
            CollectionAssert.AreEqual(expectedChartDatagameAfterGame.Values, chartDatagameAfterGame.Values,
                Comparer<Tuple<DateTime, int>>.Create((x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2 ? 0 : -1)
                );
        }
    }
}