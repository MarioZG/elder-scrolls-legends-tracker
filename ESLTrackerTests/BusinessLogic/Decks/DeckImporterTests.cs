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

namespace ESLTrackerTests.BusinessLogic.Decks
{
    [TestClass()]
    public class DeckImporterTests
    {
        Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();
        Mock<ICardInstanceFactory> cardInstanceFactory = new Mock<ICardInstanceFactory>();

        [TestInitialize]
        public void TestInitialize()
        {
            cardInstanceFactory.Setup(cif => cif.CreateFromCard(It.IsAny<Card>(), It.IsAny<int>()))
                    .Returns((Card card, int qty) => new CardInstanceFactory().CreateFromCard(card, qty));
        }

        [TestMethod]
        public void GetCardQtyTest001()
        {
            string line = "3 some card name wirh number 2";

            int actual = CreateDeckImporter().GetCardQty(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void GetCardQtyTest002()
        {
            string line = "-2 some card name wirh number 5";

            int actual = CreateDeckImporter().GetCardQty(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual(-2, actual);
        }

        [TestMethod]
        public void GetCardNameTest()
        {
            string line = "-2 some card name wirh number 5";

            string actual = CreateDeckImporter().GetCardName(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual("some card name wirh number 5", actual);
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest001.txt", "./BusinessLogic/Decks/Data/")]
        public void FindCardsDataTest001_SampleDeck()
        {
            string data = File.ReadAllText("./BusinessLogic/Decks/Data/DeckImportTest001.txt");

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new CardBuilder().Build());

            var di = CreateDeckImporter();
            di.Cards = new List<CardInstance>();
            data = di.FindCardsData(data);
            di.ImportFromTextProcess(data);

            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum( c=> c.Quantity));
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest002.txt", "./BusinessLogic/Decks/Data/")]
        public void FindCardsDataTest002_SampleDeckWithTitle()
        {
            string data = File.ReadAllText("./BusinessLogic/Decks/Data/DeckImportTest002.txt");
            string expectedName = "CVH's Reanimator Control Rage Warrior";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new Card());

            var di = CreateDeckImporter();
            di.Cards = new List<CardInstance>();
            data = di.FindCardsData(data);
            di.ImportFromTextProcess(data);


            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum(c => c.Quantity));
            Assert.AreEqual(expectedName, di.DeckName);
        }

        [TestMethod()]
        [DeploymentItem("./BusinessLogic/Decks/Data/DeckImportTest003.txt", "./BusinessLogic/Decks/Data/")]
        public void FindCardsDataTest003_SampleDeckWithTitleWithBraces()
        {
            string data = File.ReadAllText("./BusinessLogic/Decks/Data/DeckImportTest003.txt");
            string expectedName = "Control Resummon Monk (Rank 5~)";

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new Card());

            var di = CreateDeckImporter();
            di.Cards = new List<CardInstance>();
            data = di.FindCardsData(data);
            di.ImportFromTextProcess(data);


            Assert.IsTrue(di.Cards.Count > 0);
            Assert.AreEqual(50, di.Cards.Sum(c => c.Quantity));
            Assert.AreEqual(expectedName, di.DeckName);
        }

        private DeckImporter CreateDeckImporter()
        {
            return new DeckImporter(cardsDatabase.Object, cardInstanceFactory.Object);
        }
    }
}