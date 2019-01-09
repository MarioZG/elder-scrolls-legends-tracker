using ESLTracker.BusinessLogic.Decks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckPropertiesViewModel : ViewModelBase
    {
        public Deck deck;
        public Deck Deck
        {
            get { return deck; }
            set
            {
                deck = value;
               // this.DeckClassModel.SelectedClass = value.Class;
                RaisePropertyChangedEvent("");
                RaisePropertyChangedEvent("CanChangeType");
            }
        }

        public DeckType? DeckType
        {
            get
            {
                return Deck?.Type;
            }
            set
            {
                Deck.Type = value.Value;
                SetDeckName(value.Value);
                RaisePropertyChangedEvent("Deck");
            }
        }

        public bool? CanChangeType
        {
            get
            {
                return deckCalculations.GetDeckGames(Deck).Count() == 0;
            }
        }

        public IEnumerable<string> DeckTagAutocomplete
        {
            get
            {
                return tracker.DeckTags;
            }
        }

        public IDeckClassSelectorViewModel DeckClassModel { get; set; }

        private readonly IMessenger messanger;
        private readonly ITracker tracker;
        private readonly ISettings settings;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly DeckCalculations deckCalculations;

        public DeckPropertiesViewModel(
            IMessenger messanger,
            ITracker tracker,
            ISettings settings,
            IDateTimeProvider dateTimeProvider,
            DeckCalculations deckCalculations)
        {
            this.tracker = tracker;
            this.messanger = messanger;
            this.settings = settings;
            this.dateTimeProvider = dateTimeProvider;
            this.deckCalculations = deckCalculations;

            messanger.Register<NewDeckTagCreated>(this, RefreshDeckTagsList);

        }

        private void RefreshDeckTagsList(NewDeckTagCreated obj)
        {
            RaisePropertyChangedEvent(nameof(DeckTagAutocomplete));
        }

        private void SetDeckName(DeckType newType)
        {
            switch (newType)
            {
                case TESLTracker.DataModel.Enums.DeckType.Constructed:
                    Deck.Name = String.Empty;
                    break;
                case TESLTracker.DataModel.Enums.DeckType.VersusArena:
                    Deck.Name = string.Format(settings.NewDeck_VersusArenaName, dateTimeProvider.DateTimeNow);
                    break;
                case TESLTracker.DataModel.Enums.DeckType.SoloArena:
                    Deck.Name = string.Format(settings.NewDeck_SoloArenaName, dateTimeProvider.DateTimeNow);
                    break;
                default:
                    throw new NotImplementedException();
            }
            RaisePropertyChangedEvent("Deck");
        }
    }
}
