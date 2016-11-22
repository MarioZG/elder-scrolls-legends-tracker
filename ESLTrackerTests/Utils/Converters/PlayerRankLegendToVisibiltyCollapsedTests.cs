using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using System.Globalization;
using System.Windows;

namespace ESLTracker.Utils.Converters.Tests
{
    [TestClass()]
    public class PlayerRankLegendToVisibiltyCollapsedTests
    {
        [TestMethod()]
        public void ConvertTest001_IsLegendSelected()
        {
            PlayerRank playerRank = PlayerRank.TheLegend;
            object param = null;

            Visibility expected = Visibility.Visible;


            PlayerRankLegendToVisibiltyCollapsed converter = new PlayerRankLegendToVisibiltyCollapsed();

            object result = converter.Convert(
                playerRank, 
                null,
                param, 
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest002_IsNOTLegendSelected()
        {
            PlayerRank playerRank = PlayerRank.TheRitual;
            object param = null;

            Visibility expected = Visibility.Collapsed;


            PlayerRankLegendToVisibiltyCollapsed converter = new PlayerRankLegendToVisibiltyCollapsed();

            object result = converter.Convert(
                playerRank,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest003_IsLegendSelectedReversed()
        {
            PlayerRank playerRank = PlayerRank.TheLegend;
            object param = "NOT";

            Visibility expected = Visibility.Collapsed;


            PlayerRankLegendToVisibiltyCollapsed converter = new PlayerRankLegendToVisibiltyCollapsed();

            object result = converter.Convert(
                playerRank,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest004_IsNOTLegendSelectedReversed()
        {
            PlayerRank playerRank = PlayerRank.TheMage;
            object param = "NOT";

            Visibility expected = Visibility.Visible;


            PlayerRankLegendToVisibiltyCollapsed converter = new PlayerRankLegendToVisibiltyCollapsed();

            object result = converter.Convert(
                playerRank,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }
    }
}