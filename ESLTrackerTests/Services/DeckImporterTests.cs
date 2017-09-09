using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ESLTracker.Utils;
using Moq;

namespace ESLTracker.Services.Tests
{
    [TestClass()]
    public class DeckImporterTests
    {
        [TestMethod]
        public void GetCardQtyTest001()
        {
            string line = "3 some card name wirh number 2";

            int actual = new DeckImporter().GetCardQty(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void GetCardQtyTest002()
        {
            string line = "-2 some card name wirh number 5";

            int actual = new DeckImporter().GetCardQty(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual(-2, actual);
        }

        [TestMethod]
        public void GetCardNameTest()
        {
            string line = "-2 some card name wirh number 5";

            string actual = new DeckImporter().GetCardName(line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

            Assert.AreEqual("some card name wirh number 5", actual);
        }

        [TestMethod()]
        [DeploymentItem("./Services/Data/DeckImportTest001.txt", "./Services/Data/")]
        public void FindCardsDataTest001_SampleDeck()
        {
            string data = File.ReadAllText("./Services/Data/DeckImportTest001.txt");
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();

            cardsDatabase.Setup(cb => cb.FindCardByName(It.IsAny<string>())).Returns(new DataModel.Card());
            trackerFactory.Setup(tf => tf.GetService<ICardsDatabase>()).Returns(cardsDatabase.Object);

            var di = new DeckImporter(trackerFactory.Object);
            di.Cards = new List<DataModel.CardInstance>();
            data = di.FindCardsData(data);
            di.ImportFromTextProcess(data);

            Assert.IsTrue(di.Cards.Count > 0);
        }
    }
}