using ESLTracker.BusinessLogic.Cards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public class DeckCardsEditor
    {

        private readonly ICardInstanceFactory cardInstanceFactory;
        private readonly ICardsDatabase cardsDatabase;

        public DeckCardsEditor(ICardInstanceFactory cardInstanceFactory, ICardsDatabase cardsDatabase)
        {
            this.cardInstanceFactory = cardInstanceFactory;
            this.cardsDatabase = cardsDatabase;
        }

        public void ChangeCardQuantity(ICollection<CardInstance> cardSet, string cardName, int quantity, bool limitCardsQty)
        {
            Card card = cardsDatabase.FindCardByName(cardName);

            ChangeCardQuantity(cardSet, card, quantity, limitCardsQty);
        }

        public void ChangeCardQuantity(ICollection<CardInstance> cardSet, Card card, int qtyDelta, bool limitCardsQty)
        {
            if ((card.DoubleCardComponents != null)
                && (card.DoubleCardComponents.Count() > 0))
            {
                //this is double card
                foreach (var c in card.DoubleCardComponents)
                {
                    var doubleCardComponent = cardsDatabase.FindCardById(c);
                    ChangeCardQuantityInternal(cardSet, doubleCardComponent, qtyDelta, limitCardsQty);
                }
            }
            else if (card.DoubleCard.GetValueOrDefault() != Guid.Empty)
            {
                //this is part of double card
                var doubleCard = cardsDatabase.FindCardById(card.DoubleCard.Value);
                foreach (var c in doubleCard.DoubleCardComponents)
                {
                    var doubleCardComponent = cardsDatabase.FindCardById(c);
                    ChangeCardQuantityInternal(cardSet, doubleCardComponent, qtyDelta, limitCardsQty);
                }
            }
            else
            {
                ChangeCardQuantityInternal(cardSet, card, qtyDelta, limitCardsQty);
            }
        }

        private void ChangeCardQuantityInternal(ICollection<CardInstance> cardSet, Card card, int qtyDelta, bool limitCardsQty)
        {
            var cardInDeck = cardSet.Where(ci => ci.CardId == card.Id).FirstOrDefault();
            if (cardInDeck != null)
            {
                cardInDeck.Quantity += qtyDelta; //if already in deck, inc qty
                if (cardInDeck.Quantity == 0)
                {
                    cardSet.Remove(cardInDeck);
                }
            }
            else if (qtyDelta > 0)
            {
                //if adding and not yet in deck
                cardInDeck = cardInstanceFactory.CreateFromCard(card, qtyDelta);
                cardInDeck.Quantity = qtyDelta;
                cardSet.Add(cardInDeck);
            }
            else
            {
                //do nothng - removing cad not in collection
            }
            if (limitCardsQty)
            {
                EnforceCardLimit(cardInDeck);
            }
        }


        public void EnforceCardLimit(CardInstance card)
        {
            if (card.Card.IsUnique && (card.Quantity > 1))
            {
                card.Quantity = 1;
            }
            else if ((!card.Card.IsUnique) && (card.Quantity > 3))
            {
                card.Quantity = 3;
            }
        }
    }
}
