using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.Decks;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using TESLTracker.Utils;

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
                return cardsDatabaseFactory.GetCardsDatabase().GetCardsNames();
            }
        }

        public bool LimitCardCount { get; set; }

        public ICommand CommandAddCardToDeck
        {
            get
            {
                return new RelayCommand(CommandAddCardToDeckExecute);
            }
        }

        public ICommand CommandCardClicked
        {
            get
            {
                return new RelayCommand(CommandCardClickedExecute);
            }
        }

        private readonly ICardsDatabaseFactory cardsDatabaseFactory;
        private readonly IDeckService deckService;
        private readonly IMessenger messanger;
        private readonly DeckCardsEditor deckCardsEditor;

        public CardListEditorViewModel(ICardsDatabaseFactory cardsDatabaseFactory, IDeckService deckService, IMessenger messanger, DeckCardsEditor deckCardsEditor)
        {
            this.cardsDatabaseFactory = cardsDatabaseFactory;
            this.deckService = deckService;
            this.messanger = messanger;
            this.deckCardsEditor = deckCardsEditor;

            messanger.Register<CardsDbReloaded>(this, OnCardsDbReloaded);
        }

        private void OnCardsDbReloaded(CardsDbReloaded obj)
        {
            RaisePropertyChangedEvent(nameof(CardNamesList));
        }

        private void CommandAddCardToDeckExecute(object obj)
        {
            int qty = Int32.Parse(obj.ToString());
            AddCard(newCard, qty);
        }

        internal void AddCard(CardInstance value, int qty)
        {
            if (value != null)
            {
                deckCardsEditor.ChangeCardQuantity(CardsCollection, value.Card, qty, LimitCardCount);
                NewCard = null;
            }
        }



        private void CommandCardClickedExecute(object param)
        {
            CardInstance cardInstance = param as CardInstance;
            if (cardInstance == null)
            {
                return;
            }

            RemoveCard(cardInstance);

        }

        private void RemoveCard(CardInstance cardInstance)
        {
            if (cardInstance != null)
            {
                deckCardsEditor.ChangeCardQuantity(CardsCollection, cardInstance.Card, -1, false);
            }
        }
    }
}
