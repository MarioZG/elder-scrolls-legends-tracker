using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.Utils.SimpleInjector;
using ESLTrackerTests;
using ESLTrackerTests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace ESLTracker.Utils.FileUpdaters.Tests
{
    [TestClass()]
    public class Update_3_0_To_3_1Tests : BaseTest
    {
        [TestMethod()]
        public void UpdateDataFile_001()
        {
            //Mock<ITracker> tracker = new Mock<ITracker>();

            new MasserContainer();

            DeckService deckService = MasserContainer.Container.GetInstance<DeckService>();

            Deck d1 = new DeckBuilder().Build();
            deckService.CreateDeckVersion(d1, 1, 0, DateTime.Now);
            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation) {
                d1.SelectedVersion.Cards.Add(new CardInstanceBuilder()
                    .WithCard( new CardBuilder().WithId(Guid.Parse(trans.Key)).Build())
                    .Build());
            }
            deckService.CreateDeckVersion(d1, 1, 1, DateTime.Now);
            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation)
            {
                d1.SelectedVersion.Cards.Add(new CardInstanceBuilder()
                    .WithCard(new CardBuilder().WithId(Guid.Parse(trans.Key)).Build())
                    .Build());
            }
            deckService.CreateDeckVersion(d1, 1, 2, DateTime.Now);
            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation)
            {
                d1.SelectedVersion.Cards.Add(new CardInstanceBuilder()
                    .WithCard(new CardBuilder().WithId(Guid.Parse(trans.Key)).Build())
                    .Build());
            }

            Deck d2 = new DeckBuilder().Build();
            deckService.CreateDeckVersion(d2, 1, 0, DateTime.Now);
            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation)
            {
                d2.SelectedVersion.Cards.Add(new CardInstanceBuilder()
                    .WithCard(new CardBuilder().WithId(Guid.Parse(trans.Key)).Build())
                    .Build());
            }

            Tracker tracker = new Tracker();
            tracker.Decks.Add(d1);
            tracker.Decks.Add(d2);

            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation)
            {
                tracker.Rewards.Add(new RewardBuilder()
                        .WithCardInstance(
                            new CardInstanceBuilder()
                            .WithCard(
                                    new CardBuilder().WithId(Guid.Parse(trans.Key)).Build()
                            ).Build())
                        .WithQuantity(2)
                        .WithType(DataModel.Enums.RewardType.Card)
                        .Build());
            }

            PackBuilder packBuilder = new PackBuilder();
            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation)
            {
                packBuilder.WithCard(new CardInstanceBuilder()
                    .WithCard(new CardBuilder().WithId(Guid.Parse(trans.Key)).Build())
                    .Build());
            }

            Pack pack = packBuilder.Build();

            tracker.Packs.Add(pack);

            string dataFileName = "./data" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml";
            new FileSaver(null, null, null, null).SaveDatabase(dataFileName, tracker);


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(dataFileName);

            System.IO.File.Delete(dataFileName);
            
            Update_3_0_To_3_1 updater = new Update_3_0_To_3_1(mockLogger.Object);
            updater.UpdateCardsGuidInDecks(xmlDoc);

            string updatedXml = xmlDoc.InnerXml;

            TestContext.WriteLine(updatedXml);

            foreach (var trans in BusinessLogic.Cards.CardsDatabase.GuidTranslation)
            {
                Assert.IsFalse(updatedXml.Contains(trans.Key), $"Guid {trans.Key} still present!");
                Assert.IsTrue(updatedXml.Contains(trans.Value), $"Guid {trans.Value} missing in new file!");
                Assert.AreEqual(6, Regex.Matches(updatedXml, trans.Value).Count, $"Guid {trans.Value} lacks occurences in new file!");
            }

        }
    }
}