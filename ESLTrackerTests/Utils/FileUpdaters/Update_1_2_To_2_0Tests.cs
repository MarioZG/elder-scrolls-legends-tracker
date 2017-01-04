using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.FileUpdaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_1_2_To_2_0Tests
    {
        [TestMethod()]
        public void CreateInitalHistoryForExistingDecksTest001()
        {
#pragma warning disable CS0618 // Type or member is obsolete - dont want to create deck with history
            //create decks with emoty history
            Deck d1 = new Deck() { DoNotUse = null, SelectedVersionId = Guid.Empty };
            Deck d2 = new Deck() { DoNotUse = null, SelectedVersionId = Guid.Empty };
#pragma warning restore CS0618 // Type or member is obsolete

            Tracker tracker = new Tracker();
            tracker.Decks.Add(d1);
            tracker.Decks.Add(d2);


            Update_1_2_To_2_0 updater = new Update_1_2_To_2_0();

            updater.CreateInitalHistoryForExistingDecks(tracker);

            Assert.IsNotNull(d1.DoNotUse);
            Assert.IsNotNull(d1.History);
            Assert.AreNotEqual(Guid.Empty, d1.SelectedVersionId);

            Assert.IsNotNull(d2.DoNotUse);
            Assert.IsNotNull(d2.History);
            Assert.AreNotEqual(Guid.Empty, d2.SelectedVersionId);

        }
    }
}