using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System.Collections.ObjectModel;
using ESLTrackerTests;
using System.Reflection;
using ESLTracker.Services;

namespace ESLTracker.Utils.Tests
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
            Card actual = CardsDatabase.Default.FindCardByName("some randoe strng");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LoadCardsDatabaseTest()
        {
            Assert.IsNotNull(CardsDatabase.Default.Cards);

        }

        [TestMethod]
        public void CardsDatabaseTest_EnsureAllCardsHaveImages()
        {
            ResourcesService resService = new ResourcesService();
            foreach (Card card in CardsDatabase.Default.Cards)
            {
                Assert.IsNotNull(new CardInstance(card).BackgroundColor, card.Name);
                Assert.IsNotNull(new CardInstance(card).ForegroundColor, card.Name);
                Assert.IsNotNull(new CardInstance(card).RarityColor, card.Name);

                Uri imageUri = new Uri(card.ImageName, UriKind.RelativeOrAbsolute);
                Assert.IsTrue(resService.ResourceExists(imageUri), card.Name);
            }

        }

        [TestMethod]
        public void DeserialiseCard001_StandardCard()
        {
            string json = @"[{""name"":""Abecean Navigator"",
                              ""rarity"":""Common"",
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
        }
    }
}