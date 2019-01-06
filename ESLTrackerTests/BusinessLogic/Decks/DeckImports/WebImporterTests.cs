using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ESLTracker.Utils;
using Moq;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Cards;
using ESLTrackerTests.Builders;
using ESLTracker.DataModel;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Decks.DeckImports;

namespace ESLTrackerTests.BusinessLogic.Decks.DeckImports
{
    [TestClass()]
    public class WebImporterTests
    {
        Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();
        Mock<ICardsDatabaseFactory> cardsDatabaseFactory = new Mock<ICardsDatabaseFactory>();
        Mock<ICardInstanceFactory> cardInstanceFactory = new Mock<ICardInstanceFactory>();

        [TestInitialize]
        public void TestInitialize()
        {
            cardInstanceFactory.Setup(cif => cif.CreateFromCard(It.IsAny<Card>(), It.IsAny<int>()))
                    .Returns((Card card, int qty) => new CardInstanceFactory().CreateFromCard(card, qty));
            cardsDatabaseFactory.Setup(cdf => cdf.GetCardsDatabase()).Returns(cardsDatabase.Object);

        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest001.txt", "./BusinessLogic/Decks/Data/")]
        public async Task FindCardsDataTest001_SampleDeck()
        {
            string data = "./BusinessLogic/Decks/Data/DeckImportTest001.txt";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new CardBuilder().Build());

            var di = CreateDeckImporter();
            await di.Import(data);

            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum( c=> c.Quantity));
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest002.txt", "./BusinessLogic/Decks/Data/")]
        public async Task FindCardsDataTest002_SampleDeckWithTitle()
        {
            string data = "./BusinessLogic/Decks/Data/DeckImportTest002.txt";
            string expectedName = "CVH's Reanimator Control Rage Warrior";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new Card());

            var di = CreateDeckImporter();
            await di.Import(data);


            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum(c => c.Quantity));
            Assert.AreEqual(expectedName, di.DeckName);
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest003.txt", "./BusinessLogic/Decks/Data/")]
        public async Task FindCardsDataTest003_SampleDeckWithTitleWithBraces()
        {
            string data = "./BusinessLogic/Decks/Data/DeckImportTest003.txt";
            string expectedName = "Control Resummon Monk (Rank 5~)";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new Card());

            var di = CreateDeckImporter();
            await di.Import(data);


            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum(c => c.Quantity));
            Assert.AreEqual(expectedName, di.DeckName);
        }

        private WebImporter CreateDeckImporter()
        {
            return new WebImporter(cardsDatabaseFactory.Object, cardInstanceFactory.Object);
        }
    }
}