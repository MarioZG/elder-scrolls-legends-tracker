using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTrackerTests;
using System.Collections.ObjectModel;
using ESLTracker.ViewModels.Game;
using ESLTracker.DataModel.Enums;
using ESLTracker.Properties;

namespace ESLTracker.ViewModels.Tests
{
    [TestClass()]
    public class ArenaStatsViewModelTests : BaseTest
    {
        [TestMethod()]
        public void GetArenaRunStatisticsTest001_MoreThanOneRecordForSameRewardType()
        {
            Mock<ITrackerFactory> trackerfactory = new Mock<ITrackerFactory>();

            //two decks
            //d1 - 4 winn run 12g, 12g (24g in total), 34sg, 1 p, 1 card
            //d2 - another 4 winn run 18g, 20g (38g in total), 10sg ,  8sg (18sg in total), 2 p, 3 card
            //d3 - some random 1g, 2sg, 3p, 4c - just to mess data a bit
            Deck d1 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, Class = DataModel.Enums.DeckClass.Assassin };
            Deck d2 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, Class = DataModel.Enums.DeckClass.Assassin };
            Deck d3 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, Class = DataModel.Enums.DeckClass.Assassin };



            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(new System.Collections.ObjectModel.ObservableCollection<Deck>()
            {
                d1, d2, d3
            });

            tracker.Setup(t => t.Games).Returns(
                 new ObservableCollection<DataModel.Game>(
                     GenerateGamesList(d1, 4, 3, 0, 0, GameType.VersusArena).Union(
                    GenerateGamesList(d2, 4, 3, 0, 0, GameType.VersusArena))
                ));

            tracker.Setup(t => t.Rewards).Returns(
                 new ObservableCollection<DataModel.Reward>()
                     {
                     //d1 rewards
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Gold, Quantity = 12 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Gold, Quantity = 12 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 34 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Pack, Quantity = 1 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Card, Quantity = 1 },
                     //d2 rewards
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Gold, Quantity = 18 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Gold, Quantity = 20 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 10 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 8 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Pack, Quantity = 2 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Card, Quantity = 3 }
                 }.ToList()
                 );


            trackerfactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            trackerfactory.Setup(tf => tf.GetSettings()).Returns(new Mock<ISettings>().Object);

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerfactory.Object);
            model.GameType = DataModel.Enums.GameType.VersusArena;

            var result = model.GetDataSet();

            Assert.IsNotNull(result);
            Assert.AreEqual(31, result[0].Avg.Gold);
            Assert.AreEqual(26, result[0].Avg.SoulGem);
            Assert.AreEqual(1, result[0].Avg.Pack);
            Assert.AreEqual(2, result[0].Avg.Card);

            Assert.AreEqual(62, result[0].Total.Gold);
            Assert.AreEqual(52, result[0].Total.SoulGem);
            Assert.AreEqual(3, result[0].Total.Pack);
            Assert.AreEqual(4, result[0].Total.Card);

            Assert.AreEqual(38, result[0].Max.Gold);
            Assert.AreEqual(34, result[0].Max.SoulGem);
            Assert.AreEqual(2, result[0].Max.Pack);
            Assert.AreEqual(3, result[0].Max.Card);

        }

        [TestMethod()]
        public void GetArenaRunStatisticsTest002_MoreThanOneRecordForSameRewardTypeAndAddedSomeDataOutsideFilters()
        {
            Mock<ITrackerFactory> trackerfactory = new Mock<ITrackerFactory>();

            trackerfactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 11, 1, 0, 0, 1));

            //two decks
            //d1 - 4 winn run 12g, 12g (24g in total), 34sg, 1 p, 1 card
            //d2 - another 4 winn run 18g, 20g (38g in total), 10sg ,  8sg (18sg in total), 2 p, 3 card
            //d3 - some random 1g, 2sg, 3p, 4c - just to mess data a bit
            Deck d1 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, Class = DataModel.Enums.DeckClass.Assassin };
            Deck d2 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, Class = DataModel.Enums.DeckClass.Assassin };
            Deck d3 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, Class = DataModel.Enums.DeckClass.Assassin };
            //d4 - solo arena run
            Deck d4 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.SoloArena, Class = DataModel.Enums.DeckClass.Assassin };
            //d5 some old run, out of date fulter
            Deck d5 = new Deck(trackerfactory.Object) { Type = DataModel.Enums.DeckType.VersusArena, CreatedDate = new DateTime(2016, 10, 1), Class = DataModel.Enums.DeckClass.Assassin };

            Mock<ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(new System.Collections.ObjectModel.ObservableCollection<Deck>()
            {
                d1, d2, d3, d4
            });

            tracker.Setup(t => t.Games).Returns(
                 new ObservableCollection<DataModel.Game>(
                     GenerateGamesList(d1, 4, 3, 0, 0, GameType.VersusArena).Union(
                    GenerateGamesList(d2, 4, 3, 0, 0, GameType.VersusArena))
                ));


            tracker.Setup(t => t.Rewards).Returns(
                 new ObservableCollection<DataModel.Reward>()
                     {
                     //d1 rewards
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Gold, Quantity = 12 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Gold, Quantity = 12 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 34 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Pack, Quantity = 1 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d1, Type = DataModel.Enums.RewardType.Card, Quantity = 1 },
                     //d2 rewards
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Gold, Quantity = 18 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Gold, Quantity = 20 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 10 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 8 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Pack, Quantity = 2 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d2, Type = DataModel.Enums.RewardType.Card, Quantity = 3 },
                     //d4 rewards, big qrty to mess up avergae
                     new Reward(trackerfactory.Object) {ArenaDeck = d4, Type = DataModel.Enums.RewardType.Gold, Quantity = 1118 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d4, Type = DataModel.Enums.RewardType.Gold, Quantity = 1120 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d4, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 1110 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d4, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 118 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d4, Type = DataModel.Enums.RewardType.Pack, Quantity = 112 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d4, Type = DataModel.Enums.RewardType.Card, Quantity = 113 },
                     //d5 rewards, big qrty to mess up avergae - shold be incluede
                     new Reward(trackerfactory.Object) {ArenaDeck = d5, Type = DataModel.Enums.RewardType.Gold, Quantity = 1118 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d5, Type = DataModel.Enums.RewardType.Gold, Quantity = 1120 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d5, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 1110 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d5, Type = DataModel.Enums.RewardType.SoulGem, Quantity = 118 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d5, Type = DataModel.Enums.RewardType.Pack, Quantity = 112 },
                     new Reward(trackerfactory.Object) {ArenaDeck = d5, Type = DataModel.Enums.RewardType.Card, Quantity = 113 }
                 }.ToList()
                 );


            trackerfactory.Setup(tf => tf.GetTracker()).Returns(tracker.Object);

            trackerfactory.Setup(tf => tf.GetSettings()).Returns(new Mock<ISettings>().Object);

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerfactory.Object);
            model.FilterDateFrom = new DateTime(2016, 11, 1);
            model.FilterDateTo = null;
            model.GameType = DataModel.Enums.GameType.VersusArena;

            var result = model.GetDataSet();

            Assert.IsNotNull(result);
            Assert.AreEqual(31, result[0].Avg.Gold);
            Assert.AreEqual(26, result[0].Avg.SoulGem);
            Assert.AreEqual(1, result[0].Avg.Pack);
            Assert.AreEqual(2, result[0].Avg.Card);

            Assert.AreEqual(62, result[0].Total.Gold);
            Assert.AreEqual(52, result[0].Total.SoulGem);
            Assert.AreEqual(3, result[0].Total.Pack);
            Assert.AreEqual(4, result[0].Total.Card);

            Assert.AreEqual(38, result[0].Max.Gold);
            Assert.AreEqual(34, result[0].Max.SoulGem);
            Assert.AreEqual(2, result[0].Max.Pack);
            Assert.AreEqual(3, result[0].Max.Card);

        }

        [TestMethod()]
        public void SetDateFiltersTest001_All()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 3, 7));

            trackerFactory.Setup(tf => tf.GetSettings()).Returns(new Mock<ISettings>().Object);

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerFactory.Object);

            model.SetDateFilters(PredefinedDateFilter.All);

            Assert.IsNull(model.FilterDateFrom);
            Assert.IsNull(model.FilterDateTo);
        }

        [TestMethod()]
        public void SetDateFiltersTest002_PrevMonth()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 3, 7));

            trackerFactory.Setup(tf => tf.GetSettings()).Returns(new Mock<ISettings>().Object);

            DateTime expectedFrom = new DateTime(2016, 2, 1);
            DateTime expectedTo = new DateTime(2016, 2, 29);

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerFactory.Object);


            model.SetDateFilters(PredefinedDateFilter.PreviousMonth);

            Assert.AreEqual(expectedFrom, model.FilterDateFrom);
            Assert.AreEqual(expectedTo, model.FilterDateTo);
        }

        [TestMethod()]
        public void SetDateFiltersTest003_ThisMonth()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 3, 7));

            trackerFactory.Setup(tf => tf.GetSettings()).Returns(new Mock<ISettings>().Object);

            DateTime expectedFrom = new DateTime(2016, 3, 1);
            DateTime expectedTo = new DateTime(2016, 3, 31);

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerFactory.Object);


            model.SetDateFilters(PredefinedDateFilter.ThisMonth);

            Assert.AreEqual(expectedFrom, model.FilterDateFrom);
            Assert.AreEqual(expectedTo, model.FilterDateTo);
        }

        [TestMethod()]
        public void SetDateFiltersTest004_ThisWeek()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetDateTimeNow()).Returns(new DateTime(2016, 3, 7));

            trackerFactory.Setup(tf => tf.GetSettings()).Returns(new Mock<ISettings>().Object);

            DateTime expectedFrom = new DateTime(2016, 3, 1);
            DateTime expectedTo = new DateTime(2016, 3, 7);

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerFactory.Object);


            model.SetDateFilters(PredefinedDateFilter.Last7Days);

            Assert.AreEqual(expectedFrom, model.FilterDateFrom);
            Assert.AreEqual(expectedTo, model.FilterDateTo);
        }
    }
}