using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.BusinessLogic.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel.Enums;
using System.Collections.ObjectModel;
using TESLTracker.DataModel;
using ESLTrackerTests;
using ESLTracker.BusinessLogic.Cards;

namespace ESLTracker.BusinessLogic.Decks.Tests
{
    [TestClass()]
    public class CardBreakdownTests : BaseTest
    {
        [TestMethod()]
        public void GetCardsColorBreakdownTest_IncludingUnknownCard()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = Card.Unknown, Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();
            IEnumerable<KeyValuePair<DeckAttribute, int>> actual = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(DeckAttribute.Neutral, actual.First().Key); //no colors!
            Assert.AreEqual(1, actual.First().Value); //no colors!

        }

        [TestMethod]
        public void DoubleCards001_Tavyar_Rayvat()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = CardsDatabase.FindCardByName("Tavyar the Knight"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Rayvat the Mage"), Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();

            var colors = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(2, colors.Count());
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Intelligence).Single().Value);
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Endurance).Single().Value);

            var count = cardBreakdown.GetTotalCount(cardCollection);

            Assert.AreEqual(1, count);

            var manaCurve = cardBreakdown.GetManaBreakdown(cardCollection);
            Assert.AreEqual(2, ManaCureGt0(manaCurve).Count());
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 4).Single().Value);
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 5).Single().Value);

            var typeBreakdown = cardBreakdown.GetCardTypeBreakdown(cardCollection);
            Assert.AreEqual(1, typeBreakdown.Count());
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Creature).Single().Value);
        }


        [TestMethod]
        public void DoubleCards002_Balliwog_Smoked()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = CardsDatabase.FindCardByName("Baliwog Tidecrawlers"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Smoked Baliwog Leg"), Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();

            var colors = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(1, colors.Count());
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Endurance).Single().Value);

            var count = cardBreakdown.GetTotalCount(cardCollection);

            Assert.AreEqual(1, count);

            var manaCurve = cardBreakdown.GetManaBreakdown(cardCollection);
            Assert.AreEqual(2, ManaCureGt0(manaCurve).Count());
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 0).Single().Value);
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 4).Single().Value);

            var typeBreakdown = cardBreakdown.GetCardTypeBreakdown(cardCollection);
            Assert.AreEqual(2, typeBreakdown.Count());
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Creature).Single().Value);
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Action).Single().Value);
        }

        [TestMethod]
        public void DoubleCards003_Manic()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Jack"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Mutation"), Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();

            var colors = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(1, colors.Count());
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Intelligence).Single().Value);

            var count = cardBreakdown.GetTotalCount(cardCollection);

            Assert.AreEqual(1, count);

            var manaCurve = cardBreakdown.GetManaBreakdown(cardCollection);
            Assert.AreEqual(2, ManaCureGt0(manaCurve).Count());
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 1).Single().Value);
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 3).Single().Value);

            var typeBreakdown = cardBreakdown.GetCardTypeBreakdown(cardCollection);
            Assert.AreEqual(2, typeBreakdown.Count());
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Creature).Single().Value);
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Action).Single().Value);
        }

        [TestMethod]
        public void DoubleCards004_Cloak_Manic_Manic()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = CardsDatabase.FindCardByName("Cloak"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Dagger"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Grummite"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Demented Grummite"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Jack"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Mutation"), Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();

            var colors = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(3, colors.Count());
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Intelligence).Single().Value);
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Strength).Single().Value);
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Neutral).Single().Value);

            var count = cardBreakdown.GetTotalCount(cardCollection);

            Assert.AreEqual(3, count);

            var manaCurve = cardBreakdown.GetManaBreakdown(cardCollection);
            Assert.AreEqual(2, ManaCureGt0(manaCurve).Count());
            Assert.AreEqual(3, manaCurve.Where(c => c.Key == 1).Single().Value);
            Assert.AreEqual(3, manaCurve.Where(c => c.Key == 3).Single().Value);

            var typeBreakdown = cardBreakdown.GetCardTypeBreakdown(cardCollection);
            Assert.AreEqual(3, typeBreakdown.Count());
            Assert.AreEqual(2, typeBreakdown.Where(c => c.Key == CardType.Creature).Single().Value);
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Item).Single().Value);
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Action).Single().Value);
        }

        [TestMethod]
        public void DoubleCards005_Felldew_Manic_Spawn()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = CardsDatabase.FindCardByName("Felldew"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Elytra Noble"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Grummite"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Demented Grummite"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Spawn Mother"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Baliwog"), Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();

            var colors = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(3, colors.Count());
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Willpower).Single().Value);
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Agility).Single().Value);
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Neutral).Single().Value);

            var count = cardBreakdown.GetTotalCount(cardCollection);

            Assert.AreEqual(3, count);

            var manaCurve = cardBreakdown.GetManaBreakdown(cardCollection);
            Assert.AreEqual(4, ManaCureGt0(manaCurve).Count());
            Assert.AreEqual(2, manaCurve.Where(c => c.Key == 2).Single().Value);
            Assert.AreEqual(2, manaCurve.Where(c => c.Key == 3).Single().Value);
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 4).Single().Value);
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 6).Single().Value);

            var typeBreakdown = cardBreakdown.GetCardTypeBreakdown(cardCollection);
            Assert.AreEqual(2, typeBreakdown.Count());
            Assert.AreEqual(3, typeBreakdown.Where(c => c.Key == CardType.Creature).Single().Value);
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Action).Single().Value);
        }

        [TestMethod]
        public void DoubleCards006_Manic_Baliwog_Tavyar()
        {
            ObservableCollection<CardInstance> cardCollection = new ObservableCollection<CardInstance>()
            {
                new CardInstance() { Card = CardsDatabase.FindCardByName("Baliwog Tidecrawlers"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Smoked Baliwog Leg"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Manic Grummite"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Demented Grummite"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Tavyar the Knight"), Quantity = 1},
                new CardInstance() { Card = CardsDatabase.FindCardByName("Rayvat the Mage"), Quantity = 1}
            };

            CardBreakdown cardBreakdown = CreateCardBreakdownObject();

            var colors = cardBreakdown.GetCardsColorBreakdown(cardCollection);

            Assert.AreEqual(3, colors.Count());
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Intelligence).Single().Value);
            Assert.AreEqual(2, colors.Where(c => c.Key == DeckAttribute.Endurance).Single().Value);
            Assert.AreEqual(1, colors.Where(c => c.Key == DeckAttribute.Neutral).Single().Value);

            var count = cardBreakdown.GetTotalCount(cardCollection);

            Assert.AreEqual(3, count);

            var manaCurve = cardBreakdown.GetManaBreakdown(cardCollection);
            Assert.AreEqual(4, ManaCureGt0(manaCurve).Count());
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 0).Single().Value);
            Assert.AreEqual(2, manaCurve.Where(c => c.Key == 3).Single().Value);
            Assert.AreEqual(2, manaCurve.Where(c => c.Key == 4).Single().Value);
            Assert.AreEqual(1, manaCurve.Where(c => c.Key == 5).Single().Value);

            var typeBreakdown = cardBreakdown.GetCardTypeBreakdown(cardCollection);
            Assert.AreEqual(2, typeBreakdown.Count());
            Assert.AreEqual(3, typeBreakdown.Where(c => c.Key == CardType.Creature).Single().Value);
            Assert.AreEqual(1, typeBreakdown.Where(c => c.Key == CardType.Action).Single().Value);
        }


        private IEnumerable<KeyValuePair<int, int>> ManaCureGt0(IEnumerable<KeyValuePair<int, int>> manaCurve)
        {
            return manaCurve.Where(c => c.Value > 0);
        }

        private CardBreakdown CreateCardBreakdownObject()
        {
            return new CardBreakdown(new DoubleCardsCalculator(CardsDatabase));
        }
    }
}