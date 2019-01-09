using TESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using TESLTracker.DataModel.Enums;
using TESLTracker.Utils;

namespace ESLTracker.BusinessLogic.Decks
{
    public class DeckService : IDeckService
    {
        private ITracker tracker;
        ISettings settings;
        IDateTimeProvider dateTimeProvider;
        IGuidProvider guidProvider;

        public DeckService(
            ITracker tracker, 
            ISettings settings, 
            IDateTimeProvider dateTimeProvider,
            IGuidProvider guidProvider
          )
        {
            this.tracker = tracker;
            this.settings = settings;
            this.dateTimeProvider = dateTimeProvider;
            this.guidProvider = guidProvider;
        }

        public bool LimitCardCountForDeck(Deck deckToCheck)
        {
            return deckToCheck?.Type == DeckType.Constructed;
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
            return tracker.Games.Where(g => g.Deck.DeckId == deck.DeckId);
        }

        public bool CanDelete(Deck deck)
        {
            if (deck == null)
            {
                return false;
            }
            DeckDeleteMode deleteMode = settings.DeckDeleteMode;
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

        public Deck CreateNewDeck(string deckName = "")
        {
#pragma warning disable CS0618 // Type or member is obsolete
            //factory - call allowed
            Deck deck = new Deck();
#pragma warning restore CS0618 // Type or member is obsolete
            deck.DeckId = guidProvider.GetNewGuid();
            deck.CreatedDate = dateTimeProvider.DateTimeNow;
            CreateDeckVersion(deck, 1, 0, dateTimeProvider.DateTimeNow);
            deck.Name = deckName;
            return deck;
        }

        public DeckVersion CreateDeckVersion()
        {
            return new DeckVersion()
            {
                VersionId = guidProvider.GetNewGuid()
            };
        }

        /// <summary>
        /// Creates new deck version in history, adds to colletion and returns reference
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public DeckVersion CreateDeckVersion(Deck deck, int major, int minor, DateTime createdDate)
        {
            SerializableVersion version = new SerializableVersion(major, minor);
            if (deck.DoNotUse.Any(v => v.Version == version))
            {
                throw new ArgumentException(string.Format("Version {0} alread has been added to deck '{1}'", version, deck.Name));
            }
            DeckVersion dv = new DeckVersion();
            dv.VersionId = guidProvider.GetNewGuid();
            dv.CreatedDate = createdDate;
            dv.Version = version;
            deck.DoNotUse.Add(dv); //add to history
            deck.SelectedVersionId = dv.VersionId;
            return dv;
        }

        public bool CanExport(Deck deck)
        {
            return deck != null && deck.SelectedVersion != null;
        }
    }
}
