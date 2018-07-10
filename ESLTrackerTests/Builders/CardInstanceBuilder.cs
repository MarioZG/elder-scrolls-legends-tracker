using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class CardInstanceBuilder
    {
        CardInstance cardInstance;

        public CardInstanceBuilder()
        {
            cardInstance = new CardInstance();
        }

        public CardInstanceBuilder WithCard(Card card)
        {
            cardInstance.Card = card;
            return this;
        }

        public CardInstanceBuilder WithQuantity(int quantity)
        {
            cardInstance.Quantity = quantity;
            return this;
        }

        public CardInstance Build()
        {
            return cardInstance;
        }
    }
}
