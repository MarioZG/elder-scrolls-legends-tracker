using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks.DeckExports
{
    public class BBCodeExport : IDeckTextExport
    {

        public string FormatCardLine(CardInstance card)
        {
            if (card != null)
            {
                return $"{card.Quantity} [card]{card.Card.Name}[/card]";
            }
            else
            {
                return String.Empty;
            }
        }

        public string FormatDeckHeader(Deck deck)
        {
            return $"### {deck.Name} ###";
        }

        public string FormatDeckFooter(Deck deck)
        {
            return String.Empty;
        }


    }
}
