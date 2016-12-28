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

namespace ESLTracker.Utils.Tests
{
    [TestClass()]
    public class CardsDatabaseTests
    {
        [TestMethod()]
        public void FindCardByNameTest001_UnknownCard()
        {
            Card expected = Card.Unknown;
            Card actual = CardsDatabase.Default.FindCardByName("some randoe strng");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LoadCardsDatabaseTest()
        {
            Assert.IsNotNull(CardsDatabase.Default.Cards);

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

        [TestMethod()]
        public void PopulateCollectionTest001()
        {
            Pack pack = new Pack();
            ObservableCollection<string> names = new ObservableCollection<string>( 
               new string[] { "Adoring Fan", "random name", String.Empty, String.Empty, String.Empty, String.Empty }
            );
            CardsDatabase.Default.PopulateCollection(names, pack.Cards);

            Assert.AreEqual(6, pack.Cards.Count);
            Assert.AreEqual("Adoring Fan", pack.Cards[0].Card.Name);
            Assert.AreEqual(Card.Unknown, pack.Cards[1].Card);
        }
    }
}