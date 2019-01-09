using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils.FileUpdaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;
using ESLTracker.BusinessLogic.Decks;
using Moq;
using ESLTrackerTests;
using ESLTracker.Utils.SimpleInjector;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_1_2_To_2_0Tests : BaseTest
    {
        [TestMethod()]
        public void CreateInitalHistoryForExistingDecksTest001()
        {


            //create decks with emoty history
            Deck d1 = new Deck() { DoNotUse = null, SelectedVersionId = Guid.Empty };
            Deck d2 = new Deck() { DoNotUse = null, SelectedVersionId = Guid.Empty };

            Tracker tracker = new Tracker();
            tracker.Decks.Add(d1);
            tracker.Decks.Add(d2);

            new MasserContainer();
            IDeckService deckService = MasserContainer.Container.GetInstance<IDeckService>();


            Update_1_2_To_2_0 updater = new Update_1_2_To_2_0(mockLogger.Object, deckService);

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