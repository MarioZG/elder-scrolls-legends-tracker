using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    class DeckHelper
    {
        internal static void EnforceCardLimit(CardInstance card)
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
