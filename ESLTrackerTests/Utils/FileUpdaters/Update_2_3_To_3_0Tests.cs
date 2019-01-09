using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.FileUpdaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TESLTracker.DataModel;
using System.Collections.ObjectModel;
using ESLTracker.BusinessLogic.Cards;
using ESLTrackerTests.Builders;
using ESLTrackerTests;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_2_3_To_3_0Tests : BaseTest
    {
        [TestMethod()]
        public void SetDeckLastUsedTest001()
        {
            Mock<ITracker> tracker = new Mock<ITracker>();
            Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();
            cardsDatabase.SetupGet(cd => cd.CardSets).Returns(new List<CardSet>()
            {
                new CardSet() { Name  = "Core"},
                new CardSet() { Name  = "other set"},
            });

            var packs = Enumerable.Range(0, 100).Select(i => new PackBuilder().Build());
            
            tracker.Setup(t => t.Packs).Returns(new ObservableCollection<Pack>(packs));

            Update_2_3_To_3_0 updater = new Update_2_3_To_3_0(mockLogger.Object, cardsDatabase.Object);
            updater.SetPacksToCore(tracker.Object);

            Assert.IsFalse(tracker.Object.Packs.Any( p=> p.CardSet == null));
            Assert.IsTrue(tracker.Object.Packs.All(p => p.CardSet.Name == "Core"));

        }
    }
}