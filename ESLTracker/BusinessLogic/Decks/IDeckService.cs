using System;
using System.Collections.Generic;
using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public interface IDeckService
    {
        bool CanDelete(Deck deck);
        void DeleteDeck(Deck deck);
        bool LimitCardCountForDeck(Deck deckToCheck);
        void EnforceCardLimit(CardInstance card);
        IEnumerable<Game> GetDeckGames(Deck deck);
        bool SearchString(Deck d, string searchString);
        bool CommandHideDeckCanExecute(Deck deck);
        bool CommandUnHideDeckCanExecute(Deck deck);
        Deck CreateNewDeck(string deckName = "");

        DeckVersion CreateDeckVersion();
        DeckVersion CreateDeckVersion(Deck deck, int major, int minor, DateTime createdDate);
        bool CanExport(Deck deck);
    }
}