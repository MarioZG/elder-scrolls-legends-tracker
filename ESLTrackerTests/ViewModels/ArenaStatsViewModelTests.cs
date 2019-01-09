using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTrackerTests;
using System.Collections.ObjectModel;
using ESLTracker.ViewModels.Game;
using TESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.ViewModels.Enums;
using ESLTrackerTests.Builders;
using ESLTracker.ViewModels.Windows;

namespace ESLTracker.ViewModels.Tests
{
    [TestClass()]
    public class ArenaStatsViewModelTests : BaseTest
    {
        Mock<ITracker> tracker = new Mock<ITracker>();

        [TestMethod()]
        public void GetArenaRunStatisticsTest001_MoreThanOneRecordForSameRewardType()
        {

            //two decks
            //d1 - 4 winn run 12g, 12g (24g in total), 34sg, 1 p, 1 card
            //d2 - another 4 winn run 18g, 20g (38g in total), 10sg ,  8sg (18sg in total), 2 p, 3 card
            //d3 - some random 1g, 2sg, 3p, 4c - just to mess data a bit
            Deck d1 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();
            Deck d2 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();
            Deck d3 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();

            tracker.Setup(t => t.Decks).Returns(new ObservableCollection<Deck>()
            {
                d1, d2, d3
            });

            tracker.Setup(t => t.Games).Returns(
                new GameListBuilder()
                    .UsingType(GameType.VersusArena)
                    .UsingDeck(d1)
                    .WithOutcome(4, GameOutcome.Victory)
                    .WithOutcome(3, GameOutcome.Defeat)
                    .UsingDeck(d2)
                    .WithOutcome(4, GameOutcome.Victory)
                    .WithOutcome(3, GameOutcome.Defeat)
                    .Build()
                );

            tracker.Setup(t => t.Rewards).Returns(
                 new ObservableCollection<Reward>()
                     {
                     //d1 rewards
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Gold).WithQuantity(12).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Gold).WithQuantity(12).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.SoulGem).WithQuantity(34).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Pack).WithQuantity(1).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Card).WithQuantity(1).Build(),
                     //d2 rewards
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Gold).WithQuantity(18).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Gold).WithQuantity(20).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.SoulGem).WithQuantity(10).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.SoulGem).WithQuantity(8).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Pack).WithQuantity(2).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Card).WithQuantity(3).Build(),
                 }.ToList()
                 );

            ArenaStatsViewModel model = CreateArenaStatsVM();
            model.GameType = GameType.VersusArena;

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

        private ArenaStatsViewModel CreateArenaStatsVM()
        {
            return new ArenaStatsViewModel(mockSettings.Object, mockDatetimeProvider.Object, tracker.Object, new BusinessLogic.Decks.DeckCalculations(tracker.Object));
        }

        [TestMethod()]
        public void GetArenaRunStatisticsTest002_MoreThanOneRecordForSameRewardTypeAndAddedSomeDataOutsideFilters()
        {
            //two decks
            //d1 - 4 winn run 12g, 12g (24g in total), 34sg, 1 p, 1 card
            //d2 - another 4 winn run 18g, 20g (38g in total), 10sg ,  8sg (18sg in total), 2 p, 3 card
            //d3 - some random 1g, 2sg, 3p, 4c - just to mess data a bit
            Deck d1 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();
            Deck d2 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();
            Deck d3 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();
            //d4 - solo arena run
            Deck d4 = new DeckBuilder().WithType(DeckType.SoloArena).WithClass(DeckClass.Assassin).Build();
            //d5 some old run, out of date fulter
            Deck d5 = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).WithCreatedDate(new DateTime(2016, 10, 1)).Build();

            tracker.Setup(t => t.Decks).Returns(new System.Collections.ObjectModel.ObservableCollection<Deck>()
            {
                d1, d2, d3, d4, d5
            });

            tracker.Setup(t => t.Games).Returns(
                new GameListBuilder()
                    .UsingType(GameType.VersusArena)
                    .UsingDeck(d1)
                    .WithOutcome(4, GameOutcome.Victory)
                    .WithOutcome(3, GameOutcome.Defeat)
                    .UsingDeck(d2)
                    .WithOutcome(4, GameOutcome.Victory)
                    .WithOutcome(3, GameOutcome.Defeat)
                    .Build()
                );


            tracker.Setup(t => t.Rewards).Returns(
                 new ObservableCollection<Reward>()
                     {
                     //d1 rewards
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Gold).WithQuantity(12).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Gold).WithQuantity(12).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.SoulGem).WithQuantity(34).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Pack).WithQuantity(1).Build(),
                     new RewardBuilder().WithDeck(d1).WithType(RewardType.Card).WithQuantity(1).Build(),
                     //d2 rewards
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Gold).WithQuantity(18).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Gold).WithQuantity(20).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.SoulGem).WithQuantity(10).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.SoulGem).WithQuantity(8).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Pack).WithQuantity(2).Build(),
                     new RewardBuilder().WithDeck(d2).WithType(RewardType.Card).WithQuantity(3).Build(),
                     //d4 rewards, big qrty to mess up avergae
                     new RewardBuilder().WithDeck(d4).WithType(RewardType.Gold).WithQuantity(1118).Build(),
                     new RewardBuilder().WithDeck(d4).WithType(RewardType.Gold).WithQuantity(1120).Build(),
                     new RewardBuilder().WithDeck(d4).WithType(RewardType.SoulGem).WithQuantity(1110).Build(),
                     new RewardBuilder().WithDeck(d4).WithType(RewardType.SoulGem).WithQuantity(118).Build(),
                     new RewardBuilder().WithDeck(d4).WithType(RewardType.Pack).WithQuantity(112).Build(),
                     new RewardBuilder().WithDeck(d4).WithType(RewardType.Card).WithQuantity(113).Build(),
                     //d5 rewards, big qrty to mess up avergae - shold be incluede
                     new RewardBuilder().WithDeck(d5).WithType(RewardType.Gold).WithQuantity(1118).Build(),
                     new RewardBuilder().WithDeck(d5).WithType(RewardType.Gold).WithQuantity(1120).Build(),
                     new RewardBuilder().WithDeck(d5).WithType(RewardType.SoulGem).WithQuantity(1110).Build(),
                     new RewardBuilder().WithDeck(d5).WithType(RewardType.SoulGem).WithQuantity(118).Build(),
                     new RewardBuilder().WithDeck(d5).WithType(RewardType.Pack).WithQuantity(112).Build(),
                     new RewardBuilder().WithDeck(d5).WithType(RewardType.Card).WithQuantity(113).Build(),
                 }.ToList()
                 );

            ArenaStatsViewModel model = CreateArenaStatsVM();
            model.FilterDateFrom = new DateTime(2016, 11, 1);
            model.FilterDateTo = DateTime.Today.Date;
            model.GameType = GameType.VersusArena;

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
            mockDatetimeProvider.Setup(tf => tf.DateTimeNow).Returns(new DateTime(2016, 3, 7));

            ArenaStatsViewModel model = CreateArenaStatsVM();

            model.SetDateFilters(PredefinedDateFilter.All);

            Assert.AreEqual(new DateTime(2016, 1, 1) , model.FilterDateFrom);
            Assert.AreEqual(DateTime.Now.Date, model.FilterDateTo);
        }

        [TestMethod()]
        public void SetDateFiltersTest002_PrevMonth()
        {
            mockDatetimeProvider.Setup(tf => tf.DateTimeNow).Returns(new DateTime(2016, 3, 7));
            
            DateTime expectedFrom = new DateTime(2016, 2, 1);
            DateTime expectedTo = new DateTime(2016, 2, 29);

            ArenaStatsViewModel model = CreateArenaStatsVM();


            model.SetDateFilters(PredefinedDateFilter.PreviousMonth);

            Assert.AreEqual(expectedFrom, model.FilterDateFrom);
            Assert.AreEqual(expectedTo, model.FilterDateTo);
        }

        [TestMethod()]
        public void SetDateFiltersTest003_ThisMonth()
        {
            mockDatetimeProvider.Setup(tf => tf.DateTimeNow).Returns(new DateTime(2016, 3, 7));

            DateTime expectedFrom = new DateTime(2016, 3, 1);
            DateTime expectedTo = new DateTime(2016, 3, 31);

            ArenaStatsViewModel model = CreateArenaStatsVM();


            model.SetDateFilters(PredefinedDateFilter.ThisMonth);

            Assert.AreEqual(expectedFrom, model.FilterDateFrom);
            Assert.AreEqual(expectedTo, model.FilterDateTo);
        }

        [TestMethod()]
        public void SetDateFiltersTest004_ThisWeek()
        {
            mockDatetimeProvider.Setup(tf => tf.DateTimeNow).Returns(new DateTime(2016, 3, 7));

            DateTime expectedFrom = new DateTime(2016, 3, 1);
            DateTime expectedTo = new DateTime(2016, 3, 7);

            ArenaStatsViewModel model = CreateArenaStatsVM();


            model.SetDateFilters(PredefinedDateFilter.Last7Days);

            Assert.AreEqual(expectedFrom, model.FilterDateFrom);
            Assert.AreEqual(expectedTo, model.FilterDateTo);
        }
    }
}