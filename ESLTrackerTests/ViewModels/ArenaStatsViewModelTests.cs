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



            Mock< ITracker> tracker = new Mock<ITracker>();
            tracker.Setup(t => t.Decks).Returns(new System.Collections.ObjectModel.ObservableCollection<Deck>()
            {
                d1, d2, d3
            });

            tracker.Setup(t => t.Games).Returns(
                 new ObservableCollection<DataModel.Game>(
                     GenerateGamesList(d1, 4, 3).Union(
                    GenerateGamesList(d2, 4, 3))
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

            ArenaStatsViewModel model = new ArenaStatsViewModel(trackerfactory.Object);

            var result = model.GetArenaRunStatistics();

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
    }
}