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
using TESLTracker.DataModel.Enums;

namespace ESLTrackerTests.BusinessLogic.Decks.DeckImports
{
    [TestClass()]
    public class TextImporterTests
    {
        Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();
        Mock<ICardsDatabaseFactory> cardsDatabaseFactory = new Mock<ICardsDatabaseFactory>();
        Mock<ICardInstanceFactory> cardInstanceFactory = new Mock<ICardInstanceFactory>();
        Mock<IDeckService> mockDeckService = new Mock<IDeckService>();

        [TestInitialize]
        public void TestInitialize()
        {
            cardInstanceFactory.Setup(cif => cif.CreateFromCard(It.IsAny<Card>(), It.IsAny<int>()))
                    .Returns((Card card, int qty) => new CardInstanceFactory().CreateFromCard(card, qty));
            cardsDatabaseFactory.Setup(cdf => cdf.GetCardsDatabase()).Returns(cardsDatabase.Object);

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

        private TextImporter CreateDeckImporter()
        {
            return new TextImporter(cardsDatabaseFactory.Object, cardInstanceFactory.Object, mockDeckService.Object, 
                new DeckCardsEditor(cardInstanceFactory.Object, cardsDatabase.Object));
        }
    }
}