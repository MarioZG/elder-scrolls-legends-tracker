using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using System.Windows;
using System.Globalization;

namespace ESLTracker.Utils.Converters.Tests
{
    [TestClass()]
    public class DeckTypeToArenaRankVisibiltyCollapsedTests
    {
        [TestMethod()]
        public void ConvertTest001_IsArenaDeckSelected()
        {
            DeckType deckType = DeckType.SoloArena;
            object param = null;

            Visibility expected = Visibility.Visible;


            DeckTypeToArenaRankVisibilty converter = new DeckTypeToArenaRankVisibilty();

            object result = converter.Convert(
                deckType,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest002_NotArenaDeck()
        {
            DeckType deckType = DeckType.Constructed;
            object param = null;

            Visibility expected = Visibility.Collapsed;


            DeckTypeToArenaRankVisibilty converter = new DeckTypeToArenaRankVisibilty();

            object result = converter.Convert(
                deckType,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest003_ArenaDeckReversed()
        {
            DeckType deckType = DeckType.SoloArena;
            object param = "NOT";

            Visibility expected = Visibility.Collapsed;

            DeckTypeToArenaRankVisibilty converter = new DeckTypeToArenaRankVisibilty();

            object result = converter.Convert(
                deckType,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ConvertTest004_NotArenaDeckReversed()
        {
            DeckType deckType = DeckType.Constructed;
            object param = "NOT";

            Visibility expected = Visibility.Visible;

            DeckTypeToArenaRankVisibilty converter = new DeckTypeToArenaRankVisibilty();

            object result = converter.Convert(
                deckType,
                null,
                param,
                CultureInfo.CurrentCulture);

            Assert.AreEqual(expected, result);
        }
    }
}