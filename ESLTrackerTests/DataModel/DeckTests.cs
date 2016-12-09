using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Moq;
using ESLTrackerTests;
using ESLTracker.Utils;

namespace ESLTracker.DataModel.Tests
{
    [TestClass()]
    public class DeckTests : BaseTest
    {
        [TestMethod()]
        public void DeckTest001_GuidPreservedDurinDeserialisation()
        {
            Deck d = new Deck();
            Guid original = d.DeckId;

            StringBuilder tempString = new StringBuilder();
            using (TextWriter writer = new StringWriter(tempString))
            {
                var xml = new XmlSerializer(typeof(Deck));
                xml.Serialize(writer, d);
            }

            Deck deckDeserialise;

            using (TextReader reader = new StringReader(tempString.ToString()))
            {
                var xml = new XmlSerializer(typeof(Deck));
                deckDeserialise = (Deck)xml.Deserialize(reader);
            }

            Assert.AreEqual(original, deckDeserialise.DeckId);
        }

        [TestMethod()]
        public void DeckTest002_CheckDateTimeAdded()
        {
            Deck d = new Deck();

            Assert.IsTrue(d.CreatedDate.Year > 1999);

        }

        [TestMethod()]
        public void IsArenaRunFinishedTest001_ConstructedDeck()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.Constructed };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 3, 2, 2 ));

            bool expected = false;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest002_VersusArenaLoss()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.VersusArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 5, 3));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest003_VersusArenaWin()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.VersusArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 2));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest004_SoloArenaLoss()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.SoloArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 3));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IsArenaRunFinishedTest005_SoloArenaWin()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.SoloArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 9, 2));

            bool expected = true;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest006_SoloArenaInProgress()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.SoloArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 7, 2));

            bool expected = false;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void IsArenaRunFinishedTest007_VersusArenaInPorgress()
        {
            Mock<ITracker> trackerMock = new Mock<ITracker>();
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetTracker()).Returns(trackerMock.Object);

            Deck deck = new Deck(trackerFactory.Object) { Type = Enums.DeckType.VersusArena };

            trackerMock.Setup(t => t.Games)
                .Returns(GenerateGamesList(deck, 5, 2));

            bool expected = false;
            bool result = deck.IsArenaRunFinished();

            Assert.AreEqual(expected, result);
        }

    }
}