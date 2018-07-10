using ESLTracker.BusinessLogic.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckLastGamesIndicatorViewModel : ViewModelBase
    {
        private readonly DeckCalculations deckCalculations;

        public DeckLastGamesIndicatorViewModel(DeckCalculations deckCalculations)
        {
            this.deckCalculations = deckCalculations;
        }

        public IEnumerable<bool> GetLastGames(DataModel.Deck deck, int gamesCount)
        {
            IEnumerable<bool> wins = deckCalculations.GetDeckGames(deck)
                .OrderByDescending(g => g.Date)
                .Take(gamesCount)
                .Reverse()
                .Select(g => g.Outcome == DataModel.Enums.GameOutcome.Victory)
                .ToList();

            return wins;
        }
    }
}
