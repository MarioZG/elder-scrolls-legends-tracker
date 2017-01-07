using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public ICommand CommandAddCardToDeck
        {
            get
            {
                return new RelayCommand(CommandAddCardToDeckExecute);
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

        private void CommandAddCardToDeckExecute(object obj)
        {
            int qty = Int32.Parse(obj.ToString());
            AddCard(newCard, qty);
        }

        internal void AddCard(CardInstance value, int qty)
        {
            if (value == null)
            {
                return;
            }
            foreach (var ci in CardsCollection.Where(ci => ci.BorderBrush != null))
            {
                ci.BorderBrush = null;
            }
            var card = CardsCollection.Where(ci => ci.CardId == value.CardId).FirstOrDefault();
            if (card != null)
            {
                card.Quantity += qty; //if already in deck, inc qty
                card.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 100, 15));
            }
            else
            {
                value.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 100, 15));
                value.Quantity = qty;
                CardsCollection.Add(value);
            }
            NewCard = null;
        }
    }
}
