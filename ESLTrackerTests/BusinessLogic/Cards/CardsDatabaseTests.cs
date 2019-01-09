using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using System.Collections.ObjectModel;
using ESLTrackerTests;
using System.Reflection;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker;

namespace ESLTrackerTests.BusinessLogic.Cards
{
    [TestClass]
    [DeploymentItem("./Resources/cards.json", "./Resources/")]
    public class CardsDatabaseTests : BaseTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            string s = System.IO.Packaging.PackUriHelper.UriSchemePack;
            System.Windows.Application.ResourceAssembly = Assembly.GetAssembly(typeof(App));
        }

        [TestMethod]
        public void FindCardByNameTest001_UnknownCard()
        {
            Card expected = Card.Unknown;
            Card actual = CardsDatabase.FindCardByName("some randoe strng");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LoadCardsDatabaseTest()
        {
            Assert.IsNotNull(CardsDatabase.Cards);

        }

        [TestMethod]
        public void DeserialiseCard001_StandardCard()
        {
            string json = @"[{""name"":""Abecean Navigator"",
                              ""rarity"":""Common"",
                              ""set"": ""Dark Brotherhood"",  
                              ""isunique"":""true"",
                              ""type"":""creature"",
                              ""attributes"":""intelligence, willpower"",
                              ""cost"":""2"",
                              ""attack"":""3"",
                              ""health"":""1"",
                              ""race"":""highelf"",
                              ""text"":""Card text""}]";
            Card card = SerializationHelper.DeserializeJson<IEnumerable<Card>>(json).First();

            Assert.AreEqual(CardRarity.Common, card.Rarity);
            Assert.AreEqual(true, card.IsUnique);
            Assert.AreEqual(CardType.Creature, card.Type);
            Assert.AreEqual(2, card.Cost);
            CollectionAssert.AreEqual(new DeckAttributes() { DeckAttribute.Intelligence, DeckAttribute.Willpower }, card.Attributes);
            Assert.AreEqual(3, card.Attack);
            Assert.AreEqual(1, card.Health);
            Assert.AreEqual("highelf", card.Race);
            Assert.AreEqual("Card text", card.Text);
            Assert.AreEqual("Dark Brotherhood", card.Set);
        }

        [TestMethod]
        public void DeserialiseCardSet001_StandardCardSet()
        {
            Guid g = Guid.NewGuid();
            string json = @"[{""name"":""Heros Of testing"",
                              ""id"":"""+g+@""",
                              ""haspacks"": ""true""}]";
            var cardSet = SerializationHelper.DeserializeJson<IEnumerable<CardSet>>(json).First();

            Assert.AreEqual("Heros Of testing", cardSet.Name);
            Assert.AreEqual(true, cardSet.HasPacks);
            Assert.AreEqual(g, cardSet.Id);
        }

        [TestMethod]
        public void DeserialiseCard002_NoKeywords()
        {
            string json = @"[{""name"":""Abecean Navigator"",
                              ""rarity"":""Common"",
                              ""set"": ""Dark Brotherhood"",  
                              ""isunique"":""true"",
                              ""type"":""creature"",
                              ""attributes"":""intelligence, willpower"",
                              ""cost"":""2"",
                              ""attack"":""3"",
                              ""health"":""1"",
                              ""race"":""highelf"",
                              ""keywords"":[],
                              ""text"":""Card text""}]";
            Card card = SerializationHelper.DeserializeJson<IEnumerable<Card>>(json).First();

            Assert.IsNotNull(card.Keywords);
            Assert.AreEqual(0, card.Keywords.Count);
        }

        [TestMethod]
        public void DeserialiseCard003_TwoKeywords()
        {
            string json = @"[{""name"":""Abecean Navigator"",
                              ""rarity"":""Common"",
                              ""set"": ""Dark Brotherhood"",  
                              ""isunique"":""true"",
                              ""type"":""creature"",
                              ""attributes"":""intelligence, willpower"",
                              ""cost"":""2"",
                              ""attack"":""3"",
                              ""health"":""1"",
                              ""race"":""highelf"",
                              ""keywords"":[""Charge"", ""Drain""],
                              ""text"":""Card text""}]";
            Card card = SerializationHelper.DeserializeJson<IEnumerable<Card>>(json).First();

            Assert.IsNotNull(card.Keywords);
            Assert.AreEqual(2, card.Keywords.Count);
            CollectionAssert.Contains(card.Keywords, CardKeyword.Charge);
            CollectionAssert.Contains(card.Keywords, CardKeyword.Drain);
        }
    }
}