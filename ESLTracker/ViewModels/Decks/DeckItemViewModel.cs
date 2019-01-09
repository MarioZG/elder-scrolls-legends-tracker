using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Games;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckItemViewModel : ViewModelBase
    {
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

        public RelayCommand CommandDrop { get; private set; }

        private readonly DeckCalculations deckCalculations;
        private readonly ChangeGameDeck changeGameDeck;
        private readonly IMessenger messanger;
        private readonly IFileSaver fileSaver;
        private readonly ITracker tracker;
        private readonly ISettings settings;

        public DeckItemViewModel(
            DeckCalculations deckCalculations,
            ChangeGameDeck changeGameDeck, 
            IMessenger messanger,
            IFileSaver fileSaver,
            ITracker tracker,
            ISettings settings)
        {
            this.deckCalculations = deckCalculations;
            this.changeGameDeck = changeGameDeck;
            this.messanger = messanger;
            this.fileSaver = fileSaver;
            this.tracker = tracker;
            this.settings = settings;

            CommandDrop = new RelayCommand(CommandDropExecute);

            //need to keep this for refreshing attributes icons - until class have correct binding!
            messanger.Register<EditDeck>(this, EditDeckEvent, EditDeck.Context.EditFinished);
            messanger.Register<EditDeck>(this, EditDeckEvent, EditDeck.Context.StatsUpdated);

        }

        private void CommandDropExecute(object obj)
        {
            var data = obj as IDataObject;
            TESLTracker.DataModel.Game game = data.GetData(typeof(TESLTracker.DataModel.Game)) as TESLTracker.DataModel.Game;
            var prevDeck = game.Deck;
            changeGameDeck.MoveGameBetweenDecks(game, Deck, Deck.SelectedVersion);
            messanger.Send(new EditDeck() { Deck = game.Deck }, EditDeck.Context.StatsUpdated);
            messanger.Send(new EditDeck() { Deck = prevDeck }, EditDeck.Context.StatsUpdated);
            fileSaver.SaveDatabase(tracker);
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

        public IEnumerable<bool> LastGamesOutcomes
        {
            get
            {
                return deckCalculations.GetLastGames(Deck, settings.DeckViewLastGamesIndicatorCount);
            }
        }



        public void UpdateAllBindings()
        {
            RaisePropertyChangedEvent(String.Empty);
        }

        private void EditDeckEvent(EditDeck obj)
        {
            if (obj.Deck.DeckId == Deck?.DeckId)
            {
                UpdateAllBindings();
            }
        }
    }
}
