using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

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
                return Deck?.GetDeckGames().Count() == 0;
            }
        }

        public IDeckClassSelectorViewModel DeckClassModel { get; set; }

        IMessenger messanger;
        ITracker tracker;
        private TrackerFactory trackerFactory;

        public DeckPropertiesViewModel() : this(new TrackerFactory())
        {
        }

        internal DeckPropertiesViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            tracker = trackerFactory.GetTracker();
            messanger = trackerFactory.GetMessanger();
        }

        private void SetDeckName(DeckType newType)
        {
            switch (newType)
            {
                case DataModel.Enums.DeckType.Constructed:
                    Deck.Name = String.Empty;
                    break;
                case DataModel.Enums.DeckType.VersusArena:
                    Deck.Name = string.Format(trackerFactory.GetSettings().NewDeck_VersusArenaName, trackerFactory.GetDateTimeNow());
                    break;
                case DataModel.Enums.DeckType.SoloArena:
                    Deck.Name = string.Format(trackerFactory.GetSettings().NewDeck_SoloArenaName, trackerFactory.GetDateTimeNow());
                    break;
                default:
                    throw new NotImplementedException();
            }
            RaisePropertyChangedEvent("Deck");
        }
    }
}
