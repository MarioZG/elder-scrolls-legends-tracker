using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.FileUpdaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.DataModel;
using System.Collections.ObjectModel;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_2_3_To_3_0Tests
    {
        [TestMethod()]
        public void SetDeckLastUsedTest001()
        {
            Mock<ITracker> tracker = new Mock<ITracker>();

            var packs = Enumerable.Range(0, 100).Select(i => new Pack());
            
            tracker.Setup(t => t.Packs).Returns(new ObservableCollection<Pack>(packs));

            Update_2_3_To_3_0 updater = new Update_2_3_To_3_0();
            updater.SetPacksToCore(tracker.Object);

            Assert.IsFalse(tracker.Object.Packs.Any( p=> p.CardSet == null));
            Assert.IsTrue(tracker.Object.Packs.All(p => p.CardSet.Name == "Core"));

        }
    }
}