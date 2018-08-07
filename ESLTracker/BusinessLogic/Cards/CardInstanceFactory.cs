using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Cards
{
    public class CardInstanceFactory : ICardInstanceFactory
    {
        public CardInstance CreateEmpty()
        {
            return new CardInstance();
        }

        public CardInstance CreateFromCard(Card card)
        {
            var cardInstance = new CardInstance();
            cardInstance.Card = card;
            return cardInstance;
        }

        public CardInstance CreateFromCard(Card card, int quantity)
        {
            var cardInstance = new CardInstance();
            cardInstance.Card = card;
            cardInstance.Quantity = quantity;
            return cardInstance;
        }
    }
}
