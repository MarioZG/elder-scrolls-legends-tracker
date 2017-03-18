using System.Collections.Generic;
using ESLTracker.DataModel;

namespace ESLTracker.Services
{
    public interface IDeckService
    {
        bool CanDelete(Deck deck);
        void DeleteDeck(Deck deck);
        void EnforceCardLimit(CardInstance card);
        IEnumerable<Game> GetDeckGames(Deck deck);
        bool SearchString(Deck d, string searchString);
        bool CommandHideDeckCanExecute(Deck deck);
        bool CommandUnHideDeckCanExecute(Deck deck);
    }
}