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
using System.Xml;
using System.Text.RegularExpressions;
using ESLTrackerTests;
using ESLTracker.Services;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_3_0_To_3_1Tests : BaseTest
    {
        [TestMethod()]
        public void UpdateDataFile_001()
        {
            //Mock<ITracker> tracker = new Mock<ITracker>();

            Deck d1 = new Deck();
            d1.CreateVersion(1, 0, DateTime.Now);
            foreach (var trans in CardsDatabase.GuidTranslation) {
                d1.SelectedVersion.Cards.Add(new CardInstance(new Card() { Id = Guid.Parse(trans.Key) }));
            }
            d1.CreateVersion(1, 1, DateTime.Now);
            foreach (var trans in CardsDatabase.GuidTranslation)
            {
                d1.SelectedVersion.Cards.Add(new CardInstance(new Card() { Id = Guid.Parse(trans.Key) }));
            }
            d1.CreateVersion(1, 2, DateTime.Now);
            foreach (var trans in CardsDatabase.GuidTranslation)
            {
                d1.SelectedVersion.Cards.Add(new CardInstance(new Card() { Id = Guid.Parse(trans.Key) }));
            }

            Deck d2 = new Deck();
            d2.CreateVersion(1, 0, DateTime.Now);
            foreach (var trans in CardsDatabase.GuidTranslation)
            {
                d2.SelectedVersion.Cards.Add(new CardInstance(new Card() { Id = Guid.Parse(trans.Key) }));
            }

            Tracker tracker = new Tracker();
            tracker.Decks.Add(d1);
            tracker.Decks.Add(d2);

            foreach (var trans in CardsDatabase.GuidTranslation)
            {
                tracker.Rewards.Add(new Reward() { CardInstance = new CardInstance(new Card() { Id = Guid.Parse(trans.Key) }), Quantity = 2, Type = DataModel.Enums.RewardType.Card });
            }

            Pack pack = new Pack();
            foreach (var trans in CardsDatabase.GuidTranslation)
            {
                pack.Cards.Add(new CardInstance(new Card() { Id = Guid.Parse(trans.Key) }));
            }

            tracker.Packs.Add(pack);

            string dataFileName = "./data" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml";
            new FileManager().SaveDatabase(dataFileName, tracker);


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataFileName);

            System.IO.File.Delete(dataFileName);
            
            Update_3_0_To_3_1 updater = new Update_3_0_To_3_1();
            updater.UpdateCardsGuidInDecks(xmlDoc);

            string updatedXml = xmlDoc.InnerXml;

            TestContext.WriteLine(updatedXml);

            foreach (var trans in CardsDatabase.GuidTranslation)
            {
                Assert.IsFalse(updatedXml.Contains(trans.Key), $"Guid {trans.Key} still present!");
                Assert.IsTrue(updatedXml.Contains(trans.Value), $"Guid {trans.Value} missing in new file!");
                Assert.AreEqual(6, Regex.Matches(updatedXml, trans.Value).Count, $"Guid {trans.Value} lacks occurences in new file!");
            }

        }
    }
}