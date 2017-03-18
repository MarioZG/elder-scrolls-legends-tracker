using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Services
{
    public class DeckService : IDeckService
    {
        private ITrackerFactory trackerFactory;

        public DeckService(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
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

        public virtual IEnumerable<Game> GetDeckGames(Deck deck)
        {
            return trackerFactory.GetTracker().Games.Where(g => g.Deck.DeckId == deck.DeckId);
        }

        public bool CanDelete(Deck deck)
        {
            if (deck == null)
            {
                return false;
            }
            DeckDeleteMode deleteMode = trackerFactory.GetService<ISettings>().DeckDeleteMode;
            switch (deleteMode)
            {
                case DeckDeleteMode.Forbidden:
                    return false;
                case DeckDeleteMode.OnlyEmpty:
                    return GetDeckGames(deck).Count() == 0;
                case DeckDeleteMode.Any:
                    return true;
                default:
                    throw new NotImplementedException("Unknown delete mode");
            }
        }

        public void DeleteDeck(Deck deck)
        {
            ITracker tracker = trackerFactory.GetTracker();
            if (tracker.Decks.Contains(deck))
            {
                tracker.Decks.Remove(deck);
            }

            foreach (Game g in tracker.Games.Where(g => g.DeckId == deck.DeckId).ToList())
            {
                tracker.Games.Remove(g);
            }


            foreach (Reward r in tracker.Rewards.Where(r => r.ArenaDeckId == deck.DeckId))
            {
                r.ArenaDeck = null;
                r.ArenaDeckId = null;
            }


            if (tracker.ActiveDeck?.DeckId == deck.DeckId)
            {
                tracker.ActiveDeck = null;
            }
        }

        public bool SearchString(Deck d, string searchString)
        {
            bool ret = d.Name.ToLowerInvariant().Contains(searchString.ToLowerInvariant());
            if (ret)
            {
                return ret;
            }
            ret = d.SelectedVersion.Cards.Any(c => c.Card.Name.ToLowerInvariant().Contains(searchString.ToLowerInvariant()));
            return ret;
        }

        public bool CommandHideDeckCanExecute(Deck deck)
        {
            if (deck != null)
            {
                return !deck.IsHidden;
            }
            else
            {
                return false;
            }
        }

        public bool CommandUnHideDeckCanExecute(Deck deck)
        {
            if (deck != null)
            {
                return deck.IsHidden;
            }
            else
            {
                return false;
            }
        }

    }
}
