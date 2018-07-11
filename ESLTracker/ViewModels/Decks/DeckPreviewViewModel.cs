using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.Extensions;
using System.Collections.ObjectModel;
using ESLTracker.Services;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Cards;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckPreviewViewModel : ViewModelBase
    {

        private Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set {
                deck = value;
                CurrentVersion = Deck?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        public DeckVersion currentVersion;
        public DeckVersion CurrentVersion
        {
            get
            {
                return currentVersion;
            }
            set
            {
                currentVersion = value;
                if (value != null)
                {
                    Deck.SelectedVersionId = value.VersionId;
                }
                RaisePropertyChangedEvent(nameof(CurrentVersion));
            }
        }

        public Dictionary<string, ObservableCollection<CardInstance>> ChangesHistory
        {
            get
            {
                Dictionary<string, ObservableCollection<CardInstance>> changesHist = new Dictionary<string, ObservableCollection<CardInstance>>();
                if (Deck != null)
                {
                    DeckVersion prev = null;
                    foreach (DeckVersion dv in Deck.History)
                    {
                        if (prev != null)
                        {
                            changesHist.Add(prev.Version.ToString("mm")+" -> "+ dv.Version.ToString("mm"), CalculateDeckChanges(dv.Cards, prev.Cards));
                        }
                        prev = dv;
                    }
                    changesHist = changesHist.Reverse().ToDictionary(i => i.Key, i => i.Value);
                }
                return changesHist;
            }
        }

        public ObservableCollection<DataModel.Reward> ActiveDeckRewards
        {
            get
            {
                return  new ObservableCollection<Reward>(tracker.Rewards.Where(r => r.ArenaDeckId == tracker.ActiveDeck?.DeckId));

            }
        }

        private ICardInstanceFactory cardInstanceFactory;
        private IMessenger messanger;
        private ITracker tracker;

        public DeckPreviewViewModel(ICardInstanceFactory cardInstanceFactory, IMessenger messanger, ITracker tracker)
        {
            this.cardInstanceFactory = cardInstanceFactory;
            this.messanger = messanger;
            this.tracker = tracker;

            messanger.Register<ActiveDeckChanged>(this, ActiveDeckChanged);
        }

        internal ObservableCollection<CardInstance> CalculateDeckChanges(ObservableCollection<CardInstance> cards1, ObservableCollection<CardInstance> cards2)
        {
            ObservableCollection<CardInstance> result = new ObservableCollection<CardInstance>();

            result = cards1.DeepCopy<ObservableCollection<CardInstance>, CardInstance >();
            foreach(CardInstance card in cards2)
            {
                CardInstance currentCard = result.Where(ci => ci.CardId == card.CardId).FirstOrDefault();
                if (currentCard != null)
                {
                    currentCard.Quantity -= card.Quantity;
                }
                else
                {
                    result.Add(cardInstanceFactory.CreateFromCard(card.Card, -card.Quantity));
                }
            }

            return new ObservableCollection<CardInstance>(result.Where( ci=> ci.Quantity != 0));
        }

        private void ActiveDeckChanged(ActiveDeckChanged obj)
        {
            if (obj.ActiveDeck != null) //null when filter on decklist is refreshed
            {
                Deck = obj.ActiveDeck;
                RaisePropertyChangedEvent();
            }
        }

    }
}
