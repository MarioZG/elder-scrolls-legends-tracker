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
using TESLTracker.DataModel;
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
        public void FindCardsDataTest001_SampleDeck()
        {
            string data = "./BusinessLogic/Decks/Data/DeckImportTest001.txt";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new CardBuilder().Build());

            var di = CreateDeckImporter();
            di.ExecuteImport(data);

            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum( c=> c.Quantity));
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest002.txt", "./BusinessLogic/Decks/Data/")]
        public void FindCardsDataTest002_SampleDeckWithTitle()
        {
            string data = "./BusinessLogic/Decks/Data/DeckImportTest002.txt";
            string expectedName = "CVH's Reanimator Control Rage Warrior";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new Card());

            var di = CreateDeckImporter();
            di.ExecuteImport(data);


            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum(c => c.Quantity));
            Assert.AreEqual(expectedName, di.DeckName);
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest003.txt", "./BusinessLogic/Decks/Data/")]
        public void FindCardsDataTest003_SampleDeckWithTitleWithBraces()
        {
            string data = "./BusinessLogic/Decks/Data/DeckImportTest003.txt";
            string expectedName = "Control Resummon Monk (Rank 5~)";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new Card());

            var di = CreateDeckImporter();
            di.ExecuteImport(data);


            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum(c => c.Quantity));
            Assert.AreEqual(expectedName, di.DeckName);
        }

        private WebImporter CreateDeckImporter()
        {
            return new WebImporter(cardsDatabaseFactory.Object, cardInstanceFactory.Object, new DeckCardsEditor(cardInstanceFactory.Object, cardsDatabase.Object));
        }
    }
}