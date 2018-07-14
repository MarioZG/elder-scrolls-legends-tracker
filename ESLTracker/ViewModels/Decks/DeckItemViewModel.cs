using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckItemViewModel : ViewModelBase
    {

        private readonly DeckCalculations deckCalculations;

        public DeckItemViewModel(DeckCalculations deckCalculations)
        {
            this.deckCalculations = deckCalculations;
        }

        private Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set
            {
                SetProperty<Deck>(ref deck, value);
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        public int Victories
        {
            get
            {
                return deckCalculations.Victories(deck);
            }
        }

        public int Defeats
        {
            get
            {
                return deckCalculations.Defeats(deck);
            }
        }

        public string WinRatio
        {
            get
            {
                return deckCalculations.WinRatio(deck);
            }
        }

        public void UpdateAllBindings()
        {
            RaisePropertyChangedEvent(String.Empty);
        }
    }
}
