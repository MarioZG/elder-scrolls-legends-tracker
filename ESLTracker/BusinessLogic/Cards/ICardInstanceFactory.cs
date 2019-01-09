using TESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Cards
{
    public interface ICardInstanceFactory
    {
        CardInstance CreateFromCard(Card card);
        CardInstance CreateFromCard(Card card, int quantity);
        CardInstance CreateEmpty();
    }
}
