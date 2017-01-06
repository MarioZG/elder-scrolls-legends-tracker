using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Cards
{
    public class CardListEditorViewModel : ViewModelBase
    {

        private CardInstance newCard;
        public CardInstance NewCard
        {
            get { return newCard; }
            set
            {
                newCard = value;
                AddCard(value, 1);
                RaisePropertyChangedEvent(nameof(NewCard));
            }
        }

        private ObservableCollection<CardInstance> cardsCollection;

        public ObservableCollection<CardInstance> CardsCollection
        {
            get { return cardsCollection; }
            set { cardsCollection = value; RaisePropertyChangedEvent(nameof(CardsCollection)); }
        }

        public IEnumerable<string> CardNamesList
        {
            get
            {
                return trackerFactory.GetCardsDatabase().CardsNames;
            }
        }

        private ITrackerFactory trackerFactory;

        public CardListEditorViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public CardListEditorViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
        }

        internal void AddCard(CardInstance value, int qty)
        {
            var card = CardsCollection.Where(ci => ci.CardId == value.CardId).FirstOrDefault();
            if (card != null)
            {
                card.Quantity += qty; //if already in deck, inc qty
            }
            else
            {
                value.Quantity = qty;
                CardsCollection.Add(value);
            }
        }
    }
}
