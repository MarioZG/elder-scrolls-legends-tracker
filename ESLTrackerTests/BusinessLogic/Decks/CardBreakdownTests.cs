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

namespace ESLTracker.BusinessLogic.Decks.Tests
{
    [TestClass()]
    public class CardBreakdownTests
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

        private CardBreakdown CreateCardBreakdownObject()
        {
            return new CardBreakdown();
        }
    }
}