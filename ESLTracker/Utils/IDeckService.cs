using System.Collections.Generic;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    public interface IDeckService
    {
        bool CanDelete(Deck deck);
        void DeleteDeck(Deck deck);
        void EnforceCardLimit(CardInstance card);
        IEnumerable<Game> GetDeckGames(Deck deck);
        bool SearchString(Deck d, string searchString);
    }
}